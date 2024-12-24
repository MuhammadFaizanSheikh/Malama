using ExcelFilesCompiler.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IContractService
    {
        Task<ResponseDto> AddContractAsync(ContractDetails contractDetail, string loggedinUserName);
        Task<List<ContractDetails>> GetAllContracts();
        Task<ContractDetails> GetContractById(long id);
        Task<ResponseDto> UpdateContract(ContractDetails contract, string loggedinUserName);
        Task<IEnumerable<ContractDetails>> GetContractForSearchingByContractId(string contractId);
    }
}
