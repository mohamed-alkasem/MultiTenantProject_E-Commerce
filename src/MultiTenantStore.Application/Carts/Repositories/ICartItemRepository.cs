using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Carts.Repositories;

public interface ICartItemRepository : ITenantRepository<CartItem>
{
    Task<CartItem?> GetByCartAndProductAsync(
        Guid cartId,
        Guid productId,
        Guid? productVariantId,
        CancellationToken cancellationToken = default);

    Task<CartItem?> GetDetailsAsync(
        Guid cartItemId,
        CancellationToken cancellationToken = default);

    Task<List<CartItem>> GetByCartIdAsync(
        Guid cartId,
        CancellationToken cancellationToken = default);
}