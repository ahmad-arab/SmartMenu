using SmartMenu.Data.Entities;

namespace SmartMenu.Repositories.Tenant
{
    public interface ITenantRepository
    {
        Task<IEnumerable<Data.Entities.Tenant>> GetAllAsync();
        Task<Data.Entities.Tenant?> GetByIdAsync(int id);
        Task<Data.Entities.Tenant?> GetByIdNoTrackingAsync(int id);
        Task AddAsync(Data.Entities.Tenant tenant);
        Task UpdateAsync(Data.Entities.Tenant tenant);
        Task DeleteAsync(Data.Entities.Tenant tenant);
    }
}
