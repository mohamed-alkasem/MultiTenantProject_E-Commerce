using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.Auth.DTOs;
using MultiTenantStore.Application.Auth.Interfaces;

namespace MultiTenantStore.Web.Controllers.Api;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register-merchant")]
    public async Task<IActionResult> RegisterMerchant(
        [FromBody] RegisterMerchantDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterMerchantAsync(
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
        [FromBody] LoginDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(
            dto,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}