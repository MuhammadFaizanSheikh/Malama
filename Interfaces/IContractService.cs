using ExcelFilesCompiler.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IContractService
    {
        Task<ResponseDto> AddContractAsync(ContractDetails contractDetail);
        Task<(ResponseDto responseDto, List<ContractDetails> Contracts)> GetAllContracts();
        Task<(ResponseDto responseDto, ContractDetails contractDetails)> GetContractById(long id);
        Task<ResponseDto> UpdateContract(ContractDetails contract);
        //Task<List<ContractDetails>> GetAllContractsAsync();
    }
}
