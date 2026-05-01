namespace MultiTenantStore.Application.Subscriptions.DTOs;

public sealed class CreateSubscriptionPlanDto
{
    public string Name { get; set; } = default!;

    public string Code { get; set; } = default!;

    public decimal PriceMonthly { get; set; }

    public decimal PriceYearly { get; set; }

    public int MaxProducts { get; set; }

    public int MaxStaffUsers { get; set; }

    public bool IsActive { get; set; }
}