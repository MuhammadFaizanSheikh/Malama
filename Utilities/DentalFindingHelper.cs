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

    public static class DentalFindingBinder
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static List<DentalFindingDto> ParseFromJson(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<DentalFindingDto>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<DentalFindingDto>>(json, JsonOptions)
                    ?? new List<DentalFindingDto>();
            }
            catch
            {
                return new List<DentalFindingDto>();
            }
        }
    }

    public static class DentalFindingValidator
    {
        public static string? ValidateFinding(
            DentalFindingDto finding,
            int rowNumber,
            IReadOnlyCollection<int>? missingTeeth = null)
        {
            var prefix = $"Finding #{rowNumber}: ";

            if (string.IsNullOrWhiteSpace(finding.AffectedTooth))
            {
                return prefix + "Affected Tooth is required.";
            }

            var allowedTeeth = finding.IsPrimaryTooth
                ? DentalFindingConstants.PrimaryTeeth
                : DentalFindingConstants.PermanentTeeth;

            if (!allowedTeeth.Contains(finding.AffectedTooth.Trim(), StringComparer.OrdinalIgnoreCase))
            {
                return prefix + "Affected Tooth selection is invalid.";
            }

            if (IsMissingAffectedTooth(finding.AffectedTooth, finding.IsPrimaryTooth, missingTeeth))
            {
                return prefix + "Affected Tooth cannot be a missing tooth selected in PSR.";
            }

            if (string.IsNullOrWhiteSpace(finding.DiseaseConditionType)
                || !DentalFindingConstants.DiseaseConditionTypes.Contains(
                    finding.DiseaseConditionType.Trim(),
                    StringComparer.OrdinalIgnoreCase))
            {
                return prefix + "Disease / Condition Type is required.";
            }

            if (RequiresSurfaces(finding.DiseaseConditionType))
            {
                var allowedSurfaces = GetAllowedSurfaces(finding.DiseaseConditionType, finding.AffectedTooth);

                if (string.Equals(finding.DiseaseConditionType, DentalFindingConstants.Restorative, StringComparison.OrdinalIgnoreCase)
                    && allowedSurfaces.Length == 0)
                {
                    return prefix + "Affected Surface(s) require a valid permanent (1-32) or primary tooth.";
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
                || !DentalFindingConstants.Classifications.Contains(
                    finding.Classification.Trim(),
                    StringComparer.OrdinalIgnoreCase))
            {
                return prefix + "Classification is required.";
            }

            if (finding.CdtCodes == null || finding.CdtCodes.Count == 0)
            {
                return prefix + "CDT Code is required.";
            }

            if (string.Equals(finding.DiseaseConditionType, DentalFindingConstants.Restorative, StringComparison.OrdinalIgnoreCase))
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

        public static string? ValidateFindings(
            IReadOnlyList<DentalFindingDto> findings,
            IReadOnlyCollection<int>? missingTeeth = null)
        {
            for (var i = 0; i < findings.Count; i++)
            {
                var error = ValidateFinding(findings[i], i + 1, missingTeeth);
                if (error != null)
                {
                    return error;
                }
            }

            var missingConflict = ValidateMissingTeethNotUsedInFindings(findings, missingTeeth);
            if (missingConflict != null)
            {
                return missingConflict;
            }

            return null;
        }

        public static string? ValidateMissingTeethNotUsedInFindings(
            IReadOnlyList<DentalFindingDto> findings,
            IReadOnlyCollection<int>? missingTeeth)
        {
            if (findings == null || findings.Count == 0 || missingTeeth == null || missingTeeth.Count == 0)
            {
                return null;
            }

            var usedPermanentTeeth = new HashSet<int>();
            foreach (var finding in findings)
            {
                var permanent = ResolveAffectedToothToPermanentNumber(finding.AffectedTooth, finding.IsPrimaryTooth);
                if (permanent.HasValue)
                {
                    usedPermanentTeeth.Add(permanent.Value);
                }
            }

            foreach (var missingTooth in missingTeeth.Distinct().OrderBy(t => t))
            {
                if (usedPermanentTeeth.Contains(missingTooth))
                {
                    return $"Tooth {missingTooth} cannot be marked missing because it is already used in a Dental Finding.";
                }
            }

            return null;
        }

        public static int? ResolveAffectedToothToPermanentNumber(string? affectedTooth, bool isPrimaryTooth)
        {
            if (string.IsNullOrWhiteSpace(affectedTooth))
            {
                return null;
            }

            var tooth = affectedTooth.Trim();
            if (isPrimaryTooth)
            {
                return DentalFindingConstants.PrimaryToothToPermanentNumber.TryGetValue(tooth, out var permanentNumber)
                    ? permanentNumber
                    : null;
            }

            return int.TryParse(tooth, out var permanentTooth) ? permanentTooth : null;
        }

        public static bool IsMissingAffectedTooth(
            string? affectedTooth,
            bool isPrimaryTooth,
            IReadOnlyCollection<int>? missingTeeth)
        {
            if (missingTeeth == null || missingTeeth.Count == 0)
            {
                return false;
            }

            var permanent = ResolveAffectedToothToPermanentNumber(affectedTooth, isPrimaryTooth);
            return permanent.HasValue && missingTeeth.Contains(permanent.Value);
        }

        public static bool RequiresSurfaces(string? diseaseConditionType)
        {
            return string.Equals(diseaseConditionType, DentalFindingConstants.Periodontal, StringComparison.OrdinalIgnoreCase)
                || string.Equals(diseaseConditionType, DentalFindingConstants.Restorative, StringComparison.OrdinalIgnoreCase);
        }

        public static string[] GetAllowedSurfaces(string? diseaseConditionType, string? affectedTooth = null)
        {
            if (string.Equals(diseaseConditionType, DentalFindingConstants.Periodontal, StringComparison.OrdinalIgnoreCase))
            {
                return DentalFindingConstants.PeriodontalSurfaces;
            }

            if (string.Equals(diseaseConditionType, DentalFindingConstants.Restorative, StringComparison.OrdinalIgnoreCase))
            {
                return GetRestorativeSurfacesForTooth(affectedTooth);
            }

            return Array.Empty<string>();
        }

        /// <summary>
        /// Posterior permanent (1-5, 12-21, 28-32) / primary (A,B,I,J,K,L,S,T): M,D,L,B,O.
        /// Anterior permanent (6-11, 22-27) / primary (C-H, M-R): M,D,L,F,I.
        /// Primary letter groups map to permanent ranges:
        /// 1-5 → A,B | 6-11 → C-H | 12-21 → I,J,K,L | 22-27 → M-R | 28-32 → S,T.
        /// </summary>
        public static string[] GetRestorativeSurfacesForTooth(string? affectedTooth)
        {
            return ClassifyRestorativeToothGroup(affectedTooth) switch
            {
                RestorativeToothGroup.Anterior => DentalFindingConstants.RestorativeSurfacesAnterior,
                RestorativeToothGroup.Posterior => DentalFindingConstants.RestorativeSurfacesPosterior,
                _ => Array.Empty<string>()
            };
        }

        /// <summary>
        /// Restorative CDT options by tooth group (anterior/posterior) and selected surface count.
        /// Supports permanent 1-32 and primary A-T (same groups as surfaces).
        /// </summary>
        public static string[] GetRestorativeCdtCodes(string? affectedTooth, int surfaceCount)
        {
            if (surfaceCount < 1)
            {
                return Array.Empty<string>();
            }

            var group = ClassifyRestorativeToothGroup(affectedTooth);
            if (group == RestorativeToothGroup.Unknown)
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

            var resin = group == RestorativeToothGroup.Anterior
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

        private enum RestorativeToothGroup
        {
            Unknown = 0,
            Anterior = 1,
            Posterior = 2
        }

        /// <summary>
        /// Classifies permanent (1-32) and primary (A-T) teeth into anterior/posterior surface groups.
        /// </summary>
        private static RestorativeToothGroup ClassifyRestorativeToothGroup(string? affectedTooth)
        {
            var tooth = affectedTooth?.Trim() ?? string.Empty;
            if (tooth.Length == 0)
            {
                return RestorativeToothGroup.Unknown;
            }

            if (int.TryParse(tooth, out var n))
            {
                if ((n >= 6 && n <= 11) || (n >= 22 && n <= 27))
                {
                    return RestorativeToothGroup.Anterior;
                }

                if ((n >= 1 && n <= 5) || (n >= 12 && n <= 21) || (n >= 28 && n <= 32))
                {
                    return RestorativeToothGroup.Posterior;
                }

                return RestorativeToothGroup.Unknown;
            }

            // Primary letters: 1-5→A,B | 6-11→C-H | 12-21→I,J,K,L | 22-27→M-R | 28-32→S,T
            var letter = tooth.ToUpperInvariant();
            return letter switch
            {
                "C" or "D" or "E" or "F" or "G" or "H"
                    or "M" or "N" or "O" or "P" or "Q" or "R"
                    => RestorativeToothGroup.Anterior,
                "A" or "B" or "I" or "J" or "K" or "L" or "S" or "T"
                    => RestorativeToothGroup.Posterior,
                _ => RestorativeToothGroup.Unknown
            };
        }

        /// <summary>
        /// Anterior: permanent 6-11, 22-27; primary C-H, M-R.
        /// </summary>
        public static bool IsRestorativeAnteriorTooth(string? affectedTooth)
            => ClassifyRestorativeToothGroup(affectedTooth) == RestorativeToothGroup.Anterior;

        /// <summary>
        /// Posterior: permanent 1-5, 12-21, 28-32; primary A,B,I,J,K,L,S,T.
        /// </summary>
        public static bool IsRestorativePosteriorTooth(string? affectedTooth)
            => ClassifyRestorativeToothGroup(affectedTooth) == RestorativeToothGroup.Posterior;
    }
}
