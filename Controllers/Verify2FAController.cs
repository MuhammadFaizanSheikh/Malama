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
                var userId = TempData["UserIdFor2FA"]?.ToString();
                if (userId == null)
                {
                    return RedirectToAction("Login");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                var result = await _signInManager.TwoFactorSignInAsync(TokenOptions.DefaultEmailProvider, code, isPersistent: false, rememberClient: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid verification code.");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Something went wrong!");
                return View();
            }
        }
    }
}
