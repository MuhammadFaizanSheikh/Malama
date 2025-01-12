using ExcelFilesCompiler.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IContractService
    {
        Task<ResponseDto> AddContractAsync(ContractDetails contractDetail, string loggedinUserName);
        Task<List<ContractDetails>> GetAllContracts();
        Task<ResponseDto> GetContractById(long id);
        Task<ResponseDto> UpdateContract(ContractDetails contract, string loggedinUserName);
        Task<IEnumerable<ContractDetails>> GetContractForSearchingByContractId(string contractId);
        Task<ContractDetails> CheckIfContractIDAlreadyExist(string contractId, string contractName, string checkType);
    }
}
