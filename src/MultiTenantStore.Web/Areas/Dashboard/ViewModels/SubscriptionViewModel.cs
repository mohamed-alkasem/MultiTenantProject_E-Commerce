namespace MultiTenantStore.Web.Areas.Dashboard.ViewModels;

public sealed class SubscriptionViewModel
{
    public bool HasSubscription { get; set; }
    public string PlanName { get; set; } = "—";
    public string? PlanNameAr { get; set; }
    public string PlanCode { get; set; } = "";
    public string Status { get; set; } = "—";
    public string BillingCycle { get; set; } = "—";
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? TrialEndsAt { get; set; }
    public DateTime? NextBillingAt { get; set; }
    public int MaxProducts { get; set; }
    public int MaxStaffUsers { get; set; }
    public decimal PriceMonthly { get; set; }
    public decimal PriceYearly { get; set; }
}
