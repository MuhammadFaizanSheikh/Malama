using ExcelFilesCompiler.Models;
using ExcelFilesCompiler.Repositories.Interfaces;

namespace ExcelFilesCompiler.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<ContractDetails> ContractDetails { get; }
        IGenericRepository<SubContractorInfoDto> SubContractors { get; }
        Task SaveAsync();
    }
}
