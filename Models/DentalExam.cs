using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Malama.Models
{
    public static class DentalExamPsr
    {
        public static readonly string[] ScoreOptions = { "0", "1", "2", "3", "4", "*", "X" };
        public static readonly string[] CarrierRiskOptions = { "Low", "Medium", "High" };

        public const string SoftTissuesWnlYes = "Yes, within Normal Limits";
        public const string SoftTissuesWnlNo = "No, NOT within Normal Limits";
    }

    public static class DentalExamDenClass
    {
        public const string Class1 = "Class1 - No treatment needed";
        public const string Class2 = "Class 2 - Treatment needed but not expected within 12 months";
        public const string Class3 = "Class 3 - Urgent treatment needed";
        public const string Class4 = "Class 4 - Unknown / Examination incomplete";

        public static readonly string[] Options =
        {
            Class1,
            Class2,
            Class3
        };

        /// <summary>
        /// Returns the forced DRC when findings require it (Class 3 or Class 2),
        /// or null when the user may choose Class 1 or Class 2 (no findings).
        /// </summary>
        public static string? ResolveForcedFromFindings(IEnumerable<DentalFindingDto>? findings)
        {
            var list = (findings ?? Enumerable.Empty<DentalFindingDto>())
                .Where(f => !string.IsNullOrWhiteSpace(f.Classification))
                .ToList();

            if (list.Count == 0)
            {
                return null;
            }

            if (list.Any(f => DentalFindingConstants.IsClass3(f.Classification)))
            {
                return Class3;
            }

            if (list.Any(f => DentalFindingConstants.IsClass2(f.Classification)))
            {
                return Class2;
            }

            return null;
        }

        /// <summary>
        /// Validates DenClass against finding classifications.
        /// Class 4 is never allowed. With findings, DenClass must match the forced class.
        /// With no findings, only Class 1 or Class 2 may be selected.
        /// </summary>
        public static string? ValidateAgainstFindings(string? denClass, IEnumerable<DentalFindingDto>? findings)
        {
            var selected = denClass?.Trim();

            if (string.Equals(selected, Class4, StringComparison.OrdinalIgnoreCase))
            {
                return "Class 4 is no longer a valid Dental Readiness Classification.";
            }

            var forced = ResolveForcedFromFindings(findings);

            if (forced != null)
            {
                if (!string.Equals(selected, forced, StringComparison.OrdinalIgnoreCase))
                {
                    return $"Dental Readiness Classification must be set to \"{forced}\" based on the finding classifications.";
                }

                return null;
            }

            if (string.IsNullOrWhiteSpace(selected))
            {
                return null;
            }

            if (string.Equals(selected, Class3, StringComparison.OrdinalIgnoreCase))
            {
                return "Class 3 cannot be selected when there are no Class 3 findings.";
            }

            if (!string.Equals(selected, Class1, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(selected, Class2, StringComparison.OrdinalIgnoreCase))
            {
                return "Dental Readiness Classification is invalid.";
            }

            return null;
        }
    }

    /// <summary>Thin Dental Exam station header (audit + status + dentist review/signature).</summary>
    [Table("DentalExam")]
    public class DentalExam : GenericProperties
    {
        public long Id { get; set; }

        public long ServiceMembersChildId { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public virtual ServiceMembersChild ServiceMembersChild { get; set; }

        public bool QuestionnaireReviewed { get; set; }
        public string? FinalComments { get; set; }
        public bool DentistSignatureEntered { get; set; }
        public string? DentistSignatureUserId { get; set; }
        public DateTime? DentistSignatureDateTime { get; set; }

        public string Status { get; set; } = "Pending";

        public string? Source { get; set; }
    }

    public static class DentalExamSources
    {
        public const string DentalExam = "DentalExam";
        public const string DentalCoordinator = "DentalCoordinator";
    }
}
