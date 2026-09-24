using System.Text.Json;
using System.Text.Json.Serialization;
using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using ExcelFilesCompiler.Utilities;
using Malama.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class TreatmentConsentService : ITreatmentConsentService
    {
        private static readonly JsonSerializerOptions FormJsonOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private readonly IUnitOfWork _unitOfWork;
        private readonly IDentalQuestionnaireService _dentalQuestionnaireService;
        private readonly IEventStaffService _eventStaffService;
        private readonly TreatmentConsentFileSaveCoordinator _fileSaveCoordinator;
        private readonly ILogger<TreatmentConsentService> _logger;
        private const string CLASSNAME = nameof(TreatmentConsentService);

        public TreatmentConsentService(
            ILogger<TreatmentConsentService> logger,
            IUnitOfWork unitOfWork,
            IDentalQuestionnaireService dentalQuestionnaireService,
            IEventStaffService eventStaffService,
            TreatmentConsentFileSaveCoordinator fileSaveCoordinator)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _dentalQuestionnaireService = dentalQuestionnaireService;
            _eventStaffService = eventStaffService;
            _fileSaveCoordinator = fileSaveCoordinator;
        }

        public async Task<TreatmentConsentIndexViewModel> GetCheckedInServiceMembersByEventIdAsync(
            long eventId,
            string? eventIdDisplay = null)
        {
            const string methodName = nameof(GetCheckedInServiceMembersByEventIdAsync);
            _logger.LogInformation(
                "{ClassName}, {MethodName}, Called with EventId={EventId}",
                CLASSNAME, methodName, eventId);

            try
            {
                if (eventId <= 0)
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, Invalid EventId={EventId}",
                        CLASSNAME, methodName, eventId);
                    throw new ArgumentOutOfRangeException(nameof(eventId), "EventId must be greater than zero.");
                }

                var class3 = DentalExamDenClass.Class3;
                var completed = AppConstants.Status.Completed;
                var needed = AppConstants.NeededOrNA.Needed;
                var drc3 = DentalStationEligibilityHelper.SmDrcClass3;

                var serviceMembers = await _unitOfWork.ServiceMembersChild
                    .GetWithIncludeNoTracking(
                        c => c.ServiceMembersParent.EventManagement.Id == eventId &&
                             c.CheckIn == AppConstants.YesNo.Yes &&
                             (
                                 c.Drc == drc3
                                 || (
                                     c.DentalNeeded == needed
                                     && c.DentalExamRecord != null
                                     && c.DentalExamRecord.Status == completed
                                     && c.DentalDenClassRecord != null
                                     && c.DentalDenClassRecord.DenClass == class3
                                 )
                             ),
                        c => c.DentalExamRecord,
                        c => c.DentalDenClassRecord)
                    .ToListAsync();

                var smIds = serviceMembers.Select(x => x.Id).ToList();
                Dictionary<long, TreatmentConsent> consentsBySmId;
                if (smIds.Count == 0)
                {
                    consentsBySmId = new Dictionary<long, TreatmentConsent>();
                }
                else
                {
                    consentsBySmId = await _unitOfWork.TreatmentConsent
                        .GetWithIncludeNoTracking(x => smIds.Contains(x.ServiceMembersChildId))
                        .ToDictionaryAsync(x => x.ServiceMembersChildId);
                }

                var viewModel = TreatmentConsentHelper.BuildIndexViewModel(
                    serviceMembers,
                    eventIdDisplay ?? eventId.ToString(),
                    consentsBySmId);

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Retrieved {Count} Treatment Consent candidates for EventId={EventId}",
                    CLASSNAME, methodName, viewModel.TotalCount, eventId);

                return viewModel;
            }
            catch (Exception ex) when (ex is not ArgumentOutOfRangeException)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception while loading checked-in service members for EventId={EventId}",
                    CLASSNAME, methodName, eventId);
                throw new ApplicationException(
                    "An error occurred while retrieving Treatment Consent service members.",
                    ex);
            }
        }

        public async Task<TreatmentConsentStationViewModel?> GetStationPageAsync(long serviceMembersChildId)
        {
            const string methodName = nameof(GetStationPageAsync);
            _logger.LogInformation(
                "{ClassName}, {MethodName}, Called with ServiceMembersChildId={ServiceMembersChildId}",
                CLASSNAME, methodName, serviceMembersChildId);

            try
            {
                if (serviceMembersChildId <= 0)
                {
                    return null;
                }

                var serviceMember = await _unitOfWork.ServiceMembersChild
                    .GetWithIncludeNoTracking(
                        c => c.Id == serviceMembersChildId,
                        c => c.ServiceMembersParent,
                        c => c.DentalExamRecord,
                        c => c.DentalDenClassRecord)
                    .FirstOrDefaultAsync();

                if (serviceMember == null)
                {
                    return null;
                }

                if (!DentalStationEligibilityHelper.IsEligibleForTreatmentCoordinator(
                        serviceMember,
                        serviceMember.DentalExamRecord,
                        serviceMember.DentalDenClassRecord))
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, ServiceMembersChildId={ServiceMembersChildId} is not eligible for Treatment Consent (same rules as Treatment Coordinator)",
                        CLASSNAME, methodName, serviceMembersChildId);
                    return null;
                }

                var eventId = serviceMember.ServiceMembersParent?.EventManagementId ?? 0;

                var questionnaire = await _dentalQuestionnaireService.GetByServiceMembersChildIdAsync(serviceMembersChildId)
                    ?? new DentalQuestionnaire { ServiceMembersChildId = serviceMembersChildId };

                var formSelection = await GetFormSelectionAsync(serviceMembersChildId);

                var oralSurgeryDentists = eventId > 0
                    ? await _eventStaffService.GetDtDentistsByEventIdAsync(eventId, "Oral Surgery")
                    : new List<TreatmentCoordinatorAssignableDentistDto>();

                var dentalTreatmentDentists = eventId > 0
                    ? await _eventStaffService.GetDtDentistsByEventIdAsync(eventId, "Treatment")
                    : new List<TreatmentCoordinatorAssignableDentistDto>();

                return new TreatmentConsentStationViewModel
                {
                    ServiceMember = serviceMember,
                    Questionnaire = questionnaire,
                    EventId = eventId,
                    FormSelection = formSelection,
                    OralSurgeryDentists = oralSurgeryDentists,
                    DentalTreatmentDentists = dentalTreatmentDentists
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception while loading station page for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, serviceMembersChildId);
                throw new ApplicationException(
                    "An error occurred while loading the Treatment Consent station page.",
                    ex);
            }
        }

        public async Task<TreatmentConsentFormSelectionDto> GetFormSelectionAsync(long serviceMembersChildId)
        {
            var entity = await _unitOfWork.TreatmentConsent
                .GetWithIncludeNoTracking(x => x.ServiceMembersChildId == serviceMembersChildId)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                return new TreatmentConsentFormSelectionDto
                {
                    ServiceMembersChildId = serviceMembersChildId,
                    IncludeQuestionnaire = true
                };
            }

            return MapToSelectionDto(entity);
        }

        public async Task<TreatmentConsentStationSaveResult> SaveStationAsync(
            TreatmentConsentStationSaveDto dto,
            string userId)
        {
            const string methodName = nameof(SaveStationAsync);

            IDbContextTransaction? transaction = null;
            DentalXRayFileUpdatePlan? filePlan = null;
            DentalXRayFileUploadSession? fileSession = null;
            var dbSaveCompleted = false;

            try
            {
                if (dto == null || dto.ServiceMembersChildId <= 0)
                {
                    return TreatmentConsentStationSaveResult.Fail("Invalid Data", "Service member is required.");
                }

                var serviceMember = await _unitOfWork.ServiceMembersChild
                    .GetWithIncludeNoTracking(
                        c => c.Id == dto.ServiceMembersChildId,
                        c => c.ServiceMembersParent,
                        c => c.DentalExamRecord,
                        c => c.DentalDenClassRecord)
                    .FirstOrDefaultAsync();

                if (serviceMember == null)
                {
                    return TreatmentConsentStationSaveResult.Fail("Not Found", "Service member not found.");
                }

                if (!DentalStationEligibilityHelper.IsEligibleForTreatmentCoordinator(
                        serviceMember,
                        serviceMember.DentalExamRecord,
                        serviceMember.DentalDenClassRecord))
                {
                    return TreatmentConsentStationSaveResult.Fail(
                        "Not Eligible",
                        "This service member is not eligible for Treatment Consent.");
                }

                var barcode = serviceMember.Barcode;
                if (string.IsNullOrWhiteSpace(barcode))
                {
                    return TreatmentConsentStationSaveResult.Fail(
                        "Invalid Data",
                        "Service member barcode is required for signature upload.");
                }

                var questionnaireError = DentalQuestionnaireValidator.Validate(dto, serviceMember);
                if (!string.IsNullOrWhiteSpace(questionnaireError))
                {
                    return TreatmentConsentStationSaveResult.Fail("Invalid Data", questionnaireError);
                }

                var validationError = ValidateStationSave(dto);
                if (!string.IsNullOrWhiteSpace(validationError))
                {
                    return TreatmentConsentStationSaveResult.Fail("Invalid Data", validationError);
                }

                var existing = await _unitOfWork.TreatmentConsent
                    .GetWithIncludeTracking(x => x.ServiceMembersChildId == dto.ServiceMembersChildId)
                    .FirstOrDefaultAsync();

                filePlan = _fileSaveCoordinator.BuildPlan(dto, existing, barcode);
                if (!string.IsNullOrWhiteSpace(filePlan.ErrorMessage))
                {
                    return TreatmentConsentStationSaveResult.Fail("Invalid Data", filePlan.ErrorMessage);
                }

                fileSession = await _fileSaveCoordinator.UploadToStagingAsync(filePlan, barcode);
                if (!fileSession.Success)
                {
                    return TreatmentConsentStationSaveResult.Fail(
                        "Upload Failed",
                        fileSession.ErrorMessage ?? "Failed to stage signature image.");
                }

                transaction = await _unitOfWork.BeginTransactionAsync();

                await _dentalQuestionnaireService.SaveOrUpdateFromFormDataAsync(
                    dto,
                    userId,
                    DentalQuestionnaireSources.TreatmentConsent,
                    saveChanges: false);

                await ApplyConsentEntityAsync(dto, existing, userId);

                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();
                dbSaveCompleted = true;

                _fileSaveCoordinator.CommitFileChanges(filePlan, fileSession);

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Station saved. ServiceMembersChildId={ServiceMembersChildId}, UserId={UserId}",
                    CLASSNAME, methodName, dto.ServiceMembersChildId, userId);

                return TreatmentConsentStationSaveResult.Ok("Treatment Consent saved.");
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    try
                    {
                        await transaction.RollbackAsync();
                    }
                    catch (Exception rollbackEx)
                    {
                        _logger.LogError(rollbackEx,
                            "{ClassName}, {MethodName}, Failed to rollback DB transaction. ServiceMembersChildId={ServiceMembersChildId}",
                            CLASSNAME, methodName, dto?.ServiceMembersChildId);
                    }
                }

                if (!dbSaveCompleted && fileSession != null)
                {
                    await _fileSaveCoordinator.RollbackStagingAsync(fileSession);
                }

                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Failed to save station. ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, dto?.ServiceMembersChildId);

                return TreatmentConsentStationSaveResult.Fail("Error", "Unable to save Treatment Consent.");
            }
            finally
            {
                if (transaction != null)
                {
                    await transaction.DisposeAsync();
                }
            }
        }

        private async Task ApplyConsentEntityAsync(
            TreatmentConsentStationSaveDto dto,
            TreatmentConsent? existing,
            string userId)
        {
            var includeOral = dto.IncludeOralSurgeryForm;
            var includeTreatment = dto.IncludeDentalTreatmentConsent;
            var oralIds = includeOral
                ? (dto.OralSurgeryDentistEventStaffIds ?? new List<long>()).Where(id => id > 0).Distinct().ToList()
                : new List<long>();
            var treatmentIds = includeTreatment
                ? (dto.DentalTreatmentDentistEventStaffIds ?? new List<long>()).Where(id => id > 0).Distinct().ToList()
                : new List<long>();

            var oralForms = includeOral
                ? (dto.OralSurgeryForms ?? new List<TreatmentConsentOralSurgeryFormDto>())
                    .Where(f => oralIds.Contains(f.EventStaffId))
                    .Select(StripOralForPersistence)
                    .ToList()
                : new List<TreatmentConsentOralSurgeryFormDto>();

            var treatmentForms = includeTreatment
                ? (dto.DentalTreatmentForms ?? new List<TreatmentConsentDentalTreatmentFormDto>())
                    .Where(f => treatmentIds.Contains(f.EventStaffId))
                    .Select(StripTreatmentForPersistence)
                    .ToList()
                : new List<TreatmentConsentDentalTreatmentFormDto>();

            var now = DateTime.Now;
            if (existing == null)
            {
                existing = new TreatmentConsent
                {
                    ServiceMembersChildId = dto.ServiceMembersChildId,
                    AddedBy = userId,
                    AddedOn = now
                };
                await _unitOfWork.TreatmentConsent.AddAsync(existing);
            }
            else
            {
                existing.UpdatedBy = userId;
                existing.UpdatedOn = now;
            }

            existing.IncludeQuestionnaire = true;
            existing.IncludeOralSurgeryForm = includeOral;
            existing.IncludeDentalTreatmentConsent = includeTreatment;
            existing.OralSurgeryDentistEventStaffIdsJson = SerializeIds(oralIds);
            existing.DentalTreatmentDentistEventStaffIdsJson = SerializeIds(treatmentIds);
            existing.OralSurgeryProcedureText = oralForms.FirstOrDefault()?.ProcedureText;
            existing.OralSurgeryFormsJson = JsonSerializer.Serialize(oralForms, FormJsonOptions);
            existing.DentalTreatmentFormsJson = JsonSerializer.Serialize(treatmentForms, FormJsonOptions);
            existing.Status = TreatmentConsentHelper.ComputeStationStatus(
                includeOral,
                oralIds,
                oralForms,
                includeTreatment,
                treatmentIds,
                treatmentForms);
        }

        private static string? ValidateStationSave(TreatmentConsentStationSaveDto dto)
        {
            if (dto.IncludeOralSurgeryForm
                && (dto.OralSurgeryDentistEventStaffIds == null || !dto.OralSurgeryDentistEventStaffIds.Any(id => id > 0)))
            {
                return "Select at least one Oral Surgery dentist when Oral Surgery Form is included.";
            }

            if (dto.IncludeDentalTreatmentConsent
                && (dto.DentalTreatmentDentistEventStaffIds == null || !dto.DentalTreatmentDentistEventStaffIds.Any(id => id > 0)))
            {
                return "Select at least one Treatment dentist when Dental Treatment Consent is included.";
            }

            if (dto.IncludeDentalTreatmentConsent && dto.DentalTreatmentForms != null)
            {
                foreach (var form in dto.DentalTreatmentForms)
                {
                    if (form == null)
                    {
                        continue;
                    }

                    if (form.OtherTreatment && string.IsNullOrWhiteSpace(form.OtherTreatmentText))
                    {
                        var dentistLabel = string.IsNullOrWhiteSpace(form.DentistName)
                            ? "selected dentist"
                            : form.DentistName.Trim();
                        return $"Other treatment description is required when Other is checked ({dentistLabel}).";
                    }
                }
            }

            return null;
        }

        private static TreatmentConsentOralSurgeryFormDto StripOralForPersistence(TreatmentConsentOralSurgeryFormDto form)
        {
            return new TreatmentConsentOralSurgeryFormDto
            {
                EventStaffId = form.EventStaffId,
                DentistName = form.DentistName,
                ProcedureText = form.ProcedureText?.Trim(),
                SignatureFileName = form.SignatureFileName,
                IsSigned = form.IsSigned && !string.IsNullOrWhiteSpace(form.SignatureFileName)
            };
        }

        private static TreatmentConsentDentalTreatmentFormDto StripTreatmentForPersistence(TreatmentConsentDentalTreatmentFormDto form)
        {
            return new TreatmentConsentDentalTreatmentFormDto
            {
                EventStaffId = form.EventStaffId,
                DentistName = form.DentistName,
                Item1Mark = form.Item1Mark,
                Fillings = form.Fillings,
                Crowns = form.Crowns,
                Extractions = form.Extractions,
                Impacted = form.Impacted,
                RootCanal = form.RootCanal,
                Fmd = form.Fmd,
                OtherTreatment = form.OtherTreatment,
                OtherTreatmentText = form.OtherTreatmentText?.Trim(),
                Item1Initials = NormalizeInitials(form.Item1Initials),
                Item2Mark = form.Item2Mark,
                Item2Initials = NormalizeInitials(form.Item2Initials),
                Item3Mark = form.Item3Mark,
                RemovalTeeth = NormalizeTeethValue(form.RemovalTeeth),
                Item3Initials = NormalizeInitials(form.Item3Initials),
                Item4Mark = form.Item4Mark,
                Item4Initials = NormalizeInitials(form.Item4Initials),
                Item5Mark = form.Item5Mark,
                Item5Initials = NormalizeInitials(form.Item5Initials),
                Item6Mark = form.Item6Mark,
                OtherSituation = form.OtherSituation?.Trim(),
                Item6Initials = NormalizeInitials(form.Item6Initials),
                SignatureFileName = form.SignatureFileName,
                IsSigned = form.IsSigned && !string.IsNullOrWhiteSpace(form.SignatureFileName)
            };
        }

        private static string? NormalizeInitials(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var cleaned = new string(value.Trim().ToUpperInvariant().Where(char.IsLetter).Take(2).ToArray());
            return string.IsNullOrEmpty(cleaned) ? null : cleaned;
        }

        private static string? NormalizeTeethValue(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var trimmed = value.Trim().ToUpperInvariant();
            var first = trimmed[0];
            if (first is >= 'A' and <= 'T')
            {
                return first.ToString();
            }

            var digits = new string(trimmed.Where(char.IsDigit).Take(2).ToArray());
            if (string.IsNullOrEmpty(digits))
            {
                return null;
            }

            if (digits.Length == 2
                && (!int.TryParse(digits, out var number) || number < 1 || number > 32))
            {
                digits = digits[..1];
            }

            return string.IsNullOrEmpty(digits) ? null : digits;
        }

        private static TreatmentConsentFormSelectionDto MapToSelectionDto(TreatmentConsent entity)
        {
            var oralForms = TreatmentConsentFileSaveCoordinator.ParseOralForms(entity.OralSurgeryFormsJson);
            var treatmentForms = TreatmentConsentFileSaveCoordinator.ParseTreatmentForms(entity.DentalTreatmentFormsJson);

            var selection = new TreatmentConsentFormSelectionDto
            {
                ServiceMembersChildId = entity.ServiceMembersChildId,
                IncludeQuestionnaire = true,
                IncludeOralSurgeryForm = entity.IncludeOralSurgeryForm,
                IncludeDentalTreatmentConsent = entity.IncludeDentalTreatmentConsent,
                OralSurgeryDentistEventStaffIds = ParseIds(entity.OralSurgeryDentistEventStaffIdsJson),
                DentalTreatmentDentistEventStaffIds = ParseIds(entity.DentalTreatmentDentistEventStaffIdsJson),
                OralSurgeryProcedureText = entity.OralSurgeryProcedureText,
                OralSurgeryForms = oralForms,
                DentalTreatmentForms = treatmentForms
            };
            selection.Status = TreatmentConsentHelper.ComputeStationStatus(selection);
            return selection;
        }

        private static string SerializeIds(List<long> ids)
        {
            return JsonSerializer.Serialize(ids ?? new List<long>());
        }

        private static List<long> ParseIds(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<long>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<long>>(json)?
                    .Where(id => id > 0)
                    .Distinct()
                    .ToList()
                    ?? new List<long>();
            }
            catch
            {
                return new List<long>();
            }
        }
    }
}
