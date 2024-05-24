﻿using ManufacturingApp.API.Interfaces;
using ManufacturingApp.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManufacturingApp.API.Data
{
    public class ManufacturingRepository<T> : IManufacturingRepository<T> where T : class
    {
        private readonly ManufacturingContext _manufacturingContext;
        private readonly DbSet<T> _dbSets;
        private readonly ILogger<ManufacturingRepository<T>> _logger;

        public ManufacturingRepository(ManufacturingContext manufacturingContext, ILogger<ManufacturingRepository<T>> logger)
        {
            _manufacturingContext = manufacturingContext;
            _dbSets = _manufacturingContext.Set<T>();
            _logger = logger;
        }

        public async Task<ICollection<T>> GetAllAsync()
        {
            try
            {
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
            try
            {
                var entity = await _dbSets.FindAsync(id);
                return entity ?? null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when getting the entity");
                throw;
            }
        }

        public async Task CreateAsync(T entity)
        {
            try
            {
                await _dbSets.AddAsync(entity);
                await _manufacturingContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when creating the entity");
                throw;
            }
        }
       
       public async Task UpdateAsync(T entity)
        {
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
                _logger.LogError(ex, "An error occurred when deleting the entity");
                throw;
            }
        }
    }
}
