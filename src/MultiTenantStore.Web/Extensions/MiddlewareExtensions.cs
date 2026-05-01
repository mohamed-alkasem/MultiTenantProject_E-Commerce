using MultiTenantStore.Web.Middlewares;

namespace MultiTenantStore.Web.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder app)
    {
        return app.UseMiddleware<TenantResolutionMiddleware>();
    }
}