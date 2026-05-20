using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Customers.DTOs;

namespace MultiTenantStore.Application.Customers.Services;

public interface ICustomerService
{
    Task<ApiResponseDto<CustomerDto>> GetProfileAsync(
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<CustomerDto>> UpdateProfileAsync(
        UpdateCustomerDto dto,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<bool>> ChangePasswordAsync(
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default);
}