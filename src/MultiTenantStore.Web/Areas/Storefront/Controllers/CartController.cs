using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.Carts.DTOs;
using MultiTenantStore.Application.Carts.Services;
using MultiTenantStore.Application.Common.MultiTenancy;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Web.Areas.Storefront.ViewModels;

namespace MultiTenantStore.Web.Areas.Storefront.Controllers;

public sealed class CartController : StorefrontBaseController
{
    public CartController(
        ITenantStore tenantStore,
        ICurrentTenant currentTenant,
        MainDbContext mainDb,
        ICartService cartService)
        : base(tenantStore, currentTenant, mainDb, cartService)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var sessionId = GetSessionId();
        var result = await CartService.GetOrCreateCartAsync(sessionId, ct);
        return View(new CartPageViewModel { Store = GetStoreCtx(), Cart = result.Data });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(Guid productId, int quantity = 1, Guid? variantId = null,
        string? returnUrl = null, CancellationToken ct = default)
    {
        var sessionId = GetSessionId();
        await CartService.AddItemAsync(sessionId, new AddToCartDto
        {
            ProductId = productId,
            ProductVariantId = variantId,
            Quantity = quantity < 1 ? 1 : quantity
        }, ct);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Guid cartItemId, int quantity, CancellationToken ct)
    {
        var sessionId = GetSessionId();
        await CartService.UpdateItemQuantityAsync(sessionId, new UpdateCartItemDto
        {
            CartItemId = cartItemId,
            Quantity = quantity
        }, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(Guid cartItemId, CancellationToken ct)
    {
        var sessionId = GetSessionId();
        await CartService.RemoveItemAsync(sessionId, new RemoveCartItemDto { CartItemId = cartItemId }, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Clear(CancellationToken ct)
    {
        var sessionId = GetSessionId();
        await CartService.ClearCartAsync(sessionId, ct);
        return RedirectToAction(nameof(Index));
    }
}
