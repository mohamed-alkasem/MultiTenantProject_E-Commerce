using MultiTenantStore.Domain.Common;

namespace MultiTenantStore.Domain.Tenant;

public class StoreSetting : AuditableEntity, ISoftDelete
{
    public string Currency { get; set; } = "USD";
    public string Timezone { get; set; } = "UTC";
    public string DefaultLanguage { get; set; } = "en";

    public bool IsCheckoutEnabled { get; set; } = true;
    public bool TaxEnabled { get; set; }

    public string OrderPrefix { get; set; } = "ORD";

    public DateTime? DeletedAt { get; set; }
}