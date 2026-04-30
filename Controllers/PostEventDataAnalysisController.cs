using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<PostEventDataAnalysisController> _logger;
        private const string CLASSNAME = "EventManagementController";

        public PostEventDataAnalysisController(ILogger<PostEventDataAnalysisController> logger, IFileUploader fileUploader, IEventManagementService eventManagementService, UserManager<ApplicationUser> userManager)
        {
            _eventManagementService = eventManagementService;
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
        public async Task<IActionResult> PostEventDataAnalysis(long eventManagementId, string selectedStation)
        {
            const string methodName = "PostEventDataAnalysis";
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            try
            {
                var model = new PostEventDataAnalysisViewModel
                {
                    EventId = eventManagementId,
                    SelectedStation = selectedStation
                };

                var data = await _fileUploader.GetLabStationByEventIdAsync(model.EventId);

                if (!string.IsNullOrEmpty(selectedStation))
                {
                    model.ServiceMembersChild = selectedStation switch
                    {
                        "Labs" => await _fileUploader.GetLabStationByEventIdAsync(model.EventId),
                        //"Immunization" => await _fileUploader.GetLabStationByEventIdAsync(model.EventId),
                        //"Dental" => await _fileUploader.GetLabStationByEventIdAsync(model.EventId),
                        //"Hearing" => await _fileUploader.GetLabStationByEventIdAsync(model.EventId),
                        //"Vision" => await _fileUploader.GetLabStationByEventIdAsync(model.EventId),
                        //"EKG" => await _fileUploader.GetLabStationByEventIdAsync(model.EventId),
                        _ => new List<ServiceMembersChild>()
                    };
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception occurred", CLASSNAME, methodName);

                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = "Something went wrong while loading data.";

                return RedirectToAction("Index");
            }
        }
    }
}
