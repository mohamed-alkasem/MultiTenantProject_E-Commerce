namespace MultiTenantStore.Application.Common.MultiTenancy;

public interface ITenantAccessValidator
{
    Task<bool> CanAccessStoreAsync(
        Guid userId,
        Guid storeId,
        CancellationToken cancellationToken = default);

    Task<bool> HasStoreRoleAsync(
        Guid userId,
        Guid storeId,
        string role,
        CancellationToken cancellationToken = default);
}