using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultiTenantStore.Application.Carts.Repositories;
using MultiTenantStore.Application.Catalog.Repositories;
using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Application.Common.MultiTenancy;
using MultiTenantStore.Application.Invoices.Repositories;
using MultiTenantStore.Application.Invoices.Services;
using MultiTenantStore.Application.Orders.Repositories;
using MultiTenantStore.Application.Payments.Repositories;
using MultiTenantStore.Persistence.Contexts;
using MultiTenantStore.Persistence.Repositories.Generic;
using MultiTenantStore.Persistence.Repositories.Tenant.Carts;
using MultiTenantStore.Persistence.Repositories.Tenant.Catalog;
using MultiTenantStore.Persistence.Repositories.Tenant.Invoices;
using MultiTenantStore.Persistence.Repositories.Tenant.Orders;
using MultiTenantStore.Persistence.Repositories.Tenant.Payments;
using MultiTenantStore.Persistence.UnitOfWork;



namespace MultiTenantStore.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var mainConnectionString = configuration.GetConnectionString("MainDb");

        if (string.IsNullOrWhiteSpace(mainConnectionString))
        {
            throw new InvalidOperationException("Connection string 'MainDb' was not found.");
        }

        services.AddDbContext<MainDbContext>(options =>
        {
            options.UseSqlServer(mainConnectionString);
        });

        services.AddDbContext<TenantDbContext>((serviceProvider, options) =>
        {
            var currentTenant = serviceProvider.GetRequiredService<ICurrentTenant>();

            if (!currentTenant.IsResolved ||
                string.IsNullOrWhiteSpace(currentTenant.ConnectionString))
            {
                throw new InvalidOperationException(
                    "Tenant was not resolved. TenantDbContext cannot be created.");
            }

            options.UseSqlServer(currentTenant.ConnectionString);
        });

        services.AddScoped(typeof(IPlatformRepository<>), typeof(PlatformRepository<>));
        services.AddScoped(typeof(ITenantRepository<>), typeof(TenantRepository<>));

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
        services.AddScoped<IProductImageRepository, ProductImageRepository>();

        services.AddScoped<IPlatformUnitOfWork, PlatformUnitOfWork>();
        services.AddScoped<ITenantUnitOfWork, TenantUnitOfWork>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICartItemRepository, CartItemRepository>();

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IInvoiceItemRepository, InvoiceItemRepository>();

        return services;
    }
}