using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IDentalExamService
    {
        Task<DentalExam?> GetByServiceMembersChildIdAsync(long serviceMembersChildId);
        Task SaveOrUpdateFromFormDataAsync(DentalExamStationSaveDto dto, string userName);
    }
}
