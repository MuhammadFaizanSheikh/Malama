namespace Malama.Models
{
    public class PostEventLabStation : GenericProperties
    {
        public long Id { get; set; }

        public long ServiceMembersChildId { get; set; }
        public ServiceMembersChild ServiceMembersChild { get; set; }
        public long PostEventManagementId { get; set; }
        public PostEventManagement PostEventManagement { get; set; }

        // G6PD
        public bool G6pdResultReceived { get; set; }
        public string? G6pdResultReason { get; set; }
        public DateTime? G6pdResultReceivedDateTime { get; set; }
        public bool G6pdResultMalamaUploaded { get; set; }
        public string? G6pdResultMalamaUploadedFileName { get; set; }
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
        public string? AboResultMalamaUploadedFileName { get; set; }
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
        public string? HivResultMalamaUploadedFileName { get; set; }
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
        public string? PregnancyResultMalamaUploadedFileName { get; set; }
        public DateTime? PregnancyResultMalamaUploadedDateTime { get; set; }
        public bool PregnancyResultEMRUploaded { get; set; }
        public DateTime? PregnancyResultEMRUploadedDateTime { get; set; }
        public bool PregnancyResultSORUploaded { get; set; }
        public DateTime? PregnancyResultSORUploadedDateTime { get; set; }

        // LIPID PANEL
        public bool LipidPanelResultReceived { get; set; }
        public string? LipidPanelResultReason { get; set; }
        public DateTime? LipidPanelResultReceivedDateTime { get; set; }
        public bool LipidPanelResultMalamaUploaded { get; set; }
        public string? LipidPanelResultMalamaUploadedFileName { get; set; }
        public DateTime? LipidPanelResultMalamaUploadedDateTime { get; set; }
        public bool LipidPanelResultEMRUploaded { get; set; }
        public DateTime? LipidPanelResultEMRUploadedDateTime { get; set; }
        public bool LipidPanelResultSORUploaded { get; set; }
        public DateTime? LipidPanelResultSORUploadedDateTime { get; set; }

        // SICKLE CELL
        public bool SickleCellResultReceived { get; set; }
        public string? SickleCellResultReason { get; set; }
        public DateTime? SickleCellResultReceivedDateTime { get; set; }
        public bool SickleCellResultMalamaUploaded { get; set; }
        public string? SickleCellResultMalamaUploadedFileName { get; set; }
        public DateTime? SickleCellResultMalamaUploadedDateTime { get; set; }
        public bool SickleCellResultEMRUploaded { get; set; }
        public DateTime? SickleCellResultEMRUploadedDateTime { get; set; }
        public bool SickleCellResultSORUploaded { get; set; }
        public DateTime? SickleCellResultSORUploadedDateTime { get; set; }

        // DNA
        public bool DNAResultReceived { get; set; }
        public string? DNAResultReason { get; set; }
        public DateTime? DNAResultReceivedDateTime { get; set; }
        public bool DNAResultMalamaUploaded { get; set; }
        public string? DNAResultMalamaUploadedFileName { get; set; }
        public DateTime? DNAResultMalamaUploadedDateTime { get; set; }
        public bool DNAResultEMRUploaded { get; set; }
        public DateTime? DNAResultEMRUploadedDateTime { get; set; }
        public bool DNAResultSORUploaded { get; set; }
        public DateTime? DNAResultSORUploadedDateTime { get; set; }

        public string Status { get; set; }
    }
}
