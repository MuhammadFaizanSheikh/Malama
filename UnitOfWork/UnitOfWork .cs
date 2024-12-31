using ExcelFilesCompiler.Models;
using ExcelFilesCompiler.Repositories.Interfaces;
using ExcelFilesCompiler.Repositories.Services;
using ExcelToCsv.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExcelFilesCompiler.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IGenericRepository<ContractDetails> ContractDetails { get; private set; }
        public IGenericRepository<SubContractorInfoDto> SubContractors { get; private set; }
        public IGenericRepository<EventStaff> EventStaff { get; private set; }
        public IGenericRepository<LicenseInfoDTO> StaffLicenses { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            ContractDetails = new GenericRepository<ContractDetails>(_context);
            SubContractors = new GenericRepository<SubContractorInfoDto>(_context);
            EventStaff = new GenericRepository<EventStaff>(_context);
            StaffLicenses = new GenericRepository<LicenseInfoDTO>(_context);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }


        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
