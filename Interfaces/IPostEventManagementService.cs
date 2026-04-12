using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IPostEventManagementService
    {
        Task<PostEventManagementDto> GetById(long postEventManagementId);
        Task<ResponseDto> AddAsync(PostEventManagementDto model, string userName);
        Task UpdateAsync(PostEventManagementDto model, string userName);
    }
}
