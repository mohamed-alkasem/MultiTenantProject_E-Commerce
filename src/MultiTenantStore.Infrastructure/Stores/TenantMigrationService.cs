using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Stores.Interfaces;
using MultiTenantStore.Persistence.Contexts;

namespace MultiTenantStore.Infrastructure.Stores;

public sealed class TenantMigrationService : ITenantMigrationService
{
    public async Task<ApiResponseDto<bool>> MigrateAsync(
        string connectionString,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return ApiResponseDto<bool>.Fail("Tenant connection string is empty.");
        }

        try
        {
            var options = new DbContextOptionsBuilder<TenantDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            await using var context = new TenantDbContext(options);

            await context.Database.MigrateAsync(cancellationToken);

            return ApiResponseDto<bool>.Ok(true, "Tenant database migrated successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<bool>.Fail(
                "Tenant database migration failed.",
                new List<string> { ex.Message });
        }
    }
}