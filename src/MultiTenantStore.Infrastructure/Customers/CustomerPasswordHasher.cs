using Microsoft.AspNetCore.Identity;
using MultiTenantStore.Application.Customers.Services;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Infrastructure.Customers;

public sealed class CustomerPasswordHasher : ICustomerPasswordHasher
{
    private readonly PasswordHasher<Customer> _passwordHasher = new();

    public string HashPassword(Customer customer, string password)
    {
        return _passwordHasher.HashPassword(customer, password);
    }

    public bool VerifyPassword(Customer customer, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(
            customer,
            customer.PasswordHash,
            password);

        return result == PasswordVerificationResult.Success ||
               result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}