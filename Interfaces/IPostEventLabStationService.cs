using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IPostEventLabStationService
    {
        //Task<LabStation?> GetByIdAsync(long id);
        //Task<(LabStation LabStation, long EventId)> GetLabStationByIdWithEventIdAsync(long labStationId);
        Task<ResponseDto> AddAsync(PostEventLabStationDto model, string userName);
        Task<ResponseDto> UpdateAsync(PostEventLabStationDto model, string userName);
        Task<PostEventLabStation?> GetByIdAsync(long id);
        //Task<byte[]> GetLabDataAgainstEventIdAndGenerateHivPdf(long eventId);
    }
}
