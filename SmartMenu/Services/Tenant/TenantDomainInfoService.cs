using SmartMenu.Repositories.Tenant;

namespace SmartMenu.Services.Tenant
{
    public class TenantDomainInfoService : ITenantDomainInfoService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<TenantDomainInfoService> _logger;
        private readonly object _syncRoot = new();
        private readonly Dictionary<int, TenantDomainInfo> _tenantLookup = new();
        private readonly Dictionary<string, int> _domainLookup = new(StringComparer.OrdinalIgnoreCase);

        public TenantDomainInfoService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<TenantDomainInfoService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var tenantRepository = scope.ServiceProvider.GetRequiredService<ITenantRepository>();
            var tenants = await tenantRepository.GetAllAsync();

            lock (_syncRoot)
            {
                _tenantLookup.Clear();
                _domainLookup.Clear();

                foreach (var tenant in tenants)
                {
                    UpsertInternal(tenant);
                }
            }

            _logger.LogInformation("TenantDomainInfoService initialized with {Count} tenant domain mappings.", _domainLookup.Count);
        }

        public bool TryGetByDomain(string host, out TenantDomainInfo tenantDomainInfo)
        {
            tenantDomainInfo = new TenantDomainInfo();
            var normalizedHost = NormalizeDomain(host);

            if (string.IsNullOrWhiteSpace(normalizedHost))
            {
                return false;
            }

            lock (_syncRoot)
            {
                if (_domainLookup.TryGetValue(normalizedHost, out var tenantId) &&
                    _tenantLookup.TryGetValue(tenantId, out var cachedInfo))
                {
                    tenantDomainInfo = cachedInfo;
                    return true;
                }

                return false;
            }
        }

        public void UpsertTenant(Data.Entities.Tenant tenant)
        {
            lock (_syncRoot)
            {
                UpsertInternal(tenant);
            }
        }

        public void RemoveTenant(int tenantId)
        {
            lock (_syncRoot)
            {
                if (!_tenantLookup.TryGetValue(tenantId, out var existing))
                {
                    return;
                }

                _tenantLookup.Remove(tenantId);

                var oldDomain = NormalizeDomain(existing.DomainName);
                if (!string.IsNullOrWhiteSpace(oldDomain))
                {
                    _domainLookup.Remove(oldDomain);
                }
            }
        }

        private void UpsertInternal(Data.Entities.Tenant tenant)
        {
            if (_tenantLookup.TryGetValue(tenant.Id, out var previous))
            {
                var previousDomain = NormalizeDomain(previous.DomainName);
                if (!string.IsNullOrWhiteSpace(previousDomain))
                {
                    _domainLookup.Remove(previousDomain);
                }
            }

            var domain = NormalizeDomain(tenant.DomainName);
            var info = new TenantDomainInfo
            {
                TenantId = tenant.Id,
                TenantName = tenant.Name,
                DomainName = domain ?? string.Empty,
                LandingPageUrl = NormalizeOptional(tenant.LandingPageUrl)
            };

            _tenantLookup[tenant.Id] = info;

            if (!string.IsNullOrWhiteSpace(domain))
            {
                _domainLookup[domain] = tenant.Id;
            }
        }

        private static string? NormalizeDomain(string? domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
            {
                return null;
            }

            var normalized = domain.Trim().TrimEnd('.').ToLowerInvariant();
            if (normalized.StartsWith("www.", StringComparison.Ordinal))
            {
                normalized = normalized[4..];
            }

            return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
        }

        private static string? NormalizeOptional(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
