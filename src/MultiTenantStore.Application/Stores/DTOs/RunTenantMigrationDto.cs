namespace MultiTenantStore.Application.Stores.DTOs;

public sealed class RunTenantMigrationDto
{
    public Guid StoreId { get; set; }

    public string? TargetMigration { get; set; }
}