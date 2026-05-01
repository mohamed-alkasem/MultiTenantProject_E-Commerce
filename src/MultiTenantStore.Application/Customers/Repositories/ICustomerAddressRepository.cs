using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Customers.Repositories;

public interface ICustomerAddressRepository : ITenantRepository<CustomerAddress>
{
    Task<List<CustomerAddress>> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task<CustomerAddress?> GetByIdAndCustomerIdAsync(
        Guid addressId,
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task<CustomerAddress?> GetDefaultShippingAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task<CustomerAddress?> GetDefaultBillingAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);
}