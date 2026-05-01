using Microsoft.Extensions.DependencyInjection;
using MultiTenantStore.Application.Carts.Services;
using MultiTenantStore.Application.Catalog.Services;
using MultiTenantStore.Application.Checkout.Services;
using MultiTenantStore.Application.Customers.Services;
using MultiTenantStore.Application.Invoices.Services;
using MultiTenantStore.Application.Orders.Services;
using MultiTenantStore.Application.Storefront.Services;
namespace MultiTenantStore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductVariantService, ProductVariantService>();
        services.AddScoped<IProductImageService, ProductImageService>();

        services.AddScoped<IPublicCatalogService, PublicCatalogService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<ICheckoutService, CheckoutService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<ICustomerAuthService, CustomerAuthService>();

        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ICustomerAddressService, CustomerAddressService>();

        return services;
    }
}