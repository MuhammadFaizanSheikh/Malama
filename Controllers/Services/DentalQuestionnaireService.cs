using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using Malama.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class DentalQuestionnaireService : IDentalQuestionnaireService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DentalQuestionnaireService> _logger;
        private const string CLASSNAME = "DentalQuestionnaireService";

        public DentalQuestionnaireService(ILogger<DentalQuestionnaireService> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<DentalQuestionnaire?> GetByServiceMembersChildIdAsync(long serviceMembersChildId)
        {
            return await _unitOfWork.DentalQuestionnaire
                .GetWithIncludeNoTracking(q => q.ServiceMembersChildId == serviceMembersChildId)
                .FirstOrDefaultAsync();
        }

        public async Task SaveOrUpdateFromFormDataAsync(
            IDentalQuestionnaireFormData dto,
            string userName,
            string source,
            bool saveChanges = true)
        {
            const string methodName = nameof(SaveOrUpdateFromFormDataAsync);

            var existing = await _unitOfWork.DentalQuestionnaire
                .GetWithIncludeTracking(q => q.ServiceMembersChildId == dto.ServiceMembersChildId)
                .FirstOrDefaultAsync();

            if (existing != null)
            {
                MapFormDataToEntity(dto, existing);
                existing.Source = source;
                existing.UpdatedBy = userName;
                existing.UpdatedOn = DateTime.Now;

                if (saveChanges)
                {
                    await _unitOfWork.SaveAsync();
                }

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Questionnaire updated for ServiceMembersChildId={ServiceMembersChildId} Source={Source} by {User}. SaveChanges={SaveChanges}",
                    CLASSNAME, methodName, dto.ServiceMembersChildId, source, userName, saveChanges);
                return;
            }

            var entity = MapFormDataToEntity(dto);
            entity.Source = source;
            entity.AddedOn = DateTime.Now;
            entity.AddedBy = userName;

            await _unitOfWork.DentalQuestionnaire.AddAsync(entity);

            if (saveChanges)
            {
                await _unitOfWork.SaveAsync();
            }

            _logger.LogInformation(
                "{ClassName}, {MethodName}, Questionnaire created for ServiceMembersChildId={ServiceMembersChildId} Source={Source} by {User}. SaveChanges={SaveChanges}",
                CLASSNAME, methodName, dto.ServiceMembersChildId, source, userName, saveChanges);
        }

        public DentalQuestionnaire MapFormDataToEntity(IDentalQuestionnaireFormData dto, DentalQuestionnaire? existing = null)
        {
            var entity = existing ?? new DentalQuestionnaire();

            entity.ServiceMembersChildId = dto.ServiceMembersChildId;
            entity.HealthcareProviderCareLast2Years = dto.HealthcareProviderCareLast2Years;
            entity.SeriousIllnessOperationHospitalization = dto.SeriousIllnessOperationHospitalization;
            entity.SeriousIllnessOperationHospitalizationDetail = IsYes(dto.SeriousIllnessOperationHospitalization)
                ? dto.SeriousIllnessOperationHospitalizationDetail
                : null;
            entity.MedicationFoodAllergy = dto.MedicationFoodAllergy;
            entity.MedicationFoodAllergyDetail = IsYes(dto.MedicationFoodAllergy)
                ? dto.MedicationFoodAllergyDetail
                : null;
            entity.TakingMedications = dto.TakingMedications;
            entity.TakingMedicationsDetail = IsYes(dto.TakingMedications)
                ? dto.TakingMedicationsDetail
                : null;
            entity.HepatitisOrJaundice = dto.HepatitisOrJaundice;
            entity.HealthChangeLastTwoYears = dto.HealthChangeLastTwoYears;
            entity.UseTobaccoOrVape = dto.UseTobaccoOrVape;
            entity.TobaccoUseDetailsJson = IsYes(dto.UseTobaccoOrVape)
                ? JsonSerializer.Serialize(NormalizeTobaccoDetails(dto.TobaccoUseDetails))
                : null;
            entity.DrinkAlcoholicBeverages = dto.DrinkAlcoholicBeverages;
            entity.AlcoholicBeveragesFrequencyQuantity = IsYes(dto.DrinkAlcoholicBeverages)
                ? dto.AlcoholicBeveragesFrequencyQuantity
                : null;
            entity.SickFromDentalTreatment = dto.SickFromDentalTreatment;
            entity.BleederOrExcessiveBleeding = dto.BleederOrExcessiveBleeding;
            entity.ShortOfBreathOneFlightStairs = dto.ShortOfBreathOneFlightStairs;
            entity.AreYouPregnant = dto.AreYouPregnant;
            entity.PregnancyApproval = dto.PregnancyApproval;
            entity.ApplicableHealthConditionsJson = SerializeSelectedHealthConditions(
                dto.ApplicableHealthConditions,
                dto.HealthConditionDetails);

            return entity;
        }

        public static List<string> ParseTobaccoUsedTypes(string? json)
        {
            return ParseTobaccoDetails(json)
                .Where(t => string.Equals(t.Used, "Yes", StringComparison.OrdinalIgnoreCase))
                .Select(t => t.Type)
                .ToList();
        }

        public static List<DentalXRayTobaccoUseDetail> ParseTobaccoDetails(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<DentalXRayTobaccoUseDetail>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<DentalXRayTobaccoUseDetail>>(json) ?? new();
            }
            catch
            {
                return new List<DentalXRayTobaccoUseDetail>();
            }
        }

        public static List<DentalXRayHealthConditionDetail> ParseHealthConditionsJson(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<DentalXRayHealthConditionDetail>();
            }

            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.ValueKind != JsonValueKind.Array)
                {
                    return new List<DentalXRayHealthConditionDetail>();
                }

                var result = new List<DentalXRayHealthConditionDetail>();
                foreach (var element in doc.RootElement.EnumerateArray())
                {
                    if (element.ValueKind == JsonValueKind.String)
                    {
                        var condition = element.GetString();
                        if (!string.IsNullOrWhiteSpace(condition))
                        {
                            result.Add(new DentalXRayHealthConditionDetail
                            {
                                Condition = condition,
                                IsSelected = true
                            });
                        }

                        continue;
                    }

                    if (element.ValueKind == JsonValueKind.Object)
                    {
                        var condition = element.TryGetProperty("Condition", out var conditionProp)
                            ? conditionProp.GetString()
                            : null;
                        if (string.IsNullOrWhiteSpace(condition))
                        {
                            continue;
                        }

                        var detail = element.TryGetProperty("Detail", out var detailProp)
                            ? detailProp.GetString()
                            : null;

                        result.Add(new DentalXRayHealthConditionDetail
                        {
                            Condition = condition,
                            Detail = detail,
                            IsSelected = true
                        });
                    }
                }

                return result;
            }
            catch
            {
                return new List<DentalXRayHealthConditionDetail>();
            }
        }

        private static string? SerializeSelectedHealthConditions(
            List<string>? selectedConditions,
            List<DentalXRayHealthConditionDetail>? details)
        {
            if (selectedConditions == null || selectedConditions.Count == 0)
            {
                return null;
            }

            var detailLookup = (details ?? new List<DentalXRayHealthConditionDetail>())
                .Where(d => !string.IsNullOrWhiteSpace(d.Condition))
                .GroupBy(d => d.Condition, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First().Detail, StringComparer.OrdinalIgnoreCase);

            var selected = selectedConditions
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => new
                {
                    Condition = c.Trim(),
                    Detail = detailLookup.TryGetValue(c, out var detail) && !string.IsNullOrWhiteSpace(detail)
                        ? detail.Trim()
                        : null
                })
                .ToList();

            return selected.Count > 0 ? JsonSerializer.Serialize(selected) : null;
        }

        private static List<DentalXRayTobaccoUseDetail> NormalizeTobaccoDetails(List<DentalXRayTobaccoUseDetail>? details)
        {
            var normalized = new List<DentalXRayTobaccoUseDetail>();
            foreach (var type in DentalXRayQuestionnaire.TobaccoTypes)
            {
                var existing = details?.FirstOrDefault(d => string.Equals(d.Type, type, StringComparison.OrdinalIgnoreCase));
                normalized.Add(new DentalXRayTobaccoUseDetail
                {
                    Type = type,
                    Used = existing?.Used ?? "No",
                    TimesPerDay = IsYes(existing?.Used) ? existing?.TimesPerDay : null,
                    TimesPerWeek = IsYes(existing?.Used) ? existing?.TimesPerWeek : null
                });
            }

            return normalized;
        }

        private static bool IsYes(string? value)
        {
            return string.Equals(value?.Trim(), "Yes", StringComparison.OrdinalIgnoreCase);
        }
    }
}
