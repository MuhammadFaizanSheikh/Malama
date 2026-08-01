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
            Class3,
            Class4
        };
    }

    [Table("DentalExam")]
    public class DentalExam : GenericProperties
    {
        public long Id { get; set; }

        public long ServiceMembersChildId { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public virtual ServiceMembersChild ServiceMembersChild { get; set; }

        public string? PsrUpperRight { get; set; }
        public string? PsrUpperAnterior { get; set; }
        public string? PsrUpperLeft { get; set; }
        public string? PsrLowerRight { get; set; }
        public string? PsrLowerAnterior { get; set; }
        public string? PsrLowerLeft { get; set; }
        public string? PsrCarrierRisk { get; set; }
        public string? SoftTissuesWnl { get; set; }
        public string? SoftTissuesConditionDetail { get; set; }

        public bool QuestionnaireReviewed { get; set; }
        public string? FinalComments { get; set; }
        public bool DentistSignatureEntered { get; set; }
        public string? DentistSignatureUserId { get; set; }
        public DateTime? DentistSignatureDateTime { get; set; }

        public string? DenClass { get; set; }
        public string? DenClassReasonComments { get; set; }
        public bool PanoXRayAcknowledged { get; set; }

        public string Status { get; set; } = "Pending";

        [JsonIgnore]
        [ValidateNever]
        public virtual ICollection<DentalExamFinding> Findings { get; set; } = new List<DentalExamFinding>();

        [JsonIgnore]
        [ValidateNever]
        public virtual ICollection<DentalExamSelectedTooth> SelectedTeeth { get; set; } = new List<DentalExamSelectedTooth>();
    }
}
