using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Application.Common.MultiTenancy;
using MultiTenantStore.Persistence.Contexts;

namespace MultiTenantStore.Infrastructure.MultiTenancy;

public sealed class TenantStore : ITenantStore
{
    private readonly MainDbContext _context;

    public TenantStore(MainDbContext context)
    {
        _context = context;
    }

    public async Task<TenantInfo?> FindByStoreIdAsync(
        Guid storeId,
        CancellationToken cancellationToken = default)
    {
        var store = await _context.Stores
            .AsNoTracking()
            .Include(x => x.Database)
            .Include(x => x.Domains)
            .FirstOrDefaultAsync(x => x.Id == storeId, cancellationToken);

        if (store is null || store.Database is null)
        {
            return null;
        }

        var primaryDomain = store.Domains.FirstOrDefault(x => x.IsPrimary);

        return new TenantInfo
        {
            StoreId = store.Id,
            StoreName = store.StoreName,
            Slug = store.Slug,
            Subdomain = primaryDomain?.Subdomain,
            StoreStatus = store.Status.ToString(),
            SubscriptionStatus = store.SubscriptionStatus.ToString(),

            // مؤقتًا نستخدم القيمة كما هي.
            // لاحقًا إذا شفّرنا connection string نضيف EncryptionService.
            ConnectionString = store.Database.ConnectionStringEncrypted
        };
    }

    public async Task<TenantInfo?> FindBySubdomainAsync(
        string subdomain,
        CancellationToken cancellationToken = default)
    {
        var domain = await _context.StoreDomains
            .AsNoTracking()
            .Include(x => x.Store)
            .ThenInclude(x => x.Database)
            .FirstOrDefaultAsync(x => x.Subdomain == subdomain, cancellationToken);

        if (domain is null || domain.Store.Database is null)
        {
            return null;
        }

        return new TenantInfo
        {
            StoreId = domain.Store.Id,
            StoreName = domain.Store.StoreName,
            Slug = domain.Store.Slug,
            Subdomain = domain.Subdomain,
            StoreStatus = domain.Store.Status.ToString(),
            SubscriptionStatus = domain.Store.SubscriptionStatus.ToString(),

            // مؤقتًا نستخدم القيمة كما هي.
            ConnectionString = domain.Store.Database.ConnectionStringEncrypted
        };
    }
}