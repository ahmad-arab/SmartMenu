using SmartMenu.Services.Tenant;

namespace SmartMenu.Middlewares
{
    public class TenantLandingPageRouter
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantLandingPageRouter> _logger;

        public TenantLandingPageRouter(RequestDelegate next, ILogger<TenantLandingPageRouter> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ITenantDomainInfoService tenantDomainInfoService)
        {
            var host = context.Request.Host.Host;
            var path = context.Request.Path.Value ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(host) && IsRootPath(path) && tenantDomainInfoService.TryGetByDomain(host, out var tenantInfo))
            {
                var routedPath = ResolveLandingPath(tenantInfo);

                if (!string.IsNullOrWhiteSpace(routedPath))
                {
                    if (Uri.TryCreate(routedPath, UriKind.RelativeOrAbsolute, out var routeUri))
                    {
                        if (routeUri.IsAbsoluteUri)
                        {
                            context.Request.Path = routeUri.AbsolutePath;
                            context.Request.QueryString = new QueryString(routeUri.Query);
                        }
                        else
                        {
                            context.Request.Path = routedPath;
                            context.Request.QueryString = QueryString.Empty;
                        }
                    }
                    else
                    {
                        context.Request.Path = routedPath;
                        context.Request.QueryString = QueryString.Empty;
                    }

                    _logger.LogDebug(
                        "Tenant landing page route applied for host {Host}. Rewritten path to {Path}.",
                        host,
                        context.Request.Path);
                }
            }

            await _next(context);
        }

        private static bool IsRootPath(string path)
        {
            return string.IsNullOrWhiteSpace(path) || path == "/";
        }

        private static string ResolveLandingPath(TenantDomainInfo tenantInfo)
        {
            var configuredPath = tenantInfo.LandingPageUrl;
            if (string.IsNullOrWhiteSpace(configuredPath))
            {
                configuredPath = $"landing/{tenantInfo.TenantName.ToLowerInvariant()}/index.html";
            }

            configuredPath = configuredPath.Trim();
            if (configuredPath.Contains("..", StringComparison.Ordinal))
            {
                return string.Empty;
            }

            if (!configuredPath.StartsWith('/'))
            {
                configuredPath = "/" + configuredPath;
            }

            return configuredPath;
        }
    }
}
