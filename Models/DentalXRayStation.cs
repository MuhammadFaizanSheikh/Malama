using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Malama.Models
{
    public static class BwxUploadMode
    {
        public const string Consolidated = "Consolidated";
        public const string Separate = "Separate";
    }

    public static class DentalXRayQuestionnaire
    {
        public static readonly string[] TobaccoTypes = { "Vape", "Chew", "Cigarettes", "Snuff", "Nicotine" };

        public const string OtherHealthCondition = "Other";

        public static readonly string[] HealthConditions =
        {
            "Hives or skin rash",
            "Epilepsy",
            "Cancer",
            "Kidney disease",
            "Heart trouble/Chest Pain",
            "Heart murmur",
            "High Blood Pressure",
            "Rheumatic fever",
            "Frequent headaches",
            "Asthma/hay fever",
            "Thyroid disease",
            "Anemia/thin blood",
            "Arthritis/rheumatism",
            "Tuberculosis (TB)",
            "Diabetes",
            "Ulcer/stomach",
            "Stroke",
            "Liver disease",
            "Sexually transmitted diseases"
        };
    }

    public class DentalXRayTobaccoUseDetail
    {
        public string Type { get; set; } = string.Empty;
        public string? Used { get; set; }
        public string? TimesPerDay { get; set; }
        public string? TimesPerWeek { get; set; }
    }

    public class DentalXRayHealthConditionDetail
    {
        public string Condition { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
        public string? Detail { get; set; }
    }

    public class DentalXRayStationViewModel
    {
        public List<ServiceMembersChild> FileDataList { get; set; } = new();
    }

    public class DentalXRayStation : GenericProperties
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

        public string? BwxStatus { get; set; }
        public string? BwxReason { get; set; }
        public string? BwxUploadMode { get; set; }
        public DateTime? BwxUploadedDateTime { get; set; }

        public string? BwxConsolidatedFileName { get; set; }
        public string? BwxConsolidatedOriginalFileName { get; set; }
        public DateTime? BwxConsolidatedUploadedDateTime { get; set; }

        public string? BwLeftMolarFileName { get; set; }
        public string? BwLeftMolarOriginalFileName { get; set; }
        public DateTime? BwLeftMolarUploadedDateTime { get; set; }

        public string? BwLeftPremolarFileName { get; set; }
        public string? BwLeftPremolarOriginalFileName { get; set; }
        public DateTime? BwLeftPremolarUploadedDateTime { get; set; }

        public string? BwRightMolarFileName { get; set; }
        public string? BwRightMolarOriginalFileName { get; set; }
        public DateTime? BwRightMolarUploadedDateTime { get; set; }

        public string? BwRightPremolarFileName { get; set; }
        public string? BwRightPremolarOriginalFileName { get; set; }
        public DateTime? BwRightPremolarUploadedDateTime { get; set; }

        public string? PaStatus { get; set; }
        public string? PaReason { get; set; }
        public DateTime? PaUploadedDateTime { get; set; }

        [ValidateNever]
        public virtual ICollection<DentalXRayPaImage> PaImages { get; set; } = new List<DentalXRayPaImage>();

        public string? Comment { get; set; }

        public string Status { get; set; } = "Pending";
    }

    public class DentalXRayPaImage
    {
        public long Id { get; set; }
        public long DentalXRayStationId { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public virtual DentalXRayStation DentalXRayStation { get; set; }

        public string? FileName { get; set; }
        public string? OriginalFileName { get; set; }
        public DateTime? UploadedDateTime { get; set; }
        public int SortOrder { get; set; }
    }

    public class DentalXRayImageUploadModel
    {
        public string Prefix { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string UploadedFieldName { get; set; } = string.Empty;
        public string FileNameFieldName { get; set; } = string.Empty;
        public string OriginalFileNameFieldName { get; set; } = string.Empty;
        public string DateFieldName { get; set; } = string.Empty;
        public string FileInputName { get; set; } = string.Empty;
        public bool Uploaded { get; set; }
        public string? StoredFileName { get; set; }
        public string? OriginalFileName { get; set; }
        public DateTime? UploadedDateTime { get; set; }
        public string RemovedFieldName { get; set; } = string.Empty;
        public string? DownloadPrefix { get; set; }
        public bool IsPaCard { get; set; }
        public int PaCardIndex { get; set; }
        public long PaImageId { get; set; }
        public bool ShowRemovePaCardButton { get; set; }
        public bool IsFullWidth { get; set; }
    }

    public class DentalXRayPaImageDto
    {
        public long Id { get; set; }
        public bool Uploaded { get; set; }
        public string? FileName { get; set; }
        public string? OriginalFileName { get; set; }
        public DateTime? UploadedDateTime { get; set; }
        public int SortOrder { get; set; }
        public IFormFile? ImageFile { get; set; }
        public bool Removed { get; set; }
    }

    public class DentalXRayStationSaveDto
    {
        public long Id { get; set; }
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
        public string? BwxStatus { get; set; }
        public string? BwxReason { get; set; }
        public string? BwxUploadMode { get; set; }
        public DateTime? BwxUploadedDateTime { get; set; }

        public bool BwxConsolidatedUploaded { get; set; }
        public string? BwxConsolidatedFileName { get; set; }
        public string? BwxConsolidatedOriginalFileName { get; set; }
        public DateTime? BwxConsolidatedUploadedDateTime { get; set; }
        public IFormFile? BwxConsolidatedFile { get; set; }
        public bool BwxConsolidatedRemoved { get; set; }

        public bool BwLeftMolarUploaded { get; set; }
        public string? BwLeftMolarFileName { get; set; }
        public string? BwLeftMolarOriginalFileName { get; set; }
        public DateTime? BwLeftMolarUploadedDateTime { get; set; }
        public IFormFile? BwLeftMolarFile { get; set; }

        public bool BwLeftPremolarUploaded { get; set; }
        public string? BwLeftPremolarFileName { get; set; }
        public string? BwLeftPremolarOriginalFileName { get; set; }
        public DateTime? BwLeftPremolarUploadedDateTime { get; set; }
        public IFormFile? BwLeftPremolarFile { get; set; }

        public bool BwRightMolarUploaded { get; set; }
        public string? BwRightMolarFileName { get; set; }
        public string? BwRightMolarOriginalFileName { get; set; }
        public DateTime? BwRightMolarUploadedDateTime { get; set; }
        public IFormFile? BwRightMolarFile { get; set; }

        public bool BwRightPremolarUploaded { get; set; }
        public string? BwRightPremolarFileName { get; set; }
        public string? BwRightPremolarOriginalFileName { get; set; }
        public DateTime? BwRightPremolarUploadedDateTime { get; set; }
        public IFormFile? BwRightPremolarFile { get; set; }

        public string? PaStatus { get; set; }
        public string? PaReason { get; set; }
        public DateTime? PaUploadedDateTime { get; set; }
        public List<DentalXRayPaImageDto> PaImages { get; set; } = new();

        public string? Comment { get; set; }

        public string Status { get; set; } = "Pending";
        public string? SubmissionToken { get; set; }

        public bool GoToVitalStation { get; set; }

        public bool BwLeftMolarRemoved { get; set; }
        public bool BwLeftPremolarRemoved { get; set; }
        public bool BwRightMolarRemoved { get; set; }
        public bool BwRightPremolarRemoved { get; set; }
    }
}
