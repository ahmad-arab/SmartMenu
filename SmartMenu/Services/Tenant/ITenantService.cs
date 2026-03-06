using SmartMenu.Models.Tenant;

namespace SmartMenu.Services.Tenant
{
    public interface ITenantService
    {
        Task<IEnumerable<TenantListViewModel>> GetAllTenantsAsync();
        Task<EditTenantViewModel?> GetTenantForEditAsync(int id);
        Task CreateTenantAsync(CreateTenantViewModel model);
        Task<bool> UpdateTenantAsync(int id, EditTenantViewModel model);
        Task<bool> DeleteTenantAsync(int id);
    }
}
