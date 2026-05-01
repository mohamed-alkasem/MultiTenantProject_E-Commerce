using MultiTenantStore.Application.Common.MultiTenancy;
using MultiTenantStore.Infrastructure.MultiTenancy;

namespace MultiTenantStore.Web.Middlewares;

public sealed class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        TenantRouteRules routeRules,
        IClaimsTenantResolver claimsTenantResolver,
        ISubdomainTenantResolver subdomainTenantResolver,
        ITenantStore tenantStore,
        ITenantAccessValidator tenantAccessValidator,
        ICurrentTenant currentTenant)
    {
        var path = context.Request.Path;

        if (routeRules.IsExcludedPath(path))
        {
            await _next(context);
            return;
        }

        if (routeRules.IsDashboardPath(path))
        {
            var resolved = await ResolveFromClaimsAsync(
                context,
                claimsTenantResolver,
                tenantStore,
                tenantAccessValidator,
                currentTenant);

            if (!resolved)
            {
                return;
            }

            await _next(context);
            return;
        }

        if (routeRules.IsPublicTenantPath(path))
        {
            var resolved = await ResolveFromSubdomainAsync(
                context,
                subdomainTenantResolver,
                tenantStore,
                currentTenant);

            if (!resolved)
            {
                return;
            }

            await _next(context);
            return;
        }

        await _next(context);
    }

    private static async Task<bool> ResolveFromClaimsAsync(
        HttpContext context,
        IClaimsTenantResolver claimsTenantResolver,
        ITenantStore tenantStore,
        ITenantAccessValidator tenantAccessValidator,
        ICurrentTenant currentTenant)
    {
        var result = claimsTenantResolver.Resolve(context);

        if (!result.Success || result.StoreId is null || result.UserId is null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Tenant claims are missing or invalid.");
            return false;
        }

        var canAccess = await tenantAccessValidator.CanAccessStoreAsync(
            result.UserId.Value,
            result.StoreId.Value,
            context.RequestAborted);

        if (!canAccess)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("You do not have access to this store.");
            return false;
        }

        var tenant = await tenantStore.FindByStoreIdAsync(
            result.StoreId.Value,
            context.RequestAborted);

        if (tenant is null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync("Store was not found.");
            return false;
        }

        if (!tenant.IsActive)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Store is not active.");
            return false;
        }

        currentTenant.SetTenant(tenant);
        currentTenant.SetStoreRole(result.StoreRole);

        return true;
    }

    private static async Task<bool> ResolveFromSubdomainAsync(
        HttpContext context,
        ISubdomainTenantResolver subdomainTenantResolver,
        ITenantStore tenantStore,
        ICurrentTenant currentTenant)
    {
        var result = subdomainTenantResolver.Resolve(context);

        if (!result.Success || string.IsNullOrWhiteSpace(result.TenantKey))
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync("Store was not found.");
            return false;
        }

        var tenant = await tenantStore.FindBySubdomainAsync(
            result.TenantKey,
            context.RequestAborted);

        if (tenant is null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync("Store was not found.");
            return false;
        }

        if (!tenant.IsActive)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Store is not active.");
            return false;
        }

        currentTenant.SetTenant(tenant);

        return true;
    }
}