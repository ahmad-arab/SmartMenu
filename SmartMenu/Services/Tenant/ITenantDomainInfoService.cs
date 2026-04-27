namespace SmartMenu.Services.Tenant
{
    public interface ITenantDomainInfoService
    {
        Task InitializeAsync();
        bool TryGetByDomain(string host, out TenantDomainInfo tenantDomainInfo);
        void UpsertTenant(Data.Entities.Tenant tenant);
        void RemoveTenant(int tenantId);
    }
}
