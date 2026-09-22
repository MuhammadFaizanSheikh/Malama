using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using ExcelFilesCompiler.Utilities;
using Malama.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class DentalCoordinatorStationService : IDentalCoordinatorStationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDentalQuestionnaireService _dentalQuestionnaireService;
        private readonly IDentalXRayStationService _dentalXRayStationService;
        private readonly IDentalExamService _dentalExamService;
        private readonly IDentalTreatmentService _dentalTreatmentService;
        private readonly DentalXRayFileSaveCoordinator _fileSaveCoordinator;
        private readonly TreatmentCoordinatorDocumentFileSaveCoordinator _documentFileSaveCoordinator;
        private readonly ILogger<DentalCoordinatorStationService> _logger;
        private const string CLASSNAME = nameof(DentalCoordinatorStationService);

        public DentalCoordinatorStationService(
            ILogger<DentalCoordinatorStationService> logger,
            IUnitOfWork unitOfWork,
            IDentalQuestionnaireService dentalQuestionnaireService,
            IDentalXRayStationService dentalXRayStationService,
            IDentalExamService dentalExamService,
            IDentalTreatmentService dentalTreatmentService,
            DentalXRayFileSaveCoordinator fileSaveCoordinator,
            TreatmentCoordinatorDocumentFileSaveCoordinator documentFileSaveCoordinator)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _dentalQuestionnaireService = dentalQuestionnaireService;
            _dentalXRayStationService = dentalXRayStationService;
            _dentalExamService = dentalExamService;
            _dentalTreatmentService = dentalTreatmentService;
            _fileSaveCoordinator = fileSaveCoordinator;
            _documentFileSaveCoordinator = documentFileSaveCoordinator;
        }

        public async Task<DentalCoordinatorStationSaveResult> SaveStationAsync(
            DentalCoordinatorStationSaveDto dto,
            ServiceMembersChild serviceMember,
            string userName,
            string userId,
            long eventStaffId)
        {
            const string methodName = nameof(SaveStationAsync);

            DentalXRayStation? existingRecord = null;
            DentalXRayFileUpdatePlan? filePlan = null;
            DentalXRayFileUploadSession? fileSession = null;
            DentalXRayFileUpdatePlan? documentPlan = null;
            DentalXRayFileUploadSession? documentSession = null;
            IDbContextTransaction? transaction = null;
            var dbSaveCompleted = false;

            try
            {
                var barcode = serviceMember.Barcode;
                if (string.IsNullOrWhiteSpace(barcode))
                {
                    return DentalCoordinatorStationSaveResult.Fail(
                        "Invalid Data",
                        "Service member barcode is required for file upload.");
                }

                if (eventStaffId <= 0)
                {
                    return DentalCoordinatorStationSaveResult.Fail(
                        "Invalid Data",
                        "Treatment Coordinator staff profile was not found for the current user.");
                }

                if (dto.Id > 0)
                {
                    var existingResult = await _dentalXRayStationService
                        .GetDentalXRayStationByIdWithEventIdAsync(dto.Id);
                    existingRecord = existingResult.DentalXRayStation;
                    if (existingRecord == null)
                    {
                        return DentalCoordinatorStationSaveResult.Fail(
                            "Not Found",
                            "Dental X-Ray record not found.");
                    }
                }

                filePlan = _fileSaveCoordinator.BuildPlan(dto, existingRecord, barcode);
                if (!string.IsNullOrWhiteSpace(filePlan.ErrorMessage))
                {
                    return DentalCoordinatorStationSaveResult.Fail("Invalid Data", filePlan.ErrorMessage);
                }

                fileSession = await _fileSaveCoordinator.UploadToStagingAsync(filePlan, barcode);
                if (!fileSession.Success)
                {
                    return DentalCoordinatorStationSaveResult.Fail(
                        "Upload Failed",
                        fileSession.ErrorMessage ?? "Failed to upload X-Ray image.");
                }

                var existingTreatment = await _dentalTreatmentService.GetByServiceMembersChildIdAsync(dto.ServiceMembersChildId);
                var existingDocuments = TreatmentCoordinatorDocumentFileSaveCoordinator.ParseDocumentsJson(
                    existingTreatment?.DocumentsJson);

                documentPlan = _documentFileSaveCoordinator.BuildPlan(
                    dto,
                    existingDocuments,
                    barcode,
                    out var resultingDocuments);
                if (!string.IsNullOrWhiteSpace(documentPlan.ErrorMessage))
                {
                    await _fileSaveCoordinator.RollbackStagingAsync(fileSession);
                    return DentalCoordinatorStationSaveResult.Fail("Invalid Data", documentPlan.ErrorMessage);
                }

                documentSession = await _documentFileSaveCoordinator.UploadToStagingAsync(documentPlan, barcode);
                if (!documentSession.Success)
                {
                    await _fileSaveCoordinator.RollbackStagingAsync(fileSession);
                    return DentalCoordinatorStationSaveResult.Fail(
                        "Upload Failed",
                        documentSession.ErrorMessage ?? "Failed to upload documents.");
                }

                DentalXRayStationSaveValidator.SetSectionUploadedDateTimes(dto);

                var findings = DentalFindingBinder.ParseFromJson(dto.FindingsJson);
                var appointments = TreatmentCoordinatorAppointmentHelper.ParseAppointmentsJson(dto.AppointmentsJson);
                var appointmentError = TreatmentCoordinatorAppointmentHelper.Validate(appointments, findings);
                if (!string.IsNullOrWhiteSpace(appointmentError))
                {
                    await _fileSaveCoordinator.RollbackStagingAsync(fileSession);
                    await _documentFileSaveCoordinator.RollbackStagingAsync(documentSession);
                    return DentalCoordinatorStationSaveResult.Fail("Invalid Data", appointmentError);
                }

                transaction = await _unitOfWork.BeginTransactionAsync();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Saving questionnaire. ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, dto.ServiceMembersChildId);

                await _dentalQuestionnaireService.SaveOrUpdateFromFormDataAsync(
                    dto,
                    userName,
                    DentalQuestionnaireSources.DentalCoordinator,
                    saveChanges: false);

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Saving X-Ray station. ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, dto.ServiceMembersChildId);

                var questionnaireForStatus = _dentalQuestionnaireService.MapFormDataToEntity(dto);
                var entity = _dentalXRayStationService.MapSaveDtoToEntity(dto);
                entity.Status = _dentalXRayStationService.ComputeOverallStatus(
                    entity,
                    serviceMember,
                    questionnaireForStatus);

                if (dto.Id == 0)
                {
                    await _dentalXRayStationService.AddAsync(entity, userName, DentalXRaySources.DentalCoordinator, saveChanges: false);
                }
                else
                {
                    await _dentalXRayStationService.UpdateAsync(entity, userName, DentalXRaySources.DentalCoordinator, saveChanges: false);
                }

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Saving clinical sections. ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, dto.ServiceMembersChildId);

                await _dentalExamService.ApplyCoordinatorClinicalSectionsAsync(
                    dto,
                    userName,
                    saveChanges: false);

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Saving findings. ServiceMembersChildId={ServiceMembersChildId}, FindingCount={FindingCount}",
                    CLASSNAME, methodName, dto.ServiceMembersChildId, findings.Count);

                await _dentalExamService.ApplyCoordinatorFindingsAsync(
                    dto,
                    userName,
                    userId,
                    saveChanges: false);

                // Persist exam/findings so new finding Ids exist for appointment links.
                await _unitOfWork.SaveAsync();

                var examAfterSave = await _unitOfWork.DentalExam
                    .GetWithIncludeNoTracking(
                        e => e.ServiceMembersChildId == dto.ServiceMembersChildId,
                        e => e.Findings)
                    .FirstOrDefaultAsync();

                var findingIdByClientKey = (examAfterSave?.Findings ?? Enumerable.Empty<DentalFinding>())
                    .Where(f => !string.IsNullOrWhiteSpace(f.ClientKey) && f.Id > 0)
                    .GroupBy(f => f.ClientKey!.Trim(), StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.Last().Id, StringComparer.OrdinalIgnoreCase);

                // Ensure appointment keys resolve (prefer posted findings client keys).
                foreach (var finding in findings.Where(f => !string.IsNullOrWhiteSpace(f.ClientKey)))
                {
                    var key = finding.ClientKey!.Trim();
                    if (findingIdByClientKey.ContainsKey(key))
                    {
                        continue;
                    }

                    if (finding.Id > 0)
                    {
                        findingIdByClientKey[key] = finding.Id;
                    }
                }

                var existingExam = examAfterSave
                    ?? await _dentalExamService.GetByServiceMembersChildIdAsync(dto.ServiceMembersChildId);
                var coordinatorOverallStatus = DentalCoordinatorTreatmentStatusHelper
                    .ComputeCoordinatorOverallStatus(dto, existingExam);

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Saving treatment section/appointments/documents. ServiceMembersChildId={ServiceMembersChildId}, EventStaffId={EventStaffId}, AppointmentCount={AppointmentCount}, DocumentCount={DocumentCount}",
                    CLASSNAME, methodName, dto.ServiceMembersChildId, eventStaffId,
                    appointments.Count, resultingDocuments.Count);

                await _dentalTreatmentService.ApplyCoordinatorSectionAsync(
                    dto.ServiceMembersChildId,
                    dto.TreatmentCoordinatorComments,
                    coordinatorOverallStatus,
                    userName,
                    userId,
                    eventStaffId,
                    resultingDocuments,
                    appointments,
                    findingIdByClientKey,
                    saveChanges: false);

                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();
                dbSaveCompleted = true;

                _fileSaveCoordinator.CommitFileChanges(filePlan, fileSession);
                _documentFileSaveCoordinator.CommitFileChanges(documentPlan, documentSession);

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Treatment Coordinator station saved atomically. ServiceMembersChildId={ServiceMembersChildId}, User={User}, EventStaffId={EventStaffId}",
                    CLASSNAME, methodName, dto.ServiceMembersChildId, userName, eventStaffId);

                return DentalCoordinatorStationSaveResult.Ok();
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
                            CLASSNAME, methodName, dto.ServiceMembersChildId);
                    }
                }

                if (!dbSaveCompleted)
                {
                    if (fileSession != null)
                    {
                        await _fileSaveCoordinator.RollbackStagingAsync(fileSession);
                    }

                    if (documentSession != null)
                    {
                        await _documentFileSaveCoordinator.RollbackStagingAsync(documentSession);
                    }
                }

                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Failed to save Treatment Coordinator station. ServiceMembersChildId={ServiceMembersChildId}",
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
    }
}
