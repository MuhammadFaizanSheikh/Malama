using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Malama.Models
{
    [Table("TreatmentConsent")]
    public class TreatmentConsent : GenericProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long ServiceMembersChildId { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public virtual ServiceMembersChild? ServiceMembersChild { get; set; }

        /// <summary>Questionnaire is always included; this flag is informational only.</summary>
        public bool IncludeQuestionnaire { get; set; } = true;

        public bool IncludeOralSurgeryForm { get; set; }

        public bool IncludeDentalTreatmentConsent { get; set; }

        /// <summary>JSON array of EventStaff Ids selected for Oral Surgery Form.</summary>
        public string? OralSurgeryDentistEventStaffIdsJson { get; set; }

        /// <summary>JSON array of EventStaff Ids selected for Dental Treatment Consent.</summary>
        public string? DentalTreatmentDentistEventStaffIdsJson { get; set; }

        /// <summary>Legacy single procedure text; prefer per-dentist OralSurgeryFormsJson.</summary>
        public string? OralSurgeryProcedureText { get; set; }

        /// <summary>JSON array of per-dentist Oral Surgery form payloads (no signature bytes).</summary>
        public string? OralSurgeryFormsJson { get; set; }

        /// <summary>JSON array of per-dentist Dental Treatment Consent form payloads (no signature bytes).</summary>
        public string? DentalTreatmentFormsJson { get; set; }

        /// <summary>Pending until every included consent form is signed; then Completed.</summary>
        public string Status { get; set; } = "Pending";
    }

    public class TreatmentConsentFormSelectionDto
    {
        public long ServiceMembersChildId { get; set; }
        public bool IncludeQuestionnaire { get; set; } = true;
        public bool IncludeOralSurgeryForm { get; set; }
        public bool IncludeDentalTreatmentConsent { get; set; }
        public List<long> OralSurgeryDentistEventStaffIds { get; set; } = new();
        public List<long> DentalTreatmentDentistEventStaffIds { get; set; } = new();
        public string? OralSurgeryProcedureText { get; set; }
        public List<TreatmentConsentOralSurgeryFormDto> OralSurgeryForms { get; set; } = new();
        public List<TreatmentConsentDentalTreatmentFormDto> DentalTreatmentForms { get; set; } = new();
        public string Status { get; set; } = "Pending";
    }

    public class TreatmentConsentOralSurgeryFormDto
    {
        public long EventStaffId { get; set; }
        public string? DentistName { get; set; }
        public string? ProcedureText { get; set; }
        public string? SignatureFileName { get; set; }
        public bool IsSigned { get; set; }

        /// <summary>Client-only: data URL for new/updated signature ink. Not persisted in JSON.</summary>
        public string? SignatureDataUrl { get; set; }
    }

    public class TreatmentConsentDentalTreatmentFormDto
    {
        public long EventStaffId { get; set; }
        public string? DentistName { get; set; }

        public bool Item1Mark { get; set; }
        public bool Fillings { get; set; }
        public bool Crowns { get; set; }
        public bool Extractions { get; set; }
        public bool Impacted { get; set; }
        public bool RootCanal { get; set; }
        public bool Fmd { get; set; }
        public bool OtherTreatment { get; set; }
        public string? OtherTreatmentText { get; set; }
        public string? Item1Initials { get; set; }

        public bool Item2Mark { get; set; }
        public string? Item2Initials { get; set; }

        public bool Item3Mark { get; set; }
        public string? RemovalTeeth { get; set; }
        public string? Item3Initials { get; set; }

        public bool Item4Mark { get; set; }
        public string? Item4Initials { get; set; }

        public bool Item5Mark { get; set; }
        public string? Item5Initials { get; set; }

        public bool Item6Mark { get; set; }
        public string? OtherSituation { get; set; }
        public string? Item6Initials { get; set; }

        public string? SignatureFileName { get; set; }
        public bool IsSigned { get; set; }

        /// <summary>Client-only: data URL for new/updated signature ink. Not persisted in JSON.</summary>
        public string? SignatureDataUrl { get; set; }
    }

    public class TreatmentConsentStationSaveDto : IDentalQuestionnaireFormData
    {
        public long ServiceMembersChildId { get; set; }

        public string? HealthcareProviderCareLast2Years { get; set; }
        public string? SeriousIllnessOperationHospitalization { get; set; }
        public string? SeriousIllnessOperationHospitalizationDetail { get; set; }
        public string? MedicationFoodAllergy { get; set; }
        public string? MedicationFoodAllergyDetail { get; set; }
        public string? TakingMedications { get; set; }
        public string? TakingMedicationsDetail { get; set; }
        public string? HepatitisOrJaundice { get; set; }
        public string? HealthChangeLastTwoYears { get; set; }
        public string? UseTobaccoOrVape { get; set; }
        public List<DentalXRayTobaccoUseDetail> TobaccoUseDetails { get; set; } = new();
        public string? DrinkAlcoholicBeverages { get; set; }
        public string? AlcoholicBeveragesFrequencyQuantity { get; set; }
        public string? SickFromDentalTreatment { get; set; }
        public string? BleederOrExcessiveBleeding { get; set; }
        public string? ShortOfBreathOneFlightStairs { get; set; }
        public string? AreYouPregnant { get; set; }
        public string? PregnancyApproval { get; set; }
        public List<string> ApplicableHealthConditions { get; set; } = new();
        public List<DentalXRayHealthConditionDetail> HealthConditionDetails { get; set; } = new();

        public bool IncludeOralSurgeryForm { get; set; }
        public bool IncludeDentalTreatmentConsent { get; set; }
        public List<long> OralSurgeryDentistEventStaffIds { get; set; } = new();
        public List<long> DentalTreatmentDentistEventStaffIds { get; set; } = new();
        public List<TreatmentConsentOralSurgeryFormDto> OralSurgeryForms { get; set; } = new();
        public List<TreatmentConsentDentalTreatmentFormDto> DentalTreatmentForms { get; set; } = new();
    }

    public class TreatmentConsentStationSaveResult
    {
        public bool Success { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? RedirectUrl { get; set; }

        public static TreatmentConsentStationSaveResult Ok(string message, string? redirectUrl = null)
        {
            return new TreatmentConsentStationSaveResult
            {
                Success = true,
                Title = "Saved",
                Message = message,
                RedirectUrl = redirectUrl
            };
        }

        public static TreatmentConsentStationSaveResult Fail(string title, string message)
        {
            return new TreatmentConsentStationSaveResult
            {
                Success = false,
                Title = title,
                Message = message
            };
        }
    }

    public class TreatmentConsentSaveFormSelectionRequest
    {
        public long ServiceMembersChildId { get; set; }
        public bool IncludeOralSurgeryForm { get; set; }
        public bool IncludeDentalTreatmentConsent { get; set; }
        public List<long> OralSurgeryDentistEventStaffIds { get; set; } = new();
        public List<long> DentalTreatmentDentistEventStaffIds { get; set; } = new();
        public string? OralSurgeryProcedureText { get; set; }
    }

    public class TreatmentConsentSaveFormSelectionResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public TreatmentConsentFormSelectionDto? Selection { get; set; }

        public static TreatmentConsentSaveFormSelectionResponse Ok(string message, TreatmentConsentFormSelectionDto selection)
        {
            return new TreatmentConsentSaveFormSelectionResponse
            {
                Success = true,
                Message = message,
                Selection = selection
            };
        }

        public static TreatmentConsentSaveFormSelectionResponse Fail(string message)
        {
            return new TreatmentConsentSaveFormSelectionResponse
            {
                Success = false,
                Message = message
            };
        }
    }
}
