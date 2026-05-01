using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Customers.DTOs;

namespace MultiTenantStore.Application.Customers.Services;

public interface ICustomerAddressService
{
    Task<ApiResponseDto<List<CustomerAddressDto>>> GetAddressesAsync(
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<CustomerAddressDto>> AddAddressAsync(
        CreateCustomerAddressDto dto,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<CustomerAddressDto>> UpdateAddressAsync(
        UpdateCustomerAddressDto dto,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<bool>> DeleteAddressAsync(
        Guid addressId,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<CustomerAddressDto>> SetDefaultShippingAsync(
        Guid addressId,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<CustomerAddressDto>> SetDefaultBillingAsync(
        Guid addressId,
        CancellationToken cancellationToken = default);
}