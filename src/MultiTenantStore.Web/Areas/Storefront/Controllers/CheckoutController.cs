using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.Carts.Services;
using MultiTenantStore.Application.Checkout.Services;
using MultiTenantStore.Application.Common.MultiTenancy;
using MultiTenantStore.Application.Customers.Services;
using MultiTenantStore.Application.Invoices.DTOs;
using MultiTenantStore.Application.Invoices.Services;
using MultiTenantStore.Application.Orders.DTOs;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Web.Areas.Storefront.ViewModels;

namespace MultiTenantStore.Web.Areas.Storefront.Controllers;

public sealed class CheckoutController : StorefrontBaseController
{
    private readonly ICheckoutService _checkout;
    private readonly IInvoiceService _invoiceService;
    private readonly ICustomerAddressService _addressService;

    public CheckoutController(
        ITenantStore tenantStore,
        ICurrentTenant currentTenant,
        MainDbContext mainDb,
        ICartService cartService,
        ICheckoutService checkout,
        IInvoiceService invoiceService,
        ICustomerAddressService addressService)
        : base(tenantStore, currentTenant, mainDb, cartService)
    {
        _checkout = checkout;
        _invoiceService = invoiceService;
        _addressService = addressService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var sessionId = GetSessionId();
        var cartResult = await CartService.GetCartAsync(sessionId, ct);
        var cart = cartResult.Data;

        if (cart is null || !cart.Items.Any())
            return RedirectToAction("Index", "Cart");

        var vm = new CheckoutViewModel { Store = GetStoreCtx(), Cart = cart };

        var customerId = GetCustomerId();
        if (customerId.HasValue)
        {
            vm.FirstName = HttpContext.Session.GetString("CustomerFirstName") ?? "";
            vm.LastName = HttpContext.Session.GetString("CustomerLastName") ?? "";
            vm.Email = HttpContext.Session.GetString("CustomerEmail") ?? "";

            // Pre-fill address from default shipping address
            try
            {
                var addressesResult = await _addressService.GetAddressesAsync(ct);
                var defaultAddr = addressesResult.Data?
                    .FirstOrDefault(a => a.IsDefaultShipping)
                    ?? addressesResult.Data?.FirstOrDefault();

                if (defaultAddr is not null)
                {
                    vm.Phone = defaultAddr.PhoneNumber ?? "";
                    vm.Country = defaultAddr.Country;
                    vm.City = defaultAddr.City;
                    vm.District = defaultAddr.District;
                    vm.AddressLine1 = defaultAddr.AddressLine1;
                    vm.AddressLine2 = defaultAddr.AddressLine2;
                    vm.PostalCode = defaultAddr.PostalCode;
                }
            }
            catch { /* address pre-fill is non-critical */ }
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(CheckoutViewModel model, CancellationToken ct)
    {
        var sessionId = GetSessionId();
        var cartResult = await CartService.GetCartAsync(sessionId, ct);
        var cart = cartResult.Data;
        model.Store = GetStoreCtx();
        model.Cart = cart;

        if (cart is null || !cart.Items.Any())
        {
            ModelState.AddModelError("", model.Store.IsAr ? "السلة فارغة" : "Cart is empty");
            return View("Index", model);
        }

        if (!ModelState.IsValid)
            return View("Index", model);

        var shippingAddress = new CreateOrderAddressDto
        {
            FullName = $"{model.FirstName} {model.LastName}".Trim(),
            Phone = model.Phone,
            Country = model.Country,
            City = model.City,
            District = model.District,
            AddressLine1 = model.AddressLine1,
            AddressLine2 = model.AddressLine2,
            PostalCode = model.PostalCode,
        };

        var billingAddress = model.SameBillingAddress ? shippingAddress : shippingAddress;

        var dto = new CreateOrderDto
        {
            CustomerId = GetCustomerId(),
            CartId = cart.Id,
            Items = cart.Items.Select(i => new CreateOrderItemDto
            {
                ProductId = i.ProductId,
                ProductVariantId = i.ProductVariantId,
                Quantity = i.Quantity
            }).ToList(),
            ShippingAddress = shippingAddress,
            BillingAddress = billingAddress,
            PaymentMethod = model.PaymentMethod,
        };

        var result = await _checkout.CreateOrderAsync(dto, ct);

        if (!result.Success || result.Data is null)
        {
            ModelState.AddModelError("", result.Message ?? (model.Store.IsAr ? "حدث خطأ أثناء إنشاء الطلب" : "Error creating order"));
            return View("Index", model);
        }

        await CartService.ClearCartAsync(sessionId, ct);

        // Create invoice and generate PDF
        string? invoiceNumber = null;
        string? invoicePdfUrl = null;
        try
        {
            var invoiceResult = await _invoiceService.CreateForOrderAsync(
                new CreateInvoiceDto { OrderId = result.Data.Id }, ct);

            if (invoiceResult.Success && invoiceResult.Data is not null)
            {
                invoiceNumber = invoiceResult.Data.InvoiceNumber;
                var pdfResult = await _invoiceService.GenerateAndUploadPdfAsync(invoiceResult.Data.Id, ct);
                if (pdfResult.Success && pdfResult.Data is not null)
                    invoicePdfUrl = pdfResult.Data.PdfUrl;
            }
        }
        catch { /* invoice failure must not block order confirmation */ }

        return View("Confirmation", new OrderConfirmationViewModel
        {
            Store = model.Store,
            OrderNumber = result.Data.OrderNumber,
            TotalAmount = result.Data.TotalAmount,
            Currency = result.Data.Currency,
            InvoiceNumber = invoiceNumber,
            InvoicePdfUrl = invoicePdfUrl,
        });
    }
}
