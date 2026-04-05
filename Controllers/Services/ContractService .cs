using ExcelFilesCompiler.Interfaces;
using Malama.Models;
using ExcelFilesCompiler.UnitOfWork;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class ContractService : IContractService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISubmissionTokenService _submissionTokenService;
        private readonly ILogger<ContractService> _logger;
        private const string CLASSNAME = "ContractService";

        public ContractService(ILogger<ContractService> logger, IUnitOfWork unitOfWork, ISubmissionTokenService submissionTokenService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _submissionTokenService = submissionTokenService;
        }

        public async Task<ResponseDto> AddContractAsync(ContractDetails contractDetail, string submissionToken,  string loggedinUserName)
        {
            const string methodName = "AddContractAsync";
            _logger.LogInformation("{ClassName}, {MethodName}, Called with ContractID: {ContractID}, User: {UserName}",
                CLASSNAME, methodName, contractDetail.ContractID, loggedinUserName);

            var responseDto = new ResponseDto();

            try
            {
                var tokenResult = await _submissionTokenService.ValidateAndSaveAsync(submissionToken, loggedinUserName);

                if (!tokenResult.Success)
                {
                    return tokenResult;
                }

                // Check if contract already exists
                var existingContractDetails = _unitOfWork.ContractDetails.GetAllWithConditionNoTracking(sc => sc.ContractID == contractDetail.ContractID);
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

                await _unitOfWork.ContractDetails.AddAsync(contractDetail);
                await _unitOfWork.SaveAsync();
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
                contracts = _unitOfWork.ContractDetails.GetAllNoTracking()
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
                    var alreadyAssignedContract = _unitOfWork.SubContractors
                        .GetAllWithConditionNoTracking(sc => sc.ContractId == id && sc.CompanyMainName == companyName);

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

                var contractDetails = await _unitOfWork.ContractDetails.GetByIdAsync(id);

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
                var existingContractDetails = _unitOfWork.ContractDetails.GetAllWithConditionNoTracking(sc => sc.ContractID == contract.ContractID && sc.Id != contract.Id);

                var exists = await existingContractDetails.AnyAsync();

                if (exists)
                {
                    _logger.LogWarning("{ClassName}, {MethodName}, ContractID already exists in another record: {ContractID}, User: {UserName}",
                        CLASSNAME, methodName, contract.ContractID, loggedinUserName);

                    responseDto.Success = false;
                    responseDto.Message = "ContractID already exists!";
                    return responseDto;
                }

                var existingContract = await _unitOfWork.ContractDetails.GetByIdAsync(contract.Id);

                if (existingContract == null)
                {
                    responseDto.Success = false;
                    responseDto.Message = "Contract not found!";
                    return responseDto;
                }

                string addedBy = existingContract.AddedBy;
                DateTime addedOn = existingContract.AddedOn;

                _mapper.Map(contract, existingContract);
                existingContract.AddedBy = addedBy;
                existingContract.AddedOn = addedOn;
                existingContract.UpdatedBy = loggedinUserName;
                existingContract.UpdatedOn = DateTime.Now;

                await _unitOfWork.SaveAsync();

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


        public IQueryable<ContractDetails> GetContractForSearchingByContractId(string contractName)
        {
            string methodName = nameof(GetContractForSearchingByContractId);

            _logger.LogInformation(
                "{ClassName}, {MethodName}, Service called. SearchTerm: {SearchTerm}",
                CLASSNAME, methodName, contractName);

            try
            {
                IQueryable<ContractDetails> result;

                if (string.IsNullOrEmpty(contractName))
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Empty search term received. Returning all contracts.",
                        CLASSNAME, methodName);

                    result = _unitOfWork.ContractDetails.GetAllWithConditionNoTracking(c => true);
                }
                else
                {
                    result = _unitOfWork.ContractDetails.GetAllWithConditionNoTracking(
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

                    result = await _unitOfWork.ContractDetails.GetFirstOrDefaultWithConditionNoTracking(c => c.ContractID == contractId);
                }
                else
                {
                    _logger.LogInformation(
                        "{ClassName}, {MethodName}, Checking by ContractName: {ContractName}",
                        CLASSNAME, methodName, contractName);

                    result = await _unitOfWork.ContractDetails.GetFirstOrDefaultWithConditionNoTracking(c => c.ContractName == contractName);
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
