using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Models;
using ExcelFilesCompiler.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

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
                        .Where(r => r.Category == AooConstants.RolesCategory.BasicRoles)
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
                    TwoFactorEnabled = true,
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

                if (model.SelectedRoles != null && model.SelectedRoles.Any())
                {
                    var allRoles = _roleManager.Roles.ToList(); // Fetch roles in memory
                    var roleNames = allRoles
                        .Where(r => model.SelectedRoles.Contains(r.Id))
                        .Select(r => r.Name)
                        .ToList();

                    await _userManager.AddToRolesAsync(user, roleNames);
                }
                else
                {
                    return new ResponseDto { Success = false, Message = "Role not selected." };
                }

                // Confirm the user's email
                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationResult = await _userManager.ConfirmEmailAsync(user, emailConfirmationToken);

                if (!confirmationResult.Succeeded)
                {
                    return new ResponseDto { Success = false, Message = "Email confirmation failed." };
                }

                return new ResponseDto
                {
                    Success = true,
                    Message = "User has been created successfully.",
                    Data = new { user.Id, user.Email } // Returning user details as Data
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto { Success = false, Message = "An unexpected error occurred. Please try again later." };
            }
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

                user.Email = updatedUser.Email;

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
                        Message = "Failed to update user.",
                        Data = updateResult.Errors.Select(e => e.Description).ToList()
                    };
                }

                var existingRoles = await _userManager.GetRolesAsync(user);

                var rolesToRemove = existingRoles.Except(updatedUser.SelectedRoles).ToList();
                if (rolesToRemove.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    if (!removeResult.Succeeded)
                    {
                        return new ResponseDto
                        {
                            Success = false,
                            Message = "Failed to remove roles.",
                            Data = removeResult.Errors.Select(e => e.Description).ToList()
                        };
                    }
                }

                var rolesToAdd = updatedUser.SelectedRoles.Except(existingRoles).ToList();
                if (rolesToAdd.Any())
                {
                    var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                    if (!addResult.Succeeded)
                    {
                        return new ResponseDto
                        {
                            Success = false,
                            Message = "Failed to add roles.",
                            Data = addResult.Errors.Select(e => e.Description).ToList()
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
