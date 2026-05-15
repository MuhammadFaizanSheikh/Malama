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
    public class VitalStationController : Controller
    {
        private readonly IFileUploader _fileUploader;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IVitalStationService _service;
        private readonly ILogger<VitalStationController> _logger;
        private const string CLASSNAME = "VitalStationController";


        public VitalStationController(ILogger<VitalStationController> logger, IFileUploader fileUploader, IConfiguration configuration, UserManager<ApplicationUser> userManager, IVitalStationService service)
        {
            _logger = logger;
            _configuration = configuration;
            _userManager = userManager;
            _service = service;
            _fileUploader = fileUploader;
        }

        [RoleAttributeAuthorizeFromConfig("VitalStation_View")]
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

                var data = await _fileUploader.GetVitalStationByEventIdAsync(parsedEventId);

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Retrieved {Count} records for EventId={EventId}",
                    CLASSNAME, methodName, data.Count, eventId
                );

                var summary = new Dictionary<string, int>
                {
                    ["Total"] = data.Count,
                    ["Pending"] = data.Count(x => x == null || x.VitalStationRecord == null || x?.VitalStationRecord?.Status == "Pending"),
                    ["Completed"] = data.Count(x => x.VitalStationRecord?.Status == "Completed"),
                    ["NotGiven"] = data.Count(x => x.VitalStationRecord?.Status == "Not given")
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
                    "{ClassName}, {MethodName}, Exception occurred while loading VitalStation index page",
                    CLASSNAME, methodName
                );

                ViewBag.EventIdList = new List<SelectListItem>();
                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = ex.Message;

                return View();
            }
        }

        [HttpGet]
        [RoleAttributeAuthorizeFromConfig("VitalStation_View")]
        public async Task<IActionResult> VitalStation(long vitalStationId, long serviceMembersChildId)
        {
            const string methodName = "VitalStation";

            _logger.LogInformation(
                "{ClassName}, {MethodName}, Called with VitalStationId={VitalStationId}, ServiceMembersChildId={ServiceMembersChildId}",
                CLASSNAME, methodName, vitalStationId, serviceMembersChildId
            );

            try
            {
                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Fetching VitalStation data",
                    CLASSNAME, methodName
                );

                var dto = await _service.GetVitalStationByServiceMemberChildIdAsync(serviceMembersChildId);

                if (dto == null)
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, No data found for ServiceMembersChildId={ServiceMembersChildId}",
                        CLASSNAME, methodName, serviceMembersChildId
                    );

                    return NotFound();
                }

                ViewBag.EventId = dto.EventID;

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, EventId={EventId} assigned to ViewBag",
                    CLASSNAME, methodName, dto.EventId
                );

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Successfully returning view",
                    CLASSNAME, methodName
                );

                return View(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Exception occurred. VitalStationId={VitalStationId}, ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, vitalStationId, serviceMembersChildId
                );

                throw;
            }
        }

        [RoleAttributeAuthorizeFromConfig("VitalStation_Save")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveVitalStation(VitalStationVM model)
        {
            const string methodName = "SaveVitalStation";
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

                    var childId = model?.VitalStationDto?.ServiceMembersChildId ?? 0;
                    if (childId > 0)
                    {
                        var vm = await _service.GetVitalStationByServiceMemberChildIdAsync(childId);
                        return View("VitalStation", vm);
                    }

                    return View("VitalStation", model);
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

                if (model.VitalStationDto.Id == 0)
                {
                    _logger.LogInformation("{ClassName}, {MethodName}, Add operation started by User={UserName}", CLASSNAME, methodName, user.UserName);

                    await _service.AddAsync(model.VitalStationDto, user.UserName);

                    TempData["ResponseStatus"] = "success";
                    TempData["ResponseTitle"] = "Success";
                    TempData["ResponseMessage"] = "Vital record saved successfully.";
                }
                else
                {
                    _logger.LogInformation("{ClassName}, {MethodName}, Update operation started for VitalStationId={VitalStationId} by User={UserName}", CLASSNAME, methodName, model.VitalStationDto.Id, user.UserName);

                    await _service.UpdateAsync(model.VitalStationDto, user.UserName);

                    TempData["ResponseStatus"] = "success";
                    TempData["ResponseTitle"] = "Success";
                    TempData["ResponseMessage"] = "Vital record saved successfully.";
                }

                _logger.LogInformation("{ClassName}, {MethodName}, Operation completed successfully. Redirecting to Vital Station page", CLASSNAME, methodName);
                return RedirectToAction(nameof(VitalStation), new
                {
                    vitalStationId = model.VitalStationDto.Id,
                    serviceMembersChildId = model.VitalStationDto.ServiceMembersChildId
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "{ClassName}, {MethodName}, Business rule violation", CLASSNAME, methodName);

                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Cannot save";
                TempData["ResponseMessage"] = ex.Message;

                var childId = model?.VitalStationDto?.ServiceMembersChildId ?? 0;
                if (childId > 0)
                {
                    var vm = await _service.GetVitalStationByServiceMemberChildIdAsync(childId);
                    return View("VitalStation", vm);
                }

                return View("VitalStation", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception occurred while saving VitalStation record", CLASSNAME, methodName);

                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = ex.Message;

                return View("VitalStation", model);
            }
        }
    }
}