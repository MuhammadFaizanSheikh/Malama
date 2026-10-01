using Malama.Models;

namespace ExcelFilesCompiler.Utilities
{
    public static class DentalTreatmentEligibilityHelper
    {
        public static bool IsCheckedIn(ServiceMembersChild? serviceMember)
        {
            return serviceMember != null
                && string.Equals(
                    serviceMember.CheckIn?.Trim(),
                    AppConstants.YesNo.Yes,
                    StringComparison.OrdinalIgnoreCase);
        }

        public static bool HasAppointmentForDentist(
            DentalTreatmentCoordinator? coordinator,
            long eventStaffId)
        {
            if (coordinator?.Appointments == null || eventStaffId <= 0)
            {
                return false;
            }

            return coordinator.Appointments.Any(a => a.EventStaffId == eventStaffId);
        }

        public static HashSet<long> GetAssignedExamFindingIds(
            IEnumerable<DentalAppointment>? appointments,
            long eventStaffId)
        {
            var ids = new HashSet<long>();
            if (appointments == null || eventStaffId <= 0)
            {
                return ids;
            }

            foreach (var appointment in appointments.Where(a => a.EventStaffId == eventStaffId))
            {
                foreach (var link in appointment.Findings ?? Enumerable.Empty<DentalAppointmentFinding>())
                {
                    if (link.DentalFindingId > 0)
                    {
                        ids.Add(link.DentalFindingId);
                    }
                }
            }

            return ids;
        }

        public static bool IsOwnedTreatmentOnlyFinding(
            DentalTreatmentFinding? finding,
            string? userId)
        {
            if (finding == null || string.IsNullOrWhiteSpace(userId))
            {
                return false;
            }

            if (!DentalTreatmentFindingOrigin.IsTreatmentOrigin(finding.Origin)
                && finding.DentalFindingId.GetValueOrDefault() > 0)
            {
                return false;
            }

            return string.Equals(
                finding.DentistProfessional?.Trim(),
                userId.Trim(),
                StringComparison.Ordinal);
        }

        public static bool IsInCurrentDentistFindingScope(
            DentalTreatmentFinding? finding,
            ISet<long> assignedExamFindingIds,
            string? userId)
        {
            if (finding == null)
            {
                return false;
            }

            var examFindingId = finding.DentalFindingId.GetValueOrDefault();
            if (DentalTreatmentFindingOrigin.IsExamOrigin(finding.Origin) || examFindingId > 0)
            {
                return examFindingId > 0
                    && assignedExamFindingIds != null
                    && assignedExamFindingIds.Contains(examFindingId);
            }

            return IsOwnedTreatmentOnlyFinding(finding, userId);
        }
    }
}
