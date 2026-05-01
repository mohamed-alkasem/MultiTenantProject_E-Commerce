using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Catalog.Repositories;

public interface IProductVariantRepository : ITenantRepository<ProductVariant>
{
    Task<bool> ExistsBySkuAsync(
        string sku,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default);

    Task<List<ProductVariant>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default);
}