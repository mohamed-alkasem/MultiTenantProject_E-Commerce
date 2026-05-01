namespace MultiTenantStore.Application.Invoices.DTOs;

public sealed class CreateInvoiceDto
{
    public Guid OrderId { get; set; }

    public DateTime? DueDate { get; set; }
}