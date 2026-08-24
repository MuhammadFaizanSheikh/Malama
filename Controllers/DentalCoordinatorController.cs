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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DentalCoordinatorController> _logger;
        private const string CLASSNAME = "DentalCoordinatorController";

        public DentalCoordinatorController(
            ILogger<DentalCoordinatorController> logger,
            IFileUploader fileUploader,
            IDentalQuestionnaireService dentalQuestionnaireService,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _fileUploader = fileUploader;
            _dentalQuestionnaireService = dentalQuestionnaireService;
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

                var questionnaire = await _dentalQuestionnaireService.GetByServiceMembersChildIdAsync(serviceMembersChildId)
                    ?? new DentalQuestionnaire { ServiceMembersChildId = serviceMembersChildId };

                var pageModel = new DentalCoordinatorStationPageViewModel
                {
                    ServiceMember = result.ServiceMembersChild,
                    Questionnaire = questionnaire
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
        public async Task<IActionResult> SaveDentalCoordinatorStation(DentalCoordinatorStationSaveDto dto)
        {
            const string methodName = nameof(SaveDentalCoordinatorStation);
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

                var validationError = DentalQuestionnaireValidator.Validate(dto, serviceMember);
                if (!string.IsNullOrWhiteSpace(validationError))
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = validationError;
                    return RedirectToAction(nameof(DentalCoordinatorStation), new { serviceMembersChildId = dto.ServiceMembersChildId });
                }

                await _dentalQuestionnaireService.SaveOrUpdateFromFormDataAsync(
                    dto, user.UserName, DentalQuestionnaireSources.DentalCoordinator);

                TempData["ResponseStatus"] = "success";
                TempData["ResponseTitle"] = "Success";
                TempData["ResponseMessage"] = "Dental Coordinator questionnaire saved successfully.";
                return RedirectToAction(nameof(DentalCoordinatorStation), new { serviceMembersChildId = dto.ServiceMembersChildId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while saving Dental Coordinator questionnaire",
                    CLASSNAME, methodName);

                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = ex.Message;
                return RedirectToAction(nameof(DentalCoordinatorStation), new { serviceMembersChildId = dto.ServiceMembersChildId });
            }
        }
    }
}
