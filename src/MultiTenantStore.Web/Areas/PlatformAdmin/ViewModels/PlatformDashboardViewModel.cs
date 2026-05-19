using MultiTenantStore.Domain.Enums;

namespace MultiTenantStore.Web.Areas.PlatformAdmin.ViewModels;

public sealed class PlatformDashboardViewModel
{
    public int TotalStores { get; set; }
    public int ActiveStores { get; set; }
    public int PendingStores { get; set; }
    public int SuspendedStores { get; set; }
    public int TotalMerchants { get; set; }
    public List<PlatformStoreRowViewModel> RecentStores { get; set; } = new();
}

public sealed class PlatformStoreRowViewModel
{
    public Guid Id { get; set; }
    public string StoreName { get; set; } = default!;
    public string? StoreNameAr { get; set; }
    public string Slug { get; set; } = default!;
    public string OwnerName { get; set; } = default!;
    public string OwnerEmail { get; set; } = default!;
    public StoreStatus Status { get; set; }
    public SubscriptionStatus SubscriptionStatus { get; set; }
    public bool HasDatabase { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class PlatformStoresListViewModel
{
    public List<PlatformStoreRowViewModel> Stores { get; set; } = new();
    public string? Search { get; set; }
    public string? StatusFilter { get; set; }
}

public sealed class StoreDetailsViewModel
{
    public Guid Id { get; set; }
    public string StoreName { get; set; } = default!;
    public string? StoreNameAr { get; set; }
    public string Slug { get; set; } = default!;
    public StoreStatus Status { get; set; }
    public SubscriptionStatus SubscriptionStatus { get; set; }
    public string OwnerName { get; set; } = default!;
    public string OwnerEmail { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime? ActivatedAt { get; set; }
    public DateTime? SuspendedAt { get; set; }
    public bool HasDatabase { get; set; }
    public string? DatabaseName { get; set; }
    public string? ProvisioningStatus { get; set; }
    public List<string> Domains { get; set; } = new();
    public ActiveSubscriptionViewModel? ActiveSubscription { get; set; }
}

public sealed class ActiveSubscriptionViewModel
{
    public string PlanName { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string BillingCycle { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
