using ExcelFilesCompiler.Interfaces;
using Malama.Models;
using Malama.Utilities;
using System.Text.Json;

namespace ExcelFilesCompiler.Controllers.Services
{
    public sealed class TreatmentCoordinatorDocumentFileSaveCoordinator
    {
        public const string StationName = "TreatmentCoordinator";
        public const string DocumentPrefix = "document";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        private readonly IFileUploadDownloadService _fileService;
        private readonly ILogger<TreatmentCoordinatorDocumentFileSaveCoordinator> _logger;

        public TreatmentCoordinatorDocumentFileSaveCoordinator(
            IFileUploadDownloadService fileService,
            ILogger<TreatmentCoordinatorDocumentFileSaveCoordinator> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        public static List<TreatmentCoordinatorDocumentMetaDto> ParseDocumentsJson(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<TreatmentCoordinatorDocumentMetaDto>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<TreatmentCoordinatorDocumentMetaDto>>(json, JsonOptions)
                    ?? new List<TreatmentCoordinatorDocumentMetaDto>();
            }
            catch
            {
                return new List<TreatmentCoordinatorDocumentMetaDto>();
            }
        }

        public static string SerializeDocuments(IEnumerable<TreatmentCoordinatorDocumentMetaDto> documents)
        {
            var list = (documents ?? Enumerable.Empty<TreatmentCoordinatorDocumentMetaDto>())
                .Where(d => d != null && !string.IsNullOrWhiteSpace(d.FileName))
                .Select(d => new TreatmentCoordinatorDocumentMetaDto
                {
                    FileName = d.FileName.Trim(),
                    OriginalFileName = string.IsNullOrWhiteSpace(d.OriginalFileName) ? null : d.OriginalFileName.Trim(),
                    UploadedOn = d.UploadedOn
                })
                .ToList();

            return list.Count == 0 ? "[]" : JsonSerializer.Serialize(list, JsonOptions);
        }

        public DentalXRayFileUpdatePlan BuildPlan(
            DentalCoordinatorStationSaveDto dto,
            IReadOnlyList<TreatmentCoordinatorDocumentMetaDto> existingDocuments,
            string barcode,
            out List<TreatmentCoordinatorDocumentMetaDto> resultingDocuments)
        {
            var plan = new DentalXRayFileUpdatePlan();
            resultingDocuments = new List<TreatmentCoordinatorDocumentMetaDto>();

            if (string.IsNullOrWhiteSpace(barcode))
            {
                plan.ErrorMessage = "Service member barcode is required for document upload.";
                return plan;
            }

            var retainedNames = (dto.RetainedDocumentFileNames ?? new List<string>())
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => n.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var existingByName = existingDocuments
                .Where(d => !string.IsNullOrWhiteSpace(d.FileName))
                .GroupBy(d => d.FileName.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.Last(), StringComparer.OrdinalIgnoreCase);

            foreach (var name in retainedNames)
            {
                if (!existingByName.TryGetValue(name, out var meta))
                {
                    plan.ErrorMessage = $"Retained document '{name}' was not found.";
                    resultingDocuments = new List<TreatmentCoordinatorDocumentMetaDto>();
                    return plan;
                }

                resultingDocuments.Add(new TreatmentCoordinatorDocumentMetaDto
                {
                    FileName = meta.FileName,
                    OriginalFileName = meta.OriginalFileName,
                    UploadedOn = meta.UploadedOn
                });
            }

            foreach (var existing in existingDocuments)
            {
                if (string.IsNullOrWhiteSpace(existing.FileName))
                {
                    continue;
                }

                if (!retainedNames.Contains(existing.FileName))
                {
                    plan.FilesToDelete.Add(new DentalXRayFileReference
                    {
                        Prefix = DocumentPrefix,
                        FileName = existing.FileName
                    });
                }
            }

            var newFiles = (dto.TreatmentCoordinatorDocuments ?? new List<IFormFile>())
                .Where(f => f != null && f.Length > 0)
                .ToList();

            var now = DateTime.Now;
            var index = 0;
            foreach (var file in newFiles)
            {
                index += 1;
                var finalName = $"{barcode}_doc_{now:yyyyMMddHHmmss}_{index}_{Guid.NewGuid():N}.pdf";
                plan.FilesToUpload.Add(new DentalXRayStagedFileUpload
                {
                    File = file,
                    Prefix = DocumentPrefix,
                    FinalFileName = finalName
                });
                resultingDocuments.Add(new TreatmentCoordinatorDocumentMetaDto
                {
                    FileName = finalName,
                    OriginalFileName = file.FileName,
                    UploadedOn = now
                });
            }

            if (resultingDocuments.Count > TreatmentCoordinatorDocumentsValidator.MaxDocumentCount)
            {
                plan.ErrorMessage =
                    $"A maximum of {TreatmentCoordinatorDocumentsValidator.MaxDocumentCount} documents can be uploaded.";
                resultingDocuments = new List<TreatmentCoordinatorDocumentMetaDto>();
                return plan;
            }

            _logger.LogInformation(
                "Treatment Coordinator document plan created. Upload={UploadCount}, Delete={DeleteCount}, Retained={RetainedCount}",
                plan.FilesToUpload.Count,
                plan.FilesToDelete.Count,
                retainedNames.Count);

            return plan;
        }

        public async Task<DentalXRayFileUploadSession> UploadToStagingAsync(
            DentalXRayFileUpdatePlan plan,
            string barcode)
        {
            var session = new DentalXRayFileUploadSession { Success = true };

            foreach (var upload in plan.FilesToUpload)
            {
                var fileKey = Path.GetFileNameWithoutExtension(upload.FinalFileName)
                    ?.Replace($"{barcode}_", string.Empty, StringComparison.OrdinalIgnoreCase)
                    ?? Guid.NewGuid().ToString("N");

                var result = await _fileService.UploadPdfFileToStaging(
                    upload.File,
                    StationName,
                    upload.Prefix,
                    barcode,
                    fileKey);

                if (!result.Success)
                {
                    session.Success = false;
                    session.ErrorMessage = result.Message ?? "Failed to stage document.";
                    await RollbackStagingAsync(session);
                    return session;
                }

                upload.StagedFullPath = result.FullPath;
                session.StagedFullPaths.Add(result.FullPath!);
            }

            return session;
        }

        public Task RollbackStagingAsync(DentalXRayFileUploadSession session)
        {
            foreach (var path in session.StagedFullPaths.Where(p => !string.IsNullOrWhiteSpace(p)))
            {
                _ = _fileService.DeleteFileAsync(path);
            }

            session.StagedFullPaths.Clear();
            return Task.CompletedTask;
        }

        public void CommitFileChanges(DentalXRayFileUpdatePlan plan, DentalXRayFileUploadSession session)
        {
            foreach (var upload in plan.FilesToUpload)
            {
                if (string.IsNullOrWhiteSpace(upload.StagedFullPath))
                {
                    continue;
                }

                var committed = _fileService.CommitStagedImageFile(
                    upload.StagedFullPath,
                    StationName,
                    upload.Prefix,
                    upload.FinalFileName);

                if (!committed)
                {
                    throw new InvalidOperationException(
                        $"Failed to commit document '{upload.FinalFileName}'.");
                }
            }

            foreach (var delete in plan.FilesToDelete)
            {
                _fileService.DeleteFile(StationName, delete.Prefix, delete.FileName);
            }

            session.StagedFullPaths.Clear();
        }
    }
}
