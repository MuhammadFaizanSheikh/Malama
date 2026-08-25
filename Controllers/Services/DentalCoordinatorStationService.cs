using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using ExcelFilesCompiler.Utilities;
using Malama.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class DentalCoordinatorStationService : IDentalCoordinatorStationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDentalQuestionnaireService _dentalQuestionnaireService;
        private readonly IDentalXRayStationService _dentalXRayStationService;
        private readonly IDentalExamService _dentalExamService;
        private readonly DentalXRayFileSaveCoordinator _fileSaveCoordinator;
        private readonly ILogger<DentalCoordinatorStationService> _logger;
        private const string CLASSNAME = nameof(DentalCoordinatorStationService);

        public DentalCoordinatorStationService(
            ILogger<DentalCoordinatorStationService> logger,
            IUnitOfWork unitOfWork,
            IDentalQuestionnaireService dentalQuestionnaireService,
            IDentalXRayStationService dentalXRayStationService,
            IDentalExamService dentalExamService,
            DentalXRayFileSaveCoordinator fileSaveCoordinator)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _dentalQuestionnaireService = dentalQuestionnaireService;
            _dentalXRayStationService = dentalXRayStationService;
            _dentalExamService = dentalExamService;
            _fileSaveCoordinator = fileSaveCoordinator;
        }

        public async Task<DentalCoordinatorStationSaveResult> SaveStationAsync(
            DentalCoordinatorStationSaveDto dto,
            ServiceMembersChild serviceMember,
            string userName)
        {
            const string methodName = nameof(SaveStationAsync);

            DentalXRayStation? existingRecord = null;
            DentalXRayFileUpdatePlan? filePlan = null;
            DentalXRayFileUploadSession? fileSession = null;
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

                DentalXRayStationSaveValidator.SetSectionUploadedDateTimes(dto);

                transaction = await _unitOfWork.BeginTransactionAsync();

                // Section: Questionnaire (track only; committed with shared SaveAsync below)
                await _dentalQuestionnaireService.SaveOrUpdateFromFormDataAsync(
                    dto,
                    userName,
                    DentalQuestionnaireSources.DentalCoordinator,
                    saveChanges: false);

                // Section: X-Ray station (track only)
                var questionnaireForStatus = _dentalQuestionnaireService.MapFormDataToEntity(dto);
                var entity = _dentalXRayStationService.MapSaveDtoToEntity(dto);
                entity.Status = _dentalXRayStationService.ComputeOverallStatus(
                    entity,
                    serviceMember,
                    questionnaireForStatus);

                if (dto.Id == 0)
                {
                    await _dentalXRayStationService.AddAsync(entity, userName, saveChanges: false);
                }
                else
                {
                    await _dentalXRayStationService.UpdateAsync(entity, userName, saveChanges: false);
                }

                // Section: Subsequent diseases (PSR / DEN / Pano)
                await _dentalExamService.ApplyCoordinatorClinicalSectionsAsync(
                    dto,
                    userName,
                    saveChanges: false);

                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();
                dbSaveCompleted = true;

                _fileSaveCoordinator.CommitFileChanges(filePlan, fileSession);

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Dental Coordinator station saved atomically. ServiceMembersChildId={ServiceMembersChildId}, User={User}",
                    CLASSNAME, methodName, dto.ServiceMembersChildId, userName);

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

                if (!dbSaveCompleted && fileSession != null)
                {
                    await _fileSaveCoordinator.RollbackStagingAsync(fileSession);
                }

                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Failed to save Dental Coordinator station. ServiceMembersChildId={ServiceMembersChildId}",
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
