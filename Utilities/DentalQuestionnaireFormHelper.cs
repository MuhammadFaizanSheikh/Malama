using ExcelFilesCompiler.Controllers.Services;
using Malama.Models;

namespace ExcelFilesCompiler.Utilities
{
    public static class DentalQuestionnaireFormBinder
    {
        public static void BindHealthConditions(IDentalQuestionnaireFormData dto, IFormCollection form)
        {
            dto.ApplicableHealthConditions = form["ApplicableHealthConditions"].ToList();

            var details = new List<DentalXRayHealthConditionDetail>();
            for (var i = 0; i <= DentalXRayQuestionnaire.HealthConditions.Length; i++)
            {
                var condition = form[$"HealthConditionDetails[{i}].Condition"].ToString();
                if (string.IsNullOrWhiteSpace(condition))
                {
                    continue;
                }

                details.Add(new DentalXRayHealthConditionDetail
                {
                    Condition = condition,
                    Detail = form[$"HealthConditionDetails[{i}].Detail"].ToString()
                });
            }

            dto.HealthConditionDetails = details;
        }
    }

    public static class DentalQuestionnaireValidator
    {
        public static string? Validate(IDentalQuestionnaireFormData dto, ServiceMembersChild serviceMember)
        {
            var yesNoQuestions = new (string? Value, string Label)[]
            {
                (dto.HealthcareProviderCareLast2Years, "Question 1"),
                (dto.SeriousIllnessOperationHospitalization, "Question 2"),
                (dto.MedicationFoodAllergy, "Question 3"),
                (dto.TakingMedications, "Question 4"),
                (dto.HepatitisOrJaundice, "Question 5"),
                (dto.HealthChangeLastTwoYears, "Question 6"),
                (dto.UseTobaccoOrVape, "Question 7"),
                (dto.DrinkAlcoholicBeverages, "Question 8"),
                (dto.SickFromDentalTreatment, "Question 9"),
                (dto.BleederOrExcessiveBleeding, "Question 10"),
                (dto.ShortOfBreathOneFlightStairs, "Question 11")
            };

            foreach (var (value, label) in yesNoQuestions)
            {
                if (!IsYesOrNo(value))
                {
                    return $"{label} is required.";
                }
            }

            var detailError = ValidateYesDetail(
                dto.SeriousIllnessOperationHospitalization,
                dto.SeriousIllnessOperationHospitalizationDetail,
                "Question 2");
            if (detailError != null) return detailError;

            detailError = ValidateYesDetail(dto.MedicationFoodAllergy, dto.MedicationFoodAllergyDetail, "Question 3");
            if (detailError != null) return detailError;

            detailError = ValidateYesDetail(dto.TakingMedications, dto.TakingMedicationsDetail, "Question 4");
            if (detailError != null) return detailError;

            if (IsYes(dto.UseTobaccoOrVape))
            {
                foreach (var type in DentalXRayQuestionnaire.TobaccoTypes)
                {
                    var tobacco = dto.TobaccoUseDetails?.FirstOrDefault(t =>
                        string.Equals(t.Type, type, StringComparison.OrdinalIgnoreCase));
                    if (tobacco == null || !IsYesOrNo(tobacco.Used))
                    {
                        return $"Question 7 ({type}) selection is required.";
                    }

                    if (IsYes(tobacco.Used))
                    {
                        var hasDay = !string.IsNullOrWhiteSpace(tobacco.TimesPerDay);
                        var hasWeek = !string.IsNullOrWhiteSpace(tobacco.TimesPerWeek);
                        if (!hasDay && !hasWeek)
                        {
                            return $"Question 7 ({type}) requires Times per day or Times per week when Yes is selected.";
                        }
                    }
                }
            }

            if (IsYes(dto.DrinkAlcoholicBeverages) &&
                string.IsNullOrWhiteSpace(dto.AlcoholicBeveragesFrequencyQuantity))
            {
                return "Question 8 requires Frequency / Quantity when Yes is selected.";
            }

            if (DentalXRayStationService.IsFemale(serviceMember))
            {
                if (!IsYesOrNo(dto.AreYouPregnant))
                {
                    return "Question 12 is required for female service members.";
                }
            }

            return null;
        }

        private static string? ValidateYesDetail(string? answer, string? detail, string label)
        {
            if (IsYes(answer) && string.IsNullOrWhiteSpace(detail))
            {
                return $"{label} requires detail when Yes is selected.";
            }

            return null;
        }

        private static bool IsYes(string? value)
        {
            return string.Equals(value?.Trim(), "Yes", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsYesOrNo(string? value)
        {
            return IsYes(value) || string.Equals(value?.Trim(), "No", StringComparison.OrdinalIgnoreCase);
        }
    }
}
