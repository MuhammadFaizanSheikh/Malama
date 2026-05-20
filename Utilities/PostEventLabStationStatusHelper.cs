using Malama.Models;

namespace ExcelFilesCompiler.Utilities
{
    public static class PostEventLabStationStatusHelper
    {
        public static void ApplyPerLabStatuses(PostEventLabStationDto post, PreEventLabStationDto? pre)
        {
            if (post == null || pre == null)
            {
                return;
            }

            post.G6pdStatus = ComputeStandardLabStatus(
                pre.G6pdNeeded,
                post.G6pdResultReceived,
                post.G6pdResultReason,
                post.G6pdResultMalamaUploaded,
                post.G6pdResultSORUploaded,
                post.G6pdResultHRRUploaded,
                post.G6pdResultReceivedDateTime);

            post.AboStatus = ComputeStandardLabStatus(
                pre.AboNeeded,
                post.AboResultReceived,
                post.AboResultReason,
                post.AboResultMalamaUploaded,
                post.AboResultSORUploaded,
                post.AboResultHRRUploaded,
                post.AboResultReceivedDateTime);

            post.HivStatus = ComputeStandardLabStatus(
                pre.HivNeeded,
                post.HivResultReceived,
                post.HivResultReason,
                post.HivResultMalamaUploaded,
                post.HivResultSORUploaded,
                post.HivResultHRRUploaded,
                post.HivResultReceivedDateTime);

            post.PregnancyStatus = ComputeStandardLabStatus(
                pre.PregnancyTestNeeded,
                post.PregnancyResultReceived,
                post.PregnancyResultReason,
                post.PregnancyResultMalamaUploaded,
                post.PregnancyResultSORUploaded,
                post.PregnancyResultHRRUploaded,
                post.PregnancyResultReceivedDateTime);

            post.LipidPanelStatus = ComputeStandardLabStatus(
                pre.LipidPanelNeeded,
                post.LipidPanelResultReceived,
                post.LipidPanelResultReason,
                post.LipidPanelResultMalamaUploaded,
                post.LipidPanelResultSORUploaded,
                post.LipidPanelResultHRRUploaded,
                post.LipidPanelResultReceivedDateTime);

            post.SickleCellStatus = ComputeStandardLabStatus(
                pre.SickleCellNeeded,
                post.SickleCellResultReceived,
                post.SickleCellResultReason,
                post.SickleCellResultMalamaUploaded,
                post.SickleCellResultSORUploaded,
                post.SickleCellResultHRRUploaded,
                post.SickleCellResultReceivedDateTime);

            post.DnaStatus = pre.DnaNeeded == AppConstants.Status.Completed
                ? AppConstants.LabResultStatus.Complete
                : AppConstants.LabResultStatus.NotApplicable;

            post.Status = ComputeOverallStatus(pre, post);
        }

        public static string? ComputeStandardLabStatus(
            string? labNeeded,
            bool resultReceived,
            string? resultReason,
            bool malamaUploaded,
            bool sorUploaded,
            bool hrrUploaded,
            DateTime? receivedDateTime)
        {
            if (labNeeded != AppConstants.Status.Completed)
            {
                return AppConstants.LabResultStatus.NotApplicable;
            }

            if (!resultReceived)
            {
                if (string.Equals(
                        resultReason,
                        AppConstants.LabResultStatus.InsufficientBloodSampleReason,
                        StringComparison.Ordinal))
                {
                    return AppConstants.LabResultStatus.CompleteWithReason;
                }

                return AppConstants.LabResultStatus.Incomplete;
            }

            var malamaComplete = malamaUploaded;
            var sorComplete = sorUploaded;
            var hrrComplete = hrrUploaded;
            var receivedDateComplete = receivedDateTime.HasValue;

            if (malamaComplete && sorComplete && hrrComplete && receivedDateComplete)
            {
                return AppConstants.LabResultStatus.Complete;
            }

            if (!malamaComplete && !sorComplete && !hrrComplete)
            {
                return AppConstants.LabResultStatus.Incomplete;
            }

            return AppConstants.LabResultStatus.PartiallyComplete;
        }

        /// <summary>
        /// Overall page status is Completed when every required lab is finished:
        /// Complete (Yes + Malama + SOR + HRR + received date) or Complete with Reason (No + Insufficient blood sample).
        /// </summary>
        public static string ComputeOverallStatus(PreEventLabStationDto pre, PostEventLabStationDto post)
        {
            var anyRequired = false;

            if (IsRequiredLabBlockingOverall(pre.G6pdNeeded, post.G6pdStatus))
            {
                return AppConstants.Status.Pending;
            }

            anyRequired |= pre.G6pdNeeded == AppConstants.Status.Completed;

            if (IsRequiredLabBlockingOverall(pre.AboNeeded, post.AboStatus))
            {
                return AppConstants.Status.Pending;
            }

            anyRequired |= pre.AboNeeded == AppConstants.Status.Completed;

            if (IsRequiredLabBlockingOverall(pre.HivNeeded, post.HivStatus))
            {
                return AppConstants.Status.Pending;
            }

            anyRequired |= pre.HivNeeded == AppConstants.Status.Completed;

            if (IsRequiredLabBlockingOverall(pre.PregnancyTestNeeded, post.PregnancyStatus))
            {
                return AppConstants.Status.Pending;
            }

            anyRequired |= pre.PregnancyTestNeeded == AppConstants.Status.Completed;

            if (IsRequiredLabBlockingOverall(pre.LipidPanelNeeded, post.LipidPanelStatus))
            {
                return AppConstants.Status.Pending;
            }

            anyRequired |= pre.LipidPanelNeeded == AppConstants.Status.Completed;

            if (IsRequiredLabBlockingOverall(pre.SickleCellNeeded, post.SickleCellStatus))
            {
                return AppConstants.Status.Pending;
            }

            anyRequired |= pre.SickleCellNeeded == AppConstants.Status.Completed;

            if (IsRequiredLabBlockingOverall(pre.DnaNeeded, post.DnaStatus))
            {
                return AppConstants.Status.Pending;
            }

            anyRequired |= pre.DnaNeeded == AppConstants.Status.Completed;

            if (!anyRequired)
            {
                return AppConstants.Status.Pending;
            }

            return AppConstants.Status.Completed;
        }

        private static bool IsRequiredLabBlockingOverall(string? labNeeded, string? labStatus) =>
            labNeeded == AppConstants.Status.Completed && !IsLabFinishedForOverall(labStatus);

        public static bool IsLabFinishedForOverall(string? labStatus)
        {
            if (string.IsNullOrWhiteSpace(labStatus))
            {
                return false;
            }

            var normalized = labStatus == "Completed"
                ? AppConstants.LabResultStatus.Complete
                : labStatus;

            return normalized == AppConstants.LabResultStatus.Complete ||
                normalized == AppConstants.LabResultStatus.CompleteWithReason;
        }
    }
}
