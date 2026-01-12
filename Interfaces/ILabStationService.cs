using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface ILabStationService
    {
        Task<LabStation?> GetByIdAsync(long id);
        Task<LabStation> GetByIdWithParentAsync(long id); // NEW
        Task AddAsync(LabStation model, string userName);
        Task UpdateAsync(LabStation model, string userName);
        Task<byte[]> GetLabDataAgainstEventIdAndGenerateHivPdf(string eventId);
    }
}
