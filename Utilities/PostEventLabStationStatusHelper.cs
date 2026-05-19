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

        public static string ComputeOverallStatus(PreEventLabStationDto pre, PostEventLabStationDto post)
        {
            var requiredStatuses = new List<string>();

            AddRequiredStatus(requiredStatuses, pre.G6pdNeeded, post.G6pdStatus);
            AddRequiredStatus(requiredStatuses, pre.AboNeeded, post.AboStatus);
            AddRequiredStatus(requiredStatuses, pre.HivNeeded, post.HivStatus);
            AddRequiredStatus(requiredStatuses, pre.PregnancyTestNeeded, post.PregnancyStatus);
            AddRequiredStatus(requiredStatuses, pre.LipidPanelNeeded, post.LipidPanelStatus);
            AddRequiredStatus(requiredStatuses, pre.SickleCellNeeded, post.SickleCellStatus);
            AddRequiredStatus(requiredStatuses, pre.DnaNeeded, post.DnaStatus);

            if (requiredStatuses.Count == 0)
            {
                return AppConstants.Status.Pending;
            }

            var allFinished = requiredStatuses.All(IsLabFinishedStatus);

            return allFinished ? AppConstants.Status.Completed : AppConstants.Status.Pending;
        }

        private static void AddRequiredStatus(List<string> statuses, string? labNeeded, string? labStatus)
        {
            if (labNeeded == AppConstants.Status.Completed && !string.IsNullOrWhiteSpace(labStatus))
            {
                statuses.Add(labStatus);
            }
        }

        private static bool IsLabFinishedStatus(string? status) =>
            status == AppConstants.LabResultStatus.Complete ||
            status == AppConstants.LabResultStatus.CompleteWithReason ||
            status == "Completed"; // legacy value
    }
}
