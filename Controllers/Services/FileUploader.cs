using AutoMapper;
using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using Malama.Models;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using ExcelFilesCompiler.Utilities;
using System.Diagnostics.Contracts;
using NPOI.POIFS.Properties;
using iTextSharp.text.pdf;
using static ExcelFilesCompiler.Utilities.AppConstants;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class FileUploader : IFileUploader
    {
        List<string> dateHeaderNames = new List<string> { "DOB", "Date of Next Exam", "PHA Date", "Next PHA Date", "Date of Vision Screen", "Audiogram Date", "Sickle Cell Date", "G6PD Date", "Next Test Date" };
        private string DateFormat = "MM/dd/yyyy";
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEventManagementService _eventManagementService;
        private readonly ILogger<FileUploader> _logger;
        private const string CLASSNAME = "FileUploader";

        public FileUploader(ILogger<FileUploader> logger, IEventManagementService eventManagementService, IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _eventManagementService = eventManagementService;
            _logger = logger;
        }

        public List<List<Dictionary<string, object>>> UploadAndPreview(List<IFormFile> files, IFormFile G6PDFile, DateTime parsedEventDate, DateTime? parsedLastEventDate, long eventId, int lastDentalExam, int vision, int dental, int pha, int hiv, int hearing)
        {
            var eventRecords = _unitOfWork.EventManagement.GetByIdAsync(eventId).Result;
            
            if (eventRecords == null)
            {
                throw new Exception($"No Event Id found against {eventId}");
            }

            var processingSequence = GetProcessingSequence();

            var validationErrors = CheckFieldsBeforeUploading(files, processingSequence);

            DateTime visionDate = parsedEventDate.AddDays(vision);
            DateTime dentalDate = parsedEventDate.AddDays(dental);
            DateTime phaDate = parsedEventDate.AddDays(pha);
            DateTime hivDate = parsedEventDate.AddDays(hiv);
            DateTime hearingDate = parsedEventDate.AddDays(hearing);
            var fileConfigurations = GetFileConfiguration();
            var fileConfigG6PD = fileConfigurations["G6PDReport.xlsx"];
            var G6PDDataTable = GetDataFromFile(G6PDFile, fileConfigG6PD.Headers, fileConfigG6PD.SelectedColumns, fileConfigG6PD.stoppingKeyword, isG6PD: true, out string taskForceValue, out long totalRecordValueInG6pdFile);
            CheckG6PDTotalAssignedCountWithTotalRow(G6PDDataTable, totalRecordValueInG6pdFile);
            ProcessExcelFiles(files, G6PDDataTable, processingSequence, fileConfigurations, parsedLastEventDate, visionDate, dentalDate, phaDate, hivDate, hearingDate, parsedEventDate, lastDentalExam, vision, dental, pha, hiv, hearing, eventRecords.EventID);
            MergeSomeMoreColumns(G6PDDataTable, taskForceValue, eventId);
            var headerMapping = GetHeaderMapping();
            string[] predefinedColumnOrder = GetPredefinedColumnHeader();
            var preparedDataTable = RenameReorderAndProcess(G6PDDataTable, headerMapping, predefinedColumnOrder);
            var mergedData = DataTableToJson(preparedDataTable);

            return new List<List<Dictionary<string, object>>>
            {
                validationErrors,
                mergedData
            };
        }

        public async Task<ResponseDto> AddRecordsBulkAsync(List<FileDataDto> fileDataDtos, string eventId, string addedBy)
        {
            const string methodName = nameof(AddRecordsBulkAsync);

            if (fileDataDtos == null || !fileDataDtos.Any())
            {
                _logger.LogWarning("{ClassName}, {MethodName}, No data provided to insert for EventID: {EventID} by User: {UserName}",
                    CLASSNAME, methodName, eventId, addedBy);

                return new ResponseDto
                {
                    Success = false,
                    Message = "No data provided to insert.",
                    Data = null
                };
            }

            const string genericErrorMsg = "An error occurred while inserting records.";

            try
            {
                // Fetch latest EventManagement and existing parent
                var (fetchResponse, eventManagement, existingParent) =
                    await FetchLatestEventManagementWithParentAsync(eventId, addedBy);

                if (!fetchResponse.Success)
                    return fetchResponse;

                long eventManagementId = eventManagement.Id;
                int eventVersion = eventManagement.EventVersion;

                _logger.LogInformation("{ClassName}, {MethodName}, Inserting {Count} records for EventID: {EventID} (V{Version}) by User: {UserName}",
                    CLASSNAME, methodName, fileDataDtos.Count, eventId, eventVersion, addedBy);

                // ✅ Begin transaction
                await using var transaction = await _unitOfWork.BeginTransactionAsync();

                try
                {
                    // Mark existing parent as deleted
                    if (existingParent != null)
                    {
                        existingParent.isDeleted = true;
                        existingParent.UpdatedBy = addedBy;
                        existingParent.UpdatedOn = DateTime.Now;
                        await _unitOfWork.SaveAsync();

                        _logger.LogInformation("{ClassName}, {MethodName}, Marked existing ServiceMembersParent (ID: {ParentId}) as deleted for EventID: {EventID} by User: {UserName}",
                            CLASSNAME, methodName, existingParent.Id, eventId, addedBy);
                    }

                    var newServiceMembersParent = MapToServiceMembersParent(fileDataDtos.FirstOrDefault(), eventManagementId, addedBy);
                    newServiceMembersParent.ServiceMembersChildren = MapToServiceMembersChildren(fileDataDtos, addedBy);

                    await _unitOfWork.ServiceMembersParent.AddAsync(newServiceMembersParent);
                    await _unitOfWork.SaveAsync();

                    _logger.LogInformation("{ClassName}, {MethodName}, Added new ServiceMembersParent (ID: {ParentId}) with {Count} children for EventID: {EventID} by User: {UserName}",
                        CLASSNAME, methodName, newServiceMembersParent.Id, newServiceMembersParent.ServiceMembersChildren.Count, eventId, addedBy);

                    await transaction.CommitAsync();

                    _logger.LogInformation("{ClassName}, {MethodName}, Transaction committed successfully for EventID: {EventID} by User: {UserName}",
                        CLASSNAME, methodName, eventId, addedBy);

                    return new ResponseDto
                    {
                        Success = true,
                        Message = $"Successfully inserted {fileDataDtos.Count} records for EventID {eventId} (V{eventVersion}).",
                        Data = newServiceMembersParent.Id
                    };
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Error inserting records for EventID: {EventID} by User: {UserName}",
                    CLASSNAME, methodName, eventId, addedBy);

                return new ResponseDto
                {
                    Success = false,
                    Message = genericErrorMsg,
                    Data = null
                };
            }
        }

        public async Task<ResponseDto> CheckForExistingDataAgainstEventIdAsync(string eventId, string addedBy)
        {
            const string methodName = nameof(CheckForExistingDataAgainstEventIdAsync);

            if (string.IsNullOrEmpty(eventId))
            {
                _logger.LogWarning("{ClassName}, {MethodName}, EventID is null or empty", CLASSNAME, methodName);
                return new ResponseDto
                {
                    Success = false,
                    Message = "EventID cannot be empty.",
                    Data = null
                };
            }

            try
            {
                var (fetchResponse, eventManagement, existingParent) =
                    await FetchLatestEventManagementWithParentAsync(eventId, addedBy);

                if (!fetchResponse.Success)
                {
                    // EventManagement not found
                    return new ResponseDto
                    {
                        Success = false,
                        Message = fetchResponse.Message,
                        Data = null,
                        Code = fetchResponse.Code
                    };
                }

                if (existingParent != null)
                {
                    // Data exists
                    _logger.LogInformation("{ClassName}, {MethodName}, Data already exists for EventID: {EventID} (ParentID: {ParentId}) by User: {UserName}",
                        CLASSNAME, methodName, eventId, existingParent.Id, addedBy);

                    return new ResponseDto
                    {
                        Success = false,
                        Message = AppConstants.ResponseMessages.EventDataAlreadyExists,
                        Data = existingParent.Id,
                        Code = AppConstants.ResponseCodes.AlreadyExists
                    };
                }

                // No existing data
                _logger.LogInformation("{ClassName}, {MethodName}, No existing data found for EventID: {EventID} by User: {UserName}",
                    CLASSNAME, methodName, eventId, addedBy);

                return new ResponseDto
                {
                    Success = true,
                    Message = "No existing data found for this EventID.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Error checking existing data for EventID: {EventID} by User: {UserName}",
                    CLASSNAME, methodName, eventId, addedBy);

                return new ResponseDto
                {
                    Success = false,
                    Message = "Error occurred while checking existing data.",
                    Data = null
                };
            }
        }

        public List<Dictionary<string, object>> CheckFieldsBeforeUploading(List<IFormFile> files, List<string>? processingSequence)
        {
            var validationErrors = new List<Dictionary<string, object>>();
            var fileConfigurations = GetFileConfigurationForChecking();

            foreach (var fileName in processingSequence)
            {
                IFormFile file = null; // Assume file is fetched correctly based on your logic
                if (fileName.StartsWith("Taskforce_Roster", StringComparison.OrdinalIgnoreCase))
                {
                    file = files.FirstOrDefault(f => Regex.IsMatch(f.FileName, @"^Taskforce_Roster.*\.xlsx$", RegexOptions.IgnoreCase));
                }
                else
                {
                    file = files.FirstOrDefault(f => f.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase));
                }
                if (file == null) continue;

                var fileConfig = fileConfigurations[fileName];
                DataTable fileData = GetDataFromFile(file, fileConfig.Headers, fileConfig.SelectedColumns, fileConfig.stoppingKeyword, isG6PD: false, out string taskForceValue, out long totalRecordValueInG6pdFile);

                // Perform validations based on the report type
                ValidateMissingFields(fileName, fileData, fileConfig.SelectedColumns, validationErrors);

            }

            return validationErrors;
        }

        public async Task<List<ServiceMembersChild>> GetImmunizationsByEventIdAsync(long eventId)
        {
            const string methodName = nameof(GetImmunizationsByEventIdAsync);

            try
            {
                _logger.LogInformation(
                    "{ClassName}.{MethodName} - Fetching ImmunizationRecords where Imm=Needed and CheckIn=Yes for EventId={EventId}",
                    CLASSNAME, methodName, eventId);

                var result = await _unitOfWork.ServiceMembersChild
                    .GetWithIncludeNoTracking(
                        c => c.ServiceMembersParent.EventManagement.Id == eventId &&
                             c.Imm == AppConstants.NeededOrNA.Needed &&
                             c.CheckIn == "Yes",
                        c => c.ImmunizationRecord
                    )
                    .ToListAsync();

                _logger.LogInformation(
                    "{ClassName}.{MethodName} - Retrieved {Count} ImmunizationRecords for EventId={EventId}",
                    CLASSNAME, methodName, result.Count, eventId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}.{MethodName} - Error fetching ImmunizationRecords for EventId={EventId}",
                    CLASSNAME, methodName, eventId);

                throw;
            }
        }

        public async Task<List<ServiceMembersChild>> GetLabStationByEventIdAsync(long eventId, string? status = null)
        {
            const string methodName = nameof(GetLabStationByEventIdAsync);

            try
            {
                _logger.LogInformation(
                    "{ClassName}.{MethodName} - Fetching LabStation where LabNeeded=Needed, CheckIn=Yes, EventId={EventId}, Status={Status}",
                    CLASSNAME, methodName, eventId, status);

                var result = await _unitOfWork.ServiceMembersChild
                    .GetWithIncludeNoTracking(
                        c => c.ServiceMembersParent.EventManagement.Id == eventId &&
                             c.LabNeeded == AppConstants.NeededOrNA.Needed &&
                             c.CheckIn == "Yes" &&
                             (status == null ||
                              (c.LabStationRecord != null && c.LabStationRecord.Status == status)),
                        c => c.LabStationRecord
                    )
                    .ToListAsync();

                _logger.LogInformation(
                    "{ClassName}.{MethodName} - Retrieved {Count} LabStation records for EventId={EventId}, Status={Status}",
                    CLASSNAME, methodName, result.Count, eventId, status);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}.{MethodName} - Error fetching LabStation data for EventId={EventId}, Status={Status}",
                    CLASSNAME, methodName, eventId, status);

                throw;
            }
        }

        public async Task<List<ServiceMembersChild>> GetPreAndPostLabStationByEventIdAsync(long eventId)
        {
            const string methodName = nameof(GetPreAndPostLabStationByEventIdAsync);

            try
            {
                _logger.LogInformation(
                    "{ClassName}.{MethodName} - Fetching pre & post lab data for EventId={EventId}",
                    CLASSNAME, methodName, eventId);

                var result = await _unitOfWork.ServiceMembersChild
                    .GetWithIncludeNoTracking(
                        c => c.ServiceMembersParent.EventManagement.Id == eventId &&
                             c.LabNeeded == AppConstants.NeededOrNA.Needed &&
                             c.CheckIn == "Yes" &&
                             c.LabStationRecord != null &&
                             c.LabStationRecord.Status == AppConstants.Status.Completed,
                        c => c.LabStationRecord,
                        c => c.PostEventLabStation  // ✅ fixed
                    )
                    .ToListAsync();

                if (result == null || !result.Any())
                {
                    _logger.LogWarning(
                        "{ClassName}.{MethodName} - No lab data found for EventId={EventId}",
                        CLASSNAME, methodName, eventId);
                }
                else
                {
                    _logger.LogInformation(
                        "{ClassName}.{MethodName} - Retrieved {Count} records for EventId={EventId}",
                        CLASSNAME, methodName, result.Count, eventId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}.{MethodName} - Error fetching lab data for EventId={EventId}",
                    CLASSNAME, methodName, eventId);

                throw;
            }
        }

        public async Task<PostEventLabStationAnalysisDto?>GetPostEventLabStationAnalysisDtoAsync(long serviceMembersChildId)
        {
            const string methodName = nameof(GetPostEventLabStationAnalysisDtoAsync);

            try
            {
                _logger.LogInformation(
                    "{ClassName}.{MethodName} - Fetching DTO for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, serviceMembersChildId);

                var entity = await _unitOfWork.ServiceMembersChild
                    .GetWithIncludeNoTracking(
                        c => c.Id == serviceMembersChildId &&
                             c.LabNeeded == AppConstants.NeededOrNA.Needed &&
                             c.CheckIn == "Yes" &&
                             c.LabStationRecord != null &&
                             c.LabStationRecord.Status == AppConstants.Status.Completed,
                        c => c.LabStationRecord,
                        c => c.PostEventLabStation,
                        c => c.ServiceMembersParent.EventManagement,
                        c => c.ServiceMembersParent.EventManagement.PostEventManagement
                    )
                    .FirstOrDefaultAsync();

                if (entity == null)
                {
                    _logger.LogWarning(
                        "{ClassName}.{MethodName} - No data found for ServiceMembersChildId={ServiceMembersChildId}",
                        CLASSNAME, methodName, serviceMembersChildId);

                    return null;
                }

                var dto = new PostEventLabStationAnalysisDto
                {
                    EventID = entity.ServiceMembersParent?.EventManagement?.EventID,
                    EventId = entity.ServiceMembersParent.EventManagement.Id,

                    ServiceMember = new ServiceMembersChildDto
                    {
                        FullName = entity.FullName,
                        DodId = entity.DodId,
                        Barcode = entity.Barcode,
                        Dob = entity.Dob,
                        Age = entity.Age,
                        Sex = entity.Sex
                    },

                    // LabStation (Pre-Lab)
                    LabStation = new PreEventLabStationDto
                    {
                        G6pdNeeded = entity.LabStationRecord.G6pdNeeded,
                        AboNeeded = entity.LabStationRecord.AboNeeded,
                        HivNeeded = entity.LabStationRecord.HivNeeded,
                        PregnancyTestNeeded = entity.LabStationRecord.PregnancyTestNeeded,
                        LipidPanelNeeded = entity.LabStationRecord.LipidPanelNeeded,
                        SickleCellNeeded = entity.LabStationRecord.SickleCellNeeded,
                        DnaNeeded = entity.LabStationRecord.DnaNeeded,

                        G6pdGivenDateTime = entity.LabStationRecord.G6pdGivenDateTime,
                        AboGivenDateTime = entity.LabStationRecord.AboGivenDateTime,
                        HivGivenDateTime = entity.LabStationRecord.HivGivenDateTime,
                        PregnancyTestGivenDateTime = entity.LabStationRecord.PregnancyTestGivenDateTime,
                        LipidPanelGivenDateTime = entity.LabStationRecord.LipidPanelGivenDateTime,
                        SickleCellGivenDateTime = entity.LabStationRecord.SickleCellGivenDateTime,
                        DnaGivenDateTime = entity.LabStationRecord.DnaGivenDateTime,

                        LipidPanelRapidTesting = entity.LabStationRecord.LipidPanelRapidTesting,
                        HivBarcodeCarebill = entity.LabStationRecord.HivBarcodeCarebill,
                        DnaSerialNo = entity.LabStationRecord.DnaSerialNo,
                        FedExTrackingNo = entity.LabStationRecord.FedExTrackingNo,
                        PregnancyTestResult = entity.LabStationRecord.PregnancyTestResult
                    },

                    // Post Event Lab (Editable)
                    PostEventLabStation = entity.PostEventLabStation != null
                        ? new PostEventLabStationDto
                        {
                            Id = entity.PostEventLabStation?.Id ?? 0,
                            ServiceMembersChildId = entity.Id,
                            PostEventManagementId = entity?.ServiceMembersParent?.EventManagement?.PostEventManagement?.Id ?? 0,
                            Status = entity.PostEventLabStation.Status,

                            // G6PD
                            G6pdResultReceived = entity.PostEventLabStation.G6pdResultReceived,
                            G6pdResultReason = entity.PostEventLabStation.G6pdResultReason,
                            G6pdResultReceivedDateTime = entity.PostEventLabStation.G6pdResultReceivedDateTime,
                            G6pdResultMalamaUploaded = entity.PostEventLabStation.G6pdResultMalamaUploaded,
                            G6pdResultMalamaUploadedFileName = entity.PostEventLabStation.G6pdResultMalamaUploadedFileName,
                            G6pdResultMalamaUploadedDateTime = entity.PostEventLabStation.G6pdResultMalamaUploadedDateTime,
                            G6pdResultHRRUploaded = entity.PostEventLabStation.G6pdResultHRRUploaded,
                            G6pdResultHRRUploadedDateTime = entity.PostEventLabStation.G6pdResultHRRUploadedDateTime,
                            G6pdResultSORUploaded = entity.PostEventLabStation.G6pdResultSORUploaded,
                            G6pdResultSORUploadedDateTime = entity.PostEventLabStation.G6pdResultSORUploadedDateTime,

                            // LipidPanel
                            LipidPanelResultReceived = entity.PostEventLabStation.LipidPanelResultReceived,
                            LipidPanelResultReason = entity.PostEventLabStation.LipidPanelResultReason,
                            LipidPanelResultReceivedDateTime = entity.PostEventLabStation.LipidPanelResultReceivedDateTime,
                            LipidPanelResultMalamaUploaded = entity.PostEventLabStation.LipidPanelResultMalamaUploaded,
                            LipidPanelResultMalamaUploadedFileName = entity.PostEventLabStation.LipidPanelResultMalamaUploadedFileName,
                            LipidPanelResultMalamaUploadedDateTime = entity.PostEventLabStation.LipidPanelResultMalamaUploadedDateTime,
                            LipidPanelResultHRRUploaded = entity.PostEventLabStation.LipidPanelResultHRRUploaded,
                            LipidPanelResultHRRUploadedDateTime = entity.PostEventLabStation.LipidPanelResultHRRUploadedDateTime,
                            LipidPanelResultSORUploaded = entity.PostEventLabStation.LipidPanelResultSORUploaded,
                            LipidPanelResultSORUploadedDateTime = entity.PostEventLabStation.LipidPanelResultSORUploadedDateTime,

                            // ABO
                            AboResultReceived = entity.PostEventLabStation.AboResultReceived,
                            AboResultReason = entity.PostEventLabStation.AboResultReason,
                            AboResultReceivedDateTime = entity.PostEventLabStation.AboResultReceivedDateTime,
                            AboResultMalamaUploaded = entity.PostEventLabStation.AboResultMalamaUploaded,
                            AboResultMalamaUploadedFileName = entity.PostEventLabStation.AboResultMalamaUploadedFileName,
                            AboResultMalamaUploadedDateTime = entity.PostEventLabStation.AboResultMalamaUploadedDateTime,
                            AboResultHRRUploaded = entity.PostEventLabStation.AboResultHRRUploaded,
                            AboResultHRRUploadedDateTime = entity.PostEventLabStation.AboResultHRRUploadedDateTime,
                            AboResultSORUploaded = entity.PostEventLabStation.AboResultSORUploaded,
                            AboResultSORUploadedDateTime = entity.PostEventLabStation.AboResultSORUploadedDateTime,

                            // HIV
                            HivResultReceived = entity.PostEventLabStation.HivResultReceived,
                            HivResultReason = entity.PostEventLabStation.HivResultReason,
                            HivResultReceivedDateTime = entity.PostEventLabStation.HivResultReceivedDateTime,
                            HivResultMalamaUploaded = entity.PostEventLabStation.HivResultMalamaUploaded,
                            HivResultMalamaUploadedFileName = entity.PostEventLabStation.HivResultMalamaUploadedFileName,
                            HivResultMalamaUploadedDateTime = entity.PostEventLabStation.HivResultMalamaUploadedDateTime,
                            HivResultHRRUploaded = entity.PostEventLabStation.HivResultHRRUploaded,
                            HivResultHRRUploadedDateTime = entity.PostEventLabStation.HivResultHRRUploadedDateTime,
                            HivResultSORUploaded = entity.PostEventLabStation.HivResultSORUploaded,
                            HivResultSORUploadedDateTime = entity.PostEventLabStation.HivResultSORUploadedDateTime,

                            // Pregnancy
                            PregnancyResultReceived = entity.PostEventLabStation.PregnancyResultReceived,
                            PregnancyResultReason = entity.PostEventLabStation.PregnancyResultReason,
                            PregnancyResultReceivedDateTime = entity.PostEventLabStation.PregnancyResultReceivedDateTime,
                            PregnancyResultMalamaUploaded = entity.PostEventLabStation.PregnancyResultMalamaUploaded,
                            PregnancyResultMalamaUploadedFileName = entity.PostEventLabStation.PregnancyResultMalamaUploadedFileName,
                            PregnancyResultMalamaUploadedDateTime = entity.PostEventLabStation.PregnancyResultMalamaUploadedDateTime,
                            PregnancyResultHRRUploaded = entity.PostEventLabStation.PregnancyResultHRRUploaded,
                            PregnancyResultHRRUploadedDateTime = entity.PostEventLabStation.PregnancyResultHRRUploadedDateTime,
                            PregnancyResultSORUploaded = entity.PostEventLabStation.PregnancyResultSORUploaded,
                            PregnancyResultSORUploadedDateTime = entity.PostEventLabStation.PregnancyResultSORUploadedDateTime,

                            // SickleCell
                            SickleCellResultReceived = entity.PostEventLabStation.SickleCellResultReceived,
                            SickleCellResultReason = entity.PostEventLabStation.SickleCellResultReason,
                            SickleCellResultReceivedDateTime = entity.PostEventLabStation.SickleCellResultReceivedDateTime,
                            SickleCellResultMalamaUploaded = entity.PostEventLabStation.SickleCellResultMalamaUploaded,
                            SickleCellResultMalamaUploadedFileName = entity.PostEventLabStation.SickleCellResultMalamaUploadedFileName,
                            SickleCellResultMalamaUploadedDateTime = entity.PostEventLabStation.SickleCellResultMalamaUploadedDateTime,
                            SickleCellResultHRRUploaded = entity.PostEventLabStation.SickleCellResultHRRUploaded,
                            SickleCellResultHRRUploadedDateTime = entity.PostEventLabStation.SickleCellResultHRRUploadedDateTime,
                            SickleCellResultSORUploaded = entity.PostEventLabStation.SickleCellResultSORUploaded,
                            SickleCellResultSORUploadedDateTime = entity.PostEventLabStation.SickleCellResultSORUploadedDateTime
                        }
                        : new PostEventLabStationDto
                        {
                            ServiceMembersChildId = entity.Id,
                            PostEventManagementId = entity?.ServiceMembersParent?.EventManagement?.PostEventManagement?.Id ?? 0,
                        }
                };

                _logger.LogInformation(
                    "{ClassName}.{MethodName} - DTO prepared successfully for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, serviceMembersChildId);

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}.{MethodName} - Error building DTO for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, serviceMembersChildId);

                throw;
            }
        }

        //public IQueryable<FileDataDto> GetEventDataByEventIdForLab(string eventId)
        //{
        //    try
        //    {
        //        return fileUploaderRepository.GetWithInclude(
        //            f => f.EventId == eventId && f.isDeleted != true && f.LabNeeded == AppConstants.NeededOrNA.Needed && f.CheckIn == "Yes"
        //        ).Include(f => f.LabStationRecord);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error while fetching FileData by EventId", ex);
        //    }
        //}

        public async Task<List<ServiceMembersChild>> GetEventDataByEventIdForLabHivReport(long eventId)
        {
            try
            {
                _logger.LogInformation("Fetching LabStationRecords for EventID={EventId}", eventId);

                var result = await _unitOfWork.ServiceMembersChild
                .GetWithIncludeNoTracking(
                    c => c.ServiceMembersParent.EventManagement.Id == eventId,
                    c => c.LabStationRecord,
                    c => c.ServiceMembersParent.EventManagement
                )
                .Where(c => c.LabStationRecord != null &&
                            c.LabStationRecord.HivNeeded == AppConstants.Status.Completed)
                .ToListAsync();

                _logger.LogInformation("Found {Count} LabStationRecords for EventID={EventId}", result.Count, eventId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching LabStationRecords for EventID={EventId}", eventId);
                throw;
            }
        }


        public async Task<ResponseDto> AddSingleRecordAsync(FileDataDto dto, long eventId, string addedBy)
        {
            string methodName = nameof(AddSingleRecordAsync);

            try
            {
                _logger.LogInformation("{ClassName}, {MethodName}, Add single record started for EventID: {EventID}, by User: {UserName}",
                    CLASSNAME, methodName, eventId, addedBy);

                var (parent, children) = await GetServiceMembersByEventAsync(eventId);

                if (parent == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, No ServiceMembersParent found for EventID: {EventID} by User: {UserName}",
                        CLASSNAME, methodName, eventId, addedBy);

                    return new ResponseDto
                    {
                        Success = false,
                        Message = "Service Member Parent record not found",
                        Data = null
                    };
                }

                children ??= new List<ServiceMembersChild>();

                int retryCount = 0;
                const int maxRetries = 3;
                bool isSaved = false;

                while (!isSaved && retryCount < maxRetries)
                {
                    try
                    {
                        // ✅ Generate SmId
                        var latestSmId = children.Any() ? children.Max(x => x.SmId) : 0;
                        var newSmId = latestSmId + 1;

                        dto.SmId = newSmId;

                        // ✅ Barcode generation
                        if (!string.IsNullOrWhiteSpace(dto.Barcode) && dto.Barcode.Contains("-"))
                        {
                            var parts = dto.Barcode.Split('-');
                            if (parts.Length == 2)
                            {
                                dto.Barcode = parts[0].PadLeft(5, '0') + "-" + ((long)newSmId).ToString("D5");
                            }
                        }

                        ServiceMembersChild child = new ServiceMembersChild();
                        _logger.LogInformation("{ClassName}, {MethodName}, Auto Mapping Start, ServiceMembersParent Id: {Id}", CLASSNAME, methodName, parent.Id);
                        _mapper.Map(dto, child);
                        _logger.LogInformation("{ClassName}, {MethodName}, Auto Mapping Start, ServiceMembersParent Id: {Id}", CLASSNAME, methodName, parent.Id);

                        child.ServiceMembersParentId = parent.Id;
                        child.AddedBy = addedBy;
                        child.AddedOn = DateTime.Now;
                        await _unitOfWork.ServiceMembersChild.AddAsync(child);
                        await _unitOfWork.SaveAsync();

                        isSaved = true;

                        _logger.LogInformation("{ClassName}, {MethodName}, Child inserted successfully with SmId: {SmId} for ParentId: {ParentId} by User: {UserName}",
                            CLASSNAME, methodName, newSmId, parent.Id, addedBy);
                    }
                    catch (DbUpdateException ex)
                    {
                        retryCount++;

                        _logger.LogWarning(ex, "{ClassName}, {MethodName}, SmId conflict detected. Retry attempt: {RetryCount} for EventID: {EventID} by User: {UserName}",
                            CLASSNAME, methodName, retryCount, eventId, addedBy);

                        if (retryCount >= maxRetries)
                        {
                            _logger.LogError(ex, "{ClassName}, {MethodName}, Max retry reached while inserting child for EventID: {EventID} by User: {UserName}",
                                CLASSNAME, methodName, eventId, addedBy);

                            throw;
                        }
                    }
                }

                return new ResponseDto
                {
                    Success = true,
                    Message = Messages.DataInsertSuccesfully,
                    Data = dto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Error occurred while adding single record for EventID: {EventID} by User: {UserName}",
                    CLASSNAME, methodName, eventId, addedBy);

                return new ResponseDto
                {
                    Success = false,
                    Message = Messages.DataInsertFailed,
                    Data = ex.Message
                };
            }
        }

        public async Task<ResponseDto> UpdateSingleRecordAsync(FileDataDto dto, string updatedBy)
        {
            const string methodName = nameof(UpdateSingleRecordAsync);

            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, DTO is null", CLASSNAME, methodName);
                    return new ResponseDto
                    {
                        Success = false,
                        Message = "Invalid input data.",
                        Data = null
                    };
                }

                var existingChild = await _unitOfWork.ServiceMembersChild.GetByIdAsync(dto.Id);

                if (existingChild == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, No existing record found for Id: {Id}",
                        CLASSNAME, methodName, dto.Id);

                    return new ResponseDto
                    {
                        Success = false,
                        Message = "Record not found.",
                        Data = null
                    };
                }

                _logger.LogInformation("{ClassName}, {MethodName}, Mapping DTO to existing record Id: {Id}",
                    CLASSNAME, methodName, dto.Id);

                

                string barcode = existingChild.Barcode;

                _logger.LogInformation("{ClassName}, {MethodName}, Auto Mapping Start, Record Id: {Id}", CLASSNAME, methodName, dto.Id);
                _mapper.Map(dto, existingChild);

                _logger.LogInformation("{ClassName}, {MethodName}, Auto Mapping End, Record Id: {Id}", CLASSNAME, methodName, dto.Id);
                existingChild.Barcode = barcode;
                existingChild.UpdatedBy = updatedBy;
                existingChild.UpdatedOn = DateTime.Now;

                await _unitOfWork.SaveAsync();

                _logger.LogInformation("{ClassName}, {MethodName}, Successfully updated record Id: {Id}",
                    CLASSNAME, methodName, dto.Id);

                return new ResponseDto
                {
                    Success = true,
                    Message = Messages.DataUpdatedSuccesfully,
                    //Data = child
                    Data = existingChild
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Error updating record Id: {Id}",
                    CLASSNAME, methodName, dto?.Id);

                return new ResponseDto
                {
                    Success = false,
                    Message = Messages.DataUpdateFailed,
                    Data = null
                };
            }
        }

        //public async Task<ServiceMembersChild> GetByIdAsync(long id)
        //{
        //    return await fileUploaderRepository.GetByIdAsync(id);
        //}

        public async Task<(ServiceMembersChild ServiceMembersChild, long EventId)> GetServiceMemberChildWithEventIdAsync(long serviceMemberChildId)
        {
            try
            {
                if (serviceMemberChildId <= 0)
                {
                    _logger.LogWarning("GetServiceMemberChildWithEventIdAsync called with invalid Id: {Id}", serviceMemberChildId);
                    return (null, 0);
                }

                _logger.LogInformation("Fetching ServiceMembersChild Id={Id} with EventID", serviceMemberChildId);

                var result = await _unitOfWork.ServiceMembersChild
                    .GetWithIncludeNoTracking(
                        c => c.Id == serviceMemberChildId,
                        c => c.ServiceMembersParent,
                        c => c.ServiceMembersParent.EventManagement
                    )
                    .Select(c => new
                    {
                        ServiceMembersChild = c,
                        EventId = c.ServiceMembersParent.EventManagement.Id
                    })
                    .FirstOrDefaultAsync();

                if (result == null || result.ServiceMembersChild == null)
                {
                    _logger.LogInformation("No ServiceMembersChild found with Id: {Id}", serviceMemberChildId);
                    return (null, 0);
                }

                return (result.ServiceMembersChild, result.EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching ServiceMembersChild with Id: {Id}", serviceMemberChildId);
                throw;
            }
        }

        public async Task<ServiceMembersChild> GetByIdWithInclude(long id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("GetServiceMemberWithParentAndImmunizationAsync called with invalid Id: {Id}", id);
                    return null;
                }

                _logger.LogInformation("Fetching ServiceMembersChild with Id: {Id}", id);

                var childWithHierarchy = await _unitOfWork.ServiceMembersChild
                .GetWithIncludeNoTracking(
                    c => c.Id == id,         // filter by child id
                    c => c.ServiceMembersParent,               // include parent
                    c => c.ServiceMembersParent.EventManagement, // include event from parent
                    c => c.ImmunizationRecord                   // include immunization record
                )
                .FirstOrDefaultAsync();

                if (childWithHierarchy == null)
                {
                    _logger.LogInformation("No ServiceMembersChild found with Id: {Id}", id);
                }

                return childWithHierarchy;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed fetching event data: " + ex.Message);
            }
        }

        private void ValidateMissingFields(string fileName, DataTable fileData, string[] selectedColumns, List<Dictionary<string, object>> validationErrors)
        {
            var dodIdTracker = new HashSet<string>(); // To track DOD ID values for duplicates

            for (int rowIndex = 0; rowIndex < fileData.Rows.Count; rowIndex++)
            {
                var row = fileData.Rows[rowIndex];

                // Check for missing fields
                foreach (var columnName in selectedColumns)
                {
                    if (row[columnName] == null || string.IsNullOrWhiteSpace(row[columnName].ToString()))
                    {
                        validationErrors.Add(new Dictionary<string, object>
                {
                    { "FileName", fileName },
                    { "Row", rowIndex + 1 }, // 1-based index
                    { "Column", columnName },
                    { "Error", "Missing" }
                });
                    }
                }

                // Check for duplicate DOD ID in G6PDReport.xlsx
                if (fileName.Equals("G6PDReport.xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    var dodIdValue = row["DOD ID"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(dodIdValue))
                    {
                        if (dodIdTracker.Contains(dodIdValue))
                        {
                            validationErrors.Add(new Dictionary<string, object>
                    {
                        { "FileName", fileName },
                        { "Row", rowIndex + 1 }, // 1-based index
                        { "Column", "DOD ID" },
                        { "Error", "Duplicate" }
                    });
                        }
                        else
                        {
                            dodIdTracker.Add(dodIdValue);
                        }
                    }
                }
            }
        }


        private void CheckG6PDTotalAssignedCountWithTotalRow(DataTable G6PDDataTable, long totalRecordValueInG6pdFile)
        {
            if (G6PDDataTable.Rows.Count != totalRecordValueInG6pdFile)
            {
                throw new Exception("Total assigned value in G6PD file rows count mismatched!");
            }
        }

        private DataTable RenameReorderAndProcess(DataTable dataTable, Dictionary<string, string> headerMapping, string[] outputHeaders)
        {
            // Create a new DataTable to hold the renamed, reordered columns and processed values
            DataTable reorderedTable = new DataTable();

            // Add columns to the new DataTable based on the outputHeaders array
            foreach (var newHeader in outputHeaders)
            {
                reorderedTable.Columns.Add(newHeader);
            }

            // Loop through each row in the original dataTable
            foreach (DataRow row in dataTable.Rows)
            {
                // Create a new row for the reordered table
                DataRow newRow = reorderedTable.NewRow();

                // Copy data from the original row to the new row based on the header mapping
                foreach (var column in headerMapping)
                {
                    if (dataTable.Columns.Contains(column.Key))
                    {
                        var originalValue = row[column.Key];

                        // Check if the value is null or DBNull and replace with an empty string if necessary
                        if (originalValue == null || originalValue is DBNull)
                        {
                            newRow[column.Value] = string.Empty;
                        }
                        // If the value is a non-string object, convert it to a string
                        else if (originalValue.GetType().IsClass && originalValue.GetType() != typeof(string))
                        {
                            newRow[column.Value] = originalValue.ToString();
                        }
                        // Otherwise, just copy the original value
                        else
                        {
                            newRow[column.Value] = originalValue;
                        }
                    }
                }

                // Add the newly reordered and processed row to the reordered DataTable
                reorderedTable.Rows.Add(newRow);
            }

            return reorderedTable;
        }

        private void SetFileFlags(string fileName, out bool isVisionFile, out bool isDentalFile, out bool isPhaFile, out bool isHivFile, out bool isHearingFile, out bool isTaskForceFile, out bool isBloodFile, out bool isUmrFile, out bool isSickleFile, out bool isImmunizationFile)
        {
            isVisionFile = false;
            isDentalFile = false;
            isPhaFile = false;
            isHivFile = false;
            isHearingFile = false;
            isTaskForceFile = false;
            isBloodFile = false;
            isUmrFile = false;
            isSickleFile = false;
            isImmunizationFile = false;

            switch (fileName)
            {
                case "VisionReadinessReport.xlsx":
                    isVisionFile = true;
                    break;

                case "DentalReadinessReport.xlsx":
                    isDentalFile = true;
                    break;

                case "Pha_Report.xlsx":
                    isPhaFile = true;
                    break;

                case "HIVReadinessSummaryReport.xlsx":
                    isHivFile = true;
                    break;

                case "HearingReadinessReport.xlsx":
                    isHearingFile = true;
                    break;
                case "Taskforce_Roster.xlsx":
                    isTaskForceFile = true;
                    break;
                case "BloodTypeReport.xlsx":
                    isBloodFile = true;
                    break;
                case "UMR.xlsx":
                    isUmrFile = true;
                    break;
                case "SickleCellReport.xlsx":
                    isSickleFile = true;
                    break;
                case "ImmunizationProfileReport.xlsx":
                    isImmunizationFile = true;
                    break;

                default:
                    // Handle cases where the file name does not match any known reports
                    break;
            }
        }
        private void ProcessExcelFiles(List<IFormFile> files, DataTable? G6PDDataTable, List<string>? processingSequence, Dictionary<string, (string[] Headers, string[] SelectedColumns, string stoppingKeyword)> fileConfigurations, DateTime? parsedLastEventDate, DateTime visionDate, DateTime dentalDate, DateTime phaDate, DateTime hivDate, DateTime hearingDate, DateTime parsedEventDate, int lastDentalExam, int vision, int dental, int pha, int hiv, int hearing, string eventId)
        {
            foreach (var fileName in processingSequence)
            {
                if (fileName != "G6PDReport.xlsx")
                {
                    IFormFile file = null;

                    if (fileName.StartsWith("Taskforce_Roster", StringComparison.OrdinalIgnoreCase))
                    {
                        file = files.FirstOrDefault(f => Regex.IsMatch(f.FileName, @"^Taskforce_Roster.*\.xlsx$", RegexOptions.IgnoreCase));
                    }
                    else
                    {
                        file = files.FirstOrDefault(f => f.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase));
                    }

                    if (file == null) continue;

                    var fileConfig = fileConfigurations[fileName];
                    DataTable fileData = GetDataFromFile(file, fileConfig.Headers, fileConfig.SelectedColumns, fileConfig.stoppingKeyword, isG6PD: false, out string taskForceValue, out long totalRecordValueInG6pdFile);
                    bool isVisionFile = false; bool isDentalFile = false; bool isPhaFile = false; bool isHivFile = false; bool isHearingFile = false; bool isTaskForceFile = false; bool isBloodFile = false; bool isUmrFile = false; bool isSickleFile = false; bool isImmunizationFile = false;
                    SetFileFlags(fileName, out isVisionFile, out isDentalFile, out isPhaFile, out isHivFile, out isHearingFile, out isTaskForceFile, out isBloodFile, out isUmrFile, out isSickleFile, out isImmunizationFile);
                    MergeDataTables(G6PDDataTable, fileData, fileConfig.SelectedColumns, lastDentalExam, parsedLastEventDate, visionDate, dentalDate, phaDate, hivDate, hearingDate, isVisionFile, isDentalFile, isPhaFile, isHivFile, isHearingFile, isTaskForceFile, isBloodFile, isUmrFile, isSickleFile, isImmunizationFile, parsedEventDate, vision, dental, pha, hiv, hearing, eventId);
                }
            }
        }
        private string[] GetPredefinedColumnHeader()
        {
            return new string[]
                {
                //"SM ID",
                "FULL NAME",
                //"NAME",
                "FULL SSN",
                "LAST 4",
                "DOD ID",
                "RANK",
                "AGE",
                "SEX",
                "MOS",
                "AGR",
                "UIC",
                "MRC",
                "DOB",
                "OVER 40",
                "Dental Due",
                "Dental Exam",
                "Dental Needed",
                "PANO Needed",
                "BWX Needed",
                "DRC",
                "PHA Date",
                "PHA Due",
                "PHA Needed",
                "PULHES",
                "Vision Date",
                "VISION Needed",
                "NEAR VISION Needed",
                "VRC",
                "Vision 2PG",
                "Vision Mask Insert",
                "Hearing Date",
                "HEARING Needed",
                "HRC",
                "Hearing Profile",
                "Lab Requisition",
                "Lab Needed",
                "ABO",
                "ABO Needed",
                "DNA",
                "Sickle Date",
                "SICKLE",
                "G6PD",
                "G6PD Date",
                "G6PD Status",
                "HIV NEXT TEST DATE",
                "HIV",
                "Lipid Needed",
                "LIPID PANEL",
                "Cholesterol / HDL Cholesterol",
                "Framingham",
                "EKG (Date)",
                "EKG NEEDED",
                 "Pregnancy Test Needed",
                "IMM Needed",
                "Hep B Needed",
                "Hep A Needed",
                "FLU Needed",
                "Tet/TDP Needed",
                "MMR Needed",
                "Varicella Needed",
                "TaskForce",
                "Notes",
                "Over 44",
                "EventDate",
                "Event End Date",
                "EventID",
                "Vision Win",
                "Dental Win",
                "PHA Win",
                "HIV Win",
                "Hearing WIN",
                "Barcode"
                };
        }

        private Dictionary<string, string>? GetHeaderMapping()
        {
            return new Dictionary<string, string>
            {
                 //{ "SM ID", "SM ID" },
                  //{ "ONLY_NAME", "NAME" },
                { "Name", "FULL NAME" },
                { "FULL SSN", "FULL SSN" },
                { "SSN", "LAST 4" },
                { "DOD ID", "DOD ID" },
                { "Rank", "RANK" },
                { "Age", "AGE" },
                { "DOB", "DOB" },
                { "OVER 40", "OVER 40" },
                { "Sex", "SEX" },
                { "MOS", "MOS" },
                { "AGR", "AGR" },
                { "UIC", "UIC" },
                { "MRC", "MRC" },
                { "Date of Next Exam", "Dental Due" },
                { "Dental Exam", "Dental Exam" },
                { "Dental Needed", "Dental Needed" },
                { "PANO Needed", "PANO Needed" },
                { "BWX Needed", "BWX Needed" },
                { "DRC", "DRC" },
                { "PHA Date", "PHA Date" },
                { "Next PHA Date", "PHA Due" },
                { "PHA Needed", "PHA Needed" },
                { "PULHES Code", "PULHES" },
                { "Date of Vision Screen", "Vision Date" },
                { "VISION Needed", "VISION Needed" },
                { "NEAR VISION Needed", "NEAR VISION Needed" },
                { "VRC", "VRC" },
                { "2PG", "Vision 2PG" },
                { "1MI", "Vision Mask Insert" },
                { "Audiogram Date", "Hearing Date" },
                { "HEARING Needed", "HEARING Needed" },
                { "HRC", "HRC" },
                { "Hearing Profile", "Hearing Profile" },
                { "Lab Requisition", "Lab Requisition" },
                { "Lab Needed", "Lab Needed" },

                { "Blood Type", "ABO" },
                { "ABO Needed", "ABO Needed" },
                { "DNA", "DNA" },
                { "Sickle Cell Date", "Sickle Date" },
                { "SICKLE", "SICKLE" },
                { "G6PD", "G6PD" },
                { "G6PD Date", "G6PD Date" },
                { "G6PD Status", "G6PD Status" },
                { "Next Test Date", "HIV NEXT TEST DATE" },
                { "HIV", "HIV" },
                { "Lipid Needed", "Lipid Needed" },
                { "LIPID PANEL", "LIPID PANEL" },
                { "Cholesterol / HDL Cholesterol", "Cholesterol / HDL Cholesterol" },
                { "Framingham", "Framingham" },
                { "EKG (Date)", "EKG (Date)" },
                { "EKG NEEDED", "EKG NEEDED" },
                  { "Pregnancy Test Needed", "Pregnancy Test Needed" },
                { "IMM Needed", "IMM Needed" },
                { "Hep B Needed", "Hep B Needed" },
                { "Hep A Needed", "Hep A Needed" },
                { "FLU Needed", "FLU Needed" },
                { "Tet/TDP Needed", "Tet/TDP Needed" },
                { "MMR Needed", "MMR Needed" },
                { "Varicella Needed", "Varicella Needed" },
                { "TaskForce","TaskForce"},
                { "Over 44", "Over 44" },
                { "Notes", "Notes" },
                { "EventDate", "EventDate" },
                { "Event End Date", "Event End Date" },
                { "EventID", "EventID" },
                { "Vision Win", "Vision Win" },
                { "Dental Win", "Dental Win" },
                { "PHA Win", "PHA Win" },
                { "HIV Win", "HIV Win" },
                { "Hearing WIN", "Hearing WIN" },
                { "Barcode", "Barcode" }
            };
        }

        private Dictionary<string, (string[] Headers, string[] SelectedColumns, string stoppingKeyword)> GetFileConfiguration()
        {
            return new Dictionary<string, (string[] Headers, string[] SelectedColumns, string stoppingKeyword)>
            {
                { "G6PDReport.xlsx", (new string[] { "Name","Rank","IMR Exclusion","SSN","DOD ID","Age","ACO/MOS","UIC","Location","CMP","G6PD Date","G6PD Status" }, new string[] { "DOD ID", "Name","SSN","G6PD Date","G6PD Status" }, "") },
                { "Taskforce_Roster.xlsx", (new string[] { "Name","Rank","SSN","Sex","MOS","MPC","UIC","DOB","Type" }, new string[] { "Name", "SSN", "DOB" }, "") },
                { "ImmunizationProfileReport.xlsx", (new string[] { "Name","Rank","SSN","CMP","UIC","AOC/MOS","HB3","HPA","MMR","TDP","TDA","VAR" }, new string[] { "Name", "SSN", "HB3","HPA","INJ", "MMR","TDP","TDA","VAR" }, "Immunization Profile Report") },
                { "Pha_Report.xlsx", (new string[] { "Cbt Dfrmnt","Name","Rank","IMR Exclusion","SSN","Age","AOC/MOS","UIC","Location","CMP","PHA Date","Next PHA Date","PHA Completion Status","PULHES Code" }, new string[] { "Name", "SSN", "PHA Date","Next PHA Date","PULHES Code" }, "") },
                { "DentalReadinessReport.xlsx", (new string[] { "Cbt","Dfrmnt","DRC","Reason","Date of Next Exam","Panx","Name","DOD ID","Rank","IMR Exclusion","AOC/MOS","UIC","Station","CMP" }, new string[] { "DOD ID", "DRC","Reason","Date of Next Exam","Panx" }, "" ) },
                { "VisionReadinessReport.xlsx", (new string[] { "CBT Dfrmnt","VRC","Date of Vision Screen","2PG","1MI","MCEP","MCEPI","MRCL","Waiver","Name","DOD ID","Age","Rank","AOC/MOS","UIC","Station","Compo" }, new string[] { "DOD ID", "VRC","Date of Vision Screen","2PG","1MI" }, "") },
                { "SickleCellReport.xlsx", (new string[] { "Name","Rank","IMR Exclusion","SSN","DOD ID","Age","Sex","MOS","UIC","Location","CMP","Sickle Cell Status","Sickle Cell Date" }, new string[] { "DOD ID", "Sickle Cell Date" }, "") },
                { "HIVReadinessSummaryReport.xlsx", (new string[] { "Cbt Dfrmnt","HIV Test Current","Name","SSN","DOD ID","Rank","IMR Exclusion","AOC/MOS","UIC","CMP","AGR","Test Date","Next Test Date" }, new string[] { "DOD ID", "Next Test Date" }, "") },
                { "UMR.xlsx", (new string[] {  "Name", "DOD ID", "Rank", "IMR Exclusion", "Age", "Sex", "MOS", "CMP", "AGR", "UIC", "Location", "MRC", "Deployable", "MR2-DNA", "MR2-HIV", "MR2-IMM", "MR2-IME", "MR2-Temp>=8<=30", "MR2-HRC4", "MR2-VRC4", "MR3-Temp>=31 (DL1)", "MR3-DRC3 (DL2)","MR3-PRG (DL3)", "MR3-MAR2 (DL4)", "MR3-MEB (DL5)", "MR3-ND-PEB (DL6)","MR3-PERM (DL7)", "Dental Exam (DRC4)", "MR4-PHA" }, new string[] { "Name","DOD ID","Rank","Age","Sex","MOS","CMP","AGR","UIC","Location","MRC","MR2-DNA", "MR2-IMM" }, "") },
                { "HearingReadinessReport.xlsx", (new string[] { "Cbt Dfrmnt/Separation","HRC","Status","Audiogram Date","Earplugs Issued","Hearing Protection Type","Hearing Aid w/ Battery","Name","SSN","DOD ID","Rank","AOC/MOS","UIC","Location","CMP" }, new string[] { "DOD ID", "HRC","Status","Audiogram Date" }, "") },
                { "BloodTypeReport.xlsx", (new string[] { "R/G","Name","Rank","IMR Exclusion Code","SSN","DOD ID","Age","Sex","MOS","UIC","Location","CMP","Blood Type" }, new string[] { "DOD ID", "Blood Type" }, "") }

            };
        }

        private Dictionary<string, (string[] Headers, string[] SelectedColumns, string stoppingKeyword)> GetFileConfigurationForChecking()
        {
            return new Dictionary<string, (string[] Headers, string[] SelectedColumns, string stoppingKeyword)>
            {
                { "G6PDReport.xlsx", (new string[] { "Name","Rank","IMR Exclusion","SSN","DOD ID","Age","ACO/MOS","UIC","Location","CMP","G6PD Date","G6PD Status" }, new string[] { "DOD ID", "Name","SSN" }, "") },
                { "Taskforce_Roster.xlsx", (new string[] { "Name","Rank","SSN", "Sex", "MOS","MPC","UIC","DOB","Type" }, new string[] { "Name", "SSN", "DOB", "Rank", "Sex" }, "") },
                { "ImmunizationProfileReport.xlsx", (new string[] { "Name","Rank","SSN","CMP","UIC","AOC/MOS","HB3","HPA","MMR","TDP","TDA","VAR" }, new string[] { "Name", "SSN" }, "Immunization Profile Report") },
                { "Pha_Report.xlsx", (new string[] { "Cbt Dfrmnt","Name","Rank","IMR Exclusion","SSN","Age","AOC/MOS","UIC","Location","CMP","PHA Date","Next PHA Date","PHA Completion Status","PULHES Code" }, new string[] { "Name","SSN"  }, "") },
                { "DentalReadinessReport.xlsx", (new string[] { "Cbt","Dfrmnt","DRC","Reason","Date of Next Exam","Panx","Name","DOD ID","Rank","IMR Exclusion","AOC/MOS","UIC","Station","CMP" }, new string[] { "DOD ID", "DRC", "Name" }, "" ) },
                { "VisionReadinessReport.xlsx", (new string[] { "CBT Dfrmnt","VRC","Date of Vision Screen","2PG","1MI","MCEP","MCEPI","MRCL","Waiver","Name","DOD ID","Age","Rank","AOC/MOS","UIC","Station","Compo" }, new string[] { "DOD ID", "VRC","Name" }, "") },
                { "SickleCellReport.xlsx", (new string[] { "Name","Rank","IMR Exclusion","SSN","DOD ID","Age","Sex","MOS","UIC","Location","CMP","Sickle Cell Status","Sickle Cell Date" }, new string[] {  "DOD ID", "Name","SSN" }, "") },
                { "HIVReadinessSummaryReport.xlsx", (new string[] { "Cbt Dfrmnt","HIV Test Current","Name","SSN","DOD ID","Rank","IMR Exclusion","AOC/MOS","UIC","CMP","AGR","Test Date","Next Test Date" }, new string[] { "DOD ID", "Name","SSN" }, "") },
                { "UMR.xlsx", (new string[] {  "Name", "DOD ID", "Rank", "IMR Exclusion", "Age", "Sex", "MOS", "CMP", "AGR", "UIC", "Location", "MRC", "Deployable", "MR2-DNA", "MR2-HIV", "MR2-IMM", "MR2-IME", "MR2-Temp>=8<=30", "MR2-HRC4", "MR2-VRC4", "MR3-Temp>=31 (DL1)", "MR3-DRC3 (DL2)","MR3-PRG (DL3)", "MR3-MAR2 (DL4)", "MR3-MEB (DL5)", "MR3-ND-PEB (DL6)","MR3-PERM (DL7)", "Dental Exam (DRC4)", "MR4-PHA" }, new string[] { "Name","DOD ID","Rank","Sex","MOS","AGR", "UIC" }, "") },
                { "HearingReadinessReport.xlsx", (new string[] { "Cbt Dfrmnt/Separation","HRC","Status","Audiogram Date","Earplugs Issued","Hearing Protection Type","Hearing Aid w/ Battery","Name","SSN","DOD ID","Rank","AOC/MOS","UIC","Location","CMP" }, new string[] { "DOD ID", "Name","SSN" }, "") },
                { "BloodTypeReport.xlsx", (new string[] { "R/G","Name","Rank","IMR Exclusion Code","SSN","DOD ID","Age","Sex","MOS","UIC","Location","CMP","Blood Type" }, new string[] { "DOD ID", "Name","SSN" }, "") }
            };
        }

        private List<string>? GetProcessingSequence()
        {
            return new List<string>
            {
                "G6PDReport.xlsx",
                "Taskforce_Roster.xlsx",
                "DentalReadinessReport.xlsx",
                "ImmunizationProfileReport.xlsx",
                "VisionReadinessReport.xlsx",
                "SickleCellReport.xlsx",
                "HIVReadinessSummaryReport.xlsx",
                "UMR.xlsx",
                "HearingReadinessReport.xlsx",
                "BloodTypeReport.xlsx",
                "Pha_Report.xlsx"
            };
        }

        private void MergeSomeMoreColumns(DataTable parentTable, string taskForceValue, long eventId)
        {
            if (!parentTable.Columns.Contains("TaskForce"))
            {
                parentTable.Columns.Add("TaskForce", typeof(string));
            }

            if (!parentTable.Columns.Contains("Lab Requisition"))
            {
                parentTable.Columns.Add("Lab Requisition", typeof(string));
            }

            if (!parentTable.Columns.Contains("Lab Needed"))
            {
                parentTable.Columns.Add("Lab Needed", typeof(string));
            }

            if (!parentTable.Columns.Contains("Barcode"))
            {
                parentTable.Columns.Add("Barcode", typeof(string));
            }

            string paddedEventId = eventId.ToString("D5");

            foreach (DataRow parentRow in parentTable.Rows)
            {
                parentRow["TaskForce"] = taskForceValue;

                bool commonNeeded =
                    (parentTable.Columns.Contains("ABO Needed") && parentRow["ABO Needed"]?.ToString().Trim() == AppConstants.NeededOrNA.Needed) ||
                    (parentTable.Columns.Contains("G6PD") && parentRow["G6PD"]?.ToString().Trim() == AppConstants.NeededOrNA.Needed) ||
                    (parentTable.Columns.Contains("SICKLE") && parentRow["SICKLE"]?.ToString().Trim() == AppConstants.NeededOrNA.Needed) ||
                    (parentTable.Columns.Contains("LIPID NEEDED") && parentRow["LIPID NEEDED"]?.ToString().Trim() == AppConstants.NeededOrNA.Needed);

                bool additionalLabNeeded =
                    (parentTable.Columns.Contains("HIV") && parentRow["HIV"]?.ToString().Trim() == AppConstants.NeededOrNA.Needed) ||
                    (parentTable.Columns.Contains("DNA") && parentRow["DNA"]?.ToString().Trim() == AppConstants.NeededOrNA.Needed);

                if (commonNeeded)
                {
                    parentRow["Lab Requisition"] = AppConstants.NeededOrNA.Needed;
                }
                else
                {
                    parentRow["Lab Requisition"] = AppConstants.NeededOrNA.NotApplicable;
                }

                if (commonNeeded || additionalLabNeeded)
                {
                    parentRow["Lab Needed"] = AppConstants.NeededOrNA.Needed;
                }
                else
                {
                    parentRow["Lab Needed"] = AppConstants.NeededOrNA.NotApplicable;
                }

                parentRow["Barcode"] = $"{paddedEventId}";
            }

        }

        private List<Dictionary<string, object>> DataTableToJson(DataTable table)
        {
            var result = new List<Dictionary<string, object>>();

            foreach (DataRow row in table.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn column in table.Columns)
                {
                    dict[column.ColumnName] = row[column];
                }
                result.Add(dict);
            }

            return result;
        }

        private DataTable GetDataFromFile(IFormFile file, string[] headers, string[] selectedColumns, string stopKeyword, bool isG6PD, out string taskForceValue, out long totalRecordValueInG6pdFile)
        {
            DataTable dataTable = new DataTable();
            taskForceValue = string.Empty;
            totalRecordValueInG6pdFile = 0;

            using (var stream = file.OpenReadStream())
            {
                IWorkbook workbook = new XSSFWorkbook(stream); // For .xlsx; use HSSFWorkbook for .xls
                ISheet sheet = workbook.GetSheetAt(0); // Get the first sheet

                var evaluator = workbook.GetCreationHelper().CreateFormulaEvaluator(); // Formula evaluator

                IRow headerRow = null;
                bool headerFound = false;
                int headerRowIndex = 0;

                // Find header row
                for (int i = 0; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row != null)
                    {
                        for (int j = 0; j < row.LastCellNum; j++)
                        {
                            ICell cell = row.GetCell(j);
                            string cellValue = GetCellValue(cell, evaluator);

                            if (isG6PD && string.IsNullOrWhiteSpace(taskForceValue))
                            {
                                if (!string.IsNullOrEmpty(cellValue) && NormalizeWhiteSpace(cellValue).Contains("G6PD Report by Taskforce:"))
                                {
                                    taskForceValue = ExtractTaskForceValue(cellValue);
                                }
                            }

                            if (headers.Contains(cellValue))
                            {
                                headerRow = row;
                                headerRowIndex = i;
                                headerFound = true;
                                break;
                            }
                        }
                    }

                    if (headerFound) break;
                }

                if (headerRow == null)
                    throw new Exception("Header row not found!");

                if (isG6PD && totalRecordValueInG6pdFile == 0)
                {
                    totalRecordValueInG6pdFile = GetTotalAssignedRecordFromG6pdFile(sheet);
                }

                // Add selected columns to the DataTable
                foreach (var column in selectedColumns)
                {
                    dataTable.Columns.Add(column);
                }

                // Extract data from rows after the header
                for (int i = headerRowIndex + 1; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null || IsStopRow(row, stopKeyword)) break;

                    DataRow dataRow = dataTable.NewRow();

                    for (int j = 0; j < row.LastCellNum; j++)
                    {
                        ICell cell = row.GetCell(j);
                        string headerValue = GetCellValue(headerRow.GetCell(j), evaluator);

                        if (selectedColumns.Contains(headerValue))
                        {
                            string cellValue = GetCellValue(cell, evaluator);

                            if (dateHeaderNames.Contains(headerValue))
                            {
                                if (DateTime.TryParse(cellValue, out DateTime parsedDate))
                                {
                                    dataRow[headerValue] = parsedDate.ToString(DateFormat);
                                }
                                else
                                {
                                    dataRow[headerValue] = cellValue;
                                }
                            }
                            else
                            {
                                dataRow[headerValue] = cellValue;
                            }
                        }
                    }

                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }

        private string GetCellValue(ICell cell, IFormulaEvaluator evaluator)
        {
            if (cell == null) return string.Empty;

            switch (cell.CellType)
            {
                case CellType.String:
                    return cell.StringCellValue?.Trim() ?? string.Empty;

                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                        return cell.DateCellValue.ToString(); // You can format as needed
                    return cell.NumericCellValue.ToString();

                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();

                case CellType.Formula:
                    var value = evaluator.Evaluate(cell);
                    if (value == null) return string.Empty;

                    return value.CellType switch
                    {
                        CellType.String => value.StringValue,
                        CellType.Numeric => value.NumberValue.ToString(),
                        CellType.Boolean => value.BooleanValue.ToString(),
                        _ => string.Empty,
                    };

                case CellType.Blank:
                case CellType.Error:
                default:
                    return string.Empty;
            }
        }

        private bool IsPossibleDate(string value)
        {
            // You can add more checks here based on what date formats you expect
            return DateTime.TryParse(value, out _);
        }

        private string NormalizeWhiteSpace(string input)
        {
            return string.Join(" ", input.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private string ExtractTaskForceValue(string cellValue)
        {
            int startIndex = cellValue.IndexOf("Taskforce:") + "Taskforce:".Length;
            int endIndex = cellValue.IndexOf(",", startIndex);

            if (startIndex > -1 && endIndex > -1)
            {
                return cellValue.Substring(startIndex, endIndex - startIndex).Trim();
            }

            return string.Empty; // Return an empty string if not found
        }

        private long GetTotalAssignedRecordFromG6pdFile(ISheet sheet)
        {
            string targetKey = "Total Assigned";

            for (int row = 0; row <= sheet.LastRowNum; row++)
            {
                IRow currentRow = sheet.GetRow(row);

                if (currentRow != null)
                {
                    for (int col = 0; col < currentRow.LastCellNum; col++)
                    {
                        ICell keyCell = currentRow.GetCell(col);

                        if (keyCell != null && keyCell.ToString() == targetKey)
                        {
                            IRow nextRow = sheet.GetRow(row + 1);
                            if (nextRow != null)
                            {
                                ICell valueCell = nextRow.GetCell(col);
                                if (valueCell != null)
                                {
                                    try
                                    {
                                        return Convert.ToInt64(valueCell.ToString());
                                    }
                                    catch (FormatException)
                                    {
                                        throw new Exception("Total assigned record not found in G6PD file or in different format!");
                                    }
                                    catch (OverflowException)
                                    {
                                        throw new Exception("Total assigned record not found in G6PD file or in different format!");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return 0;
        }

        private bool IsStopRow(IRow row, string stopKeyword)
        {
            bool isBlankRow = row == null || row.Cells.All(cell =>
        cell == null ||
        cell.CellType == CellType.Blank ||
        (cell.CellType == CellType.String && string.IsNullOrWhiteSpace(cell.StringCellValue)) ||
        (cell.CellType == CellType.Formula && string.IsNullOrWhiteSpace(cell.StringCellValue))
    );

            bool hasStopKeyword = !string.IsNullOrEmpty(stopKeyword) &&
                                  row.Cells.Any(c => c.CellType == CellType.String &&
                                                     c.StringCellValue.IndexOf(stopKeyword, StringComparison.OrdinalIgnoreCase) >= 0);

            return isBlankRow || hasStopKeyword;
        }

        private void MergeDataTables(DataTable parentTable, DataTable childTable, string[] mergeColumns, int lastDentalExamDate, DateTime? lastEventDate, DateTime visionDate, DateTime dentalDate, DateTime phaDate,
                             DateTime hivDate, DateTime hearingDate, bool isVisionFile, bool isDentalFile, bool isPhaFile, bool isHivFile,
                             bool isHearingFile, bool isTaskForceFile, bool isBloodFile, bool isUmrFile,
                             bool isSickleFile, bool isImmunizationFile, DateTime eventDate, int vision, int dental, int pha, int hiv, int hearing, string eventId)
        {

            AddCalculatedFieldColumns(parentTable);

            //int rowNumber = 1;

            foreach (DataRow parentRow in parentTable.Rows)
            {
                //parentRow["SM ID"] = rowNumber++;
                //parentRow["ONLY_NAME"] = string.Empty;

                DataRow matchingChildRow = GetMatchingChildRow(parentTable, childTable, parentRow);

                if (matchingChildRow != null)
                {
                    MergeRows(parentTable, childTable, mergeColumns, matchingChildRow, parentRow, isTaskForceFile);


                    if (isVisionFile)
                    {
                        HandleVisionFile(parentRow, visionDate);
                    }

                    if (isDentalFile)
                    {
                        HandleDentalFile(parentRow, dentalDate, lastDentalExamDate);
                    }

                    if (isPhaFile)
                    {
                        HandlePhaFile(parentRow, phaDate);
                    }

                    if (isHivFile)
                    {
                        HandleHivFile(parentRow, hivDate);
                    }

                    if (isHearingFile)
                    {
                        HandleHearingFile(parentRow, hearingDate);
                    }

                    if (isTaskForceFile)
                    {
                        HandleTaskForceFile(parentRow, eventDate);
                    }

                    if (isBloodFile)
                    {
                        HandleBloodFile(parentRow);
                    }

                    if (isUmrFile)
                    {
                        HandleUmrFile(parentRow);
                    }

                    if (isSickleFile)
                    {
                        HandleSickleFile(parentRow);
                    }

                    if (isImmunizationFile)
                    {
                        HandleImmunizationFile(parentRow, lastEventDate);
                    }

                    HandleOtherFields(parentRow, eventDate, lastEventDate, vision, dental, pha, hiv, hearing, eventId);
                }
            }
        }

        private DataRow GetMatchingChildRow(DataTable parentTable, DataTable childTable, DataRow parentRow)
        {
            DataRow matchingChildRow = null;

            if (parentTable.Columns.Contains("DOD ID") && childTable.Columns.Contains("DOD ID"))
            {
                matchingChildRow = childTable.Rows.Cast<DataRow>()
                    .FirstOrDefault(row => row["DOD ID"]?.ToString() == parentRow["DOD ID"]?.ToString());
            }

            if (matchingChildRow == null && parentTable.Columns.Contains("Name") && parentTable.Columns.Contains("SSN") && childTable.Columns.Contains("Name") && childTable.Columns.Contains("SSN"))
            {
                matchingChildRow = childTable.Rows.Cast<DataRow>()
                    .FirstOrDefault(row => row["Name"]?.ToString() == parentRow["Name"]?.ToString() &&
                    row["SSN"]?.ToString()?.Substring(Math.Max(0, row["SSN"].ToString().Length - 4)) ==
                    parentRow["SSN"]?.ToString()?.Substring(Math.Max(0, parentRow["SSN"].ToString().Length - 4)));
            }

            return matchingChildRow;
        }

        private void MergeRows(DataTable parentTable, DataTable childTable, string[] mergeColumns, DataRow matchingChildRow, DataRow parentRow, bool isTaskForceFile)
        {
            foreach (var column in mergeColumns)
            {
                if (childTable.Columns.Contains(column))
                {
                    if (isTaskForceFile && column == "SSN")
                    {
                        parentRow["FULL SSN"] = matchingChildRow[column];
                    }
                    else if (column != "DOD ID" && column != "Name" && column != "SSN")
                    {
                        if (parentTable.Columns.Contains(column))
                        {
                            if (parentRow[column] == DBNull.Value || string.IsNullOrEmpty(parentRow[column].ToString()))
                            {
                                parentRow[column] = matchingChildRow[column];
                            }
                        }
                        else
                        {
                            parentTable.Columns.Add(column, childTable.Columns[column].DataType);
                            parentRow[column] = matchingChildRow[column];
                        }
                    }
                }
            }
        }

        private void AddCalculatedFieldColumns(DataTable parentTable)
        {
            //if (!parentTable.Columns.Contains("SM ID"))
            //    parentTable.Columns.Add("SM ID", typeof(int));

            // Add "NAME" column if it doesn't already exist (initially blank)
            //if (!parentTable.Columns.Contains("ONLY_NAME"))
            //    parentTable.Columns.Add("ONLY_NAME", typeof(string));

            if (!parentTable.Columns.Contains("FULL SSN"))
                parentTable.Columns.Add("FULL SSN", typeof(string));

            if (!parentTable.Columns.Contains("VISION Needed"))
                parentTable.Columns.Add("VISION Needed", typeof(string));

            if (!parentTable.Columns.Contains("Dental Needed"))
                parentTable.Columns.Add("Dental Needed", typeof(string));

            if (!parentTable.Columns.Contains("Dental Exam"))
                parentTable.Columns.Add("Dental Exam", typeof(string));

            if (!parentTable.Columns.Contains("PHA Needed"))
                parentTable.Columns.Add("PHA Needed", typeof(string));

            if (!parentTable.Columns.Contains("HIV"))
                parentTable.Columns.Add("HIV", typeof(string));

            if (!parentTable.Columns.Contains("HEARING Needed"))
                parentTable.Columns.Add("HEARING Needed", typeof(string));

            if (!parentTable.Columns.Contains("PANO NEEDED"))
                parentTable.Columns.Add("PANO NEEDED", typeof(string));

            if (!parentTable.Columns.Contains("BWX Needed"))
                parentTable.Columns.Add("BWX Needed", typeof(string));

            if (!parentTable.Columns.Contains("Hearing Profile"))
                parentTable.Columns.Add("Hearing Profile", typeof(string));

            if (!parentTable.Columns.Contains("ABO Needed"))
                parentTable.Columns.Add("ABO Needed", typeof(string));

            if (!parentTable.Columns.Contains("DNA"))
                parentTable.Columns.Add("DNA", typeof(string));

            if (!parentTable.Columns.Contains("SICKLE"))
                parentTable.Columns.Add("SICKLE", typeof(string));

            if (!parentTable.Columns.Contains("G6PD"))
                parentTable.Columns.Add("G6PD", typeof(string));

            if (!parentTable.Columns.Contains("Lipid Needed"))
                parentTable.Columns.Add("Lipid Needed", typeof(string));

            if (!parentTable.Columns.Contains("LIPID PANEL"))
                parentTable.Columns.Add("LIPID PANEL", typeof(string));

            if (!parentTable.Columns.Contains("Cholesterol / HDL Cholesterol"))
                parentTable.Columns.Add("Cholesterol / HDL Cholesterol", typeof(string));

            if (!parentTable.Columns.Contains("Framingham"))
                parentTable.Columns.Add("Framingham", typeof(string));

            if (!parentTable.Columns.Contains("EKG (Date)"))
                parentTable.Columns.Add("EKG (Date)", typeof(string));

            if (!parentTable.Columns.Contains("EKG NEEDED"))
                parentTable.Columns.Add("EKG NEEDED", typeof(string));

            if (!parentTable.Columns.Contains("Pregnancy Test Needed"))
                parentTable.Columns.Add("Pregnancy Test Needed", typeof(string));

            if (!parentTable.Columns.Contains("IMM Needed"))
                parentTable.Columns.Add("IMM Needed", typeof(string));

            if (!parentTable.Columns.Contains("Hep B Needed"))
                parentTable.Columns.Add("Hep B Needed", typeof(string));

            if (!parentTable.Columns.Contains("Hep A Needed"))
                parentTable.Columns.Add("Hep A Needed", typeof(string));

            if (!parentTable.Columns.Contains("FLU Needed"))
                parentTable.Columns.Add("FLU Needed", typeof(string));

            if (!parentTable.Columns.Contains("Tet/TDP Needed"))
                parentTable.Columns.Add("Tet/TDP Needed", typeof(string));

            if (!parentTable.Columns.Contains("MMR Needed"))
                parentTable.Columns.Add("MMR Needed", typeof(string));

            if (!parentTable.Columns.Contains("Varicella Needed"))
                parentTable.Columns.Add("Varicella Needed", typeof(string));

            if (!parentTable.Columns.Contains("Over 44"))
                parentTable.Columns.Add("Over 44", typeof(string));

            if (!parentTable.Columns.Contains("Notes"))
                parentTable.Columns.Add("Notes", typeof(string));

            if (!parentTable.Columns.Contains("EventDate"))
                parentTable.Columns.Add("EventDate", typeof(string));

            if (!parentTable.Columns.Contains("Event End Date"))
                parentTable.Columns.Add("Event End Date", typeof(string));

            if (!parentTable.Columns.Contains("EventID"))
                parentTable.Columns.Add("EventID", typeof(string));

            if (!parentTable.Columns.Contains("Vision Win"))
                parentTable.Columns.Add("Vision Win", typeof(string));

            if (!parentTable.Columns.Contains("Dental Win"))
                parentTable.Columns.Add("Dental Win", typeof(string));

            if (!parentTable.Columns.Contains("PHA Win"))
                parentTable.Columns.Add("PHA Win", typeof(string));

            if (!parentTable.Columns.Contains("HIV Win"))
                parentTable.Columns.Add("HIV Win", typeof(string));

            if (!parentTable.Columns.Contains("Hearing WIN"))
                parentTable.Columns.Add("Hearing WIN", typeof(string));
        }

        private void HandleVisionFile(DataRow parentRow, DateTime visionDate)
        {
            if (parentRow.Table.Columns.Contains("Date of Vision Screen") && DateTime.TryParse(parentRow["Date of Vision Screen"]?.ToString(), out DateTime visionScreenDate))
            {
                parentRow["VISION Needed"] = visionDate >= visionScreenDate ? AppConstants.NeededOrNA.Needed : AppConstants.NeededOrNA.NotApplicable;
            }
            else
            {
                parentRow["VISION Needed"] = AppConstants.NeededOrNA.Needed;
            }
        }

        private void HandleDentalFile(DataRow parentRow, DateTime dentalDate, int lastDentalExamDate)
        {
            if (parentRow.Table.Columns.Contains("Date of Next Exam") && DateTime.TryParse(parentRow["Date of Next Exam"]?.ToString(), out DateTime dentalExamDate))
            {
                parentRow["Dental Needed"] = dentalDate >= dentalExamDate ? AppConstants.NeededOrNA.Needed : AppConstants.NeededOrNA.NotApplicable;
                parentRow["BWX Needed"] = dentalDate >= dentalExamDate ? AppConstants.NeededOrNA.Needed : AppConstants.NeededOrNA.NotApplicable;
                parentRow["Dental Exam"] = dentalExamDate.AddDays(-lastDentalExamDate).ToString(DateFormat);
            }
            else
            {
                parentRow["Dental Needed"] = AppConstants.NeededOrNA.Needed;
                parentRow["BWX Needed"] = AppConstants.NeededOrNA.Needed;
                parentRow["Dental Exam"] = "";
            }

            if (parentRow.Table.Columns.Contains("PANX"))
            {
                string panxValue = parentRow["PANX"]?.ToString();
                parentRow["PANO NEEDED"] = (panxValue == "N" || string.IsNullOrEmpty(panxValue) || panxValue.ToString().Trim().Equals("Blank", StringComparison.OrdinalIgnoreCase)) ? AppConstants.NeededOrNA.Needed : AppConstants.NeededOrNA.NotApplicable;
            }
            else
            {
                parentRow["PANO NEEDED"] = AppConstants.NeededOrNA.NotApplicable;
            }
        }

        private void HandlePhaFile(DataRow parentRow, DateTime phaDate)
        {
            if (parentRow.Table.Columns.Contains("Next PHA Date") && DateTime.TryParse(parentRow["Next PHA Date"]?.ToString(), out DateTime phaExamDate))
            {
                parentRow["PHA Needed"] = phaDate >= phaExamDate ? AppConstants.NeededOrNA.Needed : AppConstants.NeededOrNA.NotApplicable;
            }
            else
            {
                parentRow["PHA Needed"] = AppConstants.NeededOrNA.Needed;
            }

            if (parentRow.Table.Columns.Contains("PULHES Code"))
            {
                string pulhesCode = parentRow["PULHES Code"]?.ToString();
                parentRow["Hearing Profile"] = !string.IsNullOrEmpty(pulhesCode) && pulhesCode.Length >= 4 ? pulhesCode[3].ToString() : "";
            }
        }

        private void HandleHivFile(DataRow parentRow, DateTime hivDate)
        {
            if (parentRow.Table.Columns.Contains("Next Test Date") && DateTime.TryParse(parentRow["Next Test Date"]?.ToString(), out DateTime hivTestDate))
            {
                parentRow["HIV"] = hivDate >= hivTestDate ? AppConstants.NeededOrNA.Needed : AppConstants.NeededOrNA.NotApplicable;
            }
            else
            {
                parentRow["HIV"] = AppConstants.NeededOrNA.Needed;
            }
        }

        private void HandleHearingFile(DataRow parentRow, DateTime hearingDate)
        {
            if (parentRow.Table.Columns.Contains("Audiogram Date") && DateTime.TryParse(parentRow["Audiogram Date"]?.ToString(), out DateTime hearingTestDate))
            {
                parentRow["HEARING Needed"] = hearingDate >= hearingTestDate ? AppConstants.NeededOrNA.Needed : AppConstants.NeededOrNA.NotApplicable;
            }
            else
            {
                parentRow["HEARING Needed"] = AppConstants.NeededOrNA.Needed;
            }
        }

        private void HandleTaskForceFile(DataRow parentRow, DateTime eventDate)
        {
            if (parentRow.Table.Columns.Contains("DOB") && DateTime.TryParse(parentRow["DOB"]?.ToString(), out DateTime dobTaskForce))
            {
                var today = eventDate;
                double exactAge = (today - dobTaskForce).TotalDays / 365.25; // Using 365.25 to account for leap years
                double ageWithGrace = ((today.AddDays(14)) - dobTaskForce).TotalDays / 365.25;

                // Over 40 logic
                if (!parentRow.Table.Columns.Contains("OVER 40"))
                {
                    parentRow.Table.Columns.Add("OVER 40", typeof(string));
                }

                // Set "OVER 40" column value based on 39.5 years threshold
                parentRow["OVER 40"] = exactAge > 39.5 ? "YES" : "NO";

                // Near Vision logic
                if (!parentRow.Table.Columns.Contains("NEAR VISION Needed"))
                {
                    parentRow.Table.Columns.Add("NEAR VISION Needed", typeof(string));
                }

                parentRow["NEAR VISION Needed"] = ageWithGrace >= 45 ? AppConstants.NeededOrNA.Needed : AppConstants.NeededOrNA.NotApplicable;


                if (!parentRow.Table.Columns.Contains("OVER 44"))
                {
                    parentRow.Table.Columns.Add("OVER 44", typeof(string));
                }

                parentRow["OVER 44"] = ageWithGrace >= 45 ? "YES" : "NO";

                // Additional logic for exact age > 39.5
                if (exactAge > 39.5)  // Check if exact age is greater than 39.5
                {
                    // Add columns if not already present and set AppConstants.NeededOrNA.Needed
                    if (!parentRow.Table.Columns.Contains("LIPID NEEDED"))
                    {
                        parentRow.Table.Columns.Add("LIPID NEEDED", typeof(string));
                    }
                    parentRow["LIPID NEEDED"] = AppConstants.NeededOrNA.Needed;

                    if (!parentRow.Table.Columns.Contains("LIPID PANEL"))
                    {
                        parentRow.Table.Columns.Add("LIPID PANEL", typeof(string));
                    }
                    parentRow["LIPID PANEL"] = AppConstants.NeededOrNA.Needed;

                    if (!parentRow.Table.Columns.Contains("Cholesterol / HDL Cholesterol"))
                    {
                        parentRow.Table.Columns.Add("Cholesterol / HDL Cholesterol", typeof(string));
                    }
                    parentRow["Cholesterol / HDL Cholesterol"] = AppConstants.NeededOrNA.Needed;

                    if (!parentRow.Table.Columns.Contains("EKG (Date)"))
                    {
                        parentRow.Table.Columns.Add("EKG (Date)", typeof(string));
                    }
                    parentRow["EKG (Date)"] = AppConstants.NeededOrNA.Needed;

                    if (!parentRow.Table.Columns.Contains("EKG NEEDED"))
                    {
                        parentRow.Table.Columns.Add("EKG NEEDED", typeof(string));
                    }
                    parentRow["EKG NEEDED"] = AppConstants.NeededOrNA.Needed;

                    if (!parentRow.Table.Columns.Contains("Framingham"))
                    {
                        parentRow.Table.Columns.Add("Framingham", typeof(string));
                    }
                    parentRow["Framingham"] = AppConstants.NeededOrNA.Needed;
                }
                else
                {
                    // Set columns to AppConstants.NeededOrNA.NotApplicable if age is less than 39.5
                    if (!parentRow.Table.Columns.Contains("LIPID NEEDED"))
                    {
                        parentRow.Table.Columns.Add("LIPID NEEDED", typeof(string));
                    }
                    parentRow["LIPID NEEDED"] = AppConstants.NeededOrNA.NotApplicable;

                    if (!parentRow.Table.Columns.Contains("LIPID PANEL"))
                    {
                        parentRow.Table.Columns.Add("LIPID PANEL", typeof(string));
                    }
                    parentRow["LIPID PANEL"] = AppConstants.NeededOrNA.NotApplicable;

                    if (!parentRow.Table.Columns.Contains("Cholesterol / HDL Cholesterol"))
                    {
                        parentRow.Table.Columns.Add("Cholesterol / HDL Cholesterol", typeof(string));
                    }
                    parentRow["Cholesterol / HDL Cholesterol"] = AppConstants.NeededOrNA.NotApplicable;

                    if (!parentRow.Table.Columns.Contains("EKG (Date)"))
                    {
                        parentRow.Table.Columns.Add("EKG (Date)", typeof(string));
                    }
                    parentRow["EKG (Date)"] = AppConstants.NeededOrNA.NotApplicable;

                    if (!parentRow.Table.Columns.Contains("EKG NEEDED"))
                    {
                        parentRow.Table.Columns.Add("EKG NEEDED", typeof(string));
                    }
                    parentRow["EKG NEEDED"] = AppConstants.NeededOrNA.NotApplicable;

                    if (!parentRow.Table.Columns.Contains("Framingham"))
                    {
                        parentRow.Table.Columns.Add("Framingham", typeof(string));
                    }
                    parentRow["Framingham"] = AppConstants.NeededOrNA.NotApplicable;
                }
            }
        }

        private void HandleBloodFile(DataRow parentRow)
        {
            if (parentRow.Table.Columns.Contains("Blood Type"))
            {
                parentRow["ABO Needed"] = (string.IsNullOrEmpty(parentRow["Blood Type"]?.ToString()) || parentRow["Blood Type"].ToString().Trim().Equals("Blank", StringComparison.OrdinalIgnoreCase)) ? AppConstants.NeededOrNA.Needed : AppConstants.NeededOrNA.NotApplicable;
            }
            else
            {
                parentRow["ABO Needed"] = AppConstants.NeededOrNA.NotApplicable; // Default to "Yes" if the "Blood Type" column doesn't exist
            }
        }

        private void HandleUmrFile(DataRow parentRow)
        {
            if (parentRow.Table.Columns.Contains("MR2-DNA"))
            {
                parentRow["DNA"] = parentRow["MR2-DNA"]?.ToString() == "N" ? AppConstants.NeededOrNA.Needed : AppConstants.NeededOrNA.NotApplicable;
            }
            else
            {
                parentRow["DNA"] = AppConstants.NeededOrNA.NotApplicable;
            }

            if (parentRow.Table.Columns.Contains("SEX") && parentRow["SEX"] != null)
            {
                string sex = parentRow["SEX"].ToString().Trim();

                // Ensure "Pregnancy Test Needed" column exists
                if (!parentRow.Table.Columns.Contains("Pregnancy Test Needed"))
                {
                    parentRow.Table.Columns.Add("Pregnancy Test Needed", typeof(string));
                }

                if (sex == "M")
                {
                    parentRow["Pregnancy Test Needed"] = AppConstants.NeededOrNA.NotApplicable;
                }
                else if (sex == "F")
                {
                    parentRow["Pregnancy Test Needed"] = "";
                }
            }

            if (parentRow.Table.Columns.Contains("MR2-IMM") && parentRow["MR2-IMM"] != null)
            {
                string mr2ImmValue = parentRow["MR2-IMM"].ToString().Trim();

                // Ensure "IMM" column exists
                if (!parentRow.Table.Columns.Contains("IMM Needed"))
                {
                    parentRow.Table.Columns.Add("IMM Needed", typeof(string));
                }

                if (string.IsNullOrWhiteSpace(mr2ImmValue) || mr2ImmValue.Trim().Equals("Blank", StringComparison.OrdinalIgnoreCase) || mr2ImmValue.Trim().Equals("IPV", StringComparison.OrdinalIgnoreCase) || mr2ImmValue.Trim().Equals("MEN", StringComparison.OrdinalIgnoreCase) || mr2ImmValue.Trim().Equals("TYV", StringComparison.OrdinalIgnoreCase) || mr2ImmValue.Trim().Equals("YEL", StringComparison.OrdinalIgnoreCase) || mr2ImmValue.Trim().Equals("ANT", StringComparison.OrdinalIgnoreCase))
                {
                    parentRow["IMM Needed"] = AppConstants.NeededOrNA.NotApplicable;
                }
                else
                {
                    parentRow["IMM Needed"] = AppConstants.NeededOrNA.Needed;
                }
            }

        }

        private void HandleSickleFile(DataRow parentRow)
        {
            if (parentRow.Table.Columns.Contains("Sickle Cell Date"))
            {
                parentRow["SICKLE"] = (string.IsNullOrWhiteSpace(parentRow["Sickle Cell Date"]?.ToString()) || parentRow["Sickle Cell Date"].ToString().Trim().Equals("Blank", StringComparison.OrdinalIgnoreCase)) ? AppConstants.NeededOrNA.Needed : AppConstants.NeededOrNA.NotApplicable;
            }
            else
            {
                parentRow["SICKLE"] = AppConstants.NeededOrNA.NotApplicable; // Default to "Needed" if the "Sickle Cell Date" column doesn't exist
            }
        }

        private void HandleImmunizationFile(DataRow parentRow, DateTime? lastEventDate)
        {
            if (parentRow.Table.Columns.Contains("HB3"))
            {
                string hb3Value = parentRow["HB3"]?.ToString()?.Trim();

                if (string.IsNullOrEmpty(hb3Value) ||
                    hb3Value.Equals("Blank", StringComparison.OrdinalIgnoreCase) ||
                    hb3Value.StartsWith("*") ||
                    (lastEventDate.HasValue && DateTime.TryParse(hb3Value, out DateTime hb3Date) && hb3Date < lastEventDate.Value))
                {
                    parentRow["Hep B Needed"] = AppConstants.NeededOrNA.Needed;
                }
                else
                {
                    parentRow["Hep B Needed"] = AppConstants.NeededOrNA.NotApplicable;
                }
            }
            else
            {
                parentRow["Hep B Needed"] = AppConstants.NeededOrNA.NotApplicable;
            }

            if (parentRow.Table.Columns.Contains("HPA"))
            {
                string hpaValue = parentRow["HPA"]?.ToString()?.Trim();

                if (string.IsNullOrEmpty(hpaValue) ||
                    hpaValue.Equals("Blank", StringComparison.OrdinalIgnoreCase) ||
                    hpaValue.StartsWith("*") ||
                    (lastEventDate.HasValue && DateTime.TryParse(hpaValue, out DateTime hpaDate) && hpaDate < lastEventDate.Value))
                {
                    parentRow["Hep A Needed"] = AppConstants.NeededOrNA.Needed;
                }
                else
                {
                    parentRow["Hep A Needed"] = AppConstants.NeededOrNA.NotApplicable;
                }
            }
            else
            {
                parentRow["Hep A Needed"] = AppConstants.NeededOrNA.NotApplicable;
            }

            if (parentRow.Table.Columns.Contains("INJ"))
            {
                string injValue = parentRow["INJ"]?.ToString()?.Trim();

                if (string.IsNullOrEmpty(injValue) ||
                    injValue.Equals("Blank", StringComparison.OrdinalIgnoreCase) ||
                    injValue.StartsWith("*") ||
                    (lastEventDate.HasValue && DateTime.TryParse(injValue, out DateTime injDate) && injDate < lastEventDate.Value))
                {
                    parentRow["FLU Needed"] = AppConstants.NeededOrNA.Needed;
                }
                else
                {
                    parentRow["FLU Needed"] = AppConstants.NeededOrNA.NotApplicable;
                }
            }
            else
            {
                parentRow["FLU Needed"] = AppConstants.NeededOrNA.NotApplicable;
            }

            if (parentRow.Table.Columns.Contains("TDP"))
            {
                string tdpValue = parentRow["TDP"]?.ToString()?.Trim();

                if (string.IsNullOrEmpty(tdpValue) ||
                    tdpValue.Equals("Blank", StringComparison.OrdinalIgnoreCase) ||
                    tdpValue.StartsWith("*") ||
                    (lastEventDate.HasValue && DateTime.TryParse(tdpValue, out DateTime tdpDate) && tdpDate < lastEventDate.Value))
                {
                    parentRow["Tet/TDP Needed"] = AppConstants.NeededOrNA.Needed;
                }
                else
                {
                    parentRow["Tet/TDP Needed"] = AppConstants.NeededOrNA.NotApplicable;
                }
            }
            else
            {
                parentRow["Tet/TDP Needed"] = AppConstants.NeededOrNA.NotApplicable;
            }


            if (parentRow.Table.Columns.Contains("MMR Needed"))
            {
                string mmrValue = parentRow["MMR Needed"]?.ToString()?.Trim();

                if (string.IsNullOrEmpty(mmrValue) ||
                    mmrValue.Equals("Blank", StringComparison.OrdinalIgnoreCase) ||
                    mmrValue.StartsWith("*") ||
                    (lastEventDate.HasValue && DateTime.TryParse(mmrValue, out DateTime mmrDate) && mmrDate < lastEventDate.Value))
                {
                    parentRow["MMR Needed"] = AppConstants.NeededOrNA.Needed;
                }
                else
                {
                    parentRow["MMR Needed"] = AppConstants.NeededOrNA.NotApplicable;
                }
            }
            else
            {
                parentRow["MMR Needed"] = AppConstants.NeededOrNA.NotApplicable;
            }


            if (parentRow.Table.Columns.Contains("VAR"))
            {
                string varValue = parentRow["VAR"]?.ToString()?.Trim();

                if (string.IsNullOrEmpty(varValue) ||
                    varValue.Equals("Blank", StringComparison.OrdinalIgnoreCase) ||
                    varValue.StartsWith("*") ||
                    (lastEventDate.HasValue && DateTime.TryParse(varValue, out DateTime varDate) && varDate < lastEventDate.Value))
                {
                    parentRow["Varicella Needed"] = AppConstants.NeededOrNA.Needed;
                }
                else
                {
                    parentRow["Varicella Needed"] = AppConstants.NeededOrNA.NotApplicable;
                }
            }
            else
            {
                parentRow["Varicella Needed"] = AppConstants.NeededOrNA.NotApplicable;
            }

        }

        private void HandleOtherFields(DataRow parentRow, DateTime eventDate, DateTime? lastEventDate, int vision, int dental, int pha, int hiv, int hearing, string eventId)
        {
            if (parentRow.Table.Columns.Contains("G6PD Status"))
            {
                parentRow["G6PD"] = (string.IsNullOrWhiteSpace(parentRow["G6PD Status"]?.ToString()) || parentRow["G6PD Status"].ToString().Trim().Equals("Blank", StringComparison.OrdinalIgnoreCase)) ? AppConstants.NeededOrNA.Needed : AppConstants.NeededOrNA.NotApplicable;
            }
            else
            {
                parentRow["G6PD"] = AppConstants.NeededOrNA.NotApplicable; // Default to AppConstants.NeededOrNA.Needed if the "G6PD date" column doesn't exist
            }

            if (parentRow.Table.Columns.Contains("EventDate"))
            {
                parentRow["EventDate"] = eventDate.ToString(DateFormat);
            }
            else
            {
                parentRow["EventDate"] = "";
            }


            if (parentRow.Table.Columns.Contains("Event End Date"))
            {
                parentRow["Event End Date"] = lastEventDate.HasValue ? lastEventDate.Value.ToString(DateFormat) : "";
            }
            else
            {
                parentRow["Event End Date"] = "";
            }

            if (parentRow.Table.Columns.Contains("Vision Win"))
            {
                parentRow["Vision Win"] = vision;
            }
            else
            {
                parentRow["Vision Win"] = "";
            }

            if (parentRow.Table.Columns.Contains("Dental Win"))
            {
                parentRow["Dental Win"] = dental;
            }
            else
            {
                parentRow["Dental Win"] = "";
            }

            if (parentRow.Table.Columns.Contains("PHA Win"))
            {
                parentRow["PHA Win"] = pha;
            }
            else
            {
                parentRow["PHA Win"] = "";
            }

            if (parentRow.Table.Columns.Contains("HIV Win"))
            {
                parentRow["HIV Win"] = hiv;
            }
            else
            {
                parentRow["HIV Win"] = "";
            }

            if (parentRow.Table.Columns.Contains("Hearing WIN"))
            {
                parentRow["Hearing WIN"] = hearing;
            }
            else
            {
                parentRow["Hearing WIN"] = "";
            }

            if (parentRow.Table.Columns.Contains("EventID"))
            {
                parentRow["EventID"] = eventId;
            }
            else
            {
                parentRow["EventID"] = "";
            }
        }

        //private ServiceMembersParent MapToParentEntity(List<FileDataDto> fileDataDtos, long eventId, string addedBy, bool isUpdate = false, ServiceMembersChild existingServiceMembersChild = null)
        //{
        //    var first = fileDataDtos.First();
        //    DateTime addedDateTime = DateTime.Now;

        //    var parent = new ServiceMembersParent
        //    {
        //        EventManagementId = eventId,
        //        VisionWin = first.VisionWin,
        //        DentalWin = first.DentalWin,
        //        PhaWin = first.PhaWin,
        //        HivWin = first.HivWin,
        //        HearingWin = first.HearingWin,
        //        //isDeleted = first.isDeleted,
        //        AddedBy = addedBy,
        //        AddedOn = addedDateTime,

        //        ServiceMembersChildren = fileDataDtos.Select(dto => new ServiceMembersChild
        //        {
        //            SmId = dto.SmId,
        //            FullName = dto.FullName,
        //            FullSsn = dto.FullSsn,
        //            Last4 = dto.Last4,
        //            DodId = dto.DodId,
        //            Rank = dto.Rank,
        //            Age = dto.Age,
        //            Sex = dto.Sex,
        //            Mos = dto.Mos,
        //            Agr = dto.Agr,
        //            Uic = dto.Uic,
        //            Mrc = dto.Mrc,
        //            Dob = dto.Dob,
        //            Over40 = dto.Over40,
        //            DentalDue = dto.DentalDue,
        //            DentalExam = dto.DentalExam,
        //            DentalNeeded = dto.DentalNeeded,
        //            PanoNeeded = dto.PanoNeeded,
        //            BwxNeeded = dto.BwxNeeded,
        //            Drc = dto.Drc,
        //            PhaDate = dto.PhaDate,
        //            PhaDue = dto.PhaDue,
        //            Pha = dto.Pha,
        //            Pulhes = dto.Pulhes,
        //            VisionDate = dto.VisionDate,
        //            Vision = dto.Vision,
        //            NearVision = dto.NearVision,
        //            Vrc = dto.Vrc,
        //            Vision2pg = dto.Vision2pg,
        //            Vision1mi = dto.Vision1mi,
        //            HearingDate = dto.HearingDate,
        //            Hearing = dto.Hearing,
        //            Hrc = dto.Hrc,
        //            HearingProfile = dto.HearingProfile,
        //            Quest = dto.Quest,
        //            LabNeeded = dto.LabNeeded,
        //            Abo = dto.Abo,
        //            AboNeeded = dto.AboNeeded,
        //            Dna = dto.Dna,
        //            SickleDate = dto.SickleDate,
        //            Sickle = dto.Sickle,
        //            G6pd = dto.G6pd,
        //            G6pdDate = dto.G6pdDate,
        //            G6pdStatus = dto.G6pdStatus,
        //            HivNextTestDate = dto.HivNextTestDate,
        //            Hiv = dto.Hiv,
        //            LipidNeeded = dto.LipidNeeded,
        //            LipidPanel = dto.LipidPanel,
        //            CholesterolHdlCholesterol = dto.CholesterolHdlCholesterol,
        //            Framingham = dto.Framingham,
        //            Ekg = dto.Ekg,
        //            EkgNeeded = dto.EkgNeeded,
        //            PregnancyTestNeeded = dto.PregnancyTestNeeded,
        //            Imm = dto.Imm,
        //            HepB = dto.HepB,
        //            HepA = dto.HepA,
        //            Flu = dto.Flu,
        //            TetTdp = dto.TetTdp,
        //            Mmr = dto.Mmr,
        //            Varicella = dto.Varicella,
        //            TaskForce = dto.TaskForce,
        //            Notes = dto.Notes,
        //            Over44 = dto.Over44,
        //            EventDate = dto.EventDate,
        //            EventEndDate = dto.EventEndDate,
        //            CheckIn = dto.CheckIn,
        //            CheckInBy = dto.CheckInBy,
        //            CheckInTime = Malama.Utilities.Helper.NormalizeDateTime(dto.CheckInTime),
        //            CheckOut = dto.CheckOut,
        //            CheckOutBy = dto.CheckOutBy,
        //            CheckOutTime = Malama.Utilities.Helper.NormalizeDateTime(dto.CheckOutTime),
        //            WalkInServiceMember = dto.WalkInServiceMember,
        //            Barcode = dto.Barcode,
        //            AddedBy = isUpdate ? existingServiceMembersChild.AddedBy : addedBy,
        //            AddedOn = isUpdate ? existingServiceMembersChild.AddedOn : addedDateTime,
        //            UpdatedBy = isUpdate ? addedBy : null,
        //            UpdatedOn = isUpdate ? addedDateTime : null
        //        }).ToList()
        //    };

        //    return parent;
        //}

        private ServiceMembersParent MapToServiceMembersParent(FileDataDto dto, long eventId, string addedBy, List<ServiceMembersChild> children = null)
        {
            return new ServiceMembersParent
            {
                EventManagementId = eventId,
                VisionWin = dto.VisionWin,
                DentalWin = dto.DentalWin,
                PhaWin = dto.PhaWin,
                HivWin = dto.HivWin,
                HearingWin = dto.HearingWin,
                AddedBy = addedBy,
                AddedOn = DateTime.Now,
                ServiceMembersChildren = children ?? new List<ServiceMembersChild>()
            };
        }
        private List<ServiceMembersChild> MapToServiceMembersChildren(List<FileDataDto> fileDataDtos, string addedBy)
        {
            DateTime addedDateTime = DateTime.Now;

            return fileDataDtos.Select(dto => new ServiceMembersChild
            {
                SmId = dto.SmId,
                FullName = dto.FullName,
                FullSsn = dto.FullSsn,
                Last4 = dto.Last4,
                DodId = dto.DodId,
                Rank = dto.Rank,
                Age = dto.Age,
                Sex = dto.Sex,
                Mos = dto.Mos,
                Agr = dto.Agr,
                Uic = dto.Uic,
                Mrc = dto.Mrc,
                Dob = dto.Dob,
                Over40 = dto.Over40,
                DentalDue = dto.DentalDue,
                DentalExam = dto.DentalExam,
                DentalNeeded = dto.DentalNeeded,
                PanoNeeded = dto.PanoNeeded,
                BwxNeeded = dto.BwxNeeded,
                Drc = dto.Drc,
                PhaDate = dto.PhaDate,
                PhaDue = dto.PhaDue,
                Pha = dto.Pha,
                Pulhes = dto.Pulhes,
                VisionDate = dto.VisionDate,
                Vision = dto.Vision,
                NearVision = dto.NearVision,
                Vrc = dto.Vrc,
                Vision2pg = dto.Vision2pg,
                Vision1mi = dto.Vision1mi,
                HearingDate = dto.HearingDate,
                Hearing = dto.Hearing,
                Hrc = dto.Hrc,
                HearingProfile = dto.HearingProfile,
                Quest = dto.Quest,
                LabNeeded = dto.LabNeeded,
                Abo = dto.Abo,
                AboNeeded = dto.AboNeeded,
                Dna = dto.Dna,
                SickleDate = dto.SickleDate,
                Sickle = dto.Sickle,
                G6pd = dto.G6pd,
                G6pdDate = dto.G6pdDate,
                G6pdStatus = dto.G6pdStatus,
                HivNextTestDate = dto.HivNextTestDate,
                Hiv = dto.Hiv,
                LipidNeeded = dto.LipidNeeded,
                LipidPanel = dto.LipidPanel,
                CholesterolHdlCholesterol = dto.CholesterolHdlCholesterol,
                Framingham = dto.Framingham,
                Ekg = dto.Ekg,
                EkgNeeded = dto.EkgNeeded,
                PregnancyTestNeeded = dto.PregnancyTestNeeded,
                Imm = dto.Imm,
                HepB = dto.HepB,
                HepA = dto.HepA,
                Flu = dto.Flu,
                TetTdp = dto.TetTdp,
                Mmr = dto.Mmr,
                Varicella = dto.Varicella,
                TaskForce = dto.TaskForce,
                Notes = dto.Notes,
                Over44 = dto.Over44,
                EventDate = dto.EventDate,
                EventEndDate = dto.EventEndDate,
                CheckIn = dto.CheckIn,
                CheckInBy = dto.CheckInBy,
                CheckInTime = Malama.Utilities.Helper.NormalizeDateTime(dto.CheckInTime),
                CheckOut = dto.CheckOut,
                CheckOutBy = dto.CheckOutBy,
                CheckOutTime = Malama.Utilities.Helper.NormalizeDateTime(dto.CheckOutTime),
                WalkInServiceMember = dto.WalkInServiceMember,
                Barcode = dto.Barcode,
                AddedBy = addedBy,
                AddedOn = addedDateTime,
                UpdatedBy = null,
                UpdatedOn = null
            }).ToList();
        }

        private async Task<(ResponseDto Response, EventManagement EventManagement, ServiceMembersParent ExistingParent)>
    FetchLatestEventManagementWithParentAsync(string eventId, string addedBy)
        {
            const string methodName = nameof(FetchLatestEventManagementWithParentAsync);

            _logger.LogInformation("{ClassName}, {MethodName}, Fetching latest EventManagement for EventID: {EventID} by User: {UserName}",
                CLASSNAME, methodName, eventId, addedBy);

            try
            {
                // Fetch EventManagement with related parents
                var eventManagement = await _unitOfWork.EventManagement
                .GetWithIncludeTracking(
                    x => x.EventID == eventId,
                    x => x.ServiceMembersParents
                )
                .OrderByDescending(x => x.EventVersion)
                .FirstOrDefaultAsync();

                if (eventManagement == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, EventManagement not found for EventID: {EventID} by User: {UserName}",
                        CLASSNAME, methodName, eventId, addedBy);

                    return (new ResponseDto
                    {
                        Success = false,
                        Message = ResponseMessages.EventNotFound(eventId),
                        Data = null,
                        Code = ResponseCodes.NotFound
                    }, null, null);
                }

                long eventManagementId = eventManagement.Id;
                int eventVersion = eventManagement.EventVersion;

                _logger.LogInformation("{ClassName}, {MethodName}, EventManagement found (V{Version}) for EventID: {EventID} by User: {UserName}",
                    CLASSNAME, methodName, eventVersion, eventId, addedBy);


                var existingParent = eventManagement?.ServiceMembersParents.FirstOrDefault(p => !(p.isDeleted ?? false));

                if (existingParent == null)
                {
                    // Parent does not exist or is marked deleted
                    existingParent = null;
                }

                return (new ResponseDto { Success = true }, eventManagement, existingParent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Error fetching EventManagement for EventID: {EventID} by User: {UserName}",
                    CLASSNAME, methodName, eventId, addedBy);

                return (new ResponseDto
                {
                    Success = false,
                    Message = "Error fetching EventManagement.",
                    Data = null
                }, null, null);
            }
        }

        public async Task<(ServiceMembersParent serviceMembersParent, List<ServiceMembersChild> serviceMembersChildren)>GetServiceMembersByEventAsync(long eventId)
        {
            try
            {
                _logger.LogInformation("{ClassName}, {MethodName}, Fetching ServiceMembersParent for EventId: {EventId}",
                    eventId);

                var serviceMembersParent = await _unitOfWork.EventManagement
                .GetWithIncludeNoTracking(
                    x => x.Id == eventId
                )
                .Include(x => x.ServiceMembersParents)
                    .ThenInclude(p => p.ServiceMembersChildren)
                .SelectMany(em => em.ServiceMembersParents)
                .Where(p => !(p.isDeleted ?? false))
                .OrderBy(p => p.Id)
                .FirstOrDefaultAsync();

                if (serviceMembersParent == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, No ServiceMembersParent found for EventId: {EventId}",
                        eventId);
                    return (null, new List<ServiceMembersChild>());
                }

                var serviceMembersChildren = serviceMembersParent.ServiceMembersChildren?.ToList() ?? new List<ServiceMembersChild>();

                _logger.LogInformation("{ClassName}, {MethodName}, Found ServiceMembersParent with {ChildCount} children");

                return (serviceMembersParent, serviceMembersChildren);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName},Error fetching ServiceMembersParent for EventId: {EventId}",
                    eventId);
                throw;
            }
        }
    }
}
