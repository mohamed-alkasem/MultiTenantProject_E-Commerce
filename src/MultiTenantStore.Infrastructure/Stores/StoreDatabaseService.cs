using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Stores.DTOs;
using MultiTenantStore.Application.Stores.Interfaces;
using MultiTenantStore.Domain.Enums;
using MultiTenantStore.Persistence.Contexts;

namespace MultiTenantStore.Infrastructure.Stores;

public sealed class StoreDatabaseService : IStoreDatabaseService
{
    private readonly MainDbContext _mainDbContext;
    private readonly ITenantMigrationService _tenantMigrationService;
    private readonly ITenantSeedService _tenantSeedService;

    public StoreDatabaseService(
        MainDbContext mainDbContext,
        ITenantMigrationService tenantMigrationService,
        ITenantSeedService tenantSeedService)
    {
        _mainDbContext = mainDbContext;
        _tenantMigrationService = tenantMigrationService;
        _tenantSeedService = tenantSeedService;
    }

    public async Task<ApiResponseDto<StoreDatabaseDto>> ProvisionDatabaseAsync(
        Guid storeId,
        CancellationToken cancellationToken = default)
    {
        var store = await _mainDbContext.Stores
            .Include(x => x.Database)
            .FirstOrDefaultAsync(x => x.Id == storeId, cancellationToken);

        if (store is null)
        {
            return ApiResponseDto<StoreDatabaseDto>.Fail("Store was not found.");
        }

        if (store.Database is null)
        {
            return ApiResponseDto<StoreDatabaseDto>.Fail("Store database record was not found.");
        }

        var storeDatabase = store.Database;

        if (storeDatabase.IsProvisioned &&
            storeDatabase.ProvisioningStatus == ProvisioningStatus.Completed)
        {
            return ApiResponseDto<StoreDatabaseDto>.Ok(
                MapToDto(storeDatabase),
                "Tenant database is already provisioned.");
        }

        try
        {
            storeDatabase.ProvisioningStatus = ProvisioningStatus.InProgress;
            storeDatabase.LastError = null;
            storeDatabase.UpdatedAt = DateTime.UtcNow;

            await _mainDbContext.SaveChangesAsync(cancellationToken);

            var connectionString = storeDatabase.ConnectionStringEncrypted;

            var migrationResult = await _tenantMigrationService.MigrateAsync(
                connectionString,
                cancellationToken);

            if (!migrationResult.Success)
            {
                await MarkFailedAsync(
                    storeDatabase,
                    store,
                    migrationResult.Message,
                    migrationResult.Errors,
                    cancellationToken);

                return ApiResponseDto<StoreDatabaseDto>.Fail(
                    migrationResult.Message ?? "Tenant migration failed.",
                    migrationResult.Errors);
            }

            storeDatabase.ProvisioningStatus = ProvisioningStatus.Migrated;
            storeDatabase.LastMigrationAt = DateTime.UtcNow;
            storeDatabase.UpdatedAt = DateTime.UtcNow;

            await _mainDbContext.SaveChangesAsync(cancellationToken);

            var seedResult = await _tenantSeedService.SeedAsync(
                connectionString,
                cancellationToken);

            if (!seedResult.Success)
            {
                await MarkFailedAsync(
                    storeDatabase,
                    store,
                    seedResult.Message,
                    seedResult.Errors,
                    cancellationToken);

                return ApiResponseDto<StoreDatabaseDto>.Fail(
                    seedResult.Message ?? "Tenant seed failed.",
                    seedResult.Errors);
            }

            storeDatabase.ProvisioningStatus = ProvisioningStatus.Completed;
            storeDatabase.IsProvisioned = true;
            storeDatabase.ProvisionedAt = DateTime.UtcNow;
            storeDatabase.UpdatedAt = DateTime.UtcNow;
            storeDatabase.LastError = null;

            store.Status = StoreStatus.Active;
            store.ActivatedAt ??= DateTime.UtcNow;
            store.UpdatedAt = DateTime.UtcNow;

            await _mainDbContext.SaveChangesAsync(cancellationToken);

            return ApiResponseDto<StoreDatabaseDto>.Ok(
                MapToDto(storeDatabase),
                "Tenant database provisioned successfully.");
        }
        catch (Exception ex)
        {
            await MarkFailedAsync(
                storeDatabase,
                store,
                "Tenant database provisioning failed.",
                new List<string> { ex.Message },
                cancellationToken);

            return ApiResponseDto<StoreDatabaseDto>.Fail(
                "Tenant database provisioning failed.",
                new List<string> { ex.Message });
        }
    }

    private async Task MarkFailedAsync(
        Domain.Platform.StoreDatabase storeDatabase,
        Domain.Platform.Store store,
        string? message,
        List<string> errors,
        CancellationToken cancellationToken)
    {
        storeDatabase.ProvisioningStatus = ProvisioningStatus.Failed;
        storeDatabase.IsProvisioned = false;
        storeDatabase.LastError = BuildErrorMessage(message, errors);
        storeDatabase.UpdatedAt = DateTime.UtcNow;

        store.Status = StoreStatus.ProvisioningFailed;
        store.UpdatedAt = DateTime.UtcNow;

        await _mainDbContext.SaveChangesAsync(cancellationToken);
    }

    private static string BuildErrorMessage(string? message, List<string> errors)
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(message))
        {
            parts.Add(message);
        }

        if (errors.Count > 0)
        {
            parts.AddRange(errors);
        }

        return string.Join(" | ", parts);
    }

    private static StoreDatabaseDto MapToDto(Domain.Platform.StoreDatabase database)
    {
        return new StoreDatabaseDto
        {
            Id = database.Id,
            StoreId = database.StoreId,
            DatabaseName = database.DatabaseName,
            DbServer = database.DbServer,
            Provider = database.Provider,
            ProvisioningStatus = database.ProvisioningStatus.ToString(),
            IsProvisioned = database.IsProvisioned,
            ProvisionedAt = database.ProvisionedAt,
            MigrationVersion = database.MigrationVersion,
            LastMigrationAt = database.LastMigrationAt,
            LastError = database.LastError
        };
    }
}