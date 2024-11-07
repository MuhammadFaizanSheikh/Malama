using ExcelFilesCompiler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExcelFilesCompiler.Controllers
{
    //public class AccountController : Controller
    //{
    //    [HttpGet]
    //    public IActionResult Login()
    //    {
    //        return View();
    //    }

    //    [HttpPost]
    //    public IActionResult Login(LoginViewModel model)
    //    {
    //        // Hardcoded credentials
    //        string hardcodedUsername = "DAWSON";
    //        string hardcodedPassword = "123456";

    //        if (model.Username != null && model.Password != null)
    //        {
    //            if (model.Username.ToUpper() == hardcodedUsername && model.Password == hardcodedPassword)
    //            {
    //                // Redirect to Home page if login is successful
    //                return RedirectToAction("Index", "Dashboard");
    //            }
    //            else
    //            {
    //                // Return an error message if login fails
    //                ViewBag.ErrorMessage = "Invalid username or password";
    //                return View();
    //            }
    //        }
    //        else
    //        {
    //            // Return an error message if login fails
    //            ViewBag.ErrorMessage = "Invalid username or password";
    //            return View();
    //        }
    //    }
    //}
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Attempt to sign in the user
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                else if (result.RequiresTwoFactor)
                {
                    //return RedirectToAction("Index", "Dashboard");
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user == null)
                    {
                        ModelState.AddModelError("", "User not found.");
                        return View(model);
                    }

                    var token = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
                    if (string.IsNullOrEmpty(token))
                    {
                        ModelState.AddModelError("", "Failed to generate 2FA token.");
                        return View(model);
                    }

                    // Send the 2FA token via email
                    await _emailSender.SendEmailAsync(user.Email, "Your 2FA Code", $"Your verification code is: {token}");

                    return RedirectToAction("Verify2FA", "Verify2FA");
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Account is locked.");
                    return View(model);
                }

                // If the login attempt fails, show an invalid login message
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");

                return View(model);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}