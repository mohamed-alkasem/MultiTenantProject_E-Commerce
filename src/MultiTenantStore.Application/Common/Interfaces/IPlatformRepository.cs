using MultiTenantStore.Domain.Common;

namespace MultiTenantStore.Application.Common.Interfaces;

public interface IPlatformRepository<T> : IRepository<T>
    where T : BaseEntity
{
}