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
    public class SubContractorService : ISubContractorService
    {
        private readonly IGenericRepository<SubContractorInfoDto> repository;

        public SubContractorService(IGenericRepository<SubContractorInfoDto> repository)
        {
            this.repository = repository;
        }

        public async Task<List<SubContractorInfoDto>> GetAllSubContractors()
        {
            var responseDto = new ResponseDto();
            List<SubContractorInfoDto> contracts = new List<SubContractorInfoDto>(); // Initialize contracts outside try-catch

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

        public async Task<SubContractorInfoDto> GetSubContractorById(long id)
        {
            SubContractorInfoDto subContractor = null;

            try
            {
                subContractor = await repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw;
            }

            return subContractor;
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
    }
}
