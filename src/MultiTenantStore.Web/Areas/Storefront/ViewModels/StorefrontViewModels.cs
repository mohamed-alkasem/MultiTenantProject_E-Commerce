using MultiTenantStore.Application.Carts.DTOs;
using MultiTenantStore.Application.Customers.DTOs;
using MultiTenantStore.Application.Orders.DTOs;
using MultiTenantStore.Application.Storefront.DTOs;

namespace MultiTenantStore.Web.Areas.Storefront.ViewModels;

public sealed class StorefrontContext
{
    public string StoreSlug { get; set; } = "";
    public string StoreName { get; set; } = "";
    public string? LogoUrl { get; set; }
    public string PrimaryColor { get; set; } = "#3498db";
    public string SecondaryColor { get; set; } = "#2ecc71";
    public string Lang { get; set; } = "ar";
    public bool IsAr => Lang == "ar";
    public int CartCount { get; set; }
    public bool IsCustomerLoggedIn { get; set; }
    public string? CustomerName { get; set; }
}

public sealed class HomePageViewModel
{
    public StorefrontContext Store { get; set; } = new();
    public List<PublicProductListDto> FeaturedProducts { get; set; } = new();
    public List<PublicCategoryDto> Categories { get; set; } = new();
}

public sealed class ProductListViewModel
{
    public StorefrontContext Store { get; set; } = new();
    public List<PublicProductListDto> Products { get; set; } = new();
    public List<PublicCategoryDto> Categories { get; set; } = new();
    public string? Search { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int PageNumber { get; set; } = 1;
    public int TotalCount { get; set; }
    public int PageSize { get; set; } = 12;
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

public sealed class ProductDetailsViewModel
{
    public StorefrontContext Store { get; set; } = new();
    public PublicProductDetailsDto Product { get; set; } = default!;
}

public sealed class CartPageViewModel
{
    public StorefrontContext Store { get; set; } = new();
    public CartDto? Cart { get; set; }
}

public sealed class CheckoutViewModel
{
    public StorefrontContext Store { get; set; } = new();
    public CartDto? Cart { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Country { get; set; } = "";
    public string City { get; set; } = "";
    public string? District { get; set; }
    public string AddressLine1 { get; set; } = "";
    public string? AddressLine2 { get; set; }
    public string? PostalCode { get; set; }
    public string PaymentMethod { get; set; } = "CashOnDelivery";
    public bool SameBillingAddress { get; set; } = true;
}

public sealed class OrderConfirmationViewModel
{
    public StorefrontContext Store { get; set; } = new();
    public string OrderNumber { get; set; } = "";
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public string? InvoiceNumber { get; set; }
    public string? InvoicePdfUrl { get; set; }
}

public sealed class CustomerLoginViewModel
{
    public StorefrontContext Store { get; set; } = new();
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string? ReturnUrl { get; set; }
}

public sealed class CustomerRegisterViewModel
{
    public StorefrontContext Store { get; set; } = new();
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string ConfirmPassword { get; set; } = "";
    public string? PhoneNumber { get; set; }
}

public sealed class CustomerOrdersViewModel
{
    public StorefrontContext Store { get; set; } = new();
    public List<OrderListDto> Orders { get; set; } = new();
    public string CustomerName { get; set; } = "";
}

public sealed class CustomerProfileViewModel
{
    public StorefrontContext Store { get; set; } = new();
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string? PhoneNumber { get; set; }
}

public sealed class CustomerAddressesViewModel
{
    public StorefrontContext Store { get; set; } = new();
    public List<CustomerAddressDto> Addresses { get; set; } = new();

    // New address form fields
    public string Title { get; set; } = "";
    public string FullName { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string Country { get; set; } = "";
    public string City { get; set; } = "";
    public string? District { get; set; }
    public string AddressLine1 { get; set; } = "";
    public string? AddressLine2 { get; set; }
    public string? PostalCode { get; set; }
    public bool IsDefaultShipping { get; set; }
    public bool IsDefaultBilling { get; set; }
}
