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
            try
            {
                var eventIds = await _fileUploader.GetDistinctEventIdsAsync();

                var dropdownList = eventIds.Select(e => new SelectListItem
                {
                    Value = e,
                    Text = e
                }).ToList();

                ViewBag.EventIdList = dropdownList;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.EventIdList = new List<SelectListItem>();
                ViewBag.ErrorMessage = "Failed to load Event IDs: " + ex.Message;
                return View();
            }
        }

        [HttpGet("Index")]
        public async Task<IActionResult> GetContainersAgainstEventId(string eventId)
        {
            List<Container> containers = new List<Container>();

            try
            {
                //ViewBag.ContainerTypes = await _service.GetAllContainerTypesAsync();

                if (!string.IsNullOrEmpty(eventId))
                {
                    containers = await _service.GetContainersByEventIdAsync(eventId);
                }

                // Keep selected event ID for maintaining state in view
                ViewBag.SelectedEventId = eventId;

                return View("Index", containers);
            }
            catch (ArgumentException argEx)
            {
                // Handles invalid argument, e.g., null or empty eventId
                _logger.LogWarning(argEx, "Invalid EventId provided: {EventId}", eventId);
                TempData["ErrorMessage"] = "Invalid Event selected. Please try again.";
                return View("Index", containers);
            }
            catch (ApplicationException appEx)
            {
                // Handles exceptions thrown by service layer
                _logger.LogError(appEx, "Error fetching data for EventId: {EventId}", eventId);
                TempData["ErrorMessage"] = "Unable to fetch vaccine records at this time.";
                return View("Index", containers);
            }
            catch (Exception ex)
            {
                // Handles unexpected errors
                _logger.LogError(ex, "Unexpected error in GetEventData for EventId: {EventId}", eventId);
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";
                return View("Index", containers);
            }
        }

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
                    StartDate = DateTime.Now.Date,
                    StartTime = DateTime.Now.TimeOfDay
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
                // Repopulate dropdowns for view reload (in case validation fails)
                ViewBag.ContainerTypes = await _service.GetAllContainerTypesAsync();

                // 1️⃣ Validate Container Type
                var containerType = (await _service.GetAllContainerTypesAsync())
                    .FirstOrDefault(ct => ct.Id == dto.ContainerTypeId);

                if (containerType == null)
                {
                    ModelState.AddModelError(nameof(dto.ContainerTypeId), "Invalid container type selected.");
                }
                else
                {
                    // 2️⃣ Validate temperature range & comment requirement
                    bool isOutOfRange = dto.InitialTemperature < containerType.TemperatureFromRange ||
                                        dto.InitialTemperature > containerType.TemperatureToRange;

                    if (isOutOfRange && string.IsNullOrWhiteSpace(dto.Comment))
                    {
                        ModelState.AddModelError(nameof(dto.Comment), "Comment is required when temperature is out of range.");
                    }
                }

                // 3️⃣ If validation fails, return same view with model
                if (!ModelState.IsValid)
                {
                    // Combine all validation messages into one string
                    var allErrors = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));

                    TempData["ErrorMessage"] = allErrors;

                    // Refill dropdowns etc. if needed
                    ViewBag.ContainerTypes = await _service.GetAllContainerTypesAsync();
                    return View(dto);
                }


                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    TempData["ErrorMessage"] = "Please login and try again.";
                    return RedirectToAction("Index");
                }

                await _service.AddContainerAsync(dto, user.UserName);


                List<Container> containers = new();
                if (!string.IsNullOrEmpty(dto.EventId))
                {
                    containers = await _service.GetContainersByEventIdAsync(dto.EventId);
                }

                ViewBag.SelectedEventId = dto.EventId;
                TempData["ResponseStatus"] = "success";
                TempData["ResponseTitle"] = "Success";
                TempData["ResponseMessage"] = "Container saved successfully!";
                return View("Index", containers);

                //return RedirectToAction("Index", new { eventId = dto.EventId });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, $"Validation error: {ex.Message}");
                return View(dto);
            }
            catch (Exception ex)
            {
                // You can log this exception if you have logging enabled
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while adding the container.");
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

                // Load previous readings for display
                var readings = await _service.GetReadingsForContainer(container.Id);
                ViewBag.Readings = readings;

                return View(container);
            }
            catch (Exception ex)
            {
                // Log exception here if you have a logger, e.g. _logger.LogError(ex, "Error loading container monitor page");
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
                //var container = await _service.GetContainerByIdAsync(id);
                //if (container == null)
                //{
                //    TempData["ErrorMessage"] = "Container not found.";
                //    return RedirectToAction("Index");
                //}

                //var containerType = container.ContainerType ?? (await _service.GetContainerByIdAsync(id))?.ContainerType;
                //if (containerType == null)
                //{
                //    ModelState.AddModelError("", "Container type not found.");
                //}

                //if (containerType != null)
                //{
                //    bool isOutOfRange = dto.Temperature < containerType.TemperatureFromRange ||
                //                        dto.Temperature > containerType.TemperatureToRange;

                //    if (isOutOfRange && string.IsNullOrWhiteSpace(dto.Comment))
                //    {
                //        TempData["ErrorMessage"] = "Comment is required when reading is abnormal.";
                //    }
                //}

                //if (!ModelState.IsValid)
                //{
                //    ViewBag.Readings = await _service.GetReadingsForContainer(id);
                //    return View(container);
                //}

                dto.ContainerId = id;
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    TempData["ErrorMessage"] = "Please login and try again.";
                    return RedirectToAction("Index");
                }
                await _service.AddReadingAsync(dto, user.UserName);

                return RedirectToAction("Monitor", await _service.GetContainerByIdAsync(id));
            }
            catch (ArgumentException ex)
            {
                // specific known validation-level exception
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the error (example: _logger.LogError(ex, "Error adding reading for container {id}", id));
                TempData["ErrorMessage"] = "An unexpected error occurred while saving the reading.";
                return RedirectToAction("Index");
            }
        }

    }
}