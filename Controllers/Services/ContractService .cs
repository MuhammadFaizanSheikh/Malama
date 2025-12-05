using ExcelFilesCompiler.Interfaces;
using Malama.Models;
using ExcelFilesCompiler.Repositories.Interfaces;
using ExcelFilesCompiler.Repositories.Services;
using ExcelFilesCompiler.UnitOfWork;
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
        private readonly ILogger<ContractService> _logger;
        private const string CLASSNAME = "ContractService";

        public ContractService(ILogger<ContractService> logger, IGenericRepository<ContractDetails> repository, IUnitOfWork unitOfWork)
        {
            this.repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ResponseDto> AddContractAsync(ContractDetails contractDetail, string loggedinUserName)
        {
            const string methodName = "AddContractAsync";
            _logger.LogInformation("{ClassName}, {MethodName}, Called with ContractID: {ContractID}, User: {UserName}",
                CLASSNAME, methodName, contractDetail.ContractID, loggedinUserName);

            var responseDto = new ResponseDto();

            try
            {
                // Check if contract already exists
                var existingContractDetails = await repository.FindForSearchingAsync(sc => sc.ContractID == contractDetail.ContractID);
                if (existingContractDetails != null && existingContractDetails.Any())
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, ContractID already exists: {ContractID}, User: {UserName}",
                        CLASSNAME, methodName, contractDetail.ContractID, loggedinUserName);

                    responseDto.Success = false;
                    responseDto.Message = "ContractID already exists!";
                    return responseDto;
                }

                // Add new contract
                contractDetail.AddedBy = loggedinUserName;
                contractDetail.AddedOn = DateTime.Now;

                await repository.AddAsync(contractDetail);
                _logger.LogInformation("{ClassName}, {MethodName}, Contract added successfully: {ContractID}, User: {UserName}",
                    CLASSNAME, methodName, contractDetail.ContractID, loggedinUserName);

                responseDto.Success = true;
                responseDto.Message = "Contract added successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Failed to add contract: {ContractID}, User: {UserName}",
                    CLASSNAME, methodName, contractDetail.ContractID, loggedinUserName);

                responseDto.Success = false;
                responseDto.Message = $"An error occurred: {ex.Message}";
            }

            return responseDto;
        }


        public async Task<List<ContractDetails>> GetAllContracts()
        {
            const string methodName = "GetAllContracts";
            _logger.LogInformation("{ClassName}, {MethodName}, Called", CLASSNAME, methodName);

            List<ContractDetails> contracts = new List<ContractDetails>();

            try
            {
                contracts = (await repository.GetAllAsync())
                    .OrderByDescending(c => c.Id)
                    .ToList();

                _logger.LogInformation("{ClassName}, {MethodName}, Successfully retrieved contracts, Count: {Count}",
                    CLASSNAME, methodName, contracts.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Failed to retrieve contracts", CLASSNAME, methodName);
                throw; // rethrow for controller to handle
            }

            return contracts;
        }


        public async Task<ResponseDto> GetContractById(long id, string companyName, bool checkIfContractAlreadyExist)
        {
            string CLASSNAME = nameof(ContractService);
            string methodName = nameof(GetContractById);

            _logger.LogInformation(
                "{ClassName}, {MethodName}, Service called. Id: {Id}, CompanyName: {CompanyName}, CheckExists: {CheckExists}",
                CLASSNAME, methodName, id, companyName, checkIfContractAlreadyExist);

            try
            {
                if (checkIfContractAlreadyExist)
                {
                    var alreadyAssignedContract = await _unitOfWork.SubContractors
                        .FindForSearchingAsync(sc => sc.ContractId == id && sc.CompanyMainName == companyName);

                    if (alreadyAssignedContract != null && alreadyAssignedContract.Any())
                    {
                        string msg = $"This Contract is already assigned to {companyName}";

                        _logger.LogInformation(
                            "{ClassName}, {MethodName}, Operation completed, Success: {Success}, Message: {Message}",
                            CLASSNAME, methodName, false, msg);

                        return new ResponseDto
                        {
                            Success = false,
                            Message = msg
                        };
                    }
                }

                var contractDetails = await repository.GetByIdAsync(id);

                if (contractDetails == null)
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Operation completed, Success: {Success}, Message: Contract not found.",
                        CLASSNAME, methodName, false);

                    return new ResponseDto
                    {
                        Success = false,
                        Message = "Contract not found."
                    };
                }

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Operation completed, Success: {Success}, Message: Contract retrieved successfully.",
                    CLASSNAME, methodName, true);

                return new ResponseDto
                {
                    Success = true,
                    Message = "Contract retrieved successfully.",
                    Data = contractDetails
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Unexpected exception occurred",
                    CLASSNAME, methodName);

                return new ResponseDto
                {
                    Success = false,
                    Message = "An error occurred while retrieving the contract."
                };
            }
        }




        public async Task<ResponseDto> UpdateContract(ContractDetails contract, string loggedinUserName)
        {
            const string methodName = "UpdateContract";
            _logger.LogInformation("{ClassName}, {MethodName}, Called with ContractID: {ContractID}, User: {UserName}",
                CLASSNAME, methodName, contract.ContractID, loggedinUserName);

            var responseDto = new ResponseDto();

            try
            {
                // Check if ContractID already exists in another record
                var existingContractDetails = await repository.FindForSearchingAsync(sc => sc.ContractID == contract.ContractID && sc.Id != contract.Id);
                if (existingContractDetails != null && existingContractDetails.Any())
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, ContractID already exists in another record: {ContractID}, User: {UserName}",
                        CLASSNAME, methodName, contract.ContractID, loggedinUserName);

                    responseDto.Success = false;
                    responseDto.Message = "ContractID already exists!";
                    return responseDto;
                }

                // Preserve original AddedBy / AddedOn
                var existingContract = await repository.GetByIdAsync(contract.Id);
                contract.AddedBy = existingContract.AddedBy;
                contract.AddedOn = existingContract.AddedOn;

                // Update contract
                contract.UpdatedBy = loggedinUserName;
                contract.UpdatedOn = DateTime.Now;

                await repository.UpdateAsync(contract);

                _logger.LogInformation("{ClassName}, {MethodName}, Contract updated successfully: {ContractID}, User: {UserName}",
                    CLASSNAME, methodName, contract.ContractID, loggedinUserName);

                responseDto.Success = true;
                responseDto.Message = "Contract updated successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Failed to update contract: {ContractID}, User: {UserName}",
                    CLASSNAME, methodName, contract.ContractID, loggedinUserName);

                responseDto.Success = false;
                responseDto.Message = $"An error occurred while updating contract: {ex.Message}";
            }

            return responseDto;
        }


        public async Task<IEnumerable<ContractDetails>> GetContractForSearchingByContractId(string contractName)
        {
            string methodName = nameof(GetContractForSearchingByContractId);

            _logger.LogInformation(
                "{ClassName}, {MethodName}, Service called. SearchTerm: {SearchTerm}",
                CLASSNAME, methodName, contractName);

            try
            {
                IEnumerable<ContractDetails> result;

                if (string.IsNullOrEmpty(contractName))
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Empty search term received. Returning all contracts.",
                        CLASSNAME, methodName);

                    result = await repository.FindForSearchingAsync(c => true);
                }
                else
                {
                    result = await repository.FindForSearchingAsync(
                                c => c.ContractName.ToLower().Contains(contractName.ToLower()));

                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Filter applied. SearchTerm: {SearchTerm}, ResultsFound: {Count}",
                        CLASSNAME, methodName, contractName, result.Count());
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Unexpected exception occurred",
                    CLASSNAME, methodName);

                throw new Exception("Error while fetching contract details.", ex);
            }
        }


        public async Task<ContractDetails> CheckIfContractIDAlreadyExist(string contractId, string contractName, string checkType)
        {
            string methodName = nameof(CheckIfContractIDAlreadyExist);

            _logger.LogInformation(
                "{ClassName}, {MethodName}, Service called. ContractId: {ContractId}, ContractName: {ContractName}, CheckType: {CheckType}",
                CLASSNAME, methodName, contractId, contractName, checkType);

            try
            {
                ContractDetails result;

                if (checkType.Equals("id"))
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Checking by ContractID: {ContractId}",
                        CLASSNAME, methodName, contractId);

                    result = await repository.FindAsync(c => c.ContractID == contractId);
                }
                else
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Checking by ContractName: {ContractName}",
                        CLASSNAME, methodName, contractName);

                    result = await repository.FindAsync(c => c.ContractName == contractName);
                }

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Operation completed, Found: {Found}",
                    CLASSNAME, methodName, result != null);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Unexpected exception occurred",
                    CLASSNAME, methodName);

                throw new Exception("Error while querying the contract.", ex);
            }
        }


    }
}
