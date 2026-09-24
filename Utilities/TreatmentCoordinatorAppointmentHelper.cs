using ExcelFilesCompiler.Interfaces;
using Malama.Models;
using System.Text.Json;

namespace ExcelFilesCompiler.Utilities
{
    public static class TreatmentCoordinatorAppointmentHelper
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static List<TreatmentCoordinatorAppointmentJsonDto> ParseAppointmentsJson(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<TreatmentCoordinatorAppointmentJsonDto>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<TreatmentCoordinatorAppointmentJsonDto>>(json, JsonOptions)
                    ?? new List<TreatmentCoordinatorAppointmentJsonDto>();
            }
            catch
            {
                return new List<TreatmentCoordinatorAppointmentJsonDto>();
            }
        }

        public static string SerializeAppointments(IEnumerable<TreatmentCoordinatorAppointmentJsonDto> appointments)
        {
            var list = (appointments ?? Enumerable.Empty<TreatmentCoordinatorAppointmentJsonDto>()).ToList();
            return JsonSerializer.Serialize(list, JsonOptions);
        }

        public static string? Validate(
            IReadOnlyList<TreatmentCoordinatorAppointmentJsonDto> appointments,
            IReadOnlyList<DentalFindingDto> findings)
        {
            if (appointments == null || appointments.Count == 0)
            {
                return null;
            }

            var findingByKey = (findings ?? new List<DentalFindingDto>())
                .Where(f => !string.IsNullOrWhiteSpace(f.ClientKey))
                .GroupBy(f => f.ClientKey!.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.Last(), StringComparer.OrdinalIgnoreCase);

            for (var i = 0; i < appointments.Count; i++)
            {
                var appt = appointments[i];
                var prefix = $"Appointment #{i + 1}: ";

                if (!long.TryParse((appt.AssignedDentist ?? string.Empty).Trim(), out var staffId) || staffId <= 0)
                {
                    return prefix + "Assigned dentist is required.";
                }

                if (string.IsNullOrWhiteSpace(appt.AppointmentDate))
                {
                    return prefix + "Appointment date is required.";
                }

                if (!TryParseAppointmentDate(appt.AppointmentDate, out _))
                {
                    return prefix + "Appointment date is invalid.";
                }

                if (string.IsNullOrWhiteSpace(appt.AppointmentStartTime))
                {
                    return prefix + "Start time is required.";
                }

                if (string.IsNullOrWhiteSpace(appt.AppointmentDuration))
                {
                    return prefix + "Duration is required.";
                }

                var keys = (appt.FindingClientKeys ?? new List<string>())
                    .Where(k => !string.IsNullOrWhiteSpace(k))
                    .Select(k => k.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (keys.Count == 0)
                {
                    return prefix + "At least one finding must be assigned.";
                }

                foreach (var key in keys)
                {
                    if (!findingByKey.TryGetValue(key, out var finding))
                    {
                        return prefix + "One or more assigned findings were not found.";
                    }

                    if (!DentalFindingConstants.IsClass3(finding.Classification)
                        || finding.IsTreatmentPossible == false)
                    {
                        return prefix + "Only Class 3 findings with treatment possible can be scheduled.";
                    }
                }
            }

            return null;
        }

        public static bool TryParseAppointmentDate(string? value, out DateTime date)
        {
            date = default;
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var raw = value.Trim();
            if (DateTime.TryParseExact(
                    raw,
                    new[] { "yyyy-MM-dd", "MM/dd/yyyy", "M/d/yyyy" },
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out date))
            {
                date = DateTime.SpecifyKind(date.Date, DateTimeKind.Unspecified);
                return true;
            }

            if (DateTime.TryParse(raw, out date))
            {
                date = DateTime.SpecifyKind(date.Date, DateTimeKind.Unspecified);
                return true;
            }

            return false;
        }

        public static List<TreatmentCoordinatorAppointmentJsonDto> ToJsonDtos(
            IEnumerable<DentalAppointment>? appointments)
        {
            return (appointments ?? Enumerable.Empty<DentalAppointment>())
                .OrderBy(a => a.SortOrder)
                .ThenBy(a => a.Id)
                .Select(a => new TreatmentCoordinatorAppointmentJsonDto
                {
                    Id = a.Id.ToString(),
                    AssignedDentist = a.EventStaffId.ToString(),
                    AppointmentDate = a.AppointmentDate.ToString("yyyy-MM-dd"),
                    AppointmentStartTime = a.AppointmentStartTime,
                    AppointmentDuration = a.AppointmentDuration,
                    FindingClientKeys = (a.Findings ?? new List<DentalAppointmentFinding>())
                        .Select(f => f.FindingClientKey)
                        .Where(k => !string.IsNullOrWhiteSpace(k))
                        .Select(k => k!.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList()
                })
                .ToList();
        }
    }
}
