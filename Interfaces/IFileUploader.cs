using ExcelFilesCompiler.Models;
using ExcelToCsv.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IFileUploader
    {
        List<List<Dictionary<string, object>>> UploadAndPreview(List<IFormFile> files, IFormFile G6PDFile, DateTime parsedEventDate, DateTime? parsedLastEventDate, string eventId, int lastDentalExam, int vision, int dental, int pha, int hiv, int hearing);
        ResponseDto CheckForExistingDataAgainstEventId(string eventId);
        ResponseDto AddRecordsBulk(IEnumerable<FileDataDto> data, string eventId, string loggedinUserName);
    }
}
