using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using Malama.Attributes;
using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace ExcelFilesCompiler.Controllers
{
    public class ContainerController : Controller
    {
        private readonly IFileUploader _fileUploader;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IContainerMonitoringService _service;
        private readonly ILogger<ContainerController> _logger;
        private const string CLASSNAME = "ContainerController";


        public ContainerController(ILogger<ContainerController> logger, IFileUploader fileUploader, IConfiguration configuration, UserManager<ApplicationUser> userManager, IContainerMonitoringService service)
        {
            _logger = logger;
            _fileUploader = fileUploader;
            _configuration = configuration;
            _userManager = userManager;
            _service = service;
        }

        [RoleAttributeAuthorizeFromConfig("Container_View")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            const string methodName = "Index";
            List<Container> containers = new List<Container>();

            try
            {
                string eventId = HttpContext.Session.GetString("GlobalEventIdLong");

                _logger.LogInformation("{ClassName}, {MethodName}, Called. EventID from session: {EventID}",
                    CLASSNAME, methodName, eventId);

                if (string.IsNullOrWhiteSpace(eventId) || !long.TryParse(eventId, out long eventIdLong))
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Invalid or missing EventID in session: {EventID}",
                        CLASSNAME, methodName, eventId);

                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Session Expired";
                    TempData["ResponseMessage"] = "Event ID is missing or invalid. Please try again.";

                    return RedirectToAction("Index");
                }

                containers = await _service.GetContainersByEventIdAsync(eventIdLong);

                _logger.LogInformation("{ClassName}, {MethodName}, Retrieved {ContainerCount} containers for EventID: {EventID}",
                    CLASSNAME, methodName, containers.Count, eventId);

                ViewBag.EventId = eventId;
                return View("Index", containers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception occurred while fetching containers. EventID: {EventID}",
                    CLASSNAME, methodName, HttpContext.Session.GetString("GlobalEventIdLong"));

                ViewBag.EventIdList = new List<SelectListItem>();
                TempData["ErrorMessage"] = "Failed to load data: " + ex.Message;
                return View("Index", containers);
            }
        }

        //[HttpGet("Index")]
        //public async Task<IActionResult> GetContainersAgainstEventId(string eventId)
        //{
        //    List<Container> containers = new List<Container>();

        //    try
        //    {
        //        if (!string.IsNullOrEmpty(eventId))
        //        {
        //            containers = await _service.GetContainersByEventIdAsync(eventId);
        //        }

        //        // Keep selected event ID for maintaining state in view
        //        ViewBag.EventId = eventId;

        //        return View("Index", containers);
        //    }
        //    catch (ArgumentException argEx)
        //    {
        //        _logger.LogWarning(argEx, "Invalid EventId provided: {EventId}", eventId);
        //        TempData["ErrorMessage"] = "Invalid Event selected. Please try again.";
        //        return View("Index", containers);
        //    }
        //    catch (ApplicationException appEx)
        //    {
        //        _logger.LogError(appEx, "Error fetching data for EventId: {EventId}", eventId);
        //        TempData["ErrorMessage"] = "Unable to fetch vaccine records at this time.";
        //        return View("Index", containers);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Unexpected error in GetEventData for EventId: {EventId}", eventId);
        //        TempData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";
        //        return View("Index", containers);
        //    }
        //}

        [RoleAttributeAuthorizeFromConfig("Container_View")]
        [HttpGet("Add")]
        public async Task<IActionResult> Add(long eventId)
        {
            const string methodName = "Add";

            try
            {
                _logger.LogInformation("{ClassName}, {MethodName}, Called with EventID: {EventID}",
                    CLASSNAME, methodName, eventId);

                if (eventId <= 0)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Invalid EventID: {EventID}. Session may have expired.",
                        CLASSNAME, methodName, eventId);

                    TempData["ErrorMessage"] = "Session expired.";
                    return View("Index");
                }

                ViewBag.ContainerTypes = _service.GetAllContainerTypes();
                ViewBag.EventId = eventId;

                var model = new CreateContainerDto
                {
                    EventId = eventId,
                    StartDate = DateTime.Now.Date
                };

                _logger.LogInformation("{ClassName}, {MethodName}, Add Container page loaded successfully for EventID: {EventID}",
                    CLASSNAME, methodName, eventId);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception occurred while loading Add Container page. EventID: {EventID}",
                    CLASSNAME, methodName, eventId);

                TempData["ErrorMessage"] = "An unexpected error occurred while loading the Add Container page.";
                return RedirectToAction("Index");
            }
        }

        [RoleAttributeAuthorizeFromConfig("Container_Save")]
        [HttpPost("Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CreateContainerDto dto)
        {
            const string methodName = "Add (POST)";

            try
            {
                _logger.LogInformation("{ClassName}, {MethodName}, Called with EventID: {EventID}, ContainerName: {ContainerName}",
                    CLASSNAME, methodName, dto.EventId, dto.ContainerName);

                var containerTypes = _service.GetAllContainerTypes();
                ViewBag.ContainerTypes = containerTypes;
                ViewBag.EventId = dto.EventId;

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, User not found or not logged in while adding container. EventID: {EventID}",
                        CLASSNAME, methodName, dto.EventId);

                    TempData["ErrorMessage"] = "Please login and try again.";
                    return RedirectToAction("Index");
                }

                // Let service handle validation and creation
                var result = await _service.AddContainerAsync(dto, user.UserName);

                if (!result.Success)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Failed to add container. EventID: {EventID}, User: {UserName}, Error: {Error}",
                        CLASSNAME, methodName, dto.EventId, user.UserName, result.Message);

                    TempData["ErrorMessage"] = result.Message;
                    return View(dto);
                }

                // Load event containers if needed
                var containers = dto.EventId <= 0
                    ? new List<Container>()
                    : await _service.GetContainersByEventIdAsync(dto.EventId);

                _logger.LogInformation("{ClassName}, {MethodName}, Container added successfully. EventID: {EventID}, User: {UserName}, ContainerName: {ContainerName}",
                    CLASSNAME, methodName, dto.EventId, user.UserName, dto.ContainerName);

                TempData["ResponseStatus"] = "success";
                TempData["ResponseTitle"] = "Success";
                TempData["ResponseMessage"] = "Container saved successfully!";

                return View("Index", containers);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "{ClassName}, {MethodName}, ArgumentException occurred while adding container. EventID: {EventID}, ContainerName: {ContainerName}",
                    CLASSNAME, methodName, dto.EventId, dto.ContainerName);

                TempData["ErrorMessage"] = ex.Message;
                return View(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception occurred while adding container. EventID: {EventID}, ContainerName: {ContainerName}",
                    CLASSNAME, methodName, dto.EventId, dto.ContainerName);

                TempData["ErrorMessage"] = "Unexpected error occurred while adding container.";
                return View(dto);
            }
        }

        [RoleAttributeAuthorizeFromConfig("Container_View")]
        [HttpGet("Monitor/{id}")]
        public async Task<IActionResult> Monitor(long id)
        {
            const string methodName = "Monitor (GET)";

            try
            {
                _logger.LogInformation("{ClassName}, {MethodName}, Called with ContainerID: {ContainerID}",
                    CLASSNAME, methodName, id);

                var container = await _service.GetContainerByIdAsync(id);
                if (container == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Container not found. ContainerID: {ContainerID}",
                        CLASSNAME, methodName, id);

                    TempData["ErrorMessage"] = "Container not found.";
                    return RedirectToAction("Index");
                }

                var readings = await _service.GetReadingsForContainer(container.Id);
                ViewBag.Readings = readings;

                _logger.LogInformation("{ClassName}, {MethodName}, Monitor page loaded successfully. ContainerID: {ContainerID}, ReadingCount: {ReadingCount}",
                    CLASSNAME, methodName, id, readings.Count);

                return View(container);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception occurred while loading monitor page. ContainerID: {ContainerID}",
                    CLASSNAME, methodName, id);

                TempData["ErrorMessage"] = "An unexpected error occurred while loading the monitor page.";
                return RedirectToAction("Index");
            }
        }


        [HttpPost("Monitor/{id}")]
        [ValidateAntiForgeryToken]
        [RoleAttributeAuthorizeFromConfig("Container_Save")]
        public async Task<IActionResult> Monitor(long id, CreateReadingDto dto)
        {
            const string methodName = "Monitor (POST)";

            try
            {
                _logger.LogInformation("{ClassName}, {MethodName}, Called with ContainerID: {ContainerID}",
                    CLASSNAME, methodName, id);

                dto.ContainerId = id;
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, User not logged in while adding reading. ContainerID: {ContainerID}",
                        CLASSNAME, methodName, id);

                    TempData["ErrorMessage"] = "Please login and try again.";
                    return RedirectToAction("Index");
                }

                var result = await _service.AddReadingAsync(dto, user.UserName);

                if (!result.Success)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Failed to add reading. ContainerID: {ContainerID}, User: {UserName}, Error: {Error}",
                        CLASSNAME, methodName, id, user.UserName, result.Message);

                    TempData["ErrorMessage"] = result.Message;
                    return RedirectToAction("Monitor", new { id });
                }

                _logger.LogInformation("{ClassName}, {MethodName}, Reading added successfully. ContainerID: {ContainerID}, User: {UserName}",
                    CLASSNAME, methodName, id, user.UserName);

                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception occurred while saving reading. ContainerID: {ContainerID}",
                    CLASSNAME, methodName, id);

                TempData["ErrorMessage"] = "An unexpected error occurred while saving the reading.";
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public async Task<IActionResult> AcknowledgeNotification([FromBody] long notificationId)
        {
            const string methodName = "AcknowledgeNotification";

            try
            {
                _logger.LogInformation("{ClassName}, {MethodName}, Called with NotificationID: {NotificationID}",
                    CLASSNAME, methodName, notificationId);

                await _service.AcknowledgeNotificationAsync(notificationId);

                _logger.LogInformation("{ClassName}, {MethodName}, Notification acknowledged successfully. NotificationID: {NotificationID}",
                    CLASSNAME, methodName, notificationId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Exception occurred while acknowledging notification. NotificationID: {NotificationID}",
                    CLASSNAME, methodName, notificationId);

                return StatusCode(500, new { message = ex.Message });
            }
        }


    }
}