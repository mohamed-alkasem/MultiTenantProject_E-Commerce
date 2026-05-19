using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MultiTenantStore.Application.Catalog.DTOs;
using MultiTenantStore.Application.Catalog.Services;
using MultiTenantStore.Web.Areas.Dashboard.ViewModels;

namespace MultiTenantStore.Web.Areas.Dashboard.Controllers;

[Area("Dashboard")]
[Authorize(AuthenticationSchemes = "Identity.Application")]
public sealed class DashboardProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public DashboardProductsController(
        IProductService productService,
        ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index(string? search, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "المنتجات";

        var result = await _productService.GetAllAsync(cancellationToken);
        var products = result.Data ?? new();

        if (!string.IsNullOrWhiteSpace(search))
        {
            products = products
                .Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                         || (p.NameAr != null && p.NameAr.Contains(search)))
                .ToList();
        }

        return View(new ProductListViewModel { Products = products, Search = search });
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "إضافة منتج";
        return View(new ProductFormViewModel
        {
            Categories = await GetCategoriesSelectList(cancellationToken)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Categories = await GetCategoriesSelectList(cancellationToken);
            ViewData["Title"] = "إضافة منتج";
            return View(model);
        }

        var dto = new CreateProductDto
        {
            CategoryId = model.CategoryId,
            Name = model.Name,
            NameAr = model.NameAr,
            Slug = model.Slug,
            ShortDescription = model.ShortDescription,
            ShortDescriptionAr = model.ShortDescriptionAr,
            Description = model.Description,
            DescriptionAr = model.DescriptionAr,
            SKU = model.SKU,
            Price = model.Price,
            CompareAtPrice = model.CompareAtPrice,
            CostPrice = model.CostPrice,
            StockQuantity = model.StockQuantity,
            TrackInventory = model.TrackInventory,
            LowStockThreshold = model.LowStockThreshold,
            IsFeatured = model.IsFeatured,
            IsActive = model.IsActive,
            SortOrder = model.SortOrder,
        };

        var result = await _productService.CreateAsync(dto, cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "حدث خطأ أثناء إضافة المنتج");
            model.Categories = await GetCategoriesSelectList(cancellationToken);
            return View(model);
        }

        TempData["SuccessMessage"] = $"تم إضافة المنتج \"{model.Name}\" بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "تعديل منتج";

        var result = await _productService.GetByIdAsync(id, cancellationToken);

        if (!result.Success || result.Data is null)
        {
            TempData["ErrorMessage"] = "المنتج غير موجود";
            return RedirectToAction(nameof(Index));
        }

        var p = result.Data;
        var model = new ProductFormViewModel
        {
            Id = p.Id,
            CategoryId = p.CategoryId,
            Name = p.Name,
            NameAr = p.NameAr,
            Slug = p.Slug,
            ShortDescription = p.ShortDescription,
            ShortDescriptionAr = p.ShortDescriptionAr,
            Description = p.Description,
            DescriptionAr = p.DescriptionAr,
            SKU = p.SKU,
            Price = p.Price,
            CompareAtPrice = p.CompareAtPrice,
            CostPrice = p.CostPrice,
            StockQuantity = p.StockQuantity,
            TrackInventory = p.TrackInventory,
            LowStockThreshold = p.LowStockThreshold,
            IsFeatured = p.IsFeatured,
            IsActive = p.IsActive,
            SortOrder = p.SortOrder,
            Categories = await GetCategoriesSelectList(cancellationToken),
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ProductFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Categories = await GetCategoriesSelectList(cancellationToken);
            ViewData["Title"] = "تعديل منتج";
            return View(model);
        }

        var dto = new UpdateProductDto
        {
            Id = id,
            CategoryId = model.CategoryId,
            Name = model.Name,
            NameAr = model.NameAr,
            Slug = model.Slug,
            ShortDescription = model.ShortDescription,
            ShortDescriptionAr = model.ShortDescriptionAr,
            Description = model.Description,
            DescriptionAr = model.DescriptionAr,
            SKU = model.SKU,
            Price = model.Price,
            CompareAtPrice = model.CompareAtPrice,
            CostPrice = model.CostPrice,
            StockQuantity = model.StockQuantity,
            TrackInventory = model.TrackInventory,
            LowStockThreshold = model.LowStockThreshold,
            IsFeatured = model.IsFeatured,
            IsActive = model.IsActive,
            SortOrder = model.SortOrder,
        };

        var result = await _productService.UpdateAsync(dto, cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "حدث خطأ أثناء تعديل المنتج");
            model.Categories = await GetCategoriesSelectList(cancellationToken);
            return View(model);
        }

        TempData["SuccessMessage"] = "تم تعديل المنتج بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _productService.DeleteAsync(id, cancellationToken);

        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] =
            result.Success ? "تم حذف المنتج بنجاح" : (result.Message ?? "حدث خطأ أثناء الحذف");

        return RedirectToAction(nameof(Index));
    }

    private async Task<List<SelectListItem>> GetCategoriesSelectList(CancellationToken ct)
    {
        var result = await _categoryService.GetAllAsync(ct);
        return (result.Data ?? new())
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = string.IsNullOrEmpty(c.NameAr) ? c.Name : $"{c.Name} / {c.NameAr}",
            })
            .ToList();
    }
}
