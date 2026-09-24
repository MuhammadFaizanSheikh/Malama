using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Utilities;
using Malama.Attributes;
using Malama.Models;
using Malama.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExcelFilesCompiler.Controllers
{
    public class TreatmentConsentController : Controller
    {
        private readonly ITreatmentConsentService _treatmentConsentService;
        private readonly IFileUploadDownloadService _fileService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEventStaffService _eventStaffService;
        private readonly ILogger<TreatmentConsentController> _logger;
        private const string CLASSNAME = nameof(TreatmentConsentController);

        public TreatmentConsentController(
            ILogger<TreatmentConsentController> logger,
            ITreatmentConsentService treatmentConsentService,
            IFileUploadDownloadService fileService,
            UserManager<ApplicationUser> userManager,
            IEventStaffService eventStaffService)
        {
            _logger = logger;
            _treatmentConsentService = treatmentConsentService;
            _fileService = fileService;
            _userManager = userManager;
            _eventStaffService = eventStaffService;
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

                var namesByUserId = await DentalExamSignatureHelper.ResolveDisplayNamesByUserIdAsync(
                    viewModel.ServiceMembers.Select(s => s.CompletedBy),
                    _userManager,
                    _eventStaffService,
                    _logger);

                foreach (var item in viewModel.ServiceMembers)
                {
                    item.CompletedBy = DentalExamSignatureHelper.FormatAuditDisplayName(
                        item.CompletedBy,
                        namesByUserId);
                }

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
                    TempData["ResponseTitle"] = "Not Eligible";
                    TempData["ResponseMessage"] = "Service member not found or not eligible for Treatment Consent.";
                    return RedirectToAction(nameof(Index));
                }

                var smModeActive = TreatmentConsentSmModeHelper.IsActive(HttpContext.Session)
                    && TreatmentConsentSmModeHelper.GetLockedServiceMembersChildId(HttpContext.Session) == serviceMembersChildId;

                ViewBag.IsTreatmentConsentSmMode = smModeActive;

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Station page ready for ServiceMembersChildId={ServiceMembersChildId}, SmMode={SmMode}",
                    CLASSNAME, methodName, serviceMembersChildId, smModeActive);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAttributeAuthorizeFromConfig("TreatmentConsent_View")]
        public async Task<IActionResult> SaveStation([FromBody] TreatmentConsentStationSaveDto dto)
        {
            const string methodName = nameof(SaveStation);
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            try
            {
                if (TreatmentConsentSmModeHelper.IsActive(HttpContext.Session))
                {
                    return Json(TreatmentConsentStationSaveResult.Fail(
                        "Locked",
                        "Unlock service member mode before saving."));
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(TreatmentConsentStationSaveResult.Fail("Unauthorized", "Please login and try again."));
                }

                var result = await _treatmentConsentService.SaveStationAsync(
                    dto,
                    user.Id);

                if (result.Success)
                {
                    result.RedirectUrl = Url.Action(nameof(Index), "TreatmentConsent");
                    TempData["ResponseStatus"] = "success";
                    TempData["ResponseTitle"] = result.Title;
                    TempData["ResponseMessage"] = result.Message;
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception while saving station", CLASSNAME, methodName);
                return Json(TreatmentConsentStationSaveResult.Fail("Error", "Unable to save Treatment Consent."));
            }
        }

        [HttpGet]
        [RoleAttributeAuthorizeFromConfig("TreatmentConsent_View")]
        public IActionResult DownloadSignature(string prefix, string fileName)
        {
            const string methodName = nameof(DownloadSignature);
            try
            {
                if (string.IsNullOrWhiteSpace(prefix) || string.IsNullOrWhiteSpace(fileName))
                {
                    return NotFound();
                }

                var allowed = prefix is TreatmentConsentFileSaveCoordinator.OralSurgeryPrefix
                    or TreatmentConsentFileSaveCoordinator.DentalTreatmentPrefix;
                if (!allowed)
                {
                    return BadRequest("Invalid signature prefix.");
                }

                var file = _fileService.GetFile(
                    TreatmentConsentFileSaveCoordinator.StationName,
                    prefix,
                    fileName);
                if (file == null || file.Bytes == null)
                {
                    return NotFound();
                }

                Response.Headers["Content-Disposition"] = $"inline; filename=\"{file.FileName}\"";
                return File(file.Bytes, file.ContentType ?? "image/png");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Failed for Prefix={Prefix}, File={File}",
                    CLASSNAME, methodName, prefix, fileName);
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAttributeAuthorizeFromConfig("TreatmentConsent_View")]
        public async Task<IActionResult> StartSmMode([FromBody] TreatmentConsentStartSmModeRequest request)
        {
            const string methodName = nameof(StartSmMode);
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            try
            {
                if (request == null || request.ServiceMembersChildId <= 0)
                {
                    return Json(TreatmentConsentSmModeResponse.Fail("Service member is required."));
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(TreatmentConsentSmModeResponse.Fail("Please login and try again."));
                }

                var pageModel = await _treatmentConsentService.GetStationPageAsync(request.ServiceMembersChildId);
                if (pageModel == null)
                {
                    return Json(TreatmentConsentSmModeResponse.Fail("Service member not found."));
                }

                TreatmentConsentSmModeHelper.Start(HttpContext.Session, request.ServiceMembersChildId, user.Id);

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, SM mode started. ServiceMembersChildId={ServiceMembersChildId}, UserId={UserId}",
                    CLASSNAME, methodName, request.ServiceMembersChildId, user.Id);

                return Json(TreatmentConsentSmModeResponse.Ok(
                    "Service member mode started. Hand the tablet to the service member.",
                    isActive: true,
                    serviceMembersChildId: request.ServiceMembersChildId,
                    redirectUrl: Url.Action(
                        nameof(TreatmentConsentStation),
                        "TreatmentConsent",
                        new { serviceMembersChildId = request.ServiceMembersChildId })));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception while starting SM mode", CLASSNAME, methodName);
                return Json(TreatmentConsentSmModeResponse.Fail("Unable to start service member mode."));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAttributeAuthorizeFromConfig("TreatmentConsent_View")]
        public async Task<IActionResult> UnlockSmMode([FromBody] TreatmentConsentUnlockSmModeRequest request)
        {
            const string methodName = nameof(UnlockSmMode);
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            try
            {
                if (!TreatmentConsentSmModeHelper.IsActive(HttpContext.Session))
                {
                    return Json(TreatmentConsentSmModeResponse.Ok(
                        "Service member mode is already inactive.",
                        isActive: false));
                }

                if (request == null || string.IsNullOrWhiteSpace(request.Password))
                {
                    return Json(TreatmentConsentSmModeResponse.Fail("Password is required."));
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(TreatmentConsentSmModeResponse.Fail("Please login and try again."));
                }

                var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
                if (!passwordValid)
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, Unlock failed due to invalid password. UserId={UserId}",
                        CLASSNAME, methodName, user.Id);
                    return Json(TreatmentConsentSmModeResponse.Fail("Incorrect password."));
                }

                var lockedSmId = TreatmentConsentSmModeHelper.GetLockedServiceMembersChildId(HttpContext.Session);
                TreatmentConsentSmModeHelper.Clear(HttpContext.Session);

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, SM mode unlocked by UserId={UserId}, LockedSmId={LockedSmId}",
                    CLASSNAME, methodName, user.Id, lockedSmId);

                return Json(TreatmentConsentSmModeResponse.Ok(
                    "Staff unlock successful.",
                    isActive: false,
                    serviceMembersChildId: lockedSmId,
                    redirectUrl: lockedSmId.HasValue
                        ? Url.Action(
                            nameof(TreatmentConsentStation),
                            "TreatmentConsent",
                            new { serviceMembersChildId = lockedSmId.Value })
                        : Url.Action(nameof(Index), "TreatmentConsent")));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception while unlocking SM mode", CLASSNAME, methodName);
                return Json(TreatmentConsentSmModeResponse.Fail("Unable to unlock service member mode."));
            }
        }

        [HttpGet]
        [RoleAttributeAuthorizeFromConfig("TreatmentConsent_View")]
        public IActionResult SmModeStatus()
        {
            var active = TreatmentConsentSmModeHelper.IsActive(HttpContext.Session);
            var smId = TreatmentConsentSmModeHelper.GetLockedServiceMembersChildId(HttpContext.Session);
            return Json(TreatmentConsentSmModeResponse.Ok(
                isActive: active,
                serviceMembersChildId: smId));
        }
    }
}
