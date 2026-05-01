using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Application.Common.MultiTenancy;
using MultiTenantStore.Persistence.Contexts;

namespace MultiTenantStore.Infrastructure.MultiTenancy;

public sealed class TenantAccessValidator : ITenantAccessValidator
{
    private readonly MainDbContext _context;

    public TenantAccessValidator(MainDbContext context)
    {
        _context = context;
    }

    public Task<bool> CanAccessStoreAsync(
        Guid userId,
        Guid storeId,
        CancellationToken cancellationToken = default)
    {
        return _context.StoreUsers
            .AsNoTracking()
            .AnyAsync(
                x => x.UserId == userId &&
                     x.StoreId == storeId &&
                     x.IsActive &&
                     x.DeletedAt == null,
                cancellationToken);
    }

    public Task<bool> HasStoreRoleAsync(
        Guid userId,
        Guid storeId,
        string role,
        CancellationToken cancellationToken = default)
    {
        return _context.StoreUsers
            .AsNoTracking()
            .AnyAsync(
                x => x.UserId == userId &&
                     x.StoreId == storeId &&
                     x.IsActive &&
                     x.Role.ToString() == role &&
                     x.DeletedAt == null,
                cancellationToken);
    }
}