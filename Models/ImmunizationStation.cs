using ExcelFilesCompiler.Controllers;
using ExcelFilesCompiler;
using Malama.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Malama.Models
{
    public class ImmunizationViewModel
    {
        public List<FileDataDto> FileDataList { get; set; } = new();
        public ImmunizationSummary Summary { get; set; } = new();
    }
    public class ImmunizationStation : GenericProperties
    {
        public long Id { get; set; }

        // Foreign key to FileDataDto (1-to-1 or 1-to-many, depending on your flow)
        public long FileDataId { get; set; }
        [JsonIgnore]
        [ValidateNever]
        public virtual FileDataDto FileData { get; set; }


        // Question 1
        public string IsSickToday { get; set; } // "Yes", "No", "YesWithReason"
        public string? IsSickTodayReason { get; set; }

        // Question 2
        public string HasAllergiesToMedicationFoodVaccineOrLatex { get; set; }
        public string? HasAllergiesReason { get; set; }

        // Question 3
        public string HadSeriousReactionAfterVaccination { get; set; }
        public string? SeriousReactionReason { get; set; }

        // Question 4
        public string HasLongTermHealthProblem { get; set; }
        public string? LongTermHealthProblemReason { get; set; }

        // Question 5
        public string HasCancerOrImmuneSystemProblem { get; set; }
        public string? CancerOrImmuneSystemReason { get; set; }

        // Question 6
        public string TookImmuneSuppressingMedicationRecently { get; set; }
        public string? ImmuneSuppressionReason { get; set; }

        // Question 7
        public string HadSeizureOrNervousSystemProblem { get; set; }
        public string? SeizureReason { get; set; }

        // Question 8
        public string HadBloodTransfusionOrAntiviralInPastYear { get; set; }
        public string? BloodTransfusionReason { get; set; }

        // Question 9 (special handling)
        public string IsPregnantOrCouldBePregnant { get; set; } // "Yes" or "No"
        public bool PregnancyCheckboxSelected { get; set; }     // Nullable

        // Question 10
        public string ReceivedVaccineInPast4Weeks { get; set; }
        public string? ReceivedVaccineReason { get; set; }

        // Hepatitis B
        public string? HepBNeeded { get; set; }             // "Yes", "No", "YesWithReason"
        public string? HepBReason { get; set; }
        public string? HepBManufacturer { get; set; }
        public string? HepBLotNo { get; set; }
        public DateTime? HepBExpirationDate { get; set; }
        public string? HepBType { get; set; }
        public string? HepBBodyPart { get; set; }
        public string? HepBBodyPartOther { get; set; }
        public string? HepBSite { get; set; }
        public string? HepBStaffName { get; set; }
        public DateTime? HepBGivenDateTime { get; set; }

        // Influenza
        public string? FluNeeded { get; set; }
        public string? FluReason { get; set; }
        public string? FluManufacturer { get; set; }
        public string? FluLotNo { get; set; }
        public DateTime? FluExpirationDate { get; set; }
        public string? FluType { get; set; }
        public string? FluBodyPart { get; set; }
        public string? FluBodyPartOther { get; set; }
        public string? FluSite { get; set; }
        public string? FluStaffName { get; set; }
        public DateTime? FluGivenDateTime { get; set; }

        // MMR
        public string? MMRNeeded { get; set; }
        public string? MMRReason { get; set; }
        public string? MMRManufacturer { get; set; }
        public string? MMRLotNo { get; set; }
        public DateTime? MMRExpirationDate { get; set; }
        public string? MMRType { get; set; }  // IM or SQ
        public string? MMRBodyPart { get; set; }  // Depends on Type
        public string? MMRBodyPartOther { get; set; }
        public string? MMRSite { get; set; }  // Left / Right
        public string? MMRStaffName { get; set; }
        public DateTime? MMRGivenDateTime { get; set; }

        // Hepatitis A
        public string? HepANeeded { get; set; }
        public string? HepAReason { get; set; }
        public string? HepAManufacturer { get; set; }
        public string? HepALotNo { get; set; }
        public DateTime? HepAExpirationDate { get; set; }
        public string? HepAType { get; set; }  // IM or SQ
        public string? HepABodyPart { get; set; }  // Depends on Type
        public string? HepABodyPartOther { get; set; }
        public string? HepASite { get; set; }  // Left / Right
        public string? HepAStaffName { get; set; }
        public DateTime? HepAGivenDateTime { get; set; }

        // Tetanus / Tdap
        public string? TetTdpNeeded { get; set; }
        public string? TetTdpReason { get; set; }
        public string? TetTdpManufacturer { get; set; }
        public string? TetTdpLotNo { get; set; }
        public DateTime? TetTdpExpirationDate { get; set; }
        public string? TetTdpType { get; set; }
        public string? TetTdpBodyPart { get; set; }
        public string? TetTdpBodyPartOther { get; set; }
        public string? TetTdpSite { get; set; }
        public string? TetTdpStaffName { get; set; }
        public DateTime? TetTdpGivenDateTime { get; set; }

        // Varicella
        public string? VaricellaNeeded { get; set; }
        public string? VaricellaReason { get; set; }
        public string? VaricellaManufacturer { get; set; }
        public string? VaricellaLotNo { get; set; }
        public DateTime? VaricellaExpirationDate { get; set; }
        public string? VaricellaType { get; set; }
        public string? VaricellaBodyPart { get; set; }
        public string? VaricellaBodyPartOther { get; set; }
        public string? VaricellaSite { get; set; }
        public string? VaricellaStaffName { get; set; }
        public DateTime? VaricellaGivenDateTime { get; set; }

        // Overall Status (optional, if you want workflow tracking)
        public string Status { get; set; }
    }
}
