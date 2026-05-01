using MultiTenantStore.Domain.Common;

namespace MultiTenantStore.Application.Common.Interfaces;

public interface IRepository<T> : IReadRepository<T>, IWriteRepository<T>
    where T : BaseEntity
{
}