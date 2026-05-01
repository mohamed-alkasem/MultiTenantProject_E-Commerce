using Microsoft.Extensions.Configuration;
using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Application.Stores.DTOs;
using MultiTenantStore.Application.Stores.Interfaces;
using MultiTenantStore.Domain.Enums;
using MultiTenantStore.Domain.Platform;

namespace MultiTenantStore.Infrastructure.Stores;

public sealed class StoreOnboardingService : IStoreOnboardingService
{
    private readonly IPlatformRepository<Store> _storeRepository;
    private readonly IPlatformRepository<StoreDomain> _storeDomainRepository;
    private readonly IPlatformRepository<StoreDatabase> _storeDatabaseRepository;
    private readonly IPlatformRepository<StoreBranding> _storeBrandingRepository;
    private readonly IPlatformRepository<StoreUser> _storeUserRepository;
    private readonly IPlatformRepository<SubscriptionPlan> _planRepository;
    private readonly IPlatformRepository<StoreSubscription> _subscriptionRepository;
    private readonly IPlatformUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public StoreOnboardingService(
        IPlatformRepository<Store> storeRepository,
        IPlatformRepository<StoreDomain> storeDomainRepository,
        IPlatformRepository<StoreDatabase> storeDatabaseRepository,
        IPlatformRepository<StoreBranding> storeBrandingRepository,
        IPlatformRepository<StoreUser> storeUserRepository,
        IPlatformRepository<SubscriptionPlan> planRepository,
        IPlatformRepository<StoreSubscription> subscriptionRepository,
        IPlatformUnitOfWork unitOfWork,
        IConfiguration configuration)
    {
        _storeRepository = storeRepository;
        _storeDomainRepository = storeDomainRepository;
        _storeDatabaseRepository = storeDatabaseRepository;
        _storeBrandingRepository = storeBrandingRepository;
        _storeUserRepository = storeUserRepository;
        _planRepository = planRepository;
        _subscriptionRepository = subscriptionRepository;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<ApiResponseDto<StoreOnboardingResultDto>> OnboardStoreForMerchantAsync(
        CreateStoreDto dto,
        CancellationToken cancellationToken = default)
    {
        var slug = NormalizeSlug(dto.Slug);

        var slugExists = await _storeRepository.ExistsAsync(
            x => x.Slug == slug,
            cancellationToken);

        if (slugExists)
        {
            return ApiResponseDto<StoreOnboardingResultDto>.Fail(
                "Store slug is already used.");
        }

        var plan = await _planRepository.FirstOrDefaultAsync(
            x => x.Code == dto.PlanCode && x.IsActive,
            cancellationToken);

        if (plan is null)
        {
            return ApiResponseDto<StoreOnboardingResultDto>.Fail(
                $"Subscription plan '{dto.PlanCode}' was not found.");
        }

        if (!Enum.TryParse<BillingCycle>(
                dto.BillingCycle,
                ignoreCase: true,
                out var billingCycle))
        {
            return ApiResponseDto<StoreOnboardingResultDto>.Fail(
                "Invalid billing cycle.");
        }

        var store = new Store
        {
            Id = Guid.NewGuid(),
            OwnerUserId = dto.OwnerUserId,
            StoreName = dto.StoreName.Trim(),
            Slug = slug,
            Status = StoreStatus.PendingProvisioning,
            SubscriptionStatus = SubscriptionStatus.Trial,
            CreatedAt = DateTime.UtcNow
        };

        await _storeRepository.AddAsync(store, cancellationToken);

        var fullDomain = BuildFullDomain(slug);

        var domain = new StoreDomain
        {
            Id = Guid.NewGuid(),
            StoreId = store.Id,
            Subdomain = slug,
            FullDomain = fullDomain,
            IsPrimary = true,
            IsVerified = true,
            VerifiedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await _storeDomainRepository.AddAsync(domain, cancellationToken);

        var databaseName = BuildTenantDatabaseName(slug);
        var connectionString = BuildTenantConnectionString(databaseName);

        var database = new StoreDatabase
        {
            Id = Guid.NewGuid(),
            StoreId = store.Id,
            DatabaseName = databaseName,
            DbServer = "DESKTOP-CNQ5IH7",
            Provider = "SqlServer",

            // مؤقتًا نخزنها كما هي.
            // لاحقًا نضيف EncryptionService ونشفّرها.
            ConnectionStringEncrypted = connectionString,

            ProvisioningStatus = ProvisioningStatus.NotStarted,
            IsProvisioned = false,
            CreatedAt = DateTime.UtcNow
        };

        await _storeDatabaseRepository.AddAsync(database, cancellationToken);

        var branding = new StoreBranding
        {
            Id = Guid.NewGuid(),
            StoreId = store.Id,
            CreatedAt = DateTime.UtcNow
        };

        await _storeBrandingRepository.AddAsync(branding, cancellationToken);

        var storeUser = new StoreUser
        {
            Id = Guid.NewGuid(),
            StoreId = store.Id,
            UserId = dto.OwnerUserId,
            Role = StoreUserRole.Owner,
            IsActive = true,
            JoinedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await _storeUserRepository.AddAsync(storeUser, cancellationToken);

        var subscription = new StoreSubscription
        {
            Id = Guid.NewGuid(),
            StoreId = store.Id,
            PlanId = plan.Id,
            BillingCycle = billingCycle,
            Status = SubscriptionStatus.Trial,
            AutoRenew = false,
            StartDate = DateTime.UtcNow,
            TrialEndsAt = DateTime.UtcNow.AddDays(14),
            CreatedAt = DateTime.UtcNow
        };

        await _subscriptionRepository.AddAsync(subscription, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<StoreOnboardingResultDto>.Ok(
            new StoreOnboardingResultDto
            {
                StoreId = store.Id,
                StoreName = store.StoreName,
                StoreSlug = store.Slug,
                StoreRole = StoreUserRole.Owner.ToString()
            },
            "Store onboarding completed.");
    }

    private static string NormalizeSlug(string slug)
    {
        return slug.Trim().ToLowerInvariant();
    }

    private static string BuildTenantDatabaseName(string slug)
    {
        var safeSlug = slug.Replace("-", "_");
        return $"MultiTenantStore_Tenant_{safeSlug}";
    }

    private string BuildTenantConnectionString(string databaseName)
    {
        var template = _configuration.GetConnectionString("TenantDbTemplate");

        if (string.IsNullOrWhiteSpace(template))
        {
            throw new InvalidOperationException(
                "Connection string 'TenantDbTemplate' was not found.");
        }

        return template.Replace("{databaseName}", databaseName);
    }

    private static string BuildFullDomain(string slug)
    {
        // مؤقتًا للتطوير المحلي.
        // لاحقًا نربطه بـ TenantResolutionOptions.BaseDomain.
        return $"{slug}.localhost";
    }
}