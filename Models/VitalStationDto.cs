namespace Malama.Models
{
    public class VitalStationVM
    {
        public long EventId { get; set; }
        public string? EventID { get; set; }
        public ServiceMembersChildDto ServiceMember { get; set; }
        public VitalStationDto VitalStationDto { get; set; }
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

        public decimal? Height { get; set; }

        public decimal? Weight { get; set; }

        public string FinalBpStatus { get; set; }

        public int TotalReadingsTaken { get; set; }

        public string Status { get; set; }

        public bool IsNextReadingRequired { get; set; }

        public int? NextReadingNumber { get; set; }

        public int? NextReadingAfterMinutes { get; set; }

        public string? Message { get; set; }

        public List<VitalStationBpReadingDto> BloodPressureReadings { get; set; }
    }
}
