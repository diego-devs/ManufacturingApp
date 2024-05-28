using ManufacturingApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ManufacturingApp.API.Interfaces
{
    public interface IManufacturingRepository<T> where T : class
    {
        // CRUD IProductRepo, IRawMaterialRepo, etc...
        public Task<ICollection<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>> include);
        public Task<ICollection<T>> GetAllAsync();
        public Task<T> GetAsync(int id, Func<IQueryable<T>, IQueryable<T>> include = null);
        public Task<T> GetAsync(object[] keyValues, Func<IQueryable<T>, IQueryable<T>> include = null);
        public Task<T> GetAsync(int id);
        public Task CreateAsync([FromBody] T entity);
        public Task UpdateAsync([FromBody] T entity);
        public Task DeleteAsync(int id);

    }
}
