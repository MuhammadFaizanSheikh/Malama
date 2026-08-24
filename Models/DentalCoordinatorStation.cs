namespace Malama.Models
{
    public class DentalCoordinatorStationPageViewModel
    {
        public ServiceMembersChild ServiceMember { get; set; } = new();
        public DentalQuestionnaire Questionnaire { get; set; } = new();
    }

    public class DentalCoordinatorStationSaveDto : IDentalQuestionnaireFormData
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
    }
}
