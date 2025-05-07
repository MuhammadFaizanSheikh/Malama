using ExcelFilesCompiler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Text.Json;

namespace ExcelFilesCompiler.Controllers
{
    public class ExcelFileUploaderController : Controller
    {
        private readonly IConfiguration _configuration;

        public ExcelFileUploaderController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult ExportToExcel([FromQuery] string filename, [FromBody] JsonElement data)
        {
            try
            {
                if (string.IsNullOrEmpty(filename))
                    return BadRequest("Filename is required.");

                var directoryPath = _configuration["ExportSettings:ExcelOutputDirectory"];
                if (string.IsNullOrEmpty(directoryPath))
                    return StatusCode(500, "Export directory path is not configured.");

                var fullPath = Path.Combine(directoryPath, Path.GetFileName(filename)); // sanitize filename

                if (data.ValueKind != JsonValueKind.Array || data.GetArrayLength() == 0)
                    return BadRequest("Invalid or empty data.");

                var records = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(data.GetRawText());

                if (records == null || records.Count == 0)
                    return BadRequest("No records to export.");

                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Data");

                // Header row
                var headers = records.First().Keys.ToList();
                IRow headerRow = sheet.CreateRow(0);
                for (int i = 0; i < headers.Count; i++)
                {
                    headerRow.CreateCell(i).SetCellValue(headers[i]);
                }

                // Data rows
                for (int i = 0; i < records.Count; i++)
                {
                    IRow row = sheet.CreateRow(i + 1);
                    for (int j = 0; j < headers.Count; j++)
                    {
                        row.CreateCell(j).SetCellValue(records[i][headers[j]] ?? string.Empty);
                    }
                }

                // Ensure directory exists
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                // Save to file
                using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                workbook.Write(fileStream);

                return Ok("Excel file saved successfully.");
            }
            catch (Exception ex)
            {
                // Optional: log error to file or monitoring system
                return StatusCode(500, $"An error occurred while exporting to Excel: {ex.Message}");
            }
        }



        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}