using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Application.Carts.Repositories;
using MultiTenantStore.Domain.Tenant;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Persistence.Repositories.Generic;

namespace MultiTenantStore.Persistence.Repositories.Tenant.Carts;

public sealed class CartItemRepository : TenantRepository<CartItem>, ICartItemRepository
{
    private readonly TenantDbContext _context;

    public CartItemRepository(TenantDbContext context)
        : base(context)
    {
        _context = context;
    }

    public Task<CartItem?> GetByCartAndProductAsync(
        Guid cartId,
        Guid productId,
        Guid? productVariantId,
        CancellationToken cancellationToken = default)
    {
        return _context.CartItems
            .FirstOrDefaultAsync(
                x => x.CartId == cartId &&
                     x.ProductId == productId &&
                     x.ProductVariantId == productVariantId &&
                     x.DeletedAt == null,
                cancellationToken);
    }

    public Task<CartItem?> GetDetailsAsync(
        Guid cartItemId,
        CancellationToken cancellationToken = default)
    {
        return _context.CartItems
            .Include(x => x.Product)
                .ThenInclude(x => x.Images)
            .Include(x => x.ProductVariant)
            .FirstOrDefaultAsync(
                x => x.Id == cartItemId &&
                     x.DeletedAt == null,
                cancellationToken);
    }

    public Task<List<CartItem>> GetByCartIdAsync(
        Guid cartId,
        CancellationToken cancellationToken = default)
    {
        return _context.CartItems
            .Include(x => x.Product)
                .ThenInclude(x => x.Images)
            .Include(x => x.ProductVariant)
            .Where(x => x.CartId == cartId &&
                        x.DeletedAt == null)
            .ToListAsync(cancellationToken);
    }
}