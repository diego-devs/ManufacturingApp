using ManufacturingApp.API.Interfaces;
using ManufacturingApp.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManufacturingApp.API.Data
{
    public class ManufacturingRepository<T> : IManufacturingRepository<T> where T : class
    {
        // Concrete implementation
        private readonly ManufacturingContext _manufacturingContext;
        private readonly DbSet<T> _dbSets;
        private readonly ILogger _logger;

        public ManufacturingRepository(ManufacturingContext manufacturingContext, ILogger logger)
        {
            _manufacturingContext = manufacturingContext;
            _dbSets = _manufacturingContext.Set<T>();
            _logger = logger;
        }

        public async Task<ICollection<T>> GetAllAsync()
        {
            try
            {
                // get all elements from entity from db
                return await _dbSets.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all entities");
                throw;
            }
            
        }
        public async Task<T> GetAsync(int id)
        {
            // get one entity from context by id
            try
            {
                var entity = await _dbSets.FindAsync(id);
                if (entity != null)
                {
                    return entity;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when getting the entity");
                throw;
            }
        }

        public async void CreateAsync(T entity)
        {
            try
            {
                // context.Add to database
                // save changes
                await _dbSets.AddAsync(entity);
                await _manufacturingContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when creating the entity");
                throw;
            }
        }
       
       public async void UpdateAsync(T entity)
        {
            // update entity 
            try
            {
                _dbSets.Update(entity);
                await _manufacturingContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when updating the entity");
                throw;
            }
        }
        public async void DeleteAsync(int id)
        {
            try
            {
                // context delete from database
                // save changes
                var entity = await GetAsync(id);
                if (entity != null)
                {
                    _dbSets.Remove(entity);
                    await _manufacturingContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when deleting the entity");
                throw;
            }
        }
    }
}
