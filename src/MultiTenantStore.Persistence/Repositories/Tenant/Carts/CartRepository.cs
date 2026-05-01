using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Application.Carts.Repositories;
using MultiTenantStore.Domain.Enums;
using MultiTenantStore.Domain.Tenant;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Persistence.Repositories.Generic;

namespace MultiTenantStore.Persistence.Repositories.Tenant.Carts;

public sealed class CartRepository : TenantRepository<Cart>, ICartRepository
{
    private readonly TenantDbContext _context;

    public CartRepository(TenantDbContext context)
        : base(context)
    {
        _context = context;
    }

    public Task<Cart?> GetActiveBySessionIdAsync(
        string sessionId,
        CancellationToken cancellationToken = default)
    {
        return _context.Carts
            .FirstOrDefaultAsync(
                x => x.SessionId == sessionId &&
                     x.Status == CartStatus.Active &&
                     x.DeletedAt == null,
                cancellationToken);
    }

    public Task<Cart?> GetDetailsAsync(
        Guid cartId,
        CancellationToken cancellationToken = default)
    {
        return _context.Carts
            .Include(x => x.Items.Where(i => i.DeletedAt == null))
                .ThenInclude(x => x.Product)
                    .ThenInclude(x => x.Images)
            .Include(x => x.Items.Where(i => i.DeletedAt == null))
                .ThenInclude(x => x.ProductVariant)
            .FirstOrDefaultAsync(
                x => x.Id == cartId &&
                     x.DeletedAt == null,
                cancellationToken);
    }

    public Task<Cart?> GetDetailsBySessionIdAsync(
        string sessionId,
        CancellationToken cancellationToken = default)
    {
        return _context.Carts
            .Include(x => x.Items.Where(i => i.DeletedAt == null))
                .ThenInclude(x => x.Product)
                    .ThenInclude(x => x.Images)
            .Include(x => x.Items.Where(i => i.DeletedAt == null))
                .ThenInclude(x => x.ProductVariant)
            .FirstOrDefaultAsync(
                x => x.SessionId == sessionId &&
                     x.Status == CartStatus.Active &&
                     x.DeletedAt == null,
                cancellationToken);
    }
}