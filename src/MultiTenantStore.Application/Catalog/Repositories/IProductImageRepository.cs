using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Catalog.Repositories;

public interface IProductImageRepository : ITenantRepository<ProductImage>
{
    Task<List<ProductImage>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default);

    Task<ProductImage?> GetPrimaryImageAsync(
        Guid productId,
        CancellationToken cancellationToken = default);
}