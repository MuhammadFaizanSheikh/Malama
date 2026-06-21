namespace Malama.Models

{

    public class DentalExamStationPageViewModel

    {

        public ServiceMembersChild ServiceMember { get; set; } = new();

        public DentalQuestionnaire Questionnaire { get; set; } = new();

        public DentalXRayStation XRayStation { get; set; } = new();

        public DentalExam DentalExam { get; set; } = new();

    }



    public class DentalExamStationSaveDto : IDentalQuestionnaireFormData

    {

        public long ServiceMembersChildId { get; set; }

        public long DentalExamId { get; set; }

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

        public bool GoToVitalStation { get; set; }



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

        public string? DentistSignatureName { get; set; }

        public string? FindingsJson { get; set; }

        public List<DentalExamFindingDto> Findings { get; set; } = new();

    }

}