using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IPostEventLabStationService
    {
        //Task<LabStation?> GetByIdAsync(long id);
        //Task<(LabStation LabStation, long EventId)> GetLabStationByIdWithEventIdAsync(long labStationId);
        Task<ResponseDto> AddAsync(PostEventLabStationDto model, string userName);
        //Task UpdateAsync(PostEventLabStationDto model, string userName);
        //Task<byte[]> GetLabDataAgainstEventIdAndGenerateHivPdf(long eventId);
    }
}
