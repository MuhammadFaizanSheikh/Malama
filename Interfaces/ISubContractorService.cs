using ExcelFilesCompiler.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface ISubContractorService
    {
        Task<List<SubContractorAndContractViewModel>> GetAllSubContractors();
        Task<string> GetLastCompanyCode();
        Task<CombinedSubContractorAndContractDto> GetSubContractorById(long id);
        Task<ResponseDto> AddContractAsync(SubContractorInfoDto contractDetail, string loggedinUserName);
        Task<ResponseDto> UpdateContract(SubContractorInfoDto contract, string loggedinUserName);
    }
}
