using ExcelFilesCompiler.Interfaces;
using Malama.Models;
using Malama.Utilities;

namespace ExcelFilesCompiler.Controllers.Services
{
    public sealed class TreatmentConsentFileSaveCoordinator
    {
        public const string StationName = "TreatmentConsent";
        public const string OralSurgeryPrefix = "oral_surgery_sig";
        public const string DentalTreatmentPrefix = "dental_treatment_sig";

        private readonly IFileUploadDownloadService _fileService;
        private readonly ILogger<TreatmentConsentFileSaveCoordinator> _logger;

        public TreatmentConsentFileSaveCoordinator(
            IFileUploadDownloadService fileService,
            ILogger<TreatmentConsentFileSaveCoordinator> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        public DentalXRayFileUpdatePlan BuildPlan(
            TreatmentConsentStationSaveDto dto,
            TreatmentConsent? existing,
            string barcode)
        {
            var plan = new DentalXRayFileUpdatePlan();
            if (string.IsNullOrWhiteSpace(barcode))
            {
                plan.ErrorMessage = "Service member barcode is required for signature upload.";
                return plan;
            }

            var existingOral = ParseOralForms(existing?.OralSurgeryFormsJson);
            var existingTreatment = ParseTreatmentForms(existing?.DentalTreatmentFormsJson);

            var oralIds = dto.IncludeOralSurgeryForm
                ? (dto.OralSurgeryDentistEventStaffIds ?? new List<long>()).Where(id => id > 0).Distinct().ToHashSet()
                : new HashSet<long>();
            var treatmentIds = dto.IncludeDentalTreatmentConsent
                ? (dto.DentalTreatmentDentistEventStaffIds ?? new List<long>()).Where(id => id > 0).Distinct().ToHashSet()
                : new HashSet<long>();

            foreach (var form in dto.OralSurgeryForms ?? new List<TreatmentConsentOralSurgeryFormDto>())
            {
                if (form.EventStaffId <= 0 || !oralIds.Contains(form.EventStaffId))
                {
                    continue;
                }

                PlanSignature(
                    plan,
                    barcode,
                    OralSurgeryPrefix,
                    BuildOralFinalName(barcode, form.EventStaffId),
                    form.SignatureDataUrl,
                    form.IsSigned,
                    existingOral.FirstOrDefault(x => x.EventStaffId == form.EventStaffId)?.SignatureFileName,
                    fileName => form.SignatureFileName = fileName,
                    signed => form.IsSigned = signed);
            }

            foreach (var form in dto.DentalTreatmentForms ?? new List<TreatmentConsentDentalTreatmentFormDto>())
            {
                if (form.EventStaffId <= 0 || !treatmentIds.Contains(form.EventStaffId))
                {
                    continue;
                }

                PlanSignature(
                    plan,
                    barcode,
                    DentalTreatmentPrefix,
                    BuildTreatmentFinalName(barcode, form.EventStaffId),
                    form.SignatureDataUrl,
                    form.IsSigned,
                    existingTreatment.FirstOrDefault(x => x.EventStaffId == form.EventStaffId)?.SignatureFileName,
                    fileName => form.SignatureFileName = fileName,
                    signed => form.IsSigned = signed);
            }

            // Dentist unchecked / form removed → delete orphaned signature files.
            foreach (var old in existingOral)
            {
                if (old.EventStaffId <= 0 || oralIds.Contains(old.EventStaffId))
                {
                    continue;
                }
                if (!string.IsNullOrWhiteSpace(old.SignatureFileName))
                {
                    plan.FilesToDelete.Add(new DentalXRayFileReference
                    {
                        Prefix = OralSurgeryPrefix,
                        FileName = old.SignatureFileName!
                    });
                }
            }

            foreach (var old in existingTreatment)
            {
                if (old.EventStaffId <= 0 || treatmentIds.Contains(old.EventStaffId))
                {
                    continue;
                }
                if (!string.IsNullOrWhiteSpace(old.SignatureFileName))
                {
                    plan.FilesToDelete.Add(new DentalXRayFileReference
                    {
                        Prefix = DentalTreatmentPrefix,
                        FileName = old.SignatureFileName!
                    });
                }
            }

            _logger.LogInformation(
                "Treatment Consent file plan created. Upload={UploadCount}, Delete={DeleteCount}, IsUpdate={IsUpdate}",
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
                    session.ErrorMessage = result.Message ?? "Failed to stage signature image.";
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
                    _logger.LogWarning(
                        "Failed to commit staged Treatment Consent signature. Staging={Staging}, Final={Final}",
                        upload.StagedFullPath,
                        upload.FinalFileName);
                }
            }

            foreach (var del in plan.FilesToDelete)
            {
                if (!_fileService.DeleteFile(StationName, del.Prefix, del.FileName))
                {
                    _logger.LogWarning(
                        "Failed to delete superseded Treatment Consent signature. Prefix={Prefix}, File={File}",
                        del.Prefix,
                        del.FileName);
                }
            }

            session.StagedFullPaths.Clear();
        }

        private static void PlanSignature(
            DentalXRayFileUpdatePlan plan,
            string barcode,
            string prefix,
            string finalFileName,
            string? signatureDataUrl,
            bool clientSaysSigned,
            string? existingFileName,
            Action<string?> setFileName,
            Action<bool> setSigned)
        {
            var hasNewInk = TreatmentConsentSignatureFileHelper.HasInkDataUrl(signatureDataUrl);
            if (hasNewInk)
            {
                var formFile = TreatmentConsentSignatureFileHelper.CreatePngFormFileFromDataUrl(
                    signatureDataUrl,
                    finalFileName);
                if (formFile == null)
                {
                    plan.ErrorMessage = "Invalid signature image data.";
                    return;
                }

                plan.FilesToUpload.Add(new DentalXRayStagedFileUpload
                {
                    File = formFile,
                    Prefix = prefix,
                    FinalFileName = finalFileName
                });

                if (!string.IsNullOrWhiteSpace(existingFileName)
                    && !string.Equals(existingFileName, finalFileName, StringComparison.OrdinalIgnoreCase))
                {
                    plan.FilesToDelete.Add(new DentalXRayFileReference
                    {
                        Prefix = prefix,
                        FileName = existingFileName!
                    });
                }

                setFileName(finalFileName);
                setSigned(true);
                return;
            }

            // No new ink: keep prior file when client still considers it signed.
            if (clientSaysSigned && !string.IsNullOrWhiteSpace(existingFileName))
            {
                setFileName(existingFileName);
                setSigned(true);
                return;
            }

            if (!string.IsNullOrWhiteSpace(existingFileName))
            {
                plan.FilesToDelete.Add(new DentalXRayFileReference
                {
                    Prefix = prefix,
                    FileName = existingFileName!
                });
            }

            setFileName(null);
            setSigned(false);
        }

        public static string BuildOralFinalName(string barcode, long eventStaffId)
            => $"{barcode}_os_{eventStaffId}.png";

        public static string BuildTreatmentFinalName(string barcode, long eventStaffId)
            => $"{barcode}_dtc_{eventStaffId}.png";

        public static List<TreatmentConsentOralSurgeryFormDto> ParseOralForms(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<TreatmentConsentOralSurgeryFormDto>();
            }

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<TreatmentConsentOralSurgeryFormDto>>(json)
                    ?? new List<TreatmentConsentOralSurgeryFormDto>();
            }
            catch
            {
                return new List<TreatmentConsentOralSurgeryFormDto>();
            }
        }

        public static List<TreatmentConsentDentalTreatmentFormDto> ParseTreatmentForms(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<TreatmentConsentDentalTreatmentFormDto>();
            }

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<TreatmentConsentDentalTreatmentFormDto>>(json)
                    ?? new List<TreatmentConsentDentalTreatmentFormDto>();
            }
            catch
            {
                return new List<TreatmentConsentDentalTreatmentFormDto>();
            }
        }
    }
}
