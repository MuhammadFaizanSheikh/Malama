namespace Malama.Models
{
    public class PostEventLabStationDto
    {
        public long Id { get; set; }
        public long ServiceMembersChildId { get; set; }

        public string Status { get; set; }

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
