using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.Catalog.DTOs;
using MultiTenantStore.Application.Catalog.Services;

namespace MultiTenantStore.Web.Controllers.Api.Dashboard;

[ApiController]
[Authorize]
[Route("api/dashboard/products/{productId:guid}/variants")]
public sealed class DashboardProductVariantsController : ControllerBase
{
    private readonly IProductVariantService _variantService;

    public DashboardProductVariantsController(IProductVariantService variantService)
    {
        _variantService = variantService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        Guid productId,
        [FromBody] CreateProductVariantDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _variantService.CreateAsync(
            productId,
            dto,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetByProductId(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var result = await _variantService.GetByProductIdAsync(
            productId,
            cancellationToken);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpPut("{variantId:guid}")]
    public async Task<IActionResult> Update(
        Guid productId,
        Guid variantId,
        [FromBody] UpdateProductVariantDto dto,
        CancellationToken cancellationToken)
    {
        if (variantId != dto.Id)
        {
            return BadRequest("Route variant id and body id do not match.");
        }

        var result = await _variantService.UpdateAsync(
            productId,
            dto,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{variantId:guid}")]
    public async Task<IActionResult> Delete(
        Guid productId,
        Guid variantId,
        CancellationToken cancellationToken)
    {
        var result = await _variantService.DeleteAsync(
            productId,
            variantId,
            cancellationToken);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}