using System.Text.Json;
using Malama.Models;

namespace ExcelFilesCompiler.Utilities
{
    public static class DentalExamCdtCodeProvider
    {
        private static IReadOnlyDictionary<string, IReadOnlyList<string>>? _cache;
        private static readonly object CacheLock = new();

        public static IReadOnlyDictionary<string, IReadOnlyList<string>> GetCodesByDiseaseType()
        {
            if (_cache != null)
            {
                return _cache;
            }

            lock (CacheLock)
            {
                if (_cache != null)
                {
                    return _cache;
                }

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "dental-cdt-codes.json");
                if (!File.Exists(path))
                {
                    _cache = new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase);
                    return _cache;
                }

                var json = File.ReadAllText(path);
                var raw = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json)
                    ?? new Dictionary<string, List<string>>();

                _cache = raw.ToDictionary(
                    kvp => kvp.Key,
                    kvp => (IReadOnlyList<string>)kvp.Value,
                    StringComparer.OrdinalIgnoreCase);

                return _cache;
            }
        }
    }

    public static class DentalExamFindingBinder
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static List<DentalExamFindingDto> ParseFromJson(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<DentalExamFindingDto>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<DentalExamFindingDto>>(json, JsonOptions)
                    ?? new List<DentalExamFindingDto>();
            }
            catch
            {
                return new List<DentalExamFindingDto>();
            }
        }
    }

    public static class DentalExamFindingValidator
    {
        public static string? ValidateFinding(DentalExamFindingDto finding, int rowNumber)
        {
            var prefix = $"Finding #{rowNumber}: ";

            if (string.IsNullOrWhiteSpace(finding.AffectedTooth))
            {
                return prefix + "Affected Tooth is required.";
            }

            var allowedTeeth = finding.IsPrimaryTooth
                ? DentalExamFindingConstants.PrimaryTeeth
                : DentalExamFindingConstants.PermanentTeeth;

            if (!allowedTeeth.Contains(finding.AffectedTooth.Trim(), StringComparer.OrdinalIgnoreCase))
            {
                return prefix + "Affected Tooth selection is invalid.";
            }

            if (string.IsNullOrWhiteSpace(finding.DiseaseConditionType)
                || !DentalExamFindingConstants.DiseaseConditionTypes.Contains(
                    finding.DiseaseConditionType.Trim(),
                    StringComparer.OrdinalIgnoreCase))
            {
                return prefix + "Disease / Condition Type is required.";
            }

            if (RequiresSurfaces(finding.DiseaseConditionType))
            {
                var allowedSurfaces = GetAllowedSurfaces(finding.DiseaseConditionType, finding.AffectedTooth);

                if (string.Equals(finding.DiseaseConditionType, DentalExamFindingConstants.Restorative, StringComparison.OrdinalIgnoreCase)
                    && allowedSurfaces.Length == 0)
                {
                    return prefix + "Affected Surface(s) require a permanent tooth number (1-32).";
                }

                if (finding.AffectedSurfaces == null || finding.AffectedSurfaces.Count == 0)
                {
                    return prefix + "Affected Surface(s) are required for the selected Disease / Condition Type.";
                }

                if (finding.AffectedSurfaces.Any(s => !allowedSurfaces.Contains(s.Trim(), StringComparer.OrdinalIgnoreCase)))
                {
                    return prefix + "Affected Surface(s) contain invalid values.";
                }
            }

            if (string.IsNullOrWhiteSpace(finding.Classification)
                || !DentalExamFindingConstants.Classifications.Contains(
                    finding.Classification.Trim(),
                    StringComparer.OrdinalIgnoreCase))
            {
                return prefix + "Classification is required.";
            }

            if (string.Equals(finding.DiseaseConditionType, DentalExamFindingConstants.Restorative, StringComparison.OrdinalIgnoreCase)
                && finding.CdtCodes != null
                && finding.CdtCodes.Count > 0)
            {
                var allowedCdtCodes = GetRestorativeCdtCodes(finding.AffectedTooth, finding.AffectedSurfaces?.Count ?? 0);
                if (allowedCdtCodes.Length == 0
                    || finding.CdtCodes.Any(c => !allowedCdtCodes.Contains(c.Trim(), StringComparer.OrdinalIgnoreCase)))
                {
                    return prefix + "CDT Code selection is invalid for the selected tooth and Affected Surface(s).";
                }
            }

            return null;
        }

        public static string? ValidateFindings(IReadOnlyList<DentalExamFindingDto> findings)
        {
            for (var i = 0; i < findings.Count; i++)
            {
                var error = ValidateFinding(findings[i], i + 1);
                if (error != null)
                {
                    return error;
                }
            }

            return null;
        }

        public static bool RequiresSurfaces(string? diseaseConditionType)
        {
            return string.Equals(diseaseConditionType, DentalExamFindingConstants.Periodontal, StringComparison.OrdinalIgnoreCase)
                || string.Equals(diseaseConditionType, DentalExamFindingConstants.Restorative, StringComparison.OrdinalIgnoreCase);
        }

        public static string[] GetAllowedSurfaces(string? diseaseConditionType, string? affectedTooth = null)
        {
            if (string.Equals(diseaseConditionType, DentalExamFindingConstants.Periodontal, StringComparison.OrdinalIgnoreCase))
            {
                return DentalExamFindingConstants.PeriodontalSurfaces;
            }

            if (string.Equals(diseaseConditionType, DentalExamFindingConstants.Restorative, StringComparison.OrdinalIgnoreCase))
            {
                return GetRestorativeSurfacesForTooth(affectedTooth);
            }

            return Array.Empty<string>();
        }

        /// <summary>
        /// Posterior (1-5, 12-21, 28-32): M,D,L,B,O.
        /// Anterior (6-11, 22-27): M,D,L,F,I.
        /// </summary>
        public static string[] GetRestorativeSurfacesForTooth(string? affectedTooth)
        {
            if (!int.TryParse(affectedTooth?.Trim(), out var tooth) || tooth < 1 || tooth > 32)
            {
                return Array.Empty<string>();
            }

            if ((tooth >= 6 && tooth <= 11) || (tooth >= 22 && tooth <= 27))
            {
                return DentalExamFindingConstants.RestorativeSurfacesAnterior;
            }

            if ((tooth >= 1 && tooth <= 5) || (tooth >= 12 && tooth <= 21) || (tooth >= 28 && tooth <= 32))
            {
                return DentalExamFindingConstants.RestorativeSurfacesPosterior;
            }

            return Array.Empty<string>();
        }

        /// <summary>
        /// Restorative CDT options by tooth group (anterior/posterior) and selected surface count.
        /// </summary>
        public static string[] GetRestorativeCdtCodes(string? affectedTooth, int surfaceCount)
        {
            if (surfaceCount < 1
                || !int.TryParse(affectedTooth?.Trim(), out var tooth)
                || tooth < 1
                || tooth > 32)
            {
                return Array.Empty<string>();
            }

            var isAnterior = (tooth >= 6 && tooth <= 11) || (tooth >= 22 && tooth <= 27);
            var isPosterior = (tooth >= 1 && tooth <= 5)
                || (tooth >= 12 && tooth <= 21)
                || (tooth >= 28 && tooth <= 32);

            if (!isAnterior && !isPosterior)
            {
                return Array.Empty<string>();
            }

            var amalgam = surfaceCount switch
            {
                1 => "D2140 - Amalgam-One Surface",
                2 => "D2150 - Amalgam-Two Surfaces",
                3 => "D2160 - Amalgam-Three Surfaces",
                _ => "D2161 - Amalgam-Four or More Surfaces"
            };

            var resin = isAnterior
                ? surfaceCount switch
                {
                    1 => "D2330 - Resin Based Composite-One Surface, Anterior",
                    2 => "D2331 - Resin Based Composite-Two Surfaces, Anterior",
                    3 => "D2332 - Resin Based Composite-Three Surfaces, Anterior",
                    _ => "D2335 - Resin Based Composite-Four or More Surfaces, Anterior"
                }
                : surfaceCount switch
                {
                    1 => "D2391 - Resin Based Composite-One Surface, Posterior",
                    2 => "D2392 - Resin Based Composite-Two Surfaces, Posterior",
                    3 => "D2393 - Resin Based Composite-Three Surfaces, Posterior",
                    _ => "D2394 - Resin Based Composite-Four or More Surfaces, Posterior"
                };

            return new[] { amalgam, resin };
        }
    }
}
