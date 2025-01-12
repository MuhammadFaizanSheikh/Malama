using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Models;
using ExcelFilesCompiler.Repositories.Interfaces;
using ExcelFilesCompiler.Repositories.Services;
using ExcelFilesCompiler.UnitOfWork;
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
        private readonly IUnitOfWork _unitOfWork;

        public ContractService(IGenericRepository<ContractDetails> repository, IUnitOfWork unitOfWork)
        {
            this.repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDto> AddContractAsync(ContractDetails contractDetail, string loggedinUserName)
        {
            var responseDto = new ResponseDto();
                
            try
            {
                var existingContractDetails = await repository.FindForSearchingAsync(sc => sc.ContractID == contractDetail.ContractID);

                if (existingContractDetails != null && existingContractDetails.Any())
                {
                    responseDto.Success = false;
                    responseDto.Message = "ContractID already exist!!";
                    return responseDto;
                }

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


        public async Task<ResponseDto> GetContractById(long id)
        {
            try
            {
                // Check if the contract is already assigned
                var alreadyAssignedContract = await _unitOfWork.SubContractors.FindForSearchingAsync(sc => sc.ContractId == id);

                if (alreadyAssignedContract != null && alreadyAssignedContract.Any())
                {
                    return new ResponseDto
                    {
                        Success = false,
                        Message = "Contract is already assigned."
                    };
                }

                // Retrieve the contract details
                var contractDetails = await repository.GetByIdAsync(id);

                if (contractDetails == null)
                {
                    return new ResponseDto
                    {
                        Success = false,
                        Message = "Contract not found."
                    };
                }

                return new ResponseDto
                {
                    Success = true,
                    Message = "Contract retrieved successfully.",
                    Data = contractDetails // Add a dynamic property or extend ResponseDto to include Data if needed
                };
            }
            catch (Exception)
            {
                // Log the error if needed
                return new ResponseDto
                {
                    Success = false,
                    Message = "An error occurred while retrieving the contract."
                };
            }
        }


        public async Task<ResponseDto> UpdateContract(ContractDetails contract, string loggedinUserName)
        {
            var responseDto = new ResponseDto();

            try
            {
                var existingContractDetails = await repository.FindForSearchingAsync(sc => sc.ContractID == contract.ContractID && sc.Id != contract.Id);

                var existingContract = await repository.GetByIdAsync(contract.Id);
                contract.AddedBy = existingContract.AddedBy;
                contract.AddedOn = existingContract.AddedOn;

                if (existingContractDetails != null && existingContractDetails.Any())
                {
                    responseDto.Success = false;
                    responseDto.Message = "ContractID already exist!!";
                    return responseDto;
                }

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

        public async Task<ContractDetails> CheckIfContractIDAlreadyExist(string contractId, string contractName, string checkType)
        {
            try
            {
                if (checkType.Equals("id"))
                {
                    return await repository.FindAsync(c => c.ContractID == contractId);
                }
                else
                {
                    return await repository.FindAsync(c => c.ContractName == contractName);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception and rethrow it for the controller to handle
                throw new Exception("Error while querying the contract.", ex);
            }
        }

    }
}
