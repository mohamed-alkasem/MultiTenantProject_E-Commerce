using MultiTenantStore.Application.Invoices.DTOs;
using MultiTenantStore.Application.Invoices.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MultiTenantStore.Infrastructure.Invoices;

public sealed class InvoicePdfGenerator : IInvoicePdfGenerator
{
    public byte[] Generate(InvoiceDto invoice)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Column(column =>
                    {
                        column.Item().Text(invoice.StoreNameSnapshot)
                            .FontSize(20)
                            .Bold();

                        column.Item().Text($"Invoice Number: {invoice.InvoiceNumber}");
                        column.Item().Text($"Issue Date: {invoice.IssueDate:yyyy-MM-dd}");
                        column.Item().Text($"Status: {invoice.Status}");
                    });

                page.Content()
                    .PaddingVertical(20)
                    .Column(column =>
                    {
                        column.Spacing(15);

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(left =>
                            {
                                left.Item().Text("Bill To").Bold();
                                left.Item().Text(invoice.CustomerNameSnapshot);
                                left.Item().Text(invoice.BillingAddressSnapshot);
                            });

                            row.RelativeItem().Column(right =>
                            {
                                right.Item().Text("Store").Bold();
                                right.Item().Text(invoice.StoreNameSnapshot);

                                if (!string.IsNullOrWhiteSpace(invoice.StoreAddressSnapshot))
                                {
                                    right.Item().Text(invoice.StoreAddressSnapshot);
                                }

                                if (!string.IsNullOrWhiteSpace(invoice.StoreTaxNumberSnapshot))
                                {
                                    right.Item().Text($"Tax No: {invoice.StoreTaxNumberSnapshot}");
                                }
                            });
                        });

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCell).Text("Product");
                                header.Cell().Element(HeaderCell).Text("SKU");
                                header.Cell().Element(HeaderCell).AlignRight().Text("Qty");
                                header.Cell().Element(HeaderCell).AlignRight().Text("Unit");
                                header.Cell().Element(HeaderCell).AlignRight().Text("Total");
                            });

                            foreach (var item in invoice.Items)
                            {
                                table.Cell().Element(BodyCell).Text(item.ProductNameSnapshot);
                                table.Cell().Element(BodyCell).Text(item.SKU);
                                table.Cell().Element(BodyCell).AlignRight().Text(item.Quantity.ToString());
                                table.Cell().Element(BodyCell).AlignRight().Text($"{item.UnitPrice:N2}");
                                table.Cell().Element(BodyCell).AlignRight().Text($"{item.LineTotal:N2}");
                            }

                            static IContainer HeaderCell(IContainer container)
                            {
                                return container
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(5)
                                    .BorderBottom(1);
                            }

                            static IContainer BodyCell(IContainer container)
                            {
                                return container
                                    .Padding(5)
                                    .BorderBottom(0.5f)
                                    .BorderColor(Colors.Grey.Lighten2);
                            }
                        });

                        column.Item().AlignRight().Column(totals =>
                        {
                            totals.Item().Text($"Subtotal: {invoice.Subtotal:N2} {invoice.Currency}");
                            totals.Item().Text($"Discount: {invoice.DiscountAmount:N2} {invoice.Currency}");
                            totals.Item().Text($"Tax: {invoice.TaxAmount:N2} {invoice.Currency}");
                            totals.Item().Text($"Shipping: {invoice.ShippingAmount:N2} {invoice.Currency}");
                            totals.Item().Text($"Total: {invoice.TotalAmount:N2} {invoice.Currency}")
                                .FontSize(14)
                                .Bold();
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Generated by MultiTenantStore");
                    });
            });
        }).GeneratePdf();
    }
}