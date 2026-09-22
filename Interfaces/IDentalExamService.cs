using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IDentalExamService
    {
        Task<DentalExam?> GetByServiceMembersChildIdAsync(long serviceMembersChildId);
        Task SaveOrUpdateFromFormDataAsync(DentalExamStationSaveDto dto, string userName, string userId);

        /// <summary>
        /// Applies PSR / DEN Class / Pano / selected teeth from Treatment Coordinator without touching findings or signature.
        /// </summary>
        Task ApplyCoordinatorClinicalSectionsAsync(
            DentalCoordinatorStationSaveDto dto,
            string userName,
            bool saveChanges = true);

        /// <summary>
        /// Applies Dental Findings from Treatment Coordinator with ownership rules (exam vs coordinator sourced).
        /// </summary>
        Task ApplyCoordinatorFindingsAsync(
            DentalCoordinatorStationSaveDto dto,
            string userName,
            string userId,
            bool saveChanges = true);
    }
}
