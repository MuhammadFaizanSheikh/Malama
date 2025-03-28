using ExcelFilesCompiler.Models;
using ExcelFilesCompiler.Repositories.Interfaces;
using ExcelToCsv.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExcelFilesCompiler.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<ContractDetails> ContractDetails { get; }
        IGenericRepository<SubContractor> SubContractors { get; }
        IGenericRepository<EventStaff> EventStaff { get; }
        IGenericRepository<StaffLicense> StaffLicense { get; }
        IGenericRepository<StaffContractAffiliation> StaffContractAffiliation { get; }
        IGenericRepository<TravelHonor> TravelHonor { get; }
        IGenericRepository<ServiceTypeProvided> ServiceTypeProvided { get; }
        IGenericRepository<EventManagement> EventManagement { get; }
        IGenericRepository<EventServiceDetail> EventServiceDetail { get; }
        IGenericRepository<EventStartEndTimeDayWise> EventStartEndTimeDayWise { get; }
        IGenericRepository<EventStaffDetail> EventStaffDetail { get; }
        IGenericRepository<EventManagementTaskforces> EventManagementTaskforces { get; }
        IGenericRepository<EventManagementStaffAvailability> EventManagementStaffAvailability { get; }
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task SaveAsync();
    }
}
