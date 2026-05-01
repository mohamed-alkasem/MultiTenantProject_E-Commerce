using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Application.Customers.DTOs;
using MultiTenantStore.Application.Customers.Repositories;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Customers.Services;

public sealed class CustomerAuthService : ICustomerAuthService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerPasswordHasher _passwordHasher;
    private readonly ICustomerTokenService _tokenService;
    private readonly ITenantUnitOfWork _unitOfWork;

    public CustomerAuthService(
        ICustomerRepository customerRepository,
        ICustomerPasswordHasher passwordHasher,
        ICustomerTokenService tokenService,
        ITenantUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto<CustomerAuthResponseDto>> RegisterAsync(
        RegisterCustomerDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.Password != dto.ConfirmPassword)
        {
            return ApiResponseDto<CustomerAuthResponseDto>.Fail(
                "Password and confirmation password do not match.");
        }

        var email = NormalizeEmail(dto.Email);

        var emailExists = await _customerRepository.ExistsByEmailAsync(
            email,
            cancellationToken: cancellationToken);

        if (emailExists)
        {
            return ApiResponseDto<CustomerAuthResponseDto>.Fail(
                "Email is already registered.");
        }

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = email,
            PhoneNumber = dto.PhoneNumber,
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        customer.PasswordHash = _passwordHasher.HashPassword(
            customer,
            dto.Password);

        await _customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _tokenService.GenerateToken(customer);

        return ApiResponseDto<CustomerAuthResponseDto>.Ok(
            token,
            "Customer registered successfully.");
    }

    public async Task<ApiResponseDto<CustomerAuthResponseDto>> LoginAsync(
        CustomerLoginDto dto,
        CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(dto.Email);

        var customer = await _customerRepository.GetByEmailAsync(
            email,
            cancellationToken);

        if (customer is null)
        {
            return ApiResponseDto<CustomerAuthResponseDto>.Fail(
                "Invalid email or password.");
        }

        if (!customer.IsActive || customer.DeletedAt is not null)
        {
            return ApiResponseDto<CustomerAuthResponseDto>.Fail(
                "Customer account is inactive.");
        }

        var passwordValid = _passwordHasher.VerifyPassword(
            customer,
            dto.Password);

        if (!passwordValid)
        {
            return ApiResponseDto<CustomerAuthResponseDto>.Fail(
                "Invalid email or password.");
        }

        customer.LastLoginAt = DateTime.UtcNow;
        customer.UpdatedAt = DateTime.UtcNow;

        _customerRepository.Update(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _tokenService.GenerateToken(customer);

        return ApiResponseDto<CustomerAuthResponseDto>.Ok(
            token,
            "Login successful.");
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }
}