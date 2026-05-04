using ExcelFilesCompiler.Utilities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

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
        public string? G6pdNeeded { get; set; }
        public string? AboNeeded { get; set; }
        public string? HivNeeded { get; set; }
        public string? PregnancyTestNeeded { get; set; }
        public string? LipidPanelNeeded { get; set; }

        public DateTime? G6pdGivenDateTime { get; set; }
        public DateTime? AboGivenDateTime { get; set; }
        public DateTime? HivGivenDateTime { get; set; }
        public DateTime? PregnancyTestGivenDateTime { get; set; }
        public DateTime? LipidPanelGivenDateTime { get; set; }

        public bool? LipidPanelRapidTesting { get; set; }
        public string? HivBarcodeCarebill { get; set; }
        public string? FedExTrackingNo { get; set; }
        public string? PregnancyTestResult { get; set; }
    }

    public class PostEventLabStationAnalysisDto
    {
        public long ServiceMembersChildId { get; set; }
        public long PostEventManagementId { get; set; }

        public string? EventID { get; set; }

        [ValidateNever]
        public ServiceMembersChildDto ServiceMember { get; set; }

        [ValidateNever]
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
        public string? HivBarcodeCarebill => LabStation?.HivBarcodeCarebill;
        public string? FedExTrackingNo => LabStation?.FedExTrackingNo;
        public string? PregnancyTestResult => LabStation?.PregnancyTestResult;
    }

    public class PostEventLabStationDto
    {
        public long Id { get; set; }
        public long ServiceMembersChildId { get; set; }
        public long PostEventManagementId { get; set; }

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

        // Lipid

        public bool LipidPanelResultReceived { get; set; }
        public string? LipidPanelResultReason { get; set; }
        public DateTime? LipidPanelResultReceivedDateTime { get; set; }
        public bool LipidPanelResultMalamaUploaded { get; set; }
        public DateTime? LipidPanelResultMalamaUploadedDateTime { get; set; }
        public bool LipidPanelResultEMRUploaded { get; set; }
        public DateTime? LipidPanelResultEMRUploadedDateTime { get; set; }
        public bool LipidPanelResultSORUploaded { get; set; }
        public DateTime? LipidPanelResultSORUploadedDateTime { get; set; }

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

    public class LabUIModel
    {
        public string Prefix { get; set; }
        public string Title { get; set; }

        public DateTime? GivenDate { get; set; }

        public string ResultReceivedName { get; set; }
        public string ReasonName { get; set; }
        public string ReceivedDateName { get; set; }
        public string MalamaDateName { get; set; }
        public string EMRName { get; set; }
        public string EMRDateName { get; set; }
        public string SORName { get; set; }
        public string SORDateName { get; set; }
        public string IsLipidRapidName { get; set; }
        public string HivBarcodeCarebillName { get; set; }
        public string FedExTrackingNoName { get; set; }
        public string PregnancyTestResultName { get; set; }

        public bool ResultReceived { get; set; }
        public string? Reason { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? MalamaDate { get; set; }
        public bool EMR { get; set; }
        public DateTime? EMRDate { get; set; }
        public bool SOR { get; set; }
        public DateTime? SORDate { get; set; }
        public bool? IsLipidRapid { get; set; }
        public string? HivBarcodeCarebill { get; set; }
        public string? PregnancyTestResult { get; set; }
    }
}
