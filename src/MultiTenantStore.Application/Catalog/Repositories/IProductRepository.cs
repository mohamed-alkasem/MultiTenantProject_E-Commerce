using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Catalog.Repositories;

public interface IProductRepository : ITenantRepository<Product>
{
    Task<Product?> GetDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<Product>> GetAllWithImagesAsync(
        CancellationToken cancellationToken = default);

    Task<Product?> GetActiveDetailsBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default);

    Task<(List<Product> Items, int TotalCount)> SearchActiveAsync(
        string? search,
        Guid? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsBySlugAsync(
        string slug,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsBySkuAsync(
        string sku,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default);
}