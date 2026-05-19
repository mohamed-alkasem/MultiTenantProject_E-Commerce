using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Application.Common.MultiTenancy;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Web.Areas.Dashboard.ViewModels;

namespace MultiTenantStore.Web.Areas.Dashboard.Controllers;

[Area("Dashboard")]
[Authorize(AuthenticationSchemes = "Identity.Application")]
public sealed class DashboardSubscriptionController : Controller
{
    private readonly MainDbContext _db;

    public DashboardSubscriptionController(MainDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "اشتراكي";

        var storeIdStr = User.FindFirstValue(TenantClaimTypes.StoreId);
        if (!Guid.TryParse(storeIdStr, out var storeId))
            return Unauthorized();

        var subscription = await _db.StoreSubscriptions
            .AsNoTracking()
            .Include(s => s.Plan)
            .Where(s => s.StoreId == storeId && s.DeletedAt == null)
            .OrderByDescending(s => s.StartDate)
            .FirstOrDefaultAsync(cancellationToken);

        var vm = new SubscriptionViewModel
        {
            HasSubscription = subscription is not null,
            PlanName = subscription?.Plan?.Name ?? "—",
            PlanNameAr = subscription?.Plan?.NameAr,
            PlanCode = subscription?.Plan?.Code ?? "",
            Status = subscription?.Status.ToString() ?? "—",
            BillingCycle = subscription?.BillingCycle.ToString() ?? "—",
            StartDate = subscription?.StartDate,
            EndDate = subscription?.EndDate,
            TrialEndsAt = subscription?.TrialEndsAt,
            NextBillingAt = subscription?.NextBillingAt,
            MaxProducts = subscription?.Plan?.MaxProducts ?? 0,
            MaxStaffUsers = subscription?.Plan?.MaxStaffUsers ?? 0,
            PriceMonthly = subscription?.Plan?.PriceMonthly ?? 0,
            PriceYearly = subscription?.Plan?.PriceYearly ?? 0,
        };

        return View(vm);
    }
}
