using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Application.Invoices.Repositories;
using MultiTenantStore.Domain.Tenant;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Persistence.Repositories.Generic;

namespace MultiTenantStore.Persistence.Repositories.Tenant.Invoices;

public sealed class InvoiceItemRepository : TenantRepository<InvoiceItem>, IInvoiceItemRepository
{
    private readonly TenantDbContext _context;

    public InvoiceItemRepository(TenantDbContext context)
        : base(context)
    {
        _context = context;
    }

    public Task<List<InvoiceItem>> GetByInvoiceIdAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default)
    {
        return _context.InvoiceItems
            .Where(x => x.InvoiceId == invoiceId && x.DeletedAt == null)
            .ToListAsync(cancellationToken);
    }
}