using ExcelFilesCompiler.Utilities;

namespace Malama.Models
{
    public class ServiceMembersChildDto
    {
        public string FullName { get; set; }
        public string DodId { get; set; }
        public string Barcode { get; set; }

        public string Dob { get; set; }
        public int? Age { get; set; }
        public string Sex { get; set; }
    }

    public class LabStationDto
    {
        public string G6pdNeeded { get; set; }
        public string AboNeeded { get; set; }
        public string HivNeeded { get; set; }
        public string PregnancyTestNeeded { get; set; }
        public string LipidPanelNeeded { get; set; }

        public DateTime? G6pdGivenDateTime { get; set; }
        public DateTime? AboGivenDateTime { get; set; }
        public DateTime? HivGivenDateTime { get; set; }
        public DateTime? PregnancyTestGivenDateTime { get; set; }
        public DateTime? LipidPanelGivenDateTime { get; set; }

        public bool? LipidPanelRapidTesting { get; set; }
        public string? HivBarcodeCarebill { get; set; }
        public string? FedExTrackingNo { get; set; }
    }

    public class PostEventLabStationAnalysisDto
    {
        public long ServiceMembersChildId { get; set; }

        public string EventID { get; set; }

        // 🔹 Display
        public ServiceMembersChildDto ServiceMember { get; set; }

        // 🔹 Pre-Lab (logic only)
        public LabStationDto LabStation { get; set; }

        // 🔹 Editable (never null)
        public PostEventLabStationDto PostEventLabStation { get; set; } = new();

        // 🔹 Section Visibility
        public bool ShowG6pdSection => LabStation?.G6pdNeeded == AppConstants.Status.Completed;
        public bool ShowAboSection => LabStation?.AboNeeded == AppConstants.Status.Completed;
        public bool ShowHivSection => LabStation?.HivNeeded == AppConstants.Status.Completed;
        public bool ShowPregnancySection => LabStation?.PregnancyTestNeeded == AppConstants.Status.Completed;
        public bool ShowLipidPanelSection => LabStation?.LipidPanelNeeded == AppConstants.Status.Completed;

        // 🔹 Given DateTimes
        public DateTime? G6pdGivenDateTime => LabStation?.G6pdGivenDateTime;
        public DateTime? AboGivenDateTime => LabStation?.AboGivenDateTime;
        public DateTime? HivGivenDateTime => LabStation?.HivGivenDateTime;
        public DateTime? PregnancyGivenDateTime => LabStation?.PregnancyTestGivenDateTime;
        public DateTime? LipidPanelGivenDateTime => LabStation?.LipidPanelGivenDateTime;

        // 🔹 Extra Info
        public bool? IsLipidRapid => LabStation?.LipidPanelRapidTesting;
        public string? HivBarcode => LabStation?.HivBarcodeCarebill;
        public string? FedExTrackingNo => LabStation?.FedExTrackingNo;

        // 🔹 Optional formatted helpers (very useful in UI)
        public string G6pdGivenDateTimeText => G6pdGivenDateTime?.ToString("dd-MMM-yyyy HH:mm") ?? "N/A";
    }

    public class PostEventLabStationDto
    {
        public long Id { get; set; }
        public long ServiceMembersChildId { get; set; }

        public string Status { get; set; } = AppConstants.Status.Pending;

        // G6PD
        public bool G6pdResultReceived { get; set; }
        public string? G6pdResultReason { get; set; }
        public DateTime? G6pdResultReceivedDateTime { get; set; }
        public bool G6pdResultMalamaUploaded { get; set; }
        public DateTime? G6pdResultMalamaUploadedDateTime { get; set; }
        public bool G6pdResultEMRUploaded { get; set; }
        public DateTime? G6pdResultEMRUploadedDateTime { get; set; }
        public bool G6pdResultSORUploaded { get; set; }
        public DateTime? G6pdResultSORUploadedDateTime { get; set; }

        // ABO
        public bool AboResultReceived { get; set; }
        public string? AboResultReason { get; set; }
        public DateTime? AboResultReceivedDateTime { get; set; }
        public bool AboResultMalamaUploaded { get; set; }
        public DateTime? AboResultMalamaUploadedDateTime { get; set; }
        public bool AboResultEMRUploaded { get; set; }
        public DateTime? AboResultEMRUploadedDateTime { get; set; }
        public bool AboResultSORUploaded { get; set; }
        public DateTime? AboResultSORUploadedDateTime { get; set; }

        // HIV
        public bool HivResultReceived { get; set; }
        public string? HivResultReason { get; set; }
        public DateTime? HivResultReceivedDateTime { get; set; }
        public bool HivResultMalamaUploaded { get; set; }
        public DateTime? HivResultMalamaUploadedDateTime { get; set; }
        public bool HivResultEMRUploaded { get; set; }
        public DateTime? HivResultEMRUploadedDateTime { get; set; }
        public bool HivResultSORUploaded { get; set; }
        public DateTime? HivResultSORUploadedDateTime { get; set; }

        // Pregnancy
        public bool PregnancyResultReceived { get; set; }
        public string? PregnancyResultReason { get; set; }
        public DateTime? PregnancyResultReceivedDateTime { get; set; }
        public bool PregnancyResultMalamaUploaded { get; set; }
        public DateTime? PregnancyResultMalamaUploadedDateTime { get; set; }
        public bool PregnancyResultEMRUploaded { get; set; }
        public DateTime? PregnancyResultEMRUploadedDateTime { get; set; }
        public bool PregnancyResultSORUploaded { get; set; }
        public DateTime? PregnancyResultSORUploadedDateTime { get; set; }
    }
}
