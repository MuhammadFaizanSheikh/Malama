using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Models;
using ExcelFilesCompiler.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExcelFilesCompiler.Controllers
{
    public class EventSelectionController : Controller
    {
        private readonly IEventManagementService _eventManagementService;
        private readonly IEventStaffService _eventStaffService;
        private readonly IUnitOfWork _unitOfWork;

        public EventSelectionController(IEventManagementService eventManagementService, IEventStaffService eventStaffService, IUnitOfWork unitOfWork)
        {
            _eventManagementService = eventManagementService;
            _eventStaffService = eventStaffService;
            _unitOfWork = unitOfWork;
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

                var roles = eventManagement.EventStaffDetailList
                    .Where(esd => esd.EventStaffId == eventStaff.Id) // Manual join using EventStaffId
                    .SelectMany(esd => esd.EventWiseStaffRoleList)
                    .Select(ewsr => ewsr.RoleId)
                    .ToList();

                // Assign role if necessary
                //var assignResult = await _eventManagementService.AssignUserRoleToEvent(userId, selectedEventId);
                //if (!assignResult)
                //{
                //    ViewBag.ErrorMessage = "Failed to assign the role.";
                //    return View("Index", await _eventManagementService.GetAllEventManagements());
                //}

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
