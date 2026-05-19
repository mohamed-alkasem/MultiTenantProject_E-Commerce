using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.Carts.Services;
using MultiTenantStore.Application.Catalog.DTOs;
using MultiTenantStore.Application.Common.MultiTenancy;
using MultiTenantStore.Application.Storefront.Services;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Web.Areas.Storefront.ViewModels;

namespace MultiTenantStore.Web.Areas.Storefront.Controllers;

public sealed class ProductsController : StorefrontBaseController
{
    private readonly IPublicCatalogService _catalog;

    public ProductsController(
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
    public async Task<IActionResult> Index(string? search, Guid? categoryId, int page = 1, CancellationToken ct = default)
    {
        var storeCtx = GetStoreCtx();
        const int pageSize = 12;

        var request = new ProductSearchRequestDto
        {
            Search = search,
            CategoryId = categoryId,
            IsActive = true,
            PageNumber = page,
            PageSize = pageSize
        };

        var productsResult = await _catalog.GetProductsAsync(request, ct);
        var categoriesResult = await _catalog.GetCategoriesAsync(ct);

        string? categoryName = null;
        if (categoryId.HasValue)
        {
            categoryName = categoriesResult.Data?.FirstOrDefault(c => c.Id == categoryId)?.Name;
        }

        return View(new ProductListViewModel
        {
            Store = storeCtx,
            Products = productsResult.Data?.Items ?? new(),
            Categories = categoriesResult.Data ?? new(),
            Search = search,
            CategoryId = categoryId,
            CategoryName = categoryName,
            PageNumber = page,
            TotalCount = productsResult.Data?.TotalCount ?? 0,
            PageSize = pageSize,
        });
    }

    [HttpGet]
    public async Task<IActionResult> Details(string slug, CancellationToken ct)
    {
        var storeCtx = GetStoreCtx();

        var result = await _catalog.GetProductBySlugAsync(slug, ct);
        if (!result.Success || result.Data is null)
            return NotFound();

        return View(new ProductDetailsViewModel
        {
            Store = storeCtx,
            Product = result.Data,
        });
    }
}
