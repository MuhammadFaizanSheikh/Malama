using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IImmunizationVaccineInfoService
    {
        Task<List<ImmunizationVaccineInfoForPreview>> GetVaccineEntriesByEventIdAsync(long eventId);
        Task<ResponseDto> AddInventoryAsync(ImmunizationVaccineInfo immunizationVaccine, string loggedinUserName);
        Task<ResponseDto> UpdateInventoryAsync(ImmunizationVaccineInfo immunizationVaccine, string loggedinUserName);
        Task<ResponseDto> GetImmunizationVaccineInfoByIdAsync(long immunizationId);
        Task<ResponseDto> GetContainersByEventIdAsync(long eventId);
        Task<ResponseDto> GetManufacturerByEventIdAsync(long eventId);
    }
}
