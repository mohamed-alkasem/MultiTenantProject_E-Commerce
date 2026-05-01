using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Application.Customers.DTOs;
using MultiTenantStore.Application.Customers.Repositories;

namespace MultiTenantStore.Application.Customers.Services;

public sealed class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICurrentCustomerService _currentCustomerService;
    private readonly ITenantUnitOfWork _unitOfWork;

    public CustomerService(
        ICustomerRepository customerRepository,
        ICurrentCustomerService currentCustomerService,
        ITenantUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _currentCustomerService = currentCustomerService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto<CustomerDto>> GetProfileAsync(
        CancellationToken cancellationToken = default)
    {
        var customerId = _currentCustomerService.CustomerId;

        if (customerId is null)
        {
            return ApiResponseDto<CustomerDto>.Fail("Customer is not authenticated.");
        }

        var customer = await _customerRepository.GetDetailsAsync(
            customerId.Value,
            cancellationToken);

        if (customer is null)
        {
            return ApiResponseDto<CustomerDto>.Fail("Customer was not found.");
        }

        return ApiResponseDto<CustomerDto>.Ok(MapToDto(customer));
    }

    public async Task<ApiResponseDto<CustomerDto>> UpdateProfileAsync(
        UpdateCustomerDto dto,
        CancellationToken cancellationToken = default)
    {
        var customerId = _currentCustomerService.CustomerId;

        if (customerId is null)
        {
            return ApiResponseDto<CustomerDto>.Fail("Customer is not authenticated.");
        }

        if (dto.Id != customerId.Value)
        {
            return ApiResponseDto<CustomerDto>.Fail("You cannot update another customer profile.");
        }

        var customer = await _customerRepository.GetDetailsAsync(
            customerId.Value,
            cancellationToken);

        if (customer is null)
        {
            return ApiResponseDto<CustomerDto>.Fail("Customer was not found.");
        }

        customer.FirstName = dto.FirstName.Trim();
        customer.LastName = dto.LastName.Trim();
        customer.PhoneNumber = dto.PhoneNumber;
        customer.IsActive = dto.IsActive;
        customer.UpdatedAt = DateTime.UtcNow;

        _customerRepository.Update(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<CustomerDto>.Ok(
            MapToDto(customer),
            "Customer profile updated successfully.");
    }

    private static CustomerDto MapToDto(Domain.Tenant.Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            IsActive = customer.IsActive,
            EmailConfirmed = customer.EmailConfirmed,
            LastLoginAt = customer.LastLoginAt,
            Addresses = customer.Addresses
                .Where(x => x.DeletedAt == null)
                .Select(x => new CustomerAddressDto
                {
                    Id = x.Id,
                    CustomerId = x.CustomerId,
                    Title = x.Title,
                    FullName = x.FullName,
                    PhoneNumber = x.PhoneNumber,
                    Country = x.Country,
                    City = x.City,
                    District = x.District,
                    AddressLine1 = x.AddressLine1,
                    AddressLine2 = x.AddressLine2,
                    PostalCode = x.PostalCode,
                    IsDefaultShipping = x.IsDefaultShipping,
                    IsDefaultBilling = x.IsDefaultBilling
                })
                .ToList()
        };
    }
}