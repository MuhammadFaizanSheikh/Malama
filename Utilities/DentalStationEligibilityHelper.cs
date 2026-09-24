using ExcelFilesCompiler.Controllers.Services;
using Malama.Models;

namespace ExcelFilesCompiler.Utilities
{
    public static class DentalStationEligibilityHelper
    {
        public const string SmDrcClass3 = "3";

        public static bool IsDentalNeeded(ServiceMembersChild? serviceMember)
        {
            return DentalXRayStationService.IsNeeded(serviceMember?.DentalNeeded);
        }

        public static bool IsSmDrc3(ServiceMembersChild? serviceMember)
        {
            return string.Equals(
                serviceMember?.Drc?.Trim(),
                SmDrcClass3,
                StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsDenClass3(DentalDenClassRecord? denClass)
        {
            return IsDenClass3(denClass?.DenClass);
        }

        public static bool IsDenClass3(string? denClass)
        {
            return string.Equals(
                denClass?.Trim(),
                DentalExamDenClass.Class3,
                StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsDentalExamCompleted(DentalExam? exam)
        {
            return exam != null
                && string.Equals(
                    exam.Status?.Trim(),
                    AppConstants.Status.Completed,
                    StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Cases 1 and 2: DentalNeeded == NEEDED (and typically checked in).
        /// </summary>
        public static bool IsEligibleForDentalExam(ServiceMembersChild? serviceMember)
        {
            return serviceMember != null && IsDentalNeeded(serviceMember);
        }

        /// <summary>
        /// Case 2/4: SM DRC == 3, or Case 1 after exam completed with DenClass Class 3.
        /// </summary>
        public static bool IsEligibleForTreatmentCoordinator(
            ServiceMembersChild? serviceMember,
            DentalExam? exam = null,
            DentalDenClassRecord? denClass = null)
        {
            if (serviceMember == null)
            {
                return false;
            }

            exam ??= serviceMember.DentalExamRecord;
            denClass ??= serviceMember.DentalDenClassRecord;

            if (IsSmDrc3(serviceMember))
            {
                return true;
            }

            return IsDentalNeeded(serviceMember)
                && IsDenClass3(denClass)
                && IsDentalExamCompleted(exam);
        }

        /// <summary>
        /// Case 2: DentalNeeded and SM DRC 3, but Dental Exam not Completed yet → TC page readonly.
        /// </summary>
        public static bool RequiresDentalExamBeforeCoordinator(
            ServiceMembersChild? serviceMember,
            DentalExam? exam = null)
        {
            if (serviceMember == null || !IsDentalNeeded(serviceMember) || !IsSmDrc3(serviceMember))
            {
                return false;
            }

            exam ??= serviceMember.DentalExamRecord;
            return !IsDentalExamCompleted(exam);
        }
    }
}
