using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Malama.Models
{
    [Table("DentalQuestionaire")]
    public class DentalQuestionnaire : GenericProperties
    {
        public long Id { get; set; }

        public long ServiceMembersChildId { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public virtual ServiceMembersChild ServiceMembersChild { get; set; }

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
        public string? TobaccoUseDetailsJson { get; set; }
        public string? DrinkAlcoholicBeverages { get; set; }
        public string? AlcoholicBeveragesFrequencyQuantity { get; set; }
        public string? SickFromDentalTreatment { get; set; }
        public string? BleederOrExcessiveBleeding { get; set; }
        public string? ShortOfBreathOneFlightStairs { get; set; }
        public string? AreYouPregnant { get; set; }
        public string? PregnancyApproval { get; set; }
        public string? ApplicableHealthConditionsJson { get; set; }
    }

    public class DentalXRayStationPageViewModel
    {
        public DentalXRayStation Station { get; set; } = new();
        public DentalQuestionnaire Questionnaire { get; set; } = new();
    }
}
