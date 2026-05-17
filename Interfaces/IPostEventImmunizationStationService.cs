using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IPostEventImmunizationStationService
    {
        Task<ResponseDto> AddAsync(PostEventImmunizationStationDto model, string userName);
        Task<ResponseDto> UpdateAsync(PostEventImmunizationStationDto model, string userName);
        Task<PostEventImmunizationStation?> GetByIdAsync(long id);
    }
}
