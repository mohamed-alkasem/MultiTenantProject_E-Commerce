using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Application.Auth.DTOs;
using MultiTenantStore.Application.Auth.Interfaces;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Web.ViewModels;

namespace MultiTenantStore.Web.Controllers;

public sealed class HomeController : Controller
{
    private readonly IAuthService _authService;
    private readonly MainDbContext _db;

    public HomeController(IAuthService authService, MainDbContext db)
    {
        _authService = authService;
        _db = db;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Register(CancellationToken cancellationToken)
    {
        var plans = await _db.SubscriptionPlans
            .AsNoTracking()
            .Where(p => p.IsActive && p.DeletedAt == null)
            .OrderBy(p => p.PriceMonthly)
            .Select(p => new PlanOptionViewModel { Code = p.Code, Name = p.Name, NameAr = p.NameAr, PriceMonthly = p.PriceMonthly, PriceYearly = p.PriceYearly })
            .ToListAsync(cancellationToken);

        var vm = new RegisterMerchantViewModel { Plans = plans };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterMerchantViewModel vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            vm.Plans = await GetPlansAsync(cancellationToken);
            return View(vm);
        }

        var dto = new RegisterMerchantDto
        {
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            Email = vm.Email,
            Password = vm.Password,
            ConfirmPassword = vm.ConfirmPassword,
            PhoneNumber = vm.PhoneNumber,
            StoreName = vm.StoreName,
            StoreSlug = vm.StoreSlug,
            PlanCode = vm.PlanCode,
            BillingCycle = vm.BillingCycle,
        };

        var result = await _authService.RegisterMerchantAsync(dto, cancellationToken);

        if (!result.Success)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);
            vm.Plans = await GetPlansAsync(cancellationToken);
            return View(vm);
        }

        TempData["RegisterSuccess"] = "true";
        return RedirectToAction(nameof(RegisterSuccess));
    }

    [HttpGet]
    public IActionResult RegisterSuccess()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SetLanguage(string lang, string? returnUrl)
    {
        var cookieOptions = new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            IsEssential = true,
            SameSite = SameSiteMode.Lax
        };
        Response.Cookies.Append("site_lang", lang == "en" ? "en" : "ar", cookieOptions);
        return Redirect(returnUrl ?? "/");
    }

    private async Task<List<PlanOptionViewModel>> GetPlansAsync(CancellationToken cancellationToken)
    {
        return await _db.SubscriptionPlans
            .AsNoTracking()
            .Where(p => p.IsActive && p.DeletedAt == null)
            .OrderBy(p => p.PriceMonthly)
            .Select(p => new PlanOptionViewModel { Code = p.Code, Name = p.Name, NameAr = p.NameAr, PriceMonthly = p.PriceMonthly, PriceYearly = p.PriceYearly })
            .ToListAsync(cancellationToken);
    }
}
