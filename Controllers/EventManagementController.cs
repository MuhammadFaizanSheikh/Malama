using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using Malama.Attributes;
using Malama.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics.Contracts;

namespace ExcelFilesCompiler.Controllers
{
    public class EventManagementController : Controller
    {
        private readonly IEventManagementService _eventManagementService;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventManagementController(IEventManagementService eventManagementService, UserManager<ApplicationUser> userManager)
        {
            _eventManagementService = eventManagementService;
            _userManager = userManager;
        }

        [RoleAttributeAuthorizeFromConfig("EventManagement_View")]
        [HttpGet]
        public async Task<IActionResult> Index(long? editId) // eventid for duplication event handling
        {
            var responseDto = new ResponseDto();
            List<EventManagementPreview> eventManagementList = new();

            try
            {
                if (editId.HasValue)
                {
                    ViewBag.EditId = editId;
                    return View();
                }

                long? claimEventId = null;

                // 🔹 If Event Manager → get EventId from claim
                if (User.IsInRole("Event Manager"))
                {
                    var eventIdClaim = User.FindFirst("EventIdLong")?.Value;

                    if (!string.IsNullOrEmpty(eventIdClaim) &&
                        long.TryParse(eventIdClaim, out long parsedId))
                    {
                        claimEventId = parsedId;
                    }
                }

                // Pass claimEventId (null means return all)
                eventManagementList = await _eventManagementService
                    .GetAllEventManagements(claimEventId);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] =
                    "We encountered an issue while loading event managements. Please try again later.";
            }

            var viewModel = new EventManagementViewModel
            {
                EventManagements = eventManagementList,
                SingleEventManagement = null
            };

            return View(viewModel);
        }

        [HttpPost]
        [RoleAttributeAuthorizeFromConfig("EventManagement_Save")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEventManagement(EventManagementViewModel eventManagement, string action, string completedSections)
        {
            try
            {
                ResponseDto res = new ResponseDto();

                if (eventManagement.SingleEventManagement.Id == 0)
                {
                    ModelState.Remove("SingleEventManagement.Id");
                }

                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState.Values)
                    {
                        foreach (var err in error.Errors)
                        {
                            Console.WriteLine($"Error: {err.ErrorMessage}");
                        }
                    }

                    return View("Index", eventManagement);
                }

                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    int totalSections = 7; // Total number of sections
                    var filledSectionsList = completedSections.Split(',').ToList();
                    eventManagement.SingleEventManagement.StatusDescription = (filledSectionsList.Count == totalSections) ? "Completed" : "Pending";
                    eventManagement.SingleEventManagement.CompletedSections = completedSections;

                    foreach (var staffDetail in eventManagement.SingleEventManagement.EventStaffDetailList)
                    {
                        // Convert SelectedRoles (list of role IDs) into EventWiseStaffRoleList (list of objects)
                        staffDetail.EventWiseStaffRoleList = staffDetail.SelectedRoles
                            .Select(roleId => new EventWiseStaffRole { RoleId = roleId })
                            .ToList();

                        staffDetail.EventWiseStaffSecondaryRoleList = staffDetail.SelectedSecondaryRoles
                            .Select(roleId => new EventWiseStaffSecondaryRole { RoleId = roleId })
                            .ToList();

                        staffDetail.AvailabilityDatesList = (staffDetail.AvailabilityDates ?? new List<DateTime>())
                        .Select(availabilityDate => new EventManagementStaffAvailability { AvailabilityDate = availabilityDate })
                        .ToList();

                        if (staffDetail.SelectedSecondaryStationList != null && staffDetail.SelectedSecondaryStationList.Count > 0)
                        {
                            staffDetail.SelectedSecondaryStation = string.Join(",", staffDetail.SelectedSecondaryStationList);
                        }
                    }

                    if (action == "Add")
                    {
                        res = await _eventManagementService.AddEventManagementAsync(eventManagement.SingleEventManagement, eventManagement.SubmissionToken, user.UserName);
                    }
                    else if (action == "Update" || action == "UpdateAndDuplicate")
                    {
                        res = await _eventManagementService.UpdateEventManagementAsync(eventManagement.SingleEventManagement, user.UserName, action);
                    }
                }
                else
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Error";
                    TempData["ResponseMessage"] = "Please login and try again";
                    TempData["ContractDto"] = eventManagement;
                    //return RedirectToAction("Index", contractDto);
                    return RedirectToAction("Index");
                }

                TempData["ResponseStatus"] = res.Success ? "success" : "error"; // SweetAlert2 icon
                TempData["ResponseTitle"] = res.Success ? "Success" : "Error";
                TempData["ResponseMessage"] = res.Message;

                if (action == "UpdateAndDuplicate")
                {
                    return RedirectToAction("Index", new { editId = eventManagement.SingleEventManagement.Id });
                }
                return RedirectToAction("Index");
                //return RedirectToAction("Index", contractDto);
            }
            catch (Exception ex)
            {
                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = "An unexpected error occurred.";
                TempData["ContractDto"] = eventManagement;
                return RedirectToAction("Index", eventManagement);
            }
        }

        [HttpGet]
        //[RoleAttributeAuthorizeFromConfig("EventManagement_View")]
        public async Task<IActionResult> GetNextEventManagementId()
        {
            try
            {
                var eventManagementId = await _eventManagementService.GetNextEventManagementId();

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        eventManagementId = eventManagementId
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching sequence of Event Management Id." });
            }
        }

        [HttpGet]
        [RoleAttributeAuthorizeFromConfig("EventManagement_View")]
        public async Task<IActionResult> GetEventManagementById(long id)
        {
            try
            {
                var combinedData = await _eventManagementService.GetEventManagementById(id);
                if (combinedData == null)
                {
                    return Json(new { success = false, message = "Contract not found." });
                }

                return Json(new { success = true, combinedData = combinedData });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEventStartAndEndDateById(long eventId)
        {
            try
            {
                var data = await _eventManagementService
                    .GetEventDetailsById(eventId);

                return Ok(new
                {
                    startDate = data.StartDate.ToString("yyyy-MM-dd"),
                    endDate = data.EndDate.ToString("yyyy-MM-dd"),
                    version = data.Version
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    message = "Something went wrong while fetching event details."
                });
            }
        }




    }
}
