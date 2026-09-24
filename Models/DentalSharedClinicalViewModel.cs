namespace Malama.Models
{
    /// <summary>
    /// Composed clinical surface for Exam/TC views and partials (PSR, DEN, Pano, findings, teeth).
    /// Not an EF entity.
    /// </summary>
    public class DentalSharedClinicalViewModel
    {
        public long ServiceMembersChildId { get; set; }

        public string? PsrUpperRight { get; set; }
        public string? PsrUpperAnterior { get; set; }
        public string? PsrUpperLeft { get; set; }
        public string? PsrLowerRight { get; set; }
        public string? PsrLowerAnterior { get; set; }
        public string? PsrLowerLeft { get; set; }
        public string? PsrCarrierRisk { get; set; }
        public string? SoftTissuesWnl { get; set; }
        public string? SoftTissuesConditionDetail { get; set; }

        public string? DenClass { get; set; }
        public string? DenClassReasonComments { get; set; }
        public bool PanoXRayAcknowledged { get; set; }

        /// <summary>Source of clinical sections (PSR/DEN/Pano) for ownership/read-only UI.</summary>
        public string? ClinicalSource { get; set; }

        public List<DentalFinding> Findings { get; set; } = new();
        public List<DentalPsrSelectedTooth> SelectedTeeth { get; set; } = new();

        public static DentalSharedClinicalViewModel FromParts(
            long serviceMembersChildId,
            DentalPsr? psr,
            DentalDenClassRecord? denClass,
            DentalPanoAcknowledgement? pano,
            IEnumerable<DentalFinding>? findings)
        {
            var clinicalSource = psr?.Source
                ?? denClass?.Source
                ?? pano?.Source;

            return new DentalSharedClinicalViewModel
            {
                ServiceMembersChildId = serviceMembersChildId,
                PsrUpperRight = psr?.PsrUpperRight,
                PsrUpperAnterior = psr?.PsrUpperAnterior,
                PsrUpperLeft = psr?.PsrUpperLeft,
                PsrLowerRight = psr?.PsrLowerRight,
                PsrLowerAnterior = psr?.PsrLowerAnterior,
                PsrLowerLeft = psr?.PsrLowerLeft,
                PsrCarrierRisk = psr?.PsrCarrierRisk,
                SoftTissuesWnl = psr?.SoftTissuesWnl,
                SoftTissuesConditionDetail = psr?.SoftTissuesConditionDetail,
                DenClass = denClass?.DenClass,
                DenClassReasonComments = denClass?.DenClassReasonComments,
                PanoXRayAcknowledged = pano?.PanoXRayAcknowledged ?? false,
                ClinicalSource = clinicalSource,
                Findings = (findings ?? Enumerable.Empty<DentalFinding>())
                    .OrderBy(f => f.SortOrder)
                    .ToList(),
                SelectedTeeth = (psr?.SelectedTeeth ?? Enumerable.Empty<DentalPsrSelectedTooth>())
                    .OrderBy(t => t.ToothNumber)
                    .ToList()
            };
        }
    }
}
