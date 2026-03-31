using ExcelFilesCompiler.Interfaces;
using Malama.Attributes;
using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExcelFilesCompiler.Controllers
{
    //[Authorize(Roles = "DAWSON Admin - Event Staff,Project Manager & Program Manager,Super Admin")]
    public class ExcelFileUploaderController : Controller
    {
        private readonly IFileUploader _fileUploader;
        private readonly IEventManagementService _eventManagementService;
        private readonly IPdfGeneratorService _pdfGenerator;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ExcelFileUploaderController> _logger;
        private const string CLASSNAME = "ExcelFileUploaderController";

        public ExcelFileUploaderController(IFileUploader fileUploader, ILogger<ExcelFileUploaderController> logger, IEventManagementService eventManagementService, IPdfGeneratorService pdfGenerator, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _fileUploader = fileUploader;
            _eventManagementService = eventManagementService;
            _configuration = configuration;
            _userManager = userManager;
            _pdfGenerator = pdfGenerator;
            _logger = logger;
        }

        [CheckInOutAuthorize]
        [HttpGet]
        public async Task<IActionResult> Index(string userType)
        {
            try
            {
                ViewBag.UserType = userType;
                return View();
            }
            catch (Exception ex)
            {
                //ViewBag.EventIdList = new List<SelectListItem>();
                ViewBag.ErrorMessage = "Failed to load Event IDs: " + ex.Message;
                return View();
            }
        }

        [RoleAttributeAuthorizeFromConfig("CheckInOutStaff_View")]
        [HttpPost]
        public async Task<IActionResult> GetServiceMembersByEventId()
        {
            try
            {
                string eventId = HttpContext.Session.GetString("GlobalEventIdLong");

                if (string.IsNullOrEmpty(eventId))
                {
                    _logger.LogWarning("GetEventDataByEventId: EventId is missing from session");

                    return Json(new { success = false, message = "No EventId selected." });
                }

                if (!int.TryParse(eventId, out int parsedEventId))
                {
                    _logger.LogWarning("GetEventDataByEventId: Invalid EventId: {eventId}", eventId);

                    return Json(new { success = false, message = "Invalid EventVersion format." });
                }

                _logger.LogInformation("Fetching service members for EventId: {EventId}, Version: {Version}", eventId, parsedEventId);

                // ✅ IMPORTANT FIX: await was missing
                var data = await _eventManagementService
                    .GetServiceMembersByEventAsync(parsedEventId);

                if (data == null || !data.Any())
                {
                    _logger.LogInformation("No data found for EventId: {EventId}", parsedEventId);

                    return new JsonResult(new { success = true, data = new List<FileDataDto>() }, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = null // preserve C# property names (PascalCase)
                    });
                }

                // Return the actual data, also preserving PascalCase
                return new JsonResult(new { success = true, data }, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null // preserve C# property names (PascalCase)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetEventDataByEventId");

                return Json(new
                {
                    success = false,
                    message = "Error fetching preview data."
                });
            }
        }

        [RoleAttributeAuthorizeFromConfig("ImmunizationStation_View")]
        [HttpPost]
        public async Task<IActionResult> GetEventDataByEventIdForImmunization([FromBody] long eventId)
        {
            try
            {
                if (eventId <= 0)
                {
                    return BadRequest("Event ID is required.");
                }

                var data = _fileUploader.GetImmunizationsByEventIdAsync(eventId);
                var result = new { success = true, data };

                // 👇 Use custom JsonSerializerOptions with null naming policy (i.e., preserve PascalCase)
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null,
                    DictionaryKeyPolicy = null,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };

                var json = JsonSerializer.Serialize(result, options);

                return Content(json, "application/json");
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

        [RoleAttributeAuthorizeFromConfig("Profile_View")]
        public async Task<IActionResult> GetDataAgainstIdAndGeneratePdf(long id)
        {
            try
            {
                // 1. Get parent + child table data
                var eventDto = await _fileUploader.GetByIdWithInclude(id);
                if (eventDto == null)
                    return NotFound("Event not found.");

                // 2. Generate PDF
                var pdfBytes = await _pdfGenerator.GenerateEventSummaryPdfAsync(eventDto);

                var fileName = $"EventSummary_{id}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                // Log error here
                return StatusCode(500, $"Error generating PDF: {ex.Message}");
            }
        }

        [RoleAttributeAuthorizeFromConfig("CheckInOutStaff_Save")]
        [HttpPost]
        public async Task<IActionResult> InsertSingleRecord([FromBody] FileDataDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto
                {
                    Success = false,
                    Message = "Invalid model state.",
                    Data = ModelState
                });
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return StatusCode(401, new ResponseDto
                    {
                        Success = false,
                        Message = "Please login and try again.",
                    });
                }

                string eventId = HttpContext.Session.GetString("GlobalEventIdLong");

                if (string.IsNullOrEmpty(eventId))
                {
                    _logger.LogWarning("GetEventDataByEventId: EventId is missing from session");

                    return Json(new { success = false, message = "No EventID selected." });
                }

                if (!int.TryParse(eventId, out int parsedEventId))
                {
                    _logger.LogWarning("GetEventDataByEventId: Invalid EventVersion: {EventId}", eventId);

                    return Json(new { success = false, message = "Invalid EventVersion format." });
                }

                _logger.LogInformation("Fetching service members for EventId: {EventId}, ", eventId);

                var response = await _fileUploader.AddSingleRecordAsync(dto, parsedEventId, user.UserName);

                if (response.Success)
                    return Ok(response);
                else
                    return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDto
                {
                    Success = false,
                    Message = "An unexpected error occurred while inserting the record.",
                    Data = ex.Message
                });
            }
        }

        [RoleAttributeAuthorizeFromConfig("CheckInOutStaff_Save")]
        [HttpPut]
        public async Task<IActionResult> UpdateSingleRecord([FromBody] FileDataDto dto)
        {
            const string methodName = nameof(UpdateSingleRecord);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("{ClassName}, {MethodName}, Invalid model state: {@ModelState}",
                    CLASSNAME, methodName, ModelState);

                return BadRequest(new ResponseDto
                {
                    Success = false,
                    Message = "Invalid model state.",
                    Data = ModelState
                });
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, User not authenticated", CLASSNAME, methodName);

                    return StatusCode(401, new ResponseDto
                    {
                        Success = false,
                        Message = "Please login and try again.",
                    });
                }

                _logger.LogInformation("{ClassName}, {MethodName}, Updating record Id: {Id} by User: {UserName}",
                    CLASSNAME, methodName, dto.Id, user.UserName);

                var response = await _fileUploader.UpdateSingleRecordAsync(dto, user.UserName);

                if (response.Success)
                {
                    _logger.LogInformation("{ClassName}, {MethodName}, Successfully updated record Id: {Id}",
                        CLASSNAME, methodName, dto.Id);

                    return Ok(response);
                }
                else
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Update failed for record Id: {Id}",
                        CLASSNAME, methodName, dto.Id);

                    return StatusCode(500, response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Unexpected error updating record Id: {Id}",
                    CLASSNAME, methodName, dto.Id);

                return StatusCode(500, new ResponseDto
                {
                    Success = false,
                    Message = "An unexpected error occurred while updating the record.",
                    Data = ex.Message
                });
            }
        }
    }
}