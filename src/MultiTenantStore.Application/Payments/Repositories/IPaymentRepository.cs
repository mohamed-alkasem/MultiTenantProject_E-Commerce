using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Payments.Repositories;

public interface IPaymentRepository : ITenantRepository<Payment>
{
    Task<List<Payment>> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task<Payment?> GetByProviderReferenceAsync(
        string providerReference,
        CancellationToken cancellationToken = default);
}