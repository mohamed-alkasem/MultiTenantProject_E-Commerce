namespace MultiTenantStore.Application.Stores.DTOs;

public sealed class UpdateStoreSettingDto
{
    public string Currency { get; set; } = default!;

    public string Timezone { get; set; } = default!;

    public string DefaultLanguage { get; set; } = default!;

    public bool IsCheckoutEnabled { get; set; }

    public bool TaxEnabled { get; set; }

    public string OrderPrefix { get; set; } = default!;
}