using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Application.Invoices.Repositories;
using MultiTenantStore.Domain.Tenant;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Persistence.Repositories.Generic;

namespace MultiTenantStore.Persistence.Repositories.Tenant.Invoices;

public sealed class InvoiceRepository : TenantRepository<Invoice>, IInvoiceRepository
{
    private readonly TenantDbContext _context;

    public InvoiceRepository(TenantDbContext context)
        : base(context)
    {
        _context = context;
    }

    public Task<Invoice?> GetDetailsAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default)
    {
        return _context.Invoices
            .Include(x => x.Items.Where(i => i.DeletedAt == null))
            .FirstOrDefaultAsync(
                x => x.Id == invoiceId && x.DeletedAt == null,
                cancellationToken);
    }

    public Task<Invoice?> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        return _context.Invoices
            .Include(x => x.Items.Where(i => i.DeletedAt == null))
            .FirstOrDefaultAsync(
                x => x.OrderId == orderId && x.DeletedAt == null,
                cancellationToken);
    }

    public Task<Invoice?> GetByInvoiceNumberAsync(
        string invoiceNumber,
        CancellationToken cancellationToken = default)
    {
        return _context.Invoices
            .Include(x => x.Items.Where(i => i.DeletedAt == null))
            .FirstOrDefaultAsync(
                x => x.InvoiceNumber == invoiceNumber && x.DeletedAt == null,
                cancellationToken);
    }

    public Task<List<Invoice>> GetAllNotDeletedAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.Invoices
            .Where(x => x.DeletedAt == null)
            .OrderByDescending(x => x.IssueDate)
            .ToListAsync(cancellationToken);
    }
}