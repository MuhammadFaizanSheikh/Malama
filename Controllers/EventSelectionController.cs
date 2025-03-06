using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Models;
using ExcelFilesCompiler.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ExcelFilesCompiler.Controllers
{
    public class EventSelectionController : Controller
    {
        private readonly IEventManagementService _eventManagementService;
        private readonly IEventStaffService _eventStaffService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;


        public EventSelectionController(IEventManagementService eventManagementService, IEventStaffService eventStaffService, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _eventManagementService = eventManagementService;
            _eventStaffService = eventStaffService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        public async Task<IActionResult> Index()
        {
            try
            {
                var events = await _eventManagementService.GetAllEventManagements();

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
            try
            {
                if (selectedEventId == 0)
                {
                    ViewBag.ErrorMessage = "Please select a valid event.";
                    var events = await _eventManagementService.GetAllEventManagements();
                    var eventViewModels = events.Select(e => new EventViewModel
                    {
                        EventId = e.Id,       
                        EventName = e.EventID
                    }).ToList();
                    return View("Index", eventViewModels);
                }

                // Get logged-in user (assuming you have Identity set up)
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    ViewBag.ErrorMessage = "User not found. Please log in again.";
                    var events = await _eventManagementService.GetAllEventManagements();
                    var eventViewModels = events.Select(e => new EventViewModel
                    {
                        EventId = e.Id,       // Assuming `Id` is the primary key in your data
                        EventName = e.EventID // Assuming `EventID` is the event name or identifier
                    }).ToList();
                    return View("Index", eventViewModels);
                }

                // Check role against EventId
                //var eventManagement = await _eventManagementService.GetEventManagementForEventSelectionById(selectedEventId);
                //if (eventManagement == null)
                //{
                //    ViewBag.ErrorMessage = "You are not assigned to this event.";
                //    return View("Index", await _eventManagementService.GetAllEventManagements());
                //}

                var eventManagement = await _eventManagementService.GetEventManagementForEventSelectionById(selectedEventId);
                var eventStaff = await _eventStaffService.GetEventStaffByColumn(userId);

                bool isUserInEvent = eventManagement.EventStaffDetailList.Any(esd => esd.EventStaffId == eventStaff.Id);
                
                if (!isUserInEvent)
                {
                    ViewBag.ErrorMessage = "You are not assigned in selected Event.";
                    var events = await _eventManagementService.GetAllEventManagements();
                    var eventViewModels = events.Select(e => new EventViewModel
                    {
                        EventId = e.Id,       // Assuming `Id` is the primary key in your data
                        EventName = e.EventID // Assuming `EventID` is the event name or identifier
                    }).ToList();
                    return View("Index", eventViewModels);
                }

                var eventRolesIds = eventManagement.EventStaffDetailList
                    .Where(esd => esd.EventStaffId == eventStaff.Id) // Manual join using EventStaffId
                    .SelectMany(esd => esd.EventWiseStaffRoleList)
                    .Select(ewsr => ewsr.RoleId)
                    .ToList();

                var allRoles = await _roleManager.Roles.ToListAsync(); // Fetch all roles first

                var eventRoleNames = allRoles
                    .Where(r => eventRolesIds.Contains(r.Id)) // Now filter in memory
                    .Select(r => r.Name)
                    .ToList();
                // Assign role if necessary
                //var assignResult = await _eventManagementService.AssignUserRoleToEvent(userId, selectedEventId);
                //if (!assignResult)
                //{
                //    ViewBag.ErrorMessage = "Failed to assign the role.";
                //    return View("Index", await _eventManagementService.GetAllEventManagements());
                //}
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }

                var identityRoles = await _userManager.GetRolesAsync(user);

                var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                identity.AddClaim(new Claim("EventId", selectedEventId.ToString())); // Store event ID in claims

                // Add all roles as claims
                foreach (var role in identityRoles.Concat(eventRoleNames))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }

                var principal = new ClaimsPrincipal(identity);
                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(user, isPersistent: false);

                ViewBag.Message = "Role successfully assigned!";
                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred: " + ex.Message;
                var events = await _eventManagementService.GetAllEventManagements();
                var eventViewModels = events.Select(e => new EventViewModel
                {
                    EventId = e.Id,       // Assuming `Id` is the primary key in your data
                    EventName = e.EventID // Assuming `EventID` is the event name or identifier
                }).ToList();
                return View("Index", eventViewModels);
            }
        }

    }
}
