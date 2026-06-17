using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Utilities;
using Malama.Attributes;
using Malama.Models;
using Malama.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExcelFilesCompiler.Controllers
{
    public class DentalExamsController : Controller
    {
        private readonly IFileUploader _fileUploader;
        private readonly IDentalQuestionnaireService _dentalQuestionnaireService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DentalExamsController> _logger;
        private const string CLASSNAME = "DentalExamsController";

        public DentalExamsController(
            ILogger<DentalExamsController> logger,
            IFileUploader fileUploader,
            IDentalQuestionnaireService dentalQuestionnaireService,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _fileUploader = fileUploader;
            _dentalQuestionnaireService = dentalQuestionnaireService;
            _userManager = userManager;
        }

        [RoleAttributeAuthorizeFromConfig("DentalExams_View")]
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

                var data = await _fileUploader.GetDentalExamsByEventIdAsync(parsedEventId);

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
                    "{ClassName}, {MethodName}, Exception occurred while loading Dental Exams index page",
                    CLASSNAME, methodName);

                ViewBag.EventIdList = new List<SelectListItem>();
                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = ex.Message;

                return View();
            }
        }

        [RoleAttributeAuthorizeFromConfig("DentalExams_View")]
        [HttpGet]
        public async Task<IActionResult> DentalExamStation(long serviceMembersChildId)
        {
            const string methodName = nameof(DentalExamStation);
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

                var questionnaire = await _dentalQuestionnaireService.GetByServiceMembersChildIdAsync(serviceMembersChildId)
                    ?? new DentalQuestionnaire { ServiceMembersChildId = serviceMembersChildId };

                var pageModel = new DentalExamStationPageViewModel
                {
                    ServiceMember = result.ServiceMembersChild,
                    Questionnaire = questionnaire,
                    IsQuestionnaireReadOnly = questionnaire.Id > 0
                };

                return View(pageModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while loading Dental Exam station page",
                    CLASSNAME, methodName);
                throw;
            }
        }

        [RoleAttributeAuthorizeFromConfig("DentalExams_Save")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDentalExamStation(DentalExamStationSaveDto dto)
        {
            const string methodName = nameof(SaveDentalExamStation);

            dto.IsQuestionnaireReadOnly = string.Equals(
                Request.Form["IsQuestionnaireReadOnly"],
                "true",
                StringComparison.OrdinalIgnoreCase);
            DentalQuestionnaireFormBinder.BindHealthConditions(dto, Request.Form);

            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

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

                var validationError = await ValidateSaveDtoAsync(dto, serviceMember);
                if (!string.IsNullOrWhiteSpace(validationError))
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = validationError;
                    return RedirectToAction(nameof(DentalExamStation), new { serviceMembersChildId = dto.ServiceMembersChildId });
                }

                if (!dto.IsQuestionnaireReadOnly)
                {
                    await _dentalQuestionnaireService.SaveOrUpdateFromFormDataAsync(dto, user.UserName);
                }

                TempData["ResponseStatus"] = "success";
                TempData["ResponseTitle"] = "Success";
                TempData["ResponseMessage"] = "Dental Exam record saved successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while saving Dental Exam record",
                    CLASSNAME, methodName);

                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = ex.Message;
                return RedirectToAction(nameof(DentalExamStation), new { serviceMembersChildId = dto.ServiceMembersChildId });
            }
        }

        private async Task<string?> ValidateSaveDtoAsync(DentalExamStationSaveDto dto, ServiceMembersChild serviceMember)
        {
            if (dto.IsQuestionnaireReadOnly)
            {
                return null;
            }

            return DentalQuestionnaireValidator.Validate(dto, serviceMember);
        }
    }
}
