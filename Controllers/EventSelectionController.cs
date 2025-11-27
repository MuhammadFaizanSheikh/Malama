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
                HttpContext.Session.Remove("GlobalEventId");

                if (selectedEventId == 0)
                {
                    //ViewBag.ErrorMessage = "Please select a valid event.";
                    //return await ReturnIndexView();
                    return RedirectToAction("Index", "Dashboard");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    ViewBag.ErrorMessage = "User not found. Please log in again.";
                    return await ReturnIndexView();
                }

                EventManagement eventManagement;
                EventStaff eventStaff;

                try
                {
                    eventManagement = await _eventManagementService.GetEventManagementForEventSelectionById(selectedEventId);
                    eventStaff = await _eventStaffService.GetEventStaffWithAttributesByUserId(userId);
                }
                catch (KeyNotFoundException ex)
                {
                    ViewBag.ErrorMessage = ex.Message; // Show user-friendly message
                    return await ReturnIndexView();
                }
                catch (ApplicationException ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                    return await ReturnIndexView();
                }

                bool isUserInEvent = eventManagement.EventStaffDetailList.Any(esd => esd.EventStaffId == eventStaff.Id);

                //if (!isUserInEvent)
                //{
                //    ViewBag.ErrorMessage = "You are not assigned to the selected event.";
                //    return await ReturnIndexView();
                //}

                bool isEventAssignedToStaff = false;
                List<string?> eventWiseRoleNames = new List<string?>();
                List<string> staffAttributes = new List<string>();

                if (isUserInEvent)
                {
                    isEventAssignedToStaff = true;

                    var staffDetails = eventManagement.EventStaffDetailList
                    .FirstOrDefault(esd => esd.EventStaffId == eventStaff.Id);


                    // Primary Roles
                    var eventRolesIds = staffDetails?.EventWiseStaffRoleList
                        .Select(r => r.RoleId)
                        .ToList() ?? new List<string>();

                    // Secondary Roles
                    var eventSecondaryRolesIds = staffDetails?.EventWiseStaffSecondaryRoleList
                        .Select(r => r.RoleId)
                        .ToList() ?? new List<string>();



                    if (staffDetails?.ProfileButtonAccess == true)
                    {
                        staffAttributes.Add("CanAccessProfile");
                    }


                    if (eventStaff.StaffQualification != null)
                    {
                        foreach (var license in eventStaff.StaffQualification)
                        {
                            foreach (var attribute in license.StaffAttributeDetails)
                            {
                                staffAttributes.Add(attribute.Attribute);
                            }
                        }
                    }

                    


                    //var eventRolesIds = eventManagement.EventStaffDetailList
                    //.Where(esd => esd.EventStaffId == eventStaff.Id)
                    //.SelectMany(esd => esd.EventWiseStaffRoleList)
                    //.Select(ewsr => ewsr.RoleId)
                    //.ToList();

                    //var eventSecondaryRolesIds = eventManagement.EventStaffDetailList
                    //    .Where(esd => esd.EventStaffId == eventStaff.Id)
                    //    .SelectMany(esd => esd.EventWiseStaffSecondaryRoleList)
                    //    .Select(ewsr => ewsr.RoleId)
                    //    .ToList();

                    var combinedRoleIds = eventRolesIds.Concat(eventSecondaryRolesIds).Distinct().ToList();

                    var allRoles = await _roleManager.Roles.ToListAsync();
                    eventWiseRoleNames = allRoles.Where(r => combinedRoleIds.Contains(r.Id)).Select(r => r.Name).ToList();
                }

                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return Unauthorized();
                }

                bool isClaimsUpdated = await UpdateUserClaimsAsync(user, selectedEventId, eventWiseRoleNames, eventManagement.EventID, isEventAssignedToStaff, staffAttributes);

                if (!isClaimsUpdated)
                {
                    ViewBag.ErrorMessage = "Failed to update user claims. Please try again.";
                    return View("Index", await _eventManagementService.GetAllEventManagements());
                }

                ViewBag.Message = "Role successfully assigned!";
                HttpContext.Session.SetString("GlobalEventId", eventManagement.EventID);//Setting eventId so that staff user can access data on station forms
                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return await ReturnIndexView();
            }
        }

        // Helper method to return the index view
        private async Task<IActionResult> ReturnIndexView()
        {
            var events = await _eventManagementService.GetAllEventManagements();
            var eventViewModels = events.Select(e => new EventViewModel
            {
                EventId = e.Id,
                EventName = e.EventID
            }).ToList();
            return View("Index", eventViewModels);
        }

        //private async Task<bool> UpdateUserClaimsAsync(ApplicationUser user, long selectedEventId, IEnumerable<string> eventRoleNames)
        //{
        //    try
        //    {
        //        var identityRoles = await _userManager.GetRolesAsync(user);
        //        var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
        //        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
        //        identity.AddClaim(new Claim("EventId", selectedEventId.ToString()));

        //        foreach (var role in identityRoles.Concat(eventRoleNames))
        //        {
        //            identity.AddClaim(new Claim(ClaimTypes.Role, role));
        //        }

        //        var principal = new ClaimsPrincipal(identity);
        //        await _signInManager.SignOutAsync();
        //        await _signInManager.SignInAsync(user, isPersistent: false);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        private async Task<bool> UpdateUserClaimsAsync(ApplicationUser user, long selectedEventId, IEnumerable<string> eventRoleNames, string eventID, bool isEventAssignedToStaff, List<string> staffAttributes)
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



    }
}
