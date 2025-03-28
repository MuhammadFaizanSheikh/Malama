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
        public IGenericRepository<SubContractor> SubContractors { get; private set; }
        public IGenericRepository<EventStaff> EventStaff { get; private set; }
        public IGenericRepository<StaffLicense> StaffLicense { get; private set; }
        public IGenericRepository<StaffContractAffiliation> StaffContractAffiliation { get; private set; }
        public IGenericRepository<TravelHonor> TravelHonor { get; private set; }
        public IGenericRepository<ServiceTypeProvided> ServiceTypeProvided { get; private set; }
        public IGenericRepository<EventManagement> EventManagement { get; private set; }
        public IGenericRepository<EventServiceDetail> EventServiceDetail { get; private set; }
        public IGenericRepository<EventStartEndTimeDayWise> EventStartEndTimeDayWise { get; private set; }
        public IGenericRepository<EventStaffDetail> EventStaffDetail { get; private set; }
        public IGenericRepository<EventManagementTaskforces> EventManagementTaskforces { get; private set; }
        public IGenericRepository<EventManagementStaffAvailability> EventManagementStaffAvailability { get; private set; }
        


        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            ContractDetails = new GenericRepository<ContractDetails>(_context);
            SubContractors = new GenericRepository<SubContractor>(_context);
            EventStaff = new GenericRepository<EventStaff>(_context);
            StaffLicense = new GenericRepository<StaffLicense>(_context);
            StaffContractAffiliation = new GenericRepository<StaffContractAffiliation>(_context);
            TravelHonor = new GenericRepository<TravelHonor>(_context);
            ServiceTypeProvided = new GenericRepository<ServiceTypeProvided>(_context);
            EventManagement = new GenericRepository<EventManagement>(_context);
            EventServiceDetail = new GenericRepository<EventServiceDetail>(_context);
            EventStartEndTimeDayWise = new GenericRepository<EventStartEndTimeDayWise>(_context);
            EventStaffDetail = new GenericRepository<EventStaffDetail>(_context);
            EventManagementTaskforces = new GenericRepository<EventManagementTaskforces>(_context);
            EventManagementStaffAvailability = new GenericRepository<EventManagementStaffAvailability>(_context);
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
