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
        private readonly ITenantDomainInfoService _tenantDomainInfoService;

        public TenantService(
            ITenantRepository tenantRepository,
            IFileUploadService fileUploadService,
            ITenantDomainInfoService tenantDomainInfoService)
        {
            _tenantRepository = tenantRepository;
            _fileUploadService = fileUploadService;
            _tenantDomainInfoService = tenantDomainInfoService;
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
                LogoUrl = tenant.LogoUrl,
                AllowedMenusCount = tenant.AllowedMenusCount,
                UseCommands = tenant.UseCommands,
                DomainName = tenant.DomainName,
                LandingPageUrl = tenant.LandingPageUrl
            };
        }

        public async Task<TenantModel?> GetTenantAsync(int id)
        {
            var tenant = await _tenantRepository.GetByIdNoTrackingAsync(id);
            if (tenant == null)
                return null;

            return new TenantModel
            {
                Id = tenant.Id,
                Name = tenant.Name,
                LogoUrl = tenant.LogoUrl,
                AllowedMenusCount = tenant.AllowedMenusCount,
                UseCommands = tenant.UseCommands,
                DomainName = tenant.DomainName,
                LandingPageUrl = tenant.LandingPageUrl
            };
        }

        public async Task CreateTenantAsync(CreateTenantViewModel model)
        {
            var logoUrl = await _fileUploadService.UploadImageAsync(model.Logo, "tenant-logos");

            var tenant = new Data.Entities.Tenant
            {
                Name = model.Name,
                LogoUrl = logoUrl,
                AllowedMenusCount = model.AllowedMenusCount,
                UseCommands = model.UseCommands,
                DomainName = NormalizeOptional(model.DomainName),
                LandingPageUrl = NormalizeOptional(model.LandingPageUrl)
            };

            await _tenantRepository.AddAsync(tenant);
            _tenantDomainInfoService.UpsertTenant(tenant);
        }

        public async Task<bool> UpdateTenantAsync(int id, EditTenantViewModel model)
        {
            var tenant = await _tenantRepository.GetByIdAsync(id);
            if (tenant == null)
                return false;

            tenant.Name = model.Name;
            tenant.AllowedMenusCount = model.AllowedMenusCount;
            tenant.UseCommands = model.UseCommands;
            tenant.DomainName = NormalizeOptional(model.DomainName);
            tenant.LandingPageUrl = NormalizeOptional(model.LandingPageUrl);

            if (model.Logo != null && model.Logo.Length > 0)
            {
                tenant.LogoUrl = await _fileUploadService.UploadImageAsync(model.Logo, "tenant-logos");
            }

            await _tenantRepository.UpdateAsync(tenant);
            _tenantDomainInfoService.UpsertTenant(tenant);
            return true;
        }

        public async Task<bool> DeleteTenantAsync(int id)
        {
            var tenant = await _tenantRepository.GetByIdAsync(id);
            if (tenant == null)
                return false;

            await _tenantRepository.DeleteAsync(tenant);
            _tenantDomainInfoService.RemoveTenant(tenant.Id);
            return true;
        }

        private static string? NormalizeOptional(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
