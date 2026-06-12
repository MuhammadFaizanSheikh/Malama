using System.ComponentModel.DataAnnotations;

namespace Malama.Models
{
    public class VitalStationVM
    {
        public long EventId { get; set; }
        public string? EventID { get; set; }
        public ServiceMembersChildDto? ServiceMembersChild { get; set; }
        public VitalStationDto VitalStationDto { get; set; }
        public string SubmissionToken { get; set; }

        /// <summary>When true, successful save redirects back to the Dental X-Ray station page.</summary>
        public bool ReturnToDentalXRay { get; set; }

        /// <summary>Dental X-Ray record id to return to (0 for new records).</summary>
        public long DentalXRayStationId { get; set; }
    }

    public class VitalStationBpReadingDto
    {
        public long Id { get; set; }

        public int ReadingNumber { get; set; }

        public int Systolic { get; set; }

        public int Diastolic { get; set; }

        public string ReadingStatus { get; set; }

        public bool IsRetakeRequired { get; set; }

        public DateTime ReadingTakenAt { get; set; }

        public string? Remarks { get; set; }
    }

    public class VitalStationDto
    {
        public long Id { get; set; }

        public long ServiceMembersChildId { get; set; }

        [Required(ErrorMessage = "Height is required.")]
        public decimal? Height { get; set; }

        [Required(ErrorMessage = "Weight is required.")]
        public decimal? Weight { get; set; }

        public string? FinalBpStatus { get; set; }

        public int TotalReadingsTaken { get; set; }

        public string Status { get; set; } = "Pending";

        public bool IsNextReadingRequired { get; set; }

        public int? NextReadingNumber { get; set; }

        public int? NextReadingAfterMinutes { get; set; }

        public string? Message { get; set; }

        /// <summary>Local time when the next BP attempt may be submitted (15 minutes after the previous reading).</summary>
        public DateTime? NextBpReadingAvailableAt { get; set; }

        public bool NextBpReadingUnlocked { get; set; }

        /// <summary>Posted only for the active attempt; not persisted.</summary>
        public int? PendingSystolic { get; set; }

        /// <summary>Posted only for the active attempt; not persisted.</summary>
        public int? PendingDiastolic { get; set; }

        public List<VitalStationBpReadingDto> BloodPressureReadings { get; set; } = new();
    }
}
