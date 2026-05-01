using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Customers.Repositories;

public interface ICustomerRepository : ITenantRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<Customer?> GetDetailsAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(
        string email,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default);
}