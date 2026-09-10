using System.Text.Json;
using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using ExcelFilesCompiler.Utilities;
using Malama.Models;
using Microsoft.EntityFrameworkCore;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class TreatmentConsentService : ITreatmentConsentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDentalQuestionnaireService _dentalQuestionnaireService;
        private readonly IEventStaffService _eventStaffService;
        private readonly ILogger<TreatmentConsentService> _logger;
        private const string CLASSNAME = nameof(TreatmentConsentService);

        public TreatmentConsentService(
            ILogger<TreatmentConsentService> logger,
            IUnitOfWork unitOfWork,
            IDentalQuestionnaireService dentalQuestionnaireService,
            IEventStaffService eventStaffService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _dentalQuestionnaireService = dentalQuestionnaireService;
            _eventStaffService = eventStaffService;
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

                var serviceMembers = await _unitOfWork.ServiceMembersChild
                    .GetWithIncludeNoTracking(
                        c => c.ServiceMembersParent.EventManagement.Id == eventId &&
                             c.CheckIn == AppConstants.YesNo.Yes)
                    .ToListAsync();

                var viewModel = TreatmentConsentHelper.BuildIndexViewModel(
                    serviceMembers,
                    eventIdDisplay ?? eventId.ToString());

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Retrieved {Count} checked-in service members for EventId={EventId}",
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
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, Invalid ServiceMembersChildId={ServiceMembersChildId}",
                        CLASSNAME, methodName, serviceMembersChildId);
                    return null;
                }

                var serviceMember = await _unitOfWork.ServiceMembersChild
                    .GetWithIncludeNoTracking(
                        c => c.Id == serviceMembersChildId,
                        c => c.ServiceMembersParent)
                    .FirstOrDefaultAsync();

                if (serviceMember == null)
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, Service member not found. ServiceMembersChildId={ServiceMembersChildId}",
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

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Station page loaded for ServiceMembersChildId={ServiceMembersChildId}, EventId={EventId}",
                    CLASSNAME, methodName, serviceMembersChildId, eventId);

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

        public async Task<TreatmentConsentSaveFormSelectionResponse> SaveFormSelectionAsync(
            TreatmentConsentSaveFormSelectionRequest request,
            string userName)
        {
            const string methodName = nameof(SaveFormSelectionAsync);

            try
            {
                if (request == null || request.ServiceMembersChildId <= 0)
                {
                    return TreatmentConsentSaveFormSelectionResponse.Fail("Service member is required.");
                }

                var serviceMemberExists = await _unitOfWork.ServiceMembersChild
                    .GetWithIncludeNoTracking(c => c.Id == request.ServiceMembersChildId)
                    .AnyAsync();

                if (!serviceMemberExists)
                {
                    return TreatmentConsentSaveFormSelectionResponse.Fail("Service member not found.");
                }

                var includeOral = request.IncludeOralSurgeryForm;
                var includeTreatment = request.IncludeDentalTreatmentConsent;
                var oralIds = includeOral
                    ? (request.OralSurgeryDentistEventStaffIds ?? new List<long>()).Where(id => id > 0).Distinct().ToList()
                    : new List<long>();
                var treatmentIds = includeTreatment
                    ? (request.DentalTreatmentDentistEventStaffIds ?? new List<long>()).Where(id => id > 0).Distinct().ToList()
                    : new List<long>();

                if (includeOral && oralIds.Count == 0)
                {
                    return TreatmentConsentSaveFormSelectionResponse.Fail(
                        "Select at least one Oral Surgery dentist when Oral Surgery Form is included.");
                }

                if (includeTreatment && treatmentIds.Count == 0)
                {
                    return TreatmentConsentSaveFormSelectionResponse.Fail(
                        "Select at least one Treatment dentist when Dental Treatment Consent is included.");
                }

                var existing = await _unitOfWork.TreatmentConsent
                    .GetWithIncludeTracking(x => x.ServiceMembersChildId == request.ServiceMembersChildId)
                    .FirstOrDefaultAsync();

                var now = DateTime.Now;
                if (existing == null)
                {
                    existing = new TreatmentConsent
                    {
                        ServiceMembersChildId = request.ServiceMembersChildId,
                        IncludeQuestionnaire = true,
                        IncludeOralSurgeryForm = includeOral,
                        IncludeDentalTreatmentConsent = includeTreatment,
                        OralSurgeryDentistEventStaffIdsJson = SerializeIds(oralIds),
                        DentalTreatmentDentistEventStaffIdsJson = SerializeIds(treatmentIds),
                        OralSurgeryProcedureText = includeOral
                            ? request.OralSurgeryProcedureText?.Trim()
                            : null,
                        AddedBy = userName,
                        AddedOn = now
                    };
                    await _unitOfWork.TreatmentConsent.AddAsync(existing);
                }
                else
                {
                    existing.IncludeQuestionnaire = true;
                    existing.IncludeOralSurgeryForm = includeOral;
                    existing.IncludeDentalTreatmentConsent = includeTreatment;
                    existing.OralSurgeryDentistEventStaffIdsJson = SerializeIds(oralIds);
                    existing.DentalTreatmentDentistEventStaffIdsJson = SerializeIds(treatmentIds);
                    existing.OralSurgeryProcedureText = includeOral
                        ? request.OralSurgeryProcedureText?.Trim()
                        : null;
                    existing.UpdatedBy = userName;
                    existing.UpdatedOn = now;
                }

                await _unitOfWork.SaveAsync();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Saved selection for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, request.ServiceMembersChildId);

                return TreatmentConsentSaveFormSelectionResponse.Ok(
                    "Form selection saved.",
                    MapToSelectionDto(existing));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Failed for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, request?.ServiceMembersChildId);
                return TreatmentConsentSaveFormSelectionResponse.Fail("Unable to save form selection.");
            }
        }

        private static TreatmentConsentFormSelectionDto MapToSelectionDto(TreatmentConsent entity)
        {
            return new TreatmentConsentFormSelectionDto
            {
                ServiceMembersChildId = entity.ServiceMembersChildId,
                IncludeQuestionnaire = true,
                IncludeOralSurgeryForm = entity.IncludeOralSurgeryForm,
                IncludeDentalTreatmentConsent = entity.IncludeDentalTreatmentConsent,
                OralSurgeryDentistEventStaffIds = ParseIds(entity.OralSurgeryDentistEventStaffIdsJson),
                DentalTreatmentDentistEventStaffIds = ParseIds(entity.DentalTreatmentDentistEventStaffIdsJson),
                OralSurgeryProcedureText = entity.OralSurgeryProcedureText
            };
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
