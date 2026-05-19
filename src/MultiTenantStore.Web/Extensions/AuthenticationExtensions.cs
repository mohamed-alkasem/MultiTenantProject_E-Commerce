using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MultiTenantStore.Domain.Platform;
using MultiTenantStore.Infrastructure.Jwt;
using MultiTenantStore.Persistence.Contexts;

namespace MultiTenantStore.Web.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddWebAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtOptions = configuration
            .GetSection("Jwt")
            .Get<JwtOptions>();

        if (jwtOptions is null)
        {
            throw new InvalidOperationException("Jwt configuration section was not found.");
        }

        if (string.IsNullOrWhiteSpace(jwtOptions.SecretKey))
        {
            throw new InvalidOperationException("Jwt SecretKey is not configured.");
        }

        services
            .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<MainDbContext>()
            .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Dashboard/DashboardAccount/Login";
            options.LogoutPath = "/Dashboard/DashboardAccount/Logout";
            options.AccessDeniedPath = "/Dashboard/DashboardAccount/Login";
            options.SlidingExpiration = true;
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
        });

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtOptions.SecretKey));

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            })
            .AddCookie("PlatformAdmin", options =>
            {
                options.LoginPath = "/PlatformAdmin/AdminAccount/Login";
                options.LogoutPath = "/PlatformAdmin/AdminAccount/Logout";
                options.AccessDeniedPath = "/PlatformAdmin/AdminAccount/Login";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.Cookie.Name = ".PlatformAdmin.Session";
            })
            .AddCookie("Storefront.Customer", options =>
            {
                options.LoginPath = "/s/{storeSlug}/Customer/Login";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.Cookie.Name = ".Storefront.Customer";
                options.Cookie.HttpOnly = true;
            });

        return services;
    }
}