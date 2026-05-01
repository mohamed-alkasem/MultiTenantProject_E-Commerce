using MultiTenantStore.Infrastructure.Seeding;

namespace MultiTenantStore.Web.Extensions;

public static class WebApplicationExtensions
{
    public static async Task SeedPlatformDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var seeder = scope.ServiceProvider.GetRequiredService<PlatformSeeder>();

        await seeder.SeedAsync();
    }
}