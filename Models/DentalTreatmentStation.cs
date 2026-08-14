namespace Malama.Models
{
    public class DentalTreatmentStationPageViewModel
    {
        public ServiceMembersChild ServiceMember { get; set; } = new();

        public DentalQuestionnaire Questionnaire { get; set; } = new();

        public DentalXRayStation XRayStation { get; set; } = new();

        public DentalExam DentalExam { get; set; } = new();

        public DentalTreatment? DentalTreatment { get; set; }
    }

    public class DentalTreatmentStationSaveDto
    {
        public long ServiceMembersChildId { get; set; }

        public long DentalExamId { get; set; }

        public string? SmFinalClassification { get; set; }

        public string? Status { get; set; }

        public List<int> PsrSelectedTeeth { get; set; } = new();

        public string? FindingsJson { get; set; }

        public string? AnesthesiaJson { get; set; }

        public string? PrescriptionsJson { get; set; }

        public string? OverallNotesJson { get; set; }

        public List<DentalTreatmentFindingFormDto> Findings { get; set; } = new();

        public List<DentalTreatmentAnesthesiaDto> AnesthesiaRecords { get; set; } = new();

        public List<DentalTreatmentPrescriptionDto> Prescriptions { get; set; } = new();

        public List<DentalTreatmentOverallNoteDto> OverallNotes { get; set; } = new();
    }

    public class DentalTreatmentFindingFormDto
    {
        public long Id { get; set; }

        public long? DentalExamFindingId { get; set; }

        /// <summary>
        /// <see cref="DentalTreatmentFindingOrigin.Exam"/> or <see cref="DentalTreatmentFindingOrigin.Treatment"/>.
        /// </summary>
        public string Origin { get; set; } = DentalTreatmentFindingOrigin.Exam;

        public bool IsTreatmentOnly { get; set; }

        public bool IsPrimaryTooth { get; set; }

        public string? AffectedTooth { get; set; }

        public string? DiseaseConditionType { get; set; }

        public List<string> AffectedSurfaces { get; set; } = new();

        public List<string> CdtCodes { get; set; } = new();

        public string? CdtCodesNotes { get; set; }

        public string? DescriptionDetails { get; set; }

        public string? Classification { get; set; }

        public int SortOrder { get; set; }

        public string? ExaminationAddedBy { get; set; }

        public DateTime? ExaminationAddedOn { get; set; }

        public string? ExaminationUpdatedBy { get; set; }

        public DateTime? ExaminationUpdatedOn { get; set; }

        public string? TreatmentCompleted { get; set; }

        public List<string> PostServiceTreatment { get; set; } = new();

        public List<string> TreatmentCdtCodes { get; set; } = new();

        public string? Reason { get; set; }

        public string? Notes { get; set; }

        public string? FinalDrc { get; set; }

        public string? DentistProfessional { get; set; }

        public string? TreatmentStatus { get; set; }

        public string? TreatmentDateTime { get; set; }

        public string? FindingDateTime { get; set; }
    }

    public class DentalTreatmentAnesthesiaDto
    {
        public long Id { get; set; }

        public string? Date { get; set; }

        public Dictionary<string, string> CarpulesByType { get; set; } = new();

        public int SortOrder { get; set; }
    }

    public class DentalTreatmentPrescriptionDto
    {
        public long Id { get; set; }

        public string? Type { get; set; }

        public string? Product { get; set; }

        public string? StartDate { get; set; }

        public string? EndDate { get; set; }

        public string? Dosage { get; set; }

        public string? Duration { get; set; }

        public string? Frequency { get; set; }

        public string? PrescribedAmount { get; set; }

        public string? Notes { get; set; }

        public string? PrescribedBy { get; set; }

        public string? PrescribedOn { get; set; }

        public int SortOrder { get; set; }
    }

    public class DentalTreatmentOverallNoteDto
    {
        public long Id { get; set; }

        public string Notes { get; set; } = string.Empty;

        public string? Dentist { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("dateTime")]
        public string? NoteDateTime { get; set; }

        public int SortOrder { get; set; }
    }
}
