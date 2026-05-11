using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Utilities;
using Malama.Attributes;
using Malama.Models;
using Malama.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ExcelFilesCompiler.Controllers
{
    public class PostEventDataAnalysisController : Controller
    {
        private readonly IEventManagementService _eventManagementService;
        private readonly IFileUploadDownloadService _fileService;
        private readonly IFileUploader _fileUploader;
        private readonly IPostEventLabStationService _postEventLabStationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<PostEventDataAnalysisController> _logger;
        private const string CLASSNAME = "PostEventDataAnalysisController";

        public PostEventDataAnalysisController(IFileUploadDownloadService fileService, ILogger<PostEventDataAnalysisController> logger, IPostEventLabStationService postEventLabStationService, IFileUploader fileUploader, IEventManagementService eventManagementService, UserManager<ApplicationUser> userManager)
        {
            _eventManagementService = eventManagementService;
            _postEventLabStationService = postEventLabStationService;
            _fileUploader = fileUploader;
            _userManager = userManager;
            _logger = logger;
            _fileService = fileService;
        }

        [RoleAttributeAuthorizeFromConfig("PostEventDataAnalysis_View")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            const string methodName = "Index";
            _logger.LogInformation("{ClassName}, {MethodName}, Called.",
                CLASSNAME, methodName);

            var responseDto = new ResponseDto();
            List<PostEventManagementPreview> eventManagementList = new();

            try
            {
                long? claimEventId = null;

                // 🔹 If Event Manager → get EventId from claim
                if (User.IsInRole("Event Manager"))
                {
                    var eventIdClaim = User.FindFirst("EventIdLong")?.Value;

                    if (!string.IsNullOrEmpty(eventIdClaim) &&
                        long.TryParse(eventIdClaim, out long parsedId))
                    {
                        claimEventId = parsedId;
                        _logger.LogInformation("{ClassName}, {MethodName}, Event Manager detected, claimEventId: {ClaimEventID}",
                            CLASSNAME, methodName, claimEventId);
                    }
                }

                eventManagementList = await _eventManagementService.GetAllForPostEventDataAnalysis(claimEventId);

                _logger.LogInformation("{ClassName}, {MethodName}, Retrieved {Count} event management records.",
                    CLASSNAME, methodName, eventManagementList.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Error while loading event managements.",
                    CLASSNAME, methodName);

                TempData["ErrorMessage"] =
                    "We encountered an issue while loading event managements. Please try again later.";
            }

            var viewModel = new PostEventManagementViewModel
            {
                EventManagements = eventManagementList,
            };

            return View(viewModel);
        }

        [RoleAttributeAuthorizeFromConfig("PostEventDataAnalysis_View")]
        [HttpGet]
        public async Task<IActionResult> SelectStation(long eventManagementId, string selectedStation)
        {
            const string methodName = "SelectStation";

            _logger.LogInformation(
                "{ClassName}.{MethodName} - Called with EventManagementId={EventManagementId}, SelectedStation={SelectedStation}",
                CLASSNAME, methodName, eventManagementId, selectedStation);

            try
            {
                var model = new PostEventDataAnalysisViewModel
                {
                    EventId = eventManagementId,
                    SelectedStation = selectedStation
                };

                // 🔹 Fetch Event Management (to get business EventID like ABC0001)
                var eventManagement = await _eventManagementService
                    .GetEventManagementForEventSelectionByIdWithoutInclude(eventManagementId);

                if (eventManagement == null)
                {
                    _logger.LogWarning(
                        "{ClassName}.{MethodName} - EventManagement not found for EventManagementId={EventManagementId}",
                        CLASSNAME, methodName, eventManagementId);

                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Not Found";
                    TempData["ResponseMessage"] = "Event not found.";

                    return RedirectToAction("Index");
                }

                // ✅ Set EventID in ViewBag
                ViewBag.EventID = eventManagement.EventID;

                _logger.LogInformation(
                    "{ClassName}.{MethodName} - Loaded EventID={EventID} for EventManagementId={EventManagementId}",
                    CLASSNAME, methodName, eventManagement.EventID, eventManagementId);

                // 🔹 Load station data
                if (!string.IsNullOrEmpty(selectedStation))
                {
                    model.ServiceMembersChild = selectedStation switch
                    {
                        "Labs" => await _fileUploader.GetPreAndPostLabStationByEventIdAsync(model.EventId),
                        //"Immunization",
                        //"Dental",
                        //"XYZ",
                        _ => new List<ServiceMembersChild>()
                    };

                    _logger.LogInformation(
                        "{ClassName}.{MethodName} - Loaded data for SelectedStation={SelectedStation}, Count={Count}",
                        CLASSNAME, methodName, selectedStation, model.ServiceMembersChild?.Count ?? 0);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}.{MethodName} - Exception occurred for EventManagementId={EventManagementId}",
                    CLASSNAME, methodName, eventManagementId);

                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = "Something went wrong while loading data.";

                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [RoleAttributeAuthorizeFromConfig("PostEventDataAnalysis_View")]
        public async Task<IActionResult> SpecificServiceMemberLabStation(long? postLabStationId, long serviceMembersChildId)
        {
            const string methodName = nameof(SpecificServiceMemberLabStation);

            _logger.LogInformation(
                "{ClassName}.{MethodName} - Called with PostLabStationId={PostLabStationId}, ServiceMembersChildId={ServiceMembersChildId}",
                CLASSNAME, methodName, postLabStationId, serviceMembersChildId);

            try
            {
                var model = await _fileUploader
                    .GetPostEventLabStationAnalysisDtoAsync(serviceMembersChildId);

                if (model == null)
                {
                    _logger.LogWarning(
                        "{ClassName}.{MethodName} - No data found for ServiceMembersChildId={ServiceMembersChildId}",
                        CLASSNAME, methodName, serviceMembersChildId);

                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseMessage"] = "Record not found.";

                    return RedirectToAction("Index");
                }

                _logger.LogInformation(
                    "{ClassName}.{MethodName} - Successfully prepared DTO for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, serviceMembersChildId);

                ViewBag.EventID = model.EventID;

                return View(model); // ✅ now sending DTO
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}.{MethodName} - Error occurred for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, serviceMembersChildId);

                TempData["ResponseStatus"] = "error";
                TempData["ResponseMessage"] = "Something went wrong.";

                return RedirectToAction("Index");
            }
        }

        [RoleAttributeAuthorizeFromConfig("PostEventDataAnalysis_Save")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePostEventLabStation(PostEventLabStationAnalysisDto model)
        {
            const string methodName = "SavePostEventLabStation";

            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, ModelState invalid", CLASSNAME, methodName);

                    var message = string.Join(" | ",
                        ModelState.Values.SelectMany(v => v.Errors)
                                         .Select(e => e.ErrorMessage));

                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = message;

                    var errorModel = await _fileUploader.GetPostEventLabStationAnalysisDtoAsync(model.PostEventLabStation.ServiceMembersChildId);
                    return View("SpecificServiceMemberLabStation", errorModel);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Unauthorized access", CLASSNAME, methodName);

                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Unauthorized";
                    TempData["ResponseMessage"] = "Please login and try again.";

                    return RedirectToAction("Index");
                }

                if (model.PostEventLabStation == null)
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Error";
                    TempData["ResponseMessage"] = "Invalid form data.";

                    return RedirectToAction("Index");
                }

                if (model.PostEventLabStation.Id == 0)
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Add operation started. User={User}",
                        CLASSNAME, methodName, user.UserName);

                    var fileUploadError = await UploadLabFilesForAdd(model);
                    
                    if (!string.IsNullOrEmpty(fileUploadError))
                    {
                        TempData["ResponseStatus"] = "error";
                        TempData["ResponseTitle"] = "Error";
                        TempData["ResponseMessage"] = fileUploadError;

                        var errorModel = await _fileUploader.GetPostEventLabStationAnalysisDtoAsync(model.PostEventLabStation.ServiceMembersChildId);
                        return View("SpecificServiceMemberLabStation", errorModel);
                    }

                    var result = await _postEventLabStationService
                        .AddAsync(model.PostEventLabStation, user.UserName);

                    if (!result.Success)
                    {
                        TempData["ResponseStatus"] = "error";
                        TempData["ResponseTitle"] = "Error";
                        TempData["ResponseMessage"] = result.Message;

                        var errorModel = await _fileUploader.GetPostEventLabStationAnalysisDtoAsync(model.PostEventLabStation.ServiceMembersChildId);
                        return View("SpecificServiceMemberLabStation", errorModel);
                    }

                    TempData["ResponseStatus"] = "success";
                    TempData["ResponseTitle"] = "Success";
                    TempData["ResponseMessage"] = result.Message;
                }
                else
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Update operation started. Id={Id}, User={User}",
                        CLASSNAME, methodName, model.PostEventLabStation.Id, user.UserName);

                    var existing = await _postEventLabStationService.GetByIdAsync(model.PostEventLabStation.Id);
                    if (existing == null)
                    {
                        _logger.LogWarning(
                            "{ClassName}, {MethodName}, Existing record not found for update. Id={Id}",
                            CLASSNAME, methodName, model.PostEventLabStation.Id);

                        TempData["ResponseStatus"] = "error";
                        TempData["ResponseTitle"] = "Error";
                        TempData["ResponseMessage"] = "Record not found.";
                        var errorModel = await _fileUploader.GetPostEventLabStationAnalysisDtoAsync(model.PostEventLabStation.ServiceMembersChildId);
                        return View("SpecificServiceMemberLabStation", errorModel);
                    }

                    var fileProcessError = await ProcessLabFilesForUpdate(model, existing);
                    if (!string.IsNullOrEmpty(fileProcessError))
                    {
                        TempData["ResponseStatus"] = "error";
                        TempData["ResponseTitle"] = "Error";
                        TempData["ResponseMessage"] = fileProcessError;

                        var errorModel = await _fileUploader.GetPostEventLabStationAnalysisDtoAsync(model.PostEventLabStation.ServiceMembersChildId);
                        return View("SpecificServiceMemberLabStation", errorModel);
                    }

                    var result = await _postEventLabStationService
                        .UpdateAsync(model.PostEventLabStation, user.UserName);
                    if (!result.Success)
                    {
                        TempData["ResponseStatus"] = "error";
                        TempData["ResponseTitle"] = "Error";
                        TempData["ResponseMessage"] = result.Message;

                        var errorModel = await _fileUploader.GetPostEventLabStationAnalysisDtoAsync(model.PostEventLabStation.ServiceMembersChildId);
                        return View("SpecificServiceMemberLabStation", errorModel);
                    }

                    TempData["ResponseStatus"] = "success";
                    TempData["ResponseTitle"] = "Success";
                    TempData["ResponseMessage"] = result.Message;
                }

                return RedirectToAction("SelectStation", new
                {
                    eventManagementId = model.EventId,
                    selectedStation = "Labs"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred",
                    CLASSNAME, methodName);

                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = "An unexpected error occurred.";

                var errorModel = await _fileUploader.GetPostEventLabStationAnalysisDtoAsync(model.PostEventLabStation.ServiceMembersChildId);
                return View("SpecificServiceMemberLabStation", errorModel);
            }
        }

        [RoleAttributeAuthorizeFromConfig("PostEventDataAnalysis_View")]
        public IActionResult DownloadMalamaFile(
            string station,
            string prefix,
            string fileName)
        {
            const string METHOD = nameof(DownloadMalamaFile);

            try
            {
                if (string.IsNullOrWhiteSpace(station) ||
                    string.IsNullOrWhiteSpace(prefix) ||
                    string.IsNullOrWhiteSpace(fileName))
                {
                    _logger.LogWarning(
                        "{Class}.{Method} - Invalid download request parameters | Station: {Station}, Prefix: {Prefix}, FileName: {FileName}",
                        CLASSNAME, METHOD, station, prefix, fileName);

                    return BadRequest("Invalid file download request. Station, prefix, and file name are required.");
                }

                _logger.LogInformation("{Class}.{Method} - Download request | Station: {Station}, Prefix: {Prefix}, FileName: {FileName}",
                    CLASSNAME, METHOD, station, prefix, fileName);

                var file = _fileService.GetFile(station, prefix, fileName);

                if (file == null)
                {
                    _logger.LogWarning("{Class}.{Method} - File not found | FileName: {FileName}",
                        CLASSNAME, METHOD, fileName);

                    return NotFound();
                }

                _logger.LogInformation("{Class}.{Method} - File returned successfully | FileName: {FileName}",
                    CLASSNAME, METHOD, fileName);
                Response.Headers["Content-Disposition"] = $"inline; filename=\"{file.FileName}\"";
                return File(file.Bytes, file.ContentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{Class}.{Method} - Exception occurred while downloading | FileName: {FileName}",
                    CLASSNAME, METHOD, fileName);

                return StatusCode(500, "Error while downloading file");
            }
        }

        private async Task<string?> UploadLabFilesForAdd(PostEventLabStationAnalysisDto model)
        {
            const string station = "Labs";
            var barcode = model.ServiceMember?.Barcode;

            if (string.IsNullOrWhiteSpace(barcode))
            {
                return "Service member barcode is required for lab file upload.";
            }

            var fileMappings = GetMalamaFileMappings();

            foreach (var fileMapping in fileMappings)
            {
                var postedFile = Request.Form.Files[fileMapping.InputName];
                if (postedFile == null || postedFile.Length == 0)
                {
                    continue;
                }

                var result = await _fileService.UploadFile(postedFile, station, fileMapping.Prefix, barcode);
                if (!result.Success)
                {
                    _logger.LogWarning(
                        "{ClassName}.{MethodName} - File upload failed on save. Prefix={Prefix}, Message={Message}",
                        CLASSNAME, nameof(UploadLabFilesForAdd), fileMapping.Prefix, result.Message);

                    return result.Message ?? $"Failed to upload {fileMapping.Prefix.ToUpperInvariant()} lab result file.";
                }

                fileMapping.SetUploaded(model.PostEventLabStation, true);
                fileMapping.SetFileName(model.PostEventLabStation, result.FileName);
                fileMapping.SetOriginalFileName(model.PostEventLabStation, postedFile.FileName);
            }

            return null;
        }

        private async Task<string?> ProcessLabFilesForUpdate(PostEventLabStationAnalysisDto model, PostEventLabStation existing)
        {
            const string station = "Labs";
            var barcode = model.ServiceMember?.Barcode;

            if (string.IsNullOrWhiteSpace(barcode))
            {
                return "Service member barcode is required for lab file upload.";
            }

            var fileMappings = GetMalamaFileMappings();
            foreach (var fileMapping in fileMappings)
            {
                var postedFile = Request.Form.Files[fileMapping.InputName];
                var currentFileName = fileMapping.GetExistingFileName(existing);
                var currentOriginalFileName = fileMapping.GetExistingOriginalFileName(existing);
                var incomingUploaded = fileMapping.GetUploaded(model.PostEventLabStation);
                var incomingFileName = fileMapping.GetFileName(model.PostEventLabStation);
                var incomingOriginalFileName = fileMapping.GetOriginalFileName(model.PostEventLabStation);

                // 1) Replace existing file (new upload selected in edit mode).
                if (postedFile != null && postedFile.Length > 0)
                {
                    var uploadResult = await _fileService.UploadFile(postedFile, station, fileMapping.Prefix, barcode);
                    if (!uploadResult.Success)
                    {
                        _logger.LogWarning(
                            "{ClassName}.{MethodName} - File replacement failed. Prefix={Prefix}, Message={Message}",
                            CLASSNAME, nameof(ProcessLabFilesForUpdate), fileMapping.Prefix, uploadResult.Message);

                        return uploadResult.Message ?? $"Failed to upload {fileMapping.Prefix.ToUpperInvariant()} lab result file.";
                    }

                    if (!string.IsNullOrWhiteSpace(currentFileName) &&
                        !string.Equals(currentFileName, uploadResult.FileName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (!_fileService.DeleteFile(station, fileMapping.Prefix, currentFileName))
                        {
                            _logger.LogWarning(
                                "{ClassName}.{MethodName} - Old file delete failed after replacement. Prefix={Prefix}, FileName={FileName}",
                                CLASSNAME, nameof(ProcessLabFilesForUpdate), fileMapping.Prefix, currentFileName);
                        }
                    }

                    fileMapping.SetUploaded(model.PostEventLabStation, true);
                    fileMapping.SetFileName(model.PostEventLabStation, uploadResult.FileName);
                    fileMapping.SetOriginalFileName(model.PostEventLabStation, postedFile.FileName);
                    continue;
                }

                // 2) Cancel existing file (no new file selected; remove from server + clear DB fields).
                if (!incomingUploaded)
                {
                    if (!string.IsNullOrWhiteSpace(currentFileName))
                    {
                        var deleteSuccess = _fileService.DeleteFile(station, fileMapping.Prefix, currentFileName);
                        if (!deleteSuccess)
                        {
                            _logger.LogWarning(
                                "{ClassName}.{MethodName} - File delete failed on cancel. Prefix={Prefix}, FileName={FileName}",
                                CLASSNAME, nameof(ProcessLabFilesForUpdate), fileMapping.Prefix, currentFileName);

                            return $"Failed to delete {fileMapping.Prefix.ToUpperInvariant()} lab result file.";
                        }
                    }

                    fileMapping.SetUploaded(model.PostEventLabStation, false);
                    fileMapping.SetFileName(model.PostEventLabStation, null);
                    fileMapping.SetOriginalFileName(model.PostEventLabStation, null);
                    fileMapping.SetDate(model.PostEventLabStation, null);
                    continue;
                }

                // 3) Keep existing file (no new file selected and user did not cancel).
                if (string.IsNullOrWhiteSpace(incomingFileName) && !string.IsNullOrWhiteSpace(currentFileName))
                {
                    fileMapping.SetFileName(model.PostEventLabStation, currentFileName);
                    fileMapping.SetUploaded(model.PostEventLabStation, true);
                    if (string.IsNullOrWhiteSpace(incomingOriginalFileName))
                    {
                        fileMapping.SetOriginalFileName(model.PostEventLabStation, currentOriginalFileName);
                    }
                }
            }

            return null;
        }

        private static (string InputName, string Prefix,
            Func<PostEventLabStationDto, bool> GetUploaded,
            Action<PostEventLabStationDto, bool> SetUploaded,
            Func<PostEventLabStation, string?> GetExistingFileName,
            Func<PostEventLabStation, string?> GetExistingOriginalFileName,
            Func<PostEventLabStationDto, string?> GetFileName,
            Action<PostEventLabStationDto, string?> SetFileName,
            Func<PostEventLabStationDto, string?> GetOriginalFileName,
            Action<PostEventLabStationDto, string?> SetOriginalFileName,
            Action<PostEventLabStationDto, DateTime?> SetDate)[] GetMalamaFileMappings()
        {
            return new (string InputName, string Prefix,
                Func<PostEventLabStationDto, bool> GetUploaded,
                Action<PostEventLabStationDto, bool> SetUploaded,
                Func<PostEventLabStation, string?> GetExistingFileName,
                Func<PostEventLabStation, string?> GetExistingOriginalFileName,
                Func<PostEventLabStationDto, string?> GetFileName,
                Action<PostEventLabStationDto, string?> SetFileName,
                Func<PostEventLabStationDto, string?> GetOriginalFileName,
                Action<PostEventLabStationDto, string?> SetOriginalFileName,
                Action<PostEventLabStationDto, DateTime?> SetDate)[]
            {
                ("g6pdMalamaFile", "g6pd",
                    (Func<PostEventLabStationDto, bool>)(dto => dto.G6pdResultMalamaUploaded),
                    (dto, value) => dto.G6pdResultMalamaUploaded = value,
                    entity => entity.G6pdResultMalamaUploadedFileName,
                    entity => entity.G6pdResultMalamaUploadedOriginalFileName,
                    dto => dto.G6pdResultMalamaUploadedFileName,
                    (dto, value) => dto.G6pdResultMalamaUploadedFileName = value,
                    dto => dto.G6pdResultMalamaUploadedOriginalFileName,
                    (dto, value) => dto.G6pdResultMalamaUploadedOriginalFileName = value,
                    (dto, value) => dto.G6pdResultMalamaUploadedDateTime = value),
                ("aboMalamaFile", "abo",
                    (Func<PostEventLabStationDto, bool>)(dto => dto.AboResultMalamaUploaded),
                    (dto, value) => dto.AboResultMalamaUploaded = value,
                    entity => entity.AboResultMalamaUploadedFileName,
                    entity => entity.AboResultMalamaUploadedOriginalFileName,
                    dto => dto.AboResultMalamaUploadedFileName,
                    (dto, value) => dto.AboResultMalamaUploadedFileName = value,
                    dto => dto.AboResultMalamaUploadedOriginalFileName,
                    (dto, value) => dto.AboResultMalamaUploadedOriginalFileName = value,
                    (dto, value) => dto.AboResultMalamaUploadedDateTime = value),
                ("lipidMalamaFile", "lipid",
                    (Func<PostEventLabStationDto, bool>)(dto => dto.LipidPanelResultMalamaUploaded),
                    (dto, value) => dto.LipidPanelResultMalamaUploaded = value,
                    entity => entity.LipidPanelResultMalamaUploadedFileName,
                    entity => entity.LipidPanelResultMalamaUploadedOriginalFileName,
                    dto => dto.LipidPanelResultMalamaUploadedFileName,
                    (dto, value) => dto.LipidPanelResultMalamaUploadedFileName = value,
                    dto => dto.LipidPanelResultMalamaUploadedOriginalFileName,
                    (dto, value) => dto.LipidPanelResultMalamaUploadedOriginalFileName = value,
                    (dto, value) => dto.LipidPanelResultMalamaUploadedDateTime = value),
                ("hivMalamaFile", "hiv",
                    (Func<PostEventLabStationDto, bool>)(dto => dto.HivResultMalamaUploaded),
                    (dto, value) => dto.HivResultMalamaUploaded = value,
                    entity => entity.HivResultMalamaUploadedFileName,
                    entity => entity.HivResultMalamaUploadedOriginalFileName,
                    dto => dto.HivResultMalamaUploadedFileName,
                    (dto, value) => dto.HivResultMalamaUploadedFileName = value,
                    dto => dto.HivResultMalamaUploadedOriginalFileName,
                    (dto, value) => dto.HivResultMalamaUploadedOriginalFileName = value,
                    (dto, value) => dto.HivResultMalamaUploadedDateTime = value),
                ("pregMalamaFile", "preg",
                    (Func<PostEventLabStationDto, bool>)(dto => dto.PregnancyResultMalamaUploaded),
                    (dto, value) => dto.PregnancyResultMalamaUploaded = value,
                    entity => entity.PregnancyResultMalamaUploadedFileName,
                    entity => entity.PregnancyResultMalamaUploadedOriginalFileName,
                    dto => dto.PregnancyResultMalamaUploadedFileName,
                    (dto, value) => dto.PregnancyResultMalamaUploadedFileName = value,
                    dto => dto.PregnancyResultMalamaUploadedOriginalFileName,
                    (dto, value) => dto.PregnancyResultMalamaUploadedOriginalFileName = value,
                    (dto, value) => dto.PregnancyResultMalamaUploadedDateTime = value),
                ("sickleMalamaFile", "sickle",
                    (Func<PostEventLabStationDto, bool>)(dto => dto.SickleCellResultMalamaUploaded),
                    (dto, value) => dto.SickleCellResultMalamaUploaded = value,
                    entity => entity.SickleCellResultMalamaUploadedFileName,
                    entity => entity.SickleCellResultMalamaUploadedOriginalFileName,
                    dto => dto.SickleCellResultMalamaUploadedFileName,
                    (dto, value) => dto.SickleCellResultMalamaUploadedFileName = value,
                    dto => dto.SickleCellResultMalamaUploadedOriginalFileName,
                    (dto, value) => dto.SickleCellResultMalamaUploadedOriginalFileName = value,
                    (dto, value) => dto.SickleCellResultMalamaUploadedDateTime = value)
            };
        }
    }
}
