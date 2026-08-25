using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Utilities;
using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExcelFilesCompiler.Controllers
{
    public class DentalCoordinatorController : Controller
    {
        private readonly IFileUploader _fileUploader;
        private readonly IDentalQuestionnaireService _dentalQuestionnaireService;
        private readonly IVitalStationService _vitalStationService;
        private readonly IDentalXRayStationService _dentalXRayStationService;
        private readonly IFileUploadDownloadService _fileService;
        private readonly DentalXRayFileSaveCoordinator _fileSaveCoordinator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DentalCoordinatorController> _logger;
        private const string CLASSNAME = "DentalCoordinatorController";
        private const string XRayStationName = "DentalXRay";

        public DentalCoordinatorController(
            ILogger<DentalCoordinatorController> logger,
            IFileUploader fileUploader,
            IDentalQuestionnaireService dentalQuestionnaireService,
            IVitalStationService vitalStationService,
            IDentalXRayStationService dentalXRayStationService,
            IFileUploadDownloadService fileService,
            DentalXRayFileSaveCoordinator fileSaveCoordinator,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _fileUploader = fileUploader;
            _dentalQuestionnaireService = dentalQuestionnaireService;
            _vitalStationService = vitalStationService;
            _dentalXRayStationService = dentalXRayStationService;
            _fileService = fileService;
            _fileSaveCoordinator = fileSaveCoordinator;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            const string methodName = "Index";
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            try
            {
                string eventId = HttpContext.Session.GetString("GlobalEventIdLong");

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Retrieved GlobalEventId: {EventId}",
                    CLASSNAME, methodName, eventId);

                if (string.IsNullOrWhiteSpace(eventId) || !int.TryParse(eventId, out int parsedEventId))
                {
                    _logger.LogWarning("{ClassName}, {MethodName}: Invalid EventId: {EventId}", CLASSNAME, methodName, eventId);

                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid EventId";
                    TempData["ResponseMessage"] = "Invalid EventId";

                    return View("Index");
                }

                var data = await _fileUploader.GetDentalCoordinatorByEventIdAsync(parsedEventId);

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Retrieved {Count} records for EventId={EventId}",
                    CLASSNAME, methodName, data.Count, eventId);

                ViewBag.Summary = new Dictionary<string, int>
                {
                    ["Total"] = data.Count
                };
                ViewBag.EventId = eventId;

                return View("Index", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while loading Dental Coordinator index page",
                    CLASSNAME, methodName);

                ViewBag.EventIdList = new List<SelectListItem>();
                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = ex.Message;

                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> DentalCoordinatorStation(long serviceMembersChildId)
        {
            const string methodName = nameof(DentalCoordinatorStation);
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            try
            {
                if (serviceMembersChildId <= 0)
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Request";
                    TempData["ResponseMessage"] = "Service member is required.";
                    return RedirectToAction(nameof(Index));
                }

                var result = await _fileUploader.GetServiceMemberChildWithEventIdAsync(serviceMembersChildId);
                if (result.ServiceMembersChild == null)
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Not Found";
                    TempData["ResponseMessage"] = "Service member not found.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.EventId = result.EventId;

                try
                {
                    var vitalVm = await _vitalStationService.GetVitalStationByServiceMemberChildIdAsync(serviceMembersChildId);
                    var vitalDto = vitalVm?.VitalStationDto ?? new VitalStationDto
                    {
                        ServiceMembersChildId = serviceMembersChildId,
                        Status = AppConstants.Status.Pending
                    };

                    ViewBag.VitalStation = vitalDto;
                    ViewBag.VitalsCompleted = string.Equals(vitalDto.Status, AppConstants.Status.Completed, StringComparison.OrdinalIgnoreCase);

                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Vital station loaded for ServiceMembersChildId={ServiceMembersChildId}. VitalStationId={VitalStationId}, Status={Status}",
                        CLASSNAME, methodName, serviceMembersChildId, vitalDto.Id, vitalDto.Status);
                }
                catch (Exception vitalEx)
                {
                    _logger.LogError(vitalEx,
                        "{ClassName}, {MethodName}, Failed to load vital station for ServiceMembersChildId={ServiceMembersChildId}",
                        CLASSNAME, methodName, serviceMembersChildId);

                    ViewBag.VitalStation = new VitalStationDto
                    {
                        ServiceMembersChildId = serviceMembersChildId,
                        Status = AppConstants.Status.Pending
                    };
                    ViewBag.VitalsCompleted = false;
                }

                var questionnaire = await _dentalQuestionnaireService.GetByServiceMembersChildIdAsync(serviceMembersChildId)
                    ?? new DentalQuestionnaire { ServiceMembersChildId = serviceMembersChildId };

                var xRayStation = await _dentalXRayStationService.GetByServiceMembersChildIdAsync(serviceMembersChildId)
                    ?? new DentalXRayStation
                    {
                        ServiceMembersChildId = serviceMembersChildId,
                        Status = AppConstants.Status.Pending,
                        PaImages = new List<DentalXRayPaImage>()
                    };

                xRayStation.ServiceMembersChild ??= result.ServiceMembersChild;

                var pageModel = new DentalCoordinatorStationPageViewModel
                {
                    ServiceMember = result.ServiceMembersChild,
                    Questionnaire = questionnaire,
                    XRayStation = xRayStation
                };

                return View(pageModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while loading Dental Coordinator station page",
                    CLASSNAME, methodName);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(104857600)]
        public async Task<IActionResult> SaveDentalCoordinatorStation(DentalXRayStationSaveDto dto)
        {
            const string methodName = nameof(SaveDentalCoordinatorStation);
            DentalQuestionnaireFormBinder.BindHealthConditions(dto, Request.Form);

            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            DentalXRayStation? existingRecord = null;
            DentalXRayFileUpdatePlan? filePlan = null;
            DentalXRayFileUploadSession? fileSession = null;
            var dbSaveCompleted = false;

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Unauthorized";
                    TempData["ResponseMessage"] = "Please login and try again.";
                    return RedirectToAction(nameof(Index));
                }

                if (dto.ServiceMembersChildId <= 0)
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = "Service member is required.";
                    return RedirectToAction(nameof(Index));
                }

                var serviceMemberResult = await _fileUploader.GetServiceMemberChildWithEventIdAsync(dto.ServiceMembersChildId);
                var serviceMember = serviceMemberResult.ServiceMembersChild;
                if (serviceMember == null)
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = "Service member not found.";
                    return RedirectToAction(nameof(Index));
                }

                var barcode = serviceMember.Barcode;
                if (string.IsNullOrWhiteSpace(barcode))
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = "Service member barcode is required for file upload.";
                    return RedirectToAction(nameof(DentalCoordinatorStation), new { serviceMembersChildId = dto.ServiceMembersChildId });
                }

                var validationError = DentalXRayStationSaveValidator.Validate(dto, serviceMember, _dentalQuestionnaireService);
                if (!string.IsNullOrWhiteSpace(validationError))
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = validationError;
                    return RedirectToAction(nameof(DentalCoordinatorStation), new { serviceMembersChildId = dto.ServiceMembersChildId });
                }

                if (dto.Id > 0)
                {
                    var existingResult = await _dentalXRayStationService
                        .GetDentalXRayStationByIdWithEventIdAsync(dto.Id);
                    existingRecord = existingResult.DentalXRayStation;
                    if (existingRecord == null)
                    {
                        TempData["ResponseStatus"] = "error";
                        TempData["ResponseTitle"] = "Not Found";
                        TempData["ResponseMessage"] = "Dental X-Ray record not found.";
                        return RedirectToAction(nameof(DentalCoordinatorStation), new { serviceMembersChildId = dto.ServiceMembersChildId });
                    }
                }

                filePlan = _fileSaveCoordinator.BuildPlan(dto, existingRecord, barcode);
                if (!string.IsNullOrWhiteSpace(filePlan.ErrorMessage))
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = filePlan.ErrorMessage;
                    return RedirectToAction(nameof(DentalCoordinatorStation), new { serviceMembersChildId = dto.ServiceMembersChildId });
                }

                fileSession = await _fileSaveCoordinator.UploadToStagingAsync(filePlan, barcode);
                if (!fileSession.Success)
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Upload Failed";
                    TempData["ResponseMessage"] = fileSession.ErrorMessage ?? "Failed to upload X-Ray image.";
                    return RedirectToAction(nameof(DentalCoordinatorStation), new { serviceMembersChildId = dto.ServiceMembersChildId });
                }

                DentalXRayStationSaveValidator.SetSectionUploadedDateTimes(dto);

                await _dentalQuestionnaireService.SaveOrUpdateFromFormDataAsync(
                    dto, user.UserName, DentalQuestionnaireSources.DentalCoordinator);

                var questionnaire = await _dentalQuestionnaireService.GetByServiceMembersChildIdAsync(dto.ServiceMembersChildId);

                var entity = _dentalXRayStationService.MapSaveDtoToEntity(dto);
                entity.Status = _dentalXRayStationService.ComputeOverallStatus(entity, serviceMember, questionnaire);

                if (dto.Id == 0)
                {
                    _logger.LogInformation("{ClassName}, {MethodName}, Add X-Ray operation started by User={UserName}",
                        CLASSNAME, methodName, user.UserName);
                    await _dentalXRayStationService.AddAsync(entity, user.UserName);
                }
                else
                {
                    _logger.LogInformation("{ClassName}, {MethodName}, Update X-Ray operation started for Id={Id} by User={UserName}",
                        CLASSNAME, methodName, dto.Id, user.UserName);
                    await _dentalXRayStationService.UpdateAsync(entity, user.UserName);
                }

                dbSaveCompleted = true;
                _fileSaveCoordinator.CommitFileChanges(filePlan, fileSession);

                TempData["ResponseStatus"] = "success";
                TempData["ResponseTitle"] = "Success";
                TempData["ResponseMessage"] = "Dental Coordinator questionnaire and X-Ray saved successfully.";
                return RedirectToAction(nameof(DentalCoordinatorStation), new { serviceMembersChildId = dto.ServiceMembersChildId });
            }
            catch (Exception ex)
            {
                if (!dbSaveCompleted && fileSession != null)
                {
                    await _fileSaveCoordinator.RollbackStagingAsync(fileSession);
                }

                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while saving Dental Coordinator station",
                    CLASSNAME, methodName);

                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = ex.Message;
                return RedirectToAction(nameof(DentalCoordinatorStation), new { serviceMembersChildId = dto.ServiceMembersChildId });
            }
        }

        public IActionResult DownloadXRayImage(string prefix, string fileName)
        {
            const string methodName = nameof(DownloadXRayImage);

            try
            {
                if (string.IsNullOrWhiteSpace(prefix) || string.IsNullOrWhiteSpace(fileName))
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Invalid download request", CLASSNAME, methodName);
                    return BadRequest("Invalid file download request.");
                }

                var file = _fileService.GetFile(XRayStationName, prefix, fileName);
                if (file == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, File not found: {FileName}", CLASSNAME, methodName, fileName);
                    return NotFound();
                }

                Response.Headers["Content-Disposition"] = $"inline; filename=\"{file.FileName}\"";
                return File(file.Bytes, file.ContentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception occurred while downloading file", CLASSNAME, methodName);
                return StatusCode(500, "Error while downloading file");
            }
        }
    }
}
