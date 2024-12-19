using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Models;
using ExcelFilesCompiler.Repositories.Interfaces;
using ExcelToCsv.Models;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class ContractService : IContractService
    {
        private readonly IGenericRepository<ContractDetails> repository;

        public ContractService(IGenericRepository<ContractDetails> repository)
        {
            this.repository = repository;
        }

        public async Task<ResponseDto> AddContractAsync(ContractDetails contractDetail)
        {
            var responseDto = new ResponseDto();

            try
            {
                // Attempt to add the contract detail to the repository
                await repository.AddAsync(contractDetail);

                // If successful, set Success to true and provide a success message
                responseDto.Success = true;
                responseDto.Message = "Contract added successfully!";
            }
            catch (Exception ex)
            {
                // If an exception occurs, set Success to false and provide the error message
                responseDto.Success = false;
                responseDto.Message = $"An error occurred: {ex.Message}";
            }

            return responseDto;
        }

        //public async Task<List<ContractDetails>> GetAllContractsAsync()
        //{
        //    return await repository.GetAllAsync();
        //}
    }
}
