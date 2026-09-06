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
        private readonly ILogger<TreatmentConsentService> _logger;
        private const string CLASSNAME = nameof(TreatmentConsentService);

        public TreatmentConsentService(
            ILogger<TreatmentConsentService> logger,
            IUnitOfWork unitOfWork,
            IDentalQuestionnaireService dentalQuestionnaireService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _dentalQuestionnaireService = dentalQuestionnaireService;
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
                    .GetWithIncludeNoTracking(c => c.Id == serviceMembersChildId)
                    .FirstOrDefaultAsync();

                if (serviceMember == null)
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, Service member not found. ServiceMembersChildId={ServiceMembersChildId}",
                        CLASSNAME, methodName, serviceMembersChildId);
                    return null;
                }

                var questionnaire = await _dentalQuestionnaireService.GetByServiceMembersChildIdAsync(serviceMembersChildId)
                    ?? new DentalQuestionnaire { ServiceMembersChildId = serviceMembersChildId };

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Station page loaded for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, serviceMembersChildId);

                return new TreatmentConsentStationViewModel
                {
                    ServiceMember = serviceMember,
                    Questionnaire = questionnaire
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
    }
}
