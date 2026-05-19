using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Application.Common.MultiTenancy;
using MultiTenantStore.Domain.Platform;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Web.Areas.Dashboard.ViewModels;

namespace MultiTenantStore.Web.Areas.Dashboard.Controllers;

[Area("Dashboard")]
[Authorize(AuthenticationSchemes = "Identity.Application")]
public sealed class DashboardBrandingController : Controller
{
    private readonly MainDbContext _db;
    private readonly IWebHostEnvironment _env;

    public DashboardBrandingController(MainDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "هوية المتجر";

        var storeId = GetStoreId();
        if (storeId is null) return Unauthorized();

        var branding = await _db.StoreBrandings
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.StoreId == storeId.Value && b.DeletedAt == null, cancellationToken);

        var vm = branding is null
            ? new BrandingViewModel()
            : new BrandingViewModel
            {
                LogoUrl = branding.LogoUrl,
                PrimaryColor = branding.PrimaryColor,
                SecondaryColor = branding.SecondaryColor,
                ContactEmail = branding.ContactEmail,
                ContactPhone = branding.ContactPhone,
                Address = branding.Address,
            };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(BrandingViewModel model, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "هوية المتجر";

        var storeId = GetStoreId();
        if (storeId is null) return Unauthorized();

        if (model.LogoFile is { Length: > 0 })
        {
            var ext = Path.GetExtension(model.LogoFile.FileName).ToLowerInvariant();
            if (ext is not (".jpg" or ".jpeg" or ".png" or ".webp" or ".svg"))
            {
                ModelState.AddModelError("LogoFile", "صيغة الصورة غير مدعومة. الصيغ المقبولة: jpg, png, webp, svg");
                return View("Index", model);
            }

            if (model.LogoFile.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("LogoFile", "حجم الصورة يجب ألا يتجاوز 2 ميجابايت");
                return View("Index", model);
            }

            var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "logos");
            Directory.CreateDirectory(uploadDir);

            var fileName = $"{storeId}-{Guid.NewGuid():N}{ext}";
            var filePath = Path.Combine(uploadDir, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await model.LogoFile.CopyToAsync(stream, cancellationToken);

            model.LogoUrl = $"/uploads/logos/{fileName}";
        }

        var branding = await _db.StoreBrandings
            .FirstOrDefaultAsync(b => b.StoreId == storeId.Value && b.DeletedAt == null, cancellationToken);

        if (branding is null)
        {
            branding = new StoreBranding { StoreId = storeId.Value };
            _db.StoreBrandings.Add(branding);
        }

        if (model.LogoUrl is not null)
            branding.LogoUrl = model.LogoUrl;

        branding.PrimaryColor = model.PrimaryColor;
        branding.SecondaryColor = model.SecondaryColor;
        branding.ContactEmail = model.ContactEmail;
        branding.ContactPhone = model.ContactPhone;
        branding.Address = model.Address;

        await _db.SaveChangesAsync(cancellationToken);

        TempData["SuccessMessage"] = "تم حفظ هوية المتجر بنجاح";
        return RedirectToAction(nameof(Index));
    }

    private Guid? GetStoreId()
    {
        var claim = User.FindFirstValue(TenantClaimTypes.StoreId);
        return Guid.TryParse(claim, out var id) ? id : null;
    }
}
