using ExcelFilesCompiler.Interfaces;
using Malama.Models;
using ExcelFilesCompiler.Utilities;
using Malama.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NPOI.SS.Formula.Functions;
using Malama.Attributes;

namespace ExcelFilesCompiler.Controllers
{
    public class AccountRegistrationController : Controller
    {
        private readonly IAccountRegistrationService _registrationService;
        private readonly IEventManagementService _eventManagementService;
        private readonly ILogger<AccountRegistrationController> _logger;
        private const string CLASSNAME = "AccountRegistrationController";

        public AccountRegistrationController(ILogger<AccountRegistrationController> logger, IEventManagementService eventManagementService, IAccountRegistrationService registrationService)
        {
            _registrationService = registrationService;
            _eventManagementService = eventManagementService;
            _logger = logger;
        }


        [HttpGet]
        [RoleAttributeAuthorizeFromConfig("AccountRegistration_View")]
        public async Task<IActionResult> Register()
        {
            const string methodName = "Register";
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            try
            {
                // Fetch roles
                var response = await _registrationService.GetRegisterRolesAsync();

                if (!response.Success)
                {
                    _logger.LogError("{ClassName}, {MethodName}, Failed to get registration roles: {Message}",
                        CLASSNAME, methodName, response.Message);

                    ViewBag.ErrorMessage = "Unable to load roles. Please try again later.";
                    return View();
                }

                if (response.Data is not List<SelectListItem> roles)
                {
                    _logger.LogError("{ClassName}, {MethodName}, Unexpected data format for roles. Received type: {Type}",
                        CLASSNAME, methodName, response.Data?.GetType());

                    ViewBag.ErrorMessage = "Unexpected error occurred while loading roles.";
                    return View();
                }

                await LoadRolesAndEventsAsync((IEnumerable<SelectListItem>)response.Data, methodName);

                _logger.LogInformation("{ClassName}, {MethodName}, Returning view", CLASSNAME, methodName);
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception occurred while loading registration page", CLASSNAME, methodName);
                ViewBag.ErrorMessage = "An unexpected error occurred while loading the registration page. Please try again later.";
                return View();
            }
        }

        [HttpPost]
        [RoleAttributeAuthorizeFromConfig("AccountRegistration_Save")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            const string methodName = "Register";
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            try
            {
                var response = await _registrationService.GetRegisterRolesAsync();

                if (!response.Success)
                {
                    _logger.LogError("{ClassName}, {MethodName}, Failed to retrieve roles: {Message}", CLASSNAME, methodName, response.Message);

                    return StatusCode(500, response.Message);
                }

                await LoadRolesAndEventsAsync((IEnumerable<SelectListItem>)response.Data, methodName);

                if (!string.IsNullOrEmpty(model.Id))
                {
                    response = await _registrationService.UpdateUserRolesAndEventsAsync(model);
                }
                else
                {
                    response = await _registrationService.RegisterUserAsync(model);
                }

                if (response.Success)
                {
                    _logger.LogInformation("{ClassName}, {MethodName}, User registration/updation successful for Email: {Email}", CLASSNAME, methodName, model.Email);

                    TempData["SuccessMessage"] = response.Message;
                    ModelState.Clear();
                    return View();
                }
                else
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, User registration failed for Email: {Email}, Reason: {Reason}", CLASSNAME, methodName, model.Email, response.Message);

                    ModelState.AddModelError(string.Empty, response.Message);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";
                _logger.LogError(ex, "{ClassName}, {MethodName}, Unexpected error during user registration", CLASSNAME, methodName);

                ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                return View(model);
            }
        }

        [HttpGet]
        [RoleAttributeAuthorizeFromConfig("AccountRegistration_Save")]
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
        [RoleAttributeAuthorizeFromConfig("AccountRegistration_Save")]
        public async Task<IActionResult> DeleteUser([FromBody] string userId)
        {
            var response = await _registrationService.DeleteUserAsync(userId);
            return Json(response);
        }

        [HttpPost]
        [RoleAttributeAuthorizeFromConfig("AccountRegistration_Save")]
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

        private async Task LoadRolesAndEventsAsync(IEnumerable<SelectListItem> roles, string methodName)
        {
            // Filter out "Super Admin"
            var filteredRoles = roles
                .Where(role => !string.Equals(role.Text, "Super Admin", StringComparison.OrdinalIgnoreCase))
                .ToList();

            ViewData["Roles"] = filteredRoles;

            _logger.LogInformation("{ClassName}, {MethodName}, Successfully retrieved roles, Count: {Count}", CLASSNAME, methodName, filteredRoles.Count);

            // Fetch events
            var events = await _eventManagementService.GetAllEventID();

            if (events == null || !events.Any())
            {
                _logger.LogWarning("{ClassName}, {MethodName}, No events found while loading page.", CLASSNAME, methodName);

                ViewData["Events"] = new List<SelectListItem>();
                return;
            }

            var eventSelectList = events
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.EventID
                })
                .ToList();

            ViewData["Events"] = eventSelectList;

            _logger.LogInformation("{ClassName}, {MethodName}, Successfully retrieved events, Count: {Count}", CLASSNAME, methodName, eventSelectList.Count);
        }
    }
}

