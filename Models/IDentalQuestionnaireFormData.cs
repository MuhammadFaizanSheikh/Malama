namespace Malama.Models
{
    public interface IDentalQuestionnaireFormData
    {
        long ServiceMembersChildId { get; set; }
        string? HealthcareProviderCareLast2Years { get; set; }
        string? SeriousIllnessOperationHospitalization { get; set; }
        string? SeriousIllnessOperationHospitalizationDetail { get; set; }
        string? MedicationFoodAllergy { get; set; }
        string? MedicationFoodAllergyDetail { get; set; }
        string? TakingMedications { get; set; }
        string? TakingMedicationsDetail { get; set; }
        string? HepatitisOrJaundice { get; set; }
        string? HealthChangeLastTwoYears { get; set; }
        string? UseTobaccoOrVape { get; set; }
        List<DentalXRayTobaccoUseDetail> TobaccoUseDetails { get; set; }
        string? DrinkAlcoholicBeverages { get; set; }
        string? AlcoholicBeveragesFrequencyQuantity { get; set; }
        string? SickFromDentalTreatment { get; set; }
        string? BleederOrExcessiveBleeding { get; set; }
        string? ShortOfBreathOneFlightStairs { get; set; }
        string? AreYouPregnant { get; set; }
        string? PregnancyApproval { get; set; }
        List<string> ApplicableHealthConditions { get; set; }
        List<DentalXRayHealthConditionDetail> HealthConditionDetails { get; set; }
    }
}
