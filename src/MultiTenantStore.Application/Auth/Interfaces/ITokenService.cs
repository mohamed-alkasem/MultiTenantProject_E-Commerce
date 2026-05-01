using MultiTenantStore.Application.Auth.DTOs;
using MultiTenantStore.Domain.Platform;

namespace MultiTenantStore.Application.Auth.Interfaces;

public interface ITokenService
{
    AuthResponseDto GenerateToken(
        ApplicationUser user,
        IEnumerable<string> roles,
        Guid? storeId = null,
        string? storeSlug = null,
        string? storeRole = null);
}