using MultiTenantStore.Domain.Common;
using MultiTenantStore.Domain.Enums;

namespace MultiTenantStore.Domain.Platform;

public class StoreDatabase : AuditableEntity, ISoftDelete
{
    public Guid StoreId { get; set; }

    public string DatabaseName { get; set; } = default!;
    public string DbServer { get; set; } = default!;
    public string Provider { get; set; } = default!;

    public string ConnectionStringEncrypted { get; set; } = default!;

    public ProvisioningStatus ProvisioningStatus { get; set; } = ProvisioningStatus.NotStarted;
    public bool IsProvisioned { get; set; }

    public DateTime? ProvisionedAt { get; set; }
    public string? MigrationVersion { get; set; }
    public DateTime? LastMigrationAt { get; set; }
    public string? LastError { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Store Store { get; set; } = default!;
}