using ExcelFilesCompiler.Controllers.Services;
using Malama.Models;
using Microsoft.AspNetCore.Http;

namespace ExcelFilesCompiler.Utilities
{
    public static class FormCheckboxHelper
    {
        public static bool IsChecked(IFormCollection form, string fieldName)
        {
            if (!form.TryGetValue(fieldName, out var values))
            {
                return false;
            }

            return values.Any(value => string.Equals(value, "true", StringComparison.OrdinalIgnoreCase));
        }
    }

    public static class DentalExamValidator
    {
        public static bool IsSubsequentDiseasesSectionActive(DentalExamStationSaveDto dto)
        {
            return dto.QuestionnaireReviewed && dto.DentistSignatureEntered;
        }

        public static string? ValidatePsr(DentalExamStationSaveDto dto)
        {
            if (!IsSubsequentDiseasesSectionActive(dto))
            {
                return null;
            }

            var sextantFields = new (string? Value, string Label)[]
            {
                (dto.PsrUpperRight, "Upper Right"),
                (dto.PsrUpperAnterior, "Upper Anterior"),
                (dto.PsrUpperLeft, "Upper Left"),
                (dto.PsrLowerRight, "Lower Right"),
                (dto.PsrLowerAnterior, "Lower Anterior"),
                (dto.PsrLowerLeft, "Lower Left")
            };

            foreach (var (value, label) in sextantFields)
            {
                if (!IsValidPsrScore(value))
                {
                    return $"PSR {label} is required.";
                }
            }

            if (string.IsNullOrWhiteSpace(dto.PsrCarrierRisk)
                || !DentalExamPsr.CarrierRiskOptions.Contains(dto.PsrCarrierRisk, StringComparer.OrdinalIgnoreCase))
            {
                return "PSR Carrier Risk is required.";
            }

            if (string.IsNullOrWhiteSpace(dto.SoftTissuesWnl))
            {
                return "Soft Tissues WNL selection is required.";
            }

            if (dto.SoftTissuesWnl.Equals(DentalExamPsr.SoftTissuesWnlNo, StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrWhiteSpace(dto.SoftTissuesConditionDetail))
            {
                return "Describe Condition not within normal limits is required when Soft Tissues is NOT within Normal Limits.";
            }

            return null;
        }

        public static string? ValidateDenClass(DentalExamStationSaveDto dto)
        {
            if (!IsSubsequentDiseasesSectionActive(dto))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(dto.DenClass)
                || !DentalExamDenClass.Options.Contains(dto.DenClass.Trim(), StringComparer.OrdinalIgnoreCase))
            {
                return "Dental Readiness Classification (DEN Class) is required.";
            }

            if (string.IsNullOrWhiteSpace(dto.DenClassReasonComments))
            {
                return "Classification Reason / Comments is required.";
            }

            return null;
        }

        public static string? ValidateQuestionnaireReview(DentalExamStationSaveDto dto)
        {
            if (dto.QuestionnaireReviewed && string.IsNullOrWhiteSpace(dto.FinalComments))
            {
                return "Final Comments is required when Dental Questionnaire has been reviewed.";
            }

            if (dto.DentistSignatureEntered && string.IsNullOrWhiteSpace(dto.DentistSignatureName))
            {
                return "Dentist name is required when Dentist Signature is entered.";
            }

            return null;
        }

        private static bool IsValidPsrScore(string? value)
        {
            return !string.IsNullOrWhiteSpace(value)
                && DentalExamPsr.ScoreOptions.Contains(value.Trim(), StringComparer.OrdinalIgnoreCase);
        }
    }

    public static class DentalExamTobaccoDisplayHelper
    {
        public static string FormatFromQuestionnaire(DentalQuestionnaire? questionnaire)
        {
            if (questionnaire == null
                || string.IsNullOrWhiteSpace(questionnaire.UseTobaccoOrVape)
                || questionnaire.UseTobaccoOrVape.Equals("No", StringComparison.OrdinalIgnoreCase))
            {
                return "No";
            }

            if (!questionnaire.UseTobaccoOrVape.Equals("Yes", StringComparison.OrdinalIgnoreCase))
            {
                return "No";
            }

            var usedTypes = DentalQuestionnaireService.ParseTobaccoUsedTypes(questionnaire.TobaccoUseDetailsJson);
            return usedTypes.Count > 0
                ? $"Yes ({string.Join(", ", usedTypes)})"
                : "Yes";
        }
    }
}
