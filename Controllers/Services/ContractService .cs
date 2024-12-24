using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Models;
using ExcelFilesCompiler.Repositories.Interfaces;
using ExcelToCsv.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Diagnostics.Contracts;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class ContractService : IContractService
    {
        private readonly IGenericRepository<ContractDetails> repository;

        public ContractService(IGenericRepository<ContractDetails> repository)
        {
            this.repository = repository;
        }

        public async Task<ResponseDto> AddContractAsync(ContractDetails contractDetail, string loggedinUserName)
        {
            var responseDto = new ResponseDto();

            try
            {
                // Attempt to add the contract detail to the repository
                contractDetail.AddedBy = loggedinUserName;
                contractDetail.AddedOn = DateTime.Now;
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

        public async Task<List<ContractDetails>> GetAllContracts()
        {
            var responseDto = new ResponseDto();
            List<ContractDetails> contracts = new List<ContractDetails>(); // Initialize contracts outside try-catch

            try
            {
                contracts = (await repository.GetAllAsync()).OrderByDescending(c => c.Id).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

            return contracts;
        }


        public async Task<ContractDetails> GetContractById(long id)
        {
            ContractDetails contractDetails = null;

            try
            {
                contractDetails = await repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw;
            }

            return contractDetails;
        }

        public async Task<ResponseDto> UpdateContract(ContractDetails contract, string loggedinUserName)
        {
            var responseDto = new ResponseDto();

            try
            {
                contract.UpdatedBy = loggedinUserName;
                contract.UpdatedOn = DateTime.Now;
                await repository.UpdateAsync(contract);
                responseDto.Success = true;
                responseDto.Message = "Contract updated successfully!";
            }
            catch (Exception ex)
            {
                responseDto.Success = false;
                responseDto.Message = $"An error occurred while updating contract: {ex.Message}";
            }

            return responseDto;
        }

        public async Task<IEnumerable<ContractDetails>> GetContractForSearchingByContractId(string contractId)
        {
            try
            {
                if (string.IsNullOrEmpty(contractId))
                {
                    return await repository.FindForSearchingAsync(c => true);
                }

                return await repository.FindForSearchingAsync(c => c.ContractID.Contains(contractId));
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching contract details.", ex);
            }
        }

    }
}
