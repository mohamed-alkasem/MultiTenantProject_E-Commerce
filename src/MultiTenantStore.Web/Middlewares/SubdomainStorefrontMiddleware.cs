using MultiTenantStore.Infrastructure.MultiTenancy;
using Microsoft.Extensions.Options;

namespace MultiTenantStore.Web.Middlewares;

public sealed class SubdomainStorefrontMiddleware
{
    private readonly RequestDelegate _next;

    public SubdomainStorefrontMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IOptions<TenantResolutionOptions> options)
    {
        var baseDomain = options.Value.BaseDomain;
        var host = context.Request.Host.Host;

        if (!string.IsNullOrWhiteSpace(host)
            && !string.IsNullOrWhiteSpace(baseDomain)
            && !host.Equals(baseDomain, StringComparison.OrdinalIgnoreCase)
            && host.EndsWith($".{baseDomain}", StringComparison.OrdinalIgnoreCase))
        {
            var subdomain = host
                .Replace($".{baseDomain}", "", StringComparison.OrdinalIgnoreCase)
                .Trim();

            if (!string.IsNullOrWhiteSpace(subdomain)
                && !subdomain.Equals("www", StringComparison.OrdinalIgnoreCase))
            {
                var path = context.Request.Path.Value ?? "/";

                // Avoid re-writing already-rewritten or excluded paths
                if (!path.StartsWith("/s/", StringComparison.OrdinalIgnoreCase)
                    && !path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase)
                    && !path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase)
                    && !path.StartsWith("/css/", StringComparison.OrdinalIgnoreCase)
                    && !path.StartsWith("/js/", StringComparison.OrdinalIgnoreCase)
                    && !path.StartsWith("/images/", StringComparison.OrdinalIgnoreCase)
                    && !path.StartsWith("/lib/", StringComparison.OrdinalIgnoreCase))
                {
                    // /  → /s/ahmad/
                    // /Products → /s/ahmad/Products
                    // /Cart/Add → /s/ahmad/Cart/Add
                    var newPath = path == "/" ? $"/s/{subdomain}" : $"/s/{subdomain}{path}";
                    context.Request.Path = newPath;
                }
            }
        }

        await _next(context);
    }
}
