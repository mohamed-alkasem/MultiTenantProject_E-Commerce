using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Application.Catalog.Repositories;
using MultiTenantStore.Domain.Tenant;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Persistence.Repositories.Generic;

namespace MultiTenantStore.Persistence.Repositories.Tenant.Catalog;

public sealed class CategoryRepository : TenantRepository<Category>, ICategoryRepository
{
    private readonly TenantDbContext _context;

    public CategoryRepository(TenantDbContext context)
        : base(context)
    {
        _context = context;
    }

    public Task<List<Category>> GetActiveAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.Categories
            .Where(x => x.DeletedAt == null && x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Category>> GetAllNotDeletedAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.Categories
            .Where(x => x.DeletedAt == null)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Category?> GetNotDeletedByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.Categories
            .FirstOrDefaultAsync(
                x => x.Id == id && x.DeletedAt == null,
                cancellationToken);
    }

    public Task<bool> ExistsBySlugAsync(
        string slug,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default)
    {
        return _context.Categories.AnyAsync(
            x => x.Slug == slug &&
                 x.DeletedAt == null &&
                 (excludedId == null || x.Id != excludedId.Value),
            cancellationToken);
    }
}