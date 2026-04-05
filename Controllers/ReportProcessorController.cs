using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using Malama.Attributes;
using Malama.Models;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.Streaming.Values;
using NPOI.XSSF.UserModel;
using SixLabors.ImageSharp.ColorSpaces;
using System;
using System.Data;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;


namespace ExcelFilesCompiler.Controllers
{
    public class ReportProcessorController : Controller
    {
        private readonly ILogger<ReportProcessorController> _logger;
        private readonly IFileUploader fileUploader;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEventManagementService _eventManagementService;
        private const string CLASSNAME = "ReportProcessorController";
        public ReportProcessorController(ILogger<ReportProcessorController> logger, IEventManagementService eventManagementService, IFileUploader _iFileUploader, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
            this.fileUploader = _iFileUploader;
            _eventManagementService = eventManagementService;
        }

        [RoleAttributeAuthorizeFromConfig("ReportProcessor_View")]
        public async Task<IActionResult> Index()
        {
            var events = await _eventManagementService.GetAllEventID(false);

            ViewBag.EventList = events
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.EventID
                })
                .ToList();

            return View();
        }


        [RoleAttributeAuthorizeFromConfig("ReportProcessor_View")]
        public IActionResult UploadAndPreview(List<IFormFile> files, string eventDate, string lastEventDate, long eventId, int lastDentalExam , int vision, int dental, int pha, int hiv, int hearing)
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
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [RoleAttributeAuthorizeFromConfig("ReportProcessor_Save", "ScrubbedSheetUploader_Save")]
        public async Task<IActionResult> CheckForExistingDataAgainstEventId([FromBody] string eventId)
        {
            const string methodName = nameof(SubmitDataInDatabase);

            try
            {
                if (eventId != null && !string.IsNullOrEmpty(eventId))
                {
                    var user = await _userManager.GetUserAsync(User);

                    if (user == null)
                    {
                        _logger.LogWarning("{ClassName}, {MethodName}, User not logged in.", CLASSNAME, methodName);
                        return StatusCode(401, new ResponseDto
                        {
                            Success = false,
                            Message = "Please login and try again.",
                            Data = null
                        });
                    }

                    var existingData = await fileUploader.CheckForExistingDataAgainstEventIdAsync(eventId, user.UserName);
                    return Json(new { success = existingData.Success, message = existingData.Message, code = existingData.Code});
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
        [RoleAttributeAuthorizeFromConfig("ReportProcessor_Save", "ScrubbedSheetUploader_Save")]
        public async Task<IActionResult> SubmitDataInDatabase([FromBody] SubmitDataDto request)
        {
            const string methodName = nameof(SubmitDataInDatabase);
            _logger.LogInformation("{ClassName}, {MethodName}, Called with EventID: {EventID}, EntityCount: {Count}",
                CLASSNAME, methodName, request?.EventId, request?.Entities?.Count ?? 0);

            try
            {
                if (request == null || string.IsNullOrEmpty(request.EventId) || request.Entities == null || request.Entities.Count == 0)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Invalid request or no data to insert.",
                        CLASSNAME, methodName);

                    return BadRequest(new ResponseDto
                    {
                        Success = false,
                        Message = "No data to insert!",
                        Data = null
                    });
                }

                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, User not logged in.", CLASSNAME, methodName);
                    return StatusCode(401, new ResponseDto
                    {
                        Success = false,
                        Message = "Please login and try again.",
                        Data = null
                    });
                }

                _logger.LogInformation("{ClassName}, {MethodName}, Inserting {Count} records for EventID: {EventID} by User: {UserName}",
                    CLASSNAME, methodName, request.Entities.Count, request.EventId, user.UserName);

                var result = await fileUploader.AddRecordsBulkAsync(request.Entities, request.EventId, user.UserName);

                if (!result.Success)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Failed to insert records for EventID: {EventID}, Message: {Message}",
                        CLASSNAME, methodName, request.EventId, result.Message);

                    return BadRequest(result);
                }

                _logger.LogInformation("{ClassName}, {MethodName}, Successfully inserted {Count} records for EventID: {EventID} (New ParentId: {ParentId})",
                    CLASSNAME, methodName, request.Entities.Count, request.EventId, result.Data);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while processing EventID: {EventID}",
                    CLASSNAME, methodName, request?.EventId);

                return StatusCode(500, new ResponseDto
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Data = null
                });
            }
        }



    }
}
