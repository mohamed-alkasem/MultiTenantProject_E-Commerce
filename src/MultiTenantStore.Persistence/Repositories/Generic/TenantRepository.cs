using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Domain.Common;
using MultiTenantStore.Persistence.Contexts;

namespace MultiTenantStore.Persistence.Repositories.Generic;

public class TenantRepository<T> : Repository<T>, ITenantRepository<T>
    where T : BaseEntity
{
    public TenantRepository(TenantDbContext context)
        : base(context)
    {
    }
}