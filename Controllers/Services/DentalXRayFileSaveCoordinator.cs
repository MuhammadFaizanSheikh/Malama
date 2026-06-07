using ExcelFilesCompiler.Interfaces;
using Malama.Models;

namespace ExcelFilesCompiler.Controllers.Services
{
    public sealed class DentalXRayFileSaveCoordinator
    {
        private const string StationName = "DentalXRay";
        private const string PaPrefix = "pa_tooth";

        private readonly IFileUploadDownloadService _fileService;
        private readonly ILogger<DentalXRayFileSaveCoordinator> _logger;

        private static readonly (string Prefix, string FileKey)[] BwxSlots =
        {
            ("bwx_left_molar", "left_molar"),
            ("bwx_left_premolar", "left_premolar"),
            ("bwx_right_molar", "right_molar"),
            ("bwx_right_premolar", "right_premolar")
        };

        public DentalXRayFileSaveCoordinator(
            IFileUploadDownloadService fileService,
            ILogger<DentalXRayFileSaveCoordinator> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        public DentalXRayFileUpdatePlan BuildPlan(
            DentalXRayStationSaveDto dto,
            DentalXRayStation? existing,
            string barcode)
        {
            var plan = new DentalXRayFileUpdatePlan();

            if (string.IsNullOrWhiteSpace(barcode))
            {
                plan.ErrorMessage = "Service member barcode is required for file upload.";
                return plan;
            }

            PlanBwxFiles(dto, existing, barcode, plan);
            PlanPaFiles(dto, existing, barcode, plan);

            _logger.LogInformation(
                "Dental X-Ray file plan created. Upload={UploadCount}, Delete={DeleteCount}, IsUpdate={IsUpdate}",
                plan.FilesToUpload.Count,
                plan.FilesToDelete.Count,
                existing != null);

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
                    ?? upload.FinalFileName;

                var result = await _fileService.UploadImageFileToStaging(
                    upload.File,
                    StationName,
                    upload.Prefix,
                    barcode,
                    fileKey);

                if (!result.Success)
                {
                    session.Success = false;
                    session.ErrorMessage = result.Message ?? "Failed to stage X-Ray image.";
                    await RollbackStagingAsync(session);
                    return session;
                }

                upload.StagedFullPath = result.FullPath;
                session.StagedFullPaths.Add(result.FullPath);
            }

            return session;
        }

        public Task RollbackStagingAsync(DentalXRayFileUploadSession session)
        {
            foreach (var path in session.StagedFullPaths.Where(p => !string.IsNullOrWhiteSpace(p)))
            {
                _fileService.DeleteFileAsync(path);
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
                    _logger.LogWarning(
                        "Failed to commit staged Dental X-Ray file. Prefix={Prefix}, FinalFileName={FinalFileName}",
                        upload.Prefix,
                        upload.FinalFileName);
                }
            }

            session.StagedFullPaths.Clear();

            foreach (var file in plan.FilesToDelete.DistinctBy(f => $"{f.Prefix}|{f.FileName}", StringComparer.OrdinalIgnoreCase))
            {
                var deleted = _fileService.DeleteFile(StationName, file.Prefix, file.FileName);
                if (!deleted)
                {
                    _logger.LogWarning(
                        "Failed to delete superseded Dental X-Ray file. Prefix={Prefix}, FileName={FileName}",
                        file.Prefix,
                        file.FileName);
                }
            }
        }

