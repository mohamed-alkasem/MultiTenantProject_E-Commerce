using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.Catalog.Services;
using MultiTenantStore.Application.Orders.DTOs;
using MultiTenantStore.Application.Orders.Services;
using MultiTenantStore.Application.StoreSettings.Services;
using MultiTenantStore.Domain.Platform;
using MultiTenantStore.Web.Areas.Dashboard.ViewModels;

namespace MultiTenantStore.Web.Areas.Dashboard.Controllers;

[Area("Dashboard")]
[Authorize(AuthenticationSchemes = "Identity.Application")]
public sealed class DashboardHomeController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IOrderService _orderService;
    private readonly IStoreSettingService _storeSettingService;

    public DashboardHomeController(
        IProductService productService,
        ICategoryService categoryService,
        IOrderService orderService,
        IStoreSettingService storeSettingService)
    {
        _productService = productService;
        _categoryService = categoryService;
        _orderService = orderService;
        _storeSettingService = storeSettingService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "نظرة عامة";

        var productsResult = await _productService.GetAllAsync(cancellationToken);
        var categoriesResult = await _categoryService.GetAllAsync(cancellationToken);
        var ordersResult = await _orderService.GetOrdersAsync(
            new OrderSearchRequestDto { PageNumber = 1, PageSize = 10 }, cancellationToken);
        var settingsResult = await _storeSettingService.GetSettingsAsync(cancellationToken);

        var vm = new DashboardHomeViewModel
        {
            TotalProducts = productsResult.Data?.Count ?? 0,
            TotalCategories = categoriesResult.Data?.Count ?? 0,
            IsCheckoutEnabled = settingsResult.Data?.IsCheckoutEnabled ?? false,
            Currency = settingsResult.Data?.Currency ?? "SAR",
        };

        if (ordersResult.Data != null)
        {
            vm.TotalOrders = ordersResult.Data.TotalCount;
            vm.PendingOrders = ordersResult.Data.Items.Count(o => o.Status == "Pending");
            vm.TotalRevenue = ordersResult.Data.Items.Sum(o => o.TotalAmount);
            vm.RecentOrders = ordersResult.Data.Items
                .Select(o => new RecentOrderViewModel
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    CustomerName = o.CustomerName,
                    Status = o.Status,
                    PaymentStatus = o.PaymentStatus,
                    TotalAmount = o.TotalAmount,
                    Currency = o.Currency,
                    CreatedAt = o.CreatedAt,
                })
                .ToList();
        }

        return View(vm);
    }
}
