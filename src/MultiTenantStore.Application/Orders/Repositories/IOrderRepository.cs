using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Application.Orders.DTOs;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Orders.Repositories;

public interface IOrderRepository : ITenantRepository<Order>
{
    Task<Order?> GetDetailsAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task<Order?> GetByOrderNumberAsync(
        string orderNumber,
        CancellationToken cancellationToken = default);

    Task<(List<Order> Items, int TotalCount)> SearchAsync(
        OrderSearchRequestDto request,
        CancellationToken cancellationToken = default);
}