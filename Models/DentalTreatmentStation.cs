namespace Malama.Models
{
    public class DentalTreatmentStationPageViewModel
    {
        public ServiceMembersChild ServiceMember { get; set; } = new();

        public DentalQuestionnaire Questionnaire { get; set; } = new();

        public DentalXRayStation XRayStation { get; set; } = new();

        public DentalExam DentalExam { get; set; } = new();

        public DentalSharedClinicalViewModel SharedClinical { get; set; } = new();

        public DentalTreatment? DentalTreatment { get; set; }

        /// <summary>Pending or Completed for the logged-in dentist.</summary>
        public string DentistStatus { get; set; } = "Pending";

        /// <summary>Exam finding Ids scheduled to the logged-in dentist for this service member.</summary>
        public HashSet<long> AssignedExamFindingIds { get; set; } = new();

        /// <summary>Appointments for the logged-in dentist with their scheduled exam finding Ids.</summary>
        public List<DentalTreatmentDentistAppointmentGroupDto> DentistAppointmentGroups { get; set; } = new();

        /// <summary>True when a DentalQuestionnaire row exists for this service member.</summary>
        public bool HasQuestionnaire { get; set; }

        /// <summary>Consent form rows for the logged-in dentist only.</summary>
        public List<TreatmentCoordinatorConsentFormStatusItem> ConsentFormStatuses { get; set; } = new();

        /// <summary>True when this dentist has a Dental Treatment Consent Form (DDS ack UI applies).</summary>
        public bool ShowDdsAcknowledgement { get; set; }

        /// <summary>
        /// True when this dentist's Dental Treatment Consent Form is signed by the service member
        /// (DDS checkbox may be checked only when this is true).
        /// </summary>
        public bool CanAcknowledgeDds { get; set; }

        public bool DdsAcknowledged { get; set; }

        public string? DdsAcknowledgedDisplayName { get; set; }

        public string? DdsAcknowledgedRoles { get; set; }

        public DateTime? DdsAcknowledgedOn { get; set; }
    }

    public class DentalTreatmentDentistAppointmentGroupDto
    {
        public long AppointmentId { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string AppointmentStartTime { get; set; } = string.Empty;

        public string AppointmentDuration { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        public List<long> ExamFindingIds { get; set; } = new();

        public string DisplayLabel
        {
            get
            {
                var datePart = AppointmentDate.ToString("MM/dd/yyyy");
                var timePart = string.IsNullOrWhiteSpace(AppointmentStartTime)
                    ? string.Empty
                    : AppointmentStartTime.Trim();
                var durationPart = string.IsNullOrWhiteSpace(AppointmentDuration)
                    ? string.Empty
                    : $" ({AppointmentDuration.Trim()})";

                if (string.IsNullOrWhiteSpace(timePart))
                {
                    return $"Appointment — {datePart}{durationPart}";
                }

                return $"Appointment — {datePart} at {timePart}{durationPart}";
            }
        }
    }

    public class DentalTreatmentStationSaveDto
    {
        public long ServiceMembersChildId { get; set; }

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

        /// <summary>True when the logged-in dentist has a Dental Treatment Consent Form.</summary>
        public bool RequiresDdsAcknowledgement { get; set; }

        public bool DdsAcknowledged { get; set; }
    }

    public class DentalTreatmentFindingFormDto
    {
        public long Id { get; set; }

        public long? DentalFindingId { get; set; }

        /// <summary>
        /// <see cref="DentalTreatmentFindingOrigin.Exam"/> or <see cref="DentalTreatmentFindingOrigin.Treatment"/>.
        /// </summary>
        public string Origin { get; set; } = DentalTreatmentFindingOrigin.Exam;

        /// <summary>Copied from <see cref="DentalFinding.Source"/> for exam-linked rows.</summary>
        public string? Source { get; set; }

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

        public long? AppointmentId { get; set; }

        public string? AppointmentLabel { get; set; }
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
