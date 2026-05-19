using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Application.Stores.Interfaces;
using MultiTenantStore.Domain.Enums;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Web.Areas.PlatformAdmin.ViewModels;

namespace MultiTenantStore.Web.Areas.PlatformAdmin.Controllers;

[Area("PlatformAdmin")]
[Authorize(AuthenticationSchemes = "PlatformAdmin")]
public sealed class PlatformStoresController : Controller
{
    private readonly MainDbContext _db;
    private readonly IStoreDatabaseService _storeDatabaseService;

    public PlatformStoresController(MainDbContext db, IStoreDatabaseService storeDatabaseService)
    {
        _db = db;
        _storeDatabaseService = storeDatabaseService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? search,
        string? status,
        CancellationToken cancellationToken)
    {
        var query = _db.Stores
            .AsNoTracking()
            .Include(s => s.OwnerUser)
            .Include(s => s.Database)
            .Where(s => s.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s =>
                s.StoreName.Contains(search) ||
                s.Slug.Contains(search) ||
                s.OwnerUser.Email!.Contains(search));

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<StoreStatus>(status, out var storeStatus))
            query = query.Where(s => s.Status == storeStatus);

        var stores = await query
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);

        var vm = new PlatformStoresListViewModel
        {
            Stores = stores.Select(s => new PlatformStoreRowViewModel
            {
                Id = s.Id,
                StoreName = s.StoreName,
                StoreNameAr = s.StoreNameAr,
                Slug = s.Slug,
                OwnerName = $"{s.OwnerUser.FirstName} {s.OwnerUser.LastName}",
                OwnerEmail = s.OwnerUser.Email ?? "",
                Status = s.Status,
                SubscriptionStatus = s.SubscriptionStatus,
                HasDatabase = s.Database?.IsProvisioned == true,
                CreatedAt = s.CreatedAt,
            }).ToList(),
            Search = search,
            StatusFilter = status,
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var store = await _db.Stores
            .AsNoTracking()
            .Include(s => s.OwnerUser)
            .Include(s => s.Database)
            .Include(s => s.Domains)
            .Include(s => s.Subscriptions.Where(sub => sub.DeletedAt == null))
                .ThenInclude(sub => sub.Plan)
            .Include(s => s.Branding)
            .FirstOrDefaultAsync(s => s.Id == id && s.DeletedAt == null, cancellationToken);

        if (store is null)
            return NotFound();

        var activeSubscription = store.Subscriptions
            .Where(s => s.DeletedAt == null)
            .OrderByDescending(s => s.StartDate)
            .FirstOrDefault();

        var vm = new StoreDetailsViewModel
        {
            Id = store.Id,
            StoreName = store.StoreName,
            StoreNameAr = store.StoreNameAr,
            Slug = store.Slug,
            Status = store.Status,
            SubscriptionStatus = store.SubscriptionStatus,
            OwnerName = $"{store.OwnerUser.FirstName} {store.OwnerUser.LastName}",
            OwnerEmail = store.OwnerUser.Email ?? "",
            CreatedAt = store.CreatedAt,
            ActivatedAt = store.ActivatedAt,
            SuspendedAt = store.SuspendedAt,
            HasDatabase = store.Database?.IsProvisioned == true,
            DatabaseName = store.Database?.DatabaseName,
            ProvisioningStatus = store.Database?.ProvisioningStatus.ToString(),
            Domains = store.Domains.Select(d => d.FullDomain).ToList(),
            ActiveSubscription = activeSubscription is null ? null : new ActiveSubscriptionViewModel
            {
                PlanName = activeSubscription.Plan?.Name ?? "—",
                Status = activeSubscription.Status.ToString(),
                BillingCycle = activeSubscription.BillingCycle.ToString(),
                StartDate = activeSubscription.StartDate,
                EndDate = activeSubscription.EndDate,
            },
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        var store = await _db.Stores
            .Include(s => s.Database)
            .FirstOrDefaultAsync(s => s.Id == id && s.DeletedAt == null, cancellationToken);

        if (store is null)
            return NotFound();

        if (store.Database?.IsProvisioned != true)
        {
            var result = await _storeDatabaseService.ProvisionDatabaseAsync(id, cancellationToken);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message ?? "فشل إنشاء قاعدة البيانات / Database provisioning failed.";
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["SuccessMessage"] = "تم إنشاء قاعدة البيانات وتفعيل المتجر بنجاح / Database provisioned and store activated.";
            return RedirectToAction(nameof(Details), new { id });
        }

        store.Status = StoreStatus.Active;
        store.ActivatedAt ??= DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        TempData["SuccessMessage"] = "تم تفعيل المتجر بنجاح / Store activated successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Suspend(Guid id, CancellationToken cancellationToken)
    {
        var store = await _db.Stores
            .FirstOrDefaultAsync(s => s.Id == id && s.DeletedAt == null, cancellationToken);

        if (store is null)
            return NotFound();

        store.Status = StoreStatus.Suspended;
        store.SuspendedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        TempData["SuccessMessage"] = "تم تعليق المتجر / Store suspended.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
