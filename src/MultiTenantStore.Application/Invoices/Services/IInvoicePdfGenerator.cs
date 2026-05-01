using MultiTenantStore.Application.Invoices.DTOs;

namespace MultiTenantStore.Application.Invoices.Services;

public interface IInvoicePdfGenerator
{
    byte[] Generate(InvoiceDto invoice);
}