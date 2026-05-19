using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Web.Areas.PlatformAdmin.ViewModels;

namespace MultiTenantStore.Web.Areas.PlatformAdmin.Controllers;

[Area("PlatformAdmin")]
[Authorize(AuthenticationSchemes = "PlatformAdmin")]
public sealed class PlatformSubscriptionsController : Controller
{
    private readonly MainDbContext _db;

    public PlatformSubscriptionsController(MainDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search, CancellationToken cancellationToken)
    {
        string lang = Request.Cookies["admin_lang"] ?? "ar";
        ViewData["Title"] = lang == "ar" ? "الاشتراكات" : "Subscriptions";

        var query = _db.StoreSubscriptions
            .AsNoTracking()
            .Include(s => s.Plan)
            .Include(s => s.Store)
                .ThenInclude(st => st.OwnerUser)
            .Where(s => s.DeletedAt == null && s.Store.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s =>
                s.Store.StoreName.Contains(search) ||
                s.Store.OwnerUser.Email!.Contains(search) ||
                (s.Plan != null && s.Plan.Name.Contains(search)));

        var subscriptions = await query
            .OrderByDescending(s => s.StartDate)
            .Select(s => new PlatformSubscriptionRowViewModel
            {
                Id = s.Id,
                StoreName = s.Store.StoreName,
                OwnerEmail = s.Store.OwnerUser.Email ?? "",
                PlanName = s.Plan != null ? s.Plan.Name : "—",
                PlanNameAr = s.Plan != null ? s.Plan.NameAr : null,
                Status = s.Status.ToString(),
                BillingCycle = s.BillingCycle.ToString(),
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                NextBillingAt = s.NextBillingAt,
            })
            .ToListAsync(cancellationToken);

        return View(new PlatformSubscriptionsListViewModel
        {
            Subscriptions = subscriptions,
            Search = search,
        });
    }
}
