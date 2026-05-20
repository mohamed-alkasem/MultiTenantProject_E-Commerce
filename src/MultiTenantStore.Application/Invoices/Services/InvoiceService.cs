using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Application.Common.MultiTenancy;
using MultiTenantStore.Application.Common.Storage;
using MultiTenantStore.Application.Invoices.DTOs;
using MultiTenantStore.Application.Invoices.Repositories;
using MultiTenantStore.Application.Orders.Repositories;
using MultiTenantStore.Domain.Enums;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Invoices.Services;

public sealed class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IInvoiceItemRepository _invoiceItemRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ITenantUnitOfWork _unitOfWork;
    private readonly ICurrentTenant _currentTenant;
    private readonly IInvoicePdfGenerator _pdfGenerator;
    private readonly IFileStorageService _fileStorageService;

    public InvoiceService(
    IInvoiceRepository invoiceRepository,
    IInvoiceItemRepository invoiceItemRepository,
    IOrderRepository orderRepository,
    ITenantUnitOfWork unitOfWork,
    ICurrentTenant currentTenant,
    IInvoicePdfGenerator pdfGenerator,
    IFileStorageService fileStorageService)
    {
        _invoiceRepository = invoiceRepository;
        _invoiceItemRepository = invoiceItemRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _currentTenant = currentTenant;
        _pdfGenerator = pdfGenerator;
        _fileStorageService = fileStorageService;
    }

    public async Task<ApiResponseDto<InvoiceDto>> CreateForOrderAsync(
        CreateInvoiceDto dto,
        CancellationToken cancellationToken = default)
    {
        var existingInvoice = await _invoiceRepository.GetByOrderIdAsync(
            dto.OrderId,
            cancellationToken);

        if (existingInvoice is not null)
        {
            return ApiResponseDto<InvoiceDto>.Ok(
                MapToDto(existingInvoice),
                "Invoice already exists for this order.");
        }

        var order = await _orderRepository.GetDetailsAsync(
            dto.OrderId,
            cancellationToken);

        if (order is null)
        {
            return ApiResponseDto<InvoiceDto>.Fail("Order was not found.");
        }

        if (order.Items.Count == 0)
        {
            return ApiResponseDto<InvoiceDto>.Fail("Order has no items.");
        }

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            InvoiceNumber = GenerateInvoiceNumber(),
            Status = InvoiceStatus.Issued,
            IssueDate = DateTime.UtcNow,
            DueDate = dto.DueDate,

            Subtotal = order.Subtotal,
            DiscountAmount = order.DiscountAmount,
            TaxAmount = order.TaxAmount,
            ShippingAmount = order.ShippingAmount,
            TotalAmount = order.TotalAmount,
            Currency = order.Currency,

            CustomerNameSnapshot = order.ShippingFullName,
            CustomerEmailSnapshot = null,
            BillingAddressSnapshot = BuildBillingAddressSnapshot(order),

            StoreNameSnapshot = _currentTenant.StoreName ?? "Store",
            StoreTaxNumberSnapshot = null,
            StoreAddressSnapshot = null,

            CreatedAt = DateTime.UtcNow
        };

        await _invoiceRepository.AddAsync(invoice, cancellationToken);

        var savedItems = new List<InvoiceItem>();
        foreach (var orderItem in order.Items.Where(x => x.DeletedAt == null).DistinctBy(x => x.Id))
        {
            var invoiceItem = new InvoiceItem
            {
                Id = Guid.NewGuid(),
                InvoiceId = invoice.Id,
                OrderItemId = orderItem.Id,

                ProductNameSnapshot = orderItem.ProductNameSnapshot,
                SKU = orderItem.SKU,
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice,

                TaxRate = 0,
                TaxAmount = 0,
                LineTotal = orderItem.LineTotal,

                CreatedAt = DateTime.UtcNow
            };

            await _invoiceItemRepository.AddAsync(invoiceItem, cancellationToken);
            savedItems.Add(invoiceItem);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (var item in savedItems)
            invoice.Items.Add(item);

        return ApiResponseDto<InvoiceDto>.Ok(
            MapToDto(invoice),
            "Invoice created successfully.");
    }

    public async Task<ApiResponseDto<List<InvoiceListDto>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var invoices = await _invoiceRepository.GetAllNotDeletedAsync(
            cancellationToken);

        var result = invoices
            .Select(x => new InvoiceListDto
            {
                Id = x.Id,
                OrderId = x.OrderId,
                InvoiceNumber = x.InvoiceNumber,
                Status = x.Status.ToString(),
                IssueDate = x.IssueDate,
                TotalAmount = x.TotalAmount,
                Currency = x.Currency,
                CustomerNameSnapshot = x.CustomerNameSnapshot,
                PdfUrl = x.PdfUrl
            })
            .ToList();

        return ApiResponseDto<List<InvoiceListDto>>.Ok(result);
    }

    public async Task<ApiResponseDto<InvoiceDto>> GetByIdAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetDetailsAsync(
            invoiceId,
            cancellationToken);

        if (invoice is null)
        {
            return ApiResponseDto<InvoiceDto>.Fail("Invoice was not found.");
        }

        return ApiResponseDto<InvoiceDto>.Ok(MapToDto(invoice));
    }

    public async Task<ApiResponseDto<InvoicePdfDto>> GeneratePdfAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetDetailsAsync(
            invoiceId,
            cancellationToken);

        if (invoice is null)
        {
            return ApiResponseDto<InvoicePdfDto>.Fail("Invoice was not found.");
        }

        var invoiceDto = MapToDto(invoice);
        var pdfBytes = _pdfGenerator.Generate(invoiceDto);

        return ApiResponseDto<InvoicePdfDto>.Ok(
            new InvoicePdfDto
            {
                FileName = $"{invoice.InvoiceNumber}.pdf",
                Content = pdfBytes,
                ContentType = "application/pdf"
            });
    }

    private static string GenerateInvoiceNumber()
    {
        return $"INV-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
    }

    private static string BuildBillingAddressSnapshot(Order order)
    {
        var parts = new[]
        {
            order.BillingFullName,
            order.BillingPhone,
            order.BillingCountry,
            order.BillingCity,
            order.BillingDistrict,
            order.BillingAddressLine1,
            order.BillingAddressLine2,
            order.BillingPostalCode
        };

        return string.Join(", ", parts.Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    private static InvoiceDto MapToDto(Invoice invoice)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            OrderId = invoice.OrderId,
            InvoiceNumber = invoice.InvoiceNumber,
            Status = invoice.Status.ToString(),
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            Subtotal = invoice.Subtotal,
            DiscountAmount = invoice.DiscountAmount,
            TaxAmount = invoice.TaxAmount,
            ShippingAmount = invoice.ShippingAmount,
            TotalAmount = invoice.TotalAmount,
            Currency = invoice.Currency,
            CustomerNameSnapshot = invoice.CustomerNameSnapshot,
            CustomerEmailSnapshot = invoice.CustomerEmailSnapshot,
            BillingAddressSnapshot = invoice.BillingAddressSnapshot,
            StoreNameSnapshot = invoice.StoreNameSnapshot,
            StoreTaxNumberSnapshot = invoice.StoreTaxNumberSnapshot,
            StoreAddressSnapshot = invoice.StoreAddressSnapshot,
            PdfUrl = invoice.PdfUrl,
            Items = invoice.Items
                .Where(x => x.DeletedAt == null)
                .DistinctBy(x => x.Id)
                .Select(x => new InvoiceItemDto
                {
                    Id = x.Id,
                    InvoiceId = x.InvoiceId,
                    OrderItemId = x.OrderItemId,
                    ProductNameSnapshot = x.ProductNameSnapshot,
                    SKU = x.SKU,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    TaxRate = x.TaxRate,
                    TaxAmount = x.TaxAmount,
                    LineTotal = x.LineTotal
                })
                .ToList()
        };
    }

    public async Task<ApiResponseDto<InvoiceDto>> GenerateAndUploadPdfAsync(
    Guid invoiceId,
    CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetDetailsAsync(
            invoiceId,
            cancellationToken);

        if (invoice is null)
        {
            return ApiResponseDto<InvoiceDto>.Fail("Invoice was not found.");
        }

        var invoiceDto = MapToDto(invoice);

        var pdfBytes = _pdfGenerator.Generate(invoiceDto);

        var fileName = $"{invoice.InvoiceNumber}.pdf";

        var folder = $"stores/{_currentTenant.StoreId}/invoices";

        var pdfUrl = await _fileStorageService.UploadAsync(
            pdfBytes,
            fileName,
            "application/pdf",
            folder,
            cancellationToken);

        invoice.PdfUrl = pdfUrl;
        invoice.UpdatedAt = DateTime.UtcNow;

        _invoiceRepository.Update(invoice);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<InvoiceDto>.Ok(
            MapToDto(invoice),
            "Invoice PDF generated and uploaded successfully.");
    }
}