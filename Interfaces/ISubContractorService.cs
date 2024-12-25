using ExcelFilesCompiler.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface ISubContractorService
    {
        Task<List<SubContractorInfoDto>> GetAllSubContractors();
        Task<string> GetLastCompanyCode();
        Task<SubContractorInfoDto> GetSubContractorById(long id);
        Task<ResponseDto> AddContractAsync(SubContractorInfoDto contractDetail, string loggedinUserName);
        Task<ResponseDto> UpdateContract(SubContractorInfoDto contract, string loggedinUserName);
    }
}
