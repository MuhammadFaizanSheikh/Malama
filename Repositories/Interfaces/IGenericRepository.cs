using Malama.Models;
using System.Linq.Expressions;

namespace ExcelFilesCompiler.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetAllNoTracking();
        IQueryable<T> GetAllWithConditionNoTracking(Expression<Func<T, bool>> predicate);
        Task<T> GetFirstOrDefaultWithConditionNoTracking(Expression<Func<T, bool>> predicate);
        Task<T?> GetByIdAsync(long? id);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task<IEnumerable<T>> GetWithIncludeAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task DeleteAgainstFieldAsync(object id, string idPropertyName);
        IQueryable<T> GetWithInclude(Expression<Func<T, bool>> predicate = null,params Expression<Func<T, object>>[] includes);
        void RemoveRange(IEnumerable<T> entities);

    }
}
