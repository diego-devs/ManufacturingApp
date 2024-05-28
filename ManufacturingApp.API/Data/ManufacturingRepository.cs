﻿using ManufacturingApp.API.Interfaces;
using ManufacturingApp.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

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
                _logger.LogError(ex, "An error occurred while getting all entities");
                throw;
            }
        }
        public async Task<ICollection<T>> GetAllAsync()
        {
            return await GetAllAsync(null);
        }
        
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
                _logger.LogError(ex, "An error occurred when getting the entity");
                throw;
            }
        }
        public async Task<T> GetAsync(object[] keyValues, Func<IQueryable<T>, IQueryable<T>> include = null)
        {
            try
            {
                IQueryable<T> query = _dbSets;

                if (include != null)
                {
                    query = include(query);
                }

                var entity = await _dbSets.FindAsync(keyValues);
                return entity ?? null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred when getting the entity");
                throw;
            }
        }

        public async Task<T> GetAsync(int id)
        {
            return await GetAsync(id, null);
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