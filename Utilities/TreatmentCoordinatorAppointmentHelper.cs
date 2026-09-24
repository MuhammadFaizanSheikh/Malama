using ExcelFilesCompiler.Interfaces;
using Malama.Models;
using System.Globalization;
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

        private static readonly IReadOnlyDictionary<string, int> DurationMinutesByLabel =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["15 min"] = 15,
                ["30 min"] = 30,
                ["45 min"] = 45,
                ["1 hr"] = 60,
                ["1 hr 15 min"] = 75,
                ["1 hr 30 min"] = 90,
                ["1 hr 45 min"] = 105,
                ["2 hr"] = 120,
                ["2 hr 15 min"] = 135,
                ["2 hr 30 min"] = 150,
                ["2 hr 45 min"] = 165,
                ["3 hr"] = 180
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

        public static string SerializeEventAppointments(IEnumerable<TreatmentCoordinatorEventAppointmentDto> appointments)
        {
            var list = (appointments ?? Enumerable.Empty<TreatmentCoordinatorEventAppointmentDto>()).ToList();
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

                if (TryGetAppointmentWindow(appt) == null)
                {
                    return prefix + "Start time or duration is invalid.";
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

            return ValidateScheduleConflicts(appointments, Array.Empty<TreatmentCoordinatorEventAppointmentDto>());
        }

        /// <summary>
        /// Same SM: any time overlap is a conflict.
        /// Cross SM: conflict only when the same dentist overlaps.
        /// Adjacent windows (e.g. 3:00–3:30 and 3:30–4:00) are allowed.
        /// </summary>
        public static string? ValidateScheduleConflicts(
            IReadOnlyList<TreatmentCoordinatorAppointmentJsonDto> currentAppointments,
            IReadOnlyList<TreatmentCoordinatorEventAppointmentDto> otherEventAppointments)
        {
            var currentWindows = (currentAppointments ?? Array.Empty<TreatmentCoordinatorAppointmentJsonDto>())
                .Select((appt, index) => new { Index = index, Window = TryGetAppointmentWindow(appt) })
                .Where(x => x.Window != null)
                .ToList();

            for (var i = 0; i < currentWindows.Count; i++)
            {
                for (var j = i + 1; j < currentWindows.Count; j++)
                {
                    if (WindowsOverlap(currentWindows[i].Window!, currentWindows[j].Window!))
                    {
                        return "This service member already has an overlapping appointment in that time window.";
                    }
                }
            }

            var otherWindows = (otherEventAppointments ?? Array.Empty<TreatmentCoordinatorEventAppointmentDto>())
                .Select(TryGetAppointmentWindow)
                .Where(w => w != null)
                .Cast<AppointmentWindow>()
                .ToList();

            foreach (var current in currentWindows)
            {
                foreach (var other in otherWindows)
                {
                    if (!string.Equals(current.Window!.DentistId, other.DentistId, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (!WindowsOverlap(current.Window, other))
                    {
                        continue;
                    }

                    var dentistLabel = !string.IsNullOrWhiteSpace(other.DentistDisplayName)
                        ? other.DentistDisplayName
                        : "the selected dentist";
                    var memberLabel = string.IsNullOrWhiteSpace(other.ServiceMemberName)
                        ? "another service member"
                        : other.ServiceMemberName;
                    return $"{dentistLabel} is already booked for {memberLabel} from {FormatMinutes(other.StartMinutes)} to {FormatMinutes(other.EndMinutes)} on {FormatDate(other.Date)}.";
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
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
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

        public static TreatmentCoordinatorEventAppointmentDto ToEventDto(
            DentalAppointment appointment,
            string serviceMemberName,
            string? dentistDisplayName = null)
        {
            return new TreatmentCoordinatorEventAppointmentDto
            {
                Id = appointment.Id,
                ServiceMembersChildId = appointment.DentalTreatmentCoordinator?.ServiceMembersChildId ?? 0,
                ServiceMemberName = serviceMemberName?.Trim() ?? string.Empty,
                AssignedDentist = appointment.EventStaffId.ToString(),
                DentistDisplayName = dentistDisplayName?.Trim() ?? string.Empty,
                AppointmentDate = appointment.AppointmentDate.ToString("yyyy-MM-dd"),
                AppointmentStartTime = appointment.AppointmentStartTime,
                AppointmentDuration = appointment.AppointmentDuration
            };
        }

        public static int? TryParseDurationMinutes(string? duration)
        {
            if (string.IsNullOrWhiteSpace(duration))
            {
                return null;
            }

            return DurationMinutesByLabel.TryGetValue(duration.Trim(), out var minutes) ? minutes : null;
        }

        public static int? TryParseStartMinutes(string? startTime)
        {
            if (string.IsNullOrWhiteSpace(startTime))
            {
                return null;
            }

            var raw = startTime.Trim();
            if (DateTime.TryParseExact(
                    raw,
                    new[] { "h:mm tt", "hh:mm tt", "H:mm", "HH:mm" },
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsed))
            {
                return (parsed.Hour * 60) + parsed.Minute;
            }

            if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            {
                return (parsed.Hour * 60) + parsed.Minute;
            }

            return null;
        }

        private static AppointmentWindow? TryGetAppointmentWindow(TreatmentCoordinatorAppointmentJsonDto appt)
        {
            if (appt == null)
            {
                return null;
            }

            var dentist = (appt.AssignedDentist ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(dentist)
                || !TryParseAppointmentDate(appt.AppointmentDate, out var date)
                || TryParseStartMinutes(appt.AppointmentStartTime) is not int start
                || TryParseDurationMinutes(appt.AppointmentDuration) is not int duration
                || duration <= 0)
            {
                return null;
            }

            return new AppointmentWindow
            {
                DentistId = dentist,
                Date = date.ToString("yyyy-MM-dd"),
                StartMinutes = start,
                EndMinutes = start + duration,
                ServiceMemberName = null
            };
        }

        private static AppointmentWindow? TryGetAppointmentWindow(TreatmentCoordinatorEventAppointmentDto appt)
        {
            if (appt == null)
            {
                return null;
            }

            var dentist = (appt.AssignedDentist ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(dentist)
                || !TryParseAppointmentDate(appt.AppointmentDate, out var date)
                || TryParseStartMinutes(appt.AppointmentStartTime) is not int start
                || TryParseDurationMinutes(appt.AppointmentDuration) is not int duration
                || duration <= 0)
            {
                return null;
            }

            return new AppointmentWindow
            {
                DentistId = dentist,
                DentistDisplayName = (appt.DentistDisplayName ?? string.Empty).Trim(),
                Date = date.ToString("yyyy-MM-dd"),
                StartMinutes = start,
                EndMinutes = start + duration,
                ServiceMemberName = appt.ServiceMemberName
            };
        }

        private static bool WindowsOverlap(AppointmentWindow a, AppointmentWindow b)
        {
            return string.Equals(a.Date, b.Date, StringComparison.OrdinalIgnoreCase)
                && a.StartMinutes < b.EndMinutes
                && b.StartMinutes < a.EndMinutes;
        }

        private static string FormatMinutes(int totalMinutes)
        {
            var minutesInDay = ((totalMinutes % (24 * 60)) + (24 * 60)) % (24 * 60);
            var hour = minutesInDay / 60;
            var minute = minutesInDay % 60;
            var meridiem = hour >= 12 ? "PM" : "AM";
            hour %= 12;
            if (hour == 0)
            {
                hour = 12;
            }

            return $"{hour}:{minute:D2} {meridiem}";
        }

        private static string FormatDate(string yyyyMmDd)
        {
            if (TryParseAppointmentDate(yyyyMmDd, out var date))
            {
                return date.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            }

            return yyyyMmDd;
        }

        private sealed class AppointmentWindow
        {
            public string DentistId { get; set; } = string.Empty;
            public string DentistDisplayName { get; set; } = string.Empty;
            public string Date { get; set; } = string.Empty;
            public int StartMinutes { get; set; }
            public int EndMinutes { get; set; }
            public string? ServiceMemberName { get; set; }
        }
    }
}
