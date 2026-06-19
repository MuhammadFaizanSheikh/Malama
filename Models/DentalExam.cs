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

        public string Status { get; set; } = "Pending";
    }
}
