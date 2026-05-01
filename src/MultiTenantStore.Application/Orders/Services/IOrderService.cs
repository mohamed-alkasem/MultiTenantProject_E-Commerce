using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Orders.DTOs;

namespace MultiTenantStore.Application.Orders.Services;

public interface IOrderService
{
    Task<ApiResponseDto<PagedResultDto<OrderListDto>>> GetOrdersAsync(
        OrderSearchRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<OrderDto>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<OrderDto>> UpdateOrderStatusAsync(
        UpdateOrderStatusDto dto,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<OrderDto>> UpdatePaymentStatusAsync(
        UpdatePaymentStatusDto dto,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<OrderDto>> UpdateShippingStatusAsync(
        UpdateShippingStatusDto dto,
        CancellationToken cancellationToken = default);
}