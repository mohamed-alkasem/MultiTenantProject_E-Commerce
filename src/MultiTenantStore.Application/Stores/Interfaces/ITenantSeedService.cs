using MultiTenantStore.Application.Common.DTOs;

namespace MultiTenantStore.Application.Stores.Interfaces;

public interface ITenantSeedService
{
    Task<ApiResponseDto<bool>> SeedAsync(
        string connectionString,
        CancellationToken cancellationToken = default);
}