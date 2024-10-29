using NPOI.SS.Formula.Functions;

namespace ExcelFilesCompiler.Repositories.Interfaces
{
    public interface IFileUploaderRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        void AddRange(IEnumerable<T> entities);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        void Save();
        void UpdateRange(IEnumerable<T> entities);
        T? FindFirstOrDefaultByEventId(string eventId);
        IEnumerable<T> FindByEventId(string eventId);
    }
}
