using ExcelFilesCompiler.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IContractService
    {
        Task<ResponseDto> AddContractAsync(ContractDetails contractDetail);
        //Task<List<ContractDetails>> GetAllContractsAsync();
    }
}
