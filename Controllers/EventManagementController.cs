using ExcelFilesCompiler.Interfaces;
using Malama.Attributes;
using Malama.Models;
using Malama.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExcelFilesCompiler.Controllers
{
    public class EventManagementController : Controller
    {
        private readonly IEventManagementService _eventManagementService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EventManagementController> _logger;
        private const string CLASSNAME = "EventManagementController";

        public EventManagementController(ILogger<EventManagementController> logger, IEventManagementService eventManagementService, UserManager<ApplicationUser> userManager)
        {
            _eventManagementService = eventManagementService;
            _userManager = userManager;
            _logger = logger;
        }

        [RoleAttributeAuthorizeFromConfig("EventManagement_View")]
        [HttpGet]
        public async Task<IActionResult> Index(long? editId) // eventid for duplication event handling
        {
            const string methodName = "Index";
            _logger.LogInformation("{ClassName}, {MethodName}, Called with editId: {EditID}",
                CLASSNAME, methodName, editId);

            var responseDto = new ResponseDto();
            List<EventManagementPreview> eventManagementList = new();

            try
            {
                if (editId.HasValue)
                {
                    ViewBag.EditId = editId;
                    _logger.LogInformation("{ClassName}, {MethodName}, EditId provided, returning View only.", CLASSNAME, methodName);
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
                        _logger.LogInformation("{ClassName}, {MethodName}, Event Manager detected, claimEventId: {ClaimEventID}",
                            CLASSNAME, methodName, claimEventId);
                    }
                }

                // Pass claimEventId (null means return all)
                eventManagementList = await _eventManagementService.GetAllEventManagements(claimEventId);

                _logger.LogInformation("{ClassName}, {MethodName}, Retrieved {Count} event management records.",
                    CLASSNAME, methodName, eventManagementList.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Error while loading event managements.",
                    CLASSNAME, methodName);

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEventManagement(EventManagementViewModel eventManagement, string action, string completedSections)
        {
            const string methodName = "CreateEventManagement";
            _logger.LogInformation("{ClassName}, {MethodName}, Called with action: {Action}, EventID: {EventID}",
                CLASSNAME, methodName, action, eventManagement.SingleEventManagement?.Id);

            try
            {
                //Check authorization because add and update roles are different
                var feature = eventManagement.SingleEventManagement.Id == 0
                ? "EventManagement_Add"
                : "EventManagement_Update";

                var authService = HttpContext.RequestServices
                    .GetRequiredService<IAuthorizationService>();

                var requirement = RoleAttributeRequirementProvider.GetRequirement(feature);

                var result = await authService.AuthorizeAsync(User, null, requirement);

                if (!result.Succeeded)
                {
                    return Forbid();
                }

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
                            _logger.LogWarning("{ClassName}, {MethodName}, ModelState error: {Error}",
                                CLASSNAME, methodName, err.ErrorMessage);
                        }
                    }

                    return View("Index", eventManagement);
                }

                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    _logger.LogInformation("{ClassName}, {MethodName}, User authenticated: {UserName}",
                        CLASSNAME, methodName, user.UserName);

                    int totalSections = 7; // Total number of sections
                    var filledSectionsList = completedSections.Split(',').ToList();
                    eventManagement.SingleEventManagement.StatusDescription = (filledSectionsList.Count == totalSections) ? "Completed" : "Pending";
                    eventManagement.SingleEventManagement.CompletedSections = completedSections;

                    foreach (var staffDetail in eventManagement.SingleEventManagement.EventStaffDetailList)
                    {
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
                        _logger.LogInformation("{ClassName}, {MethodName}, Adding new event.", CLASSNAME, methodName);
                        res = await _eventManagementService.AddEventManagementAsync(eventManagement.SingleEventManagement, eventManagement.SubmissionToken, user.UserName);
                    }
                    else if (action == "Update" || action == "UpdateAndDuplicate")
                    {
                        _logger.LogInformation("{ClassName}, {MethodName}, Updating event ID: {EventID}", CLASSNAME, methodName, eventManagement.SingleEventManagement.Id);
                        res = await _eventManagementService.UpdateEventManagementAsync(eventManagement.SingleEventManagement, user.UserName, action);
                    }

                    _logger.LogInformation("{ClassName}, {MethodName}, Operation result: Success={Success}, Message={Message}",
                        CLASSNAME, methodName, res.Success, res.Message);
                }
                else
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, User not authenticated", CLASSNAME, methodName);

                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Error";
                    TempData["ResponseMessage"] = "Please login and try again";
                    TempData["ContractDto"] = eventManagement;
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Unexpected error while creating/updating event.", CLASSNAME, methodName);

                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = "An unexpected error occurred.";
                TempData["ContractDto"] = eventManagement;
                return RedirectToAction("Index", eventManagement);
            }
        }

        [HttpGet]
        //[RoleAttributeAuthorizeFromConfig("EventManagement_View")]
        public async Task<IActionResult> GetNextEventIdNumber()
        {
            try
            {
                var eventManagementId = await _eventManagementService.GetNextEventIdNumber();

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

                // 🔹 Check update permission using existing helper
                var canEdit = DashboardAuthorizationHelper
                    .HasAccess(User, "EventManagement_Add");

                return Json(new
                {
                    success = true,
                    combinedData = combinedData,
                    canEdit = canEdit
                });
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
