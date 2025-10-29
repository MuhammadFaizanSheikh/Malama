using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IImmunizationStationService
    {
        Task<ImmunizationStation?> GetByIdAsync(long id);
        Task<ImmunizationStation> GetByIdWithParentAsync(long id); // NEW
        Task AddAsync(ImmunizationStation model, string userName);
        Task UpdateAsync(ImmunizationStation model, string userName);
        Task<ResponseDto> GetImmunizationManufacturer(string eventId);
    }
}
