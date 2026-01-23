using ExcelFilesCompiler.Controllers;
using ExcelFilesCompiler;
using Malama.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Malama.Models
{
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
        public string? HepBReasonExcusedComments { get; set; }

        public long? HepBVaccineInfoId { get; set; }
        public long? HepBVaccineLotEntryId { get; set; }

        [ForeignKey(nameof(HepBVaccineInfoId))]
        public ImmunizationVaccineInfo? HepBVaccineInfo { get; set; }

        [ForeignKey(nameof(HepBVaccineLotEntryId))]
        public ImmunizationVaccineLotEntry? HepBVaccineLot { get; set; }

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
        public string? FluReasonExcusedComments { get; set; }

        public long? FluVaccineInfoId { get; set; }
        public long? FluVaccineLotEntryId { get; set; }

        [ForeignKey(nameof(FluVaccineInfoId))]
        public ImmunizationVaccineInfo? FluVaccineInfo { get; set; }

        [ForeignKey(nameof(FluVaccineLotEntryId))]
        public ImmunizationVaccineLotEntry? FluVaccineLot { get; set; }

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
        public string? MMRReasonExcusedComments { get; set; }

        public long? MMRVaccineInfoId { get; set; }
        public long? MMRVaccineLotEntryId { get; set; }

        [ForeignKey(nameof(MMRVaccineInfoId))]
        public ImmunizationVaccineInfo? MMRVaccineInfo { get; set; }

        [ForeignKey(nameof(MMRVaccineLotEntryId))]
        public ImmunizationVaccineLotEntry? MMRVaccineLot { get; set; }

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
        public string? HepAReasonExcusedComments { get; set; }

        public long? HepAVaccineInfoId { get; set; }
        public long? HepAVaccineLotEntryId { get; set; }

        [ForeignKey(nameof(HepAVaccineInfoId))]
        public ImmunizationVaccineInfo? HepAVaccineInfo { get; set; }

        [ForeignKey(nameof(HepAVaccineLotEntryId))]
        public ImmunizationVaccineLotEntry? HepAVaccineLot { get; set; }

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
        public string? TetTdpReasonExcusedComments { get; set; }

        public long? TetTdpVaccineInfoId { get; set; }
        public long? TetTdpVaccineLotEntryId { get; set; }

        [ForeignKey(nameof(TetTdpVaccineInfoId))]
        public ImmunizationVaccineInfo? TetTdpVaccineInfo { get; set; }

        [ForeignKey(nameof(TetTdpVaccineLotEntryId))]
        public ImmunizationVaccineLotEntry? TetTdpVaccineLot { get; set; }

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
        public string? VaricellaReasonExcusedComments { get; set; }

        public long? VaricellaVaccineInfoId { get; set; }
        public long? VaricellaVaccineLotEntryId { get; set; }

        [ForeignKey(nameof(VaricellaVaccineInfoId))]
        public ImmunizationVaccineInfo? VaricellaVaccineInfo { get; set; }

        [ForeignKey(nameof(VaricellaVaccineLotEntryId))]
        public ImmunizationVaccineLotEntry? VaricellaVaccineLot { get; set; }

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
