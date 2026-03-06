using SmartMenu.Data.Entities;
using SmartMenu.Models.Tenant;
using SmartMenu.Repositories.Tenant;
using SmartMenu.Services.FileUpload;

namespace SmartMenu.Services.Tenant
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IFileUploadService _fileUploadService;

        public TenantService(ITenantRepository tenantRepository, IFileUploadService fileUploadService)
        {
            _tenantRepository = tenantRepository;
            _fileUploadService = fileUploadService;
        }

        public async Task<IEnumerable<TenantListViewModel>> GetAllTenantsAsync()
        {
            var tenants = await _tenantRepository.GetAllAsync();
            return tenants.Select(t => new TenantListViewModel
            {
                Id = t.Id,
                Name = t.Name
            });
        }

        public async Task<EditTenantViewModel?> GetTenantForEditAsync(int id)
        {
            var tenant = await _tenantRepository.GetByIdAsync(id);
            if (tenant == null)
                return null;

            return new EditTenantViewModel
            {
                Id = tenant.Id,
                Name = tenant.Name,
                LogoUrl = tenant.LogoUrl
            };
        }

        public async Task CreateTenantAsync(CreateTenantViewModel model)
        {
            var logoUrl = await _fileUploadService.UploadImageAsync(model.Logo, "tenant-logos");

            var tenant = new Data.Entities.Tenant
            {
                Name = model.Name,
                LogoUrl = logoUrl
            };

            await _tenantRepository.AddAsync(tenant);
        }

        public async Task<bool> UpdateTenantAsync(int id, EditTenantViewModel model)
        {
            var tenant = await _tenantRepository.GetByIdAsync(id);
            if (tenant == null)
                return false;

            tenant.Name = model.Name;

            if (model.Logo != null && model.Logo.Length > 0)
            {
                tenant.LogoUrl = await _fileUploadService.UploadImageAsync(model.Logo, "tenant-logos");
            }

            await _tenantRepository.UpdateAsync(tenant);
            return true;
        }

        public async Task<bool> DeleteTenantAsync(int id)
        {
            var tenant = await _tenantRepository.GetByIdAsync(id);
            if (tenant == null)
                return false;

            await _tenantRepository.DeleteAsync(tenant);
            return true;
        }
    }
}
