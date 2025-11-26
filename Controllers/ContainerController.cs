using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace ExcelFilesCompiler.Controllers
{
    //[Authorize(Roles = "DAWSON Admin - Event Staff,Project Manager & Program Manager,Super Admin")]
    public class ContainerController : Controller
    {
        private readonly IFileUploader _fileUploader;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IContainerMonitoringService _service;
        private readonly ILogger<ImmunizationStationController> _logger;
         

        public ContainerController(ILogger<ImmunizationStationController> logger, IFileUploader fileUploader, IConfiguration configuration, UserManager<ApplicationUser> userManager, IContainerMonitoringService service)
        {
            _logger = logger;
            _fileUploader = fileUploader;
            _configuration = configuration;
            _userManager = userManager;
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Container> containers = new List<Container>();

            try
            {
                //var eventIds = await _fileUploader.GetDistinctEventIdsAsync();

                //var dropdownList = eventIds.Select(e => new SelectListItem
                //{
                //    Value = e,
                //    Text = e
                //}).ToList();

                //ViewBag.EventIdList = dropdownList;

                string eventId = HttpContext.Session.GetString("GlobalEventId");
                containers = await _service.GetContainersByEventIdAsync(eventId);
                ViewBag.EventId = eventId;
                return View("Index", containers);
            }
            catch (Exception ex)
            {
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

        [HttpGet("Add")]
        public async Task<IActionResult> Add(string eventId)
        {
            try
            {
                if (string.IsNullOrEmpty(eventId))
                {
                    TempData["ErrorMessage"] = "Event ID is required.";

                    return View("Index");
                }

                ViewBag.ContainerTypes = await _service.GetAllContainerTypesAsync();
                ViewBag.EventId = eventId;

                var model = new CreateContainerDto
                {
                    EventId = eventId,
                    StartDate = DateTime.Now.Date
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while loading the Add Container page.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost("Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CreateContainerDto dto)
        {
            try
            {
                // Populate dropdown for view reload (always)
                var containerTypes = await _service.GetAllContainerTypesAsync();
                ViewBag.ContainerTypes = containerTypes;
                ViewBag.EventId = dto.EventId;

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Please login and try again.";
                    return RedirectToAction("Index");
                }

                // Let service handle validation and creation
                var result = await _service.AddContainerAsync(dto, user.UserName);

                if (!result.Success)
                {
                    TempData["ErrorMessage"] = result.Message;
                    return View(dto);
                }


                // Load event containers if needed
                var containers = string.IsNullOrEmpty(dto.EventId)
                    ? new List<Container>()
                    : await _service.GetContainersByEventIdAsync(dto.EventId);

                TempData["ResponseStatus"] = "success";
                TempData["ResponseTitle"] = "Success";
                TempData["ResponseMessage"] = "Container saved successfully!";

                return View("Index", containers);
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(dto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error occurred while adding container.";
                return View(dto);
            }
        }


        [HttpGet("Monitor/{id}")]
        public async Task<IActionResult> Monitor(long id)
        {
            try
            {
                var container = await _service.GetContainerByIdAsync(id);
                if (container == null)
                {
                    TempData["ErrorMessage"] = "Container not found.";
                    return RedirectToAction("Index");
                }

                var readings = await _service.GetReadingsForContainer(container.Id);
                ViewBag.Readings = readings;

                return View(container);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while loading the monitor page.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost("Monitor/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Monitor(long id, CreateReadingDto dto)
        {
            try
            {
                dto.ContainerId = id;
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    TempData["ErrorMessage"] = "Please login and try again.";
                    return RedirectToAction("Index");
                }

                var result = await _service.AddReadingAsync(dto, user.UserName);

                if (!result.Success)
                {
                    TempData["ErrorMessage"] = result.Message;
                    return RedirectToAction("Monitor", new { id });
                }

                TempData["SuccessMessage"] = result.Message;
                //return RedirectToAction("Monitor", new { id });
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while saving the reading.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AcknowledgeNotification([FromBody] long notificationId)
        {
            try
            {
                await _service.AcknowledgeNotificationAsync(notificationId);
                return Ok();
            }
            catch (Exception ex)
            {
                // Optionally log the exception here as well
                return StatusCode(500, new { message = ex.Message });
            }
        }


    }
}