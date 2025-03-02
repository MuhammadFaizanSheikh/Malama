using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Models;
using ExcelFilesCompiler.Utilities;
using ExcelToCsv.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RoleService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        public async Task<List<ApplicationRole>> GetRolesByCategoryAsync(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                throw new ArgumentException("Category parameter is required.", nameof(category));
            }

            var roles = _roleManager.Roles
                                    .Where(r => r.Category == category)
                                    .ToList(); // ToListAsync() not needed as it's in-memory filtering

            return roles;
        }

        public async Task<ResponseDto> UpdateUserEventStaffRolesAsync(EventStaff eventStaff)
        {
            var responseDto = new ResponseDto { Success = true };

            if (eventStaff.StaffLicense != null && eventStaff.StaffLicense.Any())
            {
                var user = await _userManager.FindByEmailAsync(eventStaff.UserEmail);

                if (user == null)
                {
                    responseDto.Success = false;
                    responseDto.Message = "User not found";
                    return responseDto;
                }

                // Get existing role names assigned to the user
                var existingRoleNames = await _userManager.GetRolesAsync(user);

                if (existingRoleNames == null || !existingRoleNames.Any())
                {
                    responseDto.Success = false;
                    responseDto.Message = "No existing roles found for the user.";
                    return responseDto;
                }

                // Fetch roles that belong to the "EventStaffRoles" category and are assigned to the user
                var allRoles = await _roleManager.Roles.ToListAsync(); // Fetch all roles into memory

                var existingRoles = allRoles
                    .Where(r => existingRoleNames.Contains(r.Name) && r.Category == AppConstants.RolesCategory.EventStaffRoles)
                    .ToList();

                if (!existingRoles.Any())
                {
                    responseDto.Success = false;
                    responseDto.Message = "User has no roles under 'EventStaffRoles' category.";
                    return responseDto;
                }

                var rolesToRemove = existingRoles.Select(r => r.Name).ToList();

                if (rolesToRemove.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    if (!removeResult.Succeeded)
                    {
                        responseDto.Success = false;
                        responseDto.Message = "Failed to remove existing roles.";
                        return responseDto;
                    }
                }


                var roleIds = eventStaff.StaffLicense.Select(l => l.RoleId).ToList();

                var newRoleNames = allRoles
                        .Where(r => roleIds.Contains(r.Id))
                        .Select(r => r.Name)
                        .ToList();

                if (!newRoleNames.Any())
                {
                    responseDto.Success = false;
                    responseDto.Message = "Invalid role IDs provided.";
                    return responseDto;
                }

                var addResult = await _userManager.AddToRolesAsync(user, newRoleNames);
                if (!addResult.Succeeded)
                {
                    responseDto.Success = false;
                    responseDto.Message = "Failed to assign new roles.";
                    return responseDto;
                }
            }
            else
            {
                responseDto.Success = false;
                responseDto.Message = "No roles selected.";
                return responseDto;
            }
            return responseDto;
        }
    }

}
