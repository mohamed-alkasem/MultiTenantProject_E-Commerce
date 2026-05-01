using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.Carts.DTOs;
using MultiTenantStore.Application.Carts.Services;

namespace MultiTenantStore.Web.Controllers.Api.Public;

[ApiController]
[AllowAnonymous]
[Route("api/cart")]
public sealed class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart(
        [FromQuery] string sessionId,
        CancellationToken cancellationToken)
    {
        var result = await _cartService.GetOrCreateCartAsync(
            sessionId,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("{sessionId}/items")]
    public async Task<IActionResult> AddItem(
        string sessionId,
        [FromBody] AddToCartDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _cartService.AddItemAsync(
            sessionId,
            dto,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPut("{sessionId}/items")]
    public async Task<IActionResult> UpdateItemQuantity(
        string sessionId,
        [FromBody] UpdateCartItemDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _cartService.UpdateItemQuantityAsync(
            sessionId,
            dto,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{sessionId}/items")]
    public async Task<IActionResult> RemoveItem(
        string sessionId,
        [FromBody] RemoveCartItemDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _cartService.RemoveItemAsync(
            sessionId,
            dto,
            cancellationToken);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpDelete("{sessionId}")]
    public async Task<IActionResult> ClearCart(
        string sessionId,
        CancellationToken cancellationToken)
    {
        var result = await _cartService.ClearCartAsync(
            sessionId,
            cancellationToken);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}