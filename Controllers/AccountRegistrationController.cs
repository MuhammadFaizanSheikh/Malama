using ExcelFilesCompiler.Models;
using ExcelFilesCompiler.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExcelFilesCompiler.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AccountRegistrationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountRegistrationController(UserManager<ApplicationUser> userManager , RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            ViewData["Roles"] = await Task.Run(() =>
                _roleManager.Roles
                    .Where(r => r.Category == AooConstants.RolesCategory.BasicRoles)
                    .Select(r => r.Name) // ✅ Return only role names (List<string>)
                    .ToList()
            );

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        IsActive = true,
                        TwoFactorEnabled = true,
                        IsEventUser = false
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        if (model.SelectedRoles != null && model.SelectedRoles.Any())
                        {
                            await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Role not selected.");
                            return View(model);
                        }

                        // Confirm the user's email by sending an email confirmation token
                        var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationResult = await _userManager.ConfirmEmailAsync(user, emailConfirmationToken);

                        if (!confirmationResult.Succeeded)
                        {
                            ModelState.AddModelError("", "Email confirmation failed.");
                            return View(model);
                        }

                        ViewBag.SuccessMessage = "User has been created successfully.";
                        ModelState.Clear(); // Clear the form data after successful registration
                        return View();
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");

                return View(model);
            }
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _userManager.Users
        .Where(u => u.IsActive)
        .Select(u => new { id = u.Id, email = u.Email })
        .ToList();

            return Json(users);
        }

        public async Task<IActionResult> GetUserDetails(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Assuming password is hashed, don't send it directly to the client
            var userDto = new { user.Email, Role = await _userManager.GetRolesAsync(user) };
            return Json(userDto);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "User ID is required." });
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found." });
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return Json(new { success = true, message = "User has been deleted successfully." });
                }
                else
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    return Json(new { success = false, message = "Failed to delete user. " + errors });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred while deleting the user." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(UserUpdateDto updatedUser)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(updatedUser.Id);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                user.Email = updatedUser.Email;

                if (!string.IsNullOrEmpty(updatedUser.Password))
                {
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, updatedUser.Password);
                }

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    return BadRequest(new { message = "Failed to update user", errors = updateResult.Errors });
                }

                var existingRoles = await _userManager.GetRolesAsync(user);

                var rolesToRemove = existingRoles.Except(updatedUser.SelectedRoles).ToList();
                if (rolesToRemove.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    if (!removeResult.Succeeded)
                    {
                        return BadRequest(new { message = "Failed to remove roles", errors = removeResult.Errors });
                    }
                }

                var rolesToAdd = updatedUser.SelectedRoles.Except(existingRoles).ToList();
                if (rolesToAdd.Any())
                {
                    var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                    if (!addResult.Succeeded)
                    {
                        return BadRequest(new { message = "Failed to add roles", errors = addResult.Errors });
                    }
                }

                return Json(new { message = "User updated successfully", assignedRoles = updatedUser.SelectedRoles });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An internal server error occurred", error = ex.Message });
            }
        }



        //[HttpPost]
        //public async Task<IActionResult> DeactivateUser(string userId)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user != null)
        //    {
        //        user.IsActive = false;
        //        await _userManager.UpdateAsync(user);
        //        ViewBag.SuccessMessage = "User has been deactivated successfully";
        //        ModelState.Clear(); // Clear form data after successful registration
        //        return View();
        //        //return Json(new { message = "User deactivated successfully." });
        //    }

        //    ModelState.AddModelError(string.Empty, "User not found!");
        //    return View();
        //}

    }
}
