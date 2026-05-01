using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.Stores.Interfaces;

namespace MultiTenantStore.Web.Controllers.Api.Platform;

[ApiController]
[Route("api/platform/stores")]
public sealed class PlatformStoresController : ControllerBase
{
    private readonly IStoreDatabaseService _storeDatabaseService;

    public PlatformStoresController(IStoreDatabaseService storeDatabaseService)
    {
        _storeDatabaseService = storeDatabaseService;
    }

    [HttpPost("{storeId:guid}/provision-database")]
    public async Task<IActionResult> ProvisionDatabase(
        Guid storeId,
        CancellationToken cancellationToken)
    {
        var result = await _storeDatabaseService.ProvisionDatabaseAsync(
            storeId,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}