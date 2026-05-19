using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.Auth.DTOs;
using MultiTenantStore.Application.Auth.Interfaces;
using MultiTenantStore.Application.Common.MultiTenancy;
using MultiTenantStore.Web.Areas.Dashboard.ViewModels;

namespace MultiTenantStore.Web.Areas.Dashboard.Controllers;

[Area("Dashboard")]
public sealed class DashboardAccountController : Controller
{
    private readonly IAuthService _authService;

    public DashboardAccountController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "DashboardHome", new { area = "Dashboard" });

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model,
        string? returnUrl = null,
        CancellationToken cancellationToken = default)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        var authResult = await _authService.LoginAsync(
            new LoginDto { Email = model.Email, Password = model.Password },
            cancellationToken);

        if (!authResult.Success || authResult.Data is null)
        {
            ModelState.AddModelError(string.Empty, "البريد الإلكتروني أو كلمة المرور غير صحيحة");
            return View(model);
        }

        var data = authResult.Data;

        if (!data.StoreId.HasValue)
        {
            ModelState.AddModelError(string.Empty, "هذا الحساب غير مرتبط بأي متجر. يُرجى التواصل مع الدعم.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, $"{data.FirstName} {data.LastName}"),
            new(ClaimTypes.Email, data.Email),
            new(TenantClaimTypes.UserId, data.UserId.ToString()),
            new(TenantClaimTypes.StoreId, data.StoreId.Value.ToString()),
        };

        if (!string.IsNullOrEmpty(data.StoreRole))
            claims.Add(new Claim(TenantClaimTypes.StoreRole, data.StoreRole));

        var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            IdentityConstants.ApplicationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = model.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(30)
                    : DateTimeOffset.UtcNow.AddHours(12),
            });

        var redirectUrl = Url.IsLocalUrl(returnUrl)
            ? returnUrl
            : Url.Action("Index", "DashboardHome", new { area = "Dashboard" })!;

        return Redirect(redirectUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
        return RedirectToAction("Login", "DashboardAccount", new { area = "Dashboard" });
    }
}
