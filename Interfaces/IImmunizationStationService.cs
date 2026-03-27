using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IImmunizationStationService
    {
        Task<ImmunizationStation?> GetByIdAsync(long id);
        Task<(ImmunizationStation Immunization, long EventId)> GetImmunizationByIdWithEventIdAsync(long immunizationId); // NEW
        Task AddAsync(ImmunizationStation model, string userName);
        Task UpdateAsync(ImmunizationStation model, string userName);
        Task<ResponseDto> GetImmunizationManufacturer(long eventId);
    }
}
