using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IVitalStationService
    {
        Task<LabStation?> GetByIdAsync(long id);
        Task<VitalStationVM> GetVitalStationByServiceMemberChildIdAsync(long serviceMemberChildId);
        Task AddAsync(VitalStationDto model, string userName);
        Task UpdateAsync(VitalStationDto model, string userName);
    }
}
