using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.Customers.DTOs;
using MultiTenantStore.Application.Customers.Services;

namespace MultiTenantStore.Web.Controllers.Api.Public;

[ApiController]
[Authorize]
[Route("api/customer/addresses")]
public sealed class CustomerAddressesController : ControllerBase
{
    private readonly ICustomerAddressService _addressService;

    public CustomerAddressesController(ICustomerAddressService addressService)
    {
        _addressService = addressService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAddresses(CancellationToken cancellationToken)
    {
        var result = await _addressService.GetAddressesAsync(cancellationToken);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddAddress(
        [FromBody] CreateCustomerAddressDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _addressService.AddAddressAsync(
            dto,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPut("{addressId:guid}")]
    public async Task<IActionResult> UpdateAddress(
        Guid addressId,
        [FromBody] UpdateCustomerAddressDto dto,
        CancellationToken cancellationToken)
    {
        if (addressId != dto.Id)
        {
            return BadRequest("Route address id and body id do not match.");
        }

        var result = await _addressService.UpdateAddressAsync(
            dto,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("{addressId:guid}/default-shipping")]
    public async Task<IActionResult> SetDefaultShipping(
        Guid addressId,
        CancellationToken cancellationToken)
    {
        var result = await _addressService.SetDefaultShippingAsync(
            addressId,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("{addressId:guid}/default-billing")]
    public async Task<IActionResult> SetDefaultBilling(
        Guid addressId,
        CancellationToken cancellationToken)
    {
        var result = await _addressService.SetDefaultBillingAsync(
            addressId,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{addressId:guid}")]
    public async Task<IActionResult> DeleteAddress(
        Guid addressId,
        CancellationToken cancellationToken)
    {
        var result = await _addressService.DeleteAddressAsync(
            addressId,
            cancellationToken);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}