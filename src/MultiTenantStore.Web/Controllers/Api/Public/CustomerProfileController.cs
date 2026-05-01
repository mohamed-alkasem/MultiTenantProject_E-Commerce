using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.Customers.DTOs;
using MultiTenantStore.Application.Customers.Services;

namespace MultiTenantStore.Web.Controllers.Api.Public;

[ApiController]
[Authorize]
[Route("api/customer/profile")]
public sealed class CustomerProfileController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerProfileController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var result = await _customerService.GetProfileAsync(cancellationToken);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateCustomerDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _customerService.UpdateProfileAsync(
            dto,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}