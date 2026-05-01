using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Orders.DTOs;

namespace MultiTenantStore.Application.Orders.Services;

public interface ICustomerOrderService
{
    Task<ApiResponseDto<List<OrderListDto>>> GetMyOrdersAsync(
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<OrderDto>> GetMyOrderByIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);
}