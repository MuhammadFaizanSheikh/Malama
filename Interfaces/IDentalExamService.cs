using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IDentalExamService
    {
        Task<DentalExam?> GetByServiceMembersChildIdAsync(long serviceMembersChildId);
        Task SaveOrUpdateFromFormDataAsync(DentalExamStationSaveDto dto, string userName, string userId);

        /// <summary>
        /// Applies PSR / DEN Class / Pano / selected teeth from Dental Coordinator without touching findings or signature.
        /// </summary>
        Task ApplyCoordinatorClinicalSectionsAsync(
            DentalCoordinatorStationSaveDto dto,
            string userName,
            bool saveChanges = true);
    }
}
