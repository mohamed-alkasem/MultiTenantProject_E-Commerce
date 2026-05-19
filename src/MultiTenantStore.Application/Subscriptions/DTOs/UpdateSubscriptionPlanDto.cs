namespace MultiTenantStore.Application.Subscriptions.DTOs;

public sealed class UpdateSubscriptionPlanDto
{
    public string Name { get; set; } = default!;
    public string? NameAr { get; set; }

    public decimal PriceMonthly { get; set; }

    public decimal PriceYearly { get; set; }

    public int MaxProducts { get; set; }

    public int MaxStaffUsers { get; set; }

    public bool IsActive { get; set; }
}