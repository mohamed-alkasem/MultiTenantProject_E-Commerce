using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.Carts.Services;
using MultiTenantStore.Application.Common.MultiTenancy;
using MultiTenantStore.Application.Customers.DTOs;
using MultiTenantStore.Application.Customers.Services;
using MultiTenantStore.Application.Orders.Services;
using MultiTenantStore.Application.Orders.DTOs;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Web.Areas.Storefront.ViewModels;

namespace MultiTenantStore.Web.Areas.Storefront.Controllers;

public sealed class CustomerController : StorefrontBaseController
{
    private readonly ICustomerAuthService _auth;
    private readonly ICustomerOrderService _orders;

    public CustomerController(
        ITenantStore tenantStore,
        ICurrentTenant currentTenant,
        MainDbContext mainDb,
        ICartService cartService,
        ICustomerAuthService auth,
        ICustomerOrderService orders)
        : base(tenantStore, currentTenant, mainDb, cartService)
    {
        _auth = auth;
        _orders = orders;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null) =>
        View(new CustomerLoginViewModel { Store = GetStoreCtx(), ReturnUrl = returnUrl });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(CustomerLoginViewModel model, CancellationToken ct)
    {
        model.Store = GetStoreCtx();
        if (!ModelState.IsValid) return View(model);

        var result = await _auth.LoginAsync(new CustomerLoginDto
        {
            Email = model.Email,
            Password = model.Password
        }, ct);

        if (!result.Success || result.Data is null)
        {
            ModelState.AddModelError("", model.Store.IsAr ? "البريد الإلكتروني أو كلمة المرور غير صحيحة" : "Invalid email or password");
            return View(model);
        }

        StoreCustomerSession(result.Data);

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        return RedirectToAction(nameof(Orders));
    }

    [HttpGet]
    public IActionResult Register() =>
        View(new CustomerRegisterViewModel { Store = GetStoreCtx() });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(CustomerRegisterViewModel model, CancellationToken ct)
    {
        model.Store = GetStoreCtx();
        if (!ModelState.IsValid) return View(model);

        if (model.Password != model.ConfirmPassword)
        {
            ModelState.AddModelError("ConfirmPassword", model.Store.IsAr ? "كلمتا المرور غير متطابقتين" : "Passwords do not match");
            return View(model);
        }

        var result = await _auth.RegisterAsync(new RegisterCustomerDto
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Password = model.Password,
            ConfirmPassword = model.ConfirmPassword,
            PhoneNumber = model.PhoneNumber
        }, ct);

        if (!result.Success || result.Data is null)
        {
            ModelState.AddModelError("", result.Message ?? (model.Store.IsAr ? "حدث خطأ أثناء إنشاء الحساب" : "Registration failed"));
            return View(model);
        }

        StoreCustomerSession(result.Data);
        return RedirectToAction(nameof(Orders));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public async Task<IActionResult> Orders(CancellationToken ct)
    {
        var customerId = GetCustomerId();
        if (!customerId.HasValue)
            return RedirectToAction(nameof(Login));

        var storeCtx = GetStoreCtx();
        var result = await _orders.GetMyOrdersAsync(ct);

        return View(new CustomerOrdersViewModel
        {
            Store = storeCtx,
            Orders = result.Data ?? new List<OrderListDto>(),
            CustomerName = GetCustomerName() ?? "",
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SetLanguage(string lang, string? returnUrl = null)
    {
        Response.Cookies.Append("store_lang", lang == "en" ? "en" : "ar", new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            HttpOnly = false,
            SameSite = SameSiteMode.Lax
        });
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
        return Redirect(Request.Headers.Referer.FirstOrDefault() ?? "/");
    }

    private void StoreCustomerSession(CustomerAuthResponseDto data)
    {
        HttpContext.Session.SetString("CustomerId", data.CustomerId.ToString());
        HttpContext.Session.SetString("CustomerName", $"{data.FirstName} {data.LastName}".Trim());
        HttpContext.Session.SetString("CustomerFirstName", data.FirstName);
        HttpContext.Session.SetString("CustomerLastName", data.LastName);
        HttpContext.Session.SetString("CustomerEmail", data.Email);
    }
}
