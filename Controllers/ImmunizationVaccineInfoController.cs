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
    public class ImmunizationVaccineInfoController : Controller
    {
        private readonly IFileUploader _fileUploader;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImmunizationStationService _immunizationStationService;
        private readonly ILogger<ImmunizationStationController> _logger;

        public ImmunizationVaccineInfoController(ILogger<ImmunizationStationController> logger, IFileUploader fileUploader, IConfiguration configuration, UserManager<ApplicationUser> userManager, IImmunizationStationService immunizationStationService)
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

        [HttpGet]
        public async Task<IActionResult> GetImmunizationVaccineDateByEventId(string eventId)
        {
            try
            {
                //if (string.IsNullOrEmpty(eventId))
                //    return BadRequest("Event ID is required.");

                var data = _fileUploader.GetEventDataByEventIdForImmunization(eventId);

                var summary = new Dictionary<string, int>
                {
                    ["Total"] = data.Count(),
                    ["Pending"] = data.Count(x => x.ImmunizationRecord == null || x.ImmunizationRecord.Status == "Pending"),
                    ["Completed"] = data.Count(x => x.ImmunizationRecord.Status == "Completed"),
                    ["NotGiven"] = data.Count(x => x.ImmunizationRecord.Status == "Not given")
                };

                ViewBag.Summary = summary;

                return View("Index", data);
                //var result = new { success = true, data };

                //// 👇 Use custom JsonSerializerOptions with null naming policy (i.e., preserve PascalCase)
                //var options = new JsonSerializerOptions
                //{
                //    PropertyNamingPolicy = null,
                //    DictionaryKeyPolicy = null,
                //    ReferenceHandler = ReferenceHandler.IgnoreCycles
                //};

                //var json = JsonSerializer.Serialize(result, options);

                //return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                var error = new { success = false, message = "Error fetching preview data.", error = ex.Message };

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null,
                    DictionaryKeyPolicy = null
                };

                var json = JsonSerializer.Serialize(error, options);

                return Content(json, "application/json");
            }
        }

        //public async Task<IActionResult> ImmunizationStation(long immunizationId, long fileDataId)
        //{
        //    try
        //    {
        //        ImmunizationStation model;

        //        if (immunizationId > 0)
        //        {
        //            // Edit mode → get child record including parent
        //            model = await _immunizationStationService.GetByIdWithParentAsync(immunizationId);
        //        }
        //        else
        //        {
        //            // Add mode → create empty child but attach parent
        //            var parent = await _fileUploader.GetByIdAsync(fileDataId);
        //            model = new ImmunizationStation
        //            {
        //                FileDataId = fileDataId,
        //                FileData = parent
        //            };
        //        }

        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        // log if needed
        //        throw;
        //    }
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> SaveImmunization(ImmunizationStation model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return View(model);
        //        }

        //        if (model.Id == 0)
        //        {
        //            await _immunizationStationService.AddAsync(model);
        //        }
        //        else
        //        {
        //            await _immunizationStationService.UpdateAsync(model);
        //        }

        //        return RedirectToAction("GetEventData", new { eventId = model.FileData.EventId });
        //    }
        //    catch (Exception ex)
        //    {
        //        //_logger.LogError(ex, "Error saving immunization");
        //        throw;
        //    }
        //}








    }
}