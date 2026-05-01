using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MultiTenantStore.Domain.Platform;

namespace MultiTenantStore.Persistence.Contexts;

public sealed class MainDbContextFactory : IDesignTimeDbContextFactory<MainDbContext>
{
    public MainDbContext CreateDbContext(string[] args)
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        var webProjectPath = Path.GetFullPath(
            Path.Combine(currentDirectory, "..", "MultiTenantStore.Web"));

        var basePath = Directory.Exists(webProjectPath)
            ? webProjectPath
            : currentDirectory;

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("MainDb");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'MainDb' was not found.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<MainDbContext>();

        optionsBuilder.UseSqlServer(connectionString);

        return new MainDbContext(optionsBuilder.Options);
    }
}