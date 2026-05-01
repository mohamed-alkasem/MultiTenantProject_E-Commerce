namespace MultiTenantStore.Application.Invoices.DTOs;

public sealed class InvoicePdfDto
{
    public string FileName { get; set; } = default!;

    public byte[] Content { get; set; } = Array.Empty<byte>();

    public string ContentType { get; set; } = "application/pdf";
}