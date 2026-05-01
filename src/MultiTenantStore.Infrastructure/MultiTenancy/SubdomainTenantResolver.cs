using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MultiTenantStore.Application.Common.MultiTenancy;

namespace MultiTenantStore.Infrastructure.MultiTenancy;

public sealed class SubdomainTenantResolver : ISubdomainTenantResolver
{
    private readonly TenantResolutionOptions _options;

    public SubdomainTenantResolver(IOptions<TenantResolutionOptions> options)
    {
        _options = options.Value;
    }

    public TenantResolutionResult Resolve(HttpContext context)
    {
        var host = context.Request.Host.Host;

        if (string.IsNullOrWhiteSpace(host))
        {
            return TenantResolutionResult.Failed("Subdomain", "Host is empty.");
        }

        if (host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
        {
            return TenantResolutionResult.Failed("Subdomain", "Localhost has no subdomain.");
        }

        var baseDomain = _options.BaseDomain;

        if (string.IsNullOrWhiteSpace(baseDomain))
        {
            return TenantResolutionResult.Failed("Subdomain", "Base domain is not configured.");
        }

        if (!host.EndsWith(baseDomain, StringComparison.OrdinalIgnoreCase))
        {
            return TenantResolutionResult.Failed("Subdomain", "Host does not match base domain.");
        }

        var subdomain = host
            .Replace($".{baseDomain}", "", StringComparison.OrdinalIgnoreCase)
            .Trim();

        if (string.IsNullOrWhiteSpace(subdomain) ||
            subdomain.Equals("www", StringComparison.OrdinalIgnoreCase))
        {
            return TenantResolutionResult.Failed("Subdomain", "Subdomain was not found.");
        }

        return TenantResolutionResult.FromSubdomain(subdomain);
    }
}