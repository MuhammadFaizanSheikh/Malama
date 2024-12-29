using ExcelFilesCompiler.Models;
using ExcelFilesCompiler.Repositories.Interfaces;
using ExcelToCsv.Models;

namespace ExcelFilesCompiler.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<ContractDetails> ContractDetails { get; }
        IGenericRepository<SubContractorInfoDto> SubContractors { get; }
        IGenericRepository<EventStaffDto> EventStaff { get; }
        Task SaveAsync();
    }
}
