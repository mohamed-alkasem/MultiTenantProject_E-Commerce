using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Domain.Common;
using MultiTenantStore.Persistence.Contexts;

namespace MultiTenantStore.Persistence.Repositories.Generic;

public class PlatformRepository<T> : Repository<T>, IPlatformRepository<T>
    where T : BaseEntity
{
    public PlatformRepository(MainDbContext context)
        : base(context)
    {
    }
}