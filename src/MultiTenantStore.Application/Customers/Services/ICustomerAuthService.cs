using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Customers.DTOs;

namespace MultiTenantStore.Application.Customers.Services;

public interface ICustomerAuthService
{
    Task<ApiResponseDto<CustomerAuthResponseDto>> RegisterAsync(
        RegisterCustomerDto dto,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<CustomerAuthResponseDto>> LoginAsync(
        CustomerLoginDto dto,
        CancellationToken cancellationToken = default);
}