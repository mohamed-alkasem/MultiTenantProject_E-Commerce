namespace MultiTenantStore.Web.Areas.Dashboard.ViewModels;

public sealed class DashboardHomeViewModel
{
    public int TotalProducts { get; set; }
    public int TotalCategories { get; set; }
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public string Currency { get; set; } = "SAR";
    public bool IsCheckoutEnabled { get; set; }
    public List<RecentOrderViewModel> RecentOrders { get; set; } = new();
}

public sealed class RecentOrderViewModel
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = default!;
    public string? CustomerName { get; set; }
    public string Status { get; set; } = default!;
    public string PaymentStatus { get; set; } = default!;
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}
