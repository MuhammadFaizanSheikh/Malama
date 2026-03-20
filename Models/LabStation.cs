using ExcelFilesCompiler.Controllers;
using ExcelFilesCompiler;
using Malama.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Malama.Models
{
    public class LabStationViewModel
    {
        public List<ServiceMembersChild> FileDataList { get; set; } = new();
        //public ImmunizationSummary Summary { get; set; } = new();
    }
    public class LabStation : GenericProperties
    {
        public long Id { get; set; }

        // Foreign key to FileDataDto (1-to-1 or 1-to-many, depending on your flow)
        public long ServiceMembersChildId { get; set; }
        [JsonIgnore]
        [ValidateNever]
        public virtual ServiceMembersChild ServiceMembersChild { get; set; }

        public string AreYouFasting { get; set; }
        public string AnyComplicationInBloodDrawn { get; set; }
        public string AllergicToLatex { get; set; }
        public string FeelAlright { get; set; }

        public string? G6pdNeeded { get; set; }
        public string? G6pdReason { get; set; }

        public string? AboNeeded { get; set; }
        public string? AboReason { get; set; }
        public string? AboGrouping { get; set; }
        public string? AboRhFactor { get; set; }

        // Lipid Panel
        public string? LipidPanelNeeded { get; set; }
        public string? LipidPanelReason { get; set; }

        public bool LipidPanelRapidTesting { get; set; } = false;

        public int? TotalCholesterol { get; set; }
        public bool TotalCholesterol_LessThan100 { get; set; }
        public bool TotalCholesterol_GreaterThan400 { get; set; }
        public int? HdlCholesterol { get; set; }
        public bool HdlCholesterol_LessThan20 { get; set; }
        public bool HdlCholesterol_GreaterThan120 { get; set; }
        public int? Triglycerides { get; set; }
        public bool Triglycerides_LessThan50 { get; set; }
        public bool Triglycerides_GreaterThan500 { get; set; }
        public int? Glucose { get; set; }
        public bool Glucose_LessThan20 { get; set; }
        public bool Glucose_GreaterThan600 { get; set; }
        public decimal? A1C { get; set; }
        public decimal? LdlCholesterol { get; set; }
        public decimal? TotalCholesterolHdlRatio { get; set; }
        public decimal? LdlHdlLipoprotiens { get; set; }
        public int? NonHdlCholesterol { get; set; }

        public string? HivNeeded { get; set; }
        public string? HivReason { get; set; }
        public string? HivBarcodeCarebill { get; set; }

        // Pregnancy Test
        public string? PregnancyTestNeeded { get; set; }
        public string? PregnancyTestResult { get; set; }
        public string? PregnancyTestReason { get; set; }
        public string? FedExTrackingNo { get; set; }

        public DateTime? G6pdGivenDateTime { get; set; }
        public DateTime? AboGivenDateTime { get; set; }
        public DateTime? LipidPanelGivenDateTime { get; set; }
        public DateTime? HivGivenDateTime { get; set; }
        public DateTime? PregnancyTestGivenDateTime { get; set; }


        public string Status { get; set; }
    }
}
