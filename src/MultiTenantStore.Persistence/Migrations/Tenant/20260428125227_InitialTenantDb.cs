using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiTenantStore.Persistence.Migrations.Tenant
{
    /// <inheritdoc />
    public partial class InitialTenantDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ASP_NET_USERS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ASP_NET_USERS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CATEGORY",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CATEGORY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CATEGORY_CATEGORY_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "CATEGORY",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CUSTOMER",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CUSTOMER", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "STORE_SETTING",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Timezone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DefaultLanguage = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsCheckoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    TaxEnabled = table.Column<bool>(type: "bit", nullable: false),
                    OrderPrefix = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STORE_SETTING", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SUBSCRIPTION_PLAN",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PriceMonthly = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PriceYearly = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxProducts = table.Column<int>(type: "int", nullable: false),
                    MaxStaffUsers = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SUBSCRIPTION_PLAN", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "STORE",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubscriptionStatus = table.Column<int>(type: "int", nullable: false),
                    ActivatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SuspendedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STORE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_STORE_ASP_NET_USERS_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "ASP_NET_USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PRODUCT",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SKU = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CompareAtPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CostPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    TrackInventory = table.Column<bool>(type: "bit", nullable: false),
                    LowStockThreshold = table.Column<int>(type: "int", nullable: true),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUCT", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PRODUCT_CATEGORY_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "CATEGORY",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CART",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CART", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CART_CUSTOMER_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CUSTOMER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CUSTOMER_ADDRESS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AddressLine1 = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AddressLine2 = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDefaultShipping = table.Column<bool>(type: "bit", nullable: false),
                    IsDefaultBilling = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CUSTOMER_ADDRESS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CUSTOMER_ADDRESS_CUSTOMER_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CUSTOMER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ORDER",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    ShippingStatus = table.Column<int>(type: "int", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ShippingAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ShippingFullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ShippingPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ShippingCountry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ShippingCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ShippingDistrict = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ShippingAddressLine1 = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ShippingAddressLine2 = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ShippingPostalCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BillingFullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BillingPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BillingCountry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BillingCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BillingDistrict = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BillingAddressLine1 = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BillingAddressLine2 = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BillingPostalCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ORDER", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ORDER_CUSTOMER_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CUSTOMER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "STORE_BRANDING",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LogoUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PrimaryColor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SecondaryColor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STORE_BRANDING", x => x.Id);
                    table.ForeignKey(
                        name: "FK_STORE_BRANDING_STORE_StoreId",
                        column: x => x.StoreId,
                        principalTable: "STORE",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "STORE_DATABASE",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DatabaseName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DbServer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConnectionStringEncrypted = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProvisioningStatus = table.Column<int>(type: "int", nullable: false),
                    IsProvisioned = table.Column<bool>(type: "bit", nullable: false),
                    ProvisionedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MigrationVersion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastMigrationAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastError = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STORE_DATABASE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_STORE_DATABASE_STORE_StoreId",
                        column: x => x.StoreId,
                        principalTable: "STORE",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "STORE_DOMAIN",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Subdomain = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FullDomain = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerificationToken = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SslStatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STORE_DOMAIN", x => x.Id);
                    table.ForeignKey(
                        name: "FK_STORE_DOMAIN_STORE_StoreId",
                        column: x => x.StoreId,
                        principalTable: "STORE",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "STORE_SUBSCRIPTION",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BillingCycle = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AutoRenew = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrialEndsAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RenewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextBillingAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STORE_SUBSCRIPTION", x => x.Id);
                    table.ForeignKey(
                        name: "FK_STORE_SUBSCRIPTION_STORE_StoreId",
                        column: x => x.StoreId,
                        principalTable: "STORE",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_STORE_SUBSCRIPTION_SUBSCRIPTION_PLAN_PlanId",
                        column: x => x.PlanId,
                        principalTable: "SUBSCRIPTION_PLAN",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "STORE_USER",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    InvitedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STORE_USER", x => x.Id);
                    table.ForeignKey(
                        name: "FK_STORE_USER_ASP_NET_USERS_UserId",
                        column: x => x.UserId,
                        principalTable: "ASP_NET_USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_STORE_USER_STORE_StoreId",
                        column: x => x.StoreId,
                        principalTable: "STORE",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PRODUCT_IMAGE",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AltText = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUCT_IMAGE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PRODUCT_IMAGE_PRODUCT_ProductId",
                        column: x => x.ProductId,
                        principalTable: "PRODUCT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PRODUCT_VARIANT",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CompareAtPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CostPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    TrackInventory = table.Column<bool>(type: "bit", nullable: false),
                    AttributesJson = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUCT_VARIANT", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PRODUCT_VARIANT_PRODUCT_ProductId",
                        column: x => x.ProductId,
                        principalTable: "PRODUCT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "INVOICE",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ShippingAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CustomerNameSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CustomerEmailSnapshot = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    BillingAddressSnapshot = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    StoreNameSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StoreTaxNumberSnapshot = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StoreAddressSnapshot = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PdfUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INVOICE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_INVOICE_ORDER_OrderId",
                        column: x => x.OrderId,
                        principalTable: "ORDER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PAYMENT",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RefundedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PaymentProvider = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProviderReference = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProviderResponseCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAYMENT", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PAYMENT_ORDER_OrderId",
                        column: x => x.OrderId,
                        principalTable: "ORDER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SUBSCRIPTION_PAYMENT",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreSubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PaymentProvider = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProviderReference = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PaymentStatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SUBSCRIPTION_PAYMENT", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SUBSCRIPTION_PAYMENT_STORE_SUBSCRIPTION_StoreSubscriptionId",
                        column: x => x.StoreSubscriptionId,
                        principalTable: "STORE_SUBSCRIPTION",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CART_ITEM",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductVariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CART_ITEM", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CART_ITEM_CART_CartId",
                        column: x => x.CartId,
                        principalTable: "CART",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CART_ITEM_PRODUCT_ProductId",
                        column: x => x.ProductId,
                        principalTable: "PRODUCT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CART_ITEM_PRODUCT_VARIANT_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "PRODUCT_VARIANT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ORDER_ITEM",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductVariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductNameSnapshot = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ProductImageUrlSnapshot = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    VariantInfoSnapshot = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SKU = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ORDER_ITEM", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ORDER_ITEM_ORDER_OrderId",
                        column: x => x.OrderId,
                        principalTable: "ORDER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ORDER_ITEM_PRODUCT_ProductId",
                        column: x => x.ProductId,
                        principalTable: "PRODUCT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ORDER_ITEM_PRODUCT_VARIANT_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "PRODUCT_VARIANT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "INVOICE_ITEM",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductNameSnapshot = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INVOICE_ITEM", x => x.Id);
                    table.ForeignKey(
                        name: "FK_INVOICE_ITEM_INVOICE_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "INVOICE",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_INVOICE_ITEM_ORDER_ITEM_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "ORDER_ITEM",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CART_CustomerId",
                table: "CART",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CART_ITEM_CartId_ProductId_ProductVariantId",
                table: "CART_ITEM",
                columns: new[] { "CartId", "ProductId", "ProductVariantId" },
                unique: true,
                filter: "[ProductVariantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CART_ITEM_ProductId",
                table: "CART_ITEM",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CART_ITEM_ProductVariantId",
                table: "CART_ITEM",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_CATEGORY_ParentCategoryId",
                table: "CATEGORY",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CATEGORY_Slug",
                table: "CATEGORY",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CUSTOMER_Email",
                table: "CUSTOMER",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CUSTOMER_ADDRESS_CustomerId",
                table: "CUSTOMER_ADDRESS",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_INVOICE_InvoiceNumber",
                table: "INVOICE",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_INVOICE_OrderId",
                table: "INVOICE",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_INVOICE_ITEM_InvoiceId",
                table: "INVOICE_ITEM",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_INVOICE_ITEM_OrderItemId",
                table: "INVOICE_ITEM",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ORDER_CustomerId",
                table: "ORDER",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ORDER_OrderNumber",
                table: "ORDER",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ORDER_ITEM_OrderId",
                table: "ORDER_ITEM",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ORDER_ITEM_ProductId",
                table: "ORDER_ITEM",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ORDER_ITEM_ProductVariantId",
                table: "ORDER_ITEM",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_PAYMENT_OrderId",
                table: "PAYMENT",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PAYMENT_ProviderReference",
                table: "PAYMENT",
                column: "ProviderReference");

            migrationBuilder.CreateIndex(
                name: "IX_PAYMENT_TransactionId",
                table: "PAYMENT",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCT_CategoryId",
                table: "PRODUCT",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCT_SKU",
                table: "PRODUCT",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCT_Slug",
                table: "PRODUCT",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCT_IMAGE_ProductId",
                table: "PRODUCT_IMAGE",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCT_VARIANT_ProductId",
                table: "PRODUCT_VARIANT",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCT_VARIANT_SKU",
                table: "PRODUCT_VARIANT",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_STORE_OwnerUserId",
                table: "STORE",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_STORE_Slug",
                table: "STORE",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_STORE_BRANDING_StoreId",
                table: "STORE_BRANDING",
                column: "StoreId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_STORE_DATABASE_StoreId",
                table: "STORE_DATABASE",
                column: "StoreId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_STORE_DOMAIN_FullDomain",
                table: "STORE_DOMAIN",
                column: "FullDomain",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_STORE_DOMAIN_StoreId",
                table: "STORE_DOMAIN",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_STORE_DOMAIN_Subdomain",
                table: "STORE_DOMAIN",
                column: "Subdomain",
                unique: true,
                filter: "[Subdomain] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_STORE_SUBSCRIPTION_PlanId",
                table: "STORE_SUBSCRIPTION",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_STORE_SUBSCRIPTION_StoreId",
                table: "STORE_SUBSCRIPTION",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_STORE_USER_StoreId_UserId",
                table: "STORE_USER",
                columns: new[] { "StoreId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_STORE_USER_UserId",
                table: "STORE_USER",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SUBSCRIPTION_PAYMENT_InvoiceNumber",
                table: "SUBSCRIPTION_PAYMENT",
                column: "InvoiceNumber");

            migrationBuilder.CreateIndex(
                name: "IX_SUBSCRIPTION_PAYMENT_ProviderReference",
                table: "SUBSCRIPTION_PAYMENT",
                column: "ProviderReference");

            migrationBuilder.CreateIndex(
                name: "IX_SUBSCRIPTION_PAYMENT_StoreSubscriptionId",
                table: "SUBSCRIPTION_PAYMENT",
                column: "StoreSubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_SUBSCRIPTION_PAYMENT_TransactionId",
                table: "SUBSCRIPTION_PAYMENT",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_SUBSCRIPTION_PLAN_Code",
                table: "SUBSCRIPTION_PLAN",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CART_ITEM");

            migrationBuilder.DropTable(
                name: "CUSTOMER_ADDRESS");

            migrationBuilder.DropTable(
                name: "INVOICE_ITEM");

            migrationBuilder.DropTable(
                name: "PAYMENT");

            migrationBuilder.DropTable(
                name: "PRODUCT_IMAGE");

            migrationBuilder.DropTable(
                name: "STORE_BRANDING");

            migrationBuilder.DropTable(
                name: "STORE_DATABASE");

            migrationBuilder.DropTable(
                name: "STORE_DOMAIN");

            migrationBuilder.DropTable(
                name: "STORE_SETTING");

            migrationBuilder.DropTable(
                name: "STORE_USER");

            migrationBuilder.DropTable(
                name: "SUBSCRIPTION_PAYMENT");

            migrationBuilder.DropTable(
                name: "CART");

            migrationBuilder.DropTable(
                name: "INVOICE");

            migrationBuilder.DropTable(
                name: "ORDER_ITEM");

            migrationBuilder.DropTable(
                name: "STORE_SUBSCRIPTION");

            migrationBuilder.DropTable(
                name: "ORDER");

            migrationBuilder.DropTable(
                name: "PRODUCT_VARIANT");

            migrationBuilder.DropTable(
                name: "STORE");

            migrationBuilder.DropTable(
                name: "SUBSCRIPTION_PLAN");

            migrationBuilder.DropTable(
                name: "CUSTOMER");

            migrationBuilder.DropTable(
                name: "PRODUCT");

            migrationBuilder.DropTable(
                name: "ASP_NET_USERS");

            migrationBuilder.DropTable(
                name: "CATEGORY");
        }
    }
}
