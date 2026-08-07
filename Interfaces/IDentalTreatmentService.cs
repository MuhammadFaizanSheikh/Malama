using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IDentalTreatmentService
    {
        Task<DentalTreatment?> GetByServiceMembersChildIdAsync(long serviceMembersChildId);
        Task SaveOrUpdateFromFormDataAsync(DentalTreatmentStationSaveDto dto, string userName, string userId);
    }
}
