using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Catalog.Repositories;

public interface ICategoryRepository : ITenantRepository<Category>
{
    Task<List<Category>> GetActiveAsync(
        CancellationToken cancellationToken = default);

    Task<List<Category>> GetAllNotDeletedAsync(
        CancellationToken cancellationToken = default);

    Task<Category?> GetNotDeletedByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsBySlugAsync(
        string slug,
        Guid? excludedId = null,
        CancellationToken cancellationToken = default);
}