using ManufacturingApp.API.Interfaces;
using ManufacturingApp.Data;
using Microsoft.EntityFrameworkCore;

namespace ManufacturingApp.API.Data
{
    public class ManufacturingRepository<T> : IManufacturingRepository<T> where T : class
    {
        // Concrete implementation

        private readonly ManufacturingContext _manufacturingContext;
        private readonly DbSet<T> _dbSets;

        public ManufacturingRepository(ManufacturingContext manufacturingContext)
        {
            _manufacturingContext = manufacturingContext;
            _dbSets = _manufacturingContext.Set<T>();
        }

        public async Task<ICollection<T>> GetAllAsync()
        {
            // get all elements from entity from db
            return await _dbSets.ToListAsync();
        }
        public async Task<T> GetAsync(int id)
        {
            var entity = await _dbSets.FindAsync(id);
            if (entity != null) 
            {
                return entity;
            } 
            else
            {
                throw new NullReferenceException();
            }
        }

        public async void CreateAsync(T entity)
        {
            // context.Add to database
            // save changes
            await _dbSets.AddAsync(entity);
            await _manufacturingContext.SaveChangesAsync();
            
        }
       
       public async void UpdateAsync(T entity)
        {
            _dbSets.Update(entity);
            await _manufacturingContext.SaveChangesAsync();
        }
        public async void DeleteAsync(int id)
        {
            // context delete from database
            // save changes
            // return true
            var entity = await GetAsync(id);
            _dbSets.Remove(entity);
            await _manufacturingContext.SaveChangesAsync();

        }
    }
}
