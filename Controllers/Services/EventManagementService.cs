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
    public class EventManagementService : IEventManagementService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventManagementService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<EventManagement>> GetAllEventManagements()
        {
            var responseDto = new ResponseDto();
            List<EventManagement> eventManagements = new List<EventManagement>();

            try
            {
                eventManagements = (await _unitOfWork.EventManagement.GetAllAsync()).OrderByDescending(c => c.Id).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

            return eventManagements;
        }

        public async Task<ResponseDto> AddEventManagementAsync(EventManagement eventManagement, string loggedinUserName)
        {
            var responseDto = new ResponseDto();

            try
            {
                //var existingContractDetails = await repository.FindForSearchingAsync(sc => sc.ContractID == contractDetail.ContractID);

                //if (existingContractDetails != null && existingContractDetails.Any())
                //{
                //    responseDto.Success = false;
                //    responseDto.Message = "ContractID already exist!!";
                //    return responseDto;
                //}

                eventManagement.AddedBy = loggedinUserName;
                eventManagement.AddedOn = DateTime.Now;
                await _unitOfWork.EventManagement.AddAsync(eventManagement);

                responseDto.Success = true;
                responseDto.Message = "Event Management added successfully!";
            }
            catch (Exception ex)
            {
                // If an exception occurs, set Success to false and provide the error message
                responseDto.Success = false;
                responseDto.Message = $"An error occurred: {ex.Message}";
            }

            return responseDto;
        }




        //public async Task<ResponseDto> GetContractById(long id, string companyName, bool checkIfContractAlreadyExist)
        //{
        //    try
        //    {
        //        if (checkIfContractAlreadyExist)
        //        {
        //            var alreadyAssignedContract = await _unitOfWork.SubContractors.FindForSearchingAsync(sc => sc.ContractId == id && sc.CompanyMainName == companyName);

        //            if (alreadyAssignedContract != null && alreadyAssignedContract.Any())
        //            {
        //                return new ResponseDto
        //                {
        //                    Success = false,
        //                    Message = $"This Contract is already assigned to {companyName}"
        //                };
        //            }
        //        }

        //        // Retrieve the contract details
        //        var contractDetails = await repository.GetByIdAsync(id);

        //        if (contractDetails == null)
        //        {
        //            return new ResponseDto
        //            {
        //                Success = false,
        //                Message = "Contract not found."
        //            };
        //        }

        //        return new ResponseDto
        //        {
        //            Success = true,
        //            Message = "Contract retrieved successfully.",
        //            Data = contractDetails // Add a dynamic property or extend ResponseDto to include Data if needed
        //        };
        //    }
        //    catch (Exception)
        //    {
        //        // Log the error if needed
        //        return new ResponseDto
        //        {
        //            Success = false,
        //            Message = "An error occurred while retrieving the contract."
        //        };
        //    }
        //}


        //public async Task<ResponseDto> UpdateContract(ContractDetails contract, string loggedinUserName)
        //{
        //    var responseDto = new ResponseDto();

        //    try
        //    {
        //        var existingContractDetails = await repository.FindForSearchingAsync(sc => sc.ContractID == contract.ContractID && sc.Id != contract.Id);

        //        var existingContract = await repository.GetByIdAsync(contract.Id);
        //        contract.AddedBy = existingContract.AddedBy;
        //        contract.AddedOn = existingContract.AddedOn;

        //        if (existingContractDetails != null && existingContractDetails.Any())
        //        {
        //            responseDto.Success = false;
        //            responseDto.Message = "ContractID already exist!!";
        //            return responseDto;
        //        }

        //        contract.UpdatedBy = loggedinUserName;
        //        contract.UpdatedOn = DateTime.Now;
        //        await repository.UpdateAsync(contract);
        //        responseDto.Success = true;
        //        responseDto.Message = "Contract updated successfully!";
        //    }
        //    catch (Exception ex)
        //    {
        //        responseDto.Success = false;
        //        responseDto.Message = $"An error occurred while updating contract: {ex.Message}";
        //    }

        //    return responseDto;
        //}

        //public async Task<IEnumerable<ContractDetails>> GetContractForSearchingByContractId(string contractName)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(contractName))
        //        {
        //            return await repository.FindForSearchingAsync(c => true);
        //        }

        //        return await repository.FindForSearchingAsync(c => c.ContractName.Contains(contractName));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error while fetching contract details.", ex);
        //    }
        //}

        //public async Task<ContractDetails> CheckIfContractIDAlreadyExist(string contractId, string contractName, string checkType)
        //{
        //    try
        //    {
        //        if (checkType.Equals("id"))
        //        {
        //            return await repository.FindAsync(c => c.ContractID == contractId);
        //        }
        //        else
        //        {
        //            return await repository.FindAsync(c => c.ContractName == contractName);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle the exception and rethrow it for the controller to handle
        //        throw new Exception("Error while querying the contract.", ex);
        //    }
        //}

    }
}
