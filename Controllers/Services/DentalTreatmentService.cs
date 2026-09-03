using AutoMapper;
using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using ExcelFilesCompiler.Utilities;
using Malama.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class DentalTreatmentService : IDentalTreatmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DentalTreatmentService> _logger;
        private const string CLASSNAME = nameof(DentalTreatmentService);

        public DentalTreatmentService(
            ILogger<DentalTreatmentService> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DentalTreatment?> GetByServiceMembersChildIdAsync(long serviceMembersChildId)
        {
            const string methodName = nameof(GetByServiceMembersChildIdAsync);

            try
            {
                var treatment = await _unitOfWork.DentalTreatment
                    .GetWithIncludeNoTracking(
                        e => e.ServiceMembersChildId == serviceMembersChildId,
                        e => e.Findings,
                        e => e.SelectedTeeth,
                        e => e.AnesthesiaRecords,
                        e => e.Prescriptions,
                        e => e.OverallNotes)
                    .FirstOrDefaultAsync();

                if (treatment == null)
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, No dental treatment found for ServiceMembersChildId={ServiceMembersChildId}",
                        CLASSNAME, methodName, serviceMembersChildId);
                    return null;
                }

                treatment.Findings = treatment.Findings?.OrderBy(f => f.SortOrder).ToList() ?? new List<DentalTreatmentFinding>();
                treatment.SelectedTeeth = treatment.SelectedTeeth?.OrderBy(t => t.ToothNumber).ToList() ?? new List<DentalTreatmentSelectedTooth>();
                treatment.AnesthesiaRecords = treatment.AnesthesiaRecords?.OrderBy(a => a.SortOrder).ToList() ?? new List<DentalTreatmentAnesthesia>();
                treatment.Prescriptions = treatment.Prescriptions?.OrderBy(p => p.SortOrder).ToList() ?? new List<DentalTreatmentPrescription>();
                treatment.OverallNotes = treatment.OverallNotes?.OrderBy(n => n.SortOrder).ToList() ?? new List<DentalTreatmentOverallNote>();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Loaded dental treatment Id={TreatmentId} for ServiceMembersChildId={ServiceMembersChildId}. FindingCount={FindingCount}, ToothCount={ToothCount}",
                    CLASSNAME, methodName, treatment.Id, serviceMembersChildId,
                    treatment.Findings.Count, treatment.SelectedTeeth.Count);

                return treatment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Failed to load dental treatment for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, serviceMembersChildId);
                throw;
            }
        }

        public async Task SaveOrUpdateFromFormDataAsync(DentalTreatmentStationSaveDto dto, string userName, string userId)
        {
            const string methodName = nameof(SaveOrUpdateFromFormDataAsync);
            IDbContextTransaction? transaction = null;

            try
            {
                dto.Findings = DentalTreatmentJson.ParseList<DentalTreatmentFindingFormDto>(dto.FindingsJson);
                dto.AnesthesiaRecords = DentalTreatmentJson.ParseList<DentalTreatmentAnesthesiaDto>(dto.AnesthesiaJson);
                dto.Prescriptions = DentalTreatmentJson.ParseList<DentalTreatmentPrescriptionDto>(dto.PrescriptionsJson);
                dto.OverallNotes = DentalTreatmentJson.ParseList<DentalTreatmentOverallNoteDto>(dto.OverallNotesJson);
                dto.PsrSelectedTeeth = DentalTreatmentValidator.NormalizeSelectedTeeth(dto.PsrSelectedTeeth);

                transaction = await _unitOfWork.BeginTransactionAsync();

                var existing = await _unitOfWork.DentalTreatment
                    .GetWithIncludeTracking(
                        e => e.ServiceMembersChildId == dto.ServiceMembersChildId,
                        e => e.Findings,
                        e => e.SelectedTeeth,
                        e => e.AnesthesiaRecords,
                        e => e.Prescriptions,
                        e => e.OverallNotes)
                    .FirstOrDefaultAsync();

                if (existing != null)
                {
                    _mapper.Map(dto, existing);
                    existing.Status = DentalTreatmentValidator.ComputeStatus(dto.SmFinalClassification, dto.Findings);
                    existing.UpdatedBy = userName;
                    existing.UpdatedOn = DateTime.Now;

                    ReplaceChildren(existing, dto, userId);
                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Dental treatment updated. Id={TreatmentId}, ServiceMembersChildId={ServiceMembersChildId}, User={User}, Status={Status}, FindingCount={FindingCount}, ToothCount={ToothCount}",
                        CLASSNAME, methodName, existing.Id, dto.ServiceMembersChildId, userName, existing.Status,
                        dto.Findings.Count, dto.PsrSelectedTeeth.Count);
                    return;
                }

                var entity = _mapper.Map<DentalTreatment>(dto);
                entity.Status = DentalTreatmentValidator.ComputeStatus(dto.SmFinalClassification, dto.Findings);
                entity.AddedBy = userName;
                entity.AddedOn = DateTime.Now;

                await _unitOfWork.DentalTreatment.AddAsync(entity);
                await _unitOfWork.SaveAsync();

                ReplaceChildren(entity, dto, userId);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Dental treatment created. Id={TreatmentId}, ServiceMembersChildId={ServiceMembersChildId}, User={User}, Status={Status}, FindingCount={FindingCount}, ToothCount={ToothCount}",
                    CLASSNAME, methodName, entity.Id, dto.ServiceMembersChildId, userName, entity.Status,
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
                            "{ClassName}, {MethodName}, Failed to rollback dental treatment transaction for ServiceMembersChildId={ServiceMembersChildId}",
                            CLASSNAME, methodName, dto.ServiceMembersChildId);
                    }
                }

                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Failed to save dental treatment for ServiceMembersChildId={ServiceMembersChildId}",
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

        public async Task ApplyCoordinatorSectionAsync(
            long serviceMembersChildId,
            string? comments,
            string status,
            string userName,
            string userId,
            bool saveChanges = true)
        {
            const string methodName = nameof(ApplyCoordinatorSectionAsync);

            try
            {
                var exam = await _unitOfWork.DentalExam
                    .GetWithIncludeNoTracking(e => e.ServiceMembersChildId == serviceMembersChildId)
                    .Select(e => new { e.Id })
                    .FirstOrDefaultAsync();

                if (exam == null || exam.Id <= 0)
                {
                    throw new InvalidOperationException(
                        "Dental Exam record is required before saving Treatment Coordinator details.");
                }

                var now = DateTime.Now;
                var trimmedComments = string.IsNullOrWhiteSpace(comments) ? null : comments.Trim();
                var resolvedStatus = string.Equals(status?.Trim(), AppConstants.Status.Completed, StringComparison.OrdinalIgnoreCase)
                    ? AppConstants.Status.Completed
                    : AppConstants.Status.Pending;

                var existing = await _unitOfWork.DentalTreatment
                    .GetWithIncludeTracking(e => e.ServiceMembersChildId == serviceMembersChildId)
                    .FirstOrDefaultAsync();

                if (existing != null)
                {
                    existing.TreatmentCoordinatorUserId = userId;
                    existing.TreatmentCoordinatorDateTime = now;
                    existing.TreatmentCoordinatorComments = trimmedComments;
                    existing.Status = resolvedStatus;
                    existing.UpdatedBy = userName;
                    existing.UpdatedOn = now;
                }
                else
                {
                    await _unitOfWork.DentalTreatment.AddAsync(new DentalTreatment
                    {
                        ServiceMembersChildId = serviceMembersChildId,
                        DentalExamId = exam.Id,
                        Status = resolvedStatus,
                        TreatmentCoordinatorUserId = userId,
                        TreatmentCoordinatorDateTime = now,
                        TreatmentCoordinatorComments = trimmedComments,
                        AddedBy = userName,
                        AddedOn = now
                    });
                }

                if (saveChanges)
                {
                    await _unitOfWork.SaveAsync();
                }

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Treatment Coordinator section applied for ServiceMembersChildId={ServiceMembersChildId} by UserId={UserId}. SaveChanges={SaveChanges}",
                    CLASSNAME, methodName, serviceMembersChildId, userId, saveChanges);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Failed to apply Treatment Coordinator section for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, serviceMembersChildId);
                throw;
            }
        }

        private void ReplaceChildren(DentalTreatment target, DentalTreatmentStationSaveDto dto, string userId)
        {
            ReplaceFindings(target, dto.Findings, userId);
            ReplaceSelectedTeeth(target, dto.PsrSelectedTeeth);
            ReplaceAnesthesia(target, dto.AnesthesiaRecords);
            ReplacePrescriptions(target, dto.Prescriptions, userId);
            ReplaceOverallNotes(target, dto.OverallNotes, userId);
        }

        private void ReplaceFindings(DentalTreatment target, List<DentalTreatmentFindingFormDto> findings, string userId)
        {
            target.Findings ??= new List<DentalTreatmentFinding>();
            if (target.Findings.Count > 0)
            {
                _unitOfWork.DentalTreatmentFinding.RemoveRange(target.Findings.ToList());
                target.Findings.Clear();
            }

            foreach (var (finding, index) in findings.Select((item, index) => (item, index)))
            {
                var origin = ResolveFindingOrigin(finding);
                var isTreatmentOrigin = DentalTreatmentFindingOrigin.IsTreatmentOrigin(origin);
                var examFindingId = !isTreatmentOrigin && finding.DentalFindingId.GetValueOrDefault() > 0
                    ? finding.DentalFindingId
                    : null;

                if (!isTreatmentOrigin)
                {
                    if (examFindingId.GetValueOrDefault() <= 0)
                    {
                        continue;
                    }
                }
                else if (string.IsNullOrWhiteSpace(finding.FinalDrc))
                {
                    continue;
                }

                var entity = _mapper.Map<DentalTreatmentFinding>(finding);
                entity.Id = 0;
                entity.DentalTreatmentId = target.Id;
                entity.DentalFindingId = examFindingId;
                entity.Origin = origin;
                entity.SortOrder = index;
                entity.PostServiceTreatmentJson = DentalTreatmentJson.SerializeList(finding.PostServiceTreatment);
                entity.TreatmentCdtCodesJson = DentalTreatmentJson.SerializeList(finding.TreatmentCdtCodes);
                entity.ProceduredDrc = finding.FinalDrc;
                entity.TreatmentStatus = DentalTreatmentValidator.ResolveFindingTreatmentStatus(finding);
                entity.FindingDateTime = string.IsNullOrWhiteSpace(finding.FindingDateTime) ? null : finding.FindingDateTime.Trim();
                entity.TreatmentDateTime = string.IsNullOrWhiteSpace(finding.TreatmentDateTime) ? null : finding.TreatmentDateTime.Trim();

                var persistDentist = isTreatmentOrigin
                    || string.Equals(finding.TreatmentCompleted, "Yes", StringComparison.OrdinalIgnoreCase);
                entity.DentistProfessional = persistDentist
                    ? (!string.IsNullOrWhiteSpace(finding.DentistProfessional) ? finding.DentistProfessional : userId)
                    : null;

                if (isTreatmentOrigin)
                {
                    entity.IsPrimaryTooth = finding.IsPrimaryTooth;
                    entity.AffectedTooth = finding.AffectedTooth?.Trim();
                    entity.DiseaseConditionType = finding.DiseaseConditionType?.Trim();
                }
                else
                {
                    // Exam-linked rows keep clinical source of truth on DentalFinding.
                    entity.IsPrimaryTooth = false;
                    entity.AffectedTooth = null;
                    entity.DiseaseConditionType = null;
                }

                target.Findings.Add(entity);
            }
        }

        private static string ResolveFindingOrigin(DentalTreatmentFindingFormDto finding)
        {
            if (!string.IsNullOrWhiteSpace(finding.Origin))
            {
                if (DentalTreatmentFindingOrigin.IsTreatmentOrigin(finding.Origin))
                {
                    return DentalTreatmentFindingOrigin.Treatment;
                }

                if (DentalTreatmentFindingOrigin.IsExamOrigin(finding.Origin))
                {
                    return DentalTreatmentFindingOrigin.Exam;
                }
            }

            if (finding.IsTreatmentOnly || finding.DentalFindingId.GetValueOrDefault() <= 0)
            {
                return DentalTreatmentFindingOrigin.Treatment;
            }

            return DentalTreatmentFindingOrigin.Exam;
        }

        private void ReplaceSelectedTeeth(DentalTreatment target, List<int> toothNumbers)
        {
            target.SelectedTeeth ??= new List<DentalTreatmentSelectedTooth>();
            if (target.SelectedTeeth.Count > 0)
            {
                _unitOfWork.DentalTreatmentSelectedTooth.RemoveRange(target.SelectedTeeth.ToList());
                target.SelectedTeeth.Clear();
            }

            foreach (var toothNumber in toothNumbers)
            {
                target.SelectedTeeth.Add(new DentalTreatmentSelectedTooth
                {
                    DentalTreatmentId = target.Id,
                    ToothNumber = toothNumber
                });
            }
        }

        private void ReplaceAnesthesia(DentalTreatment target, List<DentalTreatmentAnesthesiaDto> records)
        {
            target.AnesthesiaRecords ??= new List<DentalTreatmentAnesthesia>();
            if (target.AnesthesiaRecords.Count > 0)
            {
                _unitOfWork.DentalTreatmentAnesthesia.RemoveRange(target.AnesthesiaRecords.ToList());
                target.AnesthesiaRecords.Clear();
            }

            foreach (var (record, index) in records.Select((item, index) => (item, index)))
            {
                var entity = _mapper.Map<DentalTreatmentAnesthesia>(record);
                entity.Id = 0;
                entity.DentalTreatmentId = target.Id;
                entity.SortOrder = index;
                entity.CarpulesByTypeJson = DentalTreatmentJson.SerializeDictionary(record.CarpulesByType);
                target.AnesthesiaRecords.Add(entity);
            }
        }

        private void ReplacePrescriptions(DentalTreatment target, List<DentalTreatmentPrescriptionDto> records, string userId)
        {
            target.Prescriptions ??= new List<DentalTreatmentPrescription>();
            if (target.Prescriptions.Count > 0)
            {
                _unitOfWork.DentalTreatmentPrescription.RemoveRange(target.Prescriptions.ToList());
                target.Prescriptions.Clear();
            }

            foreach (var (record, index) in records.Select((item, index) => (item, index)))
            {
                var entity = _mapper.Map<DentalTreatmentPrescription>(record);
                entity.Id = 0;
                entity.DentalTreatmentId = target.Id;
                entity.SortOrder = index;
                entity.PrescribedBy = !string.IsNullOrWhiteSpace(record.PrescribedBy)
                    ? record.PrescribedBy
                    : userId;
                target.Prescriptions.Add(entity);
            }
        }

        private void ReplaceOverallNotes(DentalTreatment target, List<DentalTreatmentOverallNoteDto> records, string userId)
        {
            target.OverallNotes ??= new List<DentalTreatmentOverallNote>();
            if (target.OverallNotes.Count > 0)
            {
                _unitOfWork.DentalTreatmentOverallNote.RemoveRange(target.OverallNotes.ToList());
                target.OverallNotes.Clear();
            }

            foreach (var (record, index) in records.Select((item, index) => (item, index)))
            {
                var entity = _mapper.Map<DentalTreatmentOverallNote>(record);
                entity.Id = 0;
                entity.DentalTreatmentId = target.Id;
                entity.SortOrder = index;
                entity.Dentist = !string.IsNullOrWhiteSpace(record.Dentist)
                    ? record.Dentist
                    : userId;
                target.OverallNotes.Add(entity);
            }
        }
    }
}
