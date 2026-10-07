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
        private readonly ITreatmentConsentService _treatmentConsentService;
        private readonly ITreatmentConsentPdfGenerator _treatmentConsentPdfGenerator;
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
            ITreatmentConsentService treatmentConsentService,
            ITreatmentConsentPdfGenerator treatmentConsentPdfGenerator,
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
            _treatmentConsentService = treatmentConsentService;
            _treatmentConsentPdfGenerator = treatmentConsentPdfGenerator;
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

                var user = await _userManager.GetUserAsync(User);
                var eventStaffId = user != null
                    ? await _dentalTreatmentService.TryGetEventStaffIdForUserAsync(user.Id)
                    : null;

                var data = eventStaffId.HasValue
                    ? await _fileUploader.GetDentalTreatmentsByEventIdAsync(parsedEventId, eventStaffId.Value)
                    : new List<ServiceMembersChild>();

                var dentistStatuses = eventStaffId.HasValue && user != null && data.Count > 0
                    ? await _dentalTreatmentService.GetDentistTreatmentStatusesAsync(
                        data.Select(c => c.Id).ToList(),
                        eventStaffId.Value,
                        user.Id)
                    : new Dictionary<long, string>();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Retrieved {Count} records for EventId={EventId}, EventStaffId={EventStaffId}",
                    CLASSNAME, methodName, data.Count, eventId, eventStaffId);

                var summary = new Dictionary<string, int>
                {
                    ["Total"] = data.Count,
                    ["Pending"] = data.Count(x =>
                        !dentistStatuses.TryGetValue(x.Id, out var dentistStatus)
                        || !string.Equals(dentistStatus, AppConstants.Status.Completed, StringComparison.OrdinalIgnoreCase)),
                    ["Completed"] = data.Count(x =>
                        dentistStatuses.TryGetValue(x.Id, out var dentistStatus)
                        && string.Equals(dentistStatus, AppConstants.Status.Completed, StringComparison.OrdinalIgnoreCase))
                };

                ViewBag.Summary = summary;
                ViewBag.DentistStatusByChildId = dentistStatuses;
                ViewBag.EventId = eventId;
                ViewBag.AuditDisplayNamesByUserId = await DentalExamSignatureHelper.ResolveDisplayNamesByUserIdAsync(
                    data.Select(c => c.DentalExamRecord?.UpdatedBy ?? c.DentalExamRecord?.AddedBy),
                    _userManager,
                    _eventStaffService,
                    _logger);

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

                var currentUser = await _userManager.GetUserAsync(User);
                var eventStaffId = currentUser != null
                    ? await _dentalTreatmentService.TryGetEventStaffIdForUserAsync(currentUser.Id)
                    : null;
                var eventManagementId = result.EventId > 0
                    ? result.EventId
                    : DentalExamSignatureHelper.TryResolveEventManagementId(User, HttpContext.Session, result.EventId) ?? 0;

                if (!eventStaffId.HasValue
                    || !await _dentalTreatmentService.IsEligibleForDentalTreatmentAsync(
                        serviceMembersChildId,
                        eventManagementId,
                        eventStaffId.Value))
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Not Eligible";
                    TempData["ResponseMessage"] = "This service member is not eligible for Dental Treatment for the current dentist.";
                    return RedirectToAction(nameof(Index));
                }

                var dentistAppointmentGroups = await _dentalTreatmentService.GetDentistAppointmentFindingGroupsAsync(
                    serviceMembersChildId,
                    eventStaffId.Value);
                var assignedExamFindingIds = dentistAppointmentGroups
                    .SelectMany(g => g.ExamFindingIds ?? new List<long>())
                    .Where(id => id > 0)
                    .ToHashSet();

                var dentalExam = await _dentalExamService.GetByServiceMembersChildIdAsync(serviceMembersChildId)
                    ?? new DentalExam { ServiceMembersChildId = serviceMembersChildId };
                var sharedClinical = await _dentalExamService.GetSharedClinicalByServiceMembersChildIdAsync(serviceMembersChildId);

                var dentalTreatment = await _dentalTreatmentService.GetByServiceMembersChildIdAsync(serviceMembersChildId);
                ApplyTreatmentSelectedTeethToSharedClinical(sharedClinical, dentalTreatment);

                ViewBag.EventId = result.EventId;
                ViewBag.AssignedExamFindingIds = assignedExamFindingIds;
                ViewBag.DentistAppointmentGroups = dentistAppointmentGroups;

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
                var examinerUserIds = sharedClinical.Findings
                    .SelectMany(f => new[] { f.ExaminationAddedBy, f.ExaminationUpdatedBy });
                ViewBag.ExaminerNamesByUserId = await DentalExamSignatureHelper.ResolveDisplayNamesByUserIdAsync(
                    examinerUserIds.Concat(treatmentStaffUserIds),
                    _userManager,
                    _eventStaffService,
                    _logger);

                var formSelection = await _treatmentConsentService.GetFormSelectionAsync(serviceMembersChildId);
                var consentFormStatuses = TreatmentConsentHelper.BuildDentistConsentFormStatusItems(
                    formSelection,
                    eventStaffId.Value);

                var showDdsAcknowledgement = consentFormStatuses.Any(c =>
                    string.Equals(c.FormKind, "dental-treatment", StringComparison.OrdinalIgnoreCase));
                var dentistTreatmentForm = (formSelection.DentalTreatmentForms ?? new List<TreatmentConsentDentalTreatmentFormDto>())
                    .Where(f => f != null && f.EventStaffId == eventStaffId.Value)
                    .LastOrDefault();
                var canAcknowledgeDds = showDdsAcknowledgement
                    && dentistTreatmentForm != null
                    && TreatmentConsentHelper.IsFormSigned(
                        dentistTreatmentForm.IsSigned,
                        dentistTreatmentForm.SignatureFileName);
                var ddsAcknowledged = showDdsAcknowledgement && dentistTreatmentForm?.DdsAcknowledged == true;
                string? ddsDisplayName = null;
                string? ddsRoles = null;
                if (ddsAcknowledged && !string.IsNullOrWhiteSpace(dentistTreatmentForm?.DdsAcknowledgedByUserId))
                {
                    (ddsDisplayName, ddsRoles) = await ResolveDdsAcknowledgementDisplayAsync(
                        dentistTreatmentForm!.DdsAcknowledgedByUserId,
                        eventManagementId);
                }
                else if (showDdsAcknowledgement && currentUser != null)
                {
                    // Preview name/roles for unchecked state (not persisted until save).
                    (ddsDisplayName, ddsRoles) = await ResolveDdsAcknowledgementDisplayAsync(
                        currentUser.Id,
                        eventManagementId);
                }

                var pageModel = new DentalTreatmentStationPageViewModel
                {
                    ServiceMember = result.ServiceMembersChild,
                    Questionnaire = questionnaire,
                    XRayStation = xRayStation,
                    DentalExam = dentalExam,
                    SharedClinical = sharedClinical,
                    DentalTreatment = dentalTreatment,
                    DentistStatus = DentalTreatmentValidator.ComputeDentistStatus(
                        dentalTreatment?.SmFinalClassification,
                        assignedExamFindingIds,
                        dentalTreatment?.Findings,
                        currentUser?.Id),
                    AssignedExamFindingIds = assignedExamFindingIds,
                    DentistAppointmentGroups = dentistAppointmentGroups,
                    HasQuestionnaire = questionnaire.Id > 0,
                    ConsentFormStatuses = consentFormStatuses,
                    ShowDdsAcknowledgement = showDdsAcknowledgement,
                    CanAcknowledgeDds = canAcknowledgeDds,
                    DdsAcknowledged = ddsAcknowledged,
                    DdsAcknowledgedDisplayName = ddsDisplayName,
                    DdsAcknowledgedRoles = ddsRoles,
                    DdsAcknowledgedOn = ddsAcknowledged ? dentistTreatmentForm?.DdsAcknowledgedOn : null
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

                var eventStaffId = await _dentalTreatmentService.TryGetEventStaffIdForUserAsync(user.Id);
                var eventManagementId = serviceMemberResult.EventId > 0
                    ? serviceMemberResult.EventId
                    : DentalExamSignatureHelper.TryResolveEventManagementId(
                        User,
                        HttpContext.Session,
                        serviceMemberResult.EventId) ?? 0;

                if (!eventStaffId.HasValue
                    || !await _dentalTreatmentService.IsEligibleForDentalTreatmentAsync(
                        dto.ServiceMembersChildId,
                        eventManagementId,
                        eventStaffId.Value))
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Not Eligible";
                    TempData["ResponseMessage"] = "This service member is not eligible for Dental Treatment for the current dentist.";
                    return RedirectToAction(nameof(Index));
                }

                var assignedExamFindingIds = await _dentalTreatmentService.GetAssignedExamFindingIdsAsync(
                    dto.ServiceMembersChildId,
                    eventStaffId.Value);

                var sharedClinical = await _dentalExamService.GetSharedClinicalByServiceMembersChildIdAsync(dto.ServiceMembersChildId);

                dto.Findings = DentalTreatmentJson.ParseList<DentalTreatmentFindingFormDto>(dto.FindingsJson);
                dto.AnesthesiaRecords = DentalTreatmentJson.ParseList<DentalTreatmentAnesthesiaDto>(dto.AnesthesiaJson);
                dto.Prescriptions = DentalTreatmentJson.ParseList<DentalTreatmentPrescriptionDto>(dto.PrescriptionsJson);
                dto.OverallNotes = DentalTreatmentJson.ParseList<DentalTreatmentOverallNoteDto>(dto.OverallNotesJson);
                dto.PsrSelectedTeeth = DentalTreatmentValidator.NormalizeSelectedTeeth(dto.PsrSelectedTeeth);

                var validationError = DentalTreatmentValidator.ValidateSaveDto(
                    dto,
                    sharedClinical.Findings,
                    assignedExamFindingIds);
                if (!string.IsNullOrWhiteSpace(validationError))
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = validationError;
                    return RedirectToAction(nameof(DentalTreatmentStation), new { serviceMembersChildId = dto.ServiceMembersChildId });
                }

                var formSelectionForSave = await _treatmentConsentService.GetFormSelectionAsync(dto.ServiceMembersChildId);
                dto.RequiresDdsAcknowledgement = (formSelectionForSave.DentalTreatmentDentistEventStaffIds ?? new List<long>())
                    .Contains(eventStaffId.Value);

                if (dto.RequiresDdsAcknowledgement)
                {
                    var dentistTreatmentFormForSave = (formSelectionForSave.DentalTreatmentForms
                            ?? new List<TreatmentConsentDentalTreatmentFormDto>())
                        .Where(f => f != null && f.EventStaffId == eventStaffId.Value)
                        .LastOrDefault();
                    var smSignedTreatmentConsent = dentistTreatmentFormForSave != null
                        && TreatmentConsentHelper.IsFormSigned(
                            dentistTreatmentFormForSave.IsSigned,
                            dentistTreatmentFormForSave.SignatureFileName);

                    if (!smSignedTreatmentConsent)
                    {
                        TempData["ResponseStatus"] = "error";
                        TempData["ResponseTitle"] = "Service Member Signature Required";
                        TempData["ResponseMessage"] =
                            "The Dental Treatment Consent Form must be signed by the service member before DDS acknowledgment.";
                        return RedirectToAction(nameof(DentalTreatmentStation), new { serviceMembersChildId = dto.ServiceMembersChildId });
                    }

                    if (!dto.DdsAcknowledged)
                    {
                        TempData["ResponseStatus"] = "error";
                        TempData["ResponseTitle"] = "DDS Acknowledgment Required";
                        TempData["ResponseMessage"] =
                            "Please acknowledge the DDS signature on the Dental Treatment Consent Form before saving.";
                        return RedirectToAction(nameof(DentalTreatmentStation), new { serviceMembersChildId = dto.ServiceMembersChildId });
                    }
                }

                await _dentalTreatmentService.SaveOrUpdateFromFormDataAsync(
                    dto,
                    user.UserName ?? user.Id,
                    user.Id,
                    assignedExamFindingIds,
                    eventStaffId.Value);

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

        [HttpGet]
        [RoleAttributeAuthorizeFromConfig("DentalTreatment_View")]
        public async Task<IActionResult> PreviewQuestionnairePdf(long serviceMembersChildId)
        {
            const string methodName = nameof(PreviewQuestionnairePdf);
            try
            {
                if (serviceMembersChildId <= 0)
                {
                    return BadRequest("Service member is required.");
                }

                if (!await EnsureCurrentDentistCanAccessServiceMemberAsync(serviceMembersChildId))
                {
                    return Forbid();
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
        [RoleAttributeAuthorizeFromConfig("DentalTreatment_View")]
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

                var currentUser = await _userManager.GetUserAsync(User);
                var currentEventStaffId = currentUser != null
                    ? await _dentalTreatmentService.TryGetEventStaffIdForUserAsync(currentUser.Id)
                    : null;
                if (!currentEventStaffId.HasValue || currentEventStaffId.Value != eventStaffId)
                {
                    return Forbid();
                }

                if (!await EnsureCurrentDentistCanAccessServiceMemberAsync(serviceMembersChildId, currentEventStaffId))
                {
                    return Forbid();
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
                    var (ddsName, ddsRoles) = await ResolveDdsAcknowledgementDisplayAsync(
                        form.DdsAcknowledgedByUserId,
                        result.EventId > 0 ? result.EventId : null);
                    pdfBytes = _treatmentConsentPdfGenerator.GenerateDentalTreatmentConsentPdf(
                        result.ServiceMembersChild,
                        form,
                        signatureBytes,
                        ddsName,
                        ddsRoles);
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

        private async Task<(string DisplayName, string Roles)> ResolveDdsAcknowledgementDisplayAsync(
            string? userId,
            long? eventManagementId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return (string.Empty, string.Empty);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (string.Empty, string.Empty);
            }

            var displayName = await DentalExamSignatureHelper.ResolveDisplayNameAsync(
                user,
                _eventStaffService,
                _logger);
            var roles = await DentalExamSignatureHelper.ResolveEventWiseRolesAsync(
                user.Id,
                eventManagementId,
                _eventStaffService,
                _logger);
            return (displayName, roles);
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

        private async Task<bool> EnsureCurrentDentistCanAccessServiceMemberAsync(
            long serviceMembersChildId,
            long? knownEventStaffId = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || serviceMembersChildId <= 0)
            {
                return false;
            }

            var eventStaffId = knownEventStaffId
                ?? await _dentalTreatmentService.TryGetEventStaffIdForUserAsync(user.Id);
            if (!eventStaffId.HasValue)
            {
                return false;
            }

            var result = await _fileUploader.GetServiceMemberChildWithEventIdAsync(serviceMembersChildId);
            if (result.ServiceMembersChild == null)
            {
                return false;
            }

            var eventManagementId = result.EventId > 0
                ? result.EventId
                : DentalExamSignatureHelper.TryResolveEventManagementId(User, HttpContext.Session, result.EventId) ?? 0;

            return await _dentalTreatmentService.IsEligibleForDentalTreatmentAsync(
                serviceMembersChildId,
                eventManagementId,
                eventStaffId.Value);
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

        private static void ApplyTreatmentSelectedTeethToSharedClinical(
            DentalSharedClinicalViewModel sharedClinical,
            DentalTreatment? dentalTreatment)
        {
            if (dentalTreatment == null)
            {
                return;
            }

            sharedClinical.SelectedTeeth = dentalTreatment.SelectedTeeth
                .Select(t => new DentalPsrSelectedTooth
                {
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
