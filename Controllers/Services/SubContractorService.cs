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
    }
}
