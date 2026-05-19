using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Application.Carts.Services;
using MultiTenantStore.Application.Common.MultiTenancy;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Web.Areas.Storefront.ViewModels;

namespace MultiTenantStore.Web.Areas.Storefront.Controllers;

[Area("Storefront")]
public abstract class StorefrontBaseController : Controller
{
    protected readonly ITenantStore TenantStore;
    protected readonly ICurrentTenant CurrentTenant;
    protected readonly MainDbContext MainDb;
    protected readonly ICartService CartService;

    protected StorefrontBaseController(
        ITenantStore tenantStore,
        ICurrentTenant currentTenant,
        MainDbContext mainDb,
        ICartService cartService)
    {
        TenantStore = tenantStore;
        CurrentTenant = currentTenant;
        MainDb = mainDb;
        CartService = cartService;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!CurrentTenant.IsResolved)
        {
            if (context.RouteData.Values.TryGetValue("storeSlug", out var slugObj)
                && slugObj is string slug && !string.IsNullOrWhiteSpace(slug))
            {
                var tenant = await TenantStore.FindBySubdomainAsync(slug, context.HttpContext.RequestAborted);
                if (tenant is null || !tenant.IsActive)
                {
                    context.Result = NotFound();
                    return;
                }
                CurrentTenant.SetTenant(tenant);
            }
            else
            {
                context.Result = NotFound();
                return;
            }
        }

        await BuildStorefrontContextAsync(context.HttpContext);
        await next();
    }

    private async Task BuildStorefrontContextAsync(HttpContext httpContext)
    {
        var lang = httpContext.Request.Cookies["store_lang"] ?? "ar";

        var branding = await MainDb.StoreBrandings
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.StoreId == CurrentTenant.StoreId && b.DeletedAt == null);

        var cartCount = 0;
        var sessionId = GetSessionId();
        try
        {
            var cartResult = await CartService.GetCartAsync(sessionId);
            cartCount = cartResult.Data?.Items?.Sum(i => i.Quantity) ?? 0;
        }
        catch { /* cart not critical */ }

        var customerName = httpContext.Session.GetString("CustomerName");
        var customerId = httpContext.Session.GetString("CustomerId");

        var ctx = new StorefrontContext
        {
            StoreSlug = CurrentTenant.Slug ?? "",
            StoreName = CurrentTenant.StoreName ?? "",
            LogoUrl = branding?.LogoUrl,
            PrimaryColor = branding?.PrimaryColor ?? "#3498db",
            SecondaryColor = branding?.SecondaryColor ?? "#2ecc71",
            Lang = lang,
            CartCount = cartCount,
            IsCustomerLoggedIn = !string.IsNullOrEmpty(customerId),
            CustomerName = customerName,
        };

        ViewData["StorefrontContext"] = ctx;
    }

    protected StorefrontContext GetStoreCtx() =>
        ViewData["StorefrontContext"] as StorefrontContext ?? new StorefrontContext();

    protected string GetSessionId()
    {
        const string key = "cart_sid";
        if (!Request.Cookies.TryGetValue(key, out var sid) || string.IsNullOrEmpty(sid))
        {
            sid = Guid.NewGuid().ToString("N");
            Response.Cookies.Append(key, sid, new CookieOptions
            {
                MaxAge = TimeSpan.FromDays(30),
                HttpOnly = true,
                SameSite = SameSiteMode.Lax
            });
        }
        return sid;
    }

    protected Guid? GetCustomerId()
    {
        var val = HttpContext.Session.GetString("CustomerId");
        return Guid.TryParse(val, out var id) ? id : null;
    }

    protected string? GetCustomerName() => HttpContext.Session.GetString("CustomerName");

    protected string StoreUrl(string controller, string action, object? routeValues = null)
    {
        var slug = CurrentTenant.Slug ?? "";
        var dict = new RouteValueDictionary(routeValues ?? new { });
        dict["area"] = "Storefront";
        dict["storeSlug"] = slug;
        dict["controller"] = controller;
        dict["action"] = action;
        return Url.RouteUrl("storefront_dev", dict) ?? "/";
    }
}
