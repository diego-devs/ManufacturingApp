using ManufacturingApp.API.Interfaces;
using ManufacturingApp.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace ManufacturingApp.API.Data
{
    /// <summary>
    /// Generic repository for performing CRUD operations on entities.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public class ManufacturingRepository<T> : IManufacturingRepository<T> where T : class
    {
        private readonly ManufacturingContext _manufacturingContext;
        private readonly DbSet<T> _dbSets;
        private readonly ILogger<ManufacturingRepository<T>> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManufacturingRepository{T}"/> class.
        /// </summary>
        /// <param name="manufacturingContext">The manufacturing context.</param>
        /// <param name="logger">The logger instance.</param>
        public ManufacturingRepository(ManufacturingContext manufacturingContext, ILogger<ManufacturingRepository<T>> logger)
        {
            _manufacturingContext = manufacturingContext;
            _dbSets = _manufacturingContext.Set<T>();
            _logger = logger;
        }

        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <param name="include">A function to include related entities.</param>
        /// <returns>A collection of all entities.</returns>
        public async Task<ICollection<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>> include = null)
        {
            try
            {
                IQueryable<T> query = _dbSets;

                if (include != null)
                {
                    query = include(query);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorGettingAll);
                throw;
            }
        }

        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <returns>A collection of all entities.</returns>
        public async Task<ICollection<T>> GetAllAsync()
        {
            return await GetAllAsync(null);
        }

        /// <summary>
        /// Gets a specific entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity.</param>
        /// <param name="include">A function to include related entities.</param>
        /// <returns>The entity with the specified ID.</returns>
        public async Task<T> GetAsync(int id, Func<IQueryable<T>, IQueryable<T>> include = null)
        {
            try
            {
                IQueryable<T> query = _dbSets;

                if (include != null)
                {
                    query = include(query);
                }

                var entity = await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
                return entity ?? null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorGettingById);
                throw;
            }
        }

        /// <summary>
        /// Gets a specific entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity.</param>
        /// <returns>The entity with the specified ID.</returns>
        public async Task<T> GetAsync(int id)
        {
            return await GetAsync(id, null);
        }

        /// <summary>
        /// Creates a new entity.
        /// </summary>
        /// <param name="entity">The entity to create.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task CreateAsync(T entity)
        {
            try
            {
                await _dbSets.AddAsync(entity);
                await _manufacturingContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorCreating);
                throw;
            }
        }

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateAsync(T entity)
        {
            try
            {
                _dbSets.Update(entity);
                await _manufacturingContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorUpdating);
                throw;
            }
        }

        /// <summary>
        /// Deletes a specific entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await GetAsync(id);
                if (entity != null)
                {
                    _dbSets.Remove(entity);
                    await _manufacturingContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApiMessages.ErrorDeleting);
                throw;
            }
        }
    }
}
