using MultiTenantStore.Application.Common.MultiTenancy;

namespace MultiTenantStore.Web.Middlewares;

/// <summary>
/// Resolves tenant for storefront routes (/s/{slug}/...) before the controller
/// is instantiated by DI, so TenantDbContext can be created successfully.
/// </summary>
public sealed class StorefrontTenantMiddleware
{
    private readonly RequestDelegate _next;

    public StorefrontTenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantStore tenantStore, ICurrentTenant currentTenant)
    {
        if (!currentTenant.IsResolved)
        {
            var segments = context.Request.Path.Value?
                .Split('/', StringSplitOptions.RemoveEmptyEntries);

            // /s/{slug}/...  →  segments[0]="s", segments[1]=slug
            if (segments?.Length >= 2
                && segments[0].Equals("s", StringComparison.OrdinalIgnoreCase))
            {
                var slug = segments[1];
                var tenant = await tenantStore.FindBySubdomainAsync(slug, context.RequestAborted);
                if (tenant?.IsActive == true)
                    currentTenant.SetTenant(tenant);
            }
        }

        await _next(context);
    }
}
