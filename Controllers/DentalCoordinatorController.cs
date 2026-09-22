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
    public class DentalCoordinatorController : Controller
    {
        private readonly IFileUploader _fileUploader;
        private readonly IDentalQuestionnaireService _dentalQuestionnaireService;
        private readonly IVitalStationService _vitalStationService;
        private readonly IDentalXRayStationService _dentalXRayStationService;
        private readonly IDentalCoordinatorStationService _dentalCoordinatorStationService;
        private readonly IDentalExamService _dentalExamService;
        private readonly IDentalTreatmentService _dentalTreatmentService;
        private readonly ITreatmentConsentService _treatmentConsentService;
        private readonly ITreatmentConsentPdfGenerator _treatmentConsentPdfGenerator;
        private readonly IEventStaffService _eventStaffService;
        private readonly IEventManagementService _eventManagementService;
        private readonly IFileUploadDownloadService _fileService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DentalCoordinatorController> _logger;
        private const string CLASSNAME = "DentalCoordinatorController";
        private const string XRayStationName = "DentalXRay";

        public DentalCoordinatorController(
            ILogger<DentalCoordinatorController> logger,
            IFileUploader fileUploader,
            IDentalQuestionnaireService dentalQuestionnaireService,
            IVitalStationService vitalStationService,
            IDentalXRayStationService dentalXRayStationService,
            IDentalCoordinatorStationService dentalCoordinatorStationService,
            IDentalExamService dentalExamService,
            IDentalTreatmentService dentalTreatmentService,
            ITreatmentConsentService treatmentConsentService,
            ITreatmentConsentPdfGenerator treatmentConsentPdfGenerator,
            IEventStaffService eventStaffService,
            IEventManagementService eventManagementService,
            IFileUploadDownloadService fileService,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _fileUploader = fileUploader;
            _dentalQuestionnaireService = dentalQuestionnaireService;
            _vitalStationService = vitalStationService;
            _dentalXRayStationService = dentalXRayStationService;
            _dentalCoordinatorStationService = dentalCoordinatorStationService;
            _dentalExamService = dentalExamService;
            _dentalTreatmentService = dentalTreatmentService;
            _treatmentConsentService = treatmentConsentService;
            _treatmentConsentPdfGenerator = treatmentConsentPdfGenerator;
            _eventStaffService = eventStaffService;
            _eventManagementService = eventManagementService;
            _fileService = fileService;
            _userManager = userManager;
        }

        [HttpGet]
        [RoleAttributeAuthorizeFromConfig("TreatmentCoordinator_View")]
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

                var data = await _fileUploader.GetDentalCoordinatorByEventIdAsync(parsedEventId);

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Retrieved {Count} records for EventId={EventId}",
                    CLASSNAME, methodName, data.Count, eventId);

                var completedCount = data.Count(sm =>
                    string.Equals(
                        sm.DentalTreatmentRecord?.Status?.Trim(),
                        AppConstants.Status.Completed,
                        StringComparison.OrdinalIgnoreCase));
                var pendingCount = data.Count - completedCount;

                ViewBag.Summary = new Dictionary<string, int>
                {
                    ["Total"] = data.Count,
                    ["Pending"] = pendingCount,
                    ["Completed"] = completedCount
                };
                ViewBag.EventId = eventId;

                return View("Index", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while loading Treatment Coordinator index page",
                    CLASSNAME, methodName);

                ViewBag.EventIdList = new List<SelectListItem>();
                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = ex.Message;

                return View();
            }
        }

        [HttpGet]
        [RoleAttributeAuthorizeFromConfig("TreatmentCoordinator_View")]
        public async Task<IActionResult> DentalCoordinatorStation(long serviceMembersChildId)
        {
            const string methodName = nameof(DentalCoordinatorStation);
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            try
            {
                var built = await BuildDentalCoordinatorStationPageAsync(serviceMembersChildId);
                if (built.Redirect != null)
                {
                    return built.Redirect;
                }

                return View(built.PageModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while loading Treatment Coordinator station page",
                    CLASSNAME, methodName);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(104857600)]
        [RoleAttributeAuthorizeFromConfig("TreatmentCoordinator_Save")]
        public async Task<IActionResult> SaveDentalCoordinatorStation(DentalCoordinatorStationSaveDto dto)
        {
            const string methodName = nameof(SaveDentalCoordinatorStation);
            DentalQuestionnaireFormBinder.BindHealthConditions(dto, Request.Form);
            dto.PanoXRayAcknowledged = FormCheckboxHelper.IsChecked(Request.Form, "PanoXRayAcknowledged");

            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

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

                var existingExam = await _dentalExamService.GetByServiceMembersChildIdAsync(dto.ServiceMembersChildId);
                if (!DentalStationEligibilityHelper.IsEligibleForTreatmentCoordinator(serviceMember, existingExam))
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Not Eligible";
                    TempData["ResponseMessage"] = "This service member is not eligible for Treatment Coordinator.";
                    return RedirectToAction(nameof(Index));
                }

                if (DentalStationEligibilityHelper.RequiresDentalExamBeforeCoordinator(serviceMember, existingExam))
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Dental Exam Required";
                    TempData["ResponseMessage"] = "Complete Dental Exam first before saving Treatment Coordinator.";
                    return await RedisplayDentalCoordinatorStationAsync(dto);
                }

                if (DentalXRayStationService.IsNeeded(serviceMember.PanoNeeded))
                {
                    dto.PanoXRayAcknowledged = true;
                }

                var validationError = DentalXRayStationSaveValidator.Validate(dto, serviceMember, _dentalQuestionnaireService);
                if (!string.IsNullOrWhiteSpace(validationError))
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = validationError;
                    return await RedisplayDentalCoordinatorStationAsync(dto);
                }

                var retainedNames = Request.Form["RetainedDocumentFileNames"]
                    .Where(v => !string.IsNullOrWhiteSpace(v))
                    .Select(v => v!.Trim())
                    .ToList();
                dto.RetainedDocumentFileNames = retainedNames;

                var documentsError = TreatmentCoordinatorDocumentsValidator.Validate(
                    dto.TreatmentCoordinatorDocuments,
                    dto.RetainedDocumentFileNames);
                if (!string.IsNullOrWhiteSpace(documentsError))
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = documentsError;
                    return await RedisplayDentalCoordinatorStationAsync(dto);
                }

                var findings = DentalFindingBinder.ParseFromJson(dto.FindingsJson);
                var findingsError = DentalFindingValidator.ValidateFindings(findings);
                if (!string.IsNullOrWhiteSpace(findingsError))
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = findingsError;
                    return await RedisplayDentalCoordinatorStationAsync(dto);
                }

                var appointments = TreatmentCoordinatorAppointmentHelper.ParseAppointmentsJson(dto.AppointmentsJson);
                var appointmentsError = TreatmentCoordinatorAppointmentHelper.Validate(appointments, findings);
                if (!string.IsNullOrWhiteSpace(appointmentsError))
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = appointmentsError;
                    return await RedisplayDentalCoordinatorStationAsync(dto);
                }

                long eventStaffId = 0;
                try
                {
                    var eventStaff = await _eventStaffService.GetEventStaffWithAttributesByUserId(user.Id);
                    eventStaffId = eventStaff?.Id ?? 0;
                }
                catch (Exception staffEx)
                {
                    _logger.LogWarning(staffEx,
                        "{ClassName}, {MethodName}, Failed to resolve EventStaff for UserId={UserId}",
                        CLASSNAME, methodName, user.Id);
                }

                if (eventStaffId <= 0)
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = "Treatment Coordinator staff profile was not found for the current user.";
                    return await RedisplayDentalCoordinatorStationAsync(dto);
                }

                var saveResult = await _dentalCoordinatorStationService.SaveStationAsync(
                    dto,
                    serviceMember,
                    user.UserName ?? user.Id,
                    user.Id,
                    eventStaffId);

                if (!saveResult.Success)
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = saveResult.ErrorTitle ?? "Error";
                    TempData["ResponseMessage"] = saveResult.ErrorMessage ?? "Save failed.";
                    return await RedisplayDentalCoordinatorStationAsync(dto);
                }

                TempData["ResponseStatus"] = "success";
                TempData["ResponseTitle"] = "Success";
                TempData["ResponseMessage"] = "Treatment Coordinator record saved successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while saving Treatment Coordinator station",
                    CLASSNAME, methodName);

                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = ex.GetBaseException().Message;

                if (dto?.ServiceMembersChildId > 0)
                {
                    return await RedisplayDentalCoordinatorStationAsync(dto);
                }

                return RedirectToAction(nameof(Index));
            }
        }

        private async Task<IActionResult> RedisplayDentalCoordinatorStationAsync(DentalCoordinatorStationSaveDto dto)
        {
            var built = await BuildDentalCoordinatorStationPageAsync(dto.ServiceMembersChildId);
            if (built.Redirect != null)
            {
                return built.Redirect;
            }

            await ApplyPostedSaveDtoToStationPageAsync(built.PageModel!, dto);
            return View(nameof(DentalCoordinatorStation), built.PageModel);
        }

        private async Task<(IActionResult? Redirect, DentalCoordinatorStationPageViewModel? PageModel)> BuildDentalCoordinatorStationPageAsync(
            long serviceMembersChildId)
        {
            if (serviceMembersChildId <= 0)
            {
                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Invalid Request";
                TempData["ResponseMessage"] = "Service member is required.";
                return (RedirectToAction(nameof(Index)), null);
            }

            var result = await _fileUploader.GetServiceMemberChildWithEventIdAsync(serviceMembersChildId);
            if (result.ServiceMembersChild == null)
            {
                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Not Found";
                TempData["ResponseMessage"] = "Service member not found.";
                return (RedirectToAction(nameof(Index)), null);
            }

            var dentalExamForEligibility = await _dentalExamService.GetByServiceMembersChildIdAsync(serviceMembersChildId);
            if (!DentalStationEligibilityHelper.IsEligibleForTreatmentCoordinator(
                    result.ServiceMembersChild,
                    dentalExamForEligibility))
            {
                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Not Eligible";
                TempData["ResponseMessage"] = "This service member is not eligible for Treatment Coordinator.";
                return (RedirectToAction(nameof(Index)), null);
            }

            var requiresDentalExamFirst = DentalStationEligibilityHelper.RequiresDentalExamBeforeCoordinator(
                result.ServiceMembersChild,
                dentalExamForEligibility);
            ViewBag.RequiresDentalExamFirst = requiresDentalExamFirst;
            ViewBag.CoordinatorPageReadOnly = requiresDentalExamFirst;

            ViewBag.EventId = result.EventId;
            ViewBag.EventAppointmentMinDate = string.Empty;
            ViewBag.EventAppointmentMaxDate = string.Empty;
            ViewBag.EventAppointmentDayWindowsJson = "[]";
            ViewBag.AssignableDentists = new List<TreatmentCoordinatorAssignableDentistDto>();

            try
            {
                if (result.EventId > 0)
                {
                    var appointmentWindow = await _eventManagementService.GetEventAppointmentWindowAsync(result.EventId);
                    ViewBag.EventAppointmentMinDate = appointmentWindow.MinDate;
                    ViewBag.EventAppointmentMaxDate = appointmentWindow.MaxDate;
                    ViewBag.EventAppointmentDayWindowsJson = System.Text.Json.JsonSerializer.Serialize(
                        appointmentWindow.Days,
                        new System.Text.Json.JsonSerializerOptions
                        {
                            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                        });

                    ViewBag.AssignableDentists = await _eventStaffService
                        .GetTreatmentCoordinatorDentistsByEventIdAsync(result.EventId);
                }
                else
                {
                    ViewBag.AssignableDentists = new List<TreatmentCoordinatorAssignableDentistDto>();
                }
            }
            catch (Exception eventEx)
            {
                _logger.LogWarning(eventEx,
                    "{ClassName}, BuildDentalCoordinatorStationPageAsync, Failed to load event date range/dentists for EventId={EventId}",
                    CLASSNAME, result.EventId);
                ViewBag.AssignableDentists ??= new List<TreatmentCoordinatorAssignableDentistDto>();
            }

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
                    "{ClassName}, BuildDentalCoordinatorStationPageAsync, Failed to load vital station for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, serviceMembersChildId);

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

            xRayStation.ServiceMembersChild ??= result.ServiceMembersChild;

            var dentalExam = dentalExamForEligibility
                ?? new DentalExam { ServiceMembersChildId = serviceMembersChildId };

            var dentalTreatment = await _dentalTreatmentService.GetByServiceMembersChildIdAsync(serviceMembersChildId);

            var formSelection = await _treatmentConsentService.GetFormSelectionAsync(serviceMembersChildId);
            var consentFormStatuses = TreatmentConsentHelper.BuildCoordinatorConsentFormStatusItems(formSelection);

            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.TreatmentCoordinatorDisplayName = currentUser != null
                ? await DentalExamSignatureHelper.ResolveDisplayNameAsync(currentUser, _eventStaffService, _logger)
                : string.Empty;
            ViewBag.CurrentUserId = currentUser?.Id ?? string.Empty;
            ViewBag.CurrentUserDisplayName = ViewBag.TreatmentCoordinatorDisplayName;
            ViewBag.ExaminerNamesByUserId = await DentalExamSignatureHelper.ResolveExaminerNamesByUserIdAsync(
                dentalExam.Findings,
                _userManager,
                _eventStaffService,
                _logger);

            var appointmentsJson = TreatmentCoordinatorAppointmentHelper.SerializeAppointments(
                TreatmentCoordinatorAppointmentHelper.ToJsonDtos(dentalTreatment?.CoordinatorAppointments));
            ViewBag.AppointmentsJson = appointmentsJson;

            var documents = TreatmentCoordinatorDocumentFileSaveCoordinator.ParseDocumentsJson(
                dentalTreatment?.DocumentsJson);
            ViewBag.CoordinatorDocumentsJson = TreatmentCoordinatorDocumentFileSaveCoordinator.SerializeDocuments(documents);

            if (dentalTreatment?.TreatmentCoordinatorEventStaffId > 0)
            {
                try
                {
                    var coordinatorStaff = await _eventStaffService.GetEventStaffWithoutIncludeById(
                        dentalTreatment.TreatmentCoordinatorEventStaffId.Value);
                    if (coordinatorStaff != null)
                    {
                        var name = $"{coordinatorStaff.StaffFirstName} {coordinatorStaff.StaffLastName}".Trim();
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            ViewBag.TreatmentCoordinatorDisplayName = name;
                        }
                    }
                }
                catch (Exception staffEx)
                {
                    _logger.LogWarning(staffEx,
                        "{ClassName}, BuildDentalCoordinatorStationPageAsync, Failed to resolve saved TreatmentCoordinatorEventStaffId={EventStaffId}",
                        CLASSNAME, dentalTreatment.TreatmentCoordinatorEventStaffId);
                }
            }

            var pageModel = new DentalCoordinatorStationPageViewModel
            {
                ServiceMember = result.ServiceMembersChild,
                Questionnaire = questionnaire,
                XRayStation = xRayStation,
                DentalExam = dentalExam,
                DentalTreatment = dentalTreatment,
                HasQuestionnaire = questionnaire.Id > 0,
                ConsentFormStatuses = consentFormStatuses
            };

            return (null, pageModel);
        }

        private async Task ApplyPostedSaveDtoToStationPageAsync(
            DentalCoordinatorStationPageViewModel pageModel,
            DentalCoordinatorStationSaveDto dto)
        {
            pageModel.Questionnaire = _dentalQuestionnaireService.MapFormDataToEntity(dto, pageModel.Questionnaire);
            pageModel.HasQuestionnaire = pageModel.HasQuestionnaire || pageModel.Questionnaire.Id > 0;

            pageModel.XRayStation = _dentalXRayStationService.MapSaveDtoToEntity(dto, pageModel.XRayStation);
            pageModel.XRayStation.ServiceMembersChild ??= pageModel.ServiceMember;

            var exam = pageModel.DentalExam ?? new DentalExam { ServiceMembersChildId = dto.ServiceMembersChildId };
            exam.ServiceMembersChildId = dto.ServiceMembersChildId;
            if (dto.DentalExamId > 0)
            {
                exam.Id = dto.DentalExamId;
            }

            var clinicalOwnedByDentalExam = exam.Id > 0
                && string.Equals(exam.Source, DentalExamSources.DentalExam, StringComparison.OrdinalIgnoreCase);
            if (!clinicalOwnedByDentalExam)
            {
                exam.PsrUpperRight = dto.PsrUpperRight?.Trim();
                exam.PsrUpperAnterior = dto.PsrUpperAnterior?.Trim();
                exam.PsrUpperLeft = dto.PsrUpperLeft?.Trim();
                exam.PsrLowerRight = dto.PsrLowerRight?.Trim();
                exam.PsrLowerAnterior = dto.PsrLowerAnterior?.Trim();
                exam.PsrLowerLeft = dto.PsrLowerLeft?.Trim();
                exam.PsrCarrierRisk = dto.PsrCarrierRisk?.Trim();
                exam.SoftTissuesWnl = dto.SoftTissuesWnl?.Trim();
                exam.SoftTissuesConditionDetail = exam.SoftTissuesWnl != null
                    && exam.SoftTissuesWnl.Equals(DentalExamPsr.SoftTissuesWnlNo, StringComparison.OrdinalIgnoreCase)
                    ? dto.SoftTissuesConditionDetail?.Trim()
                    : null;
                exam.DenClass = dto.DenClass?.Trim();
                exam.DenClassReasonComments = dto.DenClassReasonComments?.Trim();
                exam.PanoXRayAcknowledged = dto.PanoXRayAcknowledged;

                var selectedTeeth = (dto.PsrSelectedTeeth ?? new List<int>())
                    .Where(t => t >= 1 && t <= 32)
                    .Distinct()
                    .OrderBy(t => t)
                    .ToList();
                exam.SelectedTeeth = selectedTeeth
                    .Select(toothNumber => new DentalExamSelectedTooth
                    {
                        DentalExamId = exam.Id,
                        ToothNumber = toothNumber
                    })
                    .ToList();
            }

            var postedFindings = DentalFindingBinder.ParseFromJson(dto.FindingsJson);
            exam.Findings = postedFindings
                .Select((finding, index) =>
                {
                    var entity = DentalFindingMapper.ToEntity(finding, exam.Id, index);
                    entity.Id = finding.Id;
                    if (string.IsNullOrWhiteSpace(entity.Source))
                    {
                        entity.Source = finding.Source?.Trim();
                    }
                    return entity;
                })
                .ToList();

            pageModel.DentalExam = exam;

            pageModel.DentalTreatment ??= new DentalTreatment
            {
                ServiceMembersChildId = dto.ServiceMembersChildId,
                Status = AppConstants.Status.Pending
            };
            pageModel.DentalTreatment.TreatmentCoordinatorComments = dto.TreatmentCoordinatorComments;

            ViewBag.AppointmentsJson = string.IsNullOrWhiteSpace(dto.AppointmentsJson)
                ? "[]"
                : dto.AppointmentsJson;

            var existingDocuments = TreatmentCoordinatorDocumentFileSaveCoordinator.ParseDocumentsJson(
                pageModel.DentalTreatment.DocumentsJson);
            var retained = new HashSet<string>(
                dto.RetainedDocumentFileNames ?? new List<string>(),
                StringComparer.OrdinalIgnoreCase);
            var keptDocuments = existingDocuments
                .Where(d => !string.IsNullOrWhiteSpace(d.FileName) && retained.Contains(d.FileName))
                .ToList();
            ViewBag.CoordinatorDocumentsJson = TreatmentCoordinatorDocumentFileSaveCoordinator.SerializeDocuments(keptDocuments);

            ViewBag.ExaminerNamesByUserId = await DentalExamSignatureHelper.ResolveExaminerNamesByUserIdAsync(
                exam.Findings,
                _userManager,
                _eventStaffService,
                _logger);
        }

        [HttpGet]
        [RoleAttributeAuthorizeFromConfig("TreatmentCoordinator_View")]
        public async Task<IActionResult> PreviewQuestionnairePdf(long serviceMembersChildId)
        {
            const string methodName = nameof(PreviewQuestionnairePdf);
            try
            {
                if (serviceMembersChildId <= 0)
                {
                    return BadRequest("Service member is required.");
                }

                var result = await _fileUploader.GetServiceMemberChildWithEventIdAsync(serviceMembersChildId);
                if (result.ServiceMembersChild == null)
                {
                    return NotFound("Service member not found.");
                }

                var questionnaire = await _dentalQuestionnaireService.GetByServiceMembersChildIdAsync(serviceMembersChildId);
                if (questionnaire == null || questionnaire.Id <= 0)
                {
                    return NotFound("Questionnaire is not available for this service member.");
                }

                var pdfBytes = _treatmentConsentPdfGenerator.GenerateQuestionnairePdf(
                    result.ServiceMembersChild,
                    questionnaire);

                Response.Headers["Content-Disposition"] = "inline; filename=\"DA5570-Questionnaire.pdf\"";
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Failed for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, serviceMembersChildId);
                return StatusCode(500, "Error while generating questionnaire PDF.");
            }
        }

        [HttpGet]
        [RoleAttributeAuthorizeFromConfig("TreatmentCoordinator_View")]
        public async Task<IActionResult> PreviewConsentFormPdf(
            long serviceMembersChildId,
            string formKind,
            long eventStaffId)
        {
            const string methodName = nameof(PreviewConsentFormPdf);
            try
            {
                if (serviceMembersChildId <= 0 || eventStaffId <= 0 || string.IsNullOrWhiteSpace(formKind))
                {
                    return BadRequest("Service member, form kind, and dentist are required.");
                }

                var kind = formKind.Trim().ToLowerInvariant();
                if (kind is not ("dental-treatment" or "oral-surgery"))
                {
                    return BadRequest("Invalid form kind.");
                }

                var result = await _fileUploader.GetServiceMemberChildWithEventIdAsync(serviceMembersChildId);
                if (result.ServiceMembersChild == null)
                {
                    return NotFound("Service member not found.");
                }

                var selection = await _treatmentConsentService.GetFormSelectionAsync(serviceMembersChildId);
                byte[] pdfBytes;
                string fileName;

                if (kind == "dental-treatment")
                {
                    if (!selection.IncludeDentalTreatmentConsent
                        || !(selection.DentalTreatmentDentistEventStaffIds ?? new List<long>()).Contains(eventStaffId))
                    {
                        return NotFound("Dental treatment consent form not found for this dentist.");
                    }

                    var form = (selection.DentalTreatmentForms ?? new List<TreatmentConsentDentalTreatmentFormDto>())
                        .Where(f => f != null && f.EventStaffId == eventStaffId)
                        .LastOrDefault()
                        ?? new TreatmentConsentDentalTreatmentFormDto { EventStaffId = eventStaffId };

                    var signatureBytes = TryLoadConsentSignature(
                        TreatmentConsentFileSaveCoordinator.DentalTreatmentPrefix,
                        form.SignatureFileName);
                    pdfBytes = _treatmentConsentPdfGenerator.GenerateDentalTreatmentConsentPdf(
                        result.ServiceMembersChild,
                        form,
                        signatureBytes);
                    fileName = $"Dental-Treatment-Consent-{eventStaffId}.pdf";
                }
                else
                {
                    if (!selection.IncludeOralSurgeryForm
                        || !(selection.OralSurgeryDentistEventStaffIds ?? new List<long>()).Contains(eventStaffId))
                    {
                        return NotFound("Oral surgery consent form not found for this dentist.");
                    }

                    var form = (selection.OralSurgeryForms ?? new List<TreatmentConsentOralSurgeryFormDto>())
                        .Where(f => f != null && f.EventStaffId == eventStaffId)
                        .LastOrDefault()
                        ?? new TreatmentConsentOralSurgeryFormDto { EventStaffId = eventStaffId };

                    var signatureBytes = TryLoadConsentSignature(
                        TreatmentConsentFileSaveCoordinator.OralSurgeryPrefix,
                        form.SignatureFileName);
                    pdfBytes = _treatmentConsentPdfGenerator.GenerateOralSurgeryConsentPdf(
                        result.ServiceMembersChild,
                        form,
                        signatureBytes);
                    fileName = $"Oral-Surgery-Consent-{eventStaffId}.pdf";
                }

                Response.Headers["Content-Disposition"] = $"inline; filename=\"{fileName}\"";
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Failed for ServiceMembersChildId={ServiceMembersChildId}, FormKind={FormKind}, EventStaffId={EventStaffId}",
                    CLASSNAME, methodName, serviceMembersChildId, formKind, eventStaffId);
                return StatusCode(500, "Error while generating consent form PDF.");
            }
        }

        private byte[]? TryLoadConsentSignature(string prefix, string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }

            try
            {
                var file = _fileService.GetFile(
                    TreatmentConsentFileSaveCoordinator.StationName,
                    prefix,
                    fileName);
                return file?.Bytes;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "{ClassName}, Failed to load consent signature Prefix={Prefix}, File={File}",
                    CLASSNAME, prefix, fileName);
                return null;
            }
        }

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

        [HttpGet]
        [RoleAttributeAuthorizeFromConfig("TreatmentCoordinator_View")]
        public IActionResult DownloadCoordinatorDocument(string fileName)
        {
            const string methodName = nameof(DownloadCoordinatorDocument);

            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    return BadRequest("Invalid file download request.");
                }

                var safeName = Path.GetFileName(fileName);
                if (string.IsNullOrWhiteSpace(safeName)
                    || !string.Equals(safeName, fileName, StringComparison.Ordinal))
                {
                    return BadRequest("Invalid file name.");
                }

                var file = _fileService.GetFile(
                    TreatmentCoordinatorDocumentFileSaveCoordinator.StationName,
                    TreatmentCoordinatorDocumentFileSaveCoordinator.DocumentPrefix,
                    safeName);
                if (file == null || file.Bytes == null)
                {
                    return NotFound();
                }

                Response.Headers["Content-Disposition"] = $"inline; filename=\"{file.FileName}\"";
                return File(file.Bytes, file.ContentType ?? "application/pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Failed for File={File}",
                    CLASSNAME, methodName, fileName);
                return StatusCode(500, "Error while downloading document");
            }
        }
    }
}
