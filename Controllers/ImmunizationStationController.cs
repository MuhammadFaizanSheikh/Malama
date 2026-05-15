using ExcelFilesCompiler.Interfaces;
using Malama.Attributes;
using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExcelFilesCompiler.Controllers
{
    public class ImmunizationStationController : Controller
    {
        private readonly IFileUploader _fileUploader;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImmunizationStationService _immunizationStationService;
        private readonly ILogger<ImmunizationStationController> _logger;
        private const string CLASSNAME = "ImmunizationStationController";

        public ImmunizationStationController(ILogger<ImmunizationStationController> logger, IFileUploader fileUploader, IConfiguration configuration, UserManager<ApplicationUser> userManager, IImmunizationStationService immunizationStationService)
        {
            _logger = logger;
            _fileUploader = fileUploader;
            _configuration = configuration;
            _userManager = userManager;
            _immunizationStationService = immunizationStationService;
        }

        [RoleAttributeAuthorizeFromConfig("ImmunizationStation_View")]
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

                var data = await _fileUploader.GetImmunizationsByEventIdAsync(parsedEventId);

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Retrieved {Count} records for EventId={EventId}",
                    CLASSNAME, methodName, data.Count, eventId
                );

                var summary = new Dictionary<string, int>
                {
                    ["Total"] = data.Count,
                    ["Pending"] = data.Count(x => x == null || x.ImmunizationRecord == null || x?.ImmunizationRecord?.Status == "Pending"),
                    ["Completed"] = data.Count(x => x.ImmunizationRecord?.Status == "Completed"),
                    ["NotGiven"] = data.Count(x => x.ImmunizationRecord?.Status == "Not given")
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


        [RoleAttributeAuthorizeFromConfig("ImmunizationStation_View")]
        public async Task<IActionResult> ImmunizationStation(long immunizationId, long serviceMembersChildId)
        {
            const string methodName = "ImmunizationStation";
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            try
            {
                ImmunizationStation model;
                long eventId = 0;

                if (immunizationId > 0)
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Edit mode. ImmunizationId={ImmunizationId}",
                        CLASSNAME, methodName, immunizationId
                    );

                    // Edit mode → get child record including parent
                    var result = await _immunizationStationService.GetImmunizationByIdWithEventIdAsync(immunizationId);
                    model = result.Immunization;
                    eventId = result.EventId;
                }
                else
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Add mode. FileDataId={FileDataId}",
                        CLASSNAME, methodName, serviceMembersChildId
                    );

                    // Add mode → create empty child but attach parent
                    var result = await _fileUploader.GetServiceMemberChildWithEventIdAsync(serviceMembersChildId);

                    model = new ImmunizationStation
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

                var immunizationData =
                    await _immunizationStationService.GetImmunizationManufacturer(eventId);

                if (immunizationData.Success && immunizationData.Data != null)
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Immunization manufacturer data loaded successfully",
                        CLASSNAME, methodName
                    );

                    ViewBag.ImmunizationData = immunizationData.Data;
                }
                else
                {
                    _logger.LogError(
                        "{ClassName}, {MethodName}, Failed to load immunization manufacturer data. Success={Success}",
                        CLASSNAME, methodName, immunizationData.Success
                    );

                    ViewBag.ImmunizationData = new List<object>();
                }

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

        [RoleAttributeAuthorizeFromConfig("ImmunizationStation_Save")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveImmunization(ImmunizationStation model)
        {
            const string methodName = "SaveImmunization";
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, ModelState is invalid",
                        CLASSNAME, methodName
                    );

                    var allErrors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    var message = string.Join(" | ", allErrors);

                    _logger.LogError(
                        "{ClassName}, {MethodName}, Validation failed with errors: {Errors}",
                        CLASSNAME, methodName, message
                    );

                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = message;

                    var result = await _immunizationStationService.GetImmunizationByIdWithEventIdAsync(model.Id);
                    if (result.Immunization == null)
                    {
                        TempData["ResponseStatus"] = "error";
                        TempData["ResponseTitle"] = "Not Found";
                        TempData["ResponseMessage"] = "Immunization record not found.";
                        return RedirectToAction("Index");
                    }
                    model = result.Immunization;
                    long eventId = result.EventId;
                    ViewBag.EventId = eventId;

                    _logger.LogDebug("{ClassName}, {MethodName}: Reloading view for EventId={EventId}", CLASSNAME, methodName, eventId);

                    var immunizationData = await _immunizationStationService.GetImmunizationManufacturer(eventId);

                    if (immunizationData.Success && immunizationData.Data != null)
                    {
                        ViewBag.ImmunizationData = immunizationData.Data;
                    }
                    else
                    {
                        _logger.LogError(
                            "{ClassName}, {MethodName}, Failed to load immunization manufacturer data. Success={Success}",
                            CLASSNAME, methodName, immunizationData.Success
                        );

                        ViewBag.ImmunizationData = new List<object>();
                    }

                    return View("ImmunizationStation", model);
                }

                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    _logger.LogError(
                        "{ClassName}, {MethodName}, User not found / unauthorized access",
                        CLASSNAME, methodName
                    );

                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Unauthorized";
                    TempData["ResponseMessage"] = "Please login and try again.";

                    return RedirectToAction("Index");
                }

                if (model.Id == 0)
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Add operation started by User={UserName}",
                        CLASSNAME, methodName, user.UserName
                    );

                    await _immunizationStationService.AddAsync(model, user.UserName);

                    TempData["ResponseStatus"] = "success";
                    TempData["ResponseTitle"] = "Success";
                    TempData["ResponseMessage"] = "Immunization record added successfully.";
                }
                else
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Update operation started for ImmunizationId={ImmunizationId} by User={UserName}",
                        CLASSNAME, methodName, model.Id, user.UserName
                    );

                    await _immunizationStationService.UpdateAsync(model, user.UserName);

                    TempData["ResponseStatus"] = "success";
                    TempData["ResponseTitle"] = "Success";
                    TempData["ResponseMessage"] = "Immunization record updated successfully.";
                }

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Operation completed successfully. Redirecting to Index",
                    CLASSNAME, methodName
                );

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Exception occurred while saving immunization record",
                    CLASSNAME, methodName
                );

                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = ex.Message;

                return View("ImmunizationStation", model);
            }
        }

    }
}