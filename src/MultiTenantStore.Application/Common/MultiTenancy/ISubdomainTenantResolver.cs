using Microsoft.AspNetCore.Http;

namespace MultiTenantStore.Application.Common.MultiTenancy;

public interface ISubdomainTenantResolver
{
    TenantResolutionResult Resolve(HttpContext context);
}