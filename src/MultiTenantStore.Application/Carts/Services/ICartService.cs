using MultiTenantStore.Application.Carts.DTOs;
using MultiTenantStore.Application.Common.DTOs;

namespace MultiTenantStore.Application.Carts.Services;

public interface ICartService
{
    Task<ApiResponseDto<CartDto>> GetOrCreateCartAsync(
        string sessionId,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<CartDto>> GetCartAsync(
        string sessionId,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<CartDto>> AddItemAsync(
        string sessionId,
        AddToCartDto dto,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<CartDto>> UpdateItemQuantityAsync(
        string sessionId,
        UpdateCartItemDto dto,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<CartDto>> RemoveItemAsync(
        string sessionId,
        RemoveCartItemDto dto,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<bool>> ClearCartAsync(
        string sessionId,
        CancellationToken cancellationToken = default);
}