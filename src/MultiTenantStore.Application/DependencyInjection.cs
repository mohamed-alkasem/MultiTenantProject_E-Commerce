using Microsoft.Extensions.DependencyInjection;
using MultiTenantStore.Application.Catalog.Services;
using MultiTenantStore.Application.Storefront.Services;
using MultiTenantStore.Application.Carts.Services;
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

        return services;
    }
}