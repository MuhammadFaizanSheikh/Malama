using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Malama.Models
{
    public class DentalXRayStationViewModel
    {
        public List<ServiceMembersChild> FileDataList { get; set; } = new();
    }

    public class DentalXRayStation : GenericProperties
    {
        public long Id { get; set; }

        public long ServiceMembersChildId { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public virtual ServiceMembersChild ServiceMembersChild { get; set; }

        public string? AreYouPregnant { get; set; }
        public string? PregnancyApproval { get; set; }

        public string? BwxStatus { get; set; }
        public string? BwxReason { get; set; }
        public DateTime? BwxUploadedDateTime { get; set; }

        public string? BwLeftMolarFileName { get; set; }
        public string? BwLeftMolarOriginalFileName { get; set; }
        public DateTime? BwLeftMolarUploadedDateTime { get; set; }

        public string? BwLeftPremolarFileName { get; set; }
        public string? BwLeftPremolarOriginalFileName { get; set; }
        public DateTime? BwLeftPremolarUploadedDateTime { get; set; }

        public string? BwRightMolarFileName { get; set; }
        public string? BwRightMolarOriginalFileName { get; set; }
        public DateTime? BwRightMolarUploadedDateTime { get; set; }

        public string? BwRightPremolarFileName { get; set; }
        public string? BwRightPremolarOriginalFileName { get; set; }
        public DateTime? BwRightPremolarUploadedDateTime { get; set; }

        public string? PaStatus { get; set; }
        public string? PaReason { get; set; }
        public DateTime? PaUploadedDateTime { get; set; }

        [ValidateNever]
        public virtual ICollection<DentalXRayPaImage> PaImages { get; set; } = new List<DentalXRayPaImage>();

        public string? PanoStatus { get; set; }
        public string? PanoReason { get; set; }
        public string? PanoFileName { get; set; }
        public string? PanoOriginalFileName { get; set; }
        public DateTime? PanoUploadedDateTime { get; set; }

        public string Status { get; set; } = "Pending";
    }

    public class DentalXRayPaImage
    {
        public long Id { get; set; }
        public long DentalXRayStationId { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public virtual DentalXRayStation DentalXRayStation { get; set; }

        public string? FileName { get; set; }
        public string? OriginalFileName { get; set; }
        public DateTime? UploadedDateTime { get; set; }
        public int SortOrder { get; set; }
    }

    public class DentalXRayImageUploadModel
    {
        public string Prefix { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string UploadedFieldName { get; set; } = string.Empty;
        public string FileNameFieldName { get; set; } = string.Empty;
        public string OriginalFileNameFieldName { get; set; } = string.Empty;
        public string DateFieldName { get; set; } = string.Empty;
        public string FileInputName { get; set; } = string.Empty;
        public bool Uploaded { get; set; }
        public string? StoredFileName { get; set; }
        public string? OriginalFileName { get; set; }
        public DateTime? UploadedDateTime { get; set; }
        public string RemovedFieldName { get; set; } = string.Empty;
    }

    public class DentalXRayPaImageDto
    {
        public long Id { get; set; }
        public bool Uploaded { get; set; }
        public string? FileName { get; set; }
        public string? OriginalFileName { get; set; }
        public DateTime? UploadedDateTime { get; set; }
        public int SortOrder { get; set; }
        public IFormFile? ImageFile { get; set; }
        public bool Removed { get; set; }
    }

    public class DentalXRayStationSaveDto
    {
        public long Id { get; set; }
        public long ServiceMembersChildId { get; set; }
        public string? AreYouPregnant { get; set; }
        public string? PregnancyApproval { get; set; }
        public string? BwxStatus { get; set; }
        public string? BwxReason { get; set; }
        public DateTime? BwxUploadedDateTime { get; set; }

        public bool BwLeftMolarUploaded { get; set; }
        public string? BwLeftMolarFileName { get; set; }
        public string? BwLeftMolarOriginalFileName { get; set; }
        public DateTime? BwLeftMolarUploadedDateTime { get; set; }
        public IFormFile? BwLeftMolarFile { get; set; }

        public bool BwLeftPremolarUploaded { get; set; }
        public string? BwLeftPremolarFileName { get; set; }
        public string? BwLeftPremolarOriginalFileName { get; set; }
        public DateTime? BwLeftPremolarUploadedDateTime { get; set; }
        public IFormFile? BwLeftPremolarFile { get; set; }

        public bool BwRightMolarUploaded { get; set; }
        public string? BwRightMolarFileName { get; set; }
        public string? BwRightMolarOriginalFileName { get; set; }
        public DateTime? BwRightMolarUploadedDateTime { get; set; }
        public IFormFile? BwRightMolarFile { get; set; }

        public bool BwRightPremolarUploaded { get; set; }
        public string? BwRightPremolarFileName { get; set; }
        public string? BwRightPremolarOriginalFileName { get; set; }
        public DateTime? BwRightPremolarUploadedDateTime { get; set; }
        public IFormFile? BwRightPremolarFile { get; set; }

        public string? PaStatus { get; set; }
        public string? PaReason { get; set; }
        public DateTime? PaUploadedDateTime { get; set; }
        public List<DentalXRayPaImageDto> PaImages { get; set; } = new();

        public string? PanoStatus { get; set; }
        public string? PanoReason { get; set; }
        public bool PanoUploaded { get; set; }
        public string? PanoFileName { get; set; }
        public string? PanoOriginalFileName { get; set; }
        public DateTime? PanoUploadedDateTime { get; set; }
        public IFormFile? PanoFile { get; set; }

        public string Status { get; set; } = "Pending";
        public string? SubmissionToken { get; set; }

        public bool BwLeftMolarRemoved { get; set; }
        public bool BwLeftPremolarRemoved { get; set; }
        public bool BwRightMolarRemoved { get; set; }
        public bool BwRightPremolarRemoved { get; set; }
        public bool PanoRemoved { get; set; }
    }
}
