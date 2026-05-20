using MultiTenantStore.Application.Catalog.DTOs;
using MultiTenantStore.Application.Catalog.Repositories;
using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Storefront.DTOs;

namespace MultiTenantStore.Application.Storefront.Services;

public sealed class PublicCatalogService : IPublicCatalogService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;

    public PublicCatalogService(
        ICategoryRepository categoryRepository,
        IProductRepository productRepository)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
    }

    public async Task<ApiResponseDto<List<PublicCategoryDto>>> GetCategoriesAsync(
        CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetActiveAsync(cancellationToken);

        var result = categories
            .Select(x => new PublicCategoryDto
            {
                Id = x.Id,
                Name = x.Name,
                NameAr = x.NameAr,
                Slug = x.Slug
            })
            .ToList();

        return ApiResponseDto<List<PublicCategoryDto>>.Ok(result);
    }

    public async Task<ApiResponseDto<PagedResultDto<PublicProductListDto>>> GetProductsAsync(
        ProductSearchRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

        var (products, totalCount) = await _productRepository.SearchActiveAsync(
            request.Search,
            request.CategoryId,
            request.MinPrice,
            request.MaxPrice,
            pageNumber,
            pageSize,
            cancellationToken);

        var items = products
            .Select(MapToPublicListDto)
            .ToList();

        var pagedResult = PagedResultDto<PublicProductListDto>.Create(
            items,
            pageNumber,
            pageSize,
            totalCount);

        return ApiResponseDto<PagedResultDto<PublicProductListDto>>.Ok(pagedResult);
    }

    public async Task<ApiResponseDto<PublicProductDetailsDto>> GetProductBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return ApiResponseDto<PublicProductDetailsDto>.Fail("Product was not found.");

        var normalizedSlug = slug.Trim().ToLowerInvariant();

        var product = await _productRepository.GetActiveDetailsBySlugAsync(
            normalizedSlug,
            cancellationToken);

        if (product is null)
        {
            return ApiResponseDto<PublicProductDetailsDto>.Fail("Product was not found.");
        }

        return ApiResponseDto<PublicProductDetailsDto>.Ok(
            MapToPublicDetailsDto(product));
    }

    private static PublicProductListDto MapToPublicListDto(Domain.Tenant.Product product)
    {
        var primaryImageUrl = product.Images
            .Where(x => x.DeletedAt == null)
            .OrderByDescending(x => x.IsPrimary)
            .ThenBy(x => x.SortOrder)
            .Select(x => x.ImageUrl)
            .FirstOrDefault();

        return new PublicProductListDto
        {
            Id = product.Id,
            Name = product.Name,
            NameAr = product.NameAr,
            Slug = product.Slug,
            ShortDescription = product.ShortDescription,
            ShortDescriptionAr = product.ShortDescriptionAr,
            Price = product.Price,
            CompareAtPrice = product.CompareAtPrice,
            PrimaryImageUrl = primaryImageUrl,
            IsFeatured = product.IsFeatured
        };
    }

    private static PublicProductDetailsDto MapToPublicDetailsDto(Domain.Tenant.Product product)
    {
        return new PublicProductDetailsDto
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name,
            Name = product.Name,
            NameAr = product.NameAr,
            Slug = product.Slug,
            ShortDescription = product.ShortDescription,
            ShortDescriptionAr = product.ShortDescriptionAr,
            Description = product.Description,
            DescriptionAr = product.DescriptionAr,
            SKU = product.SKU,
            Price = product.Price,
            CompareAtPrice = product.CompareAtPrice,
            StockQuantity = product.StockQuantity,
            IsFeatured = product.IsFeatured,
            Variants = product.Variants
                .Where(x => x.IsActive && x.DeletedAt == null)
                .Select(x => new PublicProductVariantDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    SKU = x.SKU,
                    Price = x.Price,
                    CompareAtPrice = x.CompareAtPrice,
                    StockQuantity = x.StockQuantity,
                    AttributesJson = x.AttributesJson
                })
                .ToList(),
            Images = product.Images
                .Where(x => x.DeletedAt == null)
                .OrderByDescending(x => x.IsPrimary)
                .ThenBy(x => x.SortOrder)
                .Select(x => new PublicProductImageDto
                {
                    Id = x.Id,
                    ImageUrl = x.ImageUrl,
                    AltText = x.AltText,
                    SortOrder = x.SortOrder,
                    IsPrimary = x.IsPrimary
                })
                .ToList()
        };
    }
}