using Malama.Models;

namespace ExcelFilesCompiler.Utilities
{
    public static class PostEventImmunizationStationStatusHelper
    {
        public static void ApplyPerVaccineStatuses(
            PostEventImmunizationStationDto post,
            PreEventImmunizationStationDto? pre)
        {
            if (post == null || pre == null)
            {
                return;
            }

            post.HepBStatus = ComputeVaccineStatus(pre.HepBNeeded, post.HepBDataEntered);
            post.HepAStatus = ComputeVaccineStatus(pre.HepANeeded, post.HepADataEntered);
            post.FluStatus = ComputeVaccineStatus(pre.FluNeeded, post.FluDataEntered);
            post.MmrStatus = ComputeVaccineStatus(pre.MmrNeeded, post.MmrDataEntered);
            post.TetTdpStatus = ComputeVaccineStatus(pre.TetTdpNeeded, post.TetTdpDataEntered);
            post.VaricellaStatus = ComputeVaccineStatus(pre.VaricellaNeeded, post.VaricellaDataEntered);

            post.Status = ComputeOverallStatus(pre, post);
        }

        public static string? ComputeVaccineStatus(string? vaccineNeeded, bool dataEntered)
        {
            if (vaccineNeeded != AppConstants.Status.Completed)
            {
                return AppConstants.LabResultStatus.NotApplicable;
            }

            return dataEntered
                ? AppConstants.LabResultStatus.Complete
                : AppConstants.LabResultStatus.Incomplete;
        }

        public static string ComputeOverallStatus(
            PreEventImmunizationStationDto pre,
            PostEventImmunizationStationDto post)
        {
            var anyRequired = false;

            if (IsRequiredVaccineBlockingOverall(pre.HepBNeeded, post.HepBStatus))
            {
                return AppConstants.Status.Pending;
            }

            anyRequired |= pre.HepBNeeded == AppConstants.Status.Completed;

            if (IsRequiredVaccineBlockingOverall(pre.HepANeeded, post.HepAStatus))
            {
                return AppConstants.Status.Pending;
            }

            anyRequired |= pre.HepANeeded == AppConstants.Status.Completed;

            if (IsRequiredVaccineBlockingOverall(pre.FluNeeded, post.FluStatus))
            {
                return AppConstants.Status.Pending;
            }

            anyRequired |= pre.FluNeeded == AppConstants.Status.Completed;

            if (IsRequiredVaccineBlockingOverall(pre.MmrNeeded, post.MmrStatus))
            {
                return AppConstants.Status.Pending;
            }

            anyRequired |= pre.MmrNeeded == AppConstants.Status.Completed;

            if (IsRequiredVaccineBlockingOverall(pre.TetTdpNeeded, post.TetTdpStatus))
            {
                return AppConstants.Status.Pending;
            }

            anyRequired |= pre.TetTdpNeeded == AppConstants.Status.Completed;

            if (IsRequiredVaccineBlockingOverall(pre.VaricellaNeeded, post.VaricellaStatus))
            {
                return AppConstants.Status.Pending;
            }

            anyRequired |= pre.VaricellaNeeded == AppConstants.Status.Completed;

            if (!anyRequired)
            {
                return AppConstants.Status.Pending;
            }

            return AppConstants.Status.Completed;
        }

        private static bool IsRequiredVaccineBlockingOverall(string? vaccineNeeded, string? vaccineStatus) =>
            vaccineNeeded == AppConstants.Status.Completed && !IsVaccineFinishedForOverall(vaccineStatus);

        public static bool IsVaccineFinishedForOverall(string? vaccineStatus)
        {
            if (string.IsNullOrWhiteSpace(vaccineStatus))
            {
                return false;
            }

            var normalized = vaccineStatus == "Completed"
                ? AppConstants.LabResultStatus.Complete
                : vaccineStatus;

            return normalized == AppConstants.LabResultStatus.Complete;
        }
    }
}
