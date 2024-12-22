using ExcelFilesCompiler.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IContractService
    {
        Task<ResponseDto> AddContractAsync(ContractDetails contractDetail);
        Task<List<ContractDetails>> GetAllContracts();
        Task<ContractDetails> GetContractById(long id);
        Task<ResponseDto> UpdateContract(ContractDetails contract);
        //Task<List<ContractDetails>> GetAllContractsAsync();
    }
}
