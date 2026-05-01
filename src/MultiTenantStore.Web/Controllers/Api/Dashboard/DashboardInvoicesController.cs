using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiTenantStore.Application.Invoices.DTOs;
using MultiTenantStore.Application.Invoices.Services;

namespace MultiTenantStore.Web.Controllers.Api.Dashboard;

[ApiController]
[Authorize]
[Route("api/dashboard/invoices")]
public sealed class DashboardInvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public DashboardInvoicesController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateInvoiceDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _invoiceService.CreateForOrderAsync(
            dto,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _invoiceService.GetAllAsync(cancellationToken);

        return Ok(result);
    }

    [HttpGet("{invoiceId:guid}")]
    public async Task<IActionResult> GetById(
        Guid invoiceId,
        CancellationToken cancellationToken)
    {
        var result = await _invoiceService.GetByIdAsync(
            invoiceId,
            cancellationToken);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpGet("{invoiceId:guid}/download")]
    public async Task<IActionResult> Download(
        Guid invoiceId,
        CancellationToken cancellationToken)
    {
        var result = await _invoiceService.GeneratePdfAsync(
            invoiceId,
            cancellationToken);

        if (!result.Success || result.Data is null)
        {
            return NotFound(result);
        }

        return File(
            result.Data.Content,
            result.Data.ContentType,
            result.Data.FileName);
    }
    [HttpPost("{invoiceId:guid}/generate-pdf")]
    public async Task<IActionResult> GenerateAndUploadPdf(
    Guid invoiceId,
    CancellationToken cancellationToken)
    {
        var result = await _invoiceService.GenerateAndUploadPdfAsync(
            invoiceId,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}