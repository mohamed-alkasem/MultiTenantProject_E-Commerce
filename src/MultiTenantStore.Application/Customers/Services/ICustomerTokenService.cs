using MultiTenantStore.Application.Customers.DTOs;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Customers.Services;

public interface ICustomerTokenService
{
    CustomerAuthResponseDto GenerateToken(Customer customer);
}