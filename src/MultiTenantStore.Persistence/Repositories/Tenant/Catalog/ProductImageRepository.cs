using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Application.Catalog.Repositories;
using MultiTenantStore.Domain.Tenant;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Persistence.Repositories.Generic;
using MultiTenantStore.Application.Catalog.Services;
namespace MultiTenantStore.Persistence.Repositories.Tenant.Catalog;

public sealed class ProductImageRepository : TenantRepository<ProductImage>, IProductImageRepository
{
    private readonly TenantDbContext _context;

    public ProductImageRepository(TenantDbContext context)
        : base(context)
    {
        _context = context;
    }

    public Task<List<ProductImage>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return _context.ProductImages
            .Where(x => x.ProductId == productId && x.DeletedAt == null)
            .OrderByDescending(x => x.IsPrimary)
            .ThenBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public Task<ProductImage?> GetPrimaryImageAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return _context.ProductImages
            .Where(x => x.ProductId == productId && x.DeletedAt == null)
            .OrderByDescending(x => x.IsPrimary)
            .ThenBy(x => x.SortOrder)
            .FirstOrDefaultAsync(cancellationToken);
    }
}