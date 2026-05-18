using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IVitalStationService
    {
        Task<VitalStationVM> GetVitalStationByServiceMemberChildIdAsync(long serviceMemberChildId);
        Task<ResponseDto> AddAsync(VitalStationDto model, string submissionToken, string userName);
        Task<ResponseDto> UpdateAsync(VitalStationDto model, string submissionToken, string userName);
    }
}
