using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using Malama.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace ExcelFilesCompiler.Controllers
{
    //[Authorize(Roles = "DAWSON Admin - Event Staff,Project Manager & Program Manager,Super Admin")]
    public class ImmunizationVaccineInfoController : Controller
    {
        private readonly IFileUploader _fileUploader;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImmunizationVaccineInfoService _immunizationVaccineInfoService;
        private readonly ILogger<ImmunizationStationController> _logger;
         

        public ImmunizationVaccineInfoController(ILogger<ImmunizationStationController> logger, IFileUploader fileUploader, IConfiguration configuration, UserManager<ApplicationUser> userManager, IImmunizationVaccineInfoService immunizationVaccineInfoService)
        {
            _logger = logger;
            _fileUploader = fileUploader;
            _configuration = configuration;
            _userManager = userManager;
            _immunizationVaccineInfoService = immunizationVaccineInfoService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new ImmunizationVaccineViewModel();

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
                model.EventId = eventId;
                model.ListOfImmunizationVaccineInfo = await _immunizationVaccineInfoService
                    .GetVaccineEntriesByEventIdAsync(eventId);

                return View("Index", model);
            }
            catch (Exception ex)
            {
                ViewBag.EventIdList = new List<SelectListItem>();
                ViewBag.ErrorMessage = "Failed to load Event IDs: " + ex.Message;
                return View("Index", model);
            }
        }

        

        //[HttpGet]
        //public async Task<IActionResult> GetVaccineEntriesByEventId(string eventId)
        //{
        //    var model = new ImmunizationVaccineViewModel();

        //    try
        //    {
        //        if (!string.IsNullOrEmpty(eventId))
        //        {
        //            model.EventId = eventId;
        //            model.ListOfImmunizationVaccineInfo = await _immunizationVaccineInfoService
        //                .GetVaccineEntriesByEventIdAsync(eventId);
        //        }

        //        return View("Index", model);
        //    }
        //    catch (ArgumentException argEx)
        //    {
        //        // Handles invalid argument, e.g., null or empty eventId
        //        _logger.LogWarning(argEx, "Invalid EventId provided: {EventId}", eventId);
        //        TempData["ErrorMessage"] = "Invalid Event selected. Please try again.";
        //        return View("Index", model);
        //    }
        //    catch (ApplicationException appEx)
        //    {
        //        // Handles exceptions thrown by service layer
        //        _logger.LogError(appEx, "Error fetching data for EventId: {EventId}", eventId);
        //        TempData["ErrorMessage"] = "Unable to fetch vaccine records at this time.";
        //        return View("Index", model);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handles unexpected errors
        //        _logger.LogError(ex, "Unexpected error in GetEventData for EventId: {EventId}", eventId);
        //        TempData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";
        //        return View("Index", model);
        //    }
        //}

        public async Task<IActionResult> AddNewVaccine(string eventId)
        {
            try
            {
                var model = new ImmunizationVaccineViewModel
                {
                    SingleImmunizationVaccineInfo = new ImmunizationVaccineInfo
                    {
                        EventId = eventId,
                        Lots = new List<ImmunizationVaccineLotEntry>()
                    }
                };

                // ✅ Get Containers for this Event
                var containerResponse = await _immunizationVaccineInfoService.GetContainersByEventIdAsync(eventId);

                if (containerResponse.Success && containerResponse.Data is List<Container> containers)
                {
                    ViewBag.ContainerList = containers.Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.ContainerName
                    }).ToList();
                }
                else
                {
                    ViewBag.ContainerList = new List<SelectListItem>();
                }

                model.EventId = eventId;
                return View("Index", model);
            }
            catch (Exception ex)
            {
                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = "An unexpected error occurred while loading containers.";

                return RedirectToAction("Index");
            }
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveVaccineEntry(ImmunizationVaccineViewModel model, string action)
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
                return View("Index", model);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Error";
                    TempData["ResponseMessage"] = "Please login and try again.";
                    return RedirectToAction("Index");
                }

                ResponseDto res;

                // ✅ Add or Update
                if (model.SingleImmunizationVaccineInfo.Id == 0)
                {
                    res = await _immunizationVaccineInfoService.AddInventoryAsync(
                        model.SingleImmunizationVaccineInfo, user.UserName
                    );
                }
                else
                {
                    res = await _immunizationVaccineInfoService.UpdateInventoryAsync(
                        model.SingleImmunizationVaccineInfo, user.UserName
                    );
                }

                // ✅ Handle response
                if (res.Success)
                {
                    TempData["ResponseStatus"] = "success";
                    TempData["ResponseTitle"] = "Success";
                    TempData["ResponseMessage"] = res.Message;
                }
                else
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Error";
                    TempData["ResponseMessage"] = res.Message;
                }

                // ✅ Redirect back to the table (event-based)
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = $"An unexpected error occurred: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetImmunizationVaccineInfoById(long immunizationId)
        {
            try
            {
                if (immunizationId <= 0)
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Invalid Request";
                    TempData["ResponseMessage"] = "Invalid parameters provided.";
                    return RedirectToAction("Index");
                }

                var response = await _immunizationVaccineInfoService.GetImmunizationVaccineInfoByIdAsync(immunizationId);

                if (!response.Success || response.Data == null)
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Not Found";
                    TempData["ResponseMessage"] = response.Message ?? "Record not found.";
                    return RedirectToAction("Index");
                }

                var vaccineInfo = response.Data as ImmunizationVaccineInfo;
                if (vaccineInfo == null)
                {
                    TempData["ResponseStatus"] = "error";
                    TempData["ResponseTitle"] = "Error";
                    TempData["ResponseMessage"] = "Invalid data type received from service.";
                    return RedirectToAction("Index");
                }

                var model = new ImmunizationVaccineViewModel
                {
                    SingleImmunizationVaccineInfo = vaccineInfo,
                    EventId = vaccineInfo.EventId
                };

                var containerResponse = await _immunizationVaccineInfoService.GetContainersByEventIdAsync(model.SingleImmunizationVaccineInfo.EventId);

                if (containerResponse.Success && containerResponse.Data is List<Container> containers)
                {
                    ViewBag.ContainerList = containers.Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.ContainerName
                    }).ToList();
                }
                else
                {
                    ViewBag.ContainerList = new List<SelectListItem>();
                }

                return View("Index",model);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error fetching ImmunizationVaccineInfo (Id: {ImmunizationId})", immunizationId);

                TempData["ResponseStatus"] = "error";
                TempData["ResponseTitle"] = "Error";
                TempData["ResponseMessage"] = "An unexpected error occurred while fetching the record.";
                return RedirectToAction("Index");
            }
        }









    }
}