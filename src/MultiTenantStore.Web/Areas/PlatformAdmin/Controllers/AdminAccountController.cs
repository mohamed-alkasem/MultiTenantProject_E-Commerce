using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Domain.Platform;
using MultiTenantStore.Infrastructure.Identity;
using MultiTenantStore.Web.Areas.PlatformAdmin.ViewModels;

namespace MultiTenantStore.Web.Areas.PlatformAdmin.Controllers;

[Area("PlatformAdmin")]
public sealed class AdminAccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminAccountController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Login(string? returnUrl = null)
    {
        var auth = await HttpContext.AuthenticateAsync("PlatformAdmin");
        if (auth.Succeeded)
            return RedirectToAction("Index", "PlatformDashboard", new { area = "PlatformAdmin" });

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        AdminLoginViewModel model,
        string? returnUrl = null,
        CancellationToken cancellationToken = default)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user is null || !user.IsActive || user.DeletedAt is not null)
        {
            ModelState.AddModelError(string.Empty, isAr() ? "البريد الإلكتروني أو كلمة المرور غير صحيحة" : "Invalid email or password.");
            return View(model);
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!passwordValid)
        {
            ModelState.AddModelError(string.Empty, isAr() ? "البريد الإلكتروني أو كلمة المرور غير صحيحة" : "Invalid email or password.");
            return View(model);
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains(IdentityRoleConstants.PlatformAdmin))
        {
            ModelState.AddModelError(string.Empty, isAr() ? "هذا الحساب لا يملك صلاحيات إدارة المنصة" : "This account does not have platform admin privileges.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Role, IdentityRoleConstants.PlatformAdmin),
        };

        var identity = new ClaimsIdentity(claims, "PlatformAdmin");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("PlatformAdmin", principal, new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
        });

        var redirectUrl = Url.IsLocalUrl(returnUrl)
            ? returnUrl
            : Url.Action("Index", "PlatformDashboard", new { area = "PlatformAdmin" })!;

        return Redirect(redirectUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("PlatformAdmin");
        return RedirectToAction("Login", "AdminAccount", new { area = "PlatformAdmin" });
    }

    [HttpPost]
    public IActionResult SetLanguage(string lang, string? returnUrl = null)
    {
        if (lang is not ("ar" or "en"))
            lang = "ar";

        Response.Cookies.Append("admin_lang", lang, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
        });

        if (Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToAction("Index", "PlatformDashboard", new { area = "PlatformAdmin" });
    }

    private bool isAr() => (Request.Cookies["admin_lang"] ?? "ar") == "ar";
}
