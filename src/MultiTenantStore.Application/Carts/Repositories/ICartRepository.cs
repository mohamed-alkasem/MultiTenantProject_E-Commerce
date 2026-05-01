using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Carts.Repositories;

public interface ICartRepository : ITenantRepository<Cart>
{
    Task<Cart?> GetActiveBySessionIdAsync(
        string sessionId,
        CancellationToken cancellationToken = default);

    Task<Cart?> GetDetailsAsync(
        Guid cartId,
        CancellationToken cancellationToken = default);

    Task<Cart?> GetDetailsBySessionIdAsync(
        string sessionId,
        CancellationToken cancellationToken = default);

    Task<Cart?> GetActiveByCustomerIdAsync(
    Guid customerId,
    CancellationToken cancellationToken = default);

    Task<Cart?> GetDetailsByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);
}