using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Persistence.Contexts;

namespace MultiTenantStore.Persistence.UnitOfWork;

public sealed class PlatformUnitOfWork : IPlatformUnitOfWork
{
    private readonly MainDbContext _context;

    public PlatformUnitOfWork(MainDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}