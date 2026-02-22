using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IContractService
    {
        Task<ResponseDto> AddContractAsync(ContractDetails contractDetail, string submissionToken, string loggedinUserName);
        Task<List<ContractDetails>> GetAllContracts();
        Task<ResponseDto> GetContractById(long id, string companyName, bool checkIfContractAlreadyExist);
        Task<ResponseDto> UpdateContract(ContractDetails contract, string loggedinUserName);
        Task<IEnumerable<ContractDetails>> GetContractForSearchingByContractId(string contractId);
        Task<ContractDetails> CheckIfContractIDAlreadyExist(string contractId, string contractName, string checkType);
    }
}