        private void PlanBwxFiles(
            DentalXRayStationSaveDto dto,
            DentalXRayStation? existing,
            string barcode,
            DentalXRayFileUpdatePlan plan)
        {
            if (dto.BwxStatus != "Completed")
            {
                QueueExistingBwxDeletes(existing, plan);
                ClearBwxDtoFields(dto);
                return;
            }

            for (var i = 0; i < BwxSlots.Length; i++)
            {
                var (prefix, fileKey) = BwxSlots[i];
                var finalFileName = $"{barcode}_{fileKey}.jpg";
                var slot = GetBwxSlotState(dto, i);
                var existingFileName = existing == null ? null : GetBwxExistingFileName(existing, i);

                if (slot.Removed)
                {
                    QueueDelete(plan, prefix, existingFileName);
                    ClearBwxSlot(dto, i);
                    continue;
                }

                if (slot.File != null && slot.File.Length > 0)
                {
                    plan.FilesToUpload.Add(new DentalXRayStagedFileUpload
                    {
                        File = slot.File,
                        Prefix = prefix,
                        FinalFileName = finalFileName
                    });

                    SetBwxSlotFileName(dto, i, finalFileName, slot.File.FileName, DateTime.Now, true);
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(slot.FileName))
                {
                    SetBwxSlotUploaded(dto, i, true);
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(existingFileName))
                {
                    SetBwxSlotFileName(
                        dto,
                        i,
                        existingFileName,
                        GetBwxExistingOriginalFileName(existing, i),
                        GetBwxExistingUploadedDateTime(existing, i),
                        true);
                }
            }
        }

        private void PlanPaFiles(
            DentalXRayStationSaveDto dto,
            DentalXRayStation? existing,
            string barcode,
            DentalXRayFileUpdatePlan plan)
        {
            if (dto.PaStatus != "Completed")
            {
                QueueExistingPaDeletes(existing, plan);
                dto.PaImages = new List<DentalXRayPaImageDto>();
                dto.PaUploadedDateTime = null;
                return;
            }

            dto.PaImages ??= new List<DentalXRayPaImageDto>();
            var retainedFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (var i = 0; i < dto.PaImages.Count; i++)
            {
                var pa = dto.PaImages[i];
                if (pa.Removed)
                {
                    QueueDelete(plan, PaPrefix, pa.FileName);
                    pa.FileName = null;
                    pa.OriginalFileName = null;
                    pa.UploadedDateTime = null;
                    pa.Uploaded = false;
                    continue;
                }

                if (pa.ImageFile != null && pa.ImageFile.Length > 0)
                {
                    var finalFileName = $"{barcode}_pa_{i + 1}.jpg";
                    plan.FilesToUpload.Add(new DentalXRayStagedFileUpload
                    {
                        File = pa.ImageFile,
                        Prefix = PaPrefix,
                        FinalFileName = finalFileName
                    });

                    pa.FileName = finalFileName;
                    pa.OriginalFileName = pa.ImageFile.FileName;
                    pa.UploadedDateTime = DateTime.Now;
                    pa.Uploaded = true;
                    retainedFileNames.Add(finalFileName);
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(pa.FileName))
                {
                    pa.Uploaded = true;
                    retainedFileNames.Add(pa.FileName);
                }
            }

            if (existing?.PaImages != null)
            {
                foreach (var existingImage in existing.PaImages)
                {
                    if (!string.IsNullOrWhiteSpace(existingImage.FileName) &&
                        !retainedFileNames.Contains(existingImage.FileName))
                    {
                        QueueDelete(plan, PaPrefix, existingImage.FileName);
                    }
                }
            }
        }

        private static void QueueExistingBwxDeletes(DentalXRayStation? existing, DentalXRayFileUpdatePlan plan)
        {
            if (existing == null)
            {
                return;
            }

            for (var i = 0; i < BwxSlots.Length; i++)
            {
                QueueDelete(plan, BwxSlots[i].Prefix, GetBwxExistingFileName(existing, i));
            }
        }

        private static void QueueExistingPaDeletes(DentalXRayStation? existing, DentalXRayFileUpdatePlan plan)
        {
            if (existing?.PaImages == null)
            {
                return;
            }

            foreach (var image in existing.PaImages)
            {
                QueueDelete(plan, PaPrefix, image.FileName);
            }
        }

        private static void QueueDelete(DentalXRayFileUpdatePlan plan, string prefix, string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            plan.FilesToDelete.Add(new DentalXRayFileReference
            {
                Prefix = prefix,
                FileName = fileName
            });
        }

        private sealed class BwxSlotState
        {
            public bool Removed { get; init; }
            public string? FileName { get; init; }
            public IFormFile? File { get; init; }
        }

