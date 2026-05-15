using ExcelFilesCompiler.Interfaces;
using Malama.Attributes;
using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExcelFilesCompiler.Controllers
{
    public class LabStationController : Controller
    {
        private readonly IFileUploader _fileUploader;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILabStationService _labStationService;
        private readonly ILogger<LabStationController> _logger;
        private const string CLASSNAME = "LabStationController";

        public LabStationController(ILogger<LabStationController> logger, IFileUploader fileUploader, IConfiguration configuration, UserManager<ApplicationUser> userManager, ILabStationService labStationService)
        {
            _logger = logger;
            _fileUploader = fileUploader;
            _configuration = configuration;
            _userManager = userManager;
            _labStationService = labStationService;
        }

        [RoleAttributeAuthorizeFromConfig("LabStation_View")]
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

                var data = await _fileUploader.GetLabStationByEventIdAsync(parsedEventId);

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Retrieved {Count} records for EventId={EventId}",
                    CLASSNAME, methodName, data.Count, eventId
                );

                var summary = new Dictionary<string, int>
                {
                    ["Total"] = data.Count,
                    ["Pending"] = data.Count(x => x == null || x.LabStationRecord == null || x?.LabStationRecord?.Status == "Pending"),
                    ["Completed"] = data.Count(x => x.LabStationRecord?.Status == "Completed"),
                    ["NotGiven"] = data.Count(x => x.LabStationRecord?.Status == "Not given")
                };

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Summary calculated: Total={Total}, Pending={Pending}, Completed={Completed}, NotGiven={NotGiven}",
                    CLASSNAME,
                    methodName,
                    summary["Total"],
                    summary["Pending"],
                    summary["Completed"],
                    summary["NotGiven"]
                );

                ViewBag.Summary = summary;
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
                    "{ClassName}, {MethodName}, Exception occurred while loading immunization index page",
                    CLASSNAME, methodName
                );

                ViewBag.EventIdList = new List<SelectListItem>();
                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = ex.Message;

                return View();
            }
        }


        [RoleAttributeAuthorizeFromConfig("LabStation_View")]
        public async Task<IActionResult> LabStation(long labStationId, long serviceMembersChildId)
        {
            const string methodName = "LabStation";
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            try
            {
                LabStation model;
                long eventId = 0;

                if (labStationId > 0)
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Edit mode. labStationId={labStationId}",
                        CLASSNAME, methodName, labStationId
                    );

                    // Edit mode → get child record including parent
                    var result = await _labStationService.GetLabStationByIdWithEventIdAsync(labStationId);
                    model = result.LabStation;
                    eventId = result.EventId;
                }
                else
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Add mode. ServiceMembersChildId={serviceMembersChildId}",
                        CLASSNAME, methodName, serviceMembersChildId
                    );

                    // Add mode → create empty child but attach parent
                    var result = await _fileUploader.GetServiceMemberChildWithEventIdAsync(serviceMembersChildId);

                    model = new LabStation
                    {

                        ServiceMembersChildId = serviceMembersChildId,
                        ServiceMembersChild = result.ServiceMembersChild
                    };

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

        [RoleAttributeAuthorizeFromConfig("LabStation_Save")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveLabStation(LabStation model)
        {
            const string methodName = "SaveLabStation";
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, ModelState is invalid", CLASSNAME, methodName);

                    var allErrors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    var message = string.Join(" | ", allErrors);

                    _logger.LogError("{ClassName}, {MethodName}, Validation failed with errors: {Errors}", CLASSNAME, methodName, message);

                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = message;

                    return View("LabStation", model);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogError("{ClassName}, {MethodName}, User not found / unauthorized access", CLASSNAME, methodName);

                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Unauthorized";
                    TempData["ResponseMessage"] = "Please login and try again.";

                    return RedirectToAction("Index");
                }

                if (model.Id == 0)
                {
                    _logger.LogInformation("{ClassName}, {MethodName}, Add operation started by User={UserName}", CLASSNAME, methodName, user.UserName);

                    await _labStationService.AddAsync(model, user.UserName);

                    TempData["ResponseStatus"] = "success";
                    TempData["ResponseTitle"] = "Success";
                    TempData["ResponseMessage"] = "Lab record added successfully.";
                }
                else
                {
                    _logger.LogInformation("{ClassName}, {MethodName}, Update operation started for LabId={LabId} by User={UserName}", CLASSNAME, methodName, model.Id, user.UserName);

                    await _labStationService.UpdateAsync(model, user.UserName);

                    TempData["ResponseStatus"] = "success";
                    TempData["ResponseTitle"] = "Success";
                    TempData["ResponseMessage"] = "Lab record updated successfully.";
                }

                _logger.LogInformation("{ClassName}, {MethodName}, Operation completed successfully. Redirecting to Index", CLASSNAME, methodName);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception occurred while saving lab record", CLASSNAME, methodName);

                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = ex.Message;

                return View("LabStation", model);
            }
        }

        [RoleAttributeAuthorizeFromConfig("LabStation_HIVSignInSheet_View")]
        public async Task<IActionResult> GetLabDataAgainstEventIdAndGenerateHivPdf(long eventId)
        {
            const string methodName = "GetLabDataAgainstEventIdAndGenerateHivPdf";
            _logger.LogInformation("{ClassName}, {MethodName}, Called with EventId={EventId}",
                CLASSNAME, methodName, eventId);

            try
            {
                if (eventId <= 0)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, EventId is null or empty",
                        CLASSNAME, methodName);

                    return BadRequest("EventId is required.");
                }

                var pdfBytes = await _labStationService
                    .GetLabDataAgainstEventIdAndGenerateHivPdf(eventId);

                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, PDF generation returned empty result for EventId={EventId}",
                        CLASSNAME, methodName, eventId);

                    return NotFound("No HIV lab data found to generate report.");
                }

                var fileName = $"HIV_SignIn_Sheet_{eventId}.pdf";

                _logger.LogInformation("{ClassName}, {MethodName}, PDF generated successfully for EventId={EventId}",
                    CLASSNAME, methodName, eventId);

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while generating HIV PDF for EventId={EventId}",
                    CLASSNAME, methodName, eventId);

                return StatusCode(500, "An error occurred while generating HIV Sign-In Sheet.");
            }
        }


    }
}