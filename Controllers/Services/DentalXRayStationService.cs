using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using ExcelFilesCompiler.Utilities;
using Malama.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class DentalXRayStationService : IDentalXRayStationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DentalXRayStationService> _logger;
        private const string CLASSNAME = "DentalXRayStationService";

        public DentalXRayStationService(ILogger<DentalXRayStationService> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<(DentalXRayStation DentalXRayStation, long EventId)> GetDentalXRayStationByIdWithEventIdAsync(long dentalXRayStationId)
        {
            const string methodName = nameof(GetDentalXRayStationByIdWithEventIdAsync);

            try
            {
                _logger.LogDebug("{ClassName}, {MethodName}, Fetching DentalXRayStation with Id: {Id}", CLASSNAME, methodName, dentalXRayStationId);

                var result = await _unitOfWork.ServiceMembersChild
                    .GetWithIncludeNoTracking(
                        c => c.DentalXRayStationRecord != null && c.DentalXRayStationRecord.Id == dentalXRayStationId,
                        c => c.DentalXRayStationRecord,
                        c => c.ServiceMembersParent.EventManagement)
                    .Include(c => c.DentalXRayStationRecord)
                        .ThenInclude(d => d.PaImages)
                    .Include(c => c.DentalXRayStationRecord)
                        .ThenInclude(d => d.ServiceMembersChild)
                    .Select(c => new
                    {
                        DentalXRayStation = c.DentalXRayStationRecord,
                        EventId = c.ServiceMembersParent.EventManagement.Id
                    })
                    .FirstOrDefaultAsync();

                if (result?.DentalXRayStation == null)
                {
                    _logger.LogInformation("{ClassName}, {MethodName}, No DentalXRayStationRecord found with Id: {Id}", CLASSNAME, methodName, dentalXRayStationId);
                    return (null, 0);
                }

                result.DentalXRayStation.PaImages = result.DentalXRayStation.PaImages
                    .OrderBy(p => p.SortOrder)
                    .ToList();

                return (result.DentalXRayStation, result.EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Error fetching DentalXRayStationRecord with Id: {Id}", CLASSNAME, methodName, dentalXRayStationId);
                throw;
            }
        }

        public async Task<DentalXRayStation?> GetByServiceMembersChildIdAsync(long serviceMembersChildId)
        {
            const string methodName = nameof(GetByServiceMembersChildIdAsync);

            try
            {
                if (serviceMembersChildId <= 0)
                {
                    return null;
                }

                var station = await _unitOfWork.DentalXRayStation
                    .GetWithIncludeNoTracking(d => d.ServiceMembersChildId == serviceMembersChildId)
                    .Include(d => d.PaImages)
                    .FirstOrDefaultAsync();

                if (station == null)
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, No DentalXRayStationRecord found for ServiceMembersChildId={ServiceMembersChildId}",
                        CLASSNAME, methodName, serviceMembersChildId);
                    return null;
                }

                station.PaImages = station.PaImages.OrderBy(p => p.SortOrder).ToList();
                return station;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Error fetching DentalXRayStationRecord for ServiceMembersChildId={ServiceMembersChildId}",
                    CLASSNAME, methodName, serviceMembersChildId);
                throw;
            }
        }

        public async Task AddAsync(DentalXRayStation model, string userName)
        {
            model.AddedOn = DateTime.Now;
            model.AddedBy = userName;
            NormalizePaImages(model);

            await _unitOfWork.DentalXRayStation.AddAsync(model);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(DentalXRayStation model, string userName)
        {
            const string methodName = nameof(UpdateAsync);

            try
            {
                var existing = await _unitOfWork.DentalXRayStation
                    .GetWithIncludeTracking(d => d.Id == model.Id, d => d.PaImages)
                    .FirstOrDefaultAsync();

                if (existing == null)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, Dental X-Ray record with Id={Id} not found by user {User}",
                        CLASSNAME, methodName, model.Id, userName);
                    throw new KeyNotFoundException($"Dental X-Ray record with Id={model.Id} not found.");
                }

                MapToEntity(model, existing, userName);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation("{ClassName}, {MethodName}, Dental X-Ray record with Id={Id} successfully updated by user {User}",
                    CLASSNAME, methodName, model.Id, userName);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Exception occurred while updating Dental X-Ray record Id={Id} by user {User}",
                    CLASSNAME, methodName, model.Id, userName);
                throw;
            }
        }

        public DentalXRayStation MapSaveDtoToEntity(DentalXRayStationSaveDto dto, DentalXRayStation? existing = null)
        {
            var entity = existing ?? new DentalXRayStation();

            entity.Id = dto.Id;
            entity.ServiceMembersChildId = dto.ServiceMembersChildId;
            entity.BwxStatus = dto.BwxStatus;
            entity.BwxReason = dto.BwxReason;
            entity.BwxUploadMode = dto.BwxUploadMode;
            entity.BwxUploadedDateTime = dto.BwxUploadedDateTime;

            entity.BwxConsolidatedFileName = dto.BwxConsolidatedRemoved ? null : dto.BwxConsolidatedFileName;
            entity.BwxConsolidatedOriginalFileName = dto.BwxConsolidatedRemoved ? null : dto.BwxConsolidatedOriginalFileName;
            entity.BwxConsolidatedUploadedDateTime = dto.BwxConsolidatedRemoved ? null : dto.BwxConsolidatedUploadedDateTime;

            entity.BwLeftMolarFileName = dto.BwLeftMolarRemoved ? null : dto.BwLeftMolarFileName;
            entity.BwLeftMolarOriginalFileName = dto.BwLeftMolarRemoved ? null : dto.BwLeftMolarOriginalFileName;
            entity.BwLeftMolarUploadedDateTime = dto.BwLeftMolarRemoved ? null : dto.BwLeftMolarUploadedDateTime;

            entity.BwLeftPremolarFileName = dto.BwLeftPremolarRemoved ? null : dto.BwLeftPremolarFileName;
            entity.BwLeftPremolarOriginalFileName = dto.BwLeftPremolarRemoved ? null : dto.BwLeftPremolarOriginalFileName;
            entity.BwLeftPremolarUploadedDateTime = dto.BwLeftPremolarRemoved ? null : dto.BwLeftPremolarUploadedDateTime;

            entity.BwRightMolarFileName = dto.BwRightMolarRemoved ? null : dto.BwRightMolarFileName;
            entity.BwRightMolarOriginalFileName = dto.BwRightMolarRemoved ? null : dto.BwRightMolarOriginalFileName;
            entity.BwRightMolarUploadedDateTime = dto.BwRightMolarRemoved ? null : dto.BwRightMolarUploadedDateTime;

            entity.BwRightPremolarFileName = dto.BwRightPremolarRemoved ? null : dto.BwRightPremolarFileName;
            entity.BwRightPremolarOriginalFileName = dto.BwRightPremolarRemoved ? null : dto.BwRightPremolarOriginalFileName;
            entity.BwRightPremolarUploadedDateTime = dto.BwRightPremolarRemoved ? null : dto.BwRightPremolarUploadedDateTime;

            entity.PaStatus = dto.PaStatus;
            entity.PaReason = dto.PaReason;
            entity.PaUploadedDateTime = dto.PaUploadedDateTime;

            entity.Comment = dto.Comment;

            entity.PaImages = dto.PaImages
                .Where(p => !p.Removed)
                .Select((p, index) => new DentalXRayPaImage
                {
                    Id = p.Id,
                    FileName = p.FileName,
                    OriginalFileName = p.OriginalFileName,
                    UploadedDateTime = p.UploadedDateTime,
                    SortOrder = index
                })
                .ToList();

            entity.Status = dto.Status;
            return entity;
        }

        public string ComputeOverallStatus(DentalXRayStation model, ServiceMembersChild serviceMember, DentalQuestionnaire? questionnaire = null)
        {
            if (!CanProceedWithXRay(questionnaire, serviceMember))
            {
                return "Pending";
            }

            if (!IsNeeded(serviceMember.BwxNeeded))
            {
                return "Pending";
            }

            if (string.IsNullOrWhiteSpace(model.BwxStatus) || string.IsNullOrWhiteSpace(model.PaStatus))
            {
                return "Pending";
            }

            var bwxComplete = IsSectionComplete(model.BwxStatus, model.BwxReason, IsBwxUploadComplete(model));
            var paComplete = IsSectionComplete(model.PaStatus, model.PaReason, IsPaUploadComplete(model));

            return bwxComplete && paComplete ? "Completed" : "Pending";
        }

        public static bool IsFemale(ServiceMembersChild? serviceMember)
        {
            if (serviceMember?.Sex == null)
            {
                return false;
            }

            var sex = serviceMember.Sex.Trim();
            return sex.Equals("F", StringComparison.OrdinalIgnoreCase)
                || sex.Equals("Female", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsNeeded(string? value)
        {
            return string.Equals(value?.Trim(), AppConstants.NeededOrNA.Needed, StringComparison.OrdinalIgnoreCase);
        }

        public static bool CanProceedWithXRay(DentalQuestionnaire? questionnaire, ServiceMembersChild serviceMember)
        {
            if (!IsFemale(serviceMember))
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(questionnaire?.AreYouPregnant))
            {
                return false;
            }

            if (questionnaire.AreYouPregnant.Equals("No", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return questionnaire.AreYouPregnant.Equals("Yes", StringComparison.OrdinalIgnoreCase)
                && questionnaire.PregnancyApproval != null
                && questionnaire.PregnancyApproval.Equals("Approved", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsSectionComplete(string? status, string? reason, bool uploadsComplete)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return false;
            }

            if (status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
            {
                return uploadsComplete;
            }

            if (status.Equals("Not Completed", StringComparison.OrdinalIgnoreCase))
            {
                return !string.IsNullOrWhiteSpace(reason);
            }

            return false;
        }

        private static bool IsBwxUploadComplete(DentalXRayStation model)
        {
            if (string.Equals(model.BwxUploadMode, BwxUploadMode.Consolidated, StringComparison.OrdinalIgnoreCase))
            {
                return !model.BwxConsolidatedFileName.IsNullOrEmpty();
            }

            if (string.Equals(model.BwxUploadMode, BwxUploadMode.Separate, StringComparison.OrdinalIgnoreCase))
            {
                return HasAllSeparateBwxUploads(model);
            }

            if (!model.BwxConsolidatedFileName.IsNullOrEmpty())
            {
                return true;
            }

            return HasAllSeparateBwxUploads(model);
        }

        private static bool HasAllSeparateBwxUploads(DentalXRayStation model)
        {
            return !model.BwLeftMolarFileName.IsNullOrEmpty()
                && !model.BwLeftPremolarFileName.IsNullOrEmpty()
                && !model.BwRightMolarFileName.IsNullOrEmpty()
                && !model.BwRightPremolarFileName.IsNullOrEmpty();
        }

        private static bool IsPaUploadComplete(DentalXRayStation model)
        {
            return model.PaImages != null
                && model.PaImages.Any()
                && model.PaImages.All(p => !p.FileName.IsNullOrEmpty());
        }

        private static bool IsYes(string? value)
        {
            return string.Equals(value?.Trim(), "Yes", StringComparison.OrdinalIgnoreCase);
        }

        private void MapToEntity(DentalXRayStation source, DentalXRayStation target, string userName)
        {
            MapBwxSection(source, target);
            MapPaSection(source, target);

            target.Comment = source.Comment;
            target.Status = source.Status;
            target.UpdatedOn = DateTime.Now;
            target.UpdatedBy = userName;
        }

        private static void MapBwxSection(DentalXRayStation source, DentalXRayStation target)
        {
            target.BwxStatus = source.BwxStatus;
            target.BwxReason = source.BwxReason;
            target.BwxUploadMode = source.BwxUploadMode;

            if (source.BwxStatus.IsNullOrEmpty())
            {
                ClearBwxUploads(target);
                target.BwxUploadedDateTime = null;
                return;
            }

            if (source.BwxStatus == "Not Completed")
            {
                ClearBwxUploads(target);
                target.BwxReason = source.BwxReason;
                target.BwxUploadedDateTime = null;
                return;
            }

            target.BwxReason = null;

            if (string.Equals(source.BwxUploadMode, BwxUploadMode.Consolidated, StringComparison.OrdinalIgnoreCase))
            {
                ClearSeparateBwxUploads(target);
                target.BwxConsolidatedFileName = source.BwxConsolidatedFileName;
                target.BwxConsolidatedOriginalFileName = source.BwxConsolidatedOriginalFileName;
                target.BwxConsolidatedUploadedDateTime = source.BwxConsolidatedUploadedDateTime;
            }
            else
            {
                ClearConsolidatedBwxUploads(target);
                target.BwLeftMolarFileName = source.BwLeftMolarFileName;
                target.BwLeftMolarOriginalFileName = source.BwLeftMolarOriginalFileName;
                target.BwLeftMolarUploadedDateTime = source.BwLeftMolarUploadedDateTime;
                target.BwLeftPremolarFileName = source.BwLeftPremolarFileName;
                target.BwLeftPremolarOriginalFileName = source.BwLeftPremolarOriginalFileName;
                target.BwLeftPremolarUploadedDateTime = source.BwLeftPremolarUploadedDateTime;
                target.BwRightMolarFileName = source.BwRightMolarFileName;
                target.BwRightMolarOriginalFileName = source.BwRightMolarOriginalFileName;
                target.BwRightMolarUploadedDateTime = source.BwRightMolarUploadedDateTime;
                target.BwRightPremolarFileName = source.BwRightPremolarFileName;
                target.BwRightPremolarOriginalFileName = source.BwRightPremolarOriginalFileName;
                target.BwRightPremolarUploadedDateTime = source.BwRightPremolarUploadedDateTime;
            }

            target.BwxUploadedDateTime = IsBwxUploadComplete(source) ? source.BwxUploadedDateTime : null;
        }

        private void MapPaSection(DentalXRayStation source, DentalXRayStation target)
        {
            target.PaStatus = source.PaStatus;
            target.PaReason = source.PaReason;

            if (source.PaStatus.IsNullOrEmpty())
            {
                target.PaUploadedDateTime = null;
                ReplacePaImages(target, new List<DentalXRayPaImage>());
                return;
            }

            if (source.PaStatus == "Not Completed")
            {
                target.PaReason = source.PaReason;
                target.PaUploadedDateTime = null;
                ReplacePaImages(target, new List<DentalXRayPaImage>());
                return;
            }

            target.PaReason = null;
            ReplacePaImages(target, source.PaImages?.ToList() ?? new List<DentalXRayPaImage>());
            target.PaUploadedDateTime = IsPaUploadComplete(source) ? source.PaUploadedDateTime : null;
        }

        private static void ClearBwxUploads(DentalXRayStation target)
        {
            ClearSeparateBwxUploads(target);
            ClearConsolidatedBwxUploads(target);
            target.BwxUploadMode = null;
            target.BwxReason = null;
        }

        private static void ClearSeparateBwxUploads(DentalXRayStation target)
        {
            target.BwLeftMolarFileName = null;
            target.BwLeftMolarOriginalFileName = null;
            target.BwLeftMolarUploadedDateTime = null;
            target.BwLeftPremolarFileName = null;
            target.BwLeftPremolarOriginalFileName = null;
            target.BwLeftPremolarUploadedDateTime = null;
            target.BwRightMolarFileName = null;
            target.BwRightMolarOriginalFileName = null;
            target.BwRightMolarUploadedDateTime = null;
            target.BwRightPremolarFileName = null;
            target.BwRightPremolarOriginalFileName = null;
            target.BwRightPremolarUploadedDateTime = null;
        }

        private static void ClearConsolidatedBwxUploads(DentalXRayStation target)
        {
            target.BwxConsolidatedFileName = null;
            target.BwxConsolidatedOriginalFileName = null;
            target.BwxConsolidatedUploadedDateTime = null;
        }

        private void ReplacePaImages(DentalXRayStation target, List<DentalXRayPaImage> newImages)
        {
            target.PaImages ??= new List<DentalXRayPaImage>();
            var existingImages = target.PaImages.ToList();
            if (existingImages.Count > 0)
            {
                _unitOfWork.DentalXRayPaImage.RemoveRange(existingImages);
                target.PaImages.Clear();
            }

            foreach (var (image, index) in newImages.Select((img, i) => (img, i)))
            {
                target.PaImages.Add(new DentalXRayPaImage
                {
                    DentalXRayStationId = target.Id,
                    FileName = image.FileName,
                    OriginalFileName = image.OriginalFileName,
                    UploadedDateTime = image.UploadedDateTime,
                    SortOrder = index
                });
            }
        }

        private static void NormalizePaImages(DentalXRayStation model)
        {
            if (model.PaImages == null)
            {
                model.PaImages = new List<DentalXRayPaImage>();
                return;
            }

            var ordered = model.PaImages.OrderBy(p => p.SortOrder).ToList();
            for (var i = 0; i < ordered.Count; i++)
            {
                ordered[i].SortOrder = i;
            }

            model.PaImages = ordered;
        }
    }
}
