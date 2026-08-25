using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using ExcelFilesCompiler.Utilities;
using Malama.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class DentalExamService : IDentalExamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDentalQuestionnaireService _dentalQuestionnaireService;
        private readonly ILogger<DentalExamService> _logger;
        private const string CLASSNAME = "DentalExamService";

        public DentalExamService(
            ILogger<DentalExamService> logger,
            IUnitOfWork unitOfWork,
            IDentalQuestionnaireService dentalQuestionnaireService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _dentalQuestionnaireService = dentalQuestionnaireService;
        }

        public async Task<DentalExam?> GetByServiceMembersChildIdAsync(long serviceMembersChildId)
        {
            const string methodName = nameof(GetByServiceMembersChildIdAsync);

            try
            {
                var exam = await _unitOfWork.DentalExam
                    .GetWithIncludeNoTracking(
                        e => e.ServiceMembersChildId == serviceMembersChildId,
                        e => e.Findings,
                        e => e.SelectedTeeth)
                    .FirstOrDefaultAsync();

                if (exam?.Findings != null)
                {
                    exam.Findings = exam.Findings.OrderBy(f => f.SortOrder).ToList();
                }

                if (exam?.SelectedTeeth != null)
                {
                    exam.SelectedTeeth = exam.SelectedTeeth.OrderBy(t => t.ToothNumber).ToList();
                }

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Loaded dental exam for ServiceMembersChildId={ServiceMembersChildId}. Found={Found}, SelectedToothCount={SelectedToothCount}",
                    CLASSNAME, methodName, serviceMembersChildId, exam != null,
                    exam?.SelectedTeeth?.Count ?? 0);

                return exam;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Failed to load dental exam for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, serviceMembersChildId);
                throw;
            }
        }

        public async Task SaveOrUpdateFromFormDataAsync(DentalExamStationSaveDto dto, string userName, string userId)
        {
            const string methodName = nameof(SaveOrUpdateFromFormDataAsync);
            IDbContextTransaction? transaction = null;

            try
            {
                dto.Findings = DentalFindingBinder.ParseFromJson(dto.FindingsJson);
                dto.PsrSelectedTeeth = NormalizeSelectedTeeth(dto.PsrSelectedTeeth);

                transaction = await _unitOfWork.BeginTransactionAsync();

                await _dentalQuestionnaireService.SaveOrUpdateFromFormDataAsync(
                    dto,
                    userName,
                    DentalQuestionnaireSources.DentalExam,
                    saveChanges: false);

                var existing = await _unitOfWork.DentalExam
                    .GetWithIncludeTracking(
                        e => e.ServiceMembersChildId == dto.ServiceMembersChildId,
                        e => e.Findings,
                        e => e.SelectedTeeth)
                    .FirstOrDefaultAsync();

                if (existing != null)
                {
                    MapFormDataToEntity(dto, existing);
                    ApplySubsequentDiseasesData(existing, dto, userId);
                    existing.UpdatedBy = userName;
                    existing.UpdatedOn = DateTime.Now;
                    existing.Status = DentalExamValidator.ComputeOverallStatus(dto);

                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Dental exam and questionnaire updated for ServiceMembersChildId={ServiceMembersChildId} by {User}. SubsequentSectionActive={SubsequentSectionActive}, FindingCount={FindingCount}, SelectedToothCount={SelectedToothCount}",
                        CLASSNAME, methodName, dto.ServiceMembersChildId, userName,
                        DentalExamValidator.IsSubsequentDiseasesSectionActive(dto),
                        dto.Findings.Count,
                        dto.PsrSelectedTeeth.Count);
                    return;
                }

                var entity = MapFormDataToEntity(dto);
                entity.AddedOn = DateTime.Now;
                entity.AddedBy = userName;
                entity.Status = DentalExamValidator.ComputeOverallStatus(dto);

                await _unitOfWork.DentalExam.AddAsync(entity);
                await _unitOfWork.SaveAsync();

                ApplySubsequentDiseasesData(entity, dto, userId);
                await _unitOfWork.SaveAsync();

                await transaction.CommitAsync();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Dental exam and questionnaire created for ServiceMembersChildId={ServiceMembersChildId} by {User}. FindingCount={FindingCount}, SelectedToothCount={SelectedToothCount}",
                    CLASSNAME, methodName, dto.ServiceMembersChildId, userName,
                    dto.Findings.Count, dto.PsrSelectedTeeth.Count);
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
                            "{ClassName}, {MethodName}, Failed to rollback dental exam transaction for ServiceMembersChildId={ServiceMembersChildId}",
                            CLASSNAME, methodName, dto.ServiceMembersChildId);
                    }
                }

                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Failed to save dental exam for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, dto.ServiceMembersChildId);
                throw;
            }
            finally
            {
                if (transaction != null)
                {
                    await transaction.DisposeAsync();
                }
            }
        }

        public async Task ApplyCoordinatorClinicalSectionsAsync(
            DentalCoordinatorStationSaveDto dto,
            string userName,
            bool saveChanges = true)
        {
            const string methodName = nameof(ApplyCoordinatorClinicalSectionsAsync);

            try
            {
                var selectedTeeth = NormalizeSelectedTeeth(dto.PsrSelectedTeeth);

                var existing = await _unitOfWork.DentalExam
                    .GetWithIncludeTracking(
                        e => e.ServiceMembersChildId == dto.ServiceMembersChildId,
                        e => e.SelectedTeeth)
                    .FirstOrDefaultAsync();

                if (existing != null)
                {
                    ApplyCoordinatorClinicalFields(existing, dto);
                    ReplaceSelectedTeeth(existing, selectedTeeth);
                    existing.UpdatedBy = userName;
                    existing.UpdatedOn = DateTime.Now;
                }
                else
                {
                    var entity = new DentalExam
                    {
                        ServiceMembersChildId = dto.ServiceMembersChildId,
                        AddedBy = userName,
                        AddedOn = DateTime.Now,
                        Status = AppConstants.Status.Pending
                    };
                    ApplyCoordinatorClinicalFields(entity, dto);
                    await _unitOfWork.DentalExam.AddAsync(entity);
                    await _unitOfWork.SaveAsync();
                    ReplaceSelectedTeeth(entity, selectedTeeth);
                }

                if (saveChanges)
                {
                    await _unitOfWork.SaveAsync();
                }

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Coordinator clinical sections applied for ServiceMembersChildId={ServiceMembersChildId} by {User}. SaveChanges={SaveChanges}, SelectedToothCount={SelectedToothCount}",
                    CLASSNAME, methodName, dto.ServiceMembersChildId, userName, saveChanges, selectedTeeth.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Failed to apply coordinator clinical sections for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, dto.ServiceMembersChildId);
                throw;
            }
        }

        private static void ApplyCoordinatorClinicalFields(DentalExam entity, DentalCoordinatorStationSaveDto dto)
        {
            entity.PsrUpperRight = dto.PsrUpperRight?.Trim();
            entity.PsrUpperAnterior = dto.PsrUpperAnterior?.Trim();
            entity.PsrUpperLeft = dto.PsrUpperLeft?.Trim();
            entity.PsrLowerRight = dto.PsrLowerRight?.Trim();
            entity.PsrLowerAnterior = dto.PsrLowerAnterior?.Trim();
            entity.PsrLowerLeft = dto.PsrLowerLeft?.Trim();
            entity.PsrCarrierRisk = dto.PsrCarrierRisk?.Trim();
            entity.SoftTissuesWnl = dto.SoftTissuesWnl?.Trim();
            entity.SoftTissuesConditionDetail = entity.SoftTissuesWnl != null
                && entity.SoftTissuesWnl.Equals(DentalExamPsr.SoftTissuesWnlNo, StringComparison.OrdinalIgnoreCase)
                ? dto.SoftTissuesConditionDetail?.Trim()
                : null;

            entity.DenClass = dto.DenClass?.Trim();
            entity.DenClassReasonComments = dto.DenClassReasonComments?.Trim();
            entity.PanoXRayAcknowledged = dto.PanoXRayAcknowledged;
        }

        private void ApplySubsequentDiseasesData(DentalExam target, DentalExamStationSaveDto dto, string userId)
        {
            if (DentalExamValidator.IsSubsequentDiseasesSectionActive(dto))
            {
                ReplaceFindings(target, dto.Findings, userId);
                ReplaceSelectedTeeth(target, dto.PsrSelectedTeeth);
                return;
            }

            ReplaceFindings(target, new List<DentalFindingDto>(), userId);
            ReplaceSelectedTeeth(target, new List<int>());
        }

        private void ReplaceFindings(DentalExam target, List<DentalFindingDto> findings, string userId)
        {
            target.Findings ??= new List<DentalFinding>();
            var existingById = target.Findings
                .Where(f => f.Id > 0)
                .ToDictionary(f => f.Id);
            var incomingIds = new HashSet<long>(
                findings.Where(f => f.Id > 0).Select(f => f.Id));

            var toRemove = target.Findings
                .Where(f => f.Id > 0 && !incomingIds.Contains(f.Id))
                .ToList();

            if (toRemove.Count > 0)
            {
                var removeIds = toRemove.Select(f => f.Id).ToList();
                var hasTreatmentLinks = _unitOfWork.DentalTreatmentFinding
                    .GetAllWithConditionNoTracking(tf =>
                        tf.DentalFindingId.HasValue
                        && removeIds.Contains(tf.DentalFindingId.Value))
                    .Any();

                if (hasTreatmentLinks)
                {
                    throw new InvalidOperationException(
                        "One or more Dental Exam findings cannot be removed because treatment has already been recorded against them.");
                }

                _unitOfWork.DentalFinding.RemoveRange(toRemove);
                foreach (var finding in toRemove)
                {
                    target.Findings.Remove(finding);
                }
            }

            var now = DateTime.Now;
            foreach (var (finding, index) in findings.Select((item, index) => (item, index)))
            {
                if (finding.Id > 0 && existingById.TryGetValue(finding.Id, out var existing))
                {
                    var clinicalChanged = !FindingClinicalContentEquals(existing, finding);
                    ApplyFindingClinicalFields(existing, finding, index);

                    if (string.IsNullOrWhiteSpace(existing.ExaminationAddedBy))
                    {
                        existing.ExaminationAddedBy = userId;
                        existing.ExaminationAddedOn = now;
                    }

                    if (clinicalChanged)
                    {
                        existing.ExaminationUpdatedBy = userId;
                        existing.ExaminationUpdatedOn = now;
                    }

                    continue;
                }

                var entity = DentalFindingMapper.ToEntity(finding, target.Id, index);
                entity.Id = 0;
                entity.ExaminationAddedBy = userId;
                entity.ExaminationAddedOn = now;
                entity.ExaminationUpdatedBy = null;
                entity.ExaminationUpdatedOn = null;
                target.Findings.Add(entity);
            }
        }

        private static void ApplyFindingClinicalFields(DentalFinding entity, DentalFindingDto dto, int sortOrder)
        {
            entity.IsPrimaryTooth = dto.IsPrimaryTooth;
            entity.AffectedTooth = dto.AffectedTooth?.Trim() ?? string.Empty;
            entity.DiseaseConditionType = dto.DiseaseConditionType?.Trim() ?? string.Empty;
            entity.AffectedSurfacesJson = DentalFindingMapper.SerializeList(dto.AffectedSurfaces);
            entity.CdtCodesJson = DentalFindingMapper.SerializeList(dto.CdtCodes);
            entity.CdtCodesNotes = dto.CdtCodesNotes?.Trim();
            entity.DescriptionDetails = dto.DescriptionDetails?.Trim();
            entity.Classification = dto.Classification?.Trim();
            entity.SortOrder = sortOrder;
        }

        private static bool FindingClinicalContentEquals(DentalFinding existing, DentalFindingDto dto)
        {
            var existingSurfaces = DentalFindingMapper.DeserializeList(existing.AffectedSurfacesJson);
            var existingCdt = DentalFindingMapper.DeserializeList(existing.CdtCodesJson);
            var dtoSurfaces = dto.AffectedSurfaces ?? new List<string>();
            var dtoCdt = dto.CdtCodes ?? new List<string>();

            return existing.IsPrimaryTooth == dto.IsPrimaryTooth
                && string.Equals(existing.AffectedTooth?.Trim(), dto.AffectedTooth?.Trim(), StringComparison.OrdinalIgnoreCase)
                && string.Equals(existing.DiseaseConditionType?.Trim(), dto.DiseaseConditionType?.Trim(), StringComparison.OrdinalIgnoreCase)
                && string.Equals(existing.CdtCodesNotes?.Trim(), dto.CdtCodesNotes?.Trim(), StringComparison.Ordinal)
                && string.Equals(existing.DescriptionDetails?.Trim(), dto.DescriptionDetails?.Trim(), StringComparison.Ordinal)
                && string.Equals(existing.Classification?.Trim(), dto.Classification?.Trim(), StringComparison.OrdinalIgnoreCase)
                && existingSurfaces.Count == dtoSurfaces.Count
                && existingSurfaces.All(s => dtoSurfaces.Contains(s, StringComparer.OrdinalIgnoreCase))
                && existingCdt.Count == dtoCdt.Count
                && existingCdt.All(c => dtoCdt.Contains(c, StringComparer.OrdinalIgnoreCase));
        }

        private void ReplaceSelectedTeeth(DentalExam target, List<int> selectedTeeth)
        {
            target.SelectedTeeth ??= new List<DentalExamSelectedTooth>();
            var existingTeeth = target.SelectedTeeth.ToList();

            if (existingTeeth.Count > 0)
            {
                _unitOfWork.DentalExamSelectedTooth.RemoveRange(existingTeeth);
                target.SelectedTeeth.Clear();
            }

            foreach (var toothNumber in selectedTeeth)
            {
                target.SelectedTeeth.Add(new DentalExamSelectedTooth
                {
                    DentalExamId = target.Id,
                    ToothNumber = toothNumber
                });
            }

            _logger.LogDebug(
                "{ClassName}, ReplaceSelectedTeeth, DentalExamId={DentalExamId}, SelectedToothCount={SelectedToothCount}",
                CLASSNAME, target.Id, selectedTeeth.Count);
        }

        private static List<int> NormalizeSelectedTeeth(IEnumerable<int>? teeth)
        {
            return (teeth ?? Enumerable.Empty<int>())
                .Where(t => t >= 1 && t <= 32)
                .Distinct()
                .OrderBy(t => t)
                .ToList();
        }

        private static DentalExam MapFormDataToEntity(DentalExamStationSaveDto dto, DentalExam? existing = null)
        {
            var entity = existing ?? new DentalExam();

            entity.ServiceMembersChildId = dto.ServiceMembersChildId;

            if (DentalExamValidator.IsSubsequentDiseasesSectionActive(dto))
            {
                MapSubsequentDiseasesFieldsFromDto(entity, dto);
            }
            else
            {
                ClearSubsequentDiseasesFields(entity);
            }

            entity.QuestionnaireReviewed = dto.QuestionnaireReviewed;
            entity.FinalComments = dto.QuestionnaireReviewed
                ? dto.FinalComments?.Trim()
                : null;

            entity.DentistSignatureEntered = dto.DentistSignatureEntered;
            if (dto.DentistSignatureEntered)
            {
                entity.DentistSignatureUserId = string.IsNullOrWhiteSpace(dto.DentistSignatureUserId)
                    ? null
                    : dto.DentistSignatureUserId.Trim();
                if (entity.DentistSignatureDateTime == null
                    && !string.IsNullOrWhiteSpace(entity.DentistSignatureUserId))
                {
                    entity.DentistSignatureDateTime = DateTime.Now;
                }
            }
            else
            {
                entity.DentistSignatureUserId = null;
            }

            entity.Status = ComputeOverallStatus(dto);

            return entity;
        }

        private static string ComputeOverallStatus(DentalExamStationSaveDto dto)
        {
            return DentalExamValidator.ComputeOverallStatus(dto);
        }

        private static void MapSubsequentDiseasesFieldsFromDto(DentalExam entity, DentalExamStationSaveDto dto)
        {
            entity.PsrUpperRight = dto.PsrUpperRight?.Trim();
            entity.PsrUpperAnterior = dto.PsrUpperAnterior?.Trim();
            entity.PsrUpperLeft = dto.PsrUpperLeft?.Trim();
            entity.PsrLowerRight = dto.PsrLowerRight?.Trim();
            entity.PsrLowerAnterior = dto.PsrLowerAnterior?.Trim();
            entity.PsrLowerLeft = dto.PsrLowerLeft?.Trim();
            entity.PsrCarrierRisk = dto.PsrCarrierRisk?.Trim();
            entity.SoftTissuesWnl = dto.SoftTissuesWnl?.Trim();
            entity.SoftTissuesConditionDetail = dto.SoftTissuesWnl != null
                && dto.SoftTissuesWnl.Equals(DentalExamPsr.SoftTissuesWnlNo, StringComparison.OrdinalIgnoreCase)
                ? dto.SoftTissuesConditionDetail?.Trim()
                : null;

            entity.DenClass = dto.DenClass?.Trim();
            entity.DenClassReasonComments = dto.DenClassReasonComments?.Trim();
            entity.PanoXRayAcknowledged = dto.PanoXRayAcknowledged;
        }

        private static void ClearSubsequentDiseasesFields(DentalExam entity)
        {
            entity.PsrUpperRight = null;
            entity.PsrUpperAnterior = null;
            entity.PsrUpperLeft = null;
            entity.PsrLowerRight = null;
            entity.PsrLowerAnterior = null;
            entity.PsrLowerLeft = null;
            entity.PsrCarrierRisk = null;
            entity.SoftTissuesWnl = null;
            entity.SoftTissuesConditionDetail = null;
            entity.DenClass = null;
            entity.DenClassReasonComments = null;
            entity.PanoXRayAcknowledged = false;
        }
    }
}
