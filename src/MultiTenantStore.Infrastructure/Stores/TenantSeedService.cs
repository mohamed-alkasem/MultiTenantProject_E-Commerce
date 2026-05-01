using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Stores.Interfaces;
using MultiTenantStore.Domain.Tenant;
using MultiTenantStore.Persistence.Contexts;

namespace MultiTenantStore.Infrastructure.Stores;

public sealed class TenantSeedService : ITenantSeedService
{
    public async Task<ApiResponseDto<bool>> SeedAsync(
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

            var hasSettings = await context.StoreSettings
                .AnyAsync(cancellationToken);

            if (!hasSettings)
            {
                var settings = new StoreSetting
                {
                    Id = Guid.NewGuid(),
                    Currency = "USD",
                    Timezone = "UTC",
                    DefaultLanguage = "en",
                    IsCheckoutEnabled = true,
                    TaxEnabled = false,
                    OrderPrefix = "ORD",
                    CreatedAt = DateTime.UtcNow
                };

                await context.StoreSettings.AddAsync(settings, cancellationToken);
            }

            await context.SaveChangesAsync(cancellationToken);

            return ApiResponseDto<bool>.Ok(true, "Tenant database seeded successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponseDto<bool>.Fail(
                "Tenant database seeding failed.",
                new List<string> { ex.Message });
        }
    }
}