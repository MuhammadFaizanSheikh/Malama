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

                //var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    if (user.TwoFactorEnabled)
                    {
                        var token = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);

                        await _emailSender.SendEmailAsync(user.Email, "Your 2FA Code", $"Your verification code is: {token}");

                        // Store userId in TempData to use in the verification step
                        TempData["UserIdFor2FA"] = user.Id;
                        return RedirectToAction("Verify2FA");
                    }

                    // Standard sign-in without 2FA
                    await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);
                    return RedirectToAction("Index", "Dashboard");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong.");
            }

            return View(model);
            //if (result.Succeeded)
            //{
            //    var user = await _userManager.FindByEmailAsync(model.Email);
            //    return RedirectToAction("Index", "Dashboard");
            //}
            //else if (result.IsLockedOut)
            //{
            //    ModelState.AddModelError(string.Empty, "Your account is locked. Please try again later.");
            //}
            //else if (result.IsNotAllowed)
            //{
            //    ModelState.AddModelError(string.Empty, "You are not allowed to login at this time.");
            //}
            //else
            //{
            //    ModelState.AddModelError(string.Empty, "Invalid username or password.");
            //}

            //return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}