using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Application.Catalog.Repositories;
using MultiTenantStore.Domain.Tenant;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Persistence.Repositories.Generic;

namespace MultiTenantStore.Persistence.Repositories.Tenant.Catalog;

public sealed class ProductVariantRepository : TenantRepository<ProductVariant>, IProductVariantRepository
{
    private readonly TenantDbContext _context;

    public ProductVariantRepository(TenantDbContext context)
        : base(context)
    {
        _context = context;
    }

    public Task<bool> ExistsBySkuAsync(
        string sku,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default)
    {
        return _context.ProductVariants.AnyAsync(
            x => x.SKU == sku &&
                 x.DeletedAt == null &&
                 (excludedId == null || x.Id != excludedId.Value),
            cancellationToken);
    }

    public Task<List<ProductVariant>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return _context.ProductVariants
            .Where(x => x.ProductId == productId && x.DeletedAt == null)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}