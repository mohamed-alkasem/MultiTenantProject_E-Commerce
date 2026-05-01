namespace MultiTenantStore.Application.Stores.DTOs;

public sealed class StoreDatabaseDto
{
    public Guid Id { get; set; }

    public Guid StoreId { get; set; }

    public string DatabaseName { get; set; } = default!;

    public string DbServer { get; set; } = default!;

    public string Provider { get; set; } = default!;

    public string ProvisioningStatus { get; set; } = default!;

    public bool IsProvisioned { get; set; }

    public DateTime? ProvisionedAt { get; set; }

    public string? MigrationVersion { get; set; }

    public DateTime? LastMigrationAt { get; set; }

    public string? LastError { get; set; }
}