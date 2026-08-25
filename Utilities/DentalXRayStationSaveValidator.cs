using ExcelFilesCompiler.Controllers.Services;
using ExcelFilesCompiler.Interfaces;
using Malama.Models;

namespace ExcelFilesCompiler.Utilities
{
    public static class DentalXRayStationSaveValidator
    {
        public static string? Validate(
            DentalXRayStationSaveDto dto,
            ServiceMembersChild serviceMember,
            IDentalQuestionnaireService dentalQuestionnaireService)
        {
            var questionnaireError = DentalQuestionnaireValidator.Validate(dto, serviceMember);
            if (questionnaireError != null) return questionnaireError;

            if (DentalXRayStationService.IsFemale(serviceMember))
            {
                if (string.IsNullOrWhiteSpace(dto.AreYouPregnant))
                {
                    return "Pregnancy question is required for female service members.";
                }

                if (dto.AreYouPregnant.Equals("Yes", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(dto.PregnancyApproval))
                    {
                        return "Approval selection is required when pregnant.";
                    }

                    if (dto.PregnancyApproval.Equals("Declined", StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }
                }
            }

            if (!DentalXRayStationService.CanProceedWithXRay(
                dentalQuestionnaireService.MapFormDataToEntity(dto),
                serviceMember))
            {
                return "Cannot proceed with X-Ray based on questionnaire responses.";
            }

            if (DentalXRayStationService.IsNeeded(serviceMember.BwxNeeded))
            {
                if (dto.BwxStatus == "Completed" && string.IsNullOrWhiteSpace(dto.BwxUploadMode))
                {
                    return "BWX upload type selection is required.";
                }

                var bwxError = ValidateBwxSection(dto);
                if (bwxError != null) return bwxError;

                var paError = ValidatePaSection(dto);
                if (paError != null) return paError;
            }

            return null;
        }

        public static string? ValidateBwxSection(DentalXRayStationSaveDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.BwxStatus))
            {
                return null;
            }

            if (dto.BwxStatus == "Not Completed" && string.IsNullOrWhiteSpace(dto.BwxReason))
            {
                return "BWX Status reason is required.";
            }

            if (dto.BwxStatus != "Completed")
            {
                return null;
            }

            if (string.Equals(dto.BwxUploadMode, BwxUploadMode.Consolidated, StringComparison.OrdinalIgnoreCase)
                && !HasConsolidatedBwxUpload(dto))
            {
                return "BWX Status requires consolidated X-Ray image upload.";
            }

            if (string.Equals(dto.BwxUploadMode, BwxUploadMode.Separate, StringComparison.OrdinalIgnoreCase)
                && !AreAllSeparateBwxUploadsPresent(dto))
            {
                return "BWX Status requires all 4 X-Ray uploads.";
            }

            return null;
        }

        public static string? ValidatePaSection(DentalXRayStationSaveDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.PaStatus))
            {
                return null;
            }

            if (dto.PaStatus == "Not Completed" && string.IsNullOrWhiteSpace(dto.PaReason))
            {
                return "Periapical (PA) X-Rays reason is required.";
            }

            if (dto.PaStatus == "Completed")
            {
                var activePaImages = dto.PaImages?.Where(p => !p.Removed).ToList() ?? new List<DentalXRayPaImageDto>();
                if (!activePaImages.Any())
                {
                    return "At least one PA X-Ray image is required.";
                }

                if (activePaImages.Count > 8)
                {
                    return "A maximum of 8 PA X-Ray uploads is allowed.";
                }

                foreach (var image in activePaImages)
                {
                    var hasExisting = !string.IsNullOrWhiteSpace(image.FileName);
                    var hasNew = image.ImageFile != null && image.ImageFile.Length > 0;
                    if (!hasExisting && !hasNew)
                    {
                        return "All PA X-Ray cards require an uploaded image.";
                    }
                }
            }

            return null;
        }

        public static void SetSectionUploadedDateTimes(DentalXRayStationSaveDto dto)
        {
            if (dto.BwxStatus == "Completed" && AreAllBwxUploadsPresent(dto))
            {
                dto.BwxUploadedDateTime ??= DateTime.Now;
            }
            else
            {
                dto.BwxUploadedDateTime = null;
            }

            if (dto.PaStatus == "Completed" &&
                dto.PaImages != null &&
                dto.PaImages.Any(p => !p.Removed && !string.IsNullOrWhiteSpace(p.FileName)))
            {
                dto.PaUploadedDateTime ??= DateTime.Now;
            }
            else
            {
                dto.PaUploadedDateTime = null;
            }
        }

        private static bool AreAllBwxUploadsPresent(DentalXRayStationSaveDto dto)
        {
            if (string.Equals(dto.BwxUploadMode, BwxUploadMode.Consolidated, StringComparison.OrdinalIgnoreCase))
            {
                return HasConsolidatedBwxUpload(dto);
            }

            return AreAllSeparateBwxUploadsPresent(dto);
        }

        private static bool HasConsolidatedBwxUpload(DentalXRayStationSaveDto dto)
        {
            return HasUpload(
                dto.BwxConsolidatedUploaded,
                dto.BwxConsolidatedFileName,
                dto.BwxConsolidatedFile,
                dto.BwxConsolidatedRemoved);
        }

        private static bool AreAllSeparateBwxUploadsPresent(DentalXRayStationSaveDto dto)
        {
            return HasUpload(dto.BwLeftMolarUploaded, dto.BwLeftMolarFileName, dto.BwLeftMolarFile, dto.BwLeftMolarRemoved)
                && HasUpload(dto.BwLeftPremolarUploaded, dto.BwLeftPremolarFileName, dto.BwLeftPremolarFile, dto.BwLeftPremolarRemoved)
                && HasUpload(dto.BwRightMolarUploaded, dto.BwRightMolarFileName, dto.BwRightMolarFile, dto.BwRightMolarRemoved)
                && HasUpload(dto.BwRightPremolarUploaded, dto.BwRightPremolarFileName, dto.BwRightPremolarFile, dto.BwRightPremolarRemoved);
        }

        private static bool HasUpload(bool uploadedFlag, string? fileName, IFormFile? file, bool removed)
        {
            if (removed)
            {
                return false;
            }

            return uploadedFlag || !string.IsNullOrWhiteSpace(fileName) || (file != null && file.Length > 0);
        }
    }
}
