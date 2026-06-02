using ExcelFilesCompiler.Controllers.Services;
using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IFileUploadDownloadService
    {
        Task<FileUploadResult> UploadFile(IFormFile file, string station, string prefix, string barcode);
        Task<FileUploadResult> UploadImageFile(IFormFile file, string station, string prefix, string barcode, string fileKey);
        FileDownloadResult GetFile(string station, string prefix, string fileName);
        bool DeleteFile(string station, string prefix, string fileName);
        Task DeleteFileAsync(string filePath);
    }
}
