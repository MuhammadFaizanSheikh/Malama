using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IImmunizationVaccineInfoService
    {
        Task<List<ImmunizationVaccineInfoForPreview>> GetVaccineEntriesByEventIdAsync(string eventId);
        Task<ResponseDto> AddInventoryAsync(ImmunizationVaccineInfo immunizationVaccine, string loggedinUserName);
        Task<ResponseDto> UpdateInventoryAsync(ImmunizationVaccineInfo immunizationVaccine, string loggedinUserName);
        Task<ResponseDto> GetImmunizationVaccineInfoByIdAsync(long immunizationId);
        Task<ResponseDto> GetContainersByEventIdAsync(string eventId);
    }
}
