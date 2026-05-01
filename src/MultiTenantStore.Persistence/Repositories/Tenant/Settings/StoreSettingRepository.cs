using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Application.StoreSettings.Repositories;
using MultiTenantStore.Application.StoreSettings.Repositories;
using MultiTenantStore.Domain.Tenant;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Persistence.Repositories.Generic;

namespace MultiTenantStore.Persistence.Repositories.Tenant.Settings;

public sealed class StoreSettingRepository
    : TenantRepository<StoreSetting>, IStoreSettingRepository
{
    private readonly TenantDbContext _context;

    public StoreSettingRepository(TenantDbContext context)
        : base(context)
    {
        _context = context;
    }

    public Task<StoreSetting?> GetCurrentAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.StoreSettings
            .FirstOrDefaultAsync(
                x => x.DeletedAt == null,
                cancellationToken);
    }
}