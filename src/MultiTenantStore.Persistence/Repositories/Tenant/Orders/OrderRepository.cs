using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Application.Orders.DTOs;
using MultiTenantStore.Application.Orders.Repositories;
using MultiTenantStore.Domain.Enums;
using MultiTenantStore.Domain.Tenant;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Persistence.Repositories.Generic;

namespace MultiTenantStore.Persistence.Repositories.Tenant.Orders;

public sealed class OrderRepository : TenantRepository<Order>, IOrderRepository
{
    private readonly TenantDbContext _context;

    public OrderRepository(TenantDbContext context)
        : base(context)
    {
        _context = context;
    }

    public Task<Order?> GetDetailsAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        return _context.Orders
            .Include(x => x.Items.Where(i => i.DeletedAt == null))
            .Include(x => x.Payments.Where(p => p.DeletedAt == null))
            .FirstOrDefaultAsync(
                x => x.Id == orderId && x.DeletedAt == null,
                cancellationToken);
    }

    public Task<Order?> GetByOrderNumberAsync(
        string orderNumber,
        CancellationToken cancellationToken = default)
    {
        return _context.Orders
            .Include(x => x.Items.Where(i => i.DeletedAt == null))
            .Include(x => x.Payments.Where(p => p.DeletedAt == null))
            .FirstOrDefaultAsync(
                x => x.OrderNumber == orderNumber && x.DeletedAt == null,
                cancellationToken);
    }

    public async Task<(List<Order> Items, int TotalCount)> SearchAsync(
        OrderSearchRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

        var query = _context.Orders
            .Where(x => x.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(x =>
                x.OrderNumber.Contains(search) ||
                x.ShippingFullName.Contains(search) ||
                x.ShippingPhone.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<OrderStatus>(request.Status, true, out var orderStatus))
        {
            query = query.Where(x => x.Status == orderStatus);
        }

        if (!string.IsNullOrWhiteSpace(request.PaymentStatus) &&
            Enum.TryParse<PaymentStatus>(request.PaymentStatus, true, out var paymentStatus))
        {
            query = query.Where(x => x.PaymentStatus == paymentStatus);
        }

        if (!string.IsNullOrWhiteSpace(request.ShippingStatus) &&
            Enum.TryParse<ShippingStatus>(request.ShippingStatus, true, out var shippingStatus))
        {
            query = query.Where(x => x.ShippingStatus == shippingStatus);
        }

        if (request.FromDate is not null)
        {
            query = query.Where(x => x.CreatedAt >= request.FromDate.Value);
        }

        if (request.ToDate is not null)
        {
            query = query.Where(x => x.CreatedAt <= request.ToDate.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}