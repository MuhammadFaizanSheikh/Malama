using Microsoft.AspNetCore.Http;

namespace Malama.Models
{
    public class DentalCoordinatorStationPageViewModel
    {
        public ServiceMembersChild ServiceMember { get; set; } = new();
        public DentalQuestionnaire Questionnaire { get; set; } = new();
        public DentalXRayStation XRayStation { get; set; } = new();
        public DentalExam DentalExam { get; set; } = new();
        public DentalSharedClinicalViewModel SharedClinical { get; set; } = new();
        public DentalTreatmentCoordinator? TreatmentCoordinator { get; set; }

        /// <summary>True when a DentalQuestionnaire row exists for this service member.</summary>
        public bool HasQuestionnaire { get; set; }

        /// <summary>Consent form rows for the Treatment Coordinator status card.</summary>
        public List<TreatmentCoordinatorConsentFormStatusItem> ConsentFormStatuses { get; set; } = new();
    }

    public class TreatmentCoordinatorConsentFormStatusItem
    {
        public string Title { get; set; } = string.Empty;

        public bool IsSigned { get; set; }

        /// <summary>dental-treatment | oral-surgery (reserved for future PDF preview).</summary>
        public string FormKind { get; set; } = string.Empty;

        public long? EventStaffId { get; set; }
    }

    /// <summary>
    /// Coordinator station post model: questionnaire + X-Ray uploads + subsequent diseases (PSR / DEN / Pano)
    /// + Treatment Coordinator comments.
    /// </summary>
    public class DentalCoordinatorStationSaveDto : DentalXRayStationSaveDto
    {
        public long DentalExamId { get; set; }

        public string? PsrUpperRight { get; set; }
        public string? PsrUpperAnterior { get; set; }
        public string? PsrUpperLeft { get; set; }
        public string? PsrLowerRight { get; set; }
        public string? PsrLowerAnterior { get; set; }
        public string? PsrLowerLeft { get; set; }
        public string? PsrCarrierRisk { get; set; }
        public List<int> PsrSelectedTeeth { get; set; } = new();
        public string? SoftTissuesWnl { get; set; }
        public string? SoftTissuesConditionDetail { get; set; }

        public string? DenClass { get; set; }
        public string? DenClassReasonComments { get; set; }
        public bool PanoXRayAcknowledged { get; set; }

        public bool IsTreatmentRequired { get; set; } = true;

        public string? TreatmentCoordinatorComments { get; set; }

        public string? FindingsJson { get; set; }

        public string? AppointmentsJson { get; set; }

        /// <summary>
        /// Optional Treatment Coordinator document uploads (PDF only).
        /// </summary>
        public List<IFormFile>? TreatmentCoordinatorDocuments { get; set; }

        /// <summary>Server-stored document file names to keep on save.</summary>
        public List<string> RetainedDocumentFileNames { get; set; } = new();
    }

    public class TreatmentCoordinatorAppointmentJsonDto
    {
        public string? Id { get; set; }
        public string? AssignedDentist { get; set; }
        public string? AppointmentDate { get; set; }
        public string? AppointmentStartTime { get; set; }
        public string? AppointmentDuration { get; set; }
        public List<string> FindingClientKeys { get; set; } = new();
    }

    /// <summary>Event-wide appointment block for the TC calendar (other service members).</summary>
    public class TreatmentCoordinatorEventAppointmentDto
    {
        public long Id { get; set; }
        public long ServiceMembersChildId { get; set; }
        public string ServiceMemberName { get; set; } = string.Empty;
        public string AssignedDentist { get; set; } = string.Empty;
        public string DentistDisplayName { get; set; } = string.Empty;
        public string AppointmentDate { get; set; } = string.Empty;
        public string AppointmentStartTime { get; set; } = string.Empty;
        public string AppointmentDuration { get; set; } = string.Empty;
    }
}
