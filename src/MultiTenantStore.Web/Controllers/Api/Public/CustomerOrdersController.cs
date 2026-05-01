using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.Orders.Services;

namespace MultiTenantStore.Web.Controllers.Api.Public;

[ApiController]
[Authorize]
[Route("api/customer/orders")]
public sealed class CustomerOrdersController : ControllerBase
{
    private readonly ICustomerOrderService _customerOrderService;

    public CustomerOrdersController(ICustomerOrderService customerOrderService)
    {
        _customerOrderService = customerOrderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyOrders(
        CancellationToken cancellationToken)
    {
        var result = await _customerOrderService.GetMyOrdersAsync(
            cancellationToken);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetMyOrderById(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var result = await _customerOrderService.GetMyOrderByIdAsync(
            orderId,
            cancellationToken);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}