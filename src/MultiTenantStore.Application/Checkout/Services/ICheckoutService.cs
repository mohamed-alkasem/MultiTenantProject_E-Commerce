using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Orders.DTOs;

namespace MultiTenantStore.Application.Checkout.Services;

public interface ICheckoutService
{
    Task<ApiResponseDto<OrderDto>> CreateOrderAsync(
        CreateOrderDto dto,
        CancellationToken cancellationToken = default);
}