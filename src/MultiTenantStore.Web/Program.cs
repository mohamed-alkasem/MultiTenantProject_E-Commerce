using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using MultiTenantStore.Application;
using MultiTenantStore.Application.Customers.Services;
using MultiTenantStore.Infrastructure;
using MultiTenantStore.Persistence;
using MultiTenantStore.Web.Extensions;
using MultiTenantStore.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWebAuthentication(builder.Configuration);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o =>
{
    o.Cookie.Name = ".Storefront.Session";
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
    o.IdleTimeout = TimeSpan.FromHours(2);
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentCustomerService, CurrentCustomerService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseMiddleware<MultiTenantStore.Web.Middlewares.SubdomainStorefrontMiddleware>();

app.UseRouting();

app.UseAuthentication();

app.UseSession();

app.UseMiddleware<MultiTenantStore.Web.Middlewares.StorefrontTenantMiddleware>();

app.UseTenantResolution();

app.UseAuthorization();

app.MapControllers();

// Storefront dev route: /s/{storeSlug}/products, /s/{storeSlug}/cart, etc.
app.MapControllerRoute(
    name: "storefront_dev",
    pattern: "s/{storeSlug}/{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "Storefront" });

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "register",
    pattern: "Register",
    defaults: new { controller = "Home", action = "Register" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.SeedPlatformDataAsync();

app.Run();