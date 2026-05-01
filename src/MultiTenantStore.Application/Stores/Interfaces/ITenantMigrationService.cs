using MultiTenantStore.Application.Common.DTOs;

namespace MultiTenantStore.Application.Stores.Interfaces;

public interface ITenantMigrationService
{
    Task<ApiResponseDto<bool>> MigrateAsync(
        string connectionString,
        CancellationToken cancellationToken = default);
}