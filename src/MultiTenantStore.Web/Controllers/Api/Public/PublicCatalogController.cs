using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.Catalog.DTOs;
using MultiTenantStore.Application.Storefront.Services;

namespace MultiTenantStore.Web.Controllers.Api.Public;

[ApiController]
[AllowAnonymous]
[Route("api/public")]
public sealed class PublicCatalogController : ControllerBase
{
    private readonly IPublicCatalogService _publicCatalogService;

    public PublicCatalogController(IPublicCatalogService publicCatalogService)
    {
        _publicCatalogService = publicCatalogService;
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var result = await _publicCatalogService.GetCategoriesAsync(cancellationToken);

        return Ok(result);
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts(
        [FromQuery] ProductSearchRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _publicCatalogService.GetProductsAsync(
            request,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("products/{slug}")]
    public async Task<IActionResult> GetProductBySlug(
        string slug,
        CancellationToken cancellationToken)
    {
        var result = await _publicCatalogService.GetProductBySlugAsync(
            slug,
            cancellationToken);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}