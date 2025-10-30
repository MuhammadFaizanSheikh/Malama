using Azure;
using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using Malama.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExcelFilesCompiler.Controllers
{
    //[Authorize(Roles = "DAWSON Admin - Event Staff,Project Manager & Program Manager,Super Admin")]
    public class ImmunizationStationController : Controller
    {
        private readonly IFileUploader _fileUploader;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImmunizationStationService _immunizationStationService;
        private readonly ILogger<ImmunizationStationController> _logger;

        public ImmunizationStationController(ILogger<ImmunizationStationController> logger, IFileUploader fileUploader, IConfiguration configuration, UserManager<ApplicationUser> userManager, IImmunizationStationService immunizationStationService)
        {
            _logger = logger;
            _fileUploader = fileUploader;
            _configuration = configuration;
            _userManager = userManager;
            _immunizationStationService = immunizationStationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
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

                var data = _fileUploader.GetEventDataByEventIdForImmunization(eventId).ToList();

                var summary = new Dictionary<string, int>
                {
                    ["Total"] = data.Count(),
                    ["Pending"] = data.Count(x => x.ImmunizationRecord == null || x.ImmunizationRecord?.Status == "Pending"),
                    ["Completed"] = data.Count(x => x.ImmunizationRecord?.Status == "Completed"),
                    ["NotGiven"] = data.Count(x => x.ImmunizationRecord?.Status == "Not given")
                };

                ViewBag.Summary = summary;
                ViewBag.EventId = eventId;
                return View("Index", data);
            }
            catch (Exception ex)
            {
                ViewBag.EventIdList = new List<SelectListItem>();
                ViewBag.ErrorMessage = "Failed to load Event IDs: " + ex.Message;
                return View();
            }
        }

        //[HttpGet]
        //public async Task<IActionResult> GetEventData(string eventId)
        //{
        //    try
        //    {
        //        var data = _fileUploader.GetEventDataByEventIdForImmunization(eventId);

        //        var summary = new Dictionary<string, int>
        //        {
        //            ["Total"] = data.Count(),
        //            ["Pending"] = data.Count(x => x.ImmunizationRecord == null || x.ImmunizationRecord.Status == "Pending"),
        //            ["Completed"] = data.Count(x => x.ImmunizationRecord.Status == "Completed"),
        //            ["NotGiven"] = data.Count(x => x.ImmunizationRecord.Status == "Not given")
        //        };

        //        ViewBag.Summary = summary;
        //        ViewBag.EventId = eventId;
        //        return View("Index",data);
        //    }
        //    catch (Exception ex)
        //    {
        //        var error = new { success = false, message = "Error fetching preview data.", error = ex.Message };

        //        var options = new JsonSerializerOptions
        //        {
        //            PropertyNamingPolicy = null,
        //            DictionaryKeyPolicy = null
        //        };

        //        var json = JsonSerializer.Serialize(error, options);

        //        return Content(json, "application/json");
        //    }
        //}

        public async Task<IActionResult> ImmunizationStation(long immunizationId, long fileDataId)
        {
            try
            {
                ImmunizationStation model;

                if (immunizationId > 0)
                {
                    // Edit mode → get child record including parent
                    model = await _immunizationStationService.GetByIdWithParentAsync(immunizationId);
                }
                else
                {
                    // Add mode → create empty child but attach parent
                    var parent = await _fileUploader.GetByIdAsync(fileDataId);
                    model = new ImmunizationStation
                    {
                        FileDataId = fileDataId,
                        FileData = parent
                    };
                }

                string eventId = model.FileData?.EventId;
                ViewBag.EventId = eventId;

                var immunizationData = await _immunizationStationService.GetImmunizationManufacturer(eventId);

                if (immunizationData.Success && immunizationData.Data != null)
                {
                    ViewBag.ImmunizationData = immunizationData.Data; // 👈 send to Razor
                }
                else
                {
                    ViewBag.ImmunizationData = new List<object>(); // empty list to avoid null
                }

                return View(model);
            }
            catch (Exception ex)
            {
                // log if needed
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveImmunization(ImmunizationStation model, string eventIdForRedirection)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var allErrors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                    // Combine into a single string (or take the first message)
                    var message = string.Join(" | ", allErrors);

                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Data";
                    TempData["ResponseMessage"] = message;

                    model = await _immunizationStationService.GetByIdWithParentAsync(model.Id);
                    string eventId = model.FileData?.EventId;
                    ViewBag.EventId = eventId;

                    var immunizationData = await _immunizationStationService.GetImmunizationManufacturer(eventId);

                    if (immunizationData.Success && immunizationData.Data != null)
                    {
                        ViewBag.ImmunizationData = immunizationData.Data; // 👈 send to Razor
                    }
                    else
                    {
                        ViewBag.ImmunizationData = new List<object>(); // empty list to avoid null
                    }

                    return View("ImmunizationStation", model);
                }

                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    TempData["ErrorMessage"] = "Please login and try again.";
                    return RedirectToAction("Index");
                }

                if (model.Id == 0)
                {
                    await _immunizationStationService.AddAsync(model, user.UserName);
                }
                else
                {
                    await _immunizationStationService.UpdateAsync(model, user.UserName);
                }

                //return RedirectToAction("Index");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error saving immunization");
                throw;
            }
        }






        //[HttpGet]
        //public async Task<IActionResult> Index()
        //{
        //    try
        //    {
        //        var eventIds = await _fileUploader.GetDistinctEventIdsAsync();

        //        var dropdownList = eventIds.Select(e => new SelectListItem
        //        {
        //            Value = e,
        //            Text = e
        //        }).ToList();

        //        ViewBag.EventIdList = dropdownList;
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.EventIdList = new List<SelectListItem>();
        //        ViewBag.ErrorMessage = "Failed to load Event IDs: " + ex.Message;
        //        return View();
        //    }
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetById(int id)
        //{
        //    try
        //    {
        //        var record = await _immunizationStationService.GetByIdAsync(id);

        //        if (record == null)
        //            return NotFound(new { Message = "Record not found" });

        //        return Ok(record);
        //    }
        //    catch (Exception ex)
        //    {
        //        // You can also log exception here
        //        return StatusCode(500, new { Message = "An error occurred while fetching the record.", Error = ex.Message });
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> SaveRecord([FromBody] ImmunizationStation model)
        //{
        //    if (model.FileDataId <= 0)
        //    {
        //        return BadRequest(new ResponseDto
        //        {
        //            Success = false,
        //            Message = "Invalid request data.",
        //            Data = ModelState
        //        });
        //    }

        //    try
        //    {
        //        var responseDto = await _immunizationStationService.SaveRecordAsync(model);

        //        // Always return 200, let frontend check Success flag
        //        return Ok(responseDto);
        //    }
        //    catch (Exception ex)
        //    {
        //        // optional: log ex here
        //        return StatusCode(500, new ResponseDto
        //        {
        //            Success = false,
        //            Message = "An unexpected error occurred while saving the record.",
        //            Data = null
        //        });
        //    }
        //}




    }

    public class ImmunizationSummary
    {
        public int Total { get; set; }
        public int Pending { get; set; }
        public int Completed { get; set; }
        public int NotGiven { get; set; }
    }

}