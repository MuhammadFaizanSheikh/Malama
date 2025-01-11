using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Models;
using ExcelFilesCompiler.Repositories.Interfaces;
using ExcelFilesCompiler.UnitOfWork;
using ExcelToCsv.Models;
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
        private readonly IGenericRepository<SubContractorInfoDto> repository;
        private readonly IUnitOfWork _unitOfWork;

        public SubContractorService(IGenericRepository<SubContractorInfoDto> repository, IUnitOfWork unitOfWork)
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
                var subcontractors = await _unitOfWork.SubContractors.GetAllAsync();
                var contracts = await _unitOfWork.ContractDetails.GetAllAsync();

                var result = from sub in subcontractors
                            join contract in contracts on sub.ContractId equals contract.Id
                            select new SubContractorAndContractViewModel
                            {
                                Id = sub.Id,
                                CompanyId = sub.CompanyId,
                                ContractId = contract.ContractID,
                                ContractClient = contract.ContractClient,
                                ContractType = contract.ContractType,
                                SmallBusinessType = sub.SmallBusinessType,
                                ContractAffiliation = sub.ContractAffiliation,
                                ContractServiceBranch = contract.ContractServiceBranch,
                                ContractComponent = contract.ContractComponent,
                                SolicitationNumber = sub.SolicitationNumber,
                                CompanyMainName = sub.CompanyMainName,
                            };

                return result.ToList();
                //contracts = (await repository.GetAllAsync()).OrderByDescending(c => c.Id).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving subcontractors.", ex);
            }
        }

        public async Task<string> GetLastCompanyCode()
        {
            try
            {
                var allCompanies = await repository.GetAllAsync();

                if (allCompanies == null || !allCompanies.Any())
                {
                    // If no records exist, return the default CompanyCode with the initial sequence
                    return "0001"; // Default starting code
                }

                // Step 2: Sort the records in descending order based on the Id or another relevant property
                var lastCompany = allCompanies
                    .OrderByDescending(c => c.Id) // Sort by Id or another property as necessary
                    .FirstOrDefault();

                // Step 3: Extract and process the CompanyCode
                var companyId = lastCompany.CompanyId;
                var numericPart = int.Parse(companyId.Substring(3)); // Get the numeric part (e.g., "0001")

                // Step 4: Increment the numeric part by 1
                numericPart++;

                // Step 5: Generate the new CompanyCode
                return numericPart.ToString("D4"); // Format the number to 4 digits (e.g., 0009, 0010, 0100)
            }
            catch (Exception)
            {
                throw new Exception("An error occurred while retrieving the CompanyCode.");
            }
        }

        public async Task<CombinedSubContractorAndContractDto> GetSubContractorById(long id)
        {

            try
            {
                var subContractor = await repository.GetByIdAsync(id);

                if (subContractor == null)
                {
                    return null; // Return null if not found
                }

                // Fetch the related contract details using the ContractId
                var contractDetails = await _unitOfWork.ContractDetails.GetByIdAsync(subContractor.ContractId);

                // If you want to combine the data into a single DTO, you can create a new DTO for that purpose
                // For example, you can create a new DTO that includes both SubContractor and ContractDetails data
                var combinedDto = new CombinedSubContractorAndContractDto
                {
                    SubContractor = subContractor,
                    ContractDetails = contractDetails
                };

                return combinedDto; // Return the combined DTO
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ResponseDto> AddContractAsync(SubContractorInfoDto contractDetail, string loggedinUserName)
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

        public async Task<ResponseDto> UpdateContract(SubContractorInfoDto contract, string loggedinUserName)
        {
            var responseDto = new ResponseDto();

            try
            {
                contract.UpdatedBy = loggedinUserName;
                contract.UpdatedOn = DateTime.Now;
                await repository.UpdateAsync(contract);
                responseDto.Success = true;
                responseDto.Message = "SubContractor updated successfully!";
            }
            catch (Exception ex)
            {
                responseDto.Success = false;
                responseDto.Message = $"An error occurred while updating SubContractor: {ex.Message}";
            }

            return responseDto;
        }

        public async Task<IEnumerable<SubContractorInfoDto>> GetSubContractorByCompanyNameForSearching(string companyName)
        {
            try
            {
                if (string.IsNullOrEmpty(companyName))
                {
                    return await repository.FindForSearchingAsync(c => true);
                }

                return await repository.FindForSearchingAsync(c => c.CompanyMainName.Contains(companyName));
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

        public async Task<IEnumerable<SubContractorInfoDto>> GetCompanyNameByTermAsync(string term)
        {
            try
            {
                if (string.IsNullOrEmpty(term))
                {
                    return new List<SubContractorInfoDto>();
                }

                var subcontractors = await _unitOfWork.SubContractors.GetAllAsync(c => c.CompanyMainName.Contains(term));

                return subcontractors
                    .Select(s => new SubContractorInfoDto
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
                return new List<SubContractorInfoDto>(); // Return an empty list if an error occurs
            }
        }

    }
}
