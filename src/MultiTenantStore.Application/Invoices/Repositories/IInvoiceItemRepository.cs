using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Invoices.Repositories;

public interface IInvoiceItemRepository : ITenantRepository<InvoiceItem>
{
    Task<List<InvoiceItem>> GetByInvoiceIdAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default);
}