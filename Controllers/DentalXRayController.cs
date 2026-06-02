using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using Malama.Attributes;
using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace ExcelFilesCompiler.Controllers
{
    //[Authorize(Roles = "DAWSON Admin - Event Staff,Project Manager & Program Manager,Super Admin")]
    public class DentalXRayController : Controller
    {
        private readonly IFileUploader _fileUploader;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IContainerMonitoringService _service;
        private readonly ILogger<DentalXRayController> _logger;
        private const string CLASSNAME = "DentalXRayController";


        public DentalXRayController(ILogger<DentalXRayController> logger, IFileUploader fileUploader, IConfiguration configuration, UserManager<ApplicationUser> userManager, IContainerMonitoringService service)
        {
            _logger = logger;
            _fileUploader = fileUploader;
            _configuration = configuration;
            _userManager = userManager;
            _service = service;
        }

        //[RoleAttributeAuthorizeFromConfig("LabStation_View")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            const string methodName = "Index";
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            try
            {
                string eventId = HttpContext.Session.GetString("GlobalEventIdLong");

                _logger.LogInformation(
                        "{ClassName}, {MethodName}, Retrieved GlobalEventId: {EventId}",
                        CLASSNAME, methodName, eventId
                    );

                if (string.IsNullOrWhiteSpace(eventId) || !int.TryParse(eventId, out int parsedEventId))
                {
                    _logger.LogWarning("\"{ClassName}, {MethodName}: Invalid EventId: {eventId}", eventId);

                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid EventId";
                    TempData["ResponseMessage"] = "Invalid EventId";

                    return View("Index");
                }

                var data = await _fileUploader.GetDentalXRayStationByEventIdAsync(parsedEventId);

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Retrieved {Count} records for EventId={EventId}",
                    CLASSNAME, methodName, data.Count, eventId
                );

                //var summary = new Dictionary<string, int>
                //{
                //    ["Total"] = data.Count,
                //    ["Pending"] = data.Count(x => x == null || x.LabStationRecord == null || x?.LabStationRecord?.Status == "Pending"),
                //    ["Completed"] = data.Count(x => x.LabStationRecord?.Status == "Completed"),
                //    ["NotGiven"] = data.Count(x => x.LabStationRecord?.Status == "Not given")
                //};

                //_logger.LogInformation(
                //    "{ClassName}, {MethodName}, Summary calculated: Total={Total}, Pending={Pending}, Completed={Completed}, NotGiven={NotGiven}",
                //    CLASSNAME,
                //    methodName,
                //    summary["Total"],
                //    summary["Pending"],
                //    summary["Completed"],
                //    summary["NotGiven"]
                //);

                //ViewBag.Summary = summary;
                ViewBag.EventId = eventId;

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Returning Index view",
                    CLASSNAME, methodName
                );

                return View("Index", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Exception occurred while loading Dental X-Ray index page",
                    CLASSNAME, methodName
                );

                ViewBag.EventIdList = new List<SelectListItem>();
                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = ex.Message;

                return View();
            }
        }


        //[RoleAttributeAuthorizeFromConfig("LabStation_View")]
        public async Task<IActionResult> DentalXRayStation(long dentalXRayStationId, long serviceMembersChildId)
        {
            const string methodName = "DentalXRayStation";
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            try
            {
                LabStation model;
                long eventId = 0;

                model = null;
                if (dentalXRayStationId > 0)
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Edit mode. dentalXRayStationId={dentalXRayStationId}",
                        CLASSNAME, methodName, dentalXRayStationId
                    );

                    // Edit mode → get child record including parent
                    //var result = await _labStationService.GetLabStationByIdWithEventIdAsync(labStationId);
                    //model = result.LabStation;
                    //eventId = result.EventId;
                }
                else
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Add mode. ServiceMembersChildId={serviceMembersChildId}",
                        CLASSNAME, methodName, serviceMembersChildId
                    );

                    // Add mode → create empty child but attach parent
                    var result = await _fileUploader.GetServiceMemberChildWithEventIdAsync(serviceMembersChildId);

                    //model = new LabStation
                    //{

                    //    ServiceMembersChildId = serviceMembersChildId,
                    //    ServiceMembersChild = result.ServiceMembersChild
                    //};

                    eventId = result.EventId;
                }

                ViewBag.EventId = eventId;

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Using EventId={EventId}",
                    CLASSNAME, methodName, eventId
                );

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Returning view",
                    CLASSNAME, methodName
                );

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Exception occurred while loading ImmunizationStation",
                    CLASSNAME, methodName
                );

                throw;
            }
        }
    }
}