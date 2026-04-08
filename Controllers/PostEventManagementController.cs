using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using Malama.Attributes;
using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExcelFilesCompiler.Controllers
{
    public class PostEventManagementController : Controller
    {
        private readonly IEventManagementService _eventManagementService;
        private readonly IPostEventManagementService _postEventManagementService;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<PostEventManagementController> _logger;
        private const string CLASSNAME = "PostEventManagementController";

        public PostEventManagementController(ILogger<PostEventManagementController> logger, IEventManagementService eventManagementService, IPostEventManagementService postEventManagementService, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _eventManagementService = eventManagementService;
            _postEventManagementService = postEventManagementService;
            _configuration = configuration;
            _userManager = userManager;
        }

        //[RoleAttributeAuthorizeFromConfig("EventManagement_View")]
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
                eventManagementList = await _eventManagementService.GetAllForPostEventManagements();

                _logger.LogInformation("{ClassName}, {MethodName}, Retrieved {Count} event management records.",
                    CLASSNAME, methodName, eventManagementList.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Error while loading event managements.",
                    CLASSNAME, methodName);

                TempData["ErrorMessage"] =
                    "We encountered an issue while loading event managements on post event management page. Please try again later.";
            }

            var viewModel = new PostEventManagementViewModel
            {
                EventManagements = eventManagementList,
            };

            return View(viewModel);
        }


        //[RoleAttributeAuthorizeFromConfig("ImmunizationStation_View")]
        public async Task<IActionResult> PostEventManagement(long eventManagementId, long postEventManagementId)
        {
            const string methodName = "PostEventManagement";
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            try
            {
                PostEventManagementDto model;
                long eventId = 0;

                if (eventManagementId > 0)
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Edit. eventManagementId={eventManagementId}, postEventManagementId={postEventManagementId}",
                        CLASSNAME, methodName, eventManagementId, postEventManagementId
                    );

                    // Edit mode → get child record including parent
                    model = await _eventManagementService.GetForPostEventManagement(eventManagementId);
                    //model = result.Immunization;
                    //eventId = result.EventId;
                }
                else
                {
                    //_logger.LogInformation(
                    //    "{ClassName}, {MethodName}, Add mode. FileDataId={FileDataId}",
                    //    CLASSNAME, methodName, serviceMembersChildId
                    //);

                    // Add mode → create empty child but attach parent
                    model = await _postEventManagementService.GetById(postEventManagementId);

                }


                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Using EventId={EventId}",
                    CLASSNAME, methodName, eventId
                );

                

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Returning view",
                    CLASSNAME, methodName
                );

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Exception occurred while loading ImmunizationStation",
                    CLASSNAME, methodName
                );

                throw;
            }
        }

        //[RoleAttributeAuthorizeFromConfig("ImmunizationStation_Save")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> SaveImmunization(ImmunizationStation model)
        //{
        //    const string methodName = "SaveImmunization";
        //    _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            _logger.LogWarning(
        //                "{ClassName}, {MethodName}, ModelState is invalid",
        //                CLASSNAME, methodName
        //            );

        //            var allErrors = ModelState.Values
        //                .SelectMany(v => v.Errors)
        //                .Select(e => e.ErrorMessage)
        //                .ToList();

        //            var message = string.Join(" | ", allErrors);

        //            _logger.LogError(
        //                "{ClassName}, {MethodName}, Validation failed with errors: {Errors}",
        //                CLASSNAME, methodName, message
        //            );

        //            TempData["ResponseStatus"] = "error";
        //            TempData["ResponseTitle"] = "Invalid Data";
        //            TempData["ResponseMessage"] = message;

        //            var result = await _immunizationStationService.GetImmunizationByIdWithEventIdAsync(model.Id);
        //            if (result.Immunization == null)
        //            {
        //                TempData["ResponseStatus"] = "error";
        //                TempData["ResponseTitle"] = "Not Found";
        //                TempData["ResponseMessage"] = "Immunization record not found.";
        //                return RedirectToAction("Index");
        //            }
        //            model = result.Immunization;
        //            long eventId = result.EventId;
        //            ViewBag.EventId = eventId;

        //            _logger.LogDebug("{ClassName}, {MethodName}: Reloading view for EventId={EventId}", CLASSNAME, methodName, eventId);

        //            var immunizationData = await _immunizationStationService.GetImmunizationManufacturer(eventId);

        //            if (immunizationData.Success && immunizationData.Data != null)
        //            {
        //                ViewBag.ImmunizationData = immunizationData.Data;
        //            }
        //            else
        //            {
        //                _logger.LogError(
        //                    "{ClassName}, {MethodName}, Failed to load immunization manufacturer data. Success={Success}",
        //                    CLASSNAME, methodName, immunizationData.Success
        //                );

        //                ViewBag.ImmunizationData = new List<object>();
        //            }

        //            return View("ImmunizationStation", model);
        //        }

        //        var user = await _userManager.GetUserAsync(User);

        //        if (user == null)
        //        {
        //            _logger.LogError(
        //                "{ClassName}, {MethodName}, User not found / unauthorized access",
        //                CLASSNAME, methodName
        //            );

        //            TempData["ResponseStatus"] = "error";
        //            TempData["ResponseTitle"] = "Unauthorized";
        //            TempData["ResponseMessage"] = "Please login and try again.";

        //            return RedirectToAction("Index");
        //        }

        //        if (model.Id == 0)
        //        {
        //            _logger.LogInformation(
        //                "{ClassName}, {MethodName}, Add operation started by User={UserName}",
        //                CLASSNAME, methodName, user.UserName
        //            );

        //            await _immunizationStationService.AddAsync(model, user.UserName);

        //            TempData["ResponseStatus"] = "success";
        //            TempData["ResponseTitle"] = "Success";
        //            TempData["ResponseMessage"] = "Immunization record added successfully.";
        //        }
        //        else
        //        {
        //            _logger.LogInformation(
        //                "{ClassName}, {MethodName}, Update operation started for ImmunizationId={ImmunizationId} by User={UserName}",
        //                CLASSNAME, methodName, model.Id, user.UserName
        //            );

        //            await _immunizationStationService.UpdateAsync(model, user.UserName);

        //            TempData["ResponseStatus"] = "success";
        //            TempData["ResponseTitle"] = "Success";
        //            TempData["ResponseMessage"] = "Immunization record updated successfully.";
        //        }

        //        _logger.LogInformation(
        //            "{ClassName}, {MethodName}, Operation completed successfully. Redirecting to Index",
        //            CLASSNAME, methodName
        //        );

        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(
        //            ex,
        //            "{ClassName}, {MethodName}, Exception occurred while saving immunization record",
        //            CLASSNAME, methodName
        //        );

        //        TempData["ResponseStatus"] = "error";
        //        TempData["ResponseTitle"] = "Error";
        //        TempData["ResponseMessage"] = ex.Message;

        //        return View("ImmunizationStation", model);
        //    }
        //}

    }
}