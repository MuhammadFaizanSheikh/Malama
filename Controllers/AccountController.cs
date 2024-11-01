using ExcelFilesCompiler.Models;
using Microsoft.AspNetCore.Identity;
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
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Attempt to sign in the user
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                return RedirectToAction("Index", "Dashboard");
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Your account is locked. Please try again later.");
            }
            else if (result.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "You are not allowed to login at this time.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}