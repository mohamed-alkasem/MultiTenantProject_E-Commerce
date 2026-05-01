using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiTenantStore.Application.Auth.DTOs;
using MultiTenantStore.Application.Auth.Interfaces;
using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Stores.DTOs;
using MultiTenantStore.Application.Stores.Interfaces;
using MultiTenantStore.Domain.Platform;
using MultiTenantStore.Infrastructure.Identity;
using MultiTenantStore.Infrastructure.Jwt;
using MultiTenantStore.Persistence.Contexts;

namespace MultiTenantStore.Infrastructure.Identity;

public sealed class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly IStoreOnboardingService _storeOnboardingService;
    private readonly MainDbContext _context;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        ITokenService tokenService,
        IStoreOnboardingService storeOnboardingService,
        MainDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _storeOnboardingService = storeOnboardingService;
        _context = context;
    }

    public async Task<ApiResponseDto<AuthResponseDto>> RegisterMerchantAsync(
        RegisterMerchantDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.Password != dto.ConfirmPassword)
        {
            return ApiResponseDto<AuthResponseDto>.Fail(
                "Password and confirmation password do not match.");
        }

        var existingUser = await _userManager.FindByEmailAsync(dto.Email);

        if (existingUser is not null)
        {
            return ApiResponseDto<AuthResponseDto>.Fail(
                "Email is already registered.");
        }

        await EnsureRoleExistsAsync(IdentityRoleConstants.Merchant);

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim(),
            UserName = dto.Email.Trim(),
            PhoneNumber = dto.PhoneNumber,
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createUserResult = await _userManager.CreateAsync(user, dto.Password);

        if (!createUserResult.Succeeded)
        {
            return ApiResponseDto<AuthResponseDto>.Fail(
                "User creation failed.",
                createUserResult.Errors.Select(x => x.Description).ToList());
        }

        var addRoleResult = await _userManager.AddToRoleAsync(
            user,
            IdentityRoleConstants.Merchant);

        if (!addRoleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            return ApiResponseDto<AuthResponseDto>.Fail(
                "Assigning merchant role failed.",
                addRoleResult.Errors.Select(x => x.Description).ToList());
        }

        var onboardingResult = await _storeOnboardingService.OnboardStoreForMerchantAsync(
            new CreateStoreDto
            {
                OwnerUserId = user.Id,
                StoreName = dto.StoreName,
                Slug = dto.StoreSlug,
                PlanCode = dto.PlanCode,
                BillingCycle = dto.BillingCycle
            },
            cancellationToken);

        if (!onboardingResult.Success || onboardingResult.Data is null)
        {
            await _userManager.DeleteAsync(user);

            return ApiResponseDto<AuthResponseDto>.Fail(
                onboardingResult.Message ?? "Store onboarding failed.",
                onboardingResult.Errors);
        }

        var roles = await _userManager.GetRolesAsync(user);

        var token = _tokenService.GenerateToken(
            user,
            roles,
            onboardingResult.Data.StoreId,
            onboardingResult.Data.StoreSlug,
            onboardingResult.Data.StoreRole);

        return ApiResponseDto<AuthResponseDto>.Ok(
            token,
            "Merchant registered successfully.");
    }

    public async Task<ApiResponseDto<AuthResponseDto>> LoginAsync(
        LoginDto dto,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user is null)
        {
            return ApiResponseDto<AuthResponseDto>.Fail(
                "Invalid email or password.");
        }

        if (!user.IsActive || user.DeletedAt is not null)
        {
            return ApiResponseDto<AuthResponseDto>.Fail(
                "User account is inactive.");
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

        if (!passwordValid)
        {
            return ApiResponseDto<AuthResponseDto>.Fail(
                "Invalid email or password.");
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);

        var storeUser = await _context.StoreUsers
            .AsNoTracking()
            .Include(x => x.Store)
            .Where(x =>
                x.UserId == user.Id &&
                x.IsActive &&
                x.DeletedAt == null &&
                x.Store.DeletedAt == null)
            .OrderBy(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        Guid? storeId = null;
        string? storeSlug = null;
        string? storeRole = null;

        if (storeUser is not null)
        {
            storeId = storeUser.StoreId;
            storeSlug = storeUser.Store.Slug;
            storeRole = storeUser.Role.ToString();
        }

        var token = _tokenService.GenerateToken(
            user,
            roles,
            storeId,
            storeSlug,
            storeRole);

        return ApiResponseDto<AuthResponseDto>.Ok(
            token,
            "Login successful.");
    }

    private async Task EnsureRoleExistsAsync(string roleName)
    {
        var exists = await _roleManager.RoleExistsAsync(roleName);

        if (exists)
        {
            return;
        }

        var result = await _roleManager.CreateAsync(
            new IdentityRole<Guid>(roleName));

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(x => x.Description));
            throw new InvalidOperationException($"Could not create role '{roleName}'. Errors: {errors}");
        }
    }
}