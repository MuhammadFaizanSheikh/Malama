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
        private readonly ILogger<DentalExamService> _logger;
        private const string CLASSNAME = "DentalExamService";

        public DentalExamService(ILogger<DentalExamService> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<DentalExam?> GetByServiceMembersChildIdAsync(long serviceMembersChildId)
        {
            var exam = await _unitOfWork.DentalExam
                .GetWithIncludeNoTracking(
                    e => e.ServiceMembersChildId == serviceMembersChildId,
                    e => e.Findings)
                .FirstOrDefaultAsync();

            if (exam?.Findings != null)
            {
                exam.Findings = exam.Findings.OrderBy(f => f.SortOrder).ToList();
            }

            return exam;
        }

        public async Task SaveOrUpdateFromFormDataAsync(DentalExamStationSaveDto dto, string userName)
        {
            const string methodName = nameof(SaveOrUpdateFromFormDataAsync);
            IDbContextTransaction? transaction = null;

            try
            {
                dto.Findings = DentalExamFindingBinder.ParseFromJson(dto.FindingsJson);

                transaction = await _unitOfWork.BeginTransactionAsync();

                var existing = await _unitOfWork.DentalExam
                    .GetWithIncludeTracking(
                        e => e.ServiceMembersChildId == dto.ServiceMembersChildId,
                        e => e.Findings)
                    .FirstOrDefaultAsync();

                if (existing != null)
                {
                    MapFormDataToEntity(dto, existing);
                    existing.UpdatedBy = userName;
                    existing.UpdatedOn = DateTime.Now;
                    existing.Status = "Completed";

                    if (DentalExamValidator.IsSubsequentDiseasesSectionActive(dto))
                    {
                        ReplaceFindings(existing, dto.Findings);
                    }

                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Dental exam updated for ServiceMembersChildId={ServiceMembersChildId} by {User}. FindingCount={FindingCount}",
                        CLASSNAME, methodName, dto.ServiceMembersChildId, userName, dto.Findings.Count);
                    return;
                }

                var entity = MapFormDataToEntity(dto);
                entity.AddedOn = DateTime.Now;
                entity.AddedBy = userName;
                entity.Status = "Completed";

                await _unitOfWork.DentalExam.AddAsync(entity);
                await _unitOfWork.SaveAsync();

                if (DentalExamValidator.IsSubsequentDiseasesSectionActive(dto))
                {
                    ReplaceFindings(entity, dto.Findings);
                    await _unitOfWork.SaveAsync();
                }

                await transaction.CommitAsync();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Dental exam created for ServiceMembersChildId={ServiceMembersChildId} by {User}. FindingCount={FindingCount}",
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

        private void ReplaceFindings(DentalExam target, List<DentalExamFindingDto> findings)
        {
            target.Findings ??= new List<DentalExamFinding>();
            var existingFindings = target.Findings.ToList();

            if (existingFindings.Count > 0)
            {
                _unitOfWork.DentalExamFinding.RemoveRange(existingFindings);
                target.Findings.Clear();
            }

            foreach (var (finding, index) in findings.Select((item, index) => (item, index)))
            {
                target.Findings.Add(DentalExamFindingMapper.ToEntity(finding, target.Id, index));
            }
        }

        private static DentalExam MapFormDataToEntity(DentalExamStationSaveDto dto, DentalExam? existing = null)
        {
            var entity = existing ?? new DentalExam();

            entity.ServiceMembersChildId = dto.ServiceMembersChildId;

            if (DentalExamValidator.IsSubsequentDiseasesSectionActive(dto))
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
            }

            entity.QuestionnaireReviewed = dto.QuestionnaireReviewed;
            entity.FinalComments = dto.QuestionnaireReviewed
                ? dto.FinalComments?.Trim()
                : null;

            entity.DentistSignatureEntered = dto.DentistSignatureEntered;
            if (dto.DentistSignatureEntered)
            {
                entity.DentistSignatureName = dto.DentistSignatureName?.Trim();
                if (entity.DentistSignatureDateTime == null
                    && !string.IsNullOrWhiteSpace(entity.DentistSignatureName))
                {
                    entity.DentistSignatureDateTime = DateTime.Now;
                }
            }
            else
            {
                entity.DentistSignatureName = null;
            }

            return entity;
        }
    }
}
