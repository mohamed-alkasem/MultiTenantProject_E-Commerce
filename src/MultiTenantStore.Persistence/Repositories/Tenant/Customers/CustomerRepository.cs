using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Application.Customers.Repositories;
using MultiTenantStore.Domain.Tenant;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Persistence.Repositories.Generic;

namespace MultiTenantStore.Persistence.Repositories.Tenant.Customers;

public sealed class CustomerRepository : TenantRepository<Customer>, ICustomerRepository
{
    private readonly TenantDbContext _context;

    public CustomerRepository(TenantDbContext context)
        : base(context)
    {
        _context = context;
    }

    public Task<Customer?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return _context.Customers
            .FirstOrDefaultAsync(
                x => x.Email == email &&
                     x.DeletedAt == null,
                cancellationToken);
    }

    public Task<Customer?> GetDetailsAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        return _context.Customers
            .Include(x => x.Addresses.Where(a => a.DeletedAt == null))
            .FirstOrDefaultAsync(
                x => x.Id == customerId &&
                     x.DeletedAt == null,
                cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(
        string email,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default)
    {
        return _context.Customers.AnyAsync(
            x => x.Email == email &&
                 x.DeletedAt == null &&
                 (excludedId == null || x.Id != excludedId.Value),
            cancellationToken);
    }
}