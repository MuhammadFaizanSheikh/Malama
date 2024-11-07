using ExcelFilesCompiler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExcelFilesCompiler.Controllers
{
    public class Verify2FAController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public Verify2FAController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Verify2FA()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Verify2FA(string code)
        {
            try
            {
                // Retrieve the authenticated user for 2FA
                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Verify the 2FA code
                var result = await _signInManager.TwoFactorSignInAsync(TokenOptions.DefaultEmailProvider, code, isPersistent: false, rememberClient: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid verification code.");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An unexpected error occurred during verification. Please try again later.");

                return View();
            }
        }

    }
}
