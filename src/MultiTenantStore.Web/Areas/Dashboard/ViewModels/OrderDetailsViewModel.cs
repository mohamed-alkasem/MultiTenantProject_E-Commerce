using MultiTenantStore.Application.Orders.DTOs;

namespace MultiTenantStore.Web.Areas.Dashboard.ViewModels;

public sealed class OrderDetailsViewModel
{
    public OrderDto Order { get; set; } = default!;
}

public sealed class OrdersListViewModel
{
    public List<OrderListDto> Orders { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public string? StatusFilter { get; set; }
}