        private static BwxSlotState GetBwxSlotState(DentalXRayStationSaveDto dto, int index) => index switch
        {
            0 => new BwxSlotState { Removed = dto.BwLeftMolarRemoved, FileName = dto.BwLeftMolarFileName, File = dto.BwLeftMolarFile },
            1 => new BwxSlotState { Removed = dto.BwLeftPremolarRemoved, FileName = dto.BwLeftPremolarFileName, File = dto.BwLeftPremolarFile },
            2 => new BwxSlotState { Removed = dto.BwRightMolarRemoved, FileName = dto.BwRightMolarFileName, File = dto.BwRightMolarFile },
            _ => new BwxSlotState { Removed = dto.BwRightPremolarRemoved, FileName = dto.BwRightPremolarFileName, File = dto.BwRightPremolarFile }
        };

        private static string? GetBwxExistingFileName(DentalXRayStation existing, int index) => index switch
        {
            0 => existing.BwLeftMolarFileName,
            1 => existing.BwLeftPremolarFileName,
            2 => existing.BwRightMolarFileName,
            _ => existing.BwRightPremolarFileName
        };

        private static string? GetBwxExistingOriginalFileName(DentalXRayStation existing, int index) => index switch
        {
            0 => existing.BwLeftMolarOriginalFileName,
            1 => existing.BwLeftPremolarOriginalFileName,
            2 => existing.BwRightMolarOriginalFileName,
            _ => existing.BwRightPremolarOriginalFileName
        };

        private static DateTime? GetBwxExistingUploadedDateTime(DentalXRayStation existing, int index) => index switch
        {
            0 => existing.BwLeftMolarUploadedDateTime,
            1 => existing.BwLeftPremolarUploadedDateTime,
            2 => existing.BwRightMolarUploadedDateTime,
            _ => existing.BwRightPremolarUploadedDateTime
        };

        private static void SetBwxSlotFileName(
            DentalXRayStationSaveDto dto,
            int index,
            string? fileName,
            string? originalFileName,
            DateTime? uploadedDateTime,
            bool uploaded)
        {
            switch (index)
            {
                case 0:
                    dto.BwLeftMolarFileName = fileName;
                    dto.BwLeftMolarOriginalFileName = originalFileName;
                    dto.BwLeftMolarUploadedDateTime = uploadedDateTime;
                    dto.BwLeftMolarUploaded = uploaded;
                    break;
                case 1:
                    dto.BwLeftPremolarFileName = fileName;
                    dto.BwLeftPremolarOriginalFileName = originalFileName;
                    dto.BwLeftPremolarUploadedDateTime = uploadedDateTime;
                    dto.BwLeftPremolarUploaded = uploaded;
                    break;
                case 2:
                    dto.BwRightMolarFileName = fileName;
                    dto.BwRightMolarOriginalFileName = originalFileName;
                    dto.BwRightMolarUploadedDateTime = uploadedDateTime;
                    dto.BwRightMolarUploaded = uploaded;
                    break;
                default:
                    dto.BwRightPremolarFileName = fileName;
                    dto.BwRightPremolarOriginalFileName = originalFileName;
                    dto.BwRightPremolarUploadedDateTime = uploadedDateTime;
                    dto.BwRightPremolarUploaded = uploaded;
                    break;
            }
        }

        private static void SetBwxSlotUploaded(DentalXRayStationSaveDto dto, int index, bool uploaded)
        {
            switch (index)
            {
                case 0: dto.BwLeftMolarUploaded = uploaded; break;
                case 1: dto.BwLeftPremolarUploaded = uploaded; break;
                case 2: dto.BwRightMolarUploaded = uploaded; break;
                default: dto.BwRightPremolarUploaded = uploaded; break;
            }
        }

        private static void ClearBwxSlot(DentalXRayStationSaveDto dto, int index)
        {
            SetBwxSlotFileName(dto, index, null, null, null, false);
            switch (index)
            {
                case 0: dto.BwLeftMolarRemoved = false; break;
                case 1: dto.BwLeftPremolarRemoved = false; break;
                case 2: dto.BwRightMolarRemoved = false; break;
                default: dto.BwRightPremolarRemoved = false; break;
            }
        }

        private static void ClearBwxDtoFields(DentalXRayStationSaveDto dto)
        {
            for (var i = 0; i < BwxSlots.Length; i++)
            {
                ClearBwxSlot(dto, i);
            }

            dto.BwxUploadedDateTime = null;
        }
    }
}
