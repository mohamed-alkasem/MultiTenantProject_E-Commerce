using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.Carts.DTOs;
using MultiTenantStore.Application.Carts.Services;

namespace MultiTenantStore.Web.Controllers.Api.Public;

[ApiController]
[Authorize]
[Route("api/customer/cart")]
public sealed class CustomerCartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CustomerCartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomerCart(
        CancellationToken cancellationToken)
    {
        var result = await _cartService.GetCustomerCartAsync(cancellationToken);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    [HttpPost("merge")]
    public async Task<IActionResult> MergeGuestCart(
        [FromBody] MergeCartDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _cartService.MergeGuestCartAsync(
            dto,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}