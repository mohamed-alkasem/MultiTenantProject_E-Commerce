using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
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

        // For ANY dashboard-related path (including excluded sub-paths such as
        // /Dashboard/DashboardAccount/Logout and /Dashboard/DashboardAccount/SetLanguage),
        // authenticate via cookie before the excluded-path short-circuit so that
        // context.User matches the identity that was used to generate antiforgery tokens.
        var isDashboardRelated =
            path.StartsWithSegments("/dashboard", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWithSegments("/api/dashboard", StringComparison.OrdinalIgnoreCase);

        if (isDashboardRelated && context.User.FindFirst(TenantClaimTypes.StoreId) is null)
        {
            var cookieAuth = await context.AuthenticateAsync(IdentityConstants.ApplicationScheme);
            if (cookieAuth.Succeeded && cookieAuth.Principal is not null)
                context.User = cookieAuth.Principal;
        }

        if (routeRules.IsExcludedPath(path))
        {
            await _next(context);
            return;
        }

        if (routeRules.IsDashboardPath(path))
        {
            var isMvcPath = !path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase);

            // Cookie auth already attempted above for all dashboard-related paths.

            var hasStoreClaim = context.User.FindFirst(TenantClaimTypes.StoreId) is not null;

            if (!hasStoreClaim)
            {
                if (isMvcPath)
                {
                    // Let [Authorize] handle the redirect to the login page
                    await _next(context);
                    return;
                }

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Tenant claims are missing or invalid.");
                return;
            }

            var resolved = await ResolveFromClaimsAsync(
                context,
                isMvcPath,
                claimsTenantResolver,
                tenantStore,
                tenantAccessValidator,
                currentTenant);

            if (!resolved)
            {
                if (isMvcPath && !context.Response.HasStarted)
                {
                    context.Response.Redirect(
                        $"/Dashboard/DashboardAccount/Login?returnUrl={Uri.EscapeDataString(context.Request.Path)}");
                }
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
        bool isMvcPath,
        IClaimsTenantResolver claimsTenantResolver,
        ITenantStore tenantStore,
        ITenantAccessValidator tenantAccessValidator,
        ICurrentTenant currentTenant)
    {
        var result = claimsTenantResolver.Resolve(context);

        if (!result.Success || result.StoreId is null || result.UserId is null)
        {
            if (!isMvcPath)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Tenant claims are missing or invalid.");
            }
            return false;
        }

        var canAccess = await tenantAccessValidator.CanAccessStoreAsync(
            result.UserId.Value,
            result.StoreId.Value,
            context.RequestAborted);

        if (!canAccess)
        {
            if (!isMvcPath)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("You do not have access to this store.");
            }
            return false;
        }

        var tenant = await tenantStore.FindByStoreIdAsync(
            result.StoreId.Value,
            context.RequestAborted);

        if (tenant is null)
        {
            if (!isMvcPath)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync("Store was not found.");
            }
            return false;
        }

        if (!tenant.IsActive)
        {
            if (!isMvcPath)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Store is not active.");
            }
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