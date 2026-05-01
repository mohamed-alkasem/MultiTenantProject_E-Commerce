using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Domain.Platform;

namespace MultiTenantStore.Persistence.Contexts;

public class MainDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public MainDbContext(DbContextOptions<MainDbContext> options)
        : base(options)
    {
    }

    public DbSet<Store> Stores => Set<Store>();
    public DbSet<StoreUser> StoreUsers => Set<StoreUser>();
    public DbSet<StoreDomain> StoreDomains => Set<StoreDomain>();
    public DbSet<StoreDatabase> StoreDatabases => Set<StoreDatabase>();
    public DbSet<StoreBranding> StoreBrandings => Set<StoreBranding>();
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<StoreSubscription> StoreSubscriptions => Set<StoreSubscription>();
    public DbSet<SubscriptionPayment> SubscriptionPayments => Set<SubscriptionPayment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().ToTable("ASP_NET_USERS");
        builder.Entity<IdentityRole<Guid>>().ToTable("ASP_NET_ROLES");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("ASP_NET_USER_ROLES");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("ASP_NET_USER_CLAIMS");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("ASP_NET_USER_LOGINS");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("ASP_NET_ROLE_CLAIMS");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("ASP_NET_USER_TOKENS");

        builder.ApplyConfigurationsFromAssembly(typeof(MainDbContext).Assembly);
    }
}