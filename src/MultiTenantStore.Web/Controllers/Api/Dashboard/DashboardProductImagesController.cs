using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.Catalog.DTOs;
using MultiTenantStore.Application.Catalog.Services;

namespace MultiTenantStore.Web.Controllers.Api.Dashboard;

[ApiController]
[Authorize]
[Route("api/dashboard/products/{productId:guid}/images")]
public sealed class DashboardProductImagesController : ControllerBase
{
    private readonly IProductImageService _imageService;

    public DashboardProductImagesController(IProductImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        Guid productId,
        [FromBody] CreateProductImageDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _imageService.CreateAsync(
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
        var result = await _imageService.GetByProductIdAsync(
            productId,
            cancellationToken);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpPut("{imageId:guid}")]
    public async Task<IActionResult> Update(
        Guid productId,
        Guid imageId,
        [FromBody] UpdateProductImageDto dto,
        CancellationToken cancellationToken)
    {
        if (imageId != dto.Id)
        {
            return BadRequest("Route image id and body id do not match.");
        }

        var result = await _imageService.UpdateAsync(
            productId,
            dto,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("{imageId:guid}/set-primary")]
    public async Task<IActionResult> SetPrimary(
        Guid productId,
        Guid imageId,
        CancellationToken cancellationToken)
    {
        var result = await _imageService.SetPrimaryAsync(
            productId,
            imageId,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{imageId:guid}")]
    public async Task<IActionResult> Delete(
        Guid productId,
        Guid imageId,
        CancellationToken cancellationToken)
    {
        var result = await _imageService.DeleteAsync(
            productId,
            imageId,
            cancellationToken);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}