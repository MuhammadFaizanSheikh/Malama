using ExcelFilesCompiler.Interfaces;
using Malama.Attributes;
using Malama.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExcelFilesCompiler.Controllers
{
    public class TreatmentConsentController : Controller
    {
        private readonly ITreatmentConsentService _treatmentConsentService;
        private readonly ILogger<TreatmentConsentController> _logger;
        private const string CLASSNAME = nameof(TreatmentConsentController);

        public TreatmentConsentController(
            ILogger<TreatmentConsentController> logger,
            ITreatmentConsentService treatmentConsentService)
        {
            _logger = logger;
            _treatmentConsentService = treatmentConsentService;
        }

        [HttpGet]
        [RoleAttributeAuthorizeFromConfig("TreatmentConsent_View")]
        public async Task<IActionResult> Index()
        {
            const string methodName = nameof(Index);
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            try
            {
                string? eventId = HttpContext.Session.GetString("GlobalEventIdLong");

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Retrieved GlobalEventId: {EventId}",
                    CLASSNAME, methodName, eventId);

                if (string.IsNullOrWhiteSpace(eventId) || !long.TryParse(eventId, out long parsedEventId))
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, Invalid EventId: {EventId}",
                        CLASSNAME, methodName, eventId);

                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid EventId";
                    TempData["ResponseMessage"] = "Invalid EventId";

                    return View(new TreatmentConsentIndexViewModel());
                }

                var viewModel = await _treatmentConsentService
                    .GetCheckedInServiceMembersByEventIdAsync(parsedEventId, eventId);

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Loaded {Count} records for EventId={EventId}",
                    CLASSNAME, methodName, viewModel.TotalCount, eventId);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while loading Treatment Consent index page",
                    CLASSNAME, methodName);

                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = ex.Message;

                return View(new TreatmentConsentIndexViewModel());
            }
        }

        [HttpGet]
        [RoleAttributeAuthorizeFromConfig("TreatmentConsent_View")]
        public async Task<IActionResult> TreatmentConsentStation(long serviceMembersChildId)
        {
            const string methodName = nameof(TreatmentConsentStation);
            _logger.LogInformation(
                "{ClassName}, {MethodName}, Called with ServiceMembersChildId={ServiceMembersChildId}",
                CLASSNAME, methodName, serviceMembersChildId);

            try
            {
                if (serviceMembersChildId <= 0)
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, Invalid ServiceMembersChildId={ServiceMembersChildId}",
                        CLASSNAME, methodName, serviceMembersChildId);

                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Request";
                    TempData["ResponseMessage"] = "Service member is required.";
                    return RedirectToAction(nameof(Index));
                }

                var pageModel = await _treatmentConsentService.GetStationPageAsync(serviceMembersChildId);
                if (pageModel == null)
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Not Found";
                    TempData["ResponseMessage"] = "Service member not found.";
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Station page ready for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, serviceMembersChildId);

                return View(pageModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception while loading Treatment Consent station for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, serviceMembersChildId);

                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
