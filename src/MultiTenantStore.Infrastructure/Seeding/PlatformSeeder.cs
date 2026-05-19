using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MultiTenantStore.Domain.Platform;
using MultiTenantStore.Infrastructure.Identity;
using MultiTenantStore.Persistence.Contexts;

namespace MultiTenantStore.Infrastructure.Seeding;

public sealed class PlatformSeeder
{
    private readonly MainDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IConfiguration _configuration;

    public PlatformSeeder(
        MainDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedRolesAsync();
        await SeedSubscriptionPlansAsync(cancellationToken);
        await SeedDefaultAdminAsync();
        await PromoteAdditionalAdminsAsync();
    }

    private async Task SeedRolesAsync()
    {
        var roles = new[]
        {
            IdentityRoleConstants.PlatformAdmin,
            IdentityRoleConstants.Merchant,
            IdentityRoleConstants.Staff
        };

        foreach (var roleName in roles)
        {
            var exists = await _roleManager.RoleExistsAsync(roleName);

            if (exists)
            {
                continue;
            }

            var result = await _roleManager.CreateAsync(
                new IdentityRole<Guid>(roleName));

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                throw new InvalidOperationException(
                    $"Could not create role '{roleName}'. Errors: {errors}");
            }
        }
    }

    private async Task SeedSubscriptionPlansAsync(CancellationToken cancellationToken)
    {
        var plans = new List<SubscriptionPlan>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Free",
                Code = "FREE",
                PriceMonthly = 0,
                PriceYearly = 0,
                MaxProducts = 50,
                MaxStaffUsers = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Basic",
                Code = "BASIC",
                PriceMonthly = 9.99m,
                PriceYearly = 99.99m,
                MaxProducts = 500,
                MaxStaffUsers = 3,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Pro",
                Code = "PRO",
                PriceMonthly = 29.99m,
                PriceYearly = 299.99m,
                MaxProducts = 5000,
                MaxStaffUsers = 10,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        foreach (var plan in plans)
        {
            var exists = await _context.SubscriptionPlans
                .AnyAsync(x => x.Code == plan.Code, cancellationToken);

            if (exists)
            {
                continue;
            }

            await _context.SubscriptionPlans.AddAsync(plan, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedDefaultAdminAsync()
    {
        var email = _configuration["Seed:DefaultAdminEmail"];
        var password = _configuration["Seed:DefaultAdminPassword"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            return;
        }

        var admin = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "Platform",
            LastName = "Admin",
            Email = email,
            UserName = email,
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(admin, password);

        if (!createResult.Succeeded)
        {
            var errors = createResult.Errors.Select(x => x.Description).ToList();
            throw new InvalidOperationException(
                $"Could not create default admin. Errors: {string.Join(", ", errors)}");
        }

        var roleResult = await _userManager.AddToRoleAsync(
            admin,
            IdentityRoleConstants.PlatformAdmin);

        if (!roleResult.Succeeded)
        {
            var errors = roleResult.Errors.Select(x => x.Description).ToList();
            throw new InvalidOperationException(
                $"Could not assign PlatformAdmin role. Errors: {string.Join(", ", errors)}");
        }
    }

    private async Task PromoteAdditionalAdminsAsync()
    {
        var additionalEmails = _configuration
            .GetSection("Seed:AdditionalAdminEmails")
            .Get<string[]>();

        if (additionalEmails is null || additionalEmails.Length == 0)
            return;

        foreach (var email in additionalEmails)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) continue;

            var isAlreadyAdmin = await _userManager.IsInRoleAsync(
                user, IdentityRoleConstants.PlatformAdmin);

            if (isAlreadyAdmin) continue;

            await _userManager.AddToRoleAsync(user, IdentityRoleConstants.PlatformAdmin);
        }
    }
}