using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Malama.Models
{
    public static class DentalFindingConstants
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

        /// <summary>
        /// Primary letter to permanent tooth number (aligned with PSR tooth chart labels).
        /// </summary>
        public static readonly IReadOnlyDictionary<string, int> PrimaryToothToPermanentNumber =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["A"] = 4, ["B"] = 5, ["C"] = 6, ["D"] = 7, ["E"] = 8,
                ["F"] = 9, ["G"] = 10, ["H"] = 11, ["I"] = 12, ["J"] = 13,
                ["K"] = 20, ["L"] = 21, ["M"] = 22, ["N"] = 23, ["O"] = 24,
                ["P"] = 25, ["Q"] = 26, ["R"] = 27, ["S"] = 28, ["T"] = 29
            };

        public const string ClassificationClass2 =
            "Class 2 - Treatment needed but not expected within 12 months";

        public const string ClassificationClass3 = "Class 3 - Urgent treatment needed";

        public static readonly string[] Classifications = { ClassificationClass2, ClassificationClass3 };

        public static bool IsClass3(string? classification)
        {
            return string.Equals(
                classification?.Trim(),
                ClassificationClass3,
                StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsClass2(string? classification)
        {
            return string.Equals(
                classification?.Trim(),
                ClassificationClass2,
                StringComparison.OrdinalIgnoreCase);
        }

        public const string Periodontal = "Periodontal";
        public const string Restorative = "Restorative";

        public const string ReasonCommandExcused = "Command Excused";
        public const string ReasonTreatmentPlanInProgress = "Treatment Plan In Progress";

        public static readonly string[] TreatmentNotPossibleReasons =
        {
            ReasonCommandExcused,
            "Service Not Offered at Event",
            ReasonTreatmentPlanInProgress,
            "Extraneous Factor",
            "Missing Equipment",
            "Service Member Refused",
            "Dentist Referred After Review",
            "Not Enough Time"
        };
    }

    [Table("DentalFinding")]
    public class DentalFinding
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

        public string? ExternalExaminerName { get; set; }

        public DateTime? ExternalExamDateTime { get; set; }

        public string? ExternalDentistRemarks { get; set; }

        /// <summary>Where the finding was captured (Dental Exam station vs Treatment Coordinator).</summary>
        public string? Source { get; set; }

        /// <summary>
        /// Treatment Coordinator: whether treatment is possible for a Class 3 finding.
        /// Null for Class 2 (treatment-possible question does not apply).
        /// </summary>
        public bool? IsTreatmentPossible { get; set; }

        /// <summary>Required when <see cref="IsTreatmentPossible"/> is false.</summary>
        public string? TreatmentNotPossibleReason { get; set; }

        /// <summary>Required when reason is Command Excused.</summary>
        public string? TreatmentNotPossibleCommandName { get; set; }

        /// <summary>Required when reason is Treatment Plan In Progress (date only).</summary>
        public DateTime? TreatmentPlanNextAppointmentDate { get; set; }

        /// <summary>Client-side stable key for appointment assignment (Treatment Coordinator).</summary>
        public string? ClientKey { get; set; }
    }

    public static class DentalFindingSources
    {
        public const string DentalExam = "DentalExam";
        public const string DentalCoordinator = "DentalCoordinator";
    }

    public class DentalFindingDto
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

        public string? ExternalExaminerName { get; set; }

        public DateTime? ExternalExamDateTime { get; set; }

        public string? ExternalDentistRemarks { get; set; }

        public string? Source { get; set; }

        /// <summary>Null means Yes/true when omitted from older JSON payloads.</summary>
        public bool? IsTreatmentPossible { get; set; }

        public string? TreatmentNotPossibleReason { get; set; }

        /// <summary>Required when reason is Command Excused.</summary>
        public string? TreatmentNotPossibleCommandName { get; set; }

        /// <summary>Required when reason is Treatment Plan In Progress (date only).</summary>
        public DateTime? TreatmentPlanNextAppointmentDate { get; set; }

        /// <summary>Client-side stable key for appointment assignment (Treatment Coordinator).</summary>
        public string? ClientKey { get; set; }
    }

    public static class DentalFindingMapper
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static DentalFindingDto ToDto(DentalFinding entity)
        {
            return new DentalFindingDto
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
                ExaminationUpdatedOn = entity.ExaminationUpdatedOn,
                ExternalExaminerName = entity.ExternalExaminerName,
                ExternalExamDateTime = entity.ExternalExamDateTime,
                ExternalDentistRemarks = entity.ExternalDentistRemarks,
                Source = entity.Source,
                IsTreatmentPossible = entity.IsTreatmentPossible,
                TreatmentNotPossibleReason = entity.TreatmentNotPossibleReason,
                TreatmentNotPossibleCommandName = entity.TreatmentNotPossibleCommandName,
                TreatmentPlanNextAppointmentDate = entity.TreatmentPlanNextAppointmentDate,
                ClientKey = entity.ClientKey
            };
        }

        public static DentalFinding ToEntity(DentalFindingDto dto, long dentalExamId, int sortOrder)
        {
            var isClass3 = DentalFindingConstants.IsClass3(dto.Classification);
            bool? isTreatmentPossible = isClass3
                ? dto.IsTreatmentPossible ?? true
                : null;
            var reason = isTreatmentPossible == false
                ? dto.TreatmentNotPossibleReason?.Trim()
                : null;

            return new DentalFinding
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
                ExaminationAddedOn = NormalizeDateTime(dto.ExaminationAddedOn),
                ExaminationUpdatedBy = dto.ExaminationUpdatedBy,
                ExaminationUpdatedOn = NormalizeDateTime(dto.ExaminationUpdatedOn),
                ExternalExaminerName = dto.ExternalExaminerName?.Trim(),
                ExternalExamDateTime = NormalizeDateTime(dto.ExternalExamDateTime),
                ExternalDentistRemarks = dto.ExternalDentistRemarks?.Trim(),
                Source = dto.Source?.Trim(),
                IsTreatmentPossible = isTreatmentPossible,
                TreatmentNotPossibleReason = reason,
                TreatmentNotPossibleCommandName = ResolveCommandName(reason, dto.TreatmentNotPossibleCommandName),
                TreatmentPlanNextAppointmentDate = ResolveNextAppointmentDate(reason, dto.TreatmentPlanNextAppointmentDate),
                ClientKey = string.IsNullOrWhiteSpace(dto.ClientKey) ? null : dto.ClientKey.Trim()
            };
        }

        public static string? ResolveCommandName(string? reason, string? commandName)
        {
            if (!string.Equals(reason, DentalFindingConstants.ReasonCommandExcused, StringComparison.Ordinal))
            {
                return null;
            }

            return string.IsNullOrWhiteSpace(commandName) ? null : commandName.Trim();
        }

        public static DateTime? ResolveNextAppointmentDate(string? reason, DateTime? nextAppointmentDate)
        {
            if (!string.Equals(reason, DentalFindingConstants.ReasonTreatmentPlanInProgress, StringComparison.Ordinal))
            {
                return null;
            }

            return NormalizeDateTime(nextAppointmentDate?.Date);
        }

        /// <summary>
        /// Npgsql rejects DateTimeKind.Utc for "timestamp without time zone" columns used across Malama.
        /// </summary>
        public static DateTime? NormalizeDateTime(DateTime? value)
        {
            if (!value.HasValue)
            {
                return null;
            }

            return DateTime.SpecifyKind(value.Value, DateTimeKind.Unspecified);
        }

        public static DateTime NormalizeDateTime(DateTime value)
        {
            return DateTime.SpecifyKind(value, DateTimeKind.Unspecified);
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
