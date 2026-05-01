using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Application.Catalog.Repositories;
using MultiTenantStore.Domain.Tenant;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Persistence.Repositories.Generic;

namespace MultiTenantStore.Persistence.Repositories.Tenant.Catalog;

public sealed class ProductRepository : TenantRepository<Product>, IProductRepository
{
    private readonly TenantDbContext _context;

    public ProductRepository(TenantDbContext context)
        : base(context)
    {
        _context = context;
    }

    public Task<Product?> GetDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.Products
            .Include(x => x.Category)
            .Include(x => x.Variants)
            .Include(x => x.Images)
            .FirstOrDefaultAsync(
                x => x.Id == id && x.DeletedAt == null,
                cancellationToken);
    }

    public Task<List<Product>> GetAllWithImagesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.Products
            .Include(x => x.Images)
            .Where(x => x.DeletedAt == null)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Product?> GetActiveDetailsBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        return _context.Products
            .Include(x => x.Category)
            .Include(x => x.Variants)
            .Include(x => x.Images)
            .FirstOrDefaultAsync(
                x => x.Slug == slug &&
                     x.IsActive &&
                     x.DeletedAt == null,
                cancellationToken);
    }

    public async Task<(List<Product> Items, int TotalCount)> SearchActiveAsync(
        string? search,
        Guid? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var query = _context.Products
            .Include(x => x.Images)
            .Where(x => x.IsActive && x.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim();

            query = query.Where(x =>
                x.Name.Contains(normalizedSearch) ||
                x.Slug.Contains(normalizedSearch) ||
                x.SKU.Contains(normalizedSearch));
        }

        if (categoryId is not null)
        {
            query = query.Where(x => x.CategoryId == categoryId.Value);
        }

        if (minPrice is not null)
        {
            query = query.Where(x => x.Price >= minPrice.Value);
        }

        if (maxPrice is not null)
        {
            query = query.Where(x => x.Price <= maxPrice.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<bool> ExistsBySlugAsync(
        string slug,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default)
    {
        return _context.Products.AnyAsync(
            x => x.Slug == slug &&
                 x.DeletedAt == null &&
                 (excludedId == null || x.Id != excludedId.Value),
            cancellationToken);
    }

    public Task<bool> ExistsBySkuAsync(
        string sku,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default)
    {
        return _context.Products.AnyAsync(
            x => x.SKU == sku &&
                 x.DeletedAt == null &&
                 (excludedId == null || x.Id != excludedId.Value),
            cancellationToken);
    }
}