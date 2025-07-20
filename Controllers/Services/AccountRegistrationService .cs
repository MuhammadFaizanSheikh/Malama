using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Models;
using ExcelFilesCompiler.Utilities;
using ExcelToCsv.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class AccountRegistrationService : IAccountRegistrationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountRegistrationService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ResponseDto> GetRegisterRolesAsync()
        {
            try
            {
                var rolesList = await Task.Run(() =>
                    _roleManager.Roles
                        .Where(r => r.Category == AppConstants.RolesCategory.BasicRoles)
                        .Select(r => new SelectListItem
                        {
                            Value = r.Id,
                            Text = r.Name
                        })
                        .ToList()
                );

                return new ResponseDto
                {
                    Success = true,
                    Message = "Roles retrieved successfully.",
                    Data = rolesList
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    Success = false,
                    Message = "An error occurred while fetching roles.",
                    Data = ex.Message
                };
            }
        }

        public async Task<ResponseDto> RegisterUserAsync(RegisterViewModel model, bool IsEventUser = false)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                {
                    return new ResponseDto { Success = false, Message = "Invalid user data." };
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    IsActive = true,
                    TwoFactorEnabled = false
                    ,
                    IsEventUser = IsEventUser
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    return new ResponseDto
                    {
                        Success = false,
                        Message = string.Join(", ", result.Errors.Select(e => e.Description))
                    };
                }

                try
                {
                    var allRoles = _roleManager.Roles.ToList(); // Fetch roles in memory
                    var roleNames = allRoles
                        .Where(r => model.SelectedRoles.Contains(r.Id))
                        .Select(r => r.Name)
                        .ToList();

                    var roleResult = await _userManager.AddToRolesAsync(user, roleNames);
                    if (!roleResult.Succeeded)
                    {
                        return await RollbackUserCreationAsync(user, "Role assignment failed: " + string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    }

                    // Confirm the user's email
                    var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationResult = await _userManager.ConfirmEmailAsync(user, emailConfirmationToken);

                    if (!confirmationResult.Succeeded)
                    {
                        return await RollbackUserCreationAsync(user, "Email confirmation failed.");
                    }

                    return new ResponseDto
                    {
                        Success = true,
                        Message = "User has been created successfully.",
                        Data = new { user }
                    };
                }
                catch (Exception)
                {
                    return await RollbackUserCreationAsync(user, "An unexpected error occurred during user setup.");
                }
            }
            catch (Exception)
            {
                return new ResponseDto { Success = false, Message = "An unexpected error occurred. Please try again later." };
            }
        }


        /// <summary>
        /// Rolls back user creation by deleting the user and returning a failure response.
        /// </summary>
        private async Task<ResponseDto> RollbackUserCreationAsync(ApplicationUser user, string errorMessage)
        {
            await _userManager.DeleteAsync(user);
            return new ResponseDto { Success = false, Message = errorMessage };
        }




        public async Task<ResponseDto> GetUsersAsync()
        {
            try
            {
                var users = _userManager.Users
                    .Where(u => u.IsActive && !u.IsEventUser) // Filtering users
                    .Select(u => new { id = u.Id, email = u.Email })
                    .ToList();

                return new ResponseDto
                {
                    Success = true,
                    Message = "Users retrieved successfully.",
                    Data = users
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    Success = false,
                    Message = "An error occurred while fetching users."
                };
            }
        }

        public async Task<ResponseDto> GetUserDetailsAsync(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return new ResponseDto { Success = false, Message = "User ID cannot be null or empty." };

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return new ResponseDto { Success = false, Message = "User not found." };

                var roleNames = await _userManager.GetRolesAsync(user);

                var roles = _roleManager.Roles
                    .AsEnumerable()
                    .Where(r => roleNames.Contains(r.Name))
                    .Select(r => new { r.Id, r.Name })
                    .ToList();

                var userDto = new
                {
                    Email = user.Email,
                    Roles = roles
                };

                return new ResponseDto
                {
                    Success = true,
                    Message = "User details retrieved successfully.",
                    Data = userDto
                };
            }
            catch (Exception)
            {
                return new ResponseDto { Success = false, Message = "An error occurred while fetching user details." };
            }
        }

        public async Task<ResponseDto> DeleteUserAsync(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return new ResponseDto { Success = false, Message = "User ID is required." };
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new ResponseDto { Success = false, Message = "User not found." };
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return new ResponseDto { Success = true, Message = "User has been deleted successfully." };
                }
                else
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    return new ResponseDto { Success = false, Message = "Failed to delete user. " + errors };
                }
            }
            catch (Exception)
            {
                return new ResponseDto { Success = false, Message = "An unexpected error occurred while deleting the user." };
            }
        }

        public async Task<ResponseDto> UpdateUserAsync(UserUpdateDto updatedUser)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(updatedUser.Id);
                if (user == null)
                {
                    return new ResponseDto { Success = false, Message = "User not found." };
                }

                var originalEmail = user.Email;
                var originalPasswordHash = user.PasswordHash;
                var originalRoles = await _userManager.GetRolesAsync(user);

                user.Email = updatedUser.Email;
                user.UserName = updatedUser.Email;

                if (!string.IsNullOrEmpty(updatedUser.Password))
                {
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, updatedUser.Password);
                }

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    return new ResponseDto
                    {
                        Success = false,
                        Message = "Failed to update user details.",
                        Data = updateResult.Errors.Select(e => e.Description).ToList()
                    };
                }

                var newRoleNames = new List<string>();

                try
                {
                    var allRoles = await _roleManager.Roles.ToListAsync();
                    var roleIds = updatedUser.SelectedRoles;
                    newRoleNames = allRoles
                        .Where(r => roleIds.Contains(r.Id) && r.Category == AppConstants.RolesCategory.EventStaffRoles)
                        .Select(r => r.Name)
                        .ToList();
                }
                catch (Exception ex)
                {
                    return new ResponseDto
                    {
                        Success = false,
                        Message = "Failed to fetch roles.",
                        Data = ex.Message
                    };
                }

                var rolesToRemove = originalRoles.Except(newRoleNames).ToList();
                if (rolesToRemove.Any())
                {
                    try
                    {
                        var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                        if (!removeResult.Succeeded)
                        {
                            throw new Exception("Failed to remove roles: " +
                                string.Join(", ", removeResult.Errors.Select(e => e.Description)));
                        }
                    }
                    catch (Exception removeEx)
                    {
                        return new ResponseDto
                        {
                            Success = false,
                            Message = "User update failed due to role removal issue. Changes rolled back.",
                            Data = removeEx.Message
                        };
                    }
                }

                var rolesToAdd = newRoleNames.Except(originalRoles).ToList();
                if (rolesToAdd.Any())
                {
                    try
                    {
                        var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                        if (!addResult.Succeeded)
                        {
                            throw new Exception("Failed to add roles: " +
                                string.Join(", ", addResult.Errors.Select(e => e.Description)));
                        }
                    }
                    catch (Exception addEx)
                    {
                        await _userManager.AddToRolesAsync(user, rolesToRemove);

                        return new ResponseDto
                        {
                            Success = false,
                            Message = "User update failed due to role addition issue. Changes rolled back.",
                            Data = addEx.Message
                        };
                    }
                }

                return new ResponseDto
                {
                    Success = true,
                    Message = "User updated successfully.",
                    Data = updatedUser.SelectedRoles
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    Success = false,
                    Message = "An internal server error occurred.",
                    Data = ex.Message
                };
            }
        }

    }
}
