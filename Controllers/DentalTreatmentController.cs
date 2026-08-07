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
    public class DentalTreatmentController : Controller
    {
        private readonly IFileUploader _fileUploader;
        private readonly IDentalQuestionnaireService _dentalQuestionnaireService;
        private readonly IDentalXRayStationService _dentalXRayStationService;
        private readonly IDentalExamService _dentalExamService;
        private readonly IDentalTreatmentService _dentalTreatmentService;
        private readonly IVitalStationService _vitalStationService;
        private readonly IEventStaffService _eventStaffService;
        private readonly IFileUploadDownloadService _fileService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DentalTreatmentController> _logger;
        private const string CLASSNAME = "DentalTreatmentController";
        private const string XRayStationName = "DentalXRay";

        public DentalTreatmentController(
            ILogger<DentalTreatmentController> logger,
            IFileUploader fileUploader,
            IDentalQuestionnaireService dentalQuestionnaireService,
            IDentalXRayStationService dentalXRayStationService,
            IDentalExamService dentalExamService,
            IDentalTreatmentService dentalTreatmentService,
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
            _dentalTreatmentService = dentalTreatmentService;
            _vitalStationService = vitalStationService;
            _eventStaffService = eventStaffService;
            _fileService = fileService;
            _userManager = userManager;
        }

        [RoleAttributeAuthorizeFromConfig("DentalTreatment_View")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            const string methodName = nameof(Index);
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

                var data = await _fileUploader.GetDentalTreatmentsByEventIdAsync(parsedEventId);

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Retrieved {Count} records for EventId={EventId}",
                    CLASSNAME, methodName, data.Count, eventId);

                var summary = new Dictionary<string, int>
                {
                    ["Total"] = data.Count
                };

                ViewBag.Summary = summary;
                ViewBag.EventId = eventId;

                return View("Index", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while loading Dental Treatment index page",
                    CLASSNAME, methodName);

                ViewBag.EventIdList = new List<SelectListItem>();
                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = ex.Message;

                return View();
            }
        }

        [RoleAttributeAuthorizeFromConfig("DentalTreatment_View")]
        [HttpGet]
        public async Task<IActionResult> DentalTreatmentStation(long serviceMembersChildId)
        {
            const string methodName = nameof(DentalTreatmentStation);
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

                var dentalExam = await _dentalExamService.GetByServiceMembersChildIdAsync(serviceMembersChildId);
                if (dentalExam == null
                    || !string.Equals(dentalExam.Status, AppConstants.Status.Completed, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(dentalExam.DenClass, DentalExamDenClass.Class3, StringComparison.OrdinalIgnoreCase))
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Not Eligible";
                    TempData["ResponseMessage"] = "This service member is not eligible for Dental Treatment. Dental Exam must be Completed with DEN Class 3.";
                    return RedirectToAction(nameof(Index));
                }

                var dentalTreatment = await _dentalTreatmentService.GetByServiceMembersChildIdAsync(serviceMembersChildId);
                ApplyTreatmentSelectedTeethToExamChart(dentalExam, dentalTreatment);

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
                ViewBag.CurrentUserDisplayName = currentUser != null
                    ? await DentalExamSignatureHelper.ResolveDisplayNameAsync(currentUser, _eventStaffService, _logger)
                    : string.Empty;
                ViewBag.CurrentUserId = currentUser?.Id ?? string.Empty;

                var treatmentStaffUserIds = CollectTreatmentStaffUserIds(dentalTreatment);
                var examinerUserIds = (dentalExam.Findings ?? Enumerable.Empty<DentalExamFinding>())
                    .SelectMany(f => new[] { f.ExaminationAddedBy, f.ExaminationUpdatedBy });
                ViewBag.ExaminerNamesByUserId = await DentalExamSignatureHelper.ResolveDisplayNamesByUserIdAsync(
                    examinerUserIds.Concat(treatmentStaffUserIds),
                    _userManager,
                    _eventStaffService,
                    _logger);

                var pageModel = new DentalTreatmentStationPageViewModel
                {
                    ServiceMember = result.ServiceMembersChild,
                    Questionnaire = questionnaire,
                    XRayStation = xRayStation,
                    DentalExam = dentalExam,
                    DentalTreatment = dentalTreatment
                };

                return View(pageModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while loading Dental Treatment station page",
                    CLASSNAME, methodName);
                throw;
            }
        }

        [RoleAttributeAuthorizeFromConfig("DentalTreatment_Save")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDentalTreatmentStation(DentalTreatmentStationSaveDto dto)
        {
            const string methodName = nameof(SaveDentalTreatmentStation);
            _logger.LogInformation(
                "{ClassName}, {MethodName}, Called. ServiceMembersChildId={ServiceMembersChildId}",
                CLASSNAME, methodName, dto.ServiceMembersChildId);

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
                if (serviceMemberResult.ServiceMembersChild == null)
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = "Service member not found.";
                    return RedirectToAction(nameof(Index));
                }

                var dentalExam = await _dentalExamService.GetByServiceMembersChildIdAsync(dto.ServiceMembersChildId);
                if (dentalExam == null
                    || !string.Equals(dentalExam.Status, AppConstants.Status.Completed, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(dentalExam.DenClass, DentalExamDenClass.Class3, StringComparison.OrdinalIgnoreCase))
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Not Eligible";
                    TempData["ResponseMessage"] = "This service member is not eligible for Dental Treatment.";
                    return RedirectToAction(nameof(Index));
                }

                dto.DentalExamId = dentalExam.Id;
                dto.Findings = DentalTreatmentJson.ParseList<DentalTreatmentFindingFormDto>(dto.FindingsJson);
                dto.AnesthesiaRecords = DentalTreatmentJson.ParseList<DentalTreatmentAnesthesiaDto>(dto.AnesthesiaJson);
                dto.Prescriptions = DentalTreatmentJson.ParseList<DentalTreatmentPrescriptionDto>(dto.PrescriptionsJson);
                dto.OverallNotes = DentalTreatmentJson.ParseList<DentalTreatmentOverallNoteDto>(dto.OverallNotesJson);
                dto.PsrSelectedTeeth = DentalTreatmentValidator.NormalizeSelectedTeeth(dto.PsrSelectedTeeth);

                var validationError = DentalTreatmentValidator.ValidateSaveDto(dto, dentalExam);
                if (!string.IsNullOrWhiteSpace(validationError))
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = validationError;
                    return RedirectToAction(nameof(DentalTreatmentStation), new { serviceMembersChildId = dto.ServiceMembersChildId });
                }

                await _dentalTreatmentService.SaveOrUpdateFromFormDataAsync(dto, user.UserName ?? user.Id, user.Id);

                TempData["ResponseStatus"] = "success";
                TempData["ResponseTitle"] = "Success";
                TempData["ResponseMessage"] = "Dental Treatment record saved successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while saving Dental Treatment record",
                    CLASSNAME, methodName);

                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = ex.Message;
                return RedirectToAction(nameof(DentalTreatmentStation), new { serviceMembersChildId = dto.ServiceMembersChildId });
            }
        }

        [RoleAttributeAuthorizeFromConfig("DentalTreatment_View")]
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

        private static void ApplyTreatmentSelectedTeethToExamChart(DentalExam dentalExam, DentalTreatment? dentalTreatment)
        {
            if (dentalTreatment == null)
            {
                return;
            }

            dentalExam.SelectedTeeth = dentalTreatment.SelectedTeeth
                .Select(t => new DentalExamSelectedTooth
                {
                    DentalExamId = dentalExam.Id,
                    ToothNumber = t.ToothNumber
                })
                .OrderBy(t => t.ToothNumber)
                .ToList();
        }

        private static IEnumerable<string?> CollectTreatmentStaffUserIds(DentalTreatment? dentalTreatment)
        {
            if (dentalTreatment == null)
            {
                return Enumerable.Empty<string?>();
            }

            return (dentalTreatment.Findings ?? Enumerable.Empty<DentalTreatmentFinding>())
                .Select(f => f.DentistProfessional)
                .Concat((dentalTreatment.OverallNotes ?? Enumerable.Empty<DentalTreatmentOverallNote>())
                    .Select(n => n.Dentist))
                .Concat((dentalTreatment.Prescriptions ?? Enumerable.Empty<DentalTreatmentPrescription>())
                    .Select(p => p.PrescribedBy));
        }
    }
}
