using ExcelFilesCompiler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExcelFilesCompiler.Controllers
{
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

                    if (user.IsActive == false)
                    {
                        ModelState.AddModelError("", "User is inactive.");
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

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    // Display an error message if no email is provided
                    ViewBag.ErrorMessage = "Please enter your email to request a password reset.";
                    return View("Login");
                }

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Send a confirmation regardless of whether the email exists for security purposes
                    ViewBag.Message = "If your email is registered, you will receive a reset link shortly.";
                    return View("Login");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, Request.Scheme);

                // Attempt to send the email with the reset link
                await _emailSender.SendEmailAsync(email, "Change password", resetLink);

                ViewBag.Message = "A reset link has been sent to your email address.";
                return View("Login");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred. Please try again later.";
                return View("Login");
            }
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
                {
                    ModelState.AddModelError(string.Empty, "Invalid password reset token or email.");
                    return RedirectToAction("Login");
                }

                var model = new ResetPasswordViewModel
                {
                    Token = token,
                    Email = email
                };
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("ResetPassword", model);
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ViewBag.Message = "Your password has been reset successfully.";
                    return View("Login");
                }

                var resetPassResult = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (!resetPassResult.Succeeded)
                {
                    foreach (var error in resetPassResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("ResetPassword", model);
                }

                ViewBag.Message = "Your password has been reset successfully.";
                return View("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                return View("ResetPassword", model);
            }
        }


        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    ViewBag.ErrorMessage = "User not found.";
                    return View(model);
                }

                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    ViewBag.SuccessMessage = "Your password has been changed successfully.";
                    return View();
                }
                else
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    ViewBag.ErrorMessage = errors;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred.";
                return View(model);
            }
        }



    }
}