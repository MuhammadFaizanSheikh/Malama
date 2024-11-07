using ExcelFilesCompiler.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExcelFilesCompiler.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AccountRegistrationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountRegistrationController(UserManager<ApplicationUser> userManager , RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
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
                        TwoFactorEnabled = true
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(model.Role))
                        {
                            var roleExists = await _roleManager.RoleExistsAsync(model.Role);
                            if (roleExists)
                            {
                                await _userManager.AddToRoleAsync(user, model.Role);
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "Role does not exist.");
                                return View(model);
                            }
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
        .Where(u => u.IsActive) // Assuming you have an IsActive property
        .Select(u => new { id = u.Id, email = u.Email })
        .ToList();

            var userss = _userManager.Users
        .Where(u => u.IsActive) // Assuming you have an IsActive property
        .ToList();

            foreach (var user in userss)
            {
                var roles = _userManager.GetRolesAsync(user).Result;

                //userList.Add(new UserWithRolesViewModel
                //{
                //    UserId = user.Id,
                //    UserName = user.UserName,
                //    Email = user.Email,
                //    Roles = roles
                //});
            }

            return Json(users);
        }

        [HttpPost]
        public async Task<IActionResult> DeactivateUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsActive = false;
                await _userManager.UpdateAsync(user);
                ViewBag.SuccessMessage = "User has been deactivated successfully";
                ModelState.Clear(); // Clear form data after successful registration
                return View();
                //return Json(new { message = "User deactivated successfully." });
            }
            
            ModelState.AddModelError(string.Empty, "User not found!");
            return View();
        }

    }
}
