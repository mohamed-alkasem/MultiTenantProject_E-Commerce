namespace MultiTenantStore.Application.Common.Interfaces;

public interface ITenantUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}