using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Application.Common.MultiTenancy;
using MultiTenantStore.Infrastructure.Email;
using MultiTenantStore.Infrastructure.Jwt;
using MultiTenantStore.Infrastructure.MultiTenancy;
using MultiTenantStore.Infrastructure.Services;
using MultiTenantStore.Infrastructure.Storage;
using MultiTenantStore.Application.Auth.Interfaces;
using MultiTenantStore.Application.Stores.Interfaces;
using MultiTenantStore.Infrastructure.Identity;
using MultiTenantStore.Infrastructure.Jwt;
using MultiTenantStore.Infrastructure.Stores;
using MultiTenantStore.Infrastructure.Seeding;

namespace MultiTenantStore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(
            configuration.GetSection("Jwt"));

        services.Configure<TenantResolutionOptions>(
            configuration.GetSection("TenantResolution"));

        services.Configure<S3StorageOptions>(
            configuration.GetSection("S3Storage"));

        services.Configure<EmailOptions>(
            configuration.GetSection("Email"));

        services.AddScoped<IDateTimeService, DateTimeService>();

        services.AddScoped<ICurrentTenant, CurrentTenant>();
        services.AddScoped<ITenantStore, TenantStore>();
        services.AddScoped<ITenantAccessValidator, TenantAccessValidator>();

        services.AddScoped<TenantRouteRules>();
        services.AddScoped<IClaimsTenantResolver, ClaimsTenantResolver>();
        services.AddScoped<ISubdomainTenantResolver, SubdomainTenantResolver>();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IStoreOnboardingService, StoreOnboardingService>();

        services.AddScoped<PlatformSeeder>();

        services.AddScoped<ITenantMigrationService, TenantMigrationService>();
        services.AddScoped<ITenantSeedService, TenantSeedService>();
        services.AddScoped<IStoreDatabaseService, StoreDatabaseService>();

        return services;
    }
}