using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.Carts.Services;
using MultiTenantStore.Application.Catalog.DTOs;
using MultiTenantStore.Application.Common.MultiTenancy;
using MultiTenantStore.Application.Storefront.Services;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Web.Areas.Storefront.ViewModels;

namespace MultiTenantStore.Web.Areas.Storefront.Controllers;

public sealed class CategoriesController : StorefrontBaseController
{
    private readonly IPublicCatalogService _catalog;

    public CategoriesController(
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
    public async Task<IActionResult> Details(string slug, int page = 1, CancellationToken ct = default)
    {
        var storeCtx = GetStoreCtx();
        var categoriesResult = await _catalog.GetCategoriesAsync(ct);
        var category = categoriesResult.Data?.FirstOrDefault(c => c.Slug == slug);
        if (category is null)
            return NotFound();

        const int pageSize = 12;
        var productsResult = await _catalog.GetProductsAsync(
            new ProductSearchRequestDto
            {
                CategoryId = category.Id,
                IsActive = true,
                PageNumber = page,
                PageSize = pageSize
            }, ct);

        return View("~/Areas/Storefront/Views/Products/Index.cshtml", new ProductListViewModel
        {
            Store = storeCtx,
            Products = productsResult.Data?.Items ?? new(),
            Categories = categoriesResult.Data ?? new(),
            CategoryId = category.Id,
            CategoryName = category.Name,
            PageNumber = page,
            TotalCount = productsResult.Data?.TotalCount ?? 0,
            PageSize = pageSize,
        });
    }
}
