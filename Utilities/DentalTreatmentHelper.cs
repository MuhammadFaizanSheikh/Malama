using System.Text.Json;
using Malama.Models;

namespace ExcelFilesCompiler.Utilities
{
    public static class DentalTreatmentJson
    {
        public static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static List<T> ParseList<T>(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<T>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<T>>(json, Options) ?? new List<T>();
            }
            catch
            {
                return new List<T>();
            }
        }

        public static string SerializeList(IEnumerable<string>? values)
        {
            return JsonSerializer.Serialize(values?.Where(v => !string.IsNullOrWhiteSpace(v)).ToList() ?? new List<string>(), Options);
        }

        public static List<string> DeserializeList(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<string>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<string>>(json, Options) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        public static string SerializeDictionary(Dictionary<string, string>? values)
        {
            return JsonSerializer.Serialize(values ?? new Dictionary<string, string>(), Options);
        }

        public static Dictionary<string, string> DeserializeDictionary(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new Dictionary<string, string>();
            }

            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, string>>(json, Options)
                    ?? new Dictionary<string, string>();
            }
            catch
            {
                return new Dictionary<string, string>();
            }
        }
    }

    public static class DentalTreatmentValidator
    {
        private static readonly string[] AllowedSmFinalClassifications = { "1", "2", "3", "4" };
        private static readonly string[] AllowedPrescriptionTypes = { "OTC Recommended", "Prescription" };
        private static readonly string[] FindingOnlyDrcValues = { "1", "2", "4" };

        public static bool IsFindingOnlyDrc(string? drc) =>
            FindingOnlyDrcValues.Contains((drc ?? string.Empty).Trim(), StringComparer.OrdinalIgnoreCase);

        public static bool IsUrgentTreatmentDrc(string? drc) =>
            string.Equals((drc ?? string.Empty).Trim(), "3", StringComparison.OrdinalIgnoreCase);

        public static string? ValidateSaveDto(DentalTreatmentStationSaveDto dto, DentalExam exam)
        {
            if (dto.ServiceMembersChildId <= 0)
            {
                return "Service member is required.";
            }

            if (dto.DentalExamId <= 0 || dto.DentalExamId != exam.Id)
            {
                return "Dental Exam reference is invalid.";
            }

            if (!string.IsNullOrWhiteSpace(dto.SmFinalClassification)
                && !AllowedSmFinalClassifications.Contains(dto.SmFinalClassification.Trim()))
            {
                return "SM Final Classification is invalid.";
            }

            var class3FindingIds = new HashSet<long>(
                (exam.Findings ?? Array.Empty<DentalExamFinding>())
                    .Where(f => string.Equals(f.Classification, DentalExamFindingConstants.ClassificationClass3, StringComparison.OrdinalIgnoreCase))
                    .Select(f => f.Id));

            for (var i = 0; i < dto.Findings.Count; i++)
            {
                var finding = dto.Findings[i];
                var isTreatmentOrigin = DentalTreatmentFindingOrigin.IsTreatmentOrigin(finding.Origin)
                    || finding.IsTreatmentOnly
                    || finding.DentalExamFindingId.GetValueOrDefault() <= 0;
                var examFindingId = finding.DentalExamFindingId.GetValueOrDefault();

                if (!isTreatmentOrigin)
                {
                    if (examFindingId <= 0 || !class3FindingIds.Contains(examFindingId))
                    {
                        return "One or more treatment findings do not match a Class 3 Dental Exam finding.";
                    }

                    continue;
                }

                if (string.IsNullOrWhiteSpace(finding.DiseaseConditionType)
                    || string.IsNullOrWhiteSpace(finding.AffectedTooth))
                {
                    return $"Finding #{i + 1}: Disease / Condition Type and Affected Tooth are required.";
                }

                if (string.IsNullOrWhiteSpace(finding.FinalDrc)
                    || !AllowedSmFinalClassifications.Contains(finding.FinalDrc.Trim()))
                {
                    return $"Finding #{i + 1}: DRC is required.";
                }

                if (IsFindingOnlyDrc(finding.FinalDrc))
                {
                    finding.TreatmentCompleted = null;
                    finding.Reason = null;
                    finding.TreatmentDateTime = null;
                }
                else if (IsUrgentTreatmentDrc(finding.FinalDrc))
                {
                    if (string.IsNullOrWhiteSpace(finding.TreatmentCompleted)
                        || (!string.Equals(finding.TreatmentCompleted, "Yes", StringComparison.OrdinalIgnoreCase)
                            && !string.Equals(finding.TreatmentCompleted, "No", StringComparison.OrdinalIgnoreCase)))
                    {
                        return $"Finding #{i + 1}: Treatment Completed is required.";
                    }

                    if (string.Equals(finding.TreatmentCompleted, "No", StringComparison.OrdinalIgnoreCase)
                        && string.IsNullOrWhiteSpace(finding.Reason))
                    {
                        return $"Finding #{i + 1}: Reason is required.";
                    }

                    if (string.Equals(finding.TreatmentCompleted, "Yes", StringComparison.OrdinalIgnoreCase))
                    {
                        finding.Reason = null;
                        finding.FindingDateTime = null;
                    }
                    else
                    {
                        finding.TreatmentDateTime = null;
                    }
                }

                finding.Classification = DentalExamFindingConstants.ClassificationClass3;
                var surfaces = (finding.PostServiceTreatment != null && finding.PostServiceTreatment.Count > 0)
                    ? finding.PostServiceTreatment
                    : (finding.AffectedSurfaces ?? new List<string>());
                var clinicalError = DentalExamFindingValidator.ValidateFinding(
                    new DentalExamFindingDto
                    {
                        IsPrimaryTooth = finding.IsPrimaryTooth,
                        AffectedTooth = finding.AffectedTooth,
                        DiseaseConditionType = finding.DiseaseConditionType,
                        AffectedSurfaces = surfaces,
                        CdtCodes = (finding.TreatmentCdtCodes != null && finding.TreatmentCdtCodes.Count > 0)
                            ? finding.TreatmentCdtCodes
                            : (finding.CdtCodes ?? new List<string>()),
                        Classification = DentalExamFindingConstants.ClassificationClass3
                    },
                    i + 1,
                    dto.PsrSelectedTeeth);

                if (!string.IsNullOrWhiteSpace(clinicalError))
                {
                    return clinicalError;
                }
            }

            var findingsForMissingCheck = dto.Findings
                .Where(f => !string.IsNullOrWhiteSpace(f.AffectedTooth))
                .Select(f => new DentalExamFindingDto
                {
                    IsPrimaryTooth = f.IsPrimaryTooth,
                    AffectedTooth = f.AffectedTooth
                })
                .ToList();
            var missingConflict = DentalExamFindingValidator.ValidateMissingTeethNotUsedInFindings(
                findingsForMissingCheck,
                dto.PsrSelectedTeeth);
            if (missingConflict != null)
            {
                return missingConflict;
            }

            foreach (var prescription in dto.Prescriptions)
            {
                if (string.IsNullOrWhiteSpace(prescription.Type)
                    || !AllowedPrescriptionTypes.Contains(prescription.Type.Trim(), StringComparer.OrdinalIgnoreCase))
                {
                    return "Prescription type is invalid.";
                }

                if (string.Equals(prescription.Type, "Prescription", StringComparison.OrdinalIgnoreCase)
                    && string.IsNullOrWhiteSpace(prescription.Product))
                {
                    return "Product is required for Prescription records.";
                }
            }

            foreach (var note in dto.OverallNotes)
            {
                if (string.IsNullOrWhiteSpace(note.Notes))
                {
                    return "Overall Notes cannot be empty.";
                }
            }

            return null;
        }

        public static string ComputeStatus(string? smFinalClassification, IEnumerable<DentalTreatmentFindingFormDto>? findings)
        {
            if (string.IsNullOrWhiteSpace(smFinalClassification))
            {
                return AppConstants.Status.Pending;
            }

            var list = findings?.ToList() ?? new List<DentalTreatmentFindingFormDto>();
            foreach (var finding in list)
            {
                var status = ResolveFindingTreatmentStatus(finding);
                if (!string.Equals(status, "Complete", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(status, "Complete with Reason", StringComparison.OrdinalIgnoreCase))
                {
                    return AppConstants.Status.Pending;
                }
            }

            return AppConstants.Status.Completed;
        }

        public static string ResolveFindingTreatmentStatus(DentalTreatmentFindingFormDto finding)
        {
            if (finding == null)
            {
                return "Incomplete";
            }

            if (string.Equals(finding.TreatmentCompleted, "No", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(finding.Reason))
            {
                return "Complete with Reason";
            }

            if (!string.IsNullOrWhiteSpace(finding.FinalDrc))
            {
                return "Complete";
            }

            if (!string.IsNullOrWhiteSpace(finding.TreatmentStatus))
            {
                return finding.TreatmentStatus.Trim();
            }

            return "Incomplete";
        }

        public static List<int> NormalizeSelectedTeeth(IEnumerable<int>? teeth)
        {
            return (teeth ?? Enumerable.Empty<int>())
                .Where(t => t >= 1 && t <= 32)
                .Distinct()
                .OrderBy(t => t)
                .ToList();
        }
    }
}
