using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Domain.Platform;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Web.Areas.PlatformAdmin.ViewModels;

namespace MultiTenantStore.Web.Areas.PlatformAdmin.Controllers;

[Area("PlatformAdmin")]
[Authorize(AuthenticationSchemes = "PlatformAdmin")]
public sealed class PlatformPlansController : Controller
{
    private readonly MainDbContext _db;

    public PlatformPlansController(MainDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        string lang = Request.Cookies["admin_lang"] ?? "ar";
        ViewData["Title"] = lang == "ar" ? "خطط الاشتراك" : "Subscription Plans";

        var plans = await _db.SubscriptionPlans
            .AsNoTracking()
            .Where(p => p.DeletedAt == null)
            .OrderBy(p => p.PriceMonthly)
            .Select(p => new PlatformPlanRowViewModel
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                NameAr = p.NameAr,
                PriceMonthly = p.PriceMonthly,
                PriceYearly = p.PriceYearly,
                MaxProducts = p.MaxProducts,
                MaxStaffUsers = p.MaxStaffUsers,
                IsActive = p.IsActive,
                SubscriberCount = p.StoreSubscriptions.Count(s => s.DeletedAt == null),
            })
            .ToListAsync(cancellationToken);

        return View(new PlatformPlansListViewModel { Plans = plans });
    }

    [HttpGet]
    public IActionResult Create()
    {
        string lang = Request.Cookies["admin_lang"] ?? "ar";
        ViewData["Title"] = lang == "ar" ? "إضافة خطة" : "Add Plan";
        return View("Form", new PlatformPlanFormViewModel());
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        string lang = Request.Cookies["admin_lang"] ?? "ar";
        ViewData["Title"] = lang == "ar" ? "تعديل الخطة" : "Edit Plan";

        var plan = await _db.SubscriptionPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null, cancellationToken);

        if (plan is null) return NotFound();

        return View("Form", new PlatformPlanFormViewModel
        {
            Id = plan.Id,
            Code = plan.Code,
            Name = plan.Name,
            NameAr = plan.NameAr,
            PriceMonthly = plan.PriceMonthly,
            PriceYearly = plan.PriceYearly,
            MaxProducts = plan.MaxProducts,
            MaxStaffUsers = plan.MaxStaffUsers,
            IsActive = plan.IsActive,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(PlatformPlanFormViewModel vm, CancellationToken cancellationToken)
    {
        string lang = Request.Cookies["admin_lang"] ?? "ar";

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = vm.Id.HasValue
                ? (lang == "ar" ? "تعديل الخطة" : "Edit Plan")
                : (lang == "ar" ? "إضافة خطة" : "Add Plan");
            return View("Form", vm);
        }

        if (vm.Id.HasValue)
        {
            var plan = await _db.SubscriptionPlans
                .FirstOrDefaultAsync(p => p.Id == vm.Id.Value && p.DeletedAt == null, cancellationToken);
            if (plan is null) return NotFound();

            plan.Code = vm.Code;
            plan.Name = vm.Name;
            plan.NameAr = vm.NameAr;
            plan.PriceMonthly = vm.PriceMonthly;
            plan.PriceYearly = vm.PriceYearly;
            plan.MaxProducts = vm.MaxProducts;
            plan.MaxStaffUsers = vm.MaxStaffUsers;
            plan.IsActive = vm.IsActive;
        }
        else
        {
            _db.SubscriptionPlans.Add(new SubscriptionPlan
            {
                Code = vm.Code,
                Name = vm.Name,
                NameAr = vm.NameAr,
                PriceMonthly = vm.PriceMonthly,
                PriceYearly = vm.PriceYearly,
                MaxProducts = vm.MaxProducts,
                MaxStaffUsers = vm.MaxStaffUsers,
                IsActive = vm.IsActive,
            });
        }

        await _db.SaveChangesAsync(cancellationToken);

        TempData["SuccessMessage"] = lang == "ar"
            ? "تم حفظ الخطة بنجاح"
            : "Plan saved successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id, CancellationToken cancellationToken)
    {
        var plan = await _db.SubscriptionPlans
            .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null, cancellationToken);

        if (plan is null) return NotFound();

        plan.IsActive = !plan.IsActive;
        await _db.SaveChangesAsync(cancellationToken);

        string lang = Request.Cookies["admin_lang"] ?? "ar";
        TempData["SuccessMessage"] = plan.IsActive
            ? (lang == "ar" ? "تم تفعيل الخطة" : "Plan activated.")
            : (lang == "ar" ? "تم إيقاف الخطة" : "Plan deactivated.");

        return RedirectToAction(nameof(Index));
    }
}
