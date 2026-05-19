namespace MultiTenantStore.Web.Areas.PlatformAdmin.ViewModels;

public sealed class PlatformSubscriptionsListViewModel
{
    public List<PlatformSubscriptionRowViewModel> Subscriptions { get; set; } = new();
    public string? Search { get; set; }
}

public sealed class PlatformSubscriptionRowViewModel
{
    public Guid Id { get; set; }
    public string StoreName { get; set; } = default!;
    public string OwnerEmail { get; set; } = default!;
    public string PlanName { get; set; } = default!;
    public string? PlanNameAr { get; set; }
    public string Status { get; set; } = default!;
    public string BillingCycle { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? NextBillingAt { get; set; }
}
