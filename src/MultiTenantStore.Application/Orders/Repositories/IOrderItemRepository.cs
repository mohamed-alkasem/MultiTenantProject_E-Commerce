using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Orders.Repositories;

public interface IOrderItemRepository : ITenantRepository<OrderItem>
{
    Task<List<OrderItem>> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);
}