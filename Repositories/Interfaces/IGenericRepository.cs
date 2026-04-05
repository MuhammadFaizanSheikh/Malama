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
        IQueryable<T> GetWithIncludeTracking(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includes);
        IQueryable<T> GetWithIncludeNoTracking(Expression<Func<T, bool>> predicate = null,params Expression<Func<T, object>>[] includes);
        void RemoveRange(IEnumerable<T> entities);

    }
}
