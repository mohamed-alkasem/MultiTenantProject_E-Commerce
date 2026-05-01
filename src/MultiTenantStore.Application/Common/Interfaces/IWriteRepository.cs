using MultiTenantStore.Domain.Common;

namespace MultiTenantStore.Application.Common.Interfaces;

public interface IWriteRepository<T> where T : BaseEntity
{
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    void Update(T entity);

    void Delete(T entity);
}