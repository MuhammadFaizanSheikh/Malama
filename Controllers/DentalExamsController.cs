using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Utilities;
using Malama.Attributes;
using Malama.Models;
using Malama.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExcelFilesCompiler.Controllers
{
    public class DentalExamsController : Controller
    {
        private readonly IFileUploader _fileUploader;
        private readonly IDentalQuestionnaireService _dentalQuestionnaireService;
        private readonly IDentalXRayStationService _dentalXRayStationService;
        private readonly IDentalExamService _dentalExamService;
        private readonly IVitalStationService _vitalStationService;
        private readonly IEventStaffService _eventStaffService;
        private readonly IFileUploadDownloadService _fileService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DentalExamsController> _logger;
        private const string CLASSNAME = "DentalExamsController";
        private const string XRayStationName = "DentalXRay";

        public DentalExamsController(
            ILogger<DentalExamsController> logger,
            IFileUploader fileUploader,
            IDentalQuestionnaireService dentalQuestionnaireService,
            IDentalXRayStationService dentalXRayStationService,
            IDentalExamService dentalExamService,
            IVitalStationService vitalStationService,
            IEventStaffService eventStaffService,
            IFileUploadDownloadService fileService,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _fileUploader = fileUploader;
            _dentalQuestionnaireService = dentalQuestionnaireService;
            _dentalXRayStationService = dentalXRayStationService;
            _dentalExamService = dentalExamService;
            _vitalStationService = vitalStationService;
            _eventStaffService = eventStaffService;
            _fileService = fileService;
            _userManager = userManager;
        }

        [RoleAttributeAuthorizeFromConfig("DentalExams_View")]
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
                    CLASSNAME, methodName, eventId);

                if (string.IsNullOrWhiteSpace(eventId) || !int.TryParse(eventId, out int parsedEventId))
                {
                    _logger.LogWarning("{ClassName}, {MethodName}: Invalid EventId: {EventId}", CLASSNAME, methodName, eventId);

                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid EventId";
                    TempData["ResponseMessage"] = "Invalid EventId";

                    return View("Index");
                }

                var data = await _fileUploader.GetDentalExamsByEventIdAsync(parsedEventId);

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Retrieved {Count} records for EventId={EventId}",
                    CLASSNAME, methodName, data.Count, eventId);

                var summary = new Dictionary<string, int>
                {
                    ["Total"] = data.Count,
                    ["Pending"] = data.Count(x => x.DentalExamRecord == null || x.DentalExamRecord.Status == "Pending"),
                    ["Completed"] = data.Count(x => x.DentalExamRecord?.Status == "Completed")
                };

                ViewBag.Summary = summary;
                ViewBag.EventId = eventId;

                return View("Index", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while loading Dental Exams index page",
                    CLASSNAME, methodName);

                ViewBag.EventIdList = new List<SelectListItem>();
                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = ex.Message;

                return View();
            }
        }

        [RoleAttributeAuthorizeFromConfig("DentalExams_View")]
        [HttpGet]
        public async Task<IActionResult> DentalExamStation(long serviceMembersChildId)
        {
            const string methodName = nameof(DentalExamStation);
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            try
            {
                if (serviceMembersChildId <= 0)
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Request";
                    TempData["ResponseMessage"] = "Service member is required.";
                    return RedirectToAction(nameof(Index));
                }

                var result = await _fileUploader.GetServiceMemberChildWithEventIdAsync(serviceMembersChildId);
                if (result.ServiceMembersChild == null)
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Not Found";
                    TempData["ResponseMessage"] = "Service member not found.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.EventId = result.EventId;

                try
                {
                    var vitalVm = await _vitalStationService.GetVitalStationByServiceMemberChildIdAsync(serviceMembersChildId);
                    var vitalDto = vitalVm?.VitalStationDto ?? new VitalStationDto
                    {
                        ServiceMembersChildId = serviceMembersChildId,
                        Status = AppConstants.Status.Pending
                    };

                    ViewBag.VitalStation = vitalDto;
                    ViewBag.VitalsCompleted = string.Equals(vitalDto.Status, AppConstants.Status.Completed, StringComparison.OrdinalIgnoreCase);

                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Vital station loaded for ServiceMembersChildId={ServiceMembersChildId}. VitalStationId={VitalStationId}, Status={Status}",
                        CLASSNAME, methodName, serviceMembersChildId, vitalDto.Id, vitalDto.Status);
                }
                catch (Exception vitalEx)
                {
                    _logger.LogError(vitalEx,
                        "{ClassName}, {MethodName}, Failed to load vital station for ServiceMembersChildId={ServiceMembersChildId}",
                        CLASSNAME, methodName, serviceMembersChildId);

                    ViewBag.VitalStation = new VitalStationDto
                    {
                        ServiceMembersChildId = serviceMembersChildId,
                        Status = AppConstants.Status.Pending
                    };
                    ViewBag.VitalsCompleted = false;
                }

                var questionnaire = await _dentalQuestionnaireService.GetByServiceMembersChildIdAsync(serviceMembersChildId)
                    ?? new DentalQuestionnaire { ServiceMembersChildId = serviceMembersChildId };

                var xRayStation = await _dentalXRayStationService.GetByServiceMembersChildIdAsync(serviceMembersChildId)
                    ?? new DentalXRayStation
                    {
                        ServiceMembersChildId = serviceMembersChildId,
                        Status = AppConstants.Status.Pending,
                        PaImages = new List<DentalXRayPaImage>()
                    };

                var dentalExam = await _dentalExamService.GetByServiceMembersChildIdAsync(serviceMembersChildId)
                    ?? new DentalExam { ServiceMembersChildId = serviceMembersChildId };

                var currentUser = await _userManager.GetUserAsync(User);
                var eventManagementId = DentalExamSignatureHelper.TryResolveEventManagementId(
                    User,
                    HttpContext.Session,
                    result.EventId);
                var (signatureDisplayName, signatureRoles) = await DentalExamSignatureHelper.ResolveDisplayAsync(
                    dentalExam.DentistSignatureEntered,
                    dentalExam.DentistSignatureUserId,
                    currentUser,
                    eventManagementId,
                    _userManager,
                    _eventStaffService,
                    _logger);

                ViewBag.DentistSignatureDisplayName = signatureDisplayName;
                ViewBag.DentistSignatureRoles = signatureRoles;
                ViewBag.CurrentUserId = currentUser?.Id ?? string.Empty;
                ViewBag.CurrentUserDisplayName = currentUser != null
                    ? await DentalExamSignatureHelper.ResolveDisplayNameAsync(currentUser, _eventStaffService, _logger)
                    : string.Empty;
                ViewBag.ExaminerNamesByUserId = await ResolveFindingExaminerNamesAsync(dentalExam);

                var pageModel = new DentalExamStationPageViewModel
                {
                    ServiceMember = result.ServiceMembersChild,
                    Questionnaire = questionnaire,
                    XRayStation = xRayStation,
                    DentalExam = dentalExam
                };

                return View(pageModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while loading Dental Exam station page",
                    CLASSNAME, methodName);
                throw;
            }
        }

        [RoleAttributeAuthorizeFromConfig("DentalExams_Save")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDentalExamStation(DentalExamStationSaveDto dto)
        {
            const string methodName = nameof(SaveDentalExamStation);

            var goToVitalStation = dto.GoToVitalStation
                || string.Equals(Request.Form["GoToVitalStation"], "true", StringComparison.OrdinalIgnoreCase);
            dto.GoToVitalStation = goToVitalStation;
            dto.QuestionnaireReviewed = FormCheckboxHelper.IsChecked(Request.Form, "QuestionnaireReviewed");
            dto.DentistSignatureEntered = FormCheckboxHelper.IsChecked(Request.Form, "DentistSignatureEntered");
            DentalQuestionnaireFormBinder.BindHealthConditions(dto, Request.Form);

            _logger.LogInformation("{ClassName}, {MethodName}, Called. GoToVitalStation={GoToVitalStation}",
                CLASSNAME, methodName, goToVitalStation);

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Unauthorized";
                    TempData["ResponseMessage"] = "Please login and try again.";
                    return RedirectToAction(nameof(Index));
                }

                if (dto.ServiceMembersChildId <= 0)
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = "Service member is required.";
                    return RedirectToAction(nameof(Index));
                }

                var serviceMemberResult = await _fileUploader.GetServiceMemberChildWithEventIdAsync(dto.ServiceMembersChildId);
                var serviceMember = serviceMemberResult.ServiceMembersChild;
                if (serviceMember == null)
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = "Service member not found.";
                    return RedirectToAction(nameof(Index));
                }

                if (DentalXRayStationService.IsNeeded(serviceMember.PanoNeeded))
                {
                    dto.PanoXRayAcknowledged = true;
                }
                else
                {
                    dto.PanoXRayAcknowledged = FormCheckboxHelper.IsChecked(Request.Form, "PanoXRayAcknowledged");
                }

                // Non-dentists (assistant, Event Manager, any future Dental Exam role) cannot change review/signature/subsequent fields.
                var isDentalExamDentist = User.IsInRole("DE- Dentist");

                if (!isDentalExamDentist && !dto.GoToVitalStation)
                {
                    await ApplyAssistantLockedDentalExamFieldsAsync(dto);
                }
                else if (dto.DentistSignatureEntered && !dto.GoToVitalStation)
                {
                    var existingExam = await _dentalExamService.GetByServiceMembersChildIdAsync(dto.ServiceMembersChildId);
                    dto.DentistSignatureUserId = ResolveDentistSignatureUserIdForSave(existingExam, user);
                }
                else if (!dto.GoToVitalStation)
                {
                    dto.DentistSignatureUserId = null;
                }

                var validationError = dto.GoToVitalStation
                    ? ValidateDraftBeforeVitalRedirect()
                    : await ValidateSaveDtoAsync(dto, serviceMember);
                if (!string.IsNullOrWhiteSpace(validationError))
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = validationError;
                    return RedirectToAction(nameof(DentalExamStation), new { serviceMembersChildId = dto.ServiceMembersChildId });
                }

                if (!dto.GoToVitalStation)
                {
                    await _dentalQuestionnaireService.SaveOrUpdateFromFormDataAsync(dto, user.UserName);
                    await _dentalExamService.SaveOrUpdateFromFormDataAsync(dto, user.UserName, user.Id);
                }

                if (dto.GoToVitalStation)
                {
                    long vitalStationId = 0;
                    try
                    {
                        var vitalVm = await _vitalStationService.GetVitalStationByServiceMemberChildIdAsync(dto.ServiceMembersChildId);
                        vitalStationId = vitalVm?.VitalStationDto?.Id ?? 0;
                    }
                    catch (Exception vitalEx)
                    {
                        _logger.LogWarning(vitalEx,
                            "{ClassName}, {MethodName}, Could not load vital station id before redirect. ServiceMembersChildId={ServiceMembersChildId}",
                            CLASSNAME, methodName, dto.ServiceMembersChildId);
                    }

                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Redirecting to Vital Station without saving. ServiceMembersChildId={ServiceMembersChildId}",
                        CLASSNAME, methodName, dto.ServiceMembersChildId);

                    return RedirectToAction("VitalStation", "VitalStation", new
                    {
                        vitalStationId,
                        serviceMembersChildId = dto.ServiceMembersChildId,
                        returnTo = "DentalExams"
                    });
                }

                TempData["ResponseStatus"] = "success";
                TempData["ResponseTitle"] = "Success";
                TempData["ResponseMessage"] = "Dental Exam record saved successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while saving Dental Exam record",
                    CLASSNAME, methodName);

                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = ex.Message;
                return RedirectToAction(nameof(DentalExamStation), new { serviceMembersChildId = dto.ServiceMembersChildId });
            }
        }

        private static string? ValidateDraftBeforeVitalRedirect()
        {
            return null;
        }

        private async Task ApplyAssistantLockedDentalExamFieldsAsync(DentalExamStationSaveDto dto)
        {
            const string methodName = nameof(ApplyAssistantLockedDentalExamFieldsAsync);

            try
            {
                var existing = await _dentalExamService.GetByServiceMembersChildIdAsync(dto.ServiceMembersChildId);
                if (existing == null)
                {
                    dto.QuestionnaireReviewed = false;
                    dto.DentistSignatureEntered = false;
                    dto.DentistSignatureUserId = null;
                    dto.FinalComments = null;
                    ClearSubsequentDiseasesDto(dto);
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, No existing dental exam. Cleared dentist-locked fields for non-dentist save. ServiceMembersChildId={ServiceMembersChildId}",
                        CLASSNAME, methodName, dto.ServiceMembersChildId);
                    return;
                }

                dto.QuestionnaireReviewed = existing.QuestionnaireReviewed;
                dto.DentistSignatureEntered = existing.DentistSignatureEntered;
                dto.DentistSignatureUserId = existing.DentistSignatureUserId;
                dto.FinalComments = existing.FinalComments;

                dto.PsrUpperRight = existing.PsrUpperRight;
                dto.PsrUpperAnterior = existing.PsrUpperAnterior;
                dto.PsrUpperLeft = existing.PsrUpperLeft;
                dto.PsrLowerRight = existing.PsrLowerRight;
                dto.PsrLowerAnterior = existing.PsrLowerAnterior;
                dto.PsrLowerLeft = existing.PsrLowerLeft;
                dto.PsrCarrierRisk = existing.PsrCarrierRisk;
                dto.SoftTissuesWnl = existing.SoftTissuesWnl;
                dto.SoftTissuesConditionDetail = existing.SoftTissuesConditionDetail;
                dto.DenClass = existing.DenClass;
                dto.DenClassReasonComments = existing.DenClassReasonComments;
                dto.PanoXRayAcknowledged = existing.PanoXRayAcknowledged;

                dto.Findings = existing.Findings?
                    .OrderBy(f => f.SortOrder)
                    .Select(DentalExamFindingMapper.ToDto)
                    .ToList() ?? new List<DentalExamFindingDto>();

                dto.FindingsJson = System.Text.Json.JsonSerializer.Serialize(
                    dto.Findings,
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                    });

                dto.PsrSelectedTeeth = existing.SelectedTeeth?
                    .Select(t => t.ToothNumber)
                    .Distinct()
                    .OrderBy(t => t)
                    .ToList() ?? new List<int>();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Preserved dentist-locked fields for non-dentist save. ServiceMembersChildId={ServiceMembersChildId}, FindingCount={FindingCount}",
                    CLASSNAME, methodName, dto.ServiceMembersChildId, dto.Findings.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Failed to preserve dentist-locked fields for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, dto.ServiceMembersChildId);
                throw;
            }
        }

        private static void ClearSubsequentDiseasesDto(DentalExamStationSaveDto dto)
        {
            dto.PsrUpperRight = null;
            dto.PsrUpperAnterior = null;
            dto.PsrUpperLeft = null;
            dto.PsrLowerRight = null;
            dto.PsrLowerAnterior = null;
            dto.PsrLowerLeft = null;
            dto.PsrCarrierRisk = null;
            dto.SoftTissuesWnl = null;
            dto.SoftTissuesConditionDetail = null;
            dto.DenClass = null;
            dto.DenClassReasonComments = null;
            dto.PanoXRayAcknowledged = false;
            dto.Findings = new List<DentalExamFindingDto>();
            dto.FindingsJson = "[]";
            dto.PsrSelectedTeeth = new List<int>();
        }

        private async Task<Dictionary<string, string>> ResolveFindingExaminerNamesAsync(DentalExam dentalExam)
        {
            return await DentalExamSignatureHelper.ResolveExaminerNamesByUserIdAsync(
                dentalExam.Findings,
                _userManager,
                _eventStaffService,
                _logger);
        }

        private static string? ResolveDentistSignatureUserIdForSave(DentalExam? existingExam, ApplicationUser user)
        {
            if (existingExam != null && !string.IsNullOrWhiteSpace(existingExam.DentistSignatureUserId))
            {
                return existingExam.DentistSignatureUserId;
            }

            return user.Id;
        }

        private Task<string?> ValidateSaveDtoAsync(DentalExamStationSaveDto dto, ServiceMembersChild serviceMember)
        {
            dto.Findings = DentalExamFindingBinder.ParseFromJson(dto.FindingsJson);

            var questionnaireError = DentalQuestionnaireValidator.Validate(dto, serviceMember);
            if (questionnaireError != null)
            {
                return Task.FromResult<string?>(questionnaireError);
            }

            var reviewError = DentalExamValidator.ValidateQuestionnaireReview(dto);
            if (reviewError != null)
            {
                return Task.FromResult<string?>(reviewError);
            }

            if (DentalExamValidator.IsSubsequentDiseasesSectionActive(dto))
            {
                var findingsError = DentalExamFindingValidator.ValidateFindings(dto.Findings, dto.PsrSelectedTeeth);
                if (findingsError != null)
                {
                    return Task.FromResult<string?>(findingsError);
                }
            }

            var denClassError = DentalExamValidator.ValidateDenClass(dto);
            if (denClassError != null)
            {
                return Task.FromResult<string?>(denClassError);
            }

            return Task.FromResult<string?>(null);
        }

        [RoleAttributeAuthorizeFromConfig("DentalExams_View")]
        public IActionResult DownloadXRayImage(string prefix, string fileName)
        {
            const string methodName = nameof(DownloadXRayImage);

            try
            {
                if (string.IsNullOrWhiteSpace(prefix) || string.IsNullOrWhiteSpace(fileName))
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Invalid download request", CLASSNAME, methodName);
                    return BadRequest("Invalid file download request.");
                }

                var file = _fileService.GetFile(XRayStationName, prefix, fileName);
                if (file == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, File not found: {FileName}", CLASSNAME, methodName, fileName);
                    return NotFound();
                }

                Response.Headers["Content-Disposition"] = $"inline; filename=\"{file.FileName}\"";
                return File(file.Bytes, file.ContentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception occurred while downloading file", CLASSNAME, methodName);
                return StatusCode(500, "Error while downloading file");
            }
        }
    }
}
