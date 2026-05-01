using MultiTenantStore.Application.Catalog.DTOs;
using MultiTenantStore.Application.Common.DTOs;

namespace MultiTenantStore.Application.Catalog.Services;

public interface IProductService
{
    Task<ApiResponseDto<ProductDto>> CreateAsync(
        CreateProductDto dto,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<List<ProductListDto>>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<ProductDto>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<ProductDto>> UpdateAsync(
        UpdateProductDto dto,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<bool>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}