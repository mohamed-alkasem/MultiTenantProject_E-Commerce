using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.Checkout.Services;
using MultiTenantStore.Application.Orders.DTOs;

namespace MultiTenantStore.Web.Controllers.Api.Public;

[ApiController]
[AllowAnonymous]
[Route("api/checkout")]
public sealed class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;

    public CheckoutController(ICheckoutService checkoutService)
    {
        _checkoutService = checkoutService;
    }

    [HttpPost("orders")]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _checkoutService.CreateOrderAsync(
            dto,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}