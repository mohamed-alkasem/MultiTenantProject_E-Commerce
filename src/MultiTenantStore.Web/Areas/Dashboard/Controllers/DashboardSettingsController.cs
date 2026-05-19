using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.StoreSettings.DTOs;
using MultiTenantStore.Application.StoreSettings.Services;
using MultiTenantStore.Web.Areas.Dashboard.ViewModels;

namespace MultiTenantStore.Web.Areas.Dashboard.Controllers;

[Area("Dashboard")]
[Authorize(AuthenticationSchemes = "Identity.Application")]
public sealed class DashboardSettingsController : Controller
{
    private readonly IStoreSettingService _storeSettingService;

    public DashboardSettingsController(IStoreSettingService storeSettingService)
    {
        _storeSettingService = storeSettingService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "إعدادات المتجر";

        var result = await _storeSettingService.GetSettingsAsync(cancellationToken);
        var data = result.Data;

        var vm = data is null
            ? new SettingsViewModel()
            : new SettingsViewModel
            {
                Currency = data.Currency,
                Timezone = data.Timezone,
                DefaultLanguage = data.DefaultLanguage,
                IsCheckoutEnabled = data.IsCheckoutEnabled,
                TaxEnabled = data.TaxEnabled,
                OrderPrefix = data.OrderPrefix,
            };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(SettingsViewModel model, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "إعدادات المتجر";

        if (!ModelState.IsValid)
            return View("Index", model);

        var dto = new UpdateStoreSettingDto
        {
            Currency = model.Currency,
            Timezone = model.Timezone,
            DefaultLanguage = model.DefaultLanguage,
            IsCheckoutEnabled = model.IsCheckoutEnabled,
            TaxEnabled = model.TaxEnabled,
            OrderPrefix = model.OrderPrefix,
        };

        var result = await _storeSettingService.UpdateSettingsAsync(dto, cancellationToken);

        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] =
            result.Success ? "تم حفظ الإعدادات بنجاح" : (result.Message ?? "حدث خطأ");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnableCheckout(CancellationToken cancellationToken)
    {
        var result = await _storeSettingService.EnableCheckoutAsync(cancellationToken);
        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] =
            result.Success ? "تم تفعيل الدفع بنجاح" : (result.Message ?? "حدث خطأ");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DisableCheckout(CancellationToken cancellationToken)
    {
        var result = await _storeSettingService.DisableCheckoutAsync(cancellationToken);
        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] =
            result.Success ? "تم تعطيل الدفع بنجاح" : (result.Message ?? "حدث خطأ");
        return RedirectToAction(nameof(Index));
    }
}
