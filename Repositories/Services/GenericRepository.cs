using ExcelFilesCompiler.Models;
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

        public async Task DeleteAsync(int id)
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

        public T? FindFirstOrDefaultByEventId(string eventId)
        {
            try
            {
                return _context.Set<T>().FirstOrDefault(e => "EventId" == eventId);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while finding the record with EventId {eventId}.", ex);
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

        public async Task<IEnumerable<T>> GetWithIncludeAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.Where(predicate).ToListAsync();
        }
    }
}
