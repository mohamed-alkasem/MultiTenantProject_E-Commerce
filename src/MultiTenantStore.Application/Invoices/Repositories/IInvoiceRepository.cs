using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Invoices.Repositories;

public interface IInvoiceRepository : ITenantRepository<Invoice>
{
    Task<Invoice?> GetDetailsAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default);

    Task<Invoice?> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task<Invoice?> GetByInvoiceNumberAsync(
        string invoiceNumber,
        CancellationToken cancellationToken = default);

    Task<List<Invoice>> GetAllNotDeletedAsync(
        CancellationToken cancellationToken = default);
}