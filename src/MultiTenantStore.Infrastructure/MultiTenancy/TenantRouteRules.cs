using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace MultiTenantStore.Infrastructure.MultiTenancy;

public sealed class TenantRouteRules
{
    private readonly TenantResolutionOptions _options;

    public TenantRouteRules(IOptions<TenantResolutionOptions> options)
    {
        _options = options.Value;
    }

    public bool IsExcludedPath(PathString path)
    {
        return _options.ExcludedPaths.Any(x =>
            path.StartsWithSegments(x, StringComparison.OrdinalIgnoreCase));
    }

    public bool IsDashboardPath(PathString path)
    {
        return _options.DashboardPaths.Any(x =>
            path.StartsWithSegments(x, StringComparison.OrdinalIgnoreCase));
    }

    public bool IsPublicTenantPath(PathString path)
    {
        return _options.PublicTenantPaths.Any(x =>
            path.StartsWithSegments(x, StringComparison.OrdinalIgnoreCase));
    }
}