namespace Malama.Models
{
    public class VitalStation : GenericProperties
    {
        public long Id { get; set; }

        public virtual ServiceMembersChild ServiceMembersChild { get; set; }
        public long ServiceMembersChildId { get; set; }

        public decimal? Height { get; set; }

        public decimal? Weight { get; set; }

        public string? FinalBpStatus { get; set; }

        public int TotalReadingsTaken { get; set; }

        public string Status { get; set; }

        public ICollection<VitalStationBloodPressureReading> BloodPressureReadings { get; set; }
    }

    public class VitalStationBloodPressureReading
    {
        public long Id { get; set; }

        public long VitalStationId { get; set; }

        public int ReadingNumber { get; set; }

        public int Systolic { get; set; }

        public int Diastolic { get; set; }

        public string ReadingStatus { get; set; }

        public bool IsRetakeRequired { get; set; }

        public DateTime ReadingTakenAt { get; set; }

        public string? Remarks { get; set; }

        public VitalStation VitalStation { get; set; }
    }
}
