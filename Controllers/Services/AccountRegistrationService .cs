using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using ExcelFilesCompiler.Utilities;
using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class AccountRegistrationService : IAccountRegistrationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserEventMappingService _userEventMappingService;
        private readonly ILogger<AccountRegistrationService> _logger;
        private const string CLASSNAME = "AccountRegistrationService";

        public AccountRegistrationService(ILogger<AccountRegistrationService> logger, IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager, IUserEventMappingService userEventMappingService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _userEventMappingService = userEventMappingService;
            _logger = logger;
        }

        public async Task<ResponseDto> GetRegisterRolesAsync()
        {
            const string methodName = "GetRegisterRolesAsync";
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

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

                _logger.LogInformation("{ClassName}, {MethodName}, Roles retrieved successfully, Count: {Count}",
                    CLASSNAME, methodName, rolesList.Count);

                return new ResponseDto
                {
                    Success = true,
                    Message = "Roles retrieved successfully.",
                    Data = rolesList
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Error occurred while fetching roles",
                    CLASSNAME, methodName);

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
            const string methodName = "RegisterUserAsync";
            _logger.LogInformation("{ClassName}, {MethodName}, Called for Email: {Email}",
                CLASSNAME, methodName, model?.Email);

            try
            {
                if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Invalid user data received",
                        CLASSNAME, methodName);

                    return new ResponseDto { Success = false, Message = "Invalid user data." };
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    IsActive = true,
                    TwoFactorEnabled = false,
                    IsEventUser = IsEventUser,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));

                    _logger.LogError("{ClassName}, {MethodName}, User creation failed for Email: {Email}, Errors: {Errors}",
                        CLASSNAME, methodName, model.Email, errorMessage);

                    return new ResponseDto
                    {
                        Success = false,
                        Message = errorMessage
                    };
                }

                _logger.LogInformation("{ClassName}, {MethodName}, User created successfully, UserId: {UserId}",
                    CLASSNAME, methodName, user.Id);

                try
                {
                    var allRoles = _roleManager.Roles.ToList();
                    var roleNames = allRoles
                        .Where(r => model.SelectedRoles.Contains(r.Id))
                        .Select(r => r.Name)
                        .ToList();

                    var roleResult = await _userManager.AddToRolesAsync(user, roleNames);
                    if (!roleResult.Succeeded)
                    {
                        var roleErrors = string.Join(", ", roleResult.Errors.Select(e => e.Description));

                        _logger.LogError("{ClassName}, {MethodName}, Role assignment failed for UserId: {UserId}, Errors: {Errors}",
                            CLASSNAME, methodName, user.Id, roleErrors);

                        return await RollbackUserCreationAsync(user, "Role assignment failed: " + roleErrors);
                    }

                    _logger.LogInformation("{ClassName}, {MethodName}, Roles assigned successfully to UserId: {UserId}",
                        CLASSNAME, methodName, user.Id);

                    if (model.SelectedEventIds != null && model.SelectedEventIds.Any())
                    {
                        var userEventMappings = model.SelectedEventIds.Select(eventId => new UserEventMapping
                        {
                            UserId = user.Id,
                            EventId = eventId
                        }).ToList();

                        _unitOfWork.UserEventMapping.AddRange(userEventMappings);
                        await _unitOfWork.SaveAsync();

                        _logger.LogInformation("{ClassName}, {MethodName}, Event mappings saved successfully, UserId: {UserId}, EventCount: {Count}",
                            CLASSNAME, methodName, user.Id, userEventMappings.Count);
                    }

                    _logger.LogInformation("{ClassName}, {MethodName}, User registration completed successfully, UserId: {UserId}",
                        CLASSNAME, methodName, user.Id);

                    return new ResponseDto
                    {
                        Success = true,
                        Message = "User has been created successfully.",
                        Data = new { user }
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{ClassName}, {MethodName}, Error occurred during user setup, UserId: {UserId}",
                        CLASSNAME, methodName, user.Id);

                    return await RollbackUserCreationAsync(user, "An unexpected error occurred during user setup.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Unexpected error occurred",
                    CLASSNAME, methodName);

                return new ResponseDto
                {
                    Success = false,
                    Message = "An unexpected error occurred. Please try again later."
                };
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
            const string methodName = nameof(GetUserDetailsAsync);
            _logger.LogInformation(
                "{ClassName}, {MethodName}, Fetching user details for UserId : {UserId}",
                CLASSNAME, methodName, userId
            );

            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, UserId is null or empty",
                        CLASSNAME, methodName
                    );

                    return new ResponseDto
                    {
                        Success = false,
                        Message = "User ID cannot be null or empty."
                    };
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, User not found for UserId : {UserId}",
                        CLASSNAME, methodName, userId
                    );

                    return new ResponseDto
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                var roleNames = await _userManager.GetRolesAsync(user);

                var roles = _roleManager.Roles
                    .AsEnumerable()
                    .Where(r => roleNames.Contains(r.Name))
                    .Select(r => new { r.Id, r.Name })
                    .ToList();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Retrieved {RoleCount} roles for UserId : {UserId}",
                    CLASSNAME, methodName, roles.Count, userId
                );

                var eventIds = await _userEventMappingService.GetEventsAgainstUserId(userId);

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Retrieved {EventCount} events for UserId : {UserId}",
                    CLASSNAME, methodName, eventIds.Count, userId
                );

                var userDto = new
                {
                    Email = user.Email,
                    Roles = roles,
                    EventIds = eventIds
                };

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Successfully fetched user details for UserId : {UserId}",
                    CLASSNAME, methodName, userId
                );

                return new ResponseDto
                {
                    Success = true,
                    Message = "User details retrieved successfully.",
                    Data = userDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Error occurred while fetching user details for UserId : {UserId}",
                    CLASSNAME, methodName, userId
                );

                return new ResponseDto
                {
                    Success = false,
                    Message = "An error occurred while fetching user details."
                };
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

        //public async Task<ResponseDto> UpdateUserAsync(UserUpdateDto updatedUser)
        //{
        //    try
        //    {
        //        var user = await _userManager.FindByIdAsync(updatedUser.Id);
        //        if (user == null)
        //        {
        //            return new ResponseDto { Success = false, Message = "User not found." };
        //        }

        //        //var originalEmail = user.Email;
        //        //var originalPasswordHash = user.PasswordHash;
        //        var originalRoles = await _userManager.GetRolesAsync(user);

        //        user.Email = updatedUser.Email;
        //        user.UserName = updatedUser.Email;

        //        if (!string.IsNullOrEmpty(updatedUser.Password))
        //        {
        //            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, updatedUser.Password);
        //        }

        //        var updateResult = await _userManager.UpdateAsync(user);
        //        if (!updateResult.Succeeded)
        //        {
        //            return new ResponseDto
        //            {
        //                Success = false,
        //                Message = "Failed to update user details.",
        //                Data = updateResult.Errors.Select(e => e.Description).ToList()
        //            };
        //        }

        //        var newRoleNames = new List<string>();

        //        try
        //        {
        //            var allRoles = await _roleManager.Roles.ToListAsync();
        //            var roleIds = updatedUser.SelectedRoles;
        //            newRoleNames = allRoles
        //                .Where(r => roleIds.Contains(r.Id))
        //                .Select(r => r.Name)
        //                .ToList();
        //        }
        //        catch (Exception ex)
        //        {
        //            return new ResponseDto
        //            {
        //                Success = false,
        //                Message = "Failed to fetch roles.",
        //                Data = ex.Message
        //            };
        //        }

        //        var rolesToRemove = originalRoles.Except(newRoleNames).ToList();
        //        if (rolesToRemove.Any())
        //        {
        //            try
        //            {
        //                var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
        //                if (!removeResult.Succeeded)
        //                {
        //                    throw new Exception("Failed to remove roles: " +
        //                        string.Join(", ", removeResult.Errors.Select(e => e.Description)));
        //                }
        //            }
        //            catch (Exception removeEx)
        //            {
        //                return new ResponseDto
        //                {
        //                    Success = false,
        //                    Message = "User update failed due to role removal issue. Changes rolled back.",
        //                    Data = removeEx.Message
        //                };
        //            }
        //        }

        //        var rolesToAdd = newRoleNames.Except(originalRoles).ToList();
        //        if (rolesToAdd.Any())
        //        {
        //            try
        //            {
        //                var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
        //                if (!addResult.Succeeded)
        //                {
        //                    throw new Exception("Failed to add roles: " +
        //                        string.Join(", ", addResult.Errors.Select(e => e.Description)));
        //                }
        //            }
        //            catch (Exception addEx)
        //            {
        //                await _userManager.AddToRolesAsync(user, rolesToRemove);

        //                return new ResponseDto
        //                {
        //                    Success = false,
        //                    Message = "User update failed due to role addition issue. Changes rolled back.",
        //                    Data = addEx.Message
        //                };
        //            }
        //        }

        //        return new ResponseDto
        //        {
        //            Success = true,
        //            Message = "User updated successfully.",
        //            Data = updatedUser.SelectedRoles
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseDto
        //        {
        //            Success = false,
        //            Message = "An internal server error occurred.",
        //            Data = ex.Message
        //        };
        //    }
        //}

        public async Task<ResponseDto> UpdateUserAsync(UserUpdateDto updatedUser)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(updatedUser.Id);
                if (user == null)
                {
                    return new ResponseDto { Success = false, Message = "User not found." };
                }

                // Get original roles
                var originalRoles = await _userManager.GetRolesAsync(user);

                // Update basic user info
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

                // Map SelectedRoles (role IDs) to role names
                var allRoles = await _roleManager.Roles.ToListAsync();
                var newRoleNames = allRoles
                    .Where(r => updatedUser.SelectedRoles.Contains(r.Id))
                    .Select(r => r.Name)
                    .ToList();

                // Remove roles that are no longer selected
                var rolesToRemove = originalRoles.Except(newRoleNames).ToList();
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

                // Add new roles
                var rolesToAdd = newRoleNames.Except(originalRoles).ToList();
                if (rolesToAdd.Any())
                {
                    var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                    if (!addResult.Succeeded)
                    {
                        // Rollback role removal
                        await _userManager.AddToRolesAsync(user, rolesToRemove);

                        return new ResponseDto
                        {
                            Success = false,
                            Message = "Failed to add roles. Changes rolled back.",
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
