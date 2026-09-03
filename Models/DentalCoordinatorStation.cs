namespace Malama.Models
{
    public class DentalCoordinatorStationPageViewModel
    {
        public ServiceMembersChild ServiceMember { get; set; } = new();
        public DentalQuestionnaire Questionnaire { get; set; } = new();
        public DentalXRayStation XRayStation { get; set; } = new();
        public DentalExam DentalExam { get; set; } = new();
        public DentalTreatment? DentalTreatment { get; set; }
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

        public string? TreatmentCoordinatorComments { get; set; }

        public string? FindingsJson { get; set; }

        public string? AppointmentsJson { get; set; }
    }

    public class TreatmentCoordinatorAppointmentJsonDto
    {
        public List<string> FindingClientKeys { get; set; } = new();
    }
}
