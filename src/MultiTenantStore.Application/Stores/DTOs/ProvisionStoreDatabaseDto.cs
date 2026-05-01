namespace MultiTenantStore.Application.Stores.DTOs;

public sealed class ProvisionStoreDatabaseDto
{
    public Guid StoreId { get; set; }

    public bool RunMigrations { get; set; }

    public bool SeedInitialData { get; set; }
}