using Malama.Models;
using Microsoft.AspNetCore.Authorization;
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
        private readonly ILogger<AccountController> _logger;
        private const string CLASSNAME = "AccountController";

        public AccountController(ILogger<AccountController> logger, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            const string methodName = "Login";

            _logger.LogInformation("{ClassName}, {MethodName}, Loading Login page",
                CLASSNAME, methodName);

            return View();
        }


        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            const string methodName = "AccessDenied";

            _logger.LogInformation("{ClassName}, {MethodName}, Access denied page requested",
                CLASSNAME, methodName);

            return View();
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CheckUserExists(string email, string userId)
        {
            const string methodName = "CheckUserExists";

            try
            {
                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Checking if user exists. Email: {Email}, UserId: {UserId}",
                    CLASSNAME, methodName, email, userId);

                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, Email is missing",
                        CLASSNAME, methodName);

                    return BadRequest(new { message = "Email is required." });
                }

                var user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, User found with Email: {Email}, ExistingUserId: {ExistingUserId}",
                        CLASSNAME, methodName, email, user.Id);

                    if (user.Id != userId)
                    {
                        _logger.LogInformation(
                            "{ClassName}, {MethodName}, User already exists and is different from provided userId",
                            CLASSNAME, methodName);

                        return Json(new { exists = true, message = "User already exists." });
                    }
                }

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, User does not exist or matches the provided userId",
                    CLASSNAME, methodName);

                return Json(new { exists = false });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Error while checking user. Email: {Email}, UserId: {UserId}",
                    CLASSNAME, methodName, email, userId);

                return StatusCode(500, new { message = "An error occurred while checking the user." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            const string methodName = "Login (POST)";

            _logger.LogInformation(
                "{ClassName}, {MethodName}, Login attempt started for Email: {Email}",
                CLASSNAME, methodName, model?.Email);

            try
            {
                // Remove eventID session if previous staff logged in on same browser
                HttpContext.Session.Remove("GlobalEventId");
                HttpContext.Session.Remove("GlobalEventVersion");
                HttpContext.Session.Remove("GlobalEventIdAndVersion");
                HttpContext.Session.Remove("GlobalEventIdLong");

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Removed Global sessions",
                    CLASSNAME, methodName);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, ModelState invalid for Email: {Email}",
                        CLASSNAME, methodName, model?.Email);

                    return View(model);
                }

                // Attempt login
                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Attempting PasswordSignInAsync for Email: {Email}",
                    CLASSNAME, methodName, model.Email);

                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                // Successful login
                if (result.Succeeded)
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Login succeeded for Email: {Email}",
                        CLASSNAME, methodName, model.Email);

                    // Check for event user
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    if (user == null)
                    {
                        _logger.LogWarning(
                            "{ClassName}, {MethodName}, Login succeeded but user not found. Email: {Email}",
                            CLASSNAME, methodName, model.Email);

                        ModelState.AddModelError(string.Empty, "Invalid user account.");
                        return View(model);
                    }

                    var roles = await _userManager.GetRolesAsync(user);

                    // Use ordinal, case-insensitive comparison
                    bool isEventManager = roles.Any(r =>
                        string.Equals(r, "Event Manager", StringComparison.OrdinalIgnoreCase));

                    if (user.IsEventUser || isEventManager)
                    {
                        _logger.LogInformation(
                            "{ClassName}, {MethodName}, Event user detected. Redirecting to EventSelection. Email: {Email}",
                            CLASSNAME, methodName, model.Email);

                        return RedirectToAction("Index", "EventSelection");
                    }


                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Redirecting to Dashboard for Email: {Email}",
                        CLASSNAME, methodName, model.Email);

                    return RedirectToAction("Index", "Dashboard");
                }
                // Requires 2FA
                else if (result.RequiresTwoFactor)
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, 2FA Required for Email: {Email}",
                        CLASSNAME, methodName, model.Email);

                    var user = await _userManager.FindByEmailAsync(model.Email);

                    if (user == null)
                    {
                        _logger.LogWarning(
                            "{ClassName}, {MethodName}, User not found for 2FA. Email: {Email}",
                            CLASSNAME, methodName, model.Email);

                        ModelState.AddModelError("", "User not found.");
                        return View(model);
                    }

                    if (!user.IsActive)
                    {
                        _logger.LogWarning(
                            "{ClassName}, {MethodName}, Inactive user for 2FA. Email: {Email}",
                            CLASSNAME, methodName, model.Email);

                        ModelState.AddModelError("", "User is inactive.");
                        return View(model);
                    }

                    var token = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);

                    if (string.IsNullOrEmpty(token))
                    {
                        _logger.LogError(
                            "{ClassName}, {MethodName}, Failed to generate 2FA token. Email: {Email}",
                            CLASSNAME, methodName, model.Email);

                        ModelState.AddModelError("", "Failed to generate 2FA token.");
                        return View(model);
                    }

                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Sending 2FA email token to: {Email}",
                        CLASSNAME, methodName, model.Email);

                    await _emailSender.SendEmailAsync(user.Email, "Your 2FA Code", $"Your verification code is: {token}");

                    return RedirectToAction("Verify2FA", "Verify2FA");
                }
                // Account locked out
                else if (result.IsLockedOut)
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, Account locked out. Email: {Email}",
                        CLASSNAME, methodName, model.Email);

                    ModelState.AddModelError("", "Account is locked.");
                    return View(model);
                }

                // Invalid login attempt
                _logger.LogWarning(
                    "{ClassName}, {MethodName}, Invalid login attempt. Email: {Email}",
                    CLASSNAME, methodName, model.Email);

                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Exception occurred during login. Email: {Email}",
                    CLASSNAME, methodName, model?.Email);

                ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            const string methodName = "Logout";

            try
            {
                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Logout request initiated",
                    CLASSNAME, methodName);

                await _signInManager.SignOutAsync();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, User successfully logged out",
                    CLASSNAME, methodName);

                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Error occurred during logout",
                    CLASSNAME, methodName);

                // Optional: show message or redirect
                TempData["ErrorMessage"] = "An error occurred while logging out, please try again.";

                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            const string methodName = "ForgotPassword";

            _logger.LogInformation(
                "{ClassName}, {MethodName}, Forgot password request initiated. Email: {Email}",
                CLASSNAME, methodName, email);

            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, Email was empty in forgot password request",
                        CLASSNAME, methodName);

                    ViewBag.ErrorMessage = "Please enter your email to request a password reset.";
                    return View("Login");
                }

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Searching for user by email: {Email}",
                    CLASSNAME, methodName, email);

                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, No user found for email: {Email}. Sending generic response.",
                        CLASSNAME, methodName, email);

                    ViewBag.Message = "If your email is registered, you will receive a reset link shortly.";
                    return View("Login");
                }

                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, Email not confirmed for user: {Email}. Sending generic response.",
                        CLASSNAME, methodName, email);

                    ViewBag.Message = "If your email is registered, you will receive a reset link shortly.";
                    return View("Login");
                }

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, User found. Generating reset token for Email: {Email}",
                    CLASSNAME, methodName, email);

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var resetLink = Url.Action("ResetPassword", "Account",
                    new { token, email = user.Email },
                    Request.Scheme);

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Sending password reset email to: {Email}",
                    CLASSNAME, methodName, email);

                await _emailSender.SendEmailAsync(email, "Change password", resetLink);

                ViewBag.Message = "A reset link has been sent to your email address.";

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Reset link sent successfully to Email: {Email}",
                    CLASSNAME, methodName, email);

                return View("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Error occurred while processing forgot password request for Email: {Email}",
                    CLASSNAME, methodName, email);

                ViewBag.ErrorMessage = "An unexpected error occurred. Please try again later.";
                return View("Login");
            }
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            const string methodName = "ResetPassword (GET)";

            _logger.LogInformation(
                "{ClassName}, {MethodName}, Reset password request received. Email: {Email}, TokenProvided: {TokenProvided}",
                CLASSNAME, methodName, email, !string.IsNullOrEmpty(token));

            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, Invalid reset password request. Token or Email is missing.",
                        CLASSNAME, methodName);

                    ModelState.AddModelError(string.Empty, "Invalid password reset token or email.");
                    return RedirectToAction("Login");
                }

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Token and Email valid. Preparing ResetPasswordViewModel for Email: {Email}",
                    CLASSNAME, methodName, email);

                var model = new ResetPasswordViewModel
                {
                    Token = token,
                    Email = email
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Exception occurred while loading ResetPassword view. Email: {Email}",
                    CLASSNAME, methodName, email);

                ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            const string methodName = "ResetPassword (POST)";

            _logger.LogInformation(
                "{ClassName}, {MethodName}, Reset password request submitted for Email: {Email}",
                CLASSNAME, methodName, model?.Email);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "{ClassName}, {MethodName}, ModelState invalid for Email: {Email}",
                    CLASSNAME, methodName, model?.Email);

                return View("ResetPassword", model);
            }

            try
            {
                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Looking up user by email: {Email}",
                    CLASSNAME, methodName, model.Email);

                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, No user found for Email: {Email}. Returning success message for security.",
                        CLASSNAME, methodName, model.Email);

                    ViewBag.Message = "Your password has been reset successfully.";
                    return View("Login");
                }

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Attempting password reset for Email: {Email}",
                    CLASSNAME, methodName, model.Email);

                var resetPassResult = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

                if (!resetPassResult.Succeeded)
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, Password reset failed for Email: {Email}. Errors: {Errors}",
                        CLASSNAME, methodName, model.Email,
                        string.Join("; ", resetPassResult.Errors.Select(e => e.Description)));

                    foreach (var error in resetPassResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View("ResetPassword", model);
                }

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Password reset successful for Email: {Email}",
                    CLASSNAME, methodName, model.Email);

                ViewBag.Message = "Your password has been reset successfully.";
                return View("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Unexpected error during password reset for Email: {Email}",
                    CLASSNAME, methodName, model?.Email);

                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                return View("ResetPassword", model);
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            const string methodName = "ChangePassword (GET)";

            _logger.LogInformation(
                "{ClassName}, {MethodName}, Change password page requested by current user",
                CLASSNAME, methodName);

            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            const string methodName = "ChangePassword (POST)";

            _logger.LogInformation(
                "{ClassName}, {MethodName}, Change password request initiated by user: {UserName}",
                CLASSNAME, methodName, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "{ClassName}, {MethodName}, ModelState invalid for user: {UserName}",
                    CLASSNAME, methodName, User.Identity?.Name);

                return View(model);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, User not found for change password request. UserName: {UserName}",
                        CLASSNAME, methodName, User.Identity?.Name);

                    ViewBag.ErrorMessage = "User not found.";
                    return View(model);
                }

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Attempting password change for user: {UserName}",
                    CLASSNAME, methodName, User.Identity?.Name);

                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Password changed successfully for user: {UserName}",
                        CLASSNAME, methodName, User.Identity?.Name);

                    ViewBag.SuccessMessage = "Your password has been changed successfully.";
                    return View();
                }
                else
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));

                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, Password change failed for user: {UserName}. Errors: {Errors}",
                        CLASSNAME, methodName, User.Identity?.Name, errors);

                    ViewBag.ErrorMessage = errors;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Unexpected error during password change for user: {UserName}",
                    CLASSNAME, methodName, User.Identity?.Name);

                ViewBag.ErrorMessage = "An unexpected error occurred.";
                return View(model);
            }
        }
    }
}