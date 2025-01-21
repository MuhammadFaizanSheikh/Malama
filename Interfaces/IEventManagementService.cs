using ExcelFilesCompiler.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IEventManagementService
    {
        Task<ResponseDto> AddEventManagementAsync(EventManagement eventManagement, string loggedinUserName);
        Task<List<EventManagement>> GetAllEventManagements();
        //Task<List<ContractDetails>> GetAllContracts();
        //Task<ResponseDto> GetContractById(long id, string companyName, bool checkIfContractAlreadyExist);
        //Task<ResponseDto> UpdateContract(ContractDetails contract, string loggedinUserName);
        //Task<IEnumerable<ContractDetails>> GetContractForSearchingByContractId(string contractId);
        //Task<ContractDetails> CheckIfContractIDAlreadyExist(string contractId, string contractName, string checkType);
    }
}
