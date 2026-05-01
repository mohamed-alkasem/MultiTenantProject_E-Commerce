using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Invoices.DTOs;

namespace MultiTenantStore.Application.Invoices.Services;

public interface IInvoiceService
{
    Task<ApiResponseDto<InvoiceDto>> CreateForOrderAsync(
        CreateInvoiceDto dto,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<List<InvoiceListDto>>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<InvoiceDto>> GetByIdAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<InvoicePdfDto>> GeneratePdfAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<InvoiceDto>> GenerateAndUploadPdfAsync(
    Guid invoiceId,
    CancellationToken cancellationToken = default);
}