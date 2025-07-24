using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Models;
using ExcelFilesCompiler.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NPOI.SS.Formula.Functions;

namespace ExcelFilesCompiler.Controllers
{
    [Authorize(Roles = "Project Manager & Program Manager,Super Admin")]
    public class AccountRegistrationController : Controller
    {
        private readonly IAccountRegistrationService _registrationService;

        public AccountRegistrationController(IAccountRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }


        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var response = await _registrationService.GetRegisterRolesAsync();

            if (!response.Success)
            {
                return StatusCode(500, response.Message);
            }

            // Cast to List<SelectListItem>
            var roles = response.Data as List<SelectListItem>;

            if (roles == null)
            {
                return StatusCode(500, "Unexpected data format for roles.");
            }

            // Filter out "Super Admin" by Text (what is shown to the user)
            var filteredRoles = roles
                .Where(role => !string.Equals(role.Text, "Super Admin", StringComparison.OrdinalIgnoreCase))
                .ToList();

            ViewData["Roles"] = filteredRoles;

            return View();
        }




        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                //var rolesList = await Task.Run(() =>
                //     _roleManager.Roles
                //         .Where(r => r.Category == AooConstants.RolesCategory.BasicRoles)
                //         .Select(r => new SelectListItem
                //         {
                //             Value = r.Id,
                //             Text = r.Name
                //         })
                //         .ToList()
                // );

                var response = await _registrationService.GetRegisterRolesAsync();

                if (!response.Success)
                {
                    return StatusCode(500, response.Message);
                }

                ViewData["Roles"] = response.Data;

                response = await _registrationService.RegisterUserAsync(model);

                if (response.Success)
                {
                    ViewBag.SuccessMessage = response.Message;
                    ModelState.Clear();
                    return View();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                return View(model);
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> Register(RegisterViewModel model)
        //{
        //    try
        //    {
        //        var rolesList = await Task.Run(() =>
        //             _roleManager.Roles
        //                 .Where(r => r.Category == AooConstants.RolesCategory.BasicRoles)
        //                 .Select(r => new SelectListItem
        //                 {
        //                     Value = r.Id,
        //                     Text = r.Name
        //                 })
        //                 .ToList()
        //         );

        //        ViewData["Roles"] = rolesList;

        //        if (ModelState.IsValid)
        //        {
        //            var user = new ApplicationUser
        //            {
        //                UserName = model.Email,
        //                Email = model.Email,
        //                IsActive = true,
        //                TwoFactorEnabled = true,
        //                IsEventUser = false
        //            };

        //            var result = await _userManager.CreateAsync(user, model.Password);

        //            if (result.Succeeded)
        //            {
        //                if (model.SelectedRoles != null && model.SelectedRoles.Any())
        //                {
        //                    // Convert role IDs to role names
        //                    var allRoles = _roleManager.Roles.ToList(); // Fetch roles in memory
        //                    var roleNames = allRoles
        //                        .Where(r => model.SelectedRoles.Contains(r.Id)) // Now filtering in memory
        //                        .Select(r => r.Name)
        //                        .ToList();


        //                    await _userManager.AddToRolesAsync(user, roleNames);
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError(string.Empty, "Role not selected.");
        //                    return View(model);
        //                }

        //                // Confirm the user's email
        //                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //                var confirmationResult = await _userManager.ConfirmEmailAsync(user, emailConfirmationToken);

        //                if (!confirmationResult.Succeeded)
        //                {
        //                    ModelState.AddModelError("", "Email confirmation failed.");
        //                    return View(model);
        //                }

        //                ViewBag.SuccessMessage = "User has been created successfully.";
        //                ModelState.Clear(); // Clear the form data after successful registration
        //                return View();
        //            }

        //            foreach (var error in result.Errors)
        //            {
        //                ModelState.AddModelError(string.Empty, error.Description);
        //            }
        //        }
        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
        //        return View(model);
        //    }
        //}


        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var response = await _registrationService.GetUsersAsync();
            return Json(response);
        }


        public async Task<IActionResult> GetUserDetails(string userId)
        {
            var response = await _registrationService.GetUserDetailsAsync(userId);
            return Json(response);
        }



        [HttpPost]
        public async Task<IActionResult> DeleteUser([FromBody] string userId)
        {
            var response = await _registrationService.DeleteUserAsync(userId);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto updatedUser)
        {
            var response = await _registrationService.UpdateUserAsync(updatedUser);
            return Ok(response);
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

public class RoleDto
{
    public string Text { get; set; }
}

