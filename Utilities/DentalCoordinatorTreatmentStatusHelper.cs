using ExcelFilesCompiler.Controllers.Services;
using Malama.Models;

namespace ExcelFilesCompiler.Utilities
{
    public static class DentalCoordinatorTreatmentStatusHelper
    {
        public const string NotApplicable = "N/A";

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
    }
}
