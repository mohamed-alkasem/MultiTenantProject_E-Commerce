using MultiTenantStore.Application.Catalog.DTOs;
using MultiTenantStore.Application.Catalog.Repositories;
using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Catalog.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductVariantRepository _variantRepository;
    private readonly IProductImageRepository _imageRepository;
    private readonly ITenantUnitOfWork _unitOfWork;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IProductVariantRepository variantRepository,
        IProductImageRepository imageRepository,
        ITenantUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _variantRepository = variantRepository;
        _imageRepository = imageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto<ProductDto>> CreateAsync(
        CreateProductDto dto,
        CancellationToken cancellationToken = default)
    {
        var categoryExists = await _categoryRepository.ExistsAsync(
            x => x.Id == dto.CategoryId && x.DeletedAt == null,
            cancellationToken);

        if (!categoryExists)
        {
            return ApiResponseDto<ProductDto>.Fail("Category was not found.");
        }

        var slug = NormalizeSlug(dto.Slug);
        var sku = NormalizeSku(dto.SKU);

        if (await _productRepository.ExistsBySlugAsync(slug, cancellationToken: cancellationToken))
        {
            return ApiResponseDto<ProductDto>.Fail("Product slug is already used.");
        }

        if (await _productRepository.ExistsBySkuAsync(sku, cancellationToken: cancellationToken))
        {
            return ApiResponseDto<ProductDto>.Fail("Product SKU is already used.");
        }

        var normalizedVariantSkus = dto.Variants
            .Select(x => NormalizeSku(x.SKU))
            .ToList();

        if (normalizedVariantSkus.Count != normalizedVariantSkus.Distinct().Count())
        {
            return ApiResponseDto<ProductDto>.Fail("Duplicate variant SKU in request.");
        }

        foreach (var variantSku in normalizedVariantSkus)
        {
            if (await _variantRepository.ExistsBySkuAsync(variantSku, cancellationToken: cancellationToken))
            {
                return ApiResponseDto<ProductDto>.Fail(
                    $"Variant SKU '{variantSku}' is already used.");
            }
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            CategoryId = dto.CategoryId,
            Name = dto.Name.Trim(),
            Slug = slug,
            ShortDescription = dto.ShortDescription,
            Description = dto.Description,
            SKU = sku,
            Price = dto.Price,
            CompareAtPrice = dto.CompareAtPrice,
            CostPrice = dto.CostPrice,
            StockQuantity = dto.StockQuantity,
            TrackInventory = dto.TrackInventory,
            LowStockThreshold = dto.LowStockThreshold,
            IsFeatured = dto.IsFeatured,
            IsActive = dto.IsActive,
            SortOrder = dto.SortOrder,
            PublishedAt = dto.IsActive ? DateTime.UtcNow : null,
            CreatedAt = DateTime.UtcNow
        };

        await _productRepository.AddAsync(product, cancellationToken);

        foreach (var variantDto in dto.Variants)
        {
            var variant = new ProductVariant
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                SKU = NormalizeSku(variantDto.SKU),
                Name = variantDto.Name.Trim(),
                Price = variantDto.Price,
                CompareAtPrice = variantDto.CompareAtPrice,
                CostPrice = variantDto.CostPrice,
                StockQuantity = variantDto.StockQuantity,
                TrackInventory = variantDto.TrackInventory,
                AttributesJson = variantDto.AttributesJson,
                IsActive = variantDto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            await _variantRepository.AddAsync(variant, cancellationToken);
            product.Variants.Add(variant);
        }

        foreach (var imageDto in dto.Images)
        {
            var image = new ProductImage
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                ImageUrl = imageDto.ImageUrl,
                AltText = imageDto.AltText,
                SortOrder = imageDto.SortOrder,
                IsPrimary = imageDto.IsPrimary,
                CreatedAt = DateTime.UtcNow
            };

            await _imageRepository.AddAsync(image, cancellationToken);
            product.Images.Add(image);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var category = await _categoryRepository.GetNotDeletedByIdAsync(
            product.CategoryId,
            cancellationToken);

        return ApiResponseDto<ProductDto>.Ok(
            MapToDto(product, category?.Name ?? string.Empty),
            "Product created successfully.");
    }

    public async Task<ApiResponseDto<List<ProductListDto>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllWithImagesAsync(cancellationToken);

        var result = products
            .Select(MapToListDto)
            .ToList();

        return ApiResponseDto<List<ProductListDto>>.Ok(result);
    }

    public async Task<ApiResponseDto<ProductDto>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetDetailsAsync(
            id,
            cancellationToken);

        if (product is null)
        {
            return ApiResponseDto<ProductDto>.Fail("Product was not found.");
        }

        return ApiResponseDto<ProductDto>.Ok(
            MapToDto(product, product.Category.Name));
    }

    public async Task<ApiResponseDto<ProductDto>> UpdateAsync(
        UpdateProductDto dto,
        CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetDetailsAsync(
            dto.Id,
            cancellationToken);

        if (product is null)
        {
            return ApiResponseDto<ProductDto>.Fail("Product was not found.");
        }

        var categoryExists = await _categoryRepository.ExistsAsync(
            x => x.Id == dto.CategoryId && x.DeletedAt == null,
            cancellationToken);

        if (!categoryExists)
        {
            return ApiResponseDto<ProductDto>.Fail("Category was not found.");
        }

        var slug = NormalizeSlug(dto.Slug);
        var sku = NormalizeSku(dto.SKU);

        if (await _productRepository.ExistsBySlugAsync(slug, dto.Id, cancellationToken))
        {
            return ApiResponseDto<ProductDto>.Fail("Product slug is already used.");
        }

        if (await _productRepository.ExistsBySkuAsync(sku, dto.Id, cancellationToken))
        {
            return ApiResponseDto<ProductDto>.Fail("Product SKU is already used.");
        }

        product.CategoryId = dto.CategoryId;
        product.Name = dto.Name.Trim();
        product.Slug = slug;
        product.ShortDescription = dto.ShortDescription;
        product.Description = dto.Description;
        product.SKU = sku;
        product.Price = dto.Price;
        product.CompareAtPrice = dto.CompareAtPrice;
        product.CostPrice = dto.CostPrice;
        product.StockQuantity = dto.StockQuantity;
        product.TrackInventory = dto.TrackInventory;
        product.LowStockThreshold = dto.LowStockThreshold;
        product.IsFeatured = dto.IsFeatured;
        product.IsActive = dto.IsActive;
        product.SortOrder = dto.SortOrder;
        product.UpdatedAt = DateTime.UtcNow;

        if (product.IsActive && product.PublishedAt is null)
        {
            product.PublishedAt = DateTime.UtcNow;
        }

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var category = await _categoryRepository.GetNotDeletedByIdAsync(
            product.CategoryId,
            cancellationToken);

        return ApiResponseDto<ProductDto>.Ok(
            MapToDto(product, category?.Name ?? string.Empty),
            "Product updated successfully.");
    }

    public async Task<ApiResponseDto<bool>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetDetailsAsync(
            id,
            cancellationToken);

        if (product is null)
        {
            return ApiResponseDto<bool>.Fail("Product was not found.");
        }

        product.DeletedAt = DateTime.UtcNow;
        product.UpdatedAt = DateTime.UtcNow;

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<bool>.Ok(true, "Product deleted successfully.");
    }

    private static string NormalizeSlug(string slug)
    {
        return slug.Trim().ToLowerInvariant();
    }

    private static string NormalizeSku(string sku)
    {
        return sku.Trim().ToUpperInvariant();
    }

    private static ProductListDto MapToListDto(Product product)
    {
        var primaryImageUrl = product.Images
            .Where(x => x.DeletedAt == null)
            .OrderByDescending(x => x.IsPrimary)
            .ThenBy(x => x.SortOrder)
            .Select(x => x.ImageUrl)
            .FirstOrDefault();

        return new ProductListDto
        {
            Id = product.Id,
            Name = product.Name,
            Slug = product.Slug,
            SKU = product.SKU,
            Price = product.Price,
            CompareAtPrice = product.CompareAtPrice,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive,
            IsFeatured = product.IsFeatured,
            PrimaryImageUrl = primaryImageUrl
        };
    }

    private static ProductDto MapToDto(Product product, string categoryName)
    {
        return new ProductDto
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            CategoryName = categoryName,
            Name = product.Name,
            Slug = product.Slug,
            ShortDescription = product.ShortDescription,
            Description = product.Description,
            SKU = product.SKU,
            Price = product.Price,
            CompareAtPrice = product.CompareAtPrice,
            CostPrice = product.CostPrice,
            StockQuantity = product.StockQuantity,
            TrackInventory = product.TrackInventory,
            LowStockThreshold = product.LowStockThreshold,
            IsFeatured = product.IsFeatured,
            IsActive = product.IsActive,
            SortOrder = product.SortOrder,
            PublishedAt = product.PublishedAt,
            Variants = product.Variants
                .Where(x => x.DeletedAt == null)
                .Select(x => new ProductVariantDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    SKU = x.SKU,
                    Name = x.Name,
                    Price = x.Price,
                    CompareAtPrice = x.CompareAtPrice,
                    CostPrice = x.CostPrice,
                    StockQuantity = x.StockQuantity,
                    TrackInventory = x.TrackInventory,
                    AttributesJson = x.AttributesJson,
                    IsActive = x.IsActive
                })
                .ToList(),
            Images = product.Images
                .Where(x => x.DeletedAt == null)
                .Select(x => new ProductImageDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    ImageUrl = x.ImageUrl,
                    AltText = x.AltText,
                    SortOrder = x.SortOrder,
                    IsPrimary = x.IsPrimary
                })
                .ToList()
        };
    }
}