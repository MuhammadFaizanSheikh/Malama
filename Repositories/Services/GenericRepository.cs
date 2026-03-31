using ExcelFilesCompiler.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExcelFilesCompiler.Repositories.Services
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public IQueryable<T> GetAllNoTracking()
        {
            return _dbSet.AsNoTracking();
        }

        public IQueryable<T> GetAllWithConditionNoTracking(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.AsNoTracking().Where(predicate);
        }

        public async Task<T> GetFirstOrDefaultWithConditionNoTracking(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public async Task<T?> GetByIdAsync(long? id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public async Task UpdateAsync(T entity)
        {
            try
            {
                var keyProperty = typeof(T).GetProperty("Id"); // Access the primary key dynamically
                if (keyProperty == null)
                {
                    throw new Exception("Entity does not have a property named 'Id'.");
                }

                var keyValue = keyProperty.GetValue(entity); // Get the value of the Id
                var existingEntity = await _dbSet.FindAsync(keyValue);

                if (existingEntity == null)
                {
                    throw new Exception("Entity does not exist in the database.");
                }

                _context.Entry(existingEntity).CurrentValues.SetValues(entity); // Update values
                await _context.SaveChangesAsync(); // Save changes to database
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the record.", ex);
            }
        }

        public async Task<IEnumerable<T>> GetWithIncludeAsync(
      Expression<Func<T, bool>> predicate = null,
      params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.ToListAsync();
        }

        public IQueryable<T> GetWithInclude(
    Expression<Func<T, bool>> predicate = null,
    params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return query; // Return IQueryable instead of executing ToListAsync()
        }

        public async Task DeleteAgainstFieldAsync(object id, string idPropertyName)
        {
            // Fetch the entity using the non-primary key (idPropertyName)
            var entities = await _dbSet.Where(e => EF.Property<object>(e, idPropertyName).Equals(id)).ToListAsync();
            if (entities.Any())
            {
                _dbSet.RemoveRange(entities);
            }
        }
    }
}
