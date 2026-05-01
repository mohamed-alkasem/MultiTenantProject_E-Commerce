using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.StoreSettings.DTOs;
using MultiTenantStore.Application.StoreSettings.Services;

namespace MultiTenantStore.Web.Controllers.Api.Dashboard;

[ApiController]
[Authorize]
[Route("api/dashboard/store-settings")]
public sealed class DashboardStoreSettingsController : ControllerBase
{
    private readonly IStoreSettingService _storeSettingService;

    public DashboardStoreSettingsController(IStoreSettingService storeSettingService)
    {
        _storeSettingService = storeSettingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSettings(
        CancellationToken cancellationToken)
    {
        var result = await _storeSettingService.GetSettingsAsync(cancellationToken);

        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateSettings(
        [FromBody] UpdateStoreSettingDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _storeSettingService.UpdateSettingsAsync(
            dto,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("enable-checkout")]
    public async Task<IActionResult> EnableCheckout(
        CancellationToken cancellationToken)
    {
        var result = await _storeSettingService.EnableCheckoutAsync(
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("disable-checkout")]
    public async Task<IActionResult> DisableCheckout(
        CancellationToken cancellationToken)
    {
        var result = await _storeSettingService.DisableCheckoutAsync(
            cancellationToken);

        return Ok(result);
    }
}