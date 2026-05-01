using MultiTenantStore.Application.Catalog.DTOs;
using MultiTenantStore.Application.Common.DTOs;

namespace MultiTenantStore.Application.Catalog.Services;

public interface IProductVariantService
{
    Task<ApiResponseDto<ProductVariantDto>> CreateAsync(
        Guid productId,
        CreateProductVariantDto dto,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<List<ProductVariantDto>>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<ProductVariantDto>> UpdateAsync(
        Guid productId,
        UpdateProductVariantDto dto,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<bool>> DeleteAsync(
        Guid productId,
        Guid variantId,
        CancellationToken cancellationToken = default);
}