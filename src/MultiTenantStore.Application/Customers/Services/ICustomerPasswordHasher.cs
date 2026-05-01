using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Customers.Services;

public interface ICustomerPasswordHasher
{
    string HashPassword(Customer customer, string password);

    bool VerifyPassword(Customer customer, string password);
}