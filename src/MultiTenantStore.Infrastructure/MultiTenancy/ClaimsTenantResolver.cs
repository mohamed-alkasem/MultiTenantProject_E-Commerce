using Microsoft.AspNetCore.Http;
using MultiTenantStore.Application.Common.MultiTenancy;

namespace MultiTenantStore.Infrastructure.MultiTenancy;

public sealed class ClaimsTenantResolver : IClaimsTenantResolver
{
    public TenantResolutionResult Resolve(HttpContext context)
    {
        var storeIdValue = context.User.FindFirst(TenantClaimTypes.StoreId)?.Value;
        var userIdValue = context.User.FindFirst(TenantClaimTypes.UserId)?.Value;
        var storeRole = context.User.FindFirst(TenantClaimTypes.StoreRole)?.Value;

        if (string.IsNullOrWhiteSpace(storeIdValue))
        {
            return TenantResolutionResult.Failed("Claims", "store_id claim was not found.");
        }

        if (string.IsNullOrWhiteSpace(userIdValue))
        {
            return TenantResolutionResult.Failed("Claims", "user_id claim was not found.");
        }

        if (!Guid.TryParse(storeIdValue, out var storeId))
        {
            return TenantResolutionResult.Failed("Claims", "store_id claim is invalid.");
        }

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return TenantResolutionResult.Failed("Claims", "user_id claim is invalid.");
        }

        return TenantResolutionResult.FromClaims(storeId, userId, storeRole);
    }
}