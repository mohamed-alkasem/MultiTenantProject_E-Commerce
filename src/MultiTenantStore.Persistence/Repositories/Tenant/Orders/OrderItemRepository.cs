using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Application.Orders.Repositories;
using MultiTenantStore.Domain.Tenant;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Persistence.Repositories.Generic;

namespace MultiTenantStore.Persistence.Repositories.Tenant.Orders;

public sealed class OrderItemRepository : TenantRepository<OrderItem>, IOrderItemRepository
{
    private readonly TenantDbContext _context;

    public OrderItemRepository(TenantDbContext context)
        : base(context)
    {
        _context = context;
    }

    public Task<List<OrderItem>> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        return _context.OrderItems
            .Where(x => x.OrderId == orderId && x.DeletedAt == null)
            .ToListAsync(cancellationToken);
    }
}