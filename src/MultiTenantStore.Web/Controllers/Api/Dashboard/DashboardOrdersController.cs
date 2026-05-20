using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.Orders.DTOs;
using MultiTenantStore.Application.Orders.Services;

namespace MultiTenantStore.Web.Controllers.Api.Dashboard;

[ApiController]
[Authorize(AuthenticationSchemes = "Bearer")]
[Route("api/dashboard/orders")]
public sealed class DashboardOrdersApiController : ControllerBase
{
    private readonly IOrderService _orderService;

    public DashboardOrdersApiController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders(
        [FromQuery] OrderSearchRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _orderService.GetOrdersAsync(
            request,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _orderService.GetByIdAsync(
            id,
            cancellationToken);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateOrderStatus(
        Guid id,
        [FromBody] UpdateOrderStatusDto dto,
        CancellationToken cancellationToken)
    {
        if (id != dto.OrderId)
        {
            return BadRequest("Route id and body order id do not match.");
        }

        var result = await _orderService.UpdateOrderStatusAsync(
            dto,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPut("{id:guid}/payment-status")]
    public async Task<IActionResult> UpdatePaymentStatus(
        Guid id,
        [FromBody] UpdatePaymentStatusDto dto,
        CancellationToken cancellationToken)
    {
        if (id != dto.OrderId)
        {
            return BadRequest("Route id and body order id do not match.");
        }

        var result = await _orderService.UpdatePaymentStatusAsync(
            dto,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPut("{id:guid}/shipping-status")]
    public async Task<IActionResult> UpdateShippingStatus(
        Guid id,
        [FromBody] UpdateShippingStatusDto dto,
        CancellationToken cancellationToken)
    {
        if (id != dto.OrderId)
        {
            return BadRequest("Route id and body order id do not match.");
        }

        var result = await _orderService.UpdateShippingStatusAsync(
            dto,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}