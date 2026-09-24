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
                    .GetWithIncludeNoTracking(e => e.ServiceMembersChildId == serviceMembersChildId)
                    .FirstOrDefaultAsync();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Loaded dental exam for ServiceMembersChildId={ServiceMembersChildId}. Found={Found}",
                    CLASSNAME, methodName, serviceMembersChildId, exam != null);

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

        public async Task<DentalSharedClinicalViewModel> GetSharedClinicalByServiceMembersChildIdAsync(long serviceMembersChildId)
        {
            var psr = await _unitOfWork.DentalPsr
                .GetWithIncludeNoTracking(
                    e => e.ServiceMembersChildId == serviceMembersChildId,
                    e => e.SelectedTeeth)
                .FirstOrDefaultAsync();

            var denClass = await _unitOfWork.DentalDenClass
                .GetWithIncludeNoTracking(e => e.ServiceMembersChildId == serviceMembersChildId)
                .FirstOrDefaultAsync();

            var pano = await _unitOfWork.DentalPanoAcknowledgement
                .GetWithIncludeNoTracking(e => e.ServiceMembersChildId == serviceMembersChildId)
                .FirstOrDefaultAsync();

            var findings = await _unitOfWork.DentalFinding
                .GetAllWithConditionNoTracking(f => f.ServiceMembersChildId == serviceMembersChildId)
                .OrderBy(f => f.SortOrder)
                .ToListAsync();

            return DentalSharedClinicalViewModel.FromParts(serviceMembersChildId, psr, denClass, pano, findings);
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
                    userId,
                    DentalQuestionnaireSources.DentalExam,
                    saveChanges: false);

                var existing = await _unitOfWork.DentalExam
                    .GetWithIncludeTracking(e => e.ServiceMembersChildId == dto.ServiceMembersChildId)
                    .FirstOrDefaultAsync();

                if (existing != null)
                {
                    MapExamHeaderFromDto(dto, existing);
                    existing.UpdatedBy = userId;
                    existing.UpdatedOn = DateTime.Now;
                    existing.Source = DentalExamSources.DentalExam;
                    existing.Status = DentalExamValidator.ComputeOverallStatus(dto);
                }
                else
                {
                    existing = MapExamHeaderFromDto(dto);
                    existing.AddedOn = DateTime.Now;
                    existing.AddedBy = userId;
                    existing.Source = DentalExamSources.DentalExam;
                    existing.Status = DentalExamValidator.ComputeOverallStatus(dto);
                    await _unitOfWork.DentalExam.AddAsync(existing);
                }

                if (DentalExamValidator.IsSubsequentDiseasesSectionActive(dto))
                {
                    await UpsertPsrAsync(dto, userId, DentalExamSources.DentalExam, forceOverwrite: true);
                    await UpsertDenClassAsync(dto, userId, DentalExamSources.DentalExam, forceOverwrite: true);
                    await UpsertPanoAsync(dto, userId, DentalExamSources.DentalExam, forceOverwrite: true);
                    await ReplaceFindingsForExamAsync(dto.ServiceMembersChildId, dto.Findings, userId);
                }
                else
                {
                    await ClearSharedClinicalForExamAsync(dto.ServiceMembersChildId);
                }

                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Dental exam saved for ServiceMembersChildId={ServiceMembersChildId} by {User}. FindingCount={FindingCount}",
                    CLASSNAME, methodName, dto.ServiceMembersChildId, userName, dto.Findings.Count);
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
            string userId,
            bool saveChanges = true)
        {
            const string methodName = nameof(ApplyCoordinatorClinicalSectionsAsync);

            try
            {
                await UpsertPsrAsync(dto, userId, DentalExamSources.DentalCoordinator, forceOverwrite: false);
                await UpsertDenClassAsync(dto, userId, DentalExamSources.DentalCoordinator, forceOverwrite: false);
                await UpsertPanoAsync(dto, userId, DentalExamSources.DentalCoordinator, forceOverwrite: false);

                if (saveChanges)
                {
                    await _unitOfWork.SaveAsync();
                }

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Coordinator clinical sections applied for ServiceMembersChildId={ServiceMembersChildId}. SaveChanges={SaveChanges}",
                    CLASSNAME, methodName, dto.ServiceMembersChildId, saveChanges);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Failed to apply coordinator clinical sections for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, dto.ServiceMembersChildId);
                throw;
            }
        }

        public async Task ApplyCoordinatorFindingsAsync(
            DentalCoordinatorStationSaveDto dto,
            string userName,
            string userId,
            bool saveChanges = true)
        {
            const string methodName = nameof(ApplyCoordinatorFindingsAsync);

            try
            {
                var findings = DentalFindingBinder.ParseFromJson(dto.FindingsJson);
                var validationError = DentalFindingValidator.ValidateFindings(findings);
                if (!string.IsNullOrWhiteSpace(validationError))
                {
                    throw new InvalidOperationException(validationError);
                }

                await ApplyCoordinatorFindingsInternalAsync(dto.ServiceMembersChildId, findings, userId);

                if (saveChanges)
                {
                    await _unitOfWork.SaveAsync();
                }

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Coordinator findings applied for ServiceMembersChildId={ServiceMembersChildId}. FindingCount={FindingCount}, SaveChanges={SaveChanges}",
                    CLASSNAME, methodName, dto.ServiceMembersChildId, findings.Count, saveChanges);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Failed to apply coordinator findings for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, dto.ServiceMembersChildId);
                throw;
            }
        }

        private async Task UpsertPsrAsync(
            DentalCoordinatorStationSaveDto dto,
            string userId,
            string source,
            bool forceOverwrite)
        {
            await UpsertPsrCoreAsync(
                dto.ServiceMembersChildId,
                dto.PsrUpperRight,
                dto.PsrUpperAnterior,
                dto.PsrUpperLeft,
                dto.PsrLowerRight,
                dto.PsrLowerAnterior,
                dto.PsrLowerLeft,
                dto.PsrCarrierRisk,
                dto.SoftTissuesWnl,
                dto.SoftTissuesConditionDetail,
                NormalizeSelectedTeeth(dto.PsrSelectedTeeth),
                userId,
                source,
                forceOverwrite);
        }

        private async Task UpsertPsrAsync(
            DentalExamStationSaveDto dto,
            string userId,
            string source,
            bool forceOverwrite)
        {
            await UpsertPsrCoreAsync(
                dto.ServiceMembersChildId,
                dto.PsrUpperRight,
                dto.PsrUpperAnterior,
                dto.PsrUpperLeft,
                dto.PsrLowerRight,
                dto.PsrLowerAnterior,
                dto.PsrLowerLeft,
                dto.PsrCarrierRisk,
                dto.SoftTissuesWnl,
                dto.SoftTissuesConditionDetail,
                NormalizeSelectedTeeth(dto.PsrSelectedTeeth),
                userId,
                source,
                forceOverwrite);
        }

        private async Task UpsertPsrCoreAsync(
            long serviceMembersChildId,
            string? psrUpperRight,
            string? psrUpperAnterior,
            string? psrUpperLeft,
            string? psrLowerRight,
            string? psrLowerAnterior,
            string? psrLowerLeft,
            string? psrCarrierRisk,
            string? softTissuesWnl,
            string? softTissuesConditionDetail,
            List<int> selectedTeeth,
            string userId,
            string source,
            bool forceOverwrite)
        {
            var existing = await _unitOfWork.DentalPsr
                .GetWithIncludeTracking(
                    e => e.ServiceMembersChildId == serviceMembersChildId,
                    e => e.SelectedTeeth)
                .FirstOrDefaultAsync()
                ?? _unitOfWork.DentalPsr.FindLocal(e => e.ServiceMembersChildId == serviceMembersChildId);

            if (existing != null
                && !forceOverwrite
                && existing.Id > 0
                && string.Equals(existing.Source, DentalExamSources.DentalExam, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation(
                    "{ClassName}, UpsertPsr, Skipping overwrite for ServiceMembersChildId={ServiceMembersChildId} because Source={Source}",
                    CLASSNAME, serviceMembersChildId, existing.Source);
                return;
            }

            if (existing == null)
            {
                existing = new DentalPsr
                {
                    ServiceMembersChildId = serviceMembersChildId,
                    AddedBy = userId,
                    AddedOn = DateTime.Now,
                    SelectedTeeth = new List<DentalPsrSelectedTooth>()
                };
                await _unitOfWork.DentalPsr.AddAsync(existing);
            }
            else
            {
                existing.UpdatedBy = userId;
                existing.UpdatedOn = DateTime.Now;
            }

            existing.PsrUpperRight = psrUpperRight?.Trim();
            existing.PsrUpperAnterior = psrUpperAnterior?.Trim();
            existing.PsrUpperLeft = psrUpperLeft?.Trim();
            existing.PsrLowerRight = psrLowerRight?.Trim();
            existing.PsrLowerAnterior = psrLowerAnterior?.Trim();
            existing.PsrLowerLeft = psrLowerLeft?.Trim();
            existing.PsrCarrierRisk = psrCarrierRisk?.Trim();
            existing.SoftTissuesWnl = softTissuesWnl?.Trim();
            existing.SoftTissuesConditionDetail = existing.SoftTissuesWnl != null
                && existing.SoftTissuesWnl.Equals(DentalExamPsr.SoftTissuesWnlNo, StringComparison.OrdinalIgnoreCase)
                ? softTissuesConditionDetail?.Trim()
                : null;
            existing.Source = source;

            ReplacePsrSelectedTeeth(existing, selectedTeeth);
        }

        private async Task UpsertDenClassAsync(
            DentalCoordinatorStationSaveDto dto,
            string userId,
            string source,
            bool forceOverwrite)
        {
            await UpsertDenClassCoreAsync(
                dto.ServiceMembersChildId,
                dto.DenClass,
                dto.DenClassReasonComments,
                userId,
                source,
                forceOverwrite);
        }

        private async Task UpsertDenClassAsync(
            DentalExamStationSaveDto dto,
            string userId,
            string source,
            bool forceOverwrite)
        {
            await UpsertDenClassCoreAsync(
                dto.ServiceMembersChildId,
                dto.DenClass,
                dto.DenClassReasonComments,
                userId,
                source,
                forceOverwrite);
        }

        private async Task UpsertDenClassCoreAsync(
            long serviceMembersChildId,
            string? denClass,
            string? denClassReasonComments,
            string userId,
            string source,
            bool forceOverwrite)
        {
            var existing = await _unitOfWork.DentalDenClass
                .GetWithIncludeTracking(e => e.ServiceMembersChildId == serviceMembersChildId)
                .FirstOrDefaultAsync()
                ?? _unitOfWork.DentalDenClass.FindLocal(e => e.ServiceMembersChildId == serviceMembersChildId);

            if (existing != null
                && !forceOverwrite
                && existing.Id > 0
                && string.Equals(existing.Source, DentalExamSources.DentalExam, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation(
                    "{ClassName}, UpsertDenClass, Skipping overwrite for ServiceMembersChildId={ServiceMembersChildId} because Source={Source}",
                    CLASSNAME, serviceMembersChildId, existing.Source);
                return;
            }

            if (existing == null)
            {
                existing = new DentalDenClassRecord
                {
                    ServiceMembersChildId = serviceMembersChildId,
                    AddedBy = userId,
                    AddedOn = DateTime.Now
                };
                await _unitOfWork.DentalDenClass.AddAsync(existing);
            }
            else
            {
                existing.UpdatedBy = userId;
                existing.UpdatedOn = DateTime.Now;
            }

            existing.DenClass = denClass?.Trim();
            existing.DenClassReasonComments = denClassReasonComments?.Trim();
            existing.Source = source;
        }

        private async Task UpsertPanoAsync(
            DentalCoordinatorStationSaveDto dto,
            string userId,
            string source,
            bool forceOverwrite)
        {
            await UpsertPanoCoreAsync(dto.ServiceMembersChildId, dto.PanoXRayAcknowledged, userId, source, forceOverwrite);
        }

        private async Task UpsertPanoAsync(
            DentalExamStationSaveDto dto,
            string userId,
            string source,
            bool forceOverwrite)
        {
            await UpsertPanoCoreAsync(dto.ServiceMembersChildId, dto.PanoXRayAcknowledged, userId, source, forceOverwrite);
        }

        private async Task UpsertPanoCoreAsync(
            long serviceMembersChildId,
            bool panoXRayAcknowledged,
            string userId,
            string source,
            bool forceOverwrite)
        {
            var existing = await _unitOfWork.DentalPanoAcknowledgement
                .GetWithIncludeTracking(e => e.ServiceMembersChildId == serviceMembersChildId)
                .FirstOrDefaultAsync()
                ?? _unitOfWork.DentalPanoAcknowledgement.FindLocal(e => e.ServiceMembersChildId == serviceMembersChildId);

            if (existing != null
                && !forceOverwrite
                && existing.Id > 0
                && string.Equals(existing.Source, DentalExamSources.DentalExam, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation(
                    "{ClassName}, UpsertPano, Skipping overwrite for ServiceMembersChildId={ServiceMembersChildId} because Source={Source}",
                    CLASSNAME, serviceMembersChildId, existing.Source);
                return;
            }

            if (existing == null)
            {
                existing = new DentalPanoAcknowledgement
                {
                    ServiceMembersChildId = serviceMembersChildId,
                    AddedBy = userId,
                    AddedOn = DateTime.Now
                };
                await _unitOfWork.DentalPanoAcknowledgement.AddAsync(existing);
            }
            else
            {
                existing.UpdatedBy = userId;
                existing.UpdatedOn = DateTime.Now;
            }

            existing.PanoXRayAcknowledged = panoXRayAcknowledged;
            existing.Source = source;
        }

        private async Task ClearSharedClinicalForExamAsync(long serviceMembersChildId)
        {
            var psr = await _unitOfWork.DentalPsr
                .GetWithIncludeTracking(
                    e => e.ServiceMembersChildId == serviceMembersChildId,
                    e => e.SelectedTeeth)
                .FirstOrDefaultAsync();
            if (psr != null)
            {
                if (psr.SelectedTeeth?.Count > 0)
                {
                    _unitOfWork.DentalPsrSelectedTooth.RemoveRange(psr.SelectedTeeth.ToList());
                }
                _unitOfWork.DentalPsr.RemoveRange(new[] { psr });
            }

            var den = await _unitOfWork.DentalDenClass
                .GetWithIncludeTracking(e => e.ServiceMembersChildId == serviceMembersChildId)
                .FirstOrDefaultAsync();
            if (den != null)
            {
                _unitOfWork.DentalDenClass.RemoveRange(new[] { den });
            }

            var pano = await _unitOfWork.DentalPanoAcknowledgement
                .GetWithIncludeTracking(e => e.ServiceMembersChildId == serviceMembersChildId)
                .FirstOrDefaultAsync();
            if (pano != null)
            {
                _unitOfWork.DentalPanoAcknowledgement.RemoveRange(new[] { pano });
            }

            var findings = await _unitOfWork.DentalFinding
                .GetWithIncludeTracking(f => f.ServiceMembersChildId == serviceMembersChildId)
                .ToListAsync();
            if (findings.Count > 0)
            {
                _unitOfWork.DentalFinding.RemoveRange(findings);
            }
        }

        private void ReplacePsrSelectedTeeth(DentalPsr target, List<int> selectedTeeth)
        {
            target.SelectedTeeth ??= new List<DentalPsrSelectedTooth>();
            var existingTeeth = target.SelectedTeeth.ToList();
            if (existingTeeth.Count > 0)
            {
                _unitOfWork.DentalPsrSelectedTooth.RemoveRange(existingTeeth);
                target.SelectedTeeth.Clear();
            }

            foreach (var toothNumber in selectedTeeth)
            {
                target.SelectedTeeth.Add(new DentalPsrSelectedTooth
                {
                    DentalPsrId = target.Id,
                    ToothNumber = toothNumber
                });
            }
        }

        private async Task ReplaceFindingsForExamAsync(
            long serviceMembersChildId,
            List<DentalFindingDto> findings,
            string userId)
        {
            var existingFindings = await _unitOfWork.DentalFinding
                .GetWithIncludeTracking(f => f.ServiceMembersChildId == serviceMembersChildId)
                .ToListAsync();

            var existingById = existingFindings
                .Where(f => f.Id > 0)
                .ToDictionary(f => f.Id);
            var incomingIds = new HashSet<long>(findings.Where(f => f.Id > 0).Select(f => f.Id));

            var toRemove = existingFindings
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
            }

            var now = DentalFindingMapper.NormalizeDateTime(DateTime.Now);
            foreach (var (finding, index) in findings.Select((item, index) => (item, index)))
            {
                if (finding.Id > 0 && existingById.TryGetValue(finding.Id, out var existing))
                {
                    var clinicalChanged = !FindingClinicalContentEquals(existing, finding);
                    ApplyFindingClinicalFields(existing, finding, index);
                    if (string.IsNullOrWhiteSpace(existing.Source))
                    {
                        existing.Source = DentalFindingSources.DentalExam;
                    }
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

                var entity = DentalFindingMapper.ToEntity(finding, serviceMembersChildId, index);
                entity.Id = 0;
                entity.Source = DentalFindingSources.DentalExam;
                entity.ExaminationAddedBy = userId;
                entity.ExaminationAddedOn = now;
                await _unitOfWork.DentalFinding.AddAsync(entity);
            }
        }

        private async Task ApplyCoordinatorFindingsInternalAsync(
            long serviceMembersChildId,
            List<DentalFindingDto> findings,
            string userId)
        {
            var existingFindings = await _unitOfWork.DentalFinding
                .GetWithIncludeTracking(f => f.ServiceMembersChildId == serviceMembersChildId)
                .ToListAsync();

            var existingById = existingFindings
                .Where(f => f.Id > 0)
                .ToDictionary(f => f.Id);
            var incomingIds = new HashSet<long>(findings.Where(f => f.Id > 0).Select(f => f.Id));

            var missingExamSourced = existingFindings
                .Where(f => f.Id > 0
                    && !incomingIds.Contains(f.Id)
                    && string.Equals(f.Source, DentalFindingSources.DentalExam, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (missingExamSourced.Count > 0)
            {
                throw new InvalidOperationException(
                    "Exam-sourced findings cannot be deleted from Treatment Coordinator.");
            }

            var toRemove = existingFindings
                .Where(f => f.Id > 0
                    && !incomingIds.Contains(f.Id)
                    && string.Equals(f.Source, DentalFindingSources.DentalCoordinator, StringComparison.OrdinalIgnoreCase))
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
                        "One or more findings cannot be removed because treatment has already been recorded against them.");
                }

                var appointmentLinks = _unitOfWork.DentalAppointmentFinding
                    .GetAllWithConditionNoTracking(af => removeIds.Contains(af.DentalFindingId))
                    .ToList();
                if (appointmentLinks.Count > 0)
                {
                    _unitOfWork.DentalAppointmentFinding.RemoveRange(appointmentLinks);
                }

                _unitOfWork.DentalFinding.RemoveRange(toRemove);
            }

            var now = DentalFindingMapper.NormalizeDateTime(DateTime.Now);
            foreach (var (finding, index) in findings.Select((item, index) => (item, index)))
            {
                if (finding.Id > 0 && existingById.TryGetValue(finding.Id, out var existing))
                {
                    var isExamSourced = string.Equals(
                        existing.Source,
                        DentalFindingSources.DentalExam,
                        StringComparison.OrdinalIgnoreCase);

                    if (isExamSourced)
                    {
                        var beforePossible = existing.IsTreatmentPossible;
                        var beforeReason = existing.TreatmentNotPossibleReason;
                        var beforeCommand = existing.TreatmentNotPossibleCommandName;
                        var beforeNext = existing.TreatmentPlanNextAppointmentDate;
                        ApplyFindingTreatmentPossibleFieldsOnly(existing, finding);
                        var changed = !Nullable.Equals(beforePossible, existing.IsTreatmentPossible)
                            || !string.Equals(beforeReason, existing.TreatmentNotPossibleReason, StringComparison.Ordinal)
                            || !string.Equals(beforeCommand, existing.TreatmentNotPossibleCommandName, StringComparison.Ordinal)
                            || !Nullable.Equals(beforeNext, existing.TreatmentPlanNextAppointmentDate);
                        if (changed)
                        {
                            existing.ExaminationUpdatedBy = userId;
                            existing.ExaminationUpdatedOn = now;
                        }
                    }
                    else
                    {
                        var clinicalChanged = !FindingClinicalContentEquals(existing, finding);
                        ApplyFindingClinicalFields(existing, finding, index);
                        if (string.IsNullOrWhiteSpace(existing.Source))
                        {
                            existing.Source = DentalFindingSources.DentalCoordinator;
                        }
                        if (clinicalChanged)
                        {
                            existing.ExaminationUpdatedBy = userId;
                            existing.ExaminationUpdatedOn = now;
                        }
                    }

                    continue;
                }

                var entity = DentalFindingMapper.ToEntity(finding, serviceMembersChildId, index);
                entity.Id = 0;
                entity.Source = DentalFindingSources.DentalCoordinator;
                entity.ExaminationAddedBy = userId;
                entity.ExaminationAddedOn = now;
                entity.ExaminationUpdatedBy = null;
                entity.ExaminationUpdatedOn = null;
                if (string.IsNullOrWhiteSpace(entity.ClientKey))
                {
                    entity.ClientKey = string.IsNullOrWhiteSpace(finding.ClientKey)
                        ? Guid.NewGuid().ToString("N")
                        : finding.ClientKey.Trim();
                }
                await _unitOfWork.DentalFinding.AddAsync(entity);
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
            entity.ExternalExaminerName = dto.ExternalExaminerName?.Trim();
            entity.ExternalExamDateTime = DentalFindingMapper.NormalizeDateTime(dto.ExternalExamDateTime);
            entity.ExternalDentistRemarks = dto.ExternalDentistRemarks?.Trim();
            var isClass3 = DentalFindingConstants.IsClass3(dto.Classification);
            bool? isTreatmentPossible = isClass3
                ? dto.IsTreatmentPossible ?? true
                : null;
            entity.IsTreatmentPossible = isTreatmentPossible;
            entity.TreatmentNotPossibleReason = isTreatmentPossible == false
                ? dto.TreatmentNotPossibleReason?.Trim()
                : null;
            var reason = entity.TreatmentNotPossibleReason;
            entity.TreatmentNotPossibleCommandName = DentalFindingMapper.ResolveCommandName(
                reason,
                dto.TreatmentNotPossibleCommandName);
            entity.TreatmentPlanNextAppointmentDate = DentalFindingMapper.ResolveNextAppointmentDate(
                reason,
                dto.TreatmentPlanNextAppointmentDate);
            if (!string.IsNullOrWhiteSpace(dto.ClientKey))
            {
                entity.ClientKey = dto.ClientKey.Trim();
            }
        }

        private static void ApplyFindingTreatmentPossibleFieldsOnly(DentalFinding entity, DentalFindingDto dto)
        {
            var isClass3 = DentalFindingConstants.IsClass3(entity.Classification);
            if (!isClass3)
            {
                entity.IsTreatmentPossible = null;
                entity.TreatmentNotPossibleReason = null;
                entity.TreatmentNotPossibleCommandName = null;
                entity.TreatmentPlanNextAppointmentDate = null;
                return;
            }

            bool? isTreatmentPossible = dto.IsTreatmentPossible ?? true;
            entity.IsTreatmentPossible = isTreatmentPossible;
            entity.TreatmentNotPossibleReason = isTreatmentPossible == false
                ? dto.TreatmentNotPossibleReason?.Trim()
                : null;
            var reason = entity.TreatmentNotPossibleReason;
            entity.TreatmentNotPossibleCommandName = DentalFindingMapper.ResolveCommandName(
                reason,
                dto.TreatmentNotPossibleCommandName);
            entity.TreatmentPlanNextAppointmentDate = DentalFindingMapper.ResolveNextAppointmentDate(
                reason,
                dto.TreatmentPlanNextAppointmentDate);
            if (!string.IsNullOrWhiteSpace(dto.ClientKey))
            {
                entity.ClientKey = dto.ClientKey.Trim();
            }
        }

        private static bool FindingClinicalContentEquals(DentalFinding existing, DentalFindingDto dto)
        {
            var existingSurfaces = DentalFindingMapper.DeserializeList(existing.AffectedSurfacesJson);
            var existingCdt = DentalFindingMapper.DeserializeList(existing.CdtCodesJson);
            var dtoSurfaces = dto.AffectedSurfaces ?? new List<string>();
            var dtoCdt = dto.CdtCodes ?? new List<string>();
            var isClass3 = DentalFindingConstants.IsClass3(dto.Classification);
            bool? dtoIsTreatmentPossible = isClass3
                ? dto.IsTreatmentPossible ?? true
                : null;
            var existingReason = existing.IsTreatmentPossible == false
                ? existing.TreatmentNotPossibleReason?.Trim()
                : null;
            var dtoReason = dtoIsTreatmentPossible == false
                ? dto.TreatmentNotPossibleReason?.Trim()
                : null;
            var existingCommandName = DentalFindingMapper.ResolveCommandName(
                existingReason,
                existing.TreatmentNotPossibleCommandName);
            var dtoCommandName = DentalFindingMapper.ResolveCommandName(
                dtoReason,
                dto.TreatmentNotPossibleCommandName);
            var existingNextAppt = DentalFindingMapper.ResolveNextAppointmentDate(
                existingReason,
                existing.TreatmentPlanNextAppointmentDate);
            var dtoNextAppt = DentalFindingMapper.ResolveNextAppointmentDate(
                dtoReason,
                dto.TreatmentPlanNextAppointmentDate);

            return existing.IsPrimaryTooth == dto.IsPrimaryTooth
                && string.Equals(existing.AffectedTooth?.Trim(), dto.AffectedTooth?.Trim(), StringComparison.OrdinalIgnoreCase)
                && string.Equals(existing.DiseaseConditionType?.Trim(), dto.DiseaseConditionType?.Trim(), StringComparison.OrdinalIgnoreCase)
                && string.Equals(existing.CdtCodesNotes?.Trim(), dto.CdtCodesNotes?.Trim(), StringComparison.Ordinal)
                && string.Equals(existing.DescriptionDetails?.Trim(), dto.DescriptionDetails?.Trim(), StringComparison.Ordinal)
                && string.Equals(existing.Classification?.Trim(), dto.Classification?.Trim(), StringComparison.OrdinalIgnoreCase)
                && string.Equals(existing.ExternalExaminerName?.Trim(), dto.ExternalExaminerName?.Trim(), StringComparison.Ordinal)
                && Nullable.Equals(existing.ExternalExamDateTime, dto.ExternalExamDateTime)
                && string.Equals(existing.ExternalDentistRemarks?.Trim(), dto.ExternalDentistRemarks?.Trim(), StringComparison.Ordinal)
                && Nullable.Equals(existing.IsTreatmentPossible, dtoIsTreatmentPossible)
                && string.Equals(existingReason, dtoReason, StringComparison.Ordinal)
                && string.Equals(existingCommandName, dtoCommandName, StringComparison.Ordinal)
                && Nullable.Equals(existingNextAppt, dtoNextAppt)
                && existingSurfaces.Count == dtoSurfaces.Count
                && existingSurfaces.All(s => dtoSurfaces.Contains(s, StringComparer.OrdinalIgnoreCase))
                && existingCdt.Count == dtoCdt.Count
                && existingCdt.All(c => dtoCdt.Contains(c, StringComparer.OrdinalIgnoreCase));
        }

        private static List<int> NormalizeSelectedTeeth(IEnumerable<int>? teeth)
        {
            return (teeth ?? Enumerable.Empty<int>())
                .Where(t => t >= 1 && t <= 32)
                .Distinct()
                .OrderBy(t => t)
                .ToList();
        }

        private static DentalExam MapExamHeaderFromDto(DentalExamStationSaveDto dto, DentalExam? existing = null)
        {
            var entity = existing ?? new DentalExam();
            entity.ServiceMembersChildId = dto.ServiceMembersChildId;
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

            return entity;
        }
    }
}
