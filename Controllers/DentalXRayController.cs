using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Utilities;
using Malama.Attributes;
using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExcelFilesCompiler.Controllers
{
    public class DentalXRayController : Controller
    {
        private readonly IFileUploader _fileUploader;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDentalXRayStationService _dentalXRayStationService;
        private readonly IDentalQuestionnaireService _dentalQuestionnaireService;
        private readonly IVitalStationService _vitalStationService;
        private readonly IFileUploadDownloadService _fileService;
        private readonly DentalXRayFileSaveCoordinator _fileSaveCoordinator;
        private readonly ILogger<DentalXRayController> _logger;
        private const string CLASSNAME = "DentalXRayController";
        private const string StationName = "DentalXRay";

        public DentalXRayController(
            ILogger<DentalXRayController> logger,
            IFileUploader fileUploader,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            IDentalXRayStationService dentalXRayStationService,
            IDentalQuestionnaireService dentalQuestionnaireService,
            IVitalStationService vitalStationService,
            IFileUploadDownloadService fileService,
            DentalXRayFileSaveCoordinator fileSaveCoordinator)
        {
            _logger = logger;
            _fileUploader = fileUploader;
            _configuration = configuration;
            _userManager = userManager;
            _dentalXRayStationService = dentalXRayStationService;
            _dentalQuestionnaireService = dentalQuestionnaireService;
            _vitalStationService = vitalStationService;
            _fileService = fileService;
            _fileSaveCoordinator = fileSaveCoordinator;
        }

        [RoleAttributeAuthorizeFromConfig("DentalXRay_View")]
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

                var data = await _fileUploader.GetDentalXRayStationByEventIdAsync(parsedEventId);

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Retrieved {Count} records for EventId={EventId}",
                    CLASSNAME, methodName, data.Count, eventId);

                var summary = new Dictionary<string, int>
                {
                    ["Total"] = data.Count,
                    ["Pending"] = data.Count(x => x.DentalXRayStationRecord == null || x.DentalXRayStationRecord.Status == "Pending"),
                    ["Completed"] = data.Count(x => x.DentalXRayStationRecord?.Status == "Completed")
                };

                ViewBag.Summary = summary;
                ViewBag.EventId = eventId;

                return View("Index", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while loading Dental X-Ray index page",
                    CLASSNAME, methodName);

                ViewBag.EventIdList = new List<SelectListItem>();
                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = ex.Message;

                return View();
            }
        }

        [RoleAttributeAuthorizeFromConfig("DentalXRay_View")]
        public async Task<IActionResult> DentalXRayStation(long dentalXRayStationId, long serviceMembersChildId)
        {
            const string methodName = "DentalXRayStation";
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            try
            {
                DentalXRayStation model;
                long eventId = 0;

                if (dentalXRayStationId > 0)
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Edit mode. dentalXRayStationId={DentalXRayStationId}",
                        CLASSNAME, methodName, dentalXRayStationId);

                    var result = await _dentalXRayStationService.GetDentalXRayStationByIdWithEventIdAsync(dentalXRayStationId);
                    model = result.DentalXRayStation;
                    eventId = result.EventId;

                    if (model == null)
                    {
                        TempData["ResponseStatus"] = "error";
                        TempData["ResponseTitle"] = "Not Found";
                        TempData["ResponseMessage"] = "Dental X-Ray record not found.";
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Add mode. ServiceMembersChildId={ServiceMembersChildId}",
                        CLASSNAME, methodName, serviceMembersChildId);

                    if (serviceMembersChildId <= 0)
                    {
                        TempData["ResponseStatus"] = "error";
                        TempData["ResponseTitle"] = "Invalid Request";
                        TempData["ResponseMessage"] = "Service member is required.";
                        return RedirectToAction(nameof(Index));
                    }

                    var result = await _fileUploader.GetServiceMemberChildWithEventIdAsync(serviceMembersChildId);

                    model = new DentalXRayStation
                    {
                        ServiceMembersChildId = serviceMembersChildId,
                        ServiceMembersChild = result.ServiceMembersChild,
                        Status = "Pending",
                        PaImages = new List<DentalXRayPaImage>
                        {
                            new DentalXRayPaImage { SortOrder = 0 }
                        }
                    };

                    eventId = result.EventId;
                }

                ViewBag.EventId = eventId;

                try
                {
                    var vitalVm = await _vitalStationService.GetVitalStationByServiceMemberChildIdAsync(model.ServiceMembersChildId);
                    var vitalDto = vitalVm?.VitalStationDto ?? new VitalStationDto
                    {
                        ServiceMembersChildId = model.ServiceMembersChildId,
                        Status = AppConstants.Status.Pending
                    };

                    ViewBag.VitalStation = vitalDto;
                    ViewBag.VitalsCompleted = string.Equals(vitalDto.Status, AppConstants.Status.Completed, StringComparison.OrdinalIgnoreCase);

                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Vital station loaded for ServiceMembersChildId={ServiceMembersChildId}. VitalStationId={VitalStationId}, Status={Status}",
                        CLASSNAME, methodName, model.ServiceMembersChildId, vitalDto.Id, vitalDto.Status);
                }
                catch (Exception vitalEx)
                {
                    _logger.LogError(vitalEx,
                        "{ClassName}, {MethodName}, Failed to load vital station for ServiceMembersChildId={ServiceMembersChildId}",
                        CLASSNAME, methodName, model.ServiceMembersChildId);

                    ViewBag.VitalStation = new VitalStationDto
                    {
                        ServiceMembersChildId = model.ServiceMembersChildId,
                        Status = AppConstants.Status.Pending
                    };
                    ViewBag.VitalsCompleted = false;
                }

                var questionnaire = await _dentalQuestionnaireService.GetByServiceMembersChildIdAsync(model.ServiceMembersChildId)
                    ?? new DentalQuestionnaire { ServiceMembersChildId = model.ServiceMembersChildId };

                var pageModel = new DentalXRayStationPageViewModel
                {
                    Station = model,
                    Questionnaire = questionnaire
                };

                return View(pageModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while loading Dental X-Ray station page",
                    CLASSNAME, methodName);
                throw;
            }
        }

        [RoleAttributeAuthorizeFromConfig("DentalXRay_Save")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(104857600)]
        public async Task<IActionResult> SaveDentalXRayStation(DentalXRayStationSaveDto dto)
        {
            const string methodName = "SaveDentalXRayStation";
            var goToVitalStation = dto.GoToVitalStation
                || string.Equals(Request.Form["GoToVitalStation"], "true", StringComparison.OrdinalIgnoreCase);
            dto.GoToVitalStation = goToVitalStation;
            BindHealthConditionsFromForm(dto, Request.Form);

            _logger.LogInformation("{ClassName}, {MethodName}, Called. GoToVitalStation={GoToVitalStation}",
                CLASSNAME, methodName, goToVitalStation);

            DentalXRayStation? existingRecord = null;
            DentalXRayFileUpdatePlan? filePlan = null;
            DentalXRayFileUploadSession? fileSession = null;
            var dbSaveCompleted = false;

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogError("{ClassName}, {MethodName}, User not found / unauthorized access", CLASSNAME, methodName);
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

                // Go to Vital Station: redirect only — do not validate or persist form/files.
                if (dto.GoToVitalStation)
                {
                    long vitalStationId = 0;
                    try
                    {
                        var vitalVm = await _vitalStationService.GetVitalStationByServiceMemberChildIdAsync(dto.ServiceMembersChildId);
                        vitalStationId = vitalVm?.VitalStationDto?.Id ?? 0;
                    }
                    catch (Exception vitalEx)
                    {
                        _logger.LogWarning(vitalEx,
                            "{ClassName}, {MethodName}, Could not load vital station id before redirect. ServiceMembersChildId={ServiceMembersChildId}",
                            CLASSNAME, methodName, dto.ServiceMembersChildId);
                    }

                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Redirecting to Vital Station without saving. DentalXRayStationId={DentalXRayStationId}, ServiceMembersChildId={ServiceMembersChildId}",
                        CLASSNAME, methodName, dto.Id, dto.ServiceMembersChildId);

                    return RedirectToAction("VitalStation", "VitalStation", new
                    {
                        vitalStationId,
                        serviceMembersChildId = dto.ServiceMembersChildId,
                        returnTo = "DentalXRay",
                        dentalXRayStationId = dto.Id
                    });
                }

                var barcode = serviceMember.Barcode;
                if (string.IsNullOrWhiteSpace(barcode))
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = "Service member barcode is required for file upload.";
                    return RedirectToAction(nameof(Index));
                }

                var validationError = DentalXRayStationSaveValidator.Validate(dto, serviceMember, _dentalQuestionnaireService);
                if (!string.IsNullOrWhiteSpace(validationError))
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Validation failed: {Error}", CLASSNAME, methodName, validationError);
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = validationError;
                    return RedirectToAction(nameof(DentalXRayStation), new
                    {
                        dentalXRayStationId = dto.Id,
                        serviceMembersChildId = dto.ServiceMembersChildId
                    });
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
                        return RedirectToAction(nameof(Index));
                    }
                }

                filePlan = _fileSaveCoordinator.BuildPlan(dto, existingRecord, barcode);
                if (!string.IsNullOrWhiteSpace(filePlan.ErrorMessage))
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = filePlan.ErrorMessage;
                    return RedirectToAction(nameof(DentalXRayStation), new
                    {
                        dentalXRayStationId = dto.Id,
                        serviceMembersChildId = dto.ServiceMembersChildId
                    });
                }

                fileSession = await _fileSaveCoordinator.UploadToStagingAsync(filePlan, barcode);
                if (!fileSession.Success)
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Upload Failed";
                    TempData["ResponseMessage"] = fileSession.ErrorMessage ?? "Failed to upload X-Ray image.";
                    return RedirectToAction(nameof(DentalXRayStation), new
                    {
                        dentalXRayStationId = dto.Id,
                        serviceMembersChildId = dto.ServiceMembersChildId
                    });
                }

                DentalXRayStationSaveValidator.SetSectionUploadedDateTimes(dto);

                await _dentalQuestionnaireService.SaveOrUpdateFromFormDataAsync(
                    dto, user.UserName, DentalQuestionnaireSources.DentalXRay);

                var questionnaire = await _dentalQuestionnaireService.GetByServiceMembersChildIdAsync(dto.ServiceMembersChildId);

                var entity = _dentalXRayStationService.MapSaveDtoToEntity(dto);
                entity.Status = _dentalXRayStationService.ComputeOverallStatus(entity, serviceMember, questionnaire);

                if (dto.Id == 0)
                {
                    _logger.LogInformation("{ClassName}, {MethodName}, Add operation started by User={UserName}",
                        CLASSNAME, methodName, user.UserName);
                    await _dentalXRayStationService.AddAsync(entity, user.UserName, DentalXRaySources.DentalXRay);
                }
                else
                {
                    _logger.LogInformation("{ClassName}, {MethodName}, Update operation started for Id={Id} by User={UserName}",
                        CLASSNAME, methodName, dto.Id, user.UserName);
                    await _dentalXRayStationService.UpdateAsync(entity, user.UserName, DentalXRaySources.DentalXRay);
                }

                dbSaveCompleted = true;
                _fileSaveCoordinator.CommitFileChanges(filePlan, fileSession);

                TempData["ResponseStatus"] = "success";
                TempData["ResponseTitle"] = "Success";
                TempData["ResponseMessage"] = "Dental X-Ray record saved successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (!dbSaveCompleted && fileSession != null)
                {
                    await _fileSaveCoordinator.RollbackStagingAsync(fileSession);
                }

                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception occurred while saving Dental X-Ray record", CLASSNAME, methodName);

                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = ex.Message;

                return RedirectToAction(nameof(DentalXRayStation), new
                {
                    dentalXRayStationId = dto.Id,
                    serviceMembersChildId = dto.ServiceMembersChildId
                });
            }
        }

        [RoleAttributeAuthorizeFromConfig("DentalXRay_View")]
        public IActionResult DownloadXRayImage(string prefix, string fileName)
        {
            const string methodName = "DownloadXRayImage";

            try
            {
                if (string.IsNullOrWhiteSpace(prefix) || string.IsNullOrWhiteSpace(fileName))
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Invalid download request", CLASSNAME, methodName);
                    return BadRequest("Invalid file download request.");
                }

                var file = _fileService.GetFile(StationName, prefix, fileName);
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

        private static void BindHealthConditionsFromForm(DentalXRayStationSaveDto dto, IFormCollection form)
        {
            DentalQuestionnaireFormBinder.BindHealthConditions(dto, form);
        }
    }
}
