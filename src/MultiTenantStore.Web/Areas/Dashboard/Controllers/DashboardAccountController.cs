using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.Auth.DTOs;
using MultiTenantStore.Application.Auth.Interfaces;
using MultiTenantStore.Application.Common.MultiTenancy;
using MultiTenantStore.Domain.Platform;
using MultiTenantStore.Web.Areas.Dashboard.ViewModels;

namespace MultiTenantStore.Web.Areas.Dashboard.Controllers;

[Area("Dashboard")]
public sealed class DashboardAccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardAccountController(IAuthService authService, UserManager<ApplicationUser> userManager)
    {
        _authService = authService;
        _userManager = userManager;
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

    [HttpGet]
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    public async Task<IActionResult> Profile()
    {
        var userId = User.FindFirstValue(TenantClaimTypes.UserId);
        if (!Guid.TryParse(userId, out var id)) return RedirectToAction(nameof(Login));

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) return RedirectToAction(nameof(Login));

        ViewData["Title"] = "معلومات الحساب";
        return View(new DashboardProfileViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? "",
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    public async Task<IActionResult> UpdateProfile(DashboardProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "معلومات الحساب";
            model.Email = User.FindFirstValue(ClaimTypes.Email) ?? "";
            return View("Profile", model);
        }

        var userId = User.FindFirstValue(TenantClaimTypes.UserId);
        if (!Guid.TryParse(userId, out var id)) return RedirectToAction(nameof(Login));

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) return RedirectToAction(nameof(Login));

        user.FirstName = model.FirstName.Trim();
        user.LastName = model.LastName.Trim();
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] =
            result.Succeeded
                ? "تم تحديث معلومات الحساب بنجاح / Profile updated"
                : string.Join(", ", result.Errors.Select(e => e.Description));

        return RedirectToAction(nameof(Profile));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    public async Task<IActionResult> ChangePassword(DashboardChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "يرجى التحقق من البيانات المدخلة";
            return RedirectToAction(nameof(Profile));
        }

        var userId = User.FindFirstValue(TenantClaimTypes.UserId);
        if (!Guid.TryParse(userId, out var id)) return RedirectToAction(nameof(Login));

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) return RedirectToAction(nameof(Login));

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] =
            result.Succeeded
                ? "تم تغيير كلمة المرور بنجاح / Password changed"
                : string.Join(", ", result.Errors.Select(e => e.Description));

        return RedirectToAction(nameof(Profile));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SetLanguage(string lang, string? returnUrl = null)
    {
        var cookieOptions = new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            IsEssential = true,
            HttpOnly = false,
        };
        Response.Cookies.Append("dash_lang", lang == "en" ? "en" : "ar", cookieOptions);

        var redirect = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Action("Index", "DashboardHome", new { area = "Dashboard" })!;
        return Redirect(redirect);
    }
}
