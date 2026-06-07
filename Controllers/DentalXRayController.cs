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
            IFileUploadDownloadService fileService,
            DentalXRayFileSaveCoordinator fileSaveCoordinator)
        {
            _logger = logger;
            _fileUploader = fileUploader;
            _configuration = configuration;
            _userManager = userManager;
            _dentalXRayStationService = dentalXRayStationService;
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
                return View(model);
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

                var barcode = serviceMember.Barcode;
                if (string.IsNullOrWhiteSpace(barcode))
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = "Service member barcode is required for file upload.";
                    return RedirectToAction(nameof(Index));
                }

                var validationError = ValidateSaveDto(dto, serviceMember);
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

                SetSectionUploadedDateTimes(dto);

                var entity = _dentalXRayStationService.MapSaveDtoToEntity(dto);
                entity.Status = _dentalXRayStationService.ComputeOverallStatus(entity, serviceMember);

                if (dto.Id == 0)
                {
                    _logger.LogInformation("{ClassName}, {MethodName}, Add operation started by User={UserName}",
                        CLASSNAME, methodName, user.UserName);
                    await _dentalXRayStationService.AddAsync(entity, user.UserName);
                }
                else
                {
                    _logger.LogInformation("{ClassName}, {MethodName}, Update operation started for Id={Id} by User={UserName}",
                        CLASSNAME, methodName, dto.Id, user.UserName);
                    await _dentalXRayStationService.UpdateAsync(entity, user.UserName);
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

        private string? ValidateSaveDto(DentalXRayStationSaveDto dto, ServiceMembersChild serviceMember)
        {
            var questionnaireError = ValidateQuestionnaire(dto, serviceMember);
            if (questionnaireError != null) return questionnaireError;

            if (DentalXRayStationService.IsFemale(serviceMember))
            {
                if (string.IsNullOrWhiteSpace(dto.AreYouPregnant))
                {
                    return "Pregnancy question is required for female service members.";
                }

                if (dto.AreYouPregnant.Equals("Yes", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(dto.PregnancyApproval))
                    {
                        return "Approval selection is required when pregnant.";
                    }

                    if (dto.PregnancyApproval.Equals("Declined", StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }
                }
            }

            if (!DentalXRayStationService.CanProceedWithXRay(
                _dentalXRayStationService.MapSaveDtoToEntity(dto), serviceMember))
            {
                return "Cannot proceed with X-Ray based on questionnaire responses.";
            }

            if (DentalXRayStationService.IsNeeded(serviceMember.BwxNeeded))
            {
                var bwxError = ValidateSection(
                    dto.BwxStatus,
                    dto.BwxReason,
                    dto.BwxStatus == "Completed" && !AreAllBwxUploadsPresent(dto),
                    "BWX Status");
                if (bwxError != null) return bwxError;
            }

            if (DentalXRayStationService.IsNeeded(serviceMember.BwxNeeded))
            {
                var paError = ValidatePaSection(dto);
                if (paError != null) return paError;
            }

            return null;
        }

        private static string? ValidateQuestionnaire(DentalXRayStationSaveDto dto, ServiceMembersChild serviceMember)
        {
            var yesNoQuestions = new (string? Value, string Label)[]
            {
                (dto.HealthcareProviderCareLast2Years, "Question 1"),
                (dto.SeriousIllnessOperationHospitalization, "Question 2"),
                (dto.MedicationFoodAllergy, "Question 3"),
                (dto.TakingMedications, "Question 4"),
                (dto.HepatitisOrJaundice, "Question 5"),
                (dto.HealthChangeLastTwoYears, "Question 6"),
                (dto.UseTobaccoOrVape, "Question 7"),
                (dto.DrinkAlcoholicBeverages, "Question 8"),
                (dto.SickFromDentalTreatment, "Question 9"),
                (dto.BleederOrExcessiveBleeding, "Question 10"),
                (dto.ShortOfBreathOneFlightStairs, "Question 11")
            };

            foreach (var (value, label) in yesNoQuestions)
            {
                if (!IsYesOrNo(value))
                {
                    return $"{label} is required.";
                }
            }

            var detailError = ValidateYesDetail(
                dto.SeriousIllnessOperationHospitalization,
                dto.SeriousIllnessOperationHospitalizationDetail,
                "Question 2");
            if (detailError != null) return detailError;

            detailError = ValidateYesDetail(dto.MedicationFoodAllergy, dto.MedicationFoodAllergyDetail, "Question 3");
            if (detailError != null) return detailError;

            detailError = ValidateYesDetail(dto.TakingMedications, dto.TakingMedicationsDetail, "Question 4");
            if (detailError != null) return detailError;

            if (IsYes(dto.UseTobaccoOrVape))
            {
                foreach (var type in DentalXRayQuestionnaire.TobaccoTypes)
                {
                    var tobacco = dto.TobaccoUseDetails?.FirstOrDefault(t =>
                        string.Equals(t.Type, type, StringComparison.OrdinalIgnoreCase));
                    if (tobacco == null || !IsYesOrNo(tobacco.Used))
                    {
                        return $"Question 7 ({type}) selection is required.";
                    }

                    if (IsYes(tobacco.Used))
                    {
                        var hasDay = !string.IsNullOrWhiteSpace(tobacco.TimesPerDay);
                        var hasWeek = !string.IsNullOrWhiteSpace(tobacco.TimesPerWeek);
                        if (!hasDay && !hasWeek)
                        {
                            return $"Question 7 ({type}) requires Times per day or Times per week when Yes is selected.";
                        }
                    }
                }
            }

            if (IsYes(dto.DrinkAlcoholicBeverages) &&
                string.IsNullOrWhiteSpace(dto.AlcoholicBeveragesFrequencyQuantity))
            {
                return "Question 8 requires Frequency / Quantity when Yes is selected.";
            }

            if (DentalXRayStationService.IsFemale(serviceMember))
            {
                if (!IsYesOrNo(dto.AreYouPregnant))
                {
                    return "Question 12 is required for female service members.";
                }
            }

            return null;
        }

        private static string? ValidateYesDetail(string? answer, string? detail, string label)
        {
            if (IsYes(answer) && string.IsNullOrWhiteSpace(detail))
            {
                return $"{label} requires detail when Yes is selected.";
            }

            return null;
        }

        private static bool IsYes(string? value)
        {
            return string.Equals(value?.Trim(), "Yes", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsYesOrNo(string? value)
        {
            return IsYes(value) || string.Equals(value?.Trim(), "No", StringComparison.OrdinalIgnoreCase);
        }

        private static string? ValidateSection(string? status, string? reason, bool uploadsMissing, string label)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return $"{label} is required.";
            }

            if (status == "Not Completed" && string.IsNullOrWhiteSpace(reason))
            {
                return $"{label} reason is required.";
            }

            if (status == "Completed" && uploadsMissing)
            {
                return $"{label} requires all image uploads.";
            }

            return null;
        }

        private static string? ValidatePaSection(DentalXRayStationSaveDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.PaStatus))
            {
                return "Periapical (PA) X-Rays Status is required.";
            }

            if (dto.PaStatus == "Not Completed" && string.IsNullOrWhiteSpace(dto.PaReason))
            {
                return "Periapical (PA) X-Rays reason is required.";
            }

            if (dto.PaStatus == "Completed")
            {
                var activePaImages = dto.PaImages?.Where(p => !p.Removed).ToList() ?? new List<DentalXRayPaImageDto>();
                if (!activePaImages.Any())
                {
                    return "At least one PA X-Ray image is required.";
                }

                if (activePaImages.Count > 8)
                {
                    return "A maximum of 8 PA X-Ray uploads is allowed.";
                }

                foreach (var image in activePaImages)
                {
                    var hasExisting = !string.IsNullOrWhiteSpace(image.FileName);
                    var hasNew = image.ImageFile != null && image.ImageFile.Length > 0;
                    if (!hasExisting && !hasNew)
                    {
                        return "All PA X-Ray cards require an uploaded image.";
                    }
                }
            }

            return null;
        }

        private static bool AreAllBwxUploadsPresent(DentalXRayStationSaveDto dto)
        {
            return HasUpload(dto.BwLeftMolarUploaded, dto.BwLeftMolarFileName, dto.BwLeftMolarFile, dto.BwLeftMolarRemoved)
                && HasUpload(dto.BwLeftPremolarUploaded, dto.BwLeftPremolarFileName, dto.BwLeftPremolarFile, dto.BwLeftPremolarRemoved)
                && HasUpload(dto.BwRightMolarUploaded, dto.BwRightMolarFileName, dto.BwRightMolarFile, dto.BwRightMolarRemoved)
                && HasUpload(dto.BwRightPremolarUploaded, dto.BwRightPremolarFileName, dto.BwRightPremolarFile, dto.BwRightPremolarRemoved);
        }

        private static bool HasUpload(bool uploadedFlag, string? fileName, IFormFile? file, bool removed)
        {
            if (removed)
            {
                return false;
            }

            return uploadedFlag || !string.IsNullOrWhiteSpace(fileName) || (file != null && file.Length > 0);
        }

        private static void SetSectionUploadedDateTimes(DentalXRayStationSaveDto dto)
        {
            if (dto.BwxStatus == "Completed" && AreAllBwxUploadsPresent(dto))
            {
                dto.BwxUploadedDateTime ??= DateTime.Now;
            }
            else
            {
                dto.BwxUploadedDateTime = null;
            }

            if (dto.PaStatus == "Completed" &&
                dto.PaImages != null &&
                dto.PaImages.Any(p => !p.Removed && !string.IsNullOrWhiteSpace(p.FileName)))
            {
                dto.PaUploadedDateTime ??= DateTime.Now;
            }
            else
            {
                dto.PaUploadedDateTime = null;
            }

        }
    }
}
