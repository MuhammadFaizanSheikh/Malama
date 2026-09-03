using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IDentalTreatmentService
    {
        Task<DentalTreatment?> GetByServiceMembersChildIdAsync(long serviceMembersChildId);
        Task SaveOrUpdateFromFormDataAsync(DentalTreatmentStationSaveDto dto, string userName, string userId);

        /// <summary>
        /// Upserts Treatment Coordinator name/datetime/comments on DentalTreatment without touching treatment children.
        /// Requires an existing DentalExam for the service member.
        /// </summary>
        Task ApplyCoordinatorSectionAsync(
            long serviceMembersChildId,
            string? comments,
            string status,
            string userName,
            string userId,
            bool saveChanges = true);
    }
}
