using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Utilities;
using Malama.Attributes;
using Malama.Models;
using Malama.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ExcelFilesCompiler.Controllers
{
    public class PostEventDataAnalysisController : Controller
    {
        private readonly IEventManagementService _eventManagementService;
        private readonly IFileUploader _fileUploader;
        private readonly IPostEventLabStationService _postEventLabStationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<PostEventDataAnalysisController> _logger;
        private const string CLASSNAME = "EventManagementController";

        public PostEventDataAnalysisController(ILogger<PostEventDataAnalysisController> logger, IPostEventLabStationService postEventLabStationService, IFileUploader fileUploader, IEventManagementService eventManagementService, UserManager<ApplicationUser> userManager)
        {
            _eventManagementService = eventManagementService;
            _postEventLabStationService = postEventLabStationService;
            _fileUploader = fileUploader;
            _userManager = userManager;
            _logger = logger;
        }

        //[RoleAttributeAuthorizeFromConfig("EventManagement_View")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            const string methodName = "Index";
            _logger.LogInformation("{ClassName}, {MethodName}, Called.",
                CLASSNAME, methodName);

            var responseDto = new ResponseDto();
            List<PostEventManagementPreview> eventManagementList = new();

            try
            {
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

                eventManagementList = await _eventManagementService.GetAllForPostEventDataAnalysis(claimEventId);

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

            var viewModel = new PostEventManagementViewModel
            {
                EventManagements = eventManagementList,
            };

            return View(viewModel);
        }

        //[RoleAttributeAuthorizeFromConfig("PostEventManagement_View")]
        [HttpGet]
        public async Task<IActionResult> SelectStation(long eventManagementId, string selectedStation)
        {
            const string methodName = "SelectStation";

            _logger.LogInformation(
                "{ClassName}.{MethodName} - Called with EventManagementId={EventManagementId}, SelectedStation={SelectedStation}",
                CLASSNAME, methodName, eventManagementId, selectedStation);

            try
            {
                var model = new PostEventDataAnalysisViewModel
                {
                    EventId = eventManagementId,
                    SelectedStation = selectedStation
                };

                // 🔹 Fetch Event Management (to get business EventID like ABC0001)
                var eventManagement = await _eventManagementService
                    .GetEventManagementForEventSelectionByIdWithoutInclude(eventManagementId);

                if (eventManagement == null)
                {
                    _logger.LogWarning(
                        "{ClassName}.{MethodName} - EventManagement not found for EventManagementId={EventManagementId}",
                        CLASSNAME, methodName, eventManagementId);

                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Not Found";
                    TempData["ResponseMessage"] = "Event not found.";

                    return RedirectToAction("Index");
                }

                // ✅ Set EventID in ViewBag
                ViewBag.EventID = eventManagement.EventID;

                _logger.LogInformation(
                    "{ClassName}.{MethodName} - Loaded EventID={EventID} for EventManagementId={EventManagementId}",
                    CLASSNAME, methodName, eventManagement.EventID, eventManagementId);

                // 🔹 Load station data
                if (!string.IsNullOrEmpty(selectedStation))
                {
                    model.ServiceMembersChild = selectedStation switch
                    {
                        "Labs" => await _fileUploader.GetPreAndPostLabStationByEventIdAsync(model.EventId),
                        //"Immunization",
                        //"Dental",
                        //"XYZ",
                        _ => new List<ServiceMembersChild>()
                    };

                    _logger.LogInformation(
                        "{ClassName}.{MethodName} - Loaded data for SelectedStation={SelectedStation}, Count={Count}",
                        CLASSNAME, methodName, selectedStation, model.ServiceMembersChild?.Count ?? 0);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}.{MethodName} - Exception occurred for EventManagementId={EventManagementId}",
                    CLASSNAME, methodName, eventManagementId);

                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = "Something went wrong while loading data.";

                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> SpecificServiceMemberLabStation(long? postLabStationId, long serviceMembersChildId)
        {
            const string methodName = nameof(SpecificServiceMemberLabStation);

            _logger.LogInformation(
                "{ClassName}.{MethodName} - Called with PostLabStationId={PostLabStationId}, ServiceMembersChildId={ServiceMembersChildId}",
                CLASSNAME, methodName, postLabStationId, serviceMembersChildId);

            try
            {
                var model = await _fileUploader
                    .GetPostEventLabStationAnalysisDtoAsync(serviceMembersChildId);

                if (model == null)
                {
                    _logger.LogWarning(
                        "{ClassName}.{MethodName} - No data found for ServiceMembersChildId={ServiceMembersChildId}",
                        CLASSNAME, methodName, serviceMembersChildId);

                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseMessage"] = "Record not found.";

                    return RedirectToAction("Index");
                }

                _logger.LogInformation(
                    "{ClassName}.{MethodName} - Successfully prepared DTO for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, serviceMembersChildId);

                ViewBag.EventID = model.EventID;

                return View(model); // ✅ now sending DTO
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}.{MethodName} - Error occurred for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, serviceMembersChildId);

                TempData["ResponseStatus"] = "error";
                TempData["ResponseMessage"] = "Something went wrong.";

                return RedirectToAction("Index");
            }
        }

        //[RoleAttributeAuthorizeFromConfig("LabStation_Save")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePostEventLabStation(PostEventLabStationAnalysisDto model)
        {
            const string methodName = "SavePostEventLabStation";

            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, ModelState invalid", CLASSNAME, methodName);

                    var message = string.Join(" | ",
                        ModelState.Values.SelectMany(v => v.Errors)
                                         .Select(e => e.ErrorMessage));

                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = message;

                    return View("SpecificServiceMemberLabStation", model);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Unauthorized access", CLASSNAME, methodName);

                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Unauthorized";
                    TempData["ResponseMessage"] = "Please login and try again.";

                    return RedirectToAction("Index");
                }

                if (model.PostEventLabStation == null)
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Error";
                    TempData["ResponseMessage"] = "Invalid form data.";

                    return View("SpecificServiceMemberLabStation", model);
                }

                if (model.PostEventLabStation.Id == 0)
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Add operation started. User={User}",
                        CLASSNAME, methodName, user.UserName);

                    var result = await _postEventLabStationService
                        .AddAsync(model.PostEventLabStation, user.UserName);

                    if (!result.Success)
                    {
                        TempData["ResponseStatus"] = "error";
                        TempData["ResponseTitle"] = "Error";
                        TempData["ResponseMessage"] = result.Message;

                        return View("SpecificServiceMemberLabStation", model);
                    }

                    TempData["ResponseStatus"] = "success";
                    TempData["ResponseTitle"] = "Success";
                    TempData["ResponseMessage"] = result.Message;
                }
                else
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Update operation started. Id={Id}, User={User}",
                        CLASSNAME, methodName, model.PostEventLabStation.Id, user.UserName);

                    // var result = await _postEventLabStationService.UpdateAsync(...);

                    TempData["ResponseStatus"] = "success";
                    TempData["ResponseTitle"] = "Success";
                    TempData["ResponseMessage"] = "Record updated successfully.";
                }

                return RedirectToAction("SelectStation", new
                {
                    eventManagementId = model.EventId,
                    selectedStation = "Labs"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred",
                    CLASSNAME, methodName);

                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = "An unexpected error occurred.";

                return View("SavePostEventLabStation", model);
            }
        }
    }
}
