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
                    FullPath = fullPath,
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

        public string GetImageFullPath(string station, string prefix, string fileName)
        {
            return Path.Combine(_baseFolder, $"{station}_Results", $"{prefix}_Results", fileName);
        }

        public async Task<FileUploadResult> UploadImageFileToStaging(
            IFormFile file,
            string station,
            string prefix,
            string barcode,
            string fileKey)
        {
            const string METHOD = nameof(UploadImageFileToStaging);

            try
            {
                if (file == null || file.Length == 0)
                {
                    return new FileUploadResult { Success = false, Message = "No file selected" };
                }

                var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
                if (extension != ".jpg" && extension != ".jpeg")
                {
                    return new FileUploadResult { Success = false, Message = "Only JPEG/JPG images are allowed" };
                }

                var prefixFolder = Path.Combine(_baseFolder, $"{station}_Results", $"{prefix}_Results");
                var stagingFolder = Path.Combine(prefixFolder, ".staging");

                Directory.CreateDirectory(stagingFolder);

                var stagingFileName = $"{barcode}_{fileKey}_{Guid.NewGuid():N}.jpg";
                var fullPath = Path.Combine(stagingFolder, stagingFileName);

                await using (var stream = new FileStream(fullPath, FileMode.CreateNew))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation(
                    "{Class}.{Method} - Staged image uploaded | Path={Path}",
                    CLASSNAME, METHOD, fullPath);

                return new FileUploadResult
                {
                    Success = true,
                    FileName = stagingFileName,
                    FullPath = fullPath
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{Class}.{Method} - Error staging image | Station={Station}, Prefix={Prefix}, FileKey={FileKey}",
                    CLASSNAME, METHOD, station, prefix, fileKey);

                return new FileUploadResult
                {
                    Success = false,
                    Message = "Error occurred while staging image"
                };
            }
        }

        public bool CommitStagedImageFile(string stagingFullPath, string station, string prefix, string finalFileName)
        {
            const string METHOD = nameof(CommitStagedImageFile);

            try
            {
                if (string.IsNullOrWhiteSpace(stagingFullPath) || !System.IO.File.Exists(stagingFullPath))
                {
                    _logger.LogWarning("{Class}.{Method} - Staged file missing | Path={Path}", CLASSNAME, METHOD, stagingFullPath);
                    return false;
                }

                var prefixFolder = Path.Combine(_baseFolder, $"{station}_Results", $"{prefix}_Results");
                Directory.CreateDirectory(prefixFolder);

                var destinationPath = Path.Combine(prefixFolder, finalFileName);
                System.IO.File.Move(stagingFullPath, destinationPath, overwrite: true);

                _logger.LogInformation(
                    "{Class}.{Method} - Staged file committed | Destination={Destination}",
                    CLASSNAME, METHOD, destinationPath);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{Class}.{Method} - Failed to commit staged file | StagingPath={StagingPath}, FinalFileName={FinalFileName}",
                    CLASSNAME, METHOD, stagingFullPath, finalFileName);
                return false;
            }
        }

        public async Task<FileUploadResult> UploadImageFile(IFormFile file, string station, string prefix, string barcode, string fileKey)
        {
            const string METHOD = nameof(UploadImageFile);

            try
            {
                _logger.LogInformation("{Class}.{Method} - Image upload started | Station: {Station}, Prefix: {Prefix}, Barcode: {Barcode}, FileKey: {FileKey}",
                    CLASSNAME, METHOD, station, prefix, barcode, fileKey);

                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("{Class}.{Method} - No file selected", CLASSNAME, METHOD);
                    return new FileUploadResult { Success = false, Message = "No file selected" };
                }

                var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
                if (extension != ".jpg" && extension != ".jpeg")
                {
                    _logger.LogWarning("{Class}.{Method} - Invalid image extension: {Extension}", CLASSNAME, METHOD, extension);
                    return new FileUploadResult { Success = false, Message = "Only JPEG/JPG images are allowed" };
                }

                var stationFolder = Path.Combine(_baseFolder, $"{station}_Results");
                var prefixFolder = Path.Combine(stationFolder, $"{prefix}_Results");

                if (!Directory.Exists(stationFolder))
                {
                    Directory.CreateDirectory(stationFolder);
                }

                if (!Directory.Exists(prefixFolder))
                {
                    Directory.CreateDirectory(prefixFolder);
                }

                var fileName = $"{barcode}_{fileKey}.jpg";
                var fullPath = Path.Combine(prefixFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation("{Class}.{Method} - Image uploaded successfully | FileName: {FileName}",
                    CLASSNAME, METHOD, fileName);

                return new FileUploadResult
                {
                    Success = true,
                    FileName = fileName,
                    FullPath = fullPath,
                    RelativePath = Path.Combine("Results", $"{station}_Results", $"{prefix}_Results", fileName)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{Class}.{Method} - Error uploading image | Station: {Station}, Prefix: {Prefix}, Barcode: {Barcode}, FileKey: {FileKey}",
                    CLASSNAME, METHOD, station, prefix, barcode, fileKey);

                return new FileUploadResult
                {
                    Success = false,
                    Message = "Error occurred while uploading image"
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

                var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
                var contentType = extension is ".jpg" or ".jpeg" ? "image/jpeg" : "application/pdf";

                _logger.LogInformation("{Class}.{Method} - File retrieved successfully | FileName: {FileName}",
                    CLASSNAME, METHOD, fileName);

                return new FileDownloadResult
                {
                    Bytes = bytes,
                    FileName = fileName,
                    ContentType = contentType
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

        public bool DeleteFile(string station, string prefix, string fileName)
        {
            const string METHOD = nameof(DeleteFile);

            try
            {
                _logger.LogInformation("{Class}.{Method} - Delete requested | Station: {Station}, Prefix: {Prefix}, FileName: {FileName}",
                    CLASSNAME, METHOD, station, prefix, fileName);

                var fullPath = Path.Combine(_baseFolder, $"{station}_Results", $"{prefix}_Results", fileName);

                if (!System.IO.File.Exists(fullPath))
                {
                    _logger.LogInformation("{Class}.{Method} - File already absent | FileName: {FileName}",
                        CLASSNAME, METHOD, fileName);
                    return true;
                }

                System.IO.File.Delete(fullPath);

                _logger.LogInformation("{Class}.{Method} - File deleted successfully | FileName: {FileName}",
                    CLASSNAME, METHOD, fileName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{Class}.{Method} - Error deleting file | Station: {Station}, Prefix: {Prefix}, FileName: {FileName}",
                    CLASSNAME, METHOD, station, prefix, fileName);
                return false;
            }
        }

        public Task DeleteFileAsync(string filePath)
        {
            const string METHOD = nameof(DeleteFileAsync);

            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    return Task.CompletedTask;
                }

                var fullPath = Path.GetFullPath(filePath);
                var basePath = Path.GetFullPath(_baseFolder);

                if (!fullPath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning(
                        "{Class}.{Method} - Delete rejected, path is outside base folder | Path={Path}",
                        CLASSNAME, METHOD, fullPath);
                    return Task.CompletedTask;
                }

                if (!System.IO.File.Exists(fullPath))
                {
                    _logger.LogInformation(
                        "{Class}.{Method} - File not found for delete | Path={Path}",
                        CLASSNAME, METHOD, fullPath);
                    return Task.CompletedTask;
                }

                System.IO.File.Delete(fullPath);

                _logger.LogInformation(
                    "{Class}.{Method} - File deleted successfully | Path={Path}",
                    CLASSNAME, METHOD, fullPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{Class}.{Method} - Rollback delete failure | Path={Path}",
                    CLASSNAME, METHOD, filePath);
            }

            return Task.CompletedTask;
        }
    }

    public class FileUploadResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public string RelativePath { get; set; }
    }

    public class FileDownloadResult
    {
        public byte[] Bytes { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }

}
