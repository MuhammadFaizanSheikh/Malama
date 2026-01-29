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
        public List<FileDataDto> FileDataList { get; set; } = new();
        //public ImmunizationSummary Summary { get; set; } = new();
    }
    public class LabStation : GenericProperties
    {
        public long Id { get; set; }

        // Foreign key to FileDataDto (1-to-1 or 1-to-many, depending on your flow)
        public long FileDataId { get; set; }
        [JsonIgnore]
        [ValidateNever]
        public virtual FileDataDto FileData { get; set; }

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

        public string? TotalCholesterol { get; set; }
        public string? HdlCholesterol { get; set; }
        public string? Triglycerides { get; set; }
        public string? Glucose { get; set; }
        public string? A1C { get; set; }
        public string? LdlCholesterol { get; set; }
        public string? TotalCholesterolHdlRatio { get; set; }
        public string? LdlHdlLipoprotiens { get; set; }
        public string? NonHdlCholesterol { get; set; }

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
