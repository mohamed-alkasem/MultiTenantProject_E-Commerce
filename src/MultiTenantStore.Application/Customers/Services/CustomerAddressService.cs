using MultiTenantStore.Application.Common.DTOs;
using MultiTenantStore.Application.Common.Interfaces;
using MultiTenantStore.Application.Customers.DTOs;
using MultiTenantStore.Application.Customers.Repositories;
using MultiTenantStore.Domain.Tenant;

namespace MultiTenantStore.Application.Customers.Services;

public sealed class CustomerAddressService : ICustomerAddressService
{
    private readonly ICustomerAddressRepository _addressRepository;
    private readonly ICurrentCustomerService _currentCustomerService;
    private readonly ITenantUnitOfWork _unitOfWork;

    public CustomerAddressService(
        ICustomerAddressRepository addressRepository,
        ICurrentCustomerService currentCustomerService,
        ITenantUnitOfWork unitOfWork)
    {
        _addressRepository = addressRepository;
        _currentCustomerService = currentCustomerService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto<List<CustomerAddressDto>>> GetAddressesAsync(
        CancellationToken cancellationToken = default)
    {
        var customerId = _currentCustomerService.CustomerId;

        if (customerId is null)
        {
            return ApiResponseDto<List<CustomerAddressDto>>.Fail("Customer is not authenticated.");
        }

        var addresses = await _addressRepository.GetByCustomerIdAsync(
            customerId.Value,
            cancellationToken);

        return ApiResponseDto<List<CustomerAddressDto>>.Ok(
            addresses.Select(MapToDto).ToList());
    }

    public async Task<ApiResponseDto<CustomerAddressDto>> AddAddressAsync(
        CreateCustomerAddressDto dto,
        CancellationToken cancellationToken = default)
    {
        var customerId = _currentCustomerService.CustomerId;

        if (customerId is null)
        {
            return ApiResponseDto<CustomerAddressDto>.Fail("Customer is not authenticated.");
        }

        if (dto.IsDefaultShipping)
        {
            await ClearDefaultShippingAsync(customerId.Value, cancellationToken);
        }

        if (dto.IsDefaultBilling)
        {
            await ClearDefaultBillingAsync(customerId.Value, cancellationToken);
        }

        var address = new CustomerAddress
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId.Value,
            Title = dto.Title.Trim(),
            FullName = dto.FullName.Trim(),
            PhoneNumber = dto.PhoneNumber.Trim(),
            Country = dto.Country.Trim(),
            City = dto.City.Trim(),
            District = dto.District,
            AddressLine1 = dto.AddressLine1.Trim(),
            AddressLine2 = dto.AddressLine2,
            PostalCode = dto.PostalCode,
            IsDefaultShipping = dto.IsDefaultShipping,
            IsDefaultBilling = dto.IsDefaultBilling,
            CreatedAt = DateTime.UtcNow
        };

        await _addressRepository.AddAsync(address, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<CustomerAddressDto>.Ok(
            MapToDto(address),
            "Customer address created successfully.");
    }

    public async Task<ApiResponseDto<CustomerAddressDto>> UpdateAddressAsync(
        UpdateCustomerAddressDto dto,
        CancellationToken cancellationToken = default)
    {
        var customerId = _currentCustomerService.CustomerId;

        if (customerId is null)
        {
            return ApiResponseDto<CustomerAddressDto>.Fail("Customer is not authenticated.");
        }

        var address = await _addressRepository.GetByIdAndCustomerIdAsync(
            dto.Id,
            customerId.Value,
            cancellationToken);

        if (address is null)
        {
            return ApiResponseDto<CustomerAddressDto>.Fail("Customer address was not found.");
        }

        if (dto.IsDefaultShipping && !address.IsDefaultShipping)
        {
            await ClearDefaultShippingAsync(customerId.Value, cancellationToken);
        }

        if (dto.IsDefaultBilling && !address.IsDefaultBilling)
        {
            await ClearDefaultBillingAsync(customerId.Value, cancellationToken);
        }

        address.Title = dto.Title.Trim();
        address.FullName = dto.FullName.Trim();
        address.PhoneNumber = dto.PhoneNumber.Trim();
        address.Country = dto.Country.Trim();
        address.City = dto.City.Trim();
        address.District = dto.District;
        address.AddressLine1 = dto.AddressLine1.Trim();
        address.AddressLine2 = dto.AddressLine2;
        address.PostalCode = dto.PostalCode;
        address.IsDefaultShipping = dto.IsDefaultShipping;
        address.IsDefaultBilling = dto.IsDefaultBilling;
        address.UpdatedAt = DateTime.UtcNow;

        _addressRepository.Update(address);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<CustomerAddressDto>.Ok(
            MapToDto(address),
            "Customer address updated successfully.");
    }

    public async Task<ApiResponseDto<bool>> DeleteAddressAsync(
        Guid addressId,
        CancellationToken cancellationToken = default)
    {
        var customerId = _currentCustomerService.CustomerId;

        if (customerId is null)
        {
            return ApiResponseDto<bool>.Fail("Customer is not authenticated.");
        }

        var address = await _addressRepository.GetByIdAndCustomerIdAsync(
            addressId,
            customerId.Value,
            cancellationToken);

        if (address is null)
        {
            return ApiResponseDto<bool>.Fail("Customer address was not found.");
        }

        address.DeletedAt = DateTime.UtcNow;
        address.UpdatedAt = DateTime.UtcNow;

        _addressRepository.Update(address);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<bool>.Ok(true, "Customer address deleted successfully.");
    }

    public async Task<ApiResponseDto<CustomerAddressDto>> SetDefaultShippingAsync(
        Guid addressId,
        CancellationToken cancellationToken = default)
    {
        var customerId = _currentCustomerService.CustomerId;

        if (customerId is null)
        {
            return ApiResponseDto<CustomerAddressDto>.Fail("Customer is not authenticated.");
        }

        var address = await _addressRepository.GetByIdAndCustomerIdAsync(
            addressId,
            customerId.Value,
            cancellationToken);

        if (address is null)
        {
            return ApiResponseDto<CustomerAddressDto>.Fail("Customer address was not found.");
        }

        await ClearDefaultShippingAsync(customerId.Value, cancellationToken);

        address.IsDefaultShipping = true;
        address.UpdatedAt = DateTime.UtcNow;

        _addressRepository.Update(address);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<CustomerAddressDto>.Ok(
            MapToDto(address),
            "Default shipping address updated successfully.");
    }

    public async Task<ApiResponseDto<CustomerAddressDto>> SetDefaultBillingAsync(
        Guid addressId,
        CancellationToken cancellationToken = default)
    {
        var customerId = _currentCustomerService.CustomerId;

        if (customerId is null)
        {
            return ApiResponseDto<CustomerAddressDto>.Fail("Customer is not authenticated.");
        }

        var address = await _addressRepository.GetByIdAndCustomerIdAsync(
            addressId,
            customerId.Value,
            cancellationToken);

        if (address is null)
        {
            return ApiResponseDto<CustomerAddressDto>.Fail("Customer address was not found.");
        }

        await ClearDefaultBillingAsync(customerId.Value, cancellationToken);

        address.IsDefaultBilling = true;
        address.UpdatedAt = DateTime.UtcNow;

        _addressRepository.Update(address);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponseDto<CustomerAddressDto>.Ok(
            MapToDto(address),
            "Default billing address updated successfully.");
    }

    private async Task ClearDefaultShippingAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        var currentDefault = await _addressRepository.GetDefaultShippingAsync(
            customerId,
            cancellationToken);

        if (currentDefault is null)
        {
            return;
        }

        currentDefault.IsDefaultShipping = false;
        currentDefault.UpdatedAt = DateTime.UtcNow;

        _addressRepository.Update(currentDefault);
    }

    private async Task ClearDefaultBillingAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        var currentDefault = await _addressRepository.GetDefaultBillingAsync(
            customerId,
            cancellationToken);

        if (currentDefault is null)
        {
            return;
        }

        currentDefault.IsDefaultBilling = false;
        currentDefault.UpdatedAt = DateTime.UtcNow;

        _addressRepository.Update(currentDefault);
    }

    private static CustomerAddressDto MapToDto(CustomerAddress address)
    {
        return new CustomerAddressDto
        {
            Id = address.Id,
            CustomerId = address.CustomerId,
            Title = address.Title,
            FullName = address.FullName,
            PhoneNumber = address.PhoneNumber,
            Country = address.Country,
            City = address.City,
            District = address.District,
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            PostalCode = address.PostalCode,
            IsDefaultShipping = address.IsDefaultShipping,
            IsDefaultBilling = address.IsDefaultBilling
        };
    }
}