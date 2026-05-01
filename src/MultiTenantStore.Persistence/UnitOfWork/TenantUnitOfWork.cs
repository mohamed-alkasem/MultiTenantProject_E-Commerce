using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Persistence.Contexts;

namespace MultiTenantStore.Persistence.UnitOfWork;

public sealed class TenantUnitOfWork : ITenantUnitOfWork
{
    private readonly TenantDbContext _context;

    public TenantUnitOfWork(TenantDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}   