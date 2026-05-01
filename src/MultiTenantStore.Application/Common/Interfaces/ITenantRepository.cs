using MultiTenantStore.Domain.Common;

namespace MultiTenantStore.Application.Common.Interfaces;

public interface ITenantRepository<T> : IRepository<T>
    where T : BaseEntity
{
}