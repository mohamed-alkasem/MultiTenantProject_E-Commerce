using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Application.Payments.Repositories;
using MultiTenantStore.Domain.Tenant;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Persistence.Repositories.Generic;

namespace MultiTenantStore.Persistence.Repositories.Tenant.Payments;

public sealed class PaymentRepository : TenantRepository<Payment>, IPaymentRepository
{
    private readonly TenantDbContext _context;

    public PaymentRepository(TenantDbContext context)
        : base(context)
    {
        _context = context;
    }

    public Task<List<Payment>> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        return _context.Payments
            .Where(x => x.OrderId == orderId && x.DeletedAt == null)
            .ToListAsync(cancellationToken);
    }

    public Task<Payment?> GetByProviderReferenceAsync(
        string providerReference,
        CancellationToken cancellationToken = default)
    {
        return _context.Payments
            .FirstOrDefaultAsync(
                x => x.ProviderReference == providerReference &&
                     x.DeletedAt == null,
                cancellationToken);
    }
}