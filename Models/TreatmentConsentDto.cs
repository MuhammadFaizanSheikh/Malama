using ExcelFilesCompiler.Utilities;

namespace Malama.Models
{
    public class TreatmentConsentListItemDto
    {
        public long ServiceMembersChildId { get; set; }
        public long? SmId { get; set; }
        public string? Drc { get; set; }
        public string? FullName { get; set; }
        public string? Last4 { get; set; }
        public string? DodId { get; set; }
        public string? Sex { get; set; }
        public string? Dob { get; set; }
        public string? Barcode { get; set; }
        public string? CheckInBy { get; set; }
        public DateTime? CheckInTime { get; set; }
        public string Status { get; set; } = AppConstants.Status.Pending;
    }

    public class TreatmentConsentIndexViewModel
    {
        public string? EventId { get; set; }
        public int TotalCount { get; set; }
        public List<TreatmentConsentListItemDto> ServiceMembers { get; set; } = new();
    }

    public class TreatmentConsentStationViewModel
    {
        public ServiceMembersChild ServiceMember { get; set; } = new();
        public DentalQuestionnaire Questionnaire { get; set; } = new();
    }
}
