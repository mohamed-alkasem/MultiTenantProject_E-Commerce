using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.Customers.DTOs;
using MultiTenantStore.Application.Customers.Services;

namespace MultiTenantStore.Web.Controllers.Api.Public;

[ApiController]
[AllowAnonymous]
[Route("api/customer/auth")]
public sealed class CustomerAuthController : ControllerBase
{
    private readonly ICustomerAuthService _customerAuthService;

    public CustomerAuthController(ICustomerAuthService customerAuthService)
    {
        _customerAuthService = customerAuthService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCustomerDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _customerAuthService.RegisterAsync(
            dto,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] CustomerLoginDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _customerAuthService.LoginAsync(
            dto,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}