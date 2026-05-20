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
    private readonly ICustomerService _customerService;
    private readonly ICustomerAddressService _addressService;

    public CustomerController(
        ITenantStore tenantStore,
        ICurrentTenant currentTenant,
        MainDbContext mainDb,
        ICartService cartService,
        ICustomerAuthService auth,
        ICustomerOrderService orders,
        ICustomerService customerService,
        ICustomerAddressService addressService)
        : base(tenantStore, currentTenant, mainDb, cartService)
    {
        _auth = auth;
        _orders = orders;
        _customerService = customerService;
        _addressService = addressService;
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

    [HttpGet]
    public async Task<IActionResult> Profile(CancellationToken ct)
    {
        var customerId = GetCustomerId();
        if (!customerId.HasValue) return RedirectToAction(nameof(Login));

        var result = await _customerService.GetProfileAsync(ct);
        var customer = result.Data;

        return View(new CustomerProfileViewModel
        {
            Store = GetStoreCtx(),
            FirstName = customer?.FirstName ?? HttpContext.Session.GetString("CustomerFirstName") ?? "",
            LastName = customer?.LastName ?? HttpContext.Session.GetString("CustomerLastName") ?? "",
            Email = customer?.Email ?? HttpContext.Session.GetString("CustomerEmail") ?? "",
            PhoneNumber = customer?.PhoneNumber,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(CustomerProfileViewModel model, CancellationToken ct)
    {
        model.Store = GetStoreCtx();
        var customerId = GetCustomerId();
        if (!customerId.HasValue) return RedirectToAction(nameof(Login));

        if (string.IsNullOrWhiteSpace(model.FirstName) || string.IsNullOrWhiteSpace(model.LastName))
        {
            TempData["ErrorMessage"] = model.Store.IsAr ? "الاسم مطلوب" : "Name is required";
            return RedirectToAction(nameof(Profile));
        }

        var result = await _customerService.UpdateProfileAsync(new UpdateCustomerDto
        {
            Id = customerId.Value,
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            PhoneNumber = model.PhoneNumber,
            IsActive = true,
        }, ct);

        if (result.Success)
        {
            HttpContext.Session.SetString("CustomerName", $"{model.FirstName} {model.LastName}".Trim());
            HttpContext.Session.SetString("CustomerFirstName", model.FirstName.Trim());
            HttpContext.Session.SetString("CustomerLastName", model.LastName.Trim());
            TempData["SuccessMessage"] = model.Store.IsAr ? "تم تحديث الملف الشخصي" : "Profile updated successfully";
        }
        else
        {
            TempData["ErrorMessage"] = result.Message ?? (model.Store.IsAr ? "حدث خطأ" : "An error occurred");
        }

        return RedirectToAction(nameof(Profile));
    }

    [HttpGet]
    public async Task<IActionResult> Addresses(CancellationToken ct)
    {
        var customerId = GetCustomerId();
        if (!customerId.HasValue) return RedirectToAction(nameof(Login));

        var result = await _addressService.GetAddressesAsync(ct);
        return View(new CustomerAddressesViewModel
        {
            Store = GetStoreCtx(),
            Addresses = result.Data ?? new(),
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAddress(CustomerAddressesViewModel model, CancellationToken ct)
    {
        model.Store = GetStoreCtx();
        var customerId = GetCustomerId();
        if (!customerId.HasValue) return RedirectToAction(nameof(Login));

        if (string.IsNullOrWhiteSpace(model.FullName) || string.IsNullOrWhiteSpace(model.Country) ||
            string.IsNullOrWhiteSpace(model.City) || string.IsNullOrWhiteSpace(model.AddressLine1))
        {
            TempData["ErrorMessage"] = model.Store.IsAr ? "يرجى ملء جميع الحقول المطلوبة" : "Please fill all required fields";
            return RedirectToAction(nameof(Addresses));
        }

        var result = await _addressService.AddAddressAsync(new CreateCustomerAddressDto
        {
            Title = string.IsNullOrWhiteSpace(model.Title) ? (model.Store.IsAr ? "عنوان" : "Address") : model.Title,
            FullName = model.FullName.Trim(),
            PhoneNumber = model.PhoneNumber ?? "",
            Country = model.Country.Trim(),
            City = model.City.Trim(),
            District = model.District,
            AddressLine1 = model.AddressLine1.Trim(),
            AddressLine2 = model.AddressLine2,
            PostalCode = model.PostalCode,
            IsDefaultShipping = model.IsDefaultShipping,
            IsDefaultBilling = model.IsDefaultBilling,
        }, ct);

        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success
            ? (model.Store.IsAr ? "تم إضافة العنوان بنجاح" : "Address added successfully")
            : (result.Message ?? (model.Store.IsAr ? "حدث خطأ" : "An error occurred"));

        return RedirectToAction(nameof(Addresses));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAddress(Guid addressId, CancellationToken ct)
    {
        var storeCtx = GetStoreCtx();
        var customerId = GetCustomerId();
        if (!customerId.HasValue) return RedirectToAction(nameof(Login));

        var result = await _addressService.DeleteAddressAsync(addressId, ct);
        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success
            ? (storeCtx.IsAr ? "تم حذف العنوان" : "Address deleted")
            : (result.Message ?? (storeCtx.IsAr ? "حدث خطأ" : "An error occurred"));

        return RedirectToAction(nameof(Addresses));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetDefaultShipping(Guid addressId, CancellationToken ct)
    {
        var storeCtx = GetStoreCtx();
        var customerId = GetCustomerId();
        if (!customerId.HasValue) return RedirectToAction(nameof(Login));

        var result = await _addressService.SetDefaultShippingAsync(addressId, ct);
        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success
            ? (storeCtx.IsAr ? "تم تعيين عنوان الشحن الافتراضي" : "Default shipping address set")
            : (result.Message ?? (storeCtx.IsAr ? "حدث خطأ" : "An error occurred"));

        return RedirectToAction(nameof(Addresses));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmNewPassword, CancellationToken ct)
    {
        var storeCtx = GetStoreCtx();
        var customerId = GetCustomerId();
        if (!customerId.HasValue) return RedirectToAction(nameof(Login));

        if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmNewPassword))
        {
            TempData["ErrorMessage"] = storeCtx.IsAr ? "يرجى ملء جميع حقول كلمة المرور" : "Please fill all password fields";
            return RedirectToAction(nameof(Profile));
        }

        if (newPassword != confirmNewPassword)
        {
            TempData["ErrorMessage"] = storeCtx.IsAr ? "كلمتا المرور الجديدتان غير متطابقتين" : "New passwords do not match";
            return RedirectToAction(nameof(Profile));
        }

        if (newPassword.Length < 6)
        {
            TempData["ErrorMessage"] = storeCtx.IsAr ? "يجب أن تكون كلمة المرور الجديدة 6 أحرف على الأقل" : "New password must be at least 6 characters";
            return RedirectToAction(nameof(Profile));
        }

        var result = await _customerService.ChangePasswordAsync(currentPassword, newPassword, ct);
        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success
            ? (storeCtx.IsAr ? "تم تغيير كلمة المرور بنجاح" : "Password changed successfully")
            : (result.Message ?? (storeCtx.IsAr ? "حدث خطأ" : "An error occurred"));

        return RedirectToAction(nameof(Profile));
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
