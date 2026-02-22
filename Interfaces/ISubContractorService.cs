using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface ISubContractorService
    {
        Task<List<SubContractorAndContractViewModel>> GetAllSubContractors();
        Task<string> GetLastCompanyCode(string companyName);
        Task<CombinedSubContractorAndContractDto> GetSubContractorById(long id);
        Task<ResponseDto> AddContractAsync(SubContractor contractDetail, string submissionToken, string loggedinUserName);
        Task<ResponseDto> UpdateContract(SubContractor contract, string loggedinUserName);
        Task<IEnumerable<SubContractor>> GetSubContractorByCompanyNameForSearching(string companyName);
        Task<List<ContractDetails>> GetContractIdsBySubContractorCompanyName(string companyName);
        Task<IEnumerable<SubContractor>> GetCompanyNameByTermAsync(string term);
    }
}
