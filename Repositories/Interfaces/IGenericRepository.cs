using ExcelFilesCompiler.Models;
using NPOI.SS.Formula.Functions;
using System.Linq.Expressions;

namespace ExcelFilesCompiler.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetByIdAsync(long id);
        Task<T?> GetByNullableIdAsync(long? id);
        void AddRange(IEnumerable<T> entities);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        void Save();
        void UpdateRange(IEnumerable<T> entities);
        //Task<IEnumerable<T>> FindByColumnAsync<T>(string columnName, object value) where T : class;
        IEnumerable<T> FindByEventId(string eventId);
        Task<IEnumerable<T>> FindForSearchingAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetWithIncludeAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task DeleteAgainstFieldAsync(object id, string idPropertyName);
        Task<T> FindAsync(Expression<Func<T, bool>> predicate);

    }
}
