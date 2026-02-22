using ExcelFilesCompiler.Interfaces;
using Malama.Models;
using ExcelFilesCompiler.Repositories.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto;
using System.ComponentModel.Design;
using System.Diagnostics.Contracts;
using Microsoft.Extensions.Logging;

namespace ExcelFilesCompiler.Controllers.Services
{
    public class SubContractorService : ISubContractorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SubContractorService> _logger;
        private const string CLASSNAME = "SubContractorService";

        public SubContractorService(ILogger<SubContractorService> logger, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<SubContractorAndContractViewModel>> GetAllSubContractors()
        {
            const string methodName = "GetAllSubContractors";

            try
            {
                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Fetching subcontractors",
                    CLASSNAME, methodName);

                var subcontractors = await _unitOfWork.SubContractors.GetWithIncludeAsync(
                    null,
                    x => x.ServiceTypeProvided
                );

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Subcontractors fetched",
                    CLASSNAME, methodName);

                var contracts = await _unitOfWork.ContractDetails.GetAllAsync();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Contracts fetched",
                    CLASSNAME, methodName);

                var result = from sub in subcontractors
                             join contract in contracts on sub.ContractId equals contract.Id
                             select new SubContractorAndContractViewModel
                             {
                                 Id = sub.Id,
                                 CompanyId = sub.CompanyId,
                                 CompanyMainName = sub.CompanyMainName,
                                 CompanyMainState = sub.CompanyMainState,
                                 CompanyMainCity = sub.CompanyMainCity,
                                 CompanyMainZip = sub.CompanyMainZip,
                                 ContractName = contract.ContractName,
                                 ContractId = contract.ContractID,
                                 ServiceTypeProvided = string.Join(", ", sub.ServiceTypeProvided.Select(stp => stp.ServiceTypeProvidedName)),
                             };

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Mapping completed, FinalCount: {Count}",
                    CLASSNAME, methodName, result.Count());

                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "{ClassName}, {MethodName}, Error retrieving subcontractors",
                    CLASSNAME, methodName);

                throw;
            }
        }



        public async Task<string> GetLastCompanyCode(string companyName)
        {
            const string methodName = nameof(GetLastCompanyCode);
            _logger.LogInformation("{ClassName}, {MethodName}, Called at {Time}, CompanyName: {CompanyName}",
                CLASSNAME, methodName, DateTime.Now, companyName);

            try
            {
                var existingCompany = await _unitOfWork.SubContractors.FindAsync(c => c.CompanyMainName == companyName);

                if (existingCompany != null)
                {
                    _logger.LogInformation("{ClassName}, {MethodName}, Existing company found, CompanyId: {CompanyId}",
                        CLASSNAME, methodName, existingCompany.CompanyId);

                    return existingCompany.CompanyId;
                }

                companyName = companyName.Replace(" ", "").ToUpper();

                var allCompanies = await _unitOfWork.SubContractors.GetAllAsync();

                if (allCompanies == null || !allCompanies.Any())
                {
                    var newCompanyCode = companyName.Substring(0, 3).ToUpper() + "0001";

                    _logger.LogInformation("{ClassName}, {MethodName}, No existing companies found, Generated CompanyCode: {CompanyCode}",
                        CLASSNAME, methodName, newCompanyCode);

                    return newCompanyCode;
                }

                var lastCompany = allCompanies.OrderByDescending(c => c.Id).FirstOrDefault();
                var companyId = lastCompany.CompanyId;
                var numericPart = int.Parse(companyId.Substring(3));
                numericPart++;
                var nextCompanyCode = companyName.Substring(0, 3).ToUpper() + numericPart.ToString("D4");

                _logger.LogInformation("{ClassName}, {MethodName}, Computed next CompanyCode: {NextCompanyCode}, LastCompanyId: {LastCompanyId}",
                    CLASSNAME, methodName, nextCompanyCode, companyId);

                return nextCompanyCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Error while generating next CompanyCode, CompanyName: {CompanyName}",
                    CLASSNAME, methodName, companyName);

                throw new Exception("An error occurred while retrieving the CompanyCode.", ex);
            }
        }



        public async Task<CombinedSubContractorAndContractDto> GetSubContractorById(long id)
        {
            const string methodName = nameof(GetSubContractorById);
            _logger.LogInformation("{ClassName}, {MethodName}, Called at {Time}, SubContractorId: {Id}",
                CLASSNAME, methodName, DateTime.Now, id);

            try
            {
                var subContractor = await _unitOfWork.SubContractors.GetByIdAsync(id);
                if (subContractor == null)
                {
                    _logger.LogInformation("{ClassName}, {MethodName}, No subcontractor found, Id: {Id}",
                        CLASSNAME, methodName, id);
                    throw new Exception("No subcontractors found.");
                }

                var contractDetails = await _unitOfWork.ContractDetails.GetByIdAsync(subContractor.ContractId);
                if (contractDetails == null)
                {
                    _logger.LogInformation("{ClassName}, {MethodName}, No contract details found for SubContractorId: {Id}",
                        CLASSNAME, methodName, id);
                    throw new Exception("No contract detail found.");
                }

                var serviceTypes = await _unitOfWork.ServiceTypeProvided.GetAllAsync(c => c.SubContractorId == id);
                if (serviceTypes == null)
                {
                    _logger.LogInformation("{ClassName}, {MethodName}, No service types found for SubContractorId: {Id}",
                        CLASSNAME, methodName, id);
                    throw new Exception("No service types found.");
                }

                subContractor.ServiceTypeProvided = serviceTypes.ToList();

                var combinedDto = new CombinedSubContractorAndContractDto
                {
                    SubContractor = subContractor,
                    ContractDetails = contractDetails
                };

                _logger.LogInformation("{ClassName}, {MethodName}, SubContractor and contract details combined successfully, Id: {Id}",
                    CLASSNAME, methodName, id);

                return combinedDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Error while combining SubContractor and contract details, Id: {Id}",
                    CLASSNAME, methodName, id);
                throw;
            }
        }


        public async Task<ResponseDto> AddContractAsync(SubContractor contractDetail, string submissionToken, string loggedinUserName)
        {
            const string methodName = nameof(AddContractAsync);
            _logger.LogInformation("{ClassName}, {MethodName}, Adding SubContractor, User: {UserName}",
                CLASSNAME, methodName, loggedinUserName);

            var responseDto = new ResponseDto();

            try
            {
                var existingToken = await _unitOfWork.SubmissionTokenRecord.FindAsync(t => t.Token == submissionToken);
                if (existingToken != null)
                {
                    return new ResponseDto
                    {
                        Success = false,
                        Message = "This form has already been submitted."
                    };
                }

                // 2️⃣ Save token first
                await _unitOfWork.SubmissionTokenRecord.AddAsync(new SubmissionTokenRecord
                {
                    Token = submissionToken,
                    CreatedAt = DateTime.Now
                });

                contractDetail.AddedBy = loggedinUserName;
                contractDetail.AddedOn = DateTime.Now;
                await _unitOfWork.SubContractors.AddAsync(contractDetail);

                responseDto.Success = true;
                responseDto.Message = "SubContractor added successfully!";

                _logger.LogInformation("{ClassName}, {MethodName}, SubContractor added successfully, Id: {SubContractorId}",
                    CLASSNAME, methodName, contractDetail.Id);
            }
            catch (Exception ex)
            {
                responseDto.Success = false;
                responseDto.Message = $"An error occurred: {ex.Message}";

                _logger.LogError(ex, "{ClassName}, {MethodName}, Failed to add SubContractor, User: {UserName}",
                    CLASSNAME, methodName, loggedinUserName);
            }

            return responseDto;
        }


        public async Task<ResponseDto> UpdateContract(SubContractor contract, string loggedinUserName)
        {
            const string methodName = nameof(UpdateContract);
            _logger.LogInformation("{ClassName}, {MethodName}, Updating SubContractor, User: {UserName}, Id: {SubContractorId}",
                CLASSNAME, methodName, loggedinUserName, contract.Id);

            var responseDto = new ResponseDto();
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var existingEvent = await _unitOfWork.SubContractors.GetByIdAsync(contract.Id);
                contract.AddedBy = existingEvent.AddedBy;
                contract.AddedOn = existingEvent.AddedOn;
                contract.UpdatedBy = loggedinUserName;
                contract.UpdatedOn = DateTime.Now;

                await _unitOfWork.SubContractors.UpdateAsync(contract);

                _logger.LogInformation("{ClassName}, {MethodName}, Updated SubContractor details in repository, Id: {SubContractorId}",
                    CLASSNAME, methodName, contract.Id);

                await _unitOfWork.ServiceTypeProvided.DeleteAgainstFieldAsync(contract.Id, "SubContractorId");
                _unitOfWork.ServiceTypeProvided.AddRange(contract.ServiceTypeProvided);

                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                responseDto.Success = true;
                responseDto.Message = "SubContractor updated successfully!";

                _logger.LogInformation("{ClassName}, {MethodName}, Update transaction committed, Success: {Success}, Id: {SubContractorId}",
                    CLASSNAME, methodName, responseDto.Success, contract.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                responseDto.Success = false;
                responseDto.Message = $"An error occurred while updating SubContractor: {ex.Message}";

                _logger.LogError(ex, "{ClassName}, {MethodName}, Failed to update SubContractor, Id: {SubContractorId}, User: {UserName}",
                    CLASSNAME, methodName, contract.Id, loggedinUserName);
            }

            return responseDto;
        }


        public async Task<IEnumerable<SubContractor>> GetSubContractorByCompanyNameForSearching(string companyName)
        {
            const string methodName = nameof(GetSubContractorByCompanyNameForSearching);
            _logger.LogInformation("{ClassName}, {MethodName}, Called at {Time}, CompanyName: {CompanyName}",
                CLASSNAME, methodName, DateTime.Now, companyName);

            try
            {
                IEnumerable<SubContractor> result;

                if (string.IsNullOrEmpty(companyName))
                {
                    result = await _unitOfWork.SubContractors.FindForSearchingAsync(c => true);
                }
                else
                {
                    result = await _unitOfWork.SubContractors.FindForSearchingAsync(
                        c => c.CompanyMainName.ToLower().Contains(companyName.ToLower())
                    );
                }

                _logger.LogInformation("{ClassName}, {MethodName}, SubContractors retrieved successfully, Count: {Count}",
                    CLASSNAME, methodName, result.Count());

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ClassName}, {MethodName}, Error occurred while searching SubContractors for CompanyName: {CompanyName}",
                    CLASSNAME, methodName, companyName);

                throw new Exception("Error while fetching contract details.", ex);
            }
        }


        public async Task<List<ContractDetails>> GetContractIdsBySubContractorCompanyName(string companyName)
        {
            const string methodName = nameof(GetContractIdsBySubContractorCompanyName);
            _logger.LogInformation(
                "{ClassName}, {MethodName}, Called at {Time}, CompanyName: {CompanyName}",
                CLASSNAME, methodName, DateTime.Now, companyName);

            try
            {
                var subcontractors = await _unitOfWork.SubContractors.FindForSearchingAsync(sc => sc.CompanyMainName == companyName);

                if (subcontractors == null || !subcontractors.Any())
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, No subcontractors found for CompanyName: {CompanyName}",
                        CLASSNAME, methodName, companyName);
                    return new List<ContractDetails>();
                }

                var contractIds = subcontractors.Select(s => s.ContractId).Distinct().ToList();
                if (contractIds == null || !contractIds.Any())
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, No contract IDs found for CompanyName: {CompanyName}",
                        CLASSNAME, methodName, companyName);
                    return new List<ContractDetails>();
                }

                List<ContractDetails> contractDetails = new List<ContractDetails>();

                foreach (var id in contractIds)
                {
                    var contractDet = await _unitOfWork.ContractDetails.FindForSearchingAsync(sc => sc.Id == id);
                    if (contractDet != null && contractDet.Any())
                    {
                        contractDetails.AddRange(contractDet);
                    }
                }

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Contract details retrieved successfully, Count: {Count}",
                    CLASSNAME, methodName, contractDetails.Count);

                return contractDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Error occurred while fetching contract details for CompanyName: {CompanyName}",
                    CLASSNAME, methodName, companyName);

                throw new Exception("Error while fetching contract details.", ex);
            }
        }


        public async Task<IEnumerable<SubContractor>> GetCompanyNameByTermAsync(string term)
        {
            const string methodName = nameof(GetCompanyNameByTermAsync);
            _logger.LogInformation(
                "{ClassName}, {MethodName}, Called at {Time}, Term: {Term}",
                CLASSNAME, methodName, DateTime.Now, term);

            try
            {
                if (string.IsNullOrEmpty(term))
                {
                    _logger.LogWarning(
                        "{ClassName}, {MethodName}, Term is null or empty, returning empty list",
                        CLASSNAME, methodName);
                    return new List<SubContractor>();
                }

                var subcontractors = await _unitOfWork.SubContractors.GetAllAsync(c => c.CompanyMainName.Contains(term));

                var result = subcontractors
                    .Select(s => new SubContractor { CompanyMainName = s.CompanyMainName })
                    .GroupBy(dto => dto.CompanyMainName)
                    .Select(group => group.First())
                    .ToList();

                _logger.LogInformation(
                    "{ClassName}, {MethodName}, Retrieved unique company names, Count: {Count}",
                    CLASSNAME, methodName, result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{ClassName}, {MethodName}, Error occurred while fetching company names for Term: {Term}",
                    CLASSNAME, methodName, term);

                return new List<SubContractor>(); // Return an empty list if an error occurs
            }
        }


    }
}
