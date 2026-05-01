using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace MultiTenantStore.Application.Common.MultiTenancy;

public interface IClaimsTenantResolver
{
    TenantResolutionResult Resolve(HttpContext context);
}