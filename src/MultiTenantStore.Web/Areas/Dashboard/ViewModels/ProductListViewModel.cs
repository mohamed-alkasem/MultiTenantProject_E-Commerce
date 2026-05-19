using MultiTenantStore.Application.Catalog.DTOs;

namespace MultiTenantStore.Web.Areas.Dashboard.ViewModels;

public sealed class ProductListViewModel
{
    public List<ProductListDto> Products { get; set; } = new();
    public string? Search { get; set; }
}
