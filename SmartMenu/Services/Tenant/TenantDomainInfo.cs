namespace SmartMenu.Services.Tenant
{
    public class TenantDomainInfo
    {
        public int TenantId { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public string DomainName { get; set; } = string.Empty;
        public string? LandingPageUrl { get; set; }
    }
}
