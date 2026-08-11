using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Malama.Models
{
    public static class DentalExamFindingConstants
    {
        public static readonly string[] DiseaseConditionTypes =
        {
            "Restorative",
            "Oral Surgery",
            "Periodontal",
            "Endodontic",
            "Prosthodontic",
            "Orthodontic",
            "Diagnostic",
            "Special Codes"
        };

        public static readonly string[] PeriodontalSurfaces = { "UL", "UR", "LL", "LR" };

        /// <summary>Restorative surfaces for posterior teeth (1-5, 12-21, 28-32 and primary A,B,I,J,K,L,S,T).</summary>
        public static readonly string[] RestorativeSurfacesPosterior = { "M", "D", "L", "B", "O" };

        /// <summary>Restorative surfaces for anterior teeth (6-11, 22-27 and primary C-H, M-R).</summary>
        public static readonly string[] RestorativeSurfacesAnterior = { "M", "D", "L", "F", "I" };

        public static readonly string[] PermanentTeeth =
            Enumerable.Range(1, 32).Select(i => i.ToString()).ToArray();

        public static readonly string[] PrimaryTeeth =
        {
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T"
        };

        public const string ClassificationClass2 =
            "Class 2 - Treatment needed but not expected within 12 months";

        public const string ClassificationClass3 = "Class 3 - Urgent treatment needed";

        public static readonly string[] Classifications = { ClassificationClass2, ClassificationClass3 };

        public const string Periodontal = "Periodontal";
        public const string Restorative = "Restorative";
    }

    [Table("DentalExamFinding")]
    public class DentalExamFinding
    {
        public long Id { get; set; }

        public long DentalExamId { get; set; }

        [JsonIgnore]
        public virtual DentalExam DentalExam { get; set; } = null!;

        public bool IsPrimaryTooth { get; set; }

        public string AffectedTooth { get; set; } = string.Empty;

        public string DiseaseConditionType { get; set; } = string.Empty;

        public string? AffectedSurfacesJson { get; set; }

        public string? CdtCodesJson { get; set; }

        public string? CdtCodesNotes { get; set; }

        public string? DescriptionDetails { get; set; }

        public string? Classification { get; set; }

        public int SortOrder { get; set; }

        public string? ExaminationAddedBy { get; set; }

        public DateTime? ExaminationAddedOn { get; set; }

        public string? ExaminationUpdatedBy { get; set; }

        public DateTime? ExaminationUpdatedOn { get; set; }
    }

    public class DentalExamFindingDto
    {
        public long Id { get; set; }

        public bool IsPrimaryTooth { get; set; }

        public string? AffectedTooth { get; set; }

        public string? DiseaseConditionType { get; set; }

        public List<string> AffectedSurfaces { get; set; } = new();

        public List<string> CdtCodes { get; set; } = new();

        public string? CdtCodesNotes { get; set; }

        public string? DescriptionDetails { get; set; }

        public string? Classification { get; set; }

        public int SortOrder { get; set; }

        public string? ExaminationAddedBy { get; set; }

        public DateTime? ExaminationAddedOn { get; set; }

        public string? ExaminationUpdatedBy { get; set; }

        public DateTime? ExaminationUpdatedOn { get; set; }
    }

    public static class DentalExamFindingMapper
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static DentalExamFindingDto ToDto(DentalExamFinding entity)
        {
            return new DentalExamFindingDto
            {
                Id = entity.Id,
                IsPrimaryTooth = entity.IsPrimaryTooth,
                AffectedTooth = entity.AffectedTooth,
                DiseaseConditionType = entity.DiseaseConditionType,
                AffectedSurfaces = DeserializeList(entity.AffectedSurfacesJson),
                CdtCodes = DeserializeList(entity.CdtCodesJson),
                CdtCodesNotes = entity.CdtCodesNotes,
                DescriptionDetails = entity.DescriptionDetails,
                Classification = entity.Classification,
                SortOrder = entity.SortOrder,
                ExaminationAddedBy = entity.ExaminationAddedBy,
                ExaminationAddedOn = entity.ExaminationAddedOn,
                ExaminationUpdatedBy = entity.ExaminationUpdatedBy,
                ExaminationUpdatedOn = entity.ExaminationUpdatedOn
            };
        }

        public static DentalExamFinding ToEntity(DentalExamFindingDto dto, long dentalExamId, int sortOrder)
        {
            return new DentalExamFinding
            {
                DentalExamId = dentalExamId,
                IsPrimaryTooth = dto.IsPrimaryTooth,
                AffectedTooth = dto.AffectedTooth?.Trim() ?? string.Empty,
                DiseaseConditionType = dto.DiseaseConditionType?.Trim() ?? string.Empty,
                AffectedSurfacesJson = SerializeList(dto.AffectedSurfaces),
                CdtCodesJson = SerializeList(dto.CdtCodes),
                CdtCodesNotes = dto.CdtCodesNotes?.Trim(),
                DescriptionDetails = dto.DescriptionDetails?.Trim(),
                Classification = dto.Classification?.Trim(),
                SortOrder = sortOrder,
                ExaminationAddedBy = dto.ExaminationAddedBy,
                ExaminationAddedOn = dto.ExaminationAddedOn,
                ExaminationUpdatedBy = dto.ExaminationUpdatedBy,
                ExaminationUpdatedOn = dto.ExaminationUpdatedOn
            };
        }

        public static string SerializeList(IEnumerable<string>? values)
        {
            var list = values?
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Select(v => v.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList() ?? new List<string>();

            return list.Count == 0 ? "[]" : JsonSerializer.Serialize(list, JsonOptions);
        }

        public static List<string> DeserializeList(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<string>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<string>>(json, JsonOptions) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}
