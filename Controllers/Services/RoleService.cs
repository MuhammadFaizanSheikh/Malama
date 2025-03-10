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

        //public async Task<ResponseDto> UpdateUserEventStaffRolesAsync(EventStaff eventStaff)
        //{
        //    var responseDto = new ResponseDto { Success = true };

        //    if (eventStaff.StaffLicense == null || !eventStaff.StaffLicense.Any())
        //    {
        //        return new ResponseDto { Success = false, Message = "No roles selected." };
        //    }

        //    try
        //    {
        //        var user = await _userManager.FindByEmailAsync(eventStaff.UserEmail);
        //        if (user == null)
        //        {
        //            return new ResponseDto { Success = false, Message = "User not found." };
        //        }

        //        // Get existing role names assigned to the user
        //        var existingRoleNames = await _userManager.GetRolesAsync(user);
        //        if (existingRoleNames == null || !existingRoleNames.Any())
        //        {
        //            return new ResponseDto { Success = false, Message = "No existing roles found for the user." };
        //        }

        //        // Fetch roles from the database
        //        var allRoles = await _roleManager.Roles.ToListAsync();

        //        // Identify event staff roles currently assigned to the user
        //        var existingRoles = allRoles
        //            .Where(r => existingRoleNames.Contains(r.Name) && r.Category == AppConstants.RolesCategory.EventStaffRoles)
        //            .ToList();

        //        if (!existingRoles.Any())
        //        {
        //            return new ResponseDto { Success = false, Message = "User has no roles under 'EventStaffRoles' category." };
        //        }

        //        var rolesToRemove = existingRoles.Select(r => r.Name).ToList();

        //        // Get new role names based on provided Role IDs
        //        var roleIds = eventStaff.StaffLicense.Select(l => l.RoleId).ToList();
        //        var newRoleNames = allRoles
        //            .Where(r => roleIds.Contains(r.Id))
        //            .Select(r => r.Name)
        //            .ToList();

        //        if (!newRoleNames.Any())
        //        {
        //            return new ResponseDto { Success = false, Message = "Invalid role IDs provided." };
        //        }

        //        // Remove existing event staff roles
        //        var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
        //        if (!removeResult.Succeeded)
        //        {
        //            return new ResponseDto { Success = false, Message = "Failed to remove existing roles." };
        //        }

        //        try
        //        {
        //            // Attempt to add new roles
        //            var addResult = await _userManager.AddToRolesAsync(user, newRoleNames);
        //            if (!addResult.Succeeded)
        //            {
        //                // Rollback: Reassign the previous roles in case of failure
        //                await _userManager.AddToRolesAsync(user, rolesToRemove);
        //                return new ResponseDto { Success = false, Message = "Failed to assign new roles. Previous roles restored." };
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            // Rollback on exception and log the error
        //            await _userManager.AddToRolesAsync(user, rolesToRemove);
        //            return new ResponseDto
        //            {
        //                Success = false,
        //                Message = "An error occurred while assigning new roles. Previous roles restored.",
        //            };
        //        }

        //        return new ResponseDto { Success = true, Message = "Roles updated successfully." };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseDto
        //        {
        //            Success = false,
        //            Message = "An unexpected error occurred. Please try again later."
        //        };
        //    }
        //}

    }

}
