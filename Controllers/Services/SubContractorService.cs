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

namespace ExcelFilesCompiler.Controllers.Services
{
    public class SubContractorService : ISubContractorService
    {
        private readonly IGenericRepository<SubContractor> repository;
        private readonly IUnitOfWork _unitOfWork;

        public SubContractorService(IGenericRepository<SubContractor> repository, IUnitOfWork unitOfWork)
        {
            this.repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SubContractorAndContractViewModel>> GetAllSubContractors()
        {
            var responseDto = new ResponseDto();
            //List<SubContractorInfoDto> contracts = new List<SubContractorInfoDto>(); // Initialize contracts outside try-catch

            try
            {
                var subcontractors = await _unitOfWork.SubContractors.GetWithIncludeAsync(
                    null,
                    x => x.ServiceTypeProvided
                );

                var contracts = await _unitOfWork.ContractDetails.GetAllAsync();

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

                return result.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving subcontractors.", ex);
            }
        }

        public async Task<string> GetLastCompanyCode(string companyName)
        {
            try
            {
                var existingCompany = await repository.FindAsync(c => c.CompanyMainName == companyName);

                if (existingCompany != null)
                {
                    return existingCompany.CompanyId;
                }

                companyName = companyName.Replace(" ", "").ToUpper();

                var allCompanies = await repository.GetAllAsync();

                if (allCompanies == null || !allCompanies.Any())
                {
                    return companyName.Substring(0, 3).ToUpper() + "0001";
                }

                var lastCompany = allCompanies
                    .OrderByDescending(c => c.Id)
                    .FirstOrDefault();

                var companyId = lastCompany.CompanyId;
                var numericPart = int.Parse(companyId.Substring(3));

                numericPart++;

                return companyName.Substring(0, 3).ToUpper() + numericPart.ToString("D4");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the CompanyCode.", ex);
            }
        }


        public async Task<CombinedSubContractorAndContractDto> GetSubContractorById(long id)
        {

            try
            {
                var subContractor = await repository.GetByIdAsync(id);

                if (subContractor == null)
                {
                    throw new Exception("No subcontractors found.");
                }

                var contractDetails = await _unitOfWork.ContractDetails.GetByIdAsync(subContractor.ContractId);

                if (contractDetails == null)
                {
                    throw new Exception("No contract detail found.");
                }

                var serviceTypes = (await _unitOfWork.ServiceTypeProvided.GetAllAsync(c => c.SubContractorId == id));

                if (serviceTypes == null)
                {
                    throw new Exception("No service types found.");
                }

                subContractor.ServiceTypeProvided = serviceTypes.ToList();

                var combinedDto = new CombinedSubContractorAndContractDto
                {
                    SubContractor = subContractor,
                    ContractDetails = contractDetails
                };

                return combinedDto;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ResponseDto> AddContractAsync(SubContractor contractDetail, string loggedinUserName)
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
                responseDto.Message = "SubContractor added successfully!";
            }
            catch (Exception ex)
            {
                // If an exception occurs, set Success to false and provide the error message
                responseDto.Success = false;
                responseDto.Message = $"An error occurred: {ex.Message}";
            }

            return responseDto;
        }

        public async Task<ResponseDto> UpdateContract(SubContractor contract, string loggedinUserName)
        {
            var responseDto = new ResponseDto();
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var existingEvent = await _unitOfWork.SubContractors.GetByIdAsync(contract.Id);
                contract.AddedBy = existingEvent.AddedBy;
                contract.AddedOn = existingEvent.AddedOn;
                contract.UpdatedBy = loggedinUserName;
                contract.UpdatedOn = DateTime.Now;
                await repository.UpdateAsync(contract);


                await _unitOfWork.ServiceTypeProvided.DeleteAgainstFieldAsync(contract.Id, "SubContractorId");
                _unitOfWork.ServiceTypeProvided.AddRange(contract.ServiceTypeProvided);

                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                responseDto.Success = true;
                responseDto.Message = "SubContractor updated successfully!";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                responseDto.Success = false;
                responseDto.Message = $"An error occurred while updating SubContractor: {ex.Message}";
            }

            return responseDto;
        }

        public async Task<IEnumerable<SubContractor>> GetSubContractorByCompanyNameForSearching(string companyName)
        {
            try
            {
                if (string.IsNullOrEmpty(companyName))
                {
                    return await repository.FindForSearchingAsync(c => true);
                }

                //return await repository.FindForSearchingAsync(c => c.CompanyMainName.Contains(companyName));
                return await repository.FindForSearchingAsync(
           c => c.CompanyMainName.ToLower().Contains(companyName.ToLower())
       );
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching contract details.", ex);
            }
        }

        public async Task<List<ContractDetails>> GetContractIdsBySubContractorCompanyName(string companyName)
        {
            try
            {
                var subcontractors = await _unitOfWork.SubContractors.FindForSearchingAsync(sc => sc.CompanyMainName == companyName);
                if (subcontractors == null || !subcontractors.Any())
                {
                    throw new Exception("No subcontractors found for the given company.");
                }

                var contractIds = subcontractors.Select(s => s.ContractId).Distinct().ToList();
                if (contractIds == null || !contractIds.Any())
                {
                    throw new Exception("No contract IDs found for the given company.");
                }

                List<ContractDetails> contractDetails = new List<ContractDetails>();

                foreach (var id in contractIds)
                {
                    var contractDet = await _unitOfWork.ContractDetails.FindForSearchingAsync(sc => sc.Id == id);
                    if (contractDet != null && contractDet.Any()) // Ensure the result is not null or empty
                    {
                        contractDetails.AddRange(contractDet); // Add all matching items
                    }
                }


                // Step 4: Check if contract details are found
                if (contractDetails == null || !contractDetails.Any())
                {
                    throw new Exception("No contract details found for the given contract IDs.");
                }

                return contractDetails.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching contract details.", ex);
            }
        }

        public async Task<IEnumerable<SubContractor>> GetCompanyNameByTermAsync(string term)
        {
            try
            {
                if (string.IsNullOrEmpty(term))
                {
                    return new List<SubContractor>();
                }

                var subcontractors = await _unitOfWork.SubContractors.GetAllAsync(c => c.CompanyMainName.Contains(term));

                return subcontractors
                    .Select(s => new SubContractor
                    {
                        CompanyMainName = s.CompanyMainName
                    })
                    .GroupBy(dto => dto.CompanyMainName) // Group by CompanyMainName to remove duplicates
                    .Select(group => group.First())     // Select the first unique instance
                    .ToList();
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework like Serilog or NLog)
                return new List<SubContractor>(); // Return an empty list if an error occurs
            }
        }

    }
}
