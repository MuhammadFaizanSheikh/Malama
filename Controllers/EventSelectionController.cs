using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using Malama.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace ExcelFilesCompiler.Controllers
{
    public class EventSelectionController : Controller
    {
        private readonly IEventManagementService _eventManagementService;
        private readonly IEventStaffService _eventStaffService;
        private readonly IUserEventMappingService _userEventMappingService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<EventSelectionController> _logger;
        private const string CLASSNAME = "EventSelectionController";


        public EventSelectionController(ILogger<EventSelectionController> logger, IUserEventMappingService userEventMappingService, IEventManagementService eventManagementService, IEventStaffService eventStaffService, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _eventManagementService = eventManagementService;
            _eventStaffService = eventStaffService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _userEventMappingService = userEventMappingService;
        }


        public async Task<IActionResult> Index()
        {
            try
            {
                var events = await _eventManagementService.GetAllEventID();

                if (events == null && !events.Any()) // Ensure there is data
                {
                    ViewBag.ErrorMessage = "No events found for selection.";
                    return View(new List<EventViewModel>()); // Return an empty list instead of JSON
                }

                var eventViewModels = events.Select(e => new EventViewModel
                {
                    EventId = e.Id,       // Assuming `Id` is the primary key in your data
                    EventName = e.EventID // Assuming `EventID` is the event name or identifier
                }).ToList();

                return View(eventViewModels);
            }
            catch (Exception ex)
            {
                // Log the error if logging is implemented
                ViewBag.ErrorMessage = "An error occurred while fetching events.";
                return View(new List<EventViewModel>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignEventRole(long selectedEventId)
        {
            const string methodName = "AssignEventRole";
            _logger.LogInformation("{ClassName}, {MethodName}, Called. SelectedEventId: {EventId}",
                CLASSNAME, methodName, selectedEventId);

            try
            {
                HttpContext.Session.Remove("GlobalEventId");

                var user = await GetLoggedInUserAsync();
                if (user == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, User not found in context", CLASSNAME, methodName);
                    return Unauthorized();
                }

                if (selectedEventId == 0)
                {
                    _logger.LogInformation("{ClassName}, {MethodName}, No event selected. Clearing claims.", CLASSNAME, methodName);
                    await ClearEventClaimsAsync(user);
                    return RedirectToAction("Index", "Dashboard");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, UserId claim missing", CLASSNAME, methodName);
                    ViewBag.ErrorMessage = "User not found. Please log in again.";
                    return await ReturnIndexView();
                }

                var roles = await _userManager.GetRolesAsync(user);
                bool isEventManager = IsEventManager(roles);

                if (isEventManager)
                {
                    return await HandleEventManagerSelectionAsync(user, userId, selectedEventId);
                }

                return await HandleEventStaffSelectionAsync(user, userId, selectedEventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, An unexpected error occurred.", CLASSNAME, methodName);

                // Show inner exception message if it's a known, user-friendly exception
                if (ex is KeyNotFoundException || ex is ApplicationException)
                {
                    ViewBag.ErrorMessage = ex.Message;
                }
                else
                {
                    // For other unknown exceptions, fallback to generic message
                    ViewBag.ErrorMessage = "An unexpected error occurred. Please try again later.";
                }

                return await ReturnIndexView();
            }

        }


        // Helper method to return the index view
        private async Task<IActionResult> ReturnIndexView()
        {
            var events = await _eventManagementService.GetAllEventID();
            var eventViewModels = events.Select(e => new EventViewModel
            {
                EventId = e.Id,
                EventName = e.EventID
            }).ToList();
            return View("Index", eventViewModels);
        }

        private async Task<ApplicationUser?> GetLoggedInUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }


        private async Task ClearEventClaimsAsync(ApplicationUser user)
        {
            await UpdateUserClaimsAsync(
                user,
                selectedEventId: 0,
                eventRoleNames: Enumerable.Empty<string>(),
                eventID: "",
                0,
                isEventAssignedToStaff: false,
                staffAttributes: new List<string>()
            );
        }

        private static bool IsEventManager(IList<string> roles)
        {
            return roles.Any(r => string.Equals(r, "Event Manager", StringComparison.OrdinalIgnoreCase));
        }

        private async Task<IActionResult> HandleEventManagerSelectionAsync(ApplicationUser user, string userId, long selectedEventId)
        {
            const string methodName = "HandleEventManagerSelection";
            _logger.LogInformation("{ClassName}, {MethodName}, Processing Event Manager flow. EventId: {EventId}",
            CLASSNAME, methodName, selectedEventId);

            bool isAssigned = await _userEventMappingService.IsUserAssignedToEventAsync(userId, selectedEventId);

            if (!isAssigned)
            {
                _logger.LogWarning("{ClassName}, {MethodName}, Event not assigned to Event Manager. EventId: {EventId}",
                    CLASSNAME, methodName, selectedEventId);

                ViewBag.ErrorMessage = "The selected event is not assigned.";
                return await ReturnIndexView();
            }

            var eventManagement = await _eventManagementService
                .GetEventManagementForEventSelectionByIdWithoutInclude(selectedEventId);

            await UpdateUserClaimsAsync(user, selectedEventId, eventManagement.EventID, eventManagement.EventVersion, true);
            HttpContext.Session.SetString("GlobalEventId", eventManagement.EventID);

            _logger.LogInformation("{ClassName}, {MethodName}, Event Manager claims updated successfully",
                CLASSNAME, methodName);

            return RedirectToAction("Index", "Dashboard");
        }


        private async Task<IActionResult> HandleEventStaffSelectionAsync(ApplicationUser user, string userId, long selectedEventId)
        {
            const string methodName = "HandleEventStaffSelection";
            _logger.LogInformation("{ClassName}, {MethodName}, Processing Event Staff flow. EventId: {EventId}",
                CLASSNAME, methodName, selectedEventId);

            var eventManagement = await _eventManagementService
                .GetEventManagementForEventSelectionById(selectedEventId);

            var eventStaff = await _eventStaffService
                .GetEventStaffWithAttributesByUserId(userId);

            ValidateEventDate(eventManagement);

            var (isAssigned, roleNames, staffAttributes) =
                BuildEventStaffClaims(eventManagement, eventStaff);

            bool updated = await UpdateUserClaimsAsync(
                user,
                selectedEventId,
                roleNames,
                eventManagement.EventID,
                eventManagement.EventVersion,
                isAssigned,
                staffAttributes
            );

            if (!updated)
            {
                _logger.LogError("{ClassName}, {MethodName}, Failed to update staff claims",
                    CLASSNAME, methodName);

                ViewBag.ErrorMessage = "Failed to update user claims.";
                return await ReturnIndexView();
            }

            HttpContext.Session.SetString("GlobalEventId", eventManagement.EventID);

            _logger.LogInformation("{ClassName}, {MethodName}, Event Staff claims updated successfully",
                CLASSNAME, methodName);

            return RedirectToAction("Index", "Dashboard");
        }

        private void ValidateEventDate(EventManagement eventManagement)
        {
            const string methodName = nameof(ValidateEventDate);

            try
            {
                if (eventManagement == null)
                {
                    _logger.LogError("{ClassName}, {MethodName}, Event data is null.", CLASSNAME, methodName);
                    throw new ApplicationException("Invalid event data.");
                }

                var allowedStartDate = eventManagement.EventStartDateUtc.AddDays(-2).Date;
                var allowedEndDate = eventManagement.EventEndDateUtc.Date;

                var nowUtc = DateTime.UtcNow.Date;

                if (nowUtc < allowedStartDate || nowUtc > allowedEndDate)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Event not active. EventID: {EventID}, TodayUTC: {TodayUTC}, AllowedStart: {AllowedStart}, EventEnd: {EventEnd}",
                        CLASSNAME, methodName, eventManagement.EventID, nowUtc, allowedStartDate, allowedEndDate);

                    throw new ApplicationException("The selected event is not active today.");
                }

                _logger.LogInformation("{ClassName}, {MethodName}, Event date validated successfully. EventID: {EventID}, TodayUTC: {TodayUTC}",
                    CLASSNAME, methodName, eventManagement.EventID, nowUtc);
            }
            catch (ApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception during event date validation. EventID: {EventID}",
                    CLASSNAME, methodName, eventManagement?.EventID);
                throw new ApplicationException("An unexpected error occurred during event date validation.");
            }
        }

        private (bool IsAssigned, List<string> RoleNames, List<string> StaffAttributes)
    BuildEventStaffClaims(EventManagement eventManagement, EventStaff eventStaff)
        {
            bool isAssigned = false;
            var roleNames = new List<string>();
            var staffAttributes = new List<string>();

            var staffDetail = eventManagement.EventStaffDetailList
                .FirstOrDefault(esd => esd.EventStaffId == eventStaff.Id);

            if (staffDetail == null)
                return (false, roleNames, staffAttributes);

            isAssigned = true;

            var roleIds = staffDetail.EventWiseStaffRoleList
                .Select(r => r.RoleId)
                .Concat(staffDetail.EventWiseStaffSecondaryRoleList.Select(r => r.RoleId))
                .Distinct()
                .ToList();

            var allRoles = _roleManager.Roles.ToList();
            roleNames = allRoles.Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToList();

            if (staffDetail.ProfileButtonAccess)
                staffAttributes.Add("CanAccessProfile");

            if (eventStaff.StaffQualification != null)
            {
                foreach (var license in eventStaff.StaffQualification)
                {
                    staffAttributes.AddRange(
                        license.StaffAttributeDetails.Select(a => a.Attribute));
                }
            }

            return (isAssigned, roleNames, staffAttributes);
        }

        private async Task<bool> UpdateUserClaimsAsync(ApplicationUser user, long selectedEventId, IEnumerable<string> eventRoleNames, string eventID, int eventVersion, bool isEventAssignedToStaff, List<string> staffAttributes)
        {
            try
            {
                // ✅ 1. Create a fresh identity for this session
                var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);

                // ✅ 2. Add core claims
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName ?? ""));
                identity.AddClaim(new Claim("EventIdLong", selectedEventId.ToString()));
                identity.AddClaim(new Claim("EventIdString", eventID.ToString()));
                identity.AddClaim(new Claim("EventVersion", eventVersion.ToString()));
                identity.AddClaim(new Claim("IsEventAssignedToStaff", isEventAssignedToStaff.ToString()));

                // ✅ 3. Add only event-specific roles as claims
                foreach (var role in eventRoleNames.Distinct())
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }

                foreach (var attribute in staffAttributes.Distinct())
                {
                    identity.AddClaim(new Claim("Attribute", attribute));
                }

                // ✅ 4. Build principal
                var principal = new ClaimsPrincipal(identity);

                // ✅ 5. Replace the existing authentication cookie with new claims
                await _signInManager.SignOutAsync();
                await _signInManager.Context.SignInAsync(
                    IdentityConstants.ApplicationScheme,
                    principal,
                    new AuthenticationProperties { IsPersistent = false }
                );

                return true;
            }
            catch (Exception ex)
            {
                // You can log ex.Message here for debugging
                return false;
            }
        }

        public async Task UpdateUserClaimsAsync(ApplicationUser user, long selectedEventId, string eventID, int eventVersion, bool isEventAssignedToStaff)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("EventIdLong", selectedEventId.ToString()),
                new Claim("EventIdString", eventID ?? string.Empty),
                new Claim("EventVersion", eventVersion.ToString()),
                new Claim("IsEventAssignedToStaff", isEventAssignedToStaff.ToString())
            };

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var identity = new ClaimsIdentity(
                claims,
                IdentityConstants.ApplicationScheme);

            var principal = new ClaimsPrincipal(identity);

            await _signInManager.SignOutAsync();
            await _signInManager.Context.SignInAsync(
                IdentityConstants.ApplicationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = false });
        }
    }
}
