using ExcelFilesCompiler.Controllers.Services;
using Malama.Models;
using System.Text.Json;

namespace ExcelFilesCompiler.Utilities
{
    public static class DentalCoordinatorTreatmentStatusHelper
    {
        public const string NotApplicable = "N/A";

        private static readonly JsonSerializerOptions AppointmentJsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static string ResolveXRayStatus(ServiceMembersChild? serviceMember, DentalXRayStation? station)
        {
            return ResolveStationStatus(
                isNeeded: DentalXRayStationService.IsNeeded(serviceMember?.BwxNeeded),
                isCheckedIn: IsCheckedIn(serviceMember),
                hasRecord: station != null && station.Id > 0,
                recordStatus: station?.Status);
        }

        public static string ResolveDentalExamStatus(ServiceMembersChild? serviceMember, DentalExam? exam)
        {
            return ResolveStationStatus(
                isNeeded: DentalXRayStationService.IsNeeded(serviceMember?.DentalNeeded),
                isCheckedIn: IsCheckedIn(serviceMember),
                hasRecord: exam != null && exam.Id > 0,
                recordStatus: exam?.Status);
        }

        private static bool IsCheckedIn(ServiceMembersChild? serviceMember)
        {
            return string.Equals(
                serviceMember?.CheckIn?.Trim(),
                AppConstants.YesNo.Yes,
                StringComparison.OrdinalIgnoreCase);
        }

        private static string ResolveStationStatus(
            bool isNeeded,
            bool isCheckedIn,
            bool hasRecord,
            string? recordStatus)
        {
            if (!isNeeded || !isCheckedIn)
            {
                return NotApplicable;
            }

            if (!hasRecord)
            {
                return AppConstants.Status.Pending;
            }

            if (string.Equals(recordStatus?.Trim(), AppConstants.Status.Completed, StringComparison.OrdinalIgnoreCase))
            {
                return AppConstants.Status.Completed;
            }

            return AppConstants.Status.Pending;
        }

        public static bool IsDenClassCompleteForCoordinator(DentalCoordinatorStationSaveDto dto)
        {
            var hasDenClass = !string.IsNullOrWhiteSpace(dto.DenClass)
                && DentalExamDenClass.Options.Contains(dto.DenClass.Trim(), StringComparer.OrdinalIgnoreCase);

            return hasDenClass && !string.IsNullOrWhiteSpace(dto.DenClassReasonComments);
        }

        public static bool AreAllFindingsAppointed(string? findingsJson, string? appointmentsJson)
        {
            var findings = DentalFindingBinder.ParseFromJson(findingsJson);
            if (findings.Count == 0)
            {
                return true;
            }

            var assignedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var appointment in ParseAppointmentsFromJson(appointmentsJson))
            {
                foreach (var key in appointment.FindingClientKeys)
                {
                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        assignedKeys.Add(key.Trim());
                    }
                }
            }

            return findings.All(finding =>
                !string.IsNullOrWhiteSpace(finding.ClientKey)
                && assignedKeys.Contains(finding.ClientKey.Trim()));
        }

        public static string ComputeCoordinatorOverallStatus(DentalCoordinatorStationSaveDto dto)
        {
            if (IsDenClassCompleteForCoordinator(dto)
                && AreAllFindingsAppointed(dto.FindingsJson, dto.AppointmentsJson))
            {
                return AppConstants.Status.Completed;
            }

            return AppConstants.Status.Pending;
        }

        private static List<TreatmentCoordinatorAppointmentJsonDto> ParseAppointmentsFromJson(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<TreatmentCoordinatorAppointmentJsonDto>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<TreatmentCoordinatorAppointmentJsonDto>>(json, AppointmentJsonOptions)
                    ?? new List<TreatmentCoordinatorAppointmentJsonDto>();
            }
            catch
            {
                return new List<TreatmentCoordinatorAppointmentJsonDto>();
            }
        }
    }
}
