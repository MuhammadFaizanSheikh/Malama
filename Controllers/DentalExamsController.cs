using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Utilities;
using Malama.Attributes;
using Malama.Models;
using Malama.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

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

                var currentUserRoles = User.FindAll(ClaimTypes.Role)
                    .Select(c => c.Value)
                    .Where(v => !string.IsNullOrWhiteSpace(v))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(r => r)
                    .ToList();

                ViewBag.CurrentUserRoles = string.Join(Environment.NewLine, currentUserRoles);

                var currentUser = await _userManager.GetUserAsync(User);
                var currentUserDisplayName = await ResolveDentistDisplayNameAsync(currentUser);
                ViewBag.DentistSignatureDisplayName = !string.IsNullOrWhiteSpace(dentalExam.DentistSignatureName)
                    ? dentalExam.DentistSignatureName
                    : currentUserDisplayName;

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

                if (dto.DentistSignatureEntered && !dto.GoToVitalStation)
                {
                    dto.DentistSignatureName = await ResolveDentistSignatureNameForSaveAsync(dto.ServiceMembersChildId, user);
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
                    await _dentalExamService.SaveOrUpdateFromFormDataAsync(dto, user.UserName);
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

        private async Task<string> ResolveDentistSignatureNameForSaveAsync(long serviceMembersChildId, ApplicationUser user)
        {
            try
            {
                var existingExam = await _dentalExamService.GetByServiceMembersChildIdAsync(serviceMembersChildId);
                if (existingExam != null && !string.IsNullOrWhiteSpace(existingExam.DentistSignatureName))
                {
                    return existingExam.DentistSignatureName.Trim();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "{ClassName}, ResolveDentistSignatureNameForSaveAsync, Could not load existing dental exam for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, serviceMembersChildId);
            }

            return await ResolveDentistDisplayNameAsync(user);
        }

        private async Task<string> ResolveDentistDisplayNameAsync(ApplicationUser? user)
        {
            if (user == null)
            {
                return string.Empty;
            }

            if (!user.IsEventUser)
            {
                return user.UserName?.Trim() ?? string.Empty;
            }

            try
            {
                var staff = await _eventStaffService.GetEventStaffWithAttributesByUserId(user.Id);
                return FormatEventStaffDisplayName(staff);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning(
                    "{ClassName}, ResolveDentistDisplayNameAsync, EventStaff not found for UserId={UserId}. Falling back to UserName.",
                    CLASSNAME, user.Id);
                return user.UserName?.Trim() ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, ResolveDentistDisplayNameAsync, Failed to resolve EventStaff name for UserId={UserId}",
                    CLASSNAME, user.Id);
                return user.UserName?.Trim() ?? string.Empty;
            }
        }

        private static string FormatEventStaffDisplayName(EventStaff staff)
        {
            return string.Join(" ",
                new[] { staff.StaffFirstName, staff.StaffMiddleInitial, staff.StaffLastName }
                    .Where(part => !string.IsNullOrWhiteSpace(part))
                    .Select(part => part!.Trim()));
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
                var findingsError = DentalExamFindingValidator.ValidateFindings(dto.Findings);
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
