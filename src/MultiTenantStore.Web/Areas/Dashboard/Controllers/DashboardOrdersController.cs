using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.Orders.DTOs;
using MultiTenantStore.Application.Orders.Services;
using MultiTenantStore.Web.Areas.Dashboard.ViewModels;

namespace MultiTenantStore.Web.Areas.Dashboard.Controllers;

[Area("Dashboard")]
[Authorize(AuthenticationSchemes = "Identity.Application")]
public sealed class DashboardOrdersController : Controller
{
    private readonly IOrderService _orderService;

    public DashboardOrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IActionResult> Index(
        string? search, string? status, int page = 1, CancellationToken cancellationToken = default)
    {
        ViewData["Title"] = "الطلبات";

        var request = new OrderSearchRequestDto
        {
            Search = search,
            Status = status,
            PageNumber = page,
            PageSize = 20,
        };

        var result = await _orderService.GetOrdersAsync(request, cancellationToken);

        var vm = new OrdersListViewModel
        {
            Orders = result.Data?.Items ?? new(),
            TotalCount = result.Data?.TotalCount ?? 0,
            PageNumber = page,
            PageSize = 20,
            Search = search,
            StatusFilter = status,
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "تفاصيل الطلب";

        var result = await _orderService.GetByIdAsync(id, cancellationToken);

        if (!result.Success || result.Data is null)
        {
            TempData["ErrorMessage"] = "الطلب غير موجود";
            return RedirectToAction(nameof(Index));
        }

        return View(new OrderDetailsViewModel { Order = result.Data });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(Guid id, string newStatus, CancellationToken cancellationToken)
    {
        var result = await _orderService.UpdateOrderStatusAsync(
            new UpdateOrderStatusDto { OrderId = id, Status = newStatus }, cancellationToken);

        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] =
            result.Success ? "تم تحديث حالة الطلب بنجاح" : (result.Message ?? "حدث خطأ");

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePaymentStatus(
        Guid id, string newStatus, CancellationToken cancellationToken)
    {
        var result = await _orderService.UpdatePaymentStatusAsync(
            new UpdatePaymentStatusDto { OrderId = id, PaymentStatus = newStatus }, cancellationToken);

        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] =
            result.Success ? "تم تحديث حالة الدفع بنجاح" : (result.Message ?? "حدث خطأ");

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateShippingStatus(
        Guid id, string newStatus, CancellationToken cancellationToken)
    {
        var result = await _orderService.UpdateShippingStatusAsync(
            new UpdateShippingStatusDto { OrderId = id, ShippingStatus = newStatus }, cancellationToken);

        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] =
            result.Success ? "تم تحديث حالة الشحن بنجاح" : (result.Message ?? "حدث خطأ");

        return RedirectToAction(nameof(Details), new { id });
    }
}
