using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Utilities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NPOI.XWPF.UserModel;
using System.Xml.Linq;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class FileUploadDownloadService : IFileUploadDownloadService
    {
        private readonly string _baseFolder;
        private readonly ILogger<FileUploadDownloadService> _logger;
        private const string CLASSNAME = "FileUploadDownloadService";

        public FileUploadDownloadService(ILogger<FileUploadDownloadService> logger)
        {
            _logger = logger;

            _baseFolder = Path.Combine(Directory.GetCurrentDirectory(), "Results");

            if (!Directory.Exists(_baseFolder))
            {
                Directory.CreateDirectory(_baseFolder);

                _logger.LogInformation("{Class}.{Method} - Created base directory: {Path}",
                    CLASSNAME, nameof(FileUploadDownloadService), _baseFolder);
            }
        }

        public async Task<FileUploadResult> UploadFile(IFormFile file, string station, string prefix, string barcode)
        {
            const string METHOD = nameof(UploadFile);

            try
            {
                _logger.LogInformation("{Class}.{Method} - Upload started | Station: {Station}, Prefix: {Prefix}, Barcode: {Barcode}",
                    CLASSNAME, METHOD, station, prefix, barcode);

                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("{Class}.{Method} - No file selected",
                        CLASSNAME, METHOD);

                    return new FileUploadResult
                    {
                        Success = false,
                        Message = "No file selected"
                    };
                }

                // 🔹 Build dynamic path
                var stationFolder = Path.Combine(_baseFolder, $"{station}_Results");
                var prefixFolder = Path.Combine(stationFolder, $"{prefix}_Results");

                // 🔹 Create directories if not exist
                if (!Directory.Exists(stationFolder))
                {
                    Directory.CreateDirectory(stationFolder);

                    _logger.LogInformation("{Class}.{Method} - Created station directory: {Path}",
                        CLASSNAME, METHOD, stationFolder);
                }

                if (!Directory.Exists(prefixFolder))
                {
                    Directory.CreateDirectory(prefixFolder);

                    _logger.LogInformation("{Class}.{Method} - Created prefix directory: {Path}",
                        CLASSNAME, METHOD, prefixFolder);
                }

                // 🔹 File name (as per your requirement)
                var fileName = $"{barcode}.pdf";
                var fullPath = Path.Combine(prefixFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation("{Class}.{Method} - File uploaded successfully | FileName: {FileName}",
                    CLASSNAME, METHOD, fileName);

                return new FileUploadResult
                {
                    Success = true,
                    FileName = fileName,
                    RelativePath = Path.Combine("Results", $"{station}_Results", $"{prefix}_Results", fileName)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{Class}.{Method} - Error uploading file | Station: {Station}, Prefix: {Prefix}, Barcode: {Barcode}",
                    CLASSNAME, METHOD, station, prefix, barcode);

                return new FileUploadResult
                {
                    Success = false,
                    Message = "Error occurred while uploading file"
                };
            }
        }

        public FileDownloadResult GetFile(string station, string prefix, string fileName)
        {
            const string METHOD = nameof(GetFile);

            try
            {
                _logger.LogInformation("{Class}.{Method} - Download requested | Station: {Station}, Prefix: {Prefix}, FileName: {FileName}",
                    CLASSNAME, METHOD, station, prefix, fileName);

                var fullPath = Path.Combine(_baseFolder, $"{station}_Results", $"{prefix}_Results", fileName);

                if (!System.IO.File.Exists(fullPath))
                {
                    _logger.LogWarning("{Class}.{Method} - File not found | FileName: {FileName}",
                        CLASSNAME, METHOD, fileName);

                    return null;
                }

                var bytes = System.IO.File.ReadAllBytes(fullPath);

                _logger.LogInformation("{Class}.{Method} - File retrieved successfully | FileName: {FileName}",
                    CLASSNAME, METHOD, fileName);

                return new FileDownloadResult
                {
                    Bytes = bytes,
                    FileName = fileName,
                    ContentType = "application/pdf"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{Class}.{Method} - Error downloading file | FileName: {FileName}",
                    CLASSNAME, METHOD, fileName);

                return null;
            }
        }
    }

    public class FileUploadResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string FileName { get; set; }
        public string RelativePath { get; set; }
    }

    public class FileDownloadResult
    {
        public byte[] Bytes { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }

}
