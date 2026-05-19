using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Domain.Enums;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Web.Areas.PlatformAdmin.ViewModels;

namespace MultiTenantStore.Web.Areas.PlatformAdmin.Controllers;

[Area("PlatformAdmin")]
[Authorize(AuthenticationSchemes = "PlatformAdmin")]
public sealed class PlatformDashboardController : Controller
{
    private readonly MainDbContext _db;

    public PlatformDashboardController(MainDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var stores = await _db.Stores
            .AsNoTracking()
            .Where(s => s.DeletedAt == null)
            .Include(s => s.OwnerUser)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);

        var merchantRoleId = await _db.Roles
            .Where(r => r.Name == "Merchant")
            .Select(r => r.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var totalMerchants = merchantRoleId != Guid.Empty
            ? await _db.UserRoles.CountAsync(ur => ur.RoleId == merchantRoleId, cancellationToken)
            : 0;

        var vm = new PlatformDashboardViewModel
        {
            TotalStores = stores.Count,
            ActiveStores = stores.Count(s => s.Status == StoreStatus.Active),
            PendingStores = stores.Count(s => s.Status == StoreStatus.PendingProvisioning),
            SuspendedStores = stores.Count(s => s.Status == StoreStatus.Suspended),
            TotalMerchants = totalMerchants,
            RecentStores = stores.Take(8).Select(s => new PlatformStoreRowViewModel
            {
                Id = s.Id,
                StoreName = s.StoreName,
                StoreNameAr = s.StoreNameAr,
                Slug = s.Slug,
                OwnerName = $"{s.OwnerUser.FirstName} {s.OwnerUser.LastName}",
                OwnerEmail = s.OwnerUser.Email ?? "",
                Status = s.Status,
                SubscriptionStatus = s.SubscriptionStatus,
                CreatedAt = s.CreatedAt,
            }).ToList(),
        };

        return View(vm);
    }
}
