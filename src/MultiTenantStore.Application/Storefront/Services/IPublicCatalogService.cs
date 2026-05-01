using MultiTenantStore.Application.Catalog.DTOs;
using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Storefront.DTOs;

namespace MultiTenantStore.Application.Storefront.Services;

public interface IPublicCatalogService
{
    Task<ApiResponseDto<List<PublicCategoryDto>>> GetCategoriesAsync(
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<PagedResultDto<PublicProductListDto>>> GetProductsAsync(
        ProductSearchRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<PublicProductDetailsDto>> GetProductBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default);
}