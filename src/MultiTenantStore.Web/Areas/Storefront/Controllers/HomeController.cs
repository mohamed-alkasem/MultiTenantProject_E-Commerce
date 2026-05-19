using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.Carts.Services;
using MultiTenantStore.Application.Catalog.DTOs;
using MultiTenantStore.Application.Common.MultiTenancy;
using MultiTenantStore.Application.Storefront.Services;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Web.Areas.Storefront.ViewModels;

namespace MultiTenantStore.Web.Areas.Storefront.Controllers;

public sealed class HomeController : StorefrontBaseController
{
    private readonly IPublicCatalogService _catalog;

    public HomeController(
        ITenantStore tenantStore,
        ICurrentTenant currentTenant,
        MainDbContext mainDb,
        ICartService cartService,
        IPublicCatalogService catalog)
        : base(tenantStore, currentTenant, mainDb, cartService)
    {
        _catalog = catalog;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var storeCtx = GetStoreCtx();

        var categoriesResult = await _catalog.GetCategoriesAsync(ct);
        var featuredResult = await _catalog.GetProductsAsync(
            new ProductSearchRequestDto { IsFeatured = true, IsActive = true, PageSize = 8 }, ct);

        return View(new HomePageViewModel
        {
            Store = storeCtx,
            FeaturedProducts = featuredResult.Data?.Items ?? new(),
            Categories = categoriesResult.Data ?? new(),
        });
    }
}
