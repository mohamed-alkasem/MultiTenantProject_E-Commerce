param(
  [string]$Root = (Get-Location).Path
)

$ErrorActionPreference = "Stop"

function Ensure-Dir([string]$Path) {
  if (-not (Test-Path -LiteralPath $Path)) {
    New-Item -ItemType Directory -Path $Path | Out-Null
  }
}

function Ensure-File([string]$Path) {
  $dir = Split-Path -Parent $Path
  if ($dir) { Ensure-Dir $dir }
  if (-not (Test-Path -LiteralPath $Path)) {
    New-Item -ItemType File -Path $Path | Out-Null
  }
}

function Ensure-TextFile([string]$Path, [string]$Content) {
  $dir = Split-Path -Parent $Path
  if ($dir) { Ensure-Dir $dir }
  if (-not (Test-Path -LiteralPath $Path)) {
    Set-Content -LiteralPath $Path -Value $Content -Encoding UTF8
  }
}

$paths = @(
  "src\MultiTenantStore.Domain\Common",
  "src\MultiTenantStore.Domain\Enums",
  "src\MultiTenantStore.Domain\Platform",
  "src\MultiTenantStore.Domain\Tenant",

  "src\MultiTenantStore.Application",
  "src\MultiTenantStore.Application\Common\DTOs",
  "src\MultiTenantStore.Application\Common\Interfaces",
  "src\MultiTenantStore.Application\Common\Authorization",
  "src\MultiTenantStore.Application\Common\MultiTenancy",

  "src\MultiTenantStore.Application\Auth\DTOs",
  "src\MultiTenantStore.Application\Auth\Interfaces",
  "src\MultiTenantStore.Application\Auth\Services",
  "src\MultiTenantStore.Application\Auth\Validators",

  "src\MultiTenantStore.Application\Stores\DTOs",
  "src\MultiTenantStore.Application\Stores\Interfaces",
  "src\MultiTenantStore.Application\Stores\Services",
  "src\MultiTenantStore.Application\Stores\Validators",

  "src\MultiTenantStore.Application\Subscriptions\DTOs",
  "src\MultiTenantStore.Application\Subscriptions\Interfaces",
  "src\MultiTenantStore.Application\Subscriptions\Services",
  "src\MultiTenantStore.Application\Subscriptions\Validators",

  "src\MultiTenantStore.Application\Catalog\DTOs",
  "src\MultiTenantStore.Application\Catalog\Interfaces",
  "src\MultiTenantStore.Application\Catalog\Services",
  "src\MultiTenantStore.Application\Catalog\Validators",

  "src\MultiTenantStore.Application\Customers\DTOs",
  "src\MultiTenantStore.Application\Customers\Interfaces",
  "src\MultiTenantStore.Application\Customers\Services",
  "src\MultiTenantStore.Application\Customers\Validators",

  "src\MultiTenantStore.Application\Carts\DTOs",
  "src\MultiTenantStore.Application\Carts\Interfaces",
  "src\MultiTenantStore.Application\Carts\Services",
  "src\MultiTenantStore.Application\Carts\Validators",

  "src\MultiTenantStore.Application\Orders\DTOs",
  "src\MultiTenantStore.Application\Orders\Interfaces",
  "src\MultiTenantStore.Application\Orders\Services",
  "src\MultiTenantStore.Application\Orders\Validators",

  "src\MultiTenantStore.Application\Payments\DTOs",
  "src\MultiTenantStore.Application\Payments\Interfaces",
  "src\MultiTenantStore.Application\Payments\Services",
  "src\MultiTenantStore.Application\Payments\Validators",

  "src\MultiTenantStore.Application\Invoices\DTOs",
  "src\MultiTenantStore.Application\Invoices\Interfaces",
  "src\MultiTenantStore.Application\Invoices\Services",
  "src\MultiTenantStore.Application\Invoices\Validators",

  "src\MultiTenantStore.Application\Storage\DTOs",
  "src\MultiTenantStore.Application\Storage\Interfaces",
  "src\MultiTenantStore.Application\Storage\Validators",

  "src\MultiTenantStore.Persistence",
  "src\MultiTenantStore.Persistence\Contexts",
  "src\MultiTenantStore.Persistence\Configurations\Platform",
  "src\MultiTenantStore.Persistence\Configurations\Tenant",
  "src\MultiTenantStore.Persistence\Repositories\Generic",
  "src\MultiTenantStore.Persistence\Repositories\Platform",
  "src\MultiTenantStore.Persistence\Repositories\Tenant",
  "src\MultiTenantStore.Persistence\UnitOfWork",
  "src\MultiTenantStore.Persistence\Migrations\Main",
  "src\MultiTenantStore.Persistence\Migrations\Tenant",

  "src\MultiTenantStore.Infrastructure",
  "src\MultiTenantStore.Infrastructure\Identity",
  "src\MultiTenantStore.Infrastructure\Jwt",
  "src\MultiTenantStore.Infrastructure\Email",
  "src\MultiTenantStore.Infrastructure\Authorization",
  "src\MultiTenantStore.Infrastructure\MultiTenancy",
  "src\MultiTenantStore.Infrastructure\Storage",
  "src\MultiTenantStore.Infrastructure\Payments",
  "src\MultiTenantStore.Infrastructure\Invoices",
  "src\MultiTenantStore.Infrastructure\Services",

  "src\MultiTenantStore.Web\Controllers\Api\Platform",
  "src\MultiTenantStore.Web\Controllers\Api\Dashboard",
  "src\MultiTenantStore.Web\Controllers\Api\Public",
  "src\MultiTenantStore.Web\Controllers\Api\Customer",

  "src\MultiTenantStore.Web\Areas\Storefront\Controllers",
  "src\MultiTenantStore.Web\Areas\Storefront\Views\Home",
  "src\MultiTenantStore.Web\Areas\Storefront\Views\Products",
  "src\MultiTenantStore.Web\Areas\Storefront\Views\Categories",
  "src\MultiTenantStore.Web\Areas\Storefront\Views\Cart",
  "src\MultiTenantStore.Web\Areas\Storefront\Views\Checkout",
  "src\MultiTenantStore.Web\Areas\Storefront\Views\Customer",
  "src\MultiTenantStore.Web\Areas\Storefront\ViewModels",

  "src\MultiTenantStore.Web\Areas\Dashboard\Controllers",
  "src\MultiTenantStore.Web\Areas\Dashboard\Views\Home",
  "src\MultiTenantStore.Web\Areas\Dashboard\Views\Products",
  "src\MultiTenantStore.Web\Areas\Dashboard\Views\Categories",
  "src\MultiTenantStore.Web\Areas\Dashboard\Views\Orders",
  "src\MultiTenantStore.Web\Areas\Dashboard\Views\Customers",
  "src\MultiTenantStore.Web\Areas\Dashboard\Views\Invoices",
  "src\MultiTenantStore.Web\Areas\Dashboard\Views\Settings",
  "src\MultiTenantStore.Web\Areas\Dashboard\Views\Branding",
  "src\MultiTenantStore.Web\Areas\Dashboard\Views\Staff",
  "src\MultiTenantStore.Web\Areas\Dashboard\ViewModels",

  "src\MultiTenantStore.Web\Areas\PlatformAdmin\Controllers",
  "src\MultiTenantStore.Web\Areas\PlatformAdmin\Views\Dashboard",
  "src\MultiTenantStore.Web\Areas\PlatformAdmin\Views\Stores",
  "src\MultiTenantStore.Web\Areas\PlatformAdmin\Views\Plans",
  "src\MultiTenantStore.Web\Areas\PlatformAdmin\Views\Subscriptions",
  "src\MultiTenantStore.Web\Areas\PlatformAdmin\ViewModels",

  "src\MultiTenantStore.Web\Views\Shared",
  "src\MultiTenantStore.Web\Filters",
  "src\MultiTenantStore.Web\Middlewares",
  "src\MultiTenantStore.Web\Extensions",
  "src\MultiTenantStore.Web\wwwroot\css",
  "src\MultiTenantStore.Web\wwwroot\js",
  "src\MultiTenantStore.Web\wwwroot\images",
  "src\MultiTenantStore.Web\wwwroot\uploads-temp",

  "tests\MultiTenantStore.UnitTests\Domain",
  "tests\MultiTenantStore.UnitTests\Application",
  "tests\MultiTenantStore.UnitTests\Infrastructure",
  "tests\MultiTenantStore.IntegrationTests\Api",
  "tests\MultiTenantStore.IntegrationTests\Persistence",
  "tests\MultiTenantStore.IntegrationTests\MultiTenancy"
)

foreach ($p in $paths) {
  Ensure-Dir (Join-Path $Root $p)
}

$files = @(
  "MultiTenantStore.sln",

  "src\MultiTenantStore.Domain\Common\BaseEntity.cs",
  "src\MultiTenantStore.Domain\Common\AuditableEntity.cs",
  "src\MultiTenantStore.Domain\Common\ISoftDelete.cs",

  "src\MultiTenantStore.Domain\Enums\StoreStatus.cs",
  "src\MultiTenantStore.Domain\Enums\SubscriptionStatus.cs",
  "src\MultiTenantStore.Domain\Enums\ProvisioningStatus.cs",
  "src\MultiTenantStore.Domain\Enums\StoreUserRole.cs",
  "src\MultiTenantStore.Domain\Enums\BillingCycle.cs",
  "src\MultiTenantStore.Domain\Enums\CartStatus.cs",
  "src\MultiTenantStore.Domain\Enums\OrderStatus.cs",
  "src\MultiTenantStore.Domain\Enums\PaymentStatus.cs",
  "src\MultiTenantStore.Domain\Enums\ShippingStatus.cs",
  "src\MultiTenantStore.Domain\Enums\InvoiceStatus.cs",
  "src\MultiTenantStore.Domain\Enums\StorageProvider.cs",

  "src\MultiTenantStore.Domain\Platform\ApplicationUser.cs",
  "src\MultiTenantStore.Domain\Platform\Store.cs",
  "src\MultiTenantStore.Domain\Platform\StoreUser.cs",
  "src\MultiTenantStore.Domain\Platform\StoreDomain.cs",
  "src\MultiTenantStore.Domain\Platform\StoreDatabase.cs",
  "src\MultiTenantStore.Domain\Platform\StoreBranding.cs",
  "src\MultiTenantStore.Domain\Platform\SubscriptionPlan.cs",
  "src\MultiTenantStore.Domain\Platform\StoreSubscription.cs",
  "src\MultiTenantStore.Domain\Platform\SubscriptionPayment.cs",

  "src\MultiTenantStore.Domain\Tenant\StoreSetting.cs",
  "src\MultiTenantStore.Domain\Tenant\Category.cs",
  "src\MultiTenantStore.Domain\Tenant\Product.cs",
  "src\MultiTenantStore.Domain\Tenant\ProductVariant.cs",
  "src\MultiTenantStore.Domain\Tenant\ProductImage.cs",
  "src\MultiTenantStore.Domain\Tenant\Customer.cs",
  "src\MultiTenantStore.Domain\Tenant\CustomerAddress.cs",
  "src\MultiTenantStore.Domain\Tenant\Cart.cs",
  "src\MultiTenantStore.Domain\Tenant\CartItem.cs",
  "src\MultiTenantStore.Domain\Tenant\Order.cs",
  "src\MultiTenantStore.Domain\Tenant\OrderItem.cs",
  "src\MultiTenantStore.Domain\Tenant\Payment.cs",
  "src\MultiTenantStore.Domain\Tenant\Invoice.cs",
  "src\MultiTenantStore.Domain\Tenant\InvoiceItem.cs",

  "src\MultiTenantStore.Application\DependencyInjection.cs",
  "src\MultiTenantStore.Application\Common\DTOs\ApiResponseDto.cs",
  "src\MultiTenantStore.Application\Common\DTOs\PagedRequestDto.cs",
  "src\MultiTenantStore.Application\Common\DTOs\PagedResultDto.cs",
  "src\MultiTenantStore.Application\Common\Interfaces\IRepository.cs",
  "src\MultiTenantStore.Application\Common\Interfaces\IReadRepository.cs",
  "src\MultiTenantStore.Application\Common\Interfaces\IWriteRepository.cs",
  "src\MultiTenantStore.Application\Common\Interfaces\IPlatformUnitOfWork.cs",
  "src\MultiTenantStore.Application\Common\Interfaces\ITenantUnitOfWork.cs",
  "src\MultiTenantStore.Application\Common\Interfaces\IDateTimeService.cs",
  "src\MultiTenantStore.Application\Common\Authorization\Permissions.cs",
  "src\MultiTenantStore.Application\Common\Authorization\PolicyNames.cs",
  "src\MultiTenantStore.Application\Common\Authorization\StoreRolePermissions.cs",
  "src\MultiTenantStore.Application\Common\MultiTenancy\ICurrentTenant.cs",
  "src\MultiTenantStore.Application\Common\MultiTenancy\ISubdomainTenantResolver.cs",
  "src\MultiTenantStore.Application\Common\MultiTenancy\IClaimsTenantResolver.cs",
  "src\MultiTenantStore.Application\Common\MultiTenancy\ITenantStore.cs",
  "src\MultiTenantStore.Application\Common\MultiTenancy\ITenantAccessValidator.cs",
  "src\MultiTenantStore.Application\Common\MultiTenancy\TenantInfo.cs",
  "src\MultiTenantStore.Application\Common\MultiTenancy\TenantResolutionResult.cs",
  "src\MultiTenantStore.Application\Common\MultiTenancy\TenantClaimTypes.cs",

  "src\MultiTenantStore.Application\Auth\DTOs\RegisterMerchantDto.cs",
  "src\MultiTenantStore.Application\Auth\DTOs\LoginDto.cs",
  "src\MultiTenantStore.Application\Auth\DTOs\AuthResponseDto.cs",
  "src\MultiTenantStore.Application\Auth\DTOs\RefreshTokenRequestDto.cs",
  "src\MultiTenantStore.Application\Auth\DTOs\ChangePasswordDto.cs",
  "src\MultiTenantStore.Application\Auth\DTOs\AssignUserRoleDto.cs",
  "src\MultiTenantStore.Application\Auth\DTOs\RemoveUserRoleDto.cs",
  "src\MultiTenantStore.Application\Auth\DTOs\RoleDto.cs",
  "src\MultiTenantStore.Application\Auth\DTOs\CreateRoleDto.cs",
  "src\MultiTenantStore.Application\Auth\Interfaces\IAuthService.cs",
  "src\MultiTenantStore.Application\Auth\Interfaces\ITokenService.cs",
  "src\MultiTenantStore.Application\Auth\Interfaces\IRoleService.cs",
  "src\MultiTenantStore.Application\Auth\Services\AuthService.cs",
  "src\MultiTenantStore.Application\Auth\Services\TokenService.cs",
  "src\MultiTenantStore.Application\Auth\Services\RoleService.cs",
  "src\MultiTenantStore.Application\Auth\Validators\RegisterMerchantDtoValidator.cs",
  "src\MultiTenantStore.Application\Auth\Validators\LoginDtoValidator.cs",
  "src\MultiTenantStore.Application\Auth\Validators\RefreshTokenRequestDtoValidator.cs",
  "src\MultiTenantStore.Application\Auth\Validators\ChangePasswordDtoValidator.cs",

  "src\MultiTenantStore.Application\Stores\DTOs\CreateStoreDto.cs",
  "src\MultiTenantStore.Application\Stores\DTOs\UpdateStoreDto.cs",
  "src\MultiTenantStore.Application\Stores\DTOs\StoreDto.cs",
  "src\MultiTenantStore.Application\Stores\DTOs\StoreSummaryDto.cs",
  "src\MultiTenantStore.Application\Stores\DTOs\ActivateStoreDto.cs",
  "src\MultiTenantStore.Application\Stores\DTOs\SuspendStoreDto.cs",
  "src\MultiTenantStore.Application\Stores\DTOs\CreateStoreDomainDto.cs",
  "src\MultiTenantStore.Application\Stores\DTOs\UpdateStoreDomainDto.cs",
  "src\MultiTenantStore.Application\Stores\DTOs\VerifyStoreDomainDto.cs",
  "src\MultiTenantStore.Application\Stores\DTOs\StoreDomainDto.cs",
  "src\MultiTenantStore.Application\Stores\DTOs\StoreDatabaseDto.cs",
  "src\MultiTenantStore.Application\Stores\DTOs\CreateStoreDatabaseDto.cs",
  "src\MultiTenantStore.Application\Stores\DTOs\ProvisionStoreDatabaseDto.cs",
  "src\MultiTenantStore.Application\Stores\DTOs\RunTenantMigrationDto.cs",
  "src\MultiTenantStore.Application\Stores\DTOs\StoreBrandingDto.cs",
  "src\MultiTenantStore.Application\Stores\DTOs\UpdateStoreBrandingDto.cs",
  "src\MultiTenantStore.Application\Stores\DTOs\StoreUserDto.cs",
  "src\MultiTenantStore.Application\Stores\DTOs\StoreUserListDto.cs",
  "src\MultiTenantStore.Application\Stores\DTOs\InviteStoreUserDto.cs",
  "src\MultiTenantStore.Application\Stores\DTOs\CreateStoreUserDto.cs",
  "src\MultiTenantStore.Application\Stores\DTOs\UpdateStoreUserRoleDto.cs",
  "src\MultiTenantStore.Application\Stores\Interfaces\IStoreService.cs",
  "src\MultiTenantStore.Application\Stores\Interfaces\IStoreOnboardingService.cs",
  "src\MultiTenantStore.Application\Stores\Interfaces\IStoreUserService.cs",
  "src\MultiTenantStore.Application\Stores\Interfaces\IStoreDomainService.cs",
  "src\MultiTenantStore.Application\Stores\Interfaces\IStoreDatabaseService.cs",
  "src\MultiTenantStore.Application\Stores\Interfaces\ITenantMigrationService.cs",
  "src\MultiTenantStore.Application\Stores\Interfaces\ITenantSeedService.cs",
  "src\MultiTenantStore.Application\Stores\Interfaces\IStoreBrandingService.cs",
  "src\MultiTenantStore.Application\Stores\Services\StoreService.cs",
  "src\MultiTenantStore.Application\Stores\Services\StoreOnboardingService.cs",
  "src\MultiTenantStore.Application\Stores\Services\StoreUserService.cs",
  "src\MultiTenantStore.Application\Stores\Services\StoreDomainService.cs",
  "src\MultiTenantStore.Application\Stores\Services\StoreDatabaseService.cs",
  "src\MultiTenantStore.Application\Stores\Services\TenantMigrationService.cs",
  "src\MultiTenantStore.Application\Stores\Services\TenantSeedService.cs",
  "src\MultiTenantStore.Application\Stores\Services\StoreBrandingService.cs",
  "src\MultiTenantStore.Application\Stores\Validators\CreateStoreDtoValidator.cs",
  "src\MultiTenantStore.Application\Stores\Validators\UpdateStoreDtoValidator.cs",
  "src\MultiTenantStore.Application\Stores\Validators\CreateStoreDomainDtoValidator.cs",
  "src\MultiTenantStore.Application\Stores\Validators\InviteStoreUserDtoValidator.cs",
  "src\MultiTenantStore.Application\Stores\Validators\UpdateStoreBrandingDtoValidator.cs",

  "src\MultiTenantStore.Application\Subscriptions\DTOs\SubscriptionPlanDto.cs",
  "src\MultiTenantStore.Application\Subscriptions\DTOs\CreateSubscriptionPlanDto.cs",
  "src\MultiTenantStore.Application\Subscriptions\DTOs\UpdateSubscriptionPlanDto.cs",
  "src\MultiTenantStore.Application\Subscriptions\DTOs\CreateStoreSubscriptionDto.cs",
  "src\MultiTenantStore.Application\Subscriptions\DTOs\ChangeStorePlanDto.cs",
  "src\MultiTenantStore.Application\Subscriptions\DTOs\CancelStoreSubscriptionDto.cs",
  "src\MultiTenantStore.Application\Subscriptions\DTOs\StoreSubscriptionDto.cs",
  "src\MultiTenantStore.Application\Subscriptions\DTOs\CreateSubscriptionPaymentDto.cs",
  "src\MultiTenantStore.Application\Subscriptions\DTOs\SubscriptionPaymentDto.cs",
  "src\MultiTenantStore.Application\Subscriptions\Interfaces\ISubscriptionPlanService.cs",
  "src\MultiTenantStore.Application\Subscriptions\Interfaces\ISubscriptionService.cs",
  "src\MultiTenantStore.Application\Subscriptions\Services\SubscriptionPlanService.cs",
  "src\MultiTenantStore.Application\Subscriptions\Services\SubscriptionService.cs",
  "src\MultiTenantStore.Application\Subscriptions\Validators\CreateSubscriptionPlanDtoValidator.cs",
  "src\MultiTenantStore.Application\Subscriptions\Validators\UpdateSubscriptionPlanDtoValidator.cs",
  "src\MultiTenantStore.Application\Subscriptions\Validators\CreateStoreSubscriptionDtoValidator.cs",
  "src\MultiTenantStore.Application\Subscriptions\Validators\ChangeStorePlanDtoValidator.cs",

  "src\MultiTenantStore.Application\Catalog\DTOs\CategoryDto.cs",
  "src\MultiTenantStore.Application\Catalog\DTOs\CategoryTreeDto.cs",
  "src\MultiTenantStore.Application\Catalog\DTOs\CreateCategoryDto.cs",
  "src\MultiTenantStore.Application\Catalog\DTOs\UpdateCategoryDto.cs",
  "src\MultiTenantStore.Application\Catalog\DTOs\ProductDto.cs",
  "src\MultiTenantStore.Application\Catalog\DTOs\ProductListDto.cs",
  "src\MultiTenantStore.Application\Catalog\DTOs\ProductSearchRequestDto.cs",
  "src\MultiTenantStore.Application\Catalog\DTOs\CreateProductDto.cs",
  "src\MultiTenantStore.Application\Catalog\DTOs\UpdateProductDto.cs",
  "src\MultiTenantStore.Application\Catalog\DTOs\ProductVariantDto.cs",
  "src\MultiTenantStore.Application\Catalog\DTOs\CreateProductVariantDto.cs",
  "src\MultiTenantStore.Application\Catalog\DTOs\UpdateProductVariantDto.cs",
  "src\MultiTenantStore.Application\Catalog\DTOs\ProductImageDto.cs",
  "src\MultiTenantStore.Application\Catalog\DTOs\CreateProductImageDto.cs",
  "src\MultiTenantStore.Application\Catalog\DTOs\UpdateProductImageDto.cs",
  "src\MultiTenantStore.Application\Catalog\Interfaces\ICategoryService.cs",
  "src\MultiTenantStore.Application\Catalog\Interfaces\IProductService.cs",
  "src\MultiTenantStore.Application\Catalog\Interfaces\IProductVariantService.cs",
  "src\MultiTenantStore.Application\Catalog\Interfaces\IProductImageService.cs",
  "src\MultiTenantStore.Application\Catalog\Interfaces\IInventoryService.cs",
  "src\MultiTenantStore.Application\Catalog\Services\CategoryService.cs",
  "src\MultiTenantStore.Application\Catalog\Services\ProductService.cs",
  "src\MultiTenantStore.Application\Catalog\Services\ProductVariantService.cs",
  "src\MultiTenantStore.Application\Catalog\Services\ProductImageService.cs",
  "src\MultiTenantStore.Application\Catalog\Services\InventoryService.cs",
  "src\MultiTenantStore.Application\Catalog\Validators\CreateCategoryDtoValidator.cs",
  "src\MultiTenantStore.Application\Catalog\Validators\UpdateCategoryDtoValidator.cs",
  "src\MultiTenantStore.Application\Catalog\Validators\CreateProductDtoValidator.cs",
  "src\MultiTenantStore.Application\Catalog\Validators\UpdateProductDtoValidator.cs",
  "src\MultiTenantStore.Application\Catalog\Validators\CreateProductVariantDtoValidator.cs",
  "src\MultiTenantStore.Application\Catalog\Validators\UpdateProductVariantDtoValidator.cs",
  "src\MultiTenantStore.Application\Catalog\Validators\CreateProductImageDtoValidator.cs",

  "src\MultiTenantStore.Application\Customers\DTOs\RegisterCustomerDto.cs",
  "src\MultiTenantStore.Application\Customers\DTOs\CustomerLoginDto.cs",
  "src\MultiTenantStore.Application\Customers\DTOs\CustomerAuthResponseDto.cs",
  "src\MultiTenantStore.Application\Customers\DTOs\CreateCustomerDto.cs",
  "src\MultiTenantStore.Application\Customers\DTOs\UpdateCustomerDto.cs",
  "src\MultiTenantStore.Application\Customers\DTOs\CustomerDto.cs",
  "src\MultiTenantStore.Application\Customers\DTOs\CustomerListDto.cs",
  "src\MultiTenantStore.Application\Customers\DTOs\CustomerAddressDto.cs",
  "src\MultiTenantStore.Application\Customers\DTOs\CreateCustomerAddressDto.cs",
  "src\MultiTenantStore.Application\Customers\DTOs\UpdateCustomerAddressDto.cs",
  "src\MultiTenantStore.Application\Customers\Interfaces\ICustomerAuthService.cs",
  "src\MultiTenantStore.Application\Customers\Interfaces\ICustomerService.cs",
  "src\MultiTenantStore.Application\Customers\Interfaces\ICustomerAddressService.cs",
  "src\MultiTenantStore.Application\Customers\Services\CustomerAuthService.cs",
  "src\MultiTenantStore.Application\Customers\Services\CustomerService.cs",
  "src\MultiTenantStore.Application\Customers\Services\CustomerAddressService.cs",
  "src\MultiTenantStore.Application\Customers\Validators\RegisterCustomerDtoValidator.cs",
  "src\MultiTenantStore.Application\Customers\Validators\CustomerLoginDtoValidator.cs",
  "src\MultiTenantStore.Application\Customers\Validators\UpdateCustomerDtoValidator.cs",
  "src\MultiTenantStore.Application\Customers\Validators\CreateCustomerAddressDtoValidator.cs",
  "src\MultiTenantStore.Application\Customers\Validators\UpdateCustomerAddressDtoValidator.cs",

  "src\MultiTenantStore.Application\Carts\DTOs\CartDto.cs",
  "src\MultiTenantStore.Application\Carts\DTOs\CartItemDto.cs",
  "src\MultiTenantStore.Application\Carts\DTOs\AddToCartDto.cs",
  "src\MultiTenantStore.Application\Carts\DTOs\UpdateCartItemDto.cs",
  "src\MultiTenantStore.Application\Carts\DTOs\RemoveCartItemDto.cs",
  "src\MultiTenantStore.Application\Carts\DTOs\MergeCartDto.cs",
  "src\MultiTenantStore.Application\Carts\Interfaces\ICartService.cs",
  "src\MultiTenantStore.Application\Carts\Services\CartService.cs",
  "src\MultiTenantStore.Application\Carts\Validators\AddToCartDtoValidator.cs",
  "src\MultiTenantStore.Application\Carts\Validators\UpdateCartItemDtoValidator.cs",
  "src\MultiTenantStore.Application\Carts\Validators\MergeCartDtoValidator.cs",

  "src\MultiTenantStore.Application\Orders\DTOs\CreateOrderDto.cs",
  "src\MultiTenantStore.Application\Orders\DTOs\CreateOrderItemDto.cs",
  "src\MultiTenantStore.Application\Orders\DTOs\CreateOrderAddressDto.cs",
  "src\MultiTenantStore.Application\Orders\DTOs\OrderDto.cs",
  "src\MultiTenantStore.Application\Orders\DTOs\OrderListDto.cs",
  "src\MultiTenantStore.Application\Orders\DTOs\OrderItemDto.cs",
  "src\MultiTenantStore.Application\Orders\DTOs\OrderAddressDto.cs",
  "src\MultiTenantStore.Application\Orders\DTOs\UpdateOrderStatusDto.cs",
  "src\MultiTenantStore.Application\Orders\DTOs\UpdatePaymentStatusDto.cs",
  "src\MultiTenantStore.Application\Orders\DTOs\UpdateShippingStatusDto.cs",
  "src\MultiTenantStore.Application\Orders\DTOs\OrderSearchRequestDto.cs",
  "src\MultiTenantStore.Application\Orders\Interfaces\IOrderService.cs",
  "src\MultiTenantStore.Application\Orders\Interfaces\IOrderNumberService.cs",
  "src\MultiTenantStore.Application\Orders\Interfaces\ICheckoutService.cs",
  "src\MultiTenantStore.Application\Orders\Services\OrderService.cs",
  "src\MultiTenantStore.Application\Orders\Services\OrderNumberService.cs",
  "src\MultiTenantStore.Application\Orders\Services\CheckoutService.cs",
  "src\MultiTenantStore.Application\Orders\Validators\CreateOrderDtoValidator.cs",
  "src\MultiTenantStore.Application\Orders\Validators\UpdateOrderStatusDtoValidator.cs",
  "src\MultiTenantStore.Application\Orders\Validators\OrderSearchRequestDtoValidator.cs",

  "src\MultiTenantStore.Application\Payments\DTOs\CreatePaymentDto.cs",
  "src\MultiTenantStore.Application\Payments\DTOs\PaymentDto.cs",
  "src\MultiTenantStore.Application\Payments\DTOs\PaymentCallbackDto.cs",
  "src\MultiTenantStore.Application\Payments\DTOs\RefundPaymentDto.cs",
  "src\MultiTenantStore.Application\Payments\Interfaces\IPaymentService.cs",
  "src\MultiTenantStore.Application\Payments\Interfaces\IPaymentProviderService.cs",
  "src\MultiTenantStore.Application\Payments\Services\PaymentService.cs",
  "src\MultiTenantStore.Application\Payments\Services\PaymentProviderService.cs",
  "src\MultiTenantStore.Application\Payments\Validators\CreatePaymentDtoValidator.cs",
  "src\MultiTenantStore.Application\Payments\Validators\PaymentCallbackDtoValidator.cs",
  "src\MultiTenantStore.Application\Payments\Validators\RefundPaymentDtoValidator.cs",

  "src\MultiTenantStore.Application\Invoices\DTOs\CreateInvoiceDto.cs",
  "src\MultiTenantStore.Application\Invoices\DTOs\InvoiceDto.cs",
  "src\MultiTenantStore.Application\Invoices\DTOs\InvoiceItemDto.cs",
  "src\MultiTenantStore.Application\Invoices\DTOs\InvoiceListDto.cs",
  "src\MultiTenantStore.Application\Invoices\DTOs\CancelInvoiceDto.cs",
  "src\MultiTenantStore.Application\Invoices\DTOs\MarkInvoicePaidDto.cs",
  "src\MultiTenantStore.Application\Invoices\DTOs\RegenerateInvoicePdfDto.cs",
  "src\MultiTenantStore.Application\Invoices\Interfaces\IInvoiceService.cs",
  "src\MultiTenantStore.Application\Invoices\Interfaces\IInvoiceNumberService.cs",
  "src\MultiTenantStore.Application\Invoices\Interfaces\IInvoicePdfService.cs",
  "src\MultiTenantStore.Application\Invoices\Interfaces\IInvoiceTemplateBuilder.cs",
  "src\MultiTenantStore.Application\Invoices\Services\InvoiceService.cs",
  "src\MultiTenantStore.Application\Invoices\Services\InvoiceNumberService.cs",
  "src\MultiTenantStore.Application\Invoices\Validators\CreateInvoiceDtoValidator.cs",
  "src\MultiTenantStore.Application\Invoices\Validators\CancelInvoiceDtoValidator.cs",
  "src\MultiTenantStore.Application\Invoices\Validators\RegenerateInvoicePdfDtoValidator.cs",

  "src\MultiTenantStore.Application\Storage\DTOs\FileUploadRequestDto.cs",
  "src\MultiTenantStore.Application\Storage\DTOs\FileUploadResultDto.cs",
  "src\MultiTenantStore.Application\Storage\Interfaces\IFileStorageService.cs",
  "src\MultiTenantStore.Application\Storage\Validators\FileUploadRequestDtoValidator.cs",

  "src\MultiTenantStore.Persistence\DependencyInjection.cs",
  "src\MultiTenantStore.Persistence\Contexts\MainDbContext.cs",
  "src\MultiTenantStore.Persistence\Contexts\TenantDbContext.cs",

  "src\MultiTenantStore.Persistence\Configurations\Platform\ApplicationUserConfiguration.cs",
  "src\MultiTenantStore.Persistence\Configurations\Platform\StoreConfiguration.cs",
  "src\MultiTenantStore.Persistence\Configurations\Platform\StoreUserConfiguration.cs",
  "src\MultiTenantStore.Persistence\Configurations\Platform\StoreDomainConfiguration.cs",
  "src\MultiTenantStore.Persistence\Configurations\Platform\StoreDatabaseConfiguration.cs",
  "src\MultiTenantStore.Persistence\Configurations\Platform\StoreBrandingConfiguration.cs",
  "src\MultiTenantStore.Persistence\Configurations\Platform\SubscriptionPlanConfiguration.cs",
  "src\MultiTenantStore.Persistence\Configurations\Platform\StoreSubscriptionConfiguration.cs",
  "src\MultiTenantStore.Persistence\Configurations\Platform\SubscriptionPaymentConfiguration.cs",

  "src\MultiTenantStore.Persistence\Configurations\Tenant\StoreSettingConfiguration.cs",
  "src\MultiTenantStore.Persistence\Configurations\Tenant\CategoryConfiguration.cs",
  "src\MultiTenantStore.Persistence\Configurations\Tenant\ProductConfiguration.cs",
  "src\MultiTenantStore.Persistence\Configurations\Tenant\ProductVariantConfiguration.cs",
  "src\MultiTenantStore.Persistence\Configurations\Tenant\ProductImageConfiguration.cs",
  "src\MultiTenantStore.Persistence\Configurations\Tenant\CustomerConfiguration.cs",
  "src\MultiTenantStore.Persistence\Configurations\Tenant\CustomerAddressConfiguration.cs",
  "src\MultiTenantStore.Persistence\Configurations\Tenant\CartConfiguration.cs",
  "src\MultiTenantStore.Persistence\Configurations\Tenant\CartItemConfiguration.cs",
  "src\MultiTenantStore.Persistence\Configurations\Tenant\OrderConfiguration.cs",
  "src\MultiTenantStore.Persistence\Configurations\Tenant\OrderItemConfiguration.cs",
  "src\MultiTenantStore.Persistence\Configurations\Tenant\PaymentConfiguration.cs",
  "src\MultiTenantStore.Persistence\Configurations\Tenant\InvoiceConfiguration.cs",
  "src\MultiTenantStore.Persistence\Configurations\Tenant\InvoiceItemConfiguration.cs",

  "src\MultiTenantStore.Persistence\Repositories\Generic\Repository.cs",
  "src\MultiTenantStore.Persistence\Repositories\Generic\ReadRepository.cs",
  "src\MultiTenantStore.Persistence\Repositories\Generic\WriteRepository.cs",

  "src\MultiTenantStore.Persistence\Repositories\Platform\ApplicationUserQueryRepository.cs",
  "src\MultiTenantStore.Persistence\Repositories\Platform\StoreRepository.cs",
  "src\MultiTenantStore.Persistence\Repositories\Platform\StoreUserRepository.cs",
  "src\MultiTenantStore.Persistence\Repositories\Platform\StoreDomainRepository.cs",
  "src\MultiTenantStore.Persistence\Repositories\Platform\StoreDatabaseRepository.cs",
  "src\MultiTenantStore.Persistence\Repositories\Platform\StoreBrandingRepository.cs",
  "src\MultiTenantStore.Persistence\Repositories\Platform\SubscriptionPlanRepository.cs",
  "src\MultiTenantStore.Persistence\Repositories\Platform\StoreSubscriptionRepository.cs",
  "src\MultiTenantStore.Persistence\Repositories\Platform\SubscriptionPaymentRepository.cs",

  "src\MultiTenantStore.Persistence\Repositories\Tenant\StoreSettingRepository.cs",
  "src\MultiTenantStore.Persistence\Repositories\Tenant\CategoryRepository.cs",
  "src\MultiTenantStore.Persistence\Repositories\Tenant\ProductRepository.cs",
  "src\MultiTenantStore.Persistence\Repositories\Tenant\ProductVariantRepository.cs",
  "src\MultiTenantStore.Persistence\Repositories\Tenant\ProductImageRepository.cs",
  "src\MultiTenantStore.Persistence\Repositories\Tenant\CustomerRepository.cs",
  "src\MultiTenantStore.Persistence\Repositories\Tenant\CustomerAddressRepository.cs",
  "src\MultiTenantStore.Persistence\Repositories\Tenant\CartRepository.cs",
  "src\MultiTenantStore.Persistence\Repositories\Tenant\CartItemRepository.cs",
  "src\MultiTenantStore.Persistence\Repositories\Tenant\OrderRepository.cs",
  "src\MultiTenantStore.Persistence\Repositories\Tenant\OrderItemRepository.cs",
  "src\MultiTenantStore.Persistence\Repositories\Tenant\PaymentRepository.cs",
  "src\MultiTenantStore.Persistence\Repositories\Tenant\InvoiceRepository.cs",
  "src\MultiTenantStore.Persistence\Repositories\Tenant\InvoiceItemRepository.cs",

  "src\MultiTenantStore.Persistence\UnitOfWork\PlatformUnitOfWork.cs",
  "src\MultiTenantStore.Persistence\UnitOfWork\TenantUnitOfWork.cs",

  "src\MultiTenantStore.Infrastructure\DependencyInjection.cs",
  "src\MultiTenantStore.Infrastructure\Identity\IdentitySeeder.cs",
  "src\MultiTenantStore.Infrastructure\Identity\IdentityRoleConstants.cs",
  "src\MultiTenantStore.Infrastructure\Jwt\JwtOptions.cs",
  "src\MultiTenantStore.Infrastructure\Jwt\TokenService.cs",
  "src\MultiTenantStore.Infrastructure\Jwt\RefreshTokenService.cs",
  "src\MultiTenantStore.Infrastructure\Email\EmailOptions.cs",
  "src\MultiTenantStore.Infrastructure\Email\EmailService.cs",
  "src\MultiTenantStore.Infrastructure\Authorization\PermissionAuthorizationHandler.cs",
  "src\MultiTenantStore.Infrastructure\Authorization\StoreRoleRequirement.cs",
  "src\MultiTenantStore.Infrastructure\Authorization\PermissionPolicyProvider.cs",
  "src\MultiTenantStore.Infrastructure\MultiTenancy\CurrentTenant.cs",
  "src\MultiTenantStore.Infrastructure\MultiTenancy\SubdomainTenantResolver.cs",
  "src\MultiTenantStore.Infrastructure\MultiTenancy\ClaimsTenantResolver.cs",
  "src\MultiTenantStore.Infrastructure\MultiTenancy\TenantStore.cs",
  "src\MultiTenantStore.Infrastructure\MultiTenancy\TenantAccessValidator.cs",
  "src\MultiTenantStore.Infrastructure\MultiTenancy\TenantRouteRules.cs",
  "src\MultiTenantStore.Infrastructure\MultiTenancy\TenantResolutionOptions.cs",
  "src\MultiTenantStore.Infrastructure\MultiTenancy\MultiTenancyServiceCollectionExtensions.cs",
  "src\MultiTenantStore.Infrastructure\Storage\S3StorageService.cs",
  "src\MultiTenantStore.Infrastructure\Storage\S3StorageOptions.cs",
  "src\MultiTenantStore.Infrastructure\Storage\FileStoragePathBuilder.cs",
  "src\MultiTenantStore.Infrastructure\Payments\CashOnDeliveryProvider.cs",
  "src\MultiTenantStore.Infrastructure\Payments\FakePaymentProvider.cs",
  "src\MultiTenantStore.Infrastructure\Invoices\QuestPdfInvoiceService.cs",
  "src\MultiTenantStore.Infrastructure\Invoices\QuestPdfInvoiceTemplateBuilder.cs",
  "src\MultiTenantStore.Infrastructure\Services\DateTimeService.cs",

  "src\MultiTenantStore.Web\Controllers\Api\AuthController.cs",
  "src\MultiTenantStore.Web\Controllers\Api\CartController.cs",
  "src\MultiTenantStore.Web\Controllers\Api\CheckoutController.cs",
  "src\MultiTenantStore.Web\Controllers\Api\PaymentCallbackController.cs",

  "src\MultiTenantStore.Web\Controllers\Api\Platform\PlatformStoresController.cs",
  "src\MultiTenantStore.Web\Controllers\Api\Platform\PlatformPlansController.cs",
  "src\MultiTenantStore.Web\Controllers\Api\Platform\PlatformSubscriptionsController.cs",
  "src\MultiTenantStore.Web\Controllers\Api\Platform\PlatformSubscriptionPaymentsController.cs",

  "src\MultiTenantStore.Web\Controllers\Api\Dashboard\DashboardStoreController.cs",
  "src\MultiTenantStore.Web\Controllers\Api\Dashboard\DashboardBrandingController.cs",
  "src\MultiTenantStore.Web\Controllers\Api\Dashboard\DashboardSettingsController.cs",
  "src\MultiTenantStore.Web\Controllers\Api\Dashboard\DashboardStaffController.cs",
  "src\MultiTenantStore.Web\Controllers\Api\Dashboard\DashboardCategoriesController.cs",
  "src\MultiTenantStore.Web\Controllers\Api\Dashboard\DashboardProductsController.cs",
  "src\MultiTenantStore.Web\Controllers\Api\Dashboard\DashboardProductVariantsController.cs",
  "src\MultiTenantStore.Web\Controllers\Api\Dashboard\DashboardProductImagesController.cs",
  "src\MultiTenantStore.Web\Controllers\Api\Dashboard\DashboardCustomersController.cs",
  "src\MultiTenantStore.Web\Controllers\Api\Dashboard\DashboardOrdersController.cs",
  "src\MultiTenantStore.Web\Controllers\Api\Dashboard\DashboardPaymentsController.cs",
  "src\MultiTenantStore.Web\Controllers\Api\Dashboard\DashboardInvoicesController.cs",
  "src\MultiTenantStore.Web\Controllers\Api\Dashboard\DashboardStorageController.cs",

  "src\MultiTenantStore.Web\Controllers\Api\Public\PublicStoreController.cs",
  "src\MultiTenantStore.Web\Controllers\Api\Public\PublicCategoriesController.cs",
  "src\MultiTenantStore.Web\Controllers\Api\Public\PublicProductsController.cs",

  "src\MultiTenantStore.Web\Controllers\Api\Customer\CustomerAuthController.cs",
  "src\MultiTenantStore.Web\Controllers\Api\Customer\CustomerProfileController.cs",
  "src\MultiTenantStore.Web\Controllers\Api\Customer\CustomerAddressesController.cs",
  "src\MultiTenantStore.Web\Controllers\Api\Customer\CustomerOrdersController.cs",
  "src\MultiTenantStore.Web\Controllers\Api\Customer\CustomerInvoicesController.cs",

  "src\MultiTenantStore.Web\Areas\Storefront\Controllers\HomeController.cs",
  "src\MultiTenantStore.Web\Areas\Storefront\Controllers\ProductsController.cs",
  "src\MultiTenantStore.Web\Areas\Storefront\Controllers\CategoriesController.cs",
  "src\MultiTenantStore.Web\Areas\Storefront\Controllers\CartController.cs",
  "src\MultiTenantStore.Web\Areas\Storefront\Controllers\CheckoutController.cs",
  "src\MultiTenantStore.Web\Areas\Storefront\Controllers\CustomerController.cs",
  "src\MultiTenantStore.Web\Areas\Storefront\ViewModels\HomePageViewModel.cs",
  "src\MultiTenantStore.Web\Areas\Storefront\ViewModels\ProductListViewModel.cs",
  "src\MultiTenantStore.Web\Areas\Storefront\ViewModels\ProductDetailsViewModel.cs",
  "src\MultiTenantStore.Web\Areas\Storefront\ViewModels\CartViewModel.cs",
  "src\MultiTenantStore.Web\Areas\Storefront\ViewModels\CheckoutViewModel.cs",

  "src\MultiTenantStore.Web\Areas\Dashboard\Controllers\DashboardHomeController.cs",
  "src\MultiTenantStore.Web\Areas\Dashboard\Controllers\DashboardProductsController.cs",
  "src\MultiTenantStore.Web\Areas\Dashboard\Controllers\DashboardCategoriesController.cs",
  "src\MultiTenantStore.Web\Areas\Dashboard\Controllers\DashboardOrdersController.cs",
  "src\MultiTenantStore.Web\Areas\Dashboard\Controllers\DashboardCustomersController.cs",
  "src\MultiTenantStore.Web\Areas\Dashboard\Controllers\DashboardInvoicesController.cs",
  "src\MultiTenantStore.Web\Areas\Dashboard\Controllers\DashboardSettingsController.cs",
  "src\MultiTenantStore.Web\Areas\Dashboard\Controllers\DashboardBrandingController.cs",
  "src\MultiTenantStore.Web\Areas\Dashboard\Controllers\DashboardStaffController.cs",
  "src\MultiTenantStore.Web\Areas\Dashboard\ViewModels\DashboardHomeViewModel.cs",
  "src\MultiTenantStore.Web\Areas\Dashboard\ViewModels\ProductFormViewModel.cs",
  "src\MultiTenantStore.Web\Areas\Dashboard\ViewModels\ProductListViewModel.cs",
  "src\MultiTenantStore.Web\Areas\Dashboard\ViewModels\OrderDetailsViewModel.cs",
  "src\MultiTenantStore.Web\Areas\Dashboard\ViewModels\InvoiceDetailsViewModel.cs",
  "src\MultiTenantStore.Web\Areas\Dashboard\ViewModels\SettingsViewModel.cs",
  "src\MultiTenantStore.Web\Areas\Dashboard\ViewModels\BrandingViewModel.cs",

  "src\MultiTenantStore.Web\Areas\PlatformAdmin\Controllers\PlatformDashboardController.cs",
  "src\MultiTenantStore.Web\Areas\PlatformAdmin\Controllers\PlatformStoresController.cs",
  "src\MultiTenantStore.Web\Areas\PlatformAdmin\Controllers\PlatformPlansController.cs",
  "src\MultiTenantStore.Web\Areas\PlatformAdmin\Controllers\PlatformSubscriptionsController.cs",
  "src\MultiTenantStore.Web\Areas\PlatformAdmin\ViewModels\PlatformDashboardViewModel.cs",
  "src\MultiTenantStore.Web\Areas\PlatformAdmin\ViewModels\StoreDetailsViewModel.cs",

  "src\MultiTenantStore.Web\Views\Shared\_Layout.cshtml",
  "src\MultiTenantStore.Web\Views\Shared\_StorefrontLayout.cshtml",
  "src\MultiTenantStore.Web\Views\Shared\_DashboardLayout.cshtml",
  "src\MultiTenantStore.Web\Views\Shared\_PlatformAdminLayout.cshtml",
  "src\MultiTenantStore.Web\Views\Shared\_ValidationScriptsPartial.cshtml",
  "src\MultiTenantStore.Web\Views\Shared\Error.cshtml",

  "src\MultiTenantStore.Web\Filters\ValidateModelFilter.cs",
  "src\MultiTenantStore.Web\Middlewares\ExceptionHandlingMiddleware.cs",
  "src\MultiTenantStore.Web\Middlewares\TenantResolutionMiddleware.cs",
  "src\MultiTenantStore.Web\Extensions\MiddlewareExtensions.cs",
  "src\MultiTenantStore.Web\Extensions\WebApplicationExtensions.cs",
  "src\MultiTenantStore.Web\Program.cs",
  "src\MultiTenantStore.Web\appsettings.json",

  "tests\MultiTenantStore.UnitTests\MultiTenantStore.UnitTests.csproj",
  "tests\MultiTenantStore.IntegrationTests\MultiTenantStore.IntegrationTests.csproj"
)

foreach ($f in $files) {
  Ensure-File (Join-Path $Root $f)
}

# Create an actual empty .sln if dotnet exists and file is still empty
$slnPath = Join-Path $Root "MultiTenantStore.sln"
try {
  $dotnet = Get-Command dotnet -ErrorAction Stop
  $len = (Get-Item -LiteralPath $slnPath).Length
  if ($len -eq 0) {
    Push-Location $Root
    try {
      dotnet new sln -n "MultiTenantStore" | Out-Null
    } finally {
      Pop-Location
    }
  }
} catch {
  # dotnet not installed - keep placeholder file
}

# Minimal placeholder csproj files for tests if they are empty
$unitProj = Join-Path $Root "tests\MultiTenantStore.UnitTests\MultiTenantStore.UnitTests.csproj"
if ((Get-Item -LiteralPath $unitProj).Length -eq 0) {
  Ensure-TextFile $unitProj @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
</Project>
"@
}

$intProj = Join-Path $Root "tests\MultiTenantStore.IntegrationTests\MultiTenantStore.IntegrationTests.csproj"
if ((Get-Item -LiteralPath $intProj).Length -eq 0) {
  Ensure-TextFile $intProj @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
</Project>
"@
}

Write-Host "Done. Structure created under: $Root"
