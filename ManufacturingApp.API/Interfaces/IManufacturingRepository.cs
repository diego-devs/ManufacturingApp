using ManufacturingApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ManufacturingApp.API.Interfaces
{
    public interface IManufacturingRepository<T> where T : class
    {
        // CRUD IProductRepo, IRawMaterialRepo, etc...
        public Task<ICollection<T>> GetAllAsync();
        public Task<T> GetAsync(int id);
        public void CreateAsync([FromBody] T entity);
        public void UpdateAsync([FromBody] T entity);
        public void DeleteAsync(int id);

    }
}
