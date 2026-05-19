using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MultiTenantStore.Application.Catalog.DTOs;
using MultiTenantStore.Application.Catalog.Services;
using MultiTenantStore.Web.Areas.Dashboard.ViewModels;

namespace MultiTenantStore.Web.Areas.Dashboard.Controllers;

[Area("Dashboard")]
[Authorize(AuthenticationSchemes = "Identity.Application")]
public sealed class DashboardCategoriesController : Controller
{
    private readonly ICategoryService _categoryService;

    public DashboardCategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "الفئات";
        var result = await _categoryService.GetAllAsync(cancellationToken);
        return View(result.Data ?? new());
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "إضافة فئة";
        return View(new CategoryFormViewModel
        {
            ParentCategories = await GetParentCategoriesSelectList(cancellationToken)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.ParentCategories = await GetParentCategoriesSelectList(cancellationToken);
            ViewData["Title"] = "إضافة فئة";
            return View(model);
        }

        var dto = new CreateCategoryDto
        {
            ParentCategoryId = model.ParentCategoryId,
            Name = model.Name,
            NameAr = model.NameAr,
            Slug = model.Slug,
            IsActive = model.IsActive,
            SortOrder = model.SortOrder,
        };

        var result = await _categoryService.CreateAsync(dto, cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "حدث خطأ أثناء إضافة الفئة");
            model.ParentCategories = await GetParentCategoriesSelectList(cancellationToken);
            return View(model);
        }

        TempData["SuccessMessage"] = $"تم إضافة الفئة \"{model.Name}\" بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "تعديل فئة";

        var result = await _categoryService.GetByIdAsync(id, cancellationToken);

        if (!result.Success || result.Data is null)
        {
            TempData["ErrorMessage"] = "الفئة غير موجودة";
            return RedirectToAction(nameof(Index));
        }

        var c = result.Data;
        var model = new CategoryFormViewModel
        {
            Id = c.Id,
            ParentCategoryId = c.ParentCategoryId,
            Name = c.Name,
            NameAr = c.NameAr,
            Slug = c.Slug,
            IsActive = c.IsActive,
            SortOrder = c.SortOrder,
            ParentCategories = await GetParentCategoriesSelectList(cancellationToken, excludeId: id),
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CategoryFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.ParentCategories = await GetParentCategoriesSelectList(cancellationToken, excludeId: id);
            ViewData["Title"] = "تعديل فئة";
            return View(model);
        }

        var dto = new UpdateCategoryDto
        {
            Id = id,
            ParentCategoryId = model.ParentCategoryId,
            Name = model.Name,
            NameAr = model.NameAr,
            Slug = model.Slug,
            IsActive = model.IsActive,
            SortOrder = model.SortOrder,
        };

        var result = await _categoryService.UpdateAsync(dto, cancellationToken);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "حدث خطأ أثناء تعديل الفئة");
            model.ParentCategories = await GetParentCategoriesSelectList(cancellationToken, excludeId: id);
            return View(model);
        }

        TempData["SuccessMessage"] = "تم تعديل الفئة بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _categoryService.DeleteAsync(id, cancellationToken);

        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] =
            result.Success ? "تم حذف الفئة بنجاح" : (result.Message ?? "حدث خطأ أثناء الحذف");

        return RedirectToAction(nameof(Index));
    }

    private async Task<List<SelectListItem>> GetParentCategoriesSelectList(
        CancellationToken ct, Guid? excludeId = null)
    {
        var result = await _categoryService.GetAllAsync(ct);
        return (result.Data ?? new())
            .Where(c => excludeId == null || c.Id != excludeId)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = string.IsNullOrEmpty(c.NameAr) ? c.Name : $"{c.Name} / {c.NameAr}",
            })
            .ToList();
    }
}
