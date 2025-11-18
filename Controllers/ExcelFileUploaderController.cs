using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using Malama.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExcelFilesCompiler.Controllers
{
    //[Authorize(Roles = "DAWSON Admin - Event Staff,Project Manager & Program Manager,Super Admin")]
    public class ExcelFileUploaderController : Controller
    {
        private readonly IFileUploader _fileUploader;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExcelFileUploaderController(IFileUploader fileUploader, IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _fileUploader = fileUploader;
            _configuration = configuration;
            _userManager = userManager;
        }

        //[HttpPost]
        //public IActionResult ExportToExcel([FromQuery] string filename, [FromBody] JsonElement data)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(filename))
        //            return BadRequest("Filename is required.");

        //        var directoryPath = _configuration["ExportSettings:ExcelOutputDirectory"];
        //        if (string.IsNullOrEmpty(directoryPath))
        //            return StatusCode(500, "Export directory path is not configured.");

        //        var fullPath = Path.Combine(directoryPath, Path.GetFileName(filename)); // sanitize filename

        //        if (data.ValueKind != JsonValueKind.Array || data.GetArrayLength() == 0)
        //            return BadRequest("Invalid or empty data.");

        //        //var records = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(data.GetRawText());
        //        var records = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(data.GetRawText());


        //        if (records == null || records.Count == 0)
        //            return BadRequest("No records to export.");

        //        IWorkbook workbook = new XSSFWorkbook();
        //        ISheet sheet = workbook.CreateSheet("Data");

        //        // Header row
        //        var headers = records.First().Keys.ToList();
        //        IRow headerRow = sheet.CreateRow(0);
        //        for (int i = 0; i < headers.Count; i++)
        //        {
        //            headerRow.CreateCell(i).SetCellValue(headers[i]);
        //        }

        //        // Data rows
        //        for (int i = 0; i < records.Count; i++)
        //        {
        //            IRow row = sheet.CreateRow(i + 1);
        //            for (int j = 0; j < headers.Count; j++)
        //            {
        //                var valueElement = records[i][headers[j]];
        //                string cellValue = valueElement.ValueKind switch
        //                {
        //                    JsonValueKind.String => valueElement.GetString(),
        //                    JsonValueKind.Number => valueElement.ToString(),
        //                    JsonValueKind.True => "true",
        //                    JsonValueKind.False => "false",
        //                    JsonValueKind.Null => string.Empty,
        //                    _ => valueElement.ToString()
        //                };

        //                row.CreateCell(j).SetCellValue(cellValue ?? string.Empty);
        //            }
        //        }

        //        // Ensure directory exists
        //        if (!Directory.Exists(directoryPath))
        //            Directory.CreateDirectory(directoryPath);

        //        // Save to file
        //        using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        //        workbook.Write(fileStream);

        //        return Ok("Excel file saved successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Optional: log error to file or monitoring system
        //        return StatusCode(500, $"An error occurred while exporting to Excel: {ex.Message}");
        //    }
        //}



        [HttpGet]
        public async Task<IActionResult> Index(string userType)
        {
            try
            {
                if (userType == "admin")
                {
                    if (!User.IsInRole("Super Admin") && !User.IsInRole("Project Manager & Program Manager"))
                    {
                        return RedirectToAction("AccessDenied", "Account");
                    }
                }
                else if (userType == "client")
                {
                    //if (!User.IsInRole("Super Admin") && !User.IsInRole("Project Manager & Program Manager") && !User.IsInRole("DAWSON Admin - Event Staff"))
                    //{
                    //    return RedirectToAction("AccessDenied", "Account");
                    //}
                }
                else
                {
                    return RedirectToAction("AccessDenied", "Account");
                }


                //var eventIds = await _fileUploader.GetDistinctEventIdsAsync();

                //var dropdownList = eventIds.Select(e => new SelectListItem
                //{
                //    Value = e,
                //    Text = e
                //}).ToList();

                //ViewBag.EventIdList = dropdownList;
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

        [HttpPost]
        public async Task<IActionResult> GetEventDataByEventId()
        {
            try
            {
                string eventId = HttpContext.Session.GetString("GlobalEventId");
                if (string.IsNullOrEmpty(eventId))
                {
                    var noIdResponse = new { success = false, message = "No EventID selected." };
                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = null,
                        DictionaryKeyPolicy = null
                    };
                    var json = JsonSerializer.Serialize(noIdResponse, options);
                    return Content(json, "application/json");
                }

                var data = _fileUploader.GetEventDataByEventId(eventId);

                var result = new { success = true, data };

                var optionsSuccess = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null,
                    DictionaryKeyPolicy = null
                };

                var jsonSuccess = JsonSerializer.Serialize(result, optionsSuccess);
                return Content(jsonSuccess, "application/json");
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


        [HttpPost]
        public async Task<IActionResult> GetEventDataByEventIdForImmunization([FromBody] string eventId)
        {
            try
            {
                if (string.IsNullOrEmpty(eventId))
                    return BadRequest("Event ID is required.");

                var data = _fileUploader.GetEventDataByEventIdForImmunization(eventId);

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

                var response = await _fileUploader.AddSingleRecordAsync(dto, user.UserName);

                if (response.Success)
                    return Ok(response);
                else
                    return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                // Optional: log exception with your logger
                // _logger.LogError(ex, "Error inserting record");

                return StatusCode(500, new ResponseDto
                {
                    Success = false,
                    Message = "An unexpected error occurred while inserting the record.",
                    Data = ex.Message
                });
            }
        }


        [HttpPut]
        public async Task<IActionResult> UpdateSingleRecord([FromBody] FileDataDto dto)
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

                var response = await _fileUploader.UpdateSingleRecordAsync(dto, user.UserName);

                if (response.Success)
                    return Ok(response);
                else
                    return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                // Log the exception here if you have a logger, e.g., _logger.LogError(ex, "Error updating record");

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