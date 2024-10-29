using ExcelToCsv.Models;

namespace ExcelFilesCompiler.Models
{
    public class SubmitDataDto
    {
        public List<FileDataDto> Entities { get; set; } = new List<FileDataDto>();
        public string EventId { get; set; } = string.Empty;
    }
}
