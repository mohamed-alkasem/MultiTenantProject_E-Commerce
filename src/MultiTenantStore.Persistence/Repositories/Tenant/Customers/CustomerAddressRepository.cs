using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Application.Customers.Repositories;
using MultiTenantStore.Domain.Tenant;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Persistence.Repositories.Generic;

namespace MultiTenantStore.Persistence.Repositories.Tenant.Customers;

public sealed class CustomerAddressRepository
    : TenantRepository<CustomerAddress>, ICustomerAddressRepository
{
    private readonly TenantDbContext _context;

    public CustomerAddressRepository(TenantDbContext context)
        : base(context)
    {
        _context = context;
    }

    public Task<List<CustomerAddress>> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        return _context.CustomerAddresses
            .Where(x => x.CustomerId == customerId && x.DeletedAt == null)
            .OrderByDescending(x => x.IsDefaultShipping)
            .ThenByDescending(x => x.IsDefaultBilling)
            .ThenBy(x => x.Title)
            .ToListAsync(cancellationToken);
    }

    public Task<CustomerAddress?> GetByIdAndCustomerIdAsync(
        Guid addressId,
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        return _context.CustomerAddresses
            .FirstOrDefaultAsync(
                x => x.Id == addressId &&
                     x.CustomerId == customerId &&
                     x.DeletedAt == null,
                cancellationToken);
    }

    public Task<CustomerAddress?> GetDefaultShippingAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        return _context.CustomerAddresses
            .FirstOrDefaultAsync(
                x => x.CustomerId == customerId &&
                     x.IsDefaultShipping &&
                     x.DeletedAt == null,
                cancellationToken);
    }

    public Task<CustomerAddress?> GetDefaultBillingAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        return _context.CustomerAddresses
            .FirstOrDefaultAsync(
                x => x.CustomerId == customerId &&
                     x.IsDefaultBilling &&
                     x.DeletedAt == null,
                cancellationToken);
    }
}