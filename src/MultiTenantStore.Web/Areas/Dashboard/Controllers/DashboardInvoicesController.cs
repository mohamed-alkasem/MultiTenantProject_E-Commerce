using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.Invoices.Services;
using MultiTenantStore.Web.Areas.Dashboard.ViewModels;

namespace MultiTenantStore.Web.Areas.Dashboard.Controllers;

[Area("Dashboard")]
[Authorize(AuthenticationSchemes = "Identity.Application")]
public sealed class DashboardInvoicesController : Controller
{
    private readonly IInvoiceService _invoiceService;

    public DashboardInvoicesController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "الفواتير";
        var result = await _invoiceService.GetAllAsync(cancellationToken);
        return View(result.Data ?? new());
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "تفاصيل الفاتورة";

        var result = await _invoiceService.GetByIdAsync(id, cancellationToken);

        if (!result.Success || result.Data is null)
        {
            TempData["ErrorMessage"] = "الفاتورة غير موجودة";
            return RedirectToAction(nameof(Index));
        }

        return View(new InvoiceDetailsViewModel { Invoice = result.Data });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GeneratePdf(Guid id, CancellationToken cancellationToken)
    {
        var result = await _invoiceService.GenerateAndUploadPdfAsync(id, cancellationToken);

        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] =
            result.Success ? "تم إنشاء ملف PDF بنجاح" : (result.Message ?? "حدث خطأ");

        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> DownloadPdf(Guid id, CancellationToken cancellationToken)
    {
        var result = await _invoiceService.GeneratePdfAsync(id, cancellationToken);

        if (!result.Success || result.Data is null)
        {
            TempData["ErrorMessage"] = "تعذّر تحميل الفاتورة";
            return RedirectToAction(nameof(Details), new { id });
        }

        return File(result.Data.Content, result.Data.ContentType, result.Data.FileName);
    }
}
