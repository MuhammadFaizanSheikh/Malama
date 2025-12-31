using Malama.Models;
using ExcelFilesCompiler.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return await _dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving all records.", ex);
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(long id)
        {
            try
            {
                return await _dbSet.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving the record with ID {id}.", ex);
            }
        }

        public async Task<T?> GetByNullableIdAsync(long? id)
        {
            try
            {
                return await _dbSet.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving the record with ID {id}.", ex);
            }
        }

        public void AddRange(IEnumerable<T> entities)
        {
            try
            {
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                _dbSet.AddRange(entities);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding multiple records.", ex);
            }
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            try
            {
                await _dbSet.AddRangeAsync(entities);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding multiple records.", ex);
            }
        }

        public async Task AddAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity); // Add the entity to the DbSet
                await _context.SaveChangesAsync(); // Save changes to the database
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding a record.", ex);
            }
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

        public async Task DeleteAsync(long id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while deleting the record with ID {id}.", ex);
            }
        }

        public void Save()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while saving changes.", ex);
            }
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            try
            {
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                _dbSet.UpdateRange(entities);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating multiple records.", ex);
            }
        }

        public IEnumerable<T> FindByEventId(string eventId)
        {
            try
            {
                return _context.Set<T>()
                .Where(e => EF.Property<string>(e, "EventId") == eventId && EF.Property<bool>(e, "isDeleted") == false).ToList();

            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while finding records with EventId {eventId}.", ex);
            }
        }

        public async Task<IEnumerable<T>> FindForSearchingAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await _dbSet.Where(predicate).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while querying the database.", ex);
            }
        }

        public IQueryable<T> FindForSearching(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return _dbSet.Where(predicate).AsNoTracking(); // ✅ returns IQueryable
            }
            catch (Exception ex)
            {
                throw new Exception("Error while querying the database.", ex);
            }
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                // Find the first matching entity
                return await _dbSet.FirstOrDefaultAsync(predicate);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while querying the database.", ex);
            }
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetWithIncludeAsync(
      Expression<Func<T, bool>> predicate = null,
      params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            // Apply includes for eager loading
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // Apply predicate only if it is not null
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


        public void RemoveRange(IEnumerable<T> entities)
        {
            try
            { 
                _dbSet.RemoveRange(entities);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding multiple records.", ex);
            }
        }
    }
}
