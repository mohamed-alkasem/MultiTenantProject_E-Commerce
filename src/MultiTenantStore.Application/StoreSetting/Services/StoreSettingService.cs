using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Application.StoreSettings.DTOs;
using MultiTenantStore.Application.StoreSettings.Repositories;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.StoreSettings.Services;

public sealed class StoreSettingService : IStoreSettingService
{
    private readonly IStoreSettingRepository _settingRepository;
    private readonly ITenantUnitOfWork _unitOfWork;

    public StoreSettingService(
        IStoreSettingRepository settingRepository,
        ITenantUnitOfWork unitOfWork)
    {
        _settingRepository = settingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto<StoreSettingDto>> GetSettingsAsync(
        CancellationToken cancellationToken = default)
    {
        var settings = await GetOrCreateDefaultSettingsAsync(cancellationToken);

        return ApiResponseDto<StoreSettingDto>.Ok(MapToDto(settings));
    }

    public async Task<ApiResponseDto<StoreSettingDto>> UpdateSettingsAsync(
        UpdateStoreSettingDto dto,
        CancellationToken cancellationToken = default)
    {
        var settings = await GetOrCreateDefaultSettingsAsync(cancellationToken);

        settings.Currency = NormalizeCurrency(dto.Currency);
        settings.Timezone = string.IsNullOrWhiteSpace(dto.Timezone)
            ? "UTC"
            : dto.Timezone.Trim();

        settings.DefaultLanguage = string.IsNullOrWhiteSpace(dto.DefaultLanguage)
            ? "en"
            : dto.DefaultLanguage.Trim().ToLowerInvariant();

        settings.IsCheckoutEnabled = dto.IsCheckoutEnabled;
        settings.TaxEnabled = dto.TaxEnabled;
        settings.OrderPrefix = NormalizeOrderPrefix(dto.OrderPrefix);
        settings.UpdatedAt = DateTime.UtcNow;

        _settingRepository.Update(settings);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<StoreSettingDto>.Ok(
            MapToDto(settings),
            "Store settings updated successfully.");
    }

    public async Task<ApiResponseDto<StoreSettingDto>> EnableCheckoutAsync(
        CancellationToken cancellationToken = default)
    {
        var settings = await GetOrCreateDefaultSettingsAsync(cancellationToken);

        settings.IsCheckoutEnabled = true;
        settings.UpdatedAt = DateTime.UtcNow;

        _settingRepository.Update(settings);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<StoreSettingDto>.Ok(
            MapToDto(settings),
            "Checkout enabled successfully.");
    }

    public async Task<ApiResponseDto<StoreSettingDto>> DisableCheckoutAsync(
        CancellationToken cancellationToken = default)
    {
        var settings = await GetOrCreateDefaultSettingsAsync(cancellationToken);

        settings.IsCheckoutEnabled = false;
        settings.UpdatedAt = DateTime.UtcNow;

        _settingRepository.Update(settings);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<StoreSettingDto>.Ok(
            MapToDto(settings),
            "Checkout disabled successfully.");
    }

    private async Task<StoreSetting> GetOrCreateDefaultSettingsAsync(
        CancellationToken cancellationToken)
    {
        var settings = await _settingRepository.GetCurrentAsync(cancellationToken);

        if (settings is not null)
        {
            return settings;
        }

        settings = new StoreSetting
        {
            Id = Guid.NewGuid(),
            Currency = "USD",
            Timezone = "UTC",
            DefaultLanguage = "en",
            IsCheckoutEnabled = true,
            TaxEnabled = false,
            OrderPrefix = "ORD",
            CreatedAt = DateTime.UtcNow
        };

        await _settingRepository.AddAsync(settings, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return settings;
    }

    private static StoreSettingDto MapToDto(StoreSetting settings)
    {
        return new StoreSettingDto
        {
            Id = settings.Id,
            Currency = settings.Currency,
            Timezone = settings.Timezone,
            DefaultLanguage = settings.DefaultLanguage,
            IsCheckoutEnabled = settings.IsCheckoutEnabled,
            TaxEnabled = settings.TaxEnabled,
            OrderPrefix = settings.OrderPrefix
        };
    }

    private static string NormalizeCurrency(string currency)
    {
        return string.IsNullOrWhiteSpace(currency)
            ? "USD"
            : currency.Trim().ToUpperInvariant();
    }

    private static string NormalizeOrderPrefix(string orderPrefix)
    {
        return string.IsNullOrWhiteSpace(orderPrefix)
            ? "ORD"
            : orderPrefix.Trim().ToUpperInvariant();
    }
}