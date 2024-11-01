using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Models;
using ExcelFilesCompiler.Repositories.Services;
using ExcelToCsv.Models;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.Streaming.Values;
using NPOI.XSSF.UserModel;
using SixLabors.ImageSharp.ColorSpaces;
using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;


namespace ExcelFilesCompiler.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFileUploader fileUploader;
        private readonly UserManager<IdentityUser> _userManager;
        public HomeController(ILogger<HomeController> logger, IFileUploader _iFileUploader, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
            this.fileUploader = _iFileUploader;
        }

        public IActionResult Index()
        {
            //string sessionId = HttpContext.Session.GetString("SessionID");
            return View();
        }

        public IActionResult UploadAndPreview(List<IFormFile> files, string eventDate, string lastEventDate, string eventId, int lastDentalExam , int vision, int dental, int pha, int hiv, int hearing)
        {
            try
            {
                if (files == null || files.Count == 0)
                {
                    return BadRequest("No files uploaded.");
                }

                DateTime parsedEventDate;

                if (!DateTime.TryParse(eventDate, out parsedEventDate))
                {
                    return BadRequest("Invalid event date format.");
                }

                var G6PDFile = files.FirstOrDefault(f => f.FileName.StartsWith("G6PDReport"));

                if (G6PDFile == null)
                {
                    return BadRequest("G6PDReport file is missing.");
                }

                DateTime? parsedLastEventDate = null;
                if (DateTime.TryParse(lastEventDate, out DateTime parsedLastEventDateTmp))
                {
                    parsedLastEventDate = parsedLastEventDateTmp;
                }

                var jsonResult = fileUploader.UploadAndPreview(files, G6PDFile, parsedEventDate, parsedLastEventDate, eventId, lastDentalExam, vision, dental, pha, hiv, hearing);
                return Json(jsonResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult CheckForExistingDataAgainstEventId([FromBody] string eventId)
        {
            try
            {
                if (eventId != null && !string.IsNullOrEmpty(eventId))
                {
                    var existingData = fileUploader.CheckForExistingDataAgainstEventId(eventId);
                    return Json(new { success = existingData.Success, message = existingData.Message });
                }
                return BadRequest("No data to check!");
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return Json(new { success = false, message = Messages.ErrorOcurred, error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult SubmitDataInDatabase([FromBody] SubmitDataDto request)
        {
            try
            {
                if (request != null && !string.IsNullOrEmpty(request.EventId) &&  request.Entities != null && request.Entities.Count > 0)
                {
                    var user = _userManager.GetUserAsync(User).Result;
                    var result = fileUploader.AddRecordsBulk(request.Entities, request.EventId, user.UserName);
                    
                    if (!result.Success)
                    {
                        return BadRequest(result.Message);
                    }
                    return Json(new { success = result.Success, message = result.Message });
                }

                return BadRequest("No data to insert!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }

    public class CheckEventIdDto
    {
        public string EventId { get; set; }
    }
}
