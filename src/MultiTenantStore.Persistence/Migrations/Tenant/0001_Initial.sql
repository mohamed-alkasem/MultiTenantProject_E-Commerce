/*
    Tenant (Store) database schema for MultiTenantStore.
    Target: SQL Server
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'tenant')
BEGIN
    EXEC('CREATE SCHEMA [tenant]');
END
GO

CREATE TABLE [tenant].[StoreSettings]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_StoreSettings] PRIMARY KEY,
    [SettingKey] NVARCHAR(100) NOT NULL,
    [SettingValue] NVARCHAR(MAX) NULL,
    [ValueType] NVARCHAR(30) NULL,
    [IsPublic] BIT NOT NULL CONSTRAINT [DF_StoreSettings_IsPublic] DEFAULT(0),
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_StoreSettings_CreatedAt] DEFAULT(SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NULL
);
GO

CREATE UNIQUE INDEX [UX_StoreSettings_SettingKey] ON [tenant].[StoreSettings]([SettingKey]);
GO

CREATE TABLE [tenant].[Categories]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_Categories] PRIMARY KEY,
    [ParentCategoryId] UNIQUEIDENTIFIER NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [Slug] NVARCHAR(220) NOT NULL,
    [Description] NVARCHAR(1000) NULL,
    [ImageUrl] NVARCHAR(500) NULL,
    [IsActive] BIT NOT NULL CONSTRAINT [DF_Categories_IsActive] DEFAULT(1),
    [SortOrder] INT NOT NULL CONSTRAINT [DF_Categories_SortOrder] DEFAULT(0),
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Categories_CreatedAt] DEFAULT(SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [FK_Categories_Parent] FOREIGN KEY ([ParentCategoryId]) REFERENCES [tenant].[Categories]([Id])
);
GO

CREATE UNIQUE INDEX [UX_Categories_Slug] ON [tenant].[Categories]([Slug]);
CREATE INDEX [IX_Categories_ParentCategoryId] ON [tenant].[Categories]([ParentCategoryId]);
GO

CREATE TABLE [tenant].[Products]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_Products] PRIMARY KEY,
    [CategoryId] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(250) NOT NULL,
    [Slug] NVARCHAR(260) NOT NULL,
    [ShortDescription] NVARCHAR(600) NULL,
    [Description] NVARCHAR(MAX) NULL,
    [SKU] NVARCHAR(100) NOT NULL,
    [Price] DECIMAL(18,2) NOT NULL,
    [CompareAtPrice] DECIMAL(18,2) NULL,
    [CostPrice] DECIMAL(18,2) NULL,
    [StockQuantity] INT NOT NULL CONSTRAINT [DF_Products_StockQuantity] DEFAULT(0),
    [TrackInventory] BIT NOT NULL CONSTRAINT [DF_Products_TrackInventory] DEFAULT(1),
    [LowStockThreshold] INT NULL,
    [IsFeatured] BIT NOT NULL CONSTRAINT [DF_Products_IsFeatured] DEFAULT(0),
    [IsActive] BIT NOT NULL CONSTRAINT [DF_Products_IsActive] DEFAULT(1),
    [SortOrder] INT NOT NULL CONSTRAINT [DF_Products_SortOrder] DEFAULT(0),
    [PublishedAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Products_CreatedAt] DEFAULT(SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [FK_Products_Category] FOREIGN KEY ([CategoryId]) REFERENCES [tenant].[Categories]([Id])
);
GO

CREATE UNIQUE INDEX [UX_Products_Slug] ON [tenant].[Products]([Slug]);
CREATE UNIQUE INDEX [UX_Products_SKU] ON [tenant].[Products]([SKU]);
CREATE INDEX [IX_Products_CategoryId] ON [tenant].[Products]([CategoryId]);
GO

CREATE TABLE [tenant].[ProductVariants]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_ProductVariants] PRIMARY KEY,
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [SKU] NVARCHAR(100) NOT NULL,
    [Price] DECIMAL(18,2) NULL,
    [StockQuantity] INT NOT NULL CONSTRAINT [DF_ProductVariants_StockQuantity] DEFAULT(0),
    [AttributesJson] NVARCHAR(MAX) NULL,
    [IsActive] BIT NOT NULL CONSTRAINT [DF_ProductVariants_IsActive] DEFAULT(1),
    [SortOrder] INT NOT NULL CONSTRAINT [DF_ProductVariants_SortOrder] DEFAULT(0),
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_ProductVariants_CreatedAt] DEFAULT(SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [FK_ProductVariants_Product] FOREIGN KEY ([ProductId]) REFERENCES [tenant].[Products]([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [UX_ProductVariants_SKU] ON [tenant].[ProductVariants]([SKU]);
CREATE INDEX [IX_ProductVariants_ProductId] ON [tenant].[ProductVariants]([ProductId]);
GO

CREATE TABLE [tenant].[ProductImages]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_ProductImages] PRIMARY KEY,
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [VariantId] UNIQUEIDENTIFIER NULL,
    [ImageUrl] NVARCHAR(500) NOT NULL,
    [AltText] NVARCHAR(250) NULL,
    [IsPrimary] BIT NOT NULL CONSTRAINT [DF_ProductImages_IsPrimary] DEFAULT(0),
    [SortOrder] INT NOT NULL CONSTRAINT [DF_ProductImages_SortOrder] DEFAULT(0),
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_ProductImages_CreatedAt] DEFAULT(SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [FK_ProductImages_Product] FOREIGN KEY ([ProductId]) REFERENCES [tenant].[Products]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ProductImages_Variant] FOREIGN KEY ([VariantId]) REFERENCES [tenant].[ProductVariants]([Id])
);
GO

CREATE INDEX [IX_ProductImages_ProductId] ON [tenant].[ProductImages]([ProductId]);
CREATE INDEX [IX_ProductImages_VariantId] ON [tenant].[ProductImages]([VariantId]);
GO

CREATE TABLE [tenant].[Customers]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_Customers] PRIMARY KEY,
    [Email] NVARCHAR(255) NOT NULL,
    [PhoneNumber] NVARCHAR(30) NULL,
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [PasswordHash] NVARCHAR(MAX) NULL,
    [IsActive] BIT NOT NULL CONSTRAINT [DF_Customers_IsActive] DEFAULT(1),
    [IsEmailVerified] BIT NOT NULL CONSTRAINT [DF_Customers_IsEmailVerified] DEFAULT(0),
    [LastLoginAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Customers_CreatedAt] DEFAULT(SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NULL
);
GO

CREATE UNIQUE INDEX [UX_Customers_Email] ON [tenant].[Customers]([Email]);
GO

CREATE TABLE [tenant].[CustomerAddresses]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_CustomerAddresses] PRIMARY KEY,
    [CustomerId] UNIQUEIDENTIFIER NOT NULL,
    [AddressName] NVARCHAR(100) NOT NULL,
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [PhoneNumber] NVARCHAR(30) NULL,
    [Country] NVARCHAR(100) NOT NULL,
    [City] NVARCHAR(100) NOT NULL,
    [StateOrProvince] NVARCHAR(100) NULL,
    [PostalCode] NVARCHAR(30) NULL,
    [AddressLine1] NVARCHAR(255) NOT NULL,
    [AddressLine2] NVARCHAR(255) NULL,
    [IsDefaultShipping] BIT NOT NULL CONSTRAINT [DF_CustomerAddresses_IsDefaultShipping] DEFAULT(0),
    [IsDefaultBilling] BIT NOT NULL CONSTRAINT [DF_CustomerAddresses_IsDefaultBilling] DEFAULT(0),
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_CustomerAddresses_CreatedAt] DEFAULT(SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [FK_CustomerAddresses_Customer] FOREIGN KEY ([CustomerId]) REFERENCES [tenant].[Customers]([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_CustomerAddresses_CustomerId] ON [tenant].[CustomerAddresses]([CustomerId]);
GO

CREATE TABLE [tenant].[Carts]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_Carts] PRIMARY KEY,
    [CustomerId] UNIQUEIDENTIFIER NULL,
    [SessionId] NVARCHAR(100) NULL,
    [Status] NVARCHAR(20) NOT NULL,
    [Currency] NVARCHAR(10) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Carts_CreatedAt] DEFAULT(SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [FK_Carts_Customer] FOREIGN KEY ([CustomerId]) REFERENCES [tenant].[Customers]([Id])
);
GO

CREATE INDEX [IX_Carts_CustomerId] ON [tenant].[Carts]([CustomerId]);
CREATE INDEX [IX_Carts_SessionId] ON [tenant].[Carts]([SessionId]);
GO

CREATE TABLE [tenant].[CartItems]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_CartItems] PRIMARY KEY,
    [CartId] UNIQUEIDENTIFIER NOT NULL,
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [VariantId] UNIQUEIDENTIFIER NULL,
    [Quantity] INT NOT NULL,
    [UnitPrice] DECIMAL(18,2) NOT NULL,
    [LineTotal] DECIMAL(18,2) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_CartItems_CreatedAt] DEFAULT(SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [FK_CartItems_Cart] FOREIGN KEY ([CartId]) REFERENCES [tenant].[Carts]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CartItems_Product] FOREIGN KEY ([ProductId]) REFERENCES [tenant].[Products]([Id]),
    CONSTRAINT [FK_CartItems_Variant] FOREIGN KEY ([VariantId]) REFERENCES [tenant].[ProductVariants]([Id])
);
GO

CREATE INDEX [IX_CartItems_CartId] ON [tenant].[CartItems]([CartId]);
CREATE INDEX [IX_CartItems_ProductId] ON [tenant].[CartItems]([ProductId]);
GO

CREATE TABLE [tenant].[Orders]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_Orders] PRIMARY KEY,
    [OrderNumber] NVARCHAR(50) NOT NULL,
    [CustomerId] UNIQUEIDENTIFIER NULL,
    [Status] NVARCHAR(30) NOT NULL,
    [PaymentStatus] NVARCHAR(30) NOT NULL,
    [ShippingStatus] NVARCHAR(30) NOT NULL,
    [Currency] NVARCHAR(10) NOT NULL,
    [Subtotal] DECIMAL(18,2) NOT NULL,
    [DiscountAmount] DECIMAL(18,2) NOT NULL CONSTRAINT [DF_Orders_DiscountAmount] DEFAULT(0),
    [ShippingAmount] DECIMAL(18,2) NOT NULL CONSTRAINT [DF_Orders_ShippingAmount] DEFAULT(0),
    [TaxAmount] DECIMAL(18,2) NOT NULL CONSTRAINT [DF_Orders_TaxAmount] DEFAULT(0),
    [TotalAmount] DECIMAL(18,2) NOT NULL,
    [ShippingFirstName] NVARCHAR(100) NOT NULL,
    [ShippingLastName] NVARCHAR(100) NOT NULL,
    [ShippingPhoneNumber] NVARCHAR(30) NULL,
    [ShippingCountry] NVARCHAR(100) NOT NULL,
    [ShippingCity] NVARCHAR(100) NOT NULL,
    [ShippingStateOrProvince] NVARCHAR(100) NULL,
    [ShippingPostalCode] NVARCHAR(30) NULL,
    [ShippingAddressLine1] NVARCHAR(255) NOT NULL,
    [ShippingAddressLine2] NVARCHAR(255) NULL,
    [BillingFirstName] NVARCHAR(100) NOT NULL,
    [BillingLastName] NVARCHAR(100) NOT NULL,
    [BillingPhoneNumber] NVARCHAR(30) NULL,
    [BillingCountry] NVARCHAR(100) NOT NULL,
    [BillingCity] NVARCHAR(100) NOT NULL,
    [BillingStateOrProvince] NVARCHAR(100) NULL,
    [BillingPostalCode] NVARCHAR(30) NULL,
    [BillingAddressLine1] NVARCHAR(255) NOT NULL,
    [BillingAddressLine2] NVARCHAR(255) NULL,
    [Notes] NVARCHAR(2000) NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Orders_CreatedAt] DEFAULT(SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [FK_Orders_Customer] FOREIGN KEY ([CustomerId]) REFERENCES [tenant].[Customers]([Id])
);
GO

CREATE UNIQUE INDEX [UX_Orders_OrderNumber] ON [tenant].[Orders]([OrderNumber]);
CREATE INDEX [IX_Orders_CustomerId] ON [tenant].[Orders]([CustomerId]);
GO

CREATE TABLE [tenant].[OrderItems]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_OrderItems] PRIMARY KEY,
    [OrderId] UNIQUEIDENTIFIER NOT NULL,
    [ProductId] UNIQUEIDENTIFIER NOT NULL,
    [VariantId] UNIQUEIDENTIFIER NULL,
    [ProductNameSnapshot] NVARCHAR(250) NOT NULL,
    [SKU] NVARCHAR(100) NOT NULL,
    [Quantity] INT NOT NULL,
    [UnitPrice] DECIMAL(18,2) NOT NULL,
    [LineTotal] DECIMAL(18,2) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_OrderItems_CreatedAt] DEFAULT(SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [FK_OrderItems_Order] FOREIGN KEY ([OrderId]) REFERENCES [tenant].[Orders]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderItems_Product] FOREIGN KEY ([ProductId]) REFERENCES [tenant].[Products]([Id]),
    CONSTRAINT [FK_OrderItems_Variant] FOREIGN KEY ([VariantId]) REFERENCES [tenant].[ProductVariants]([Id])
);
GO

CREATE INDEX [IX_OrderItems_OrderId] ON [tenant].[OrderItems]([OrderId]);
GO

CREATE TABLE [tenant].[Payments]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_Payments] PRIMARY KEY,
    [OrderId] UNIQUEIDENTIFIER NOT NULL,
    [Amount] DECIMAL(18,2) NOT NULL,
    [Currency] NVARCHAR(10) NOT NULL,
    [Status] NVARCHAR(30) NOT NULL,
    [Method] NVARCHAR(50) NULL,
    [Provider] NVARCHAR(50) NULL,
    [ProviderPaymentId] NVARCHAR(255) NULL,
    [ReferenceCode] NVARCHAR(100) NULL,
    [PaidAt] DATETIME2 NULL,
    [RefundedAt] DATETIME2 NULL,
    [FailureReason] NVARCHAR(1000) NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Payments_CreatedAt] DEFAULT(SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [FK_Payments_Order] FOREIGN KEY ([OrderId]) REFERENCES [tenant].[Orders]([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Payments_OrderId] ON [tenant].[Payments]([OrderId]);
CREATE INDEX [IX_Payments_ProviderPaymentId] ON [tenant].[Payments]([ProviderPaymentId]);
GO

CREATE TABLE [tenant].[Invoices]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_Invoices] PRIMARY KEY,
    [OrderId] UNIQUEIDENTIFIER NULL,
    [CustomerId] UNIQUEIDENTIFIER NULL,
    [InvoiceNumber] NVARCHAR(50) NOT NULL,
    [Status] NVARCHAR(30) NOT NULL,
    [Currency] NVARCHAR(10) NOT NULL,
    [Subtotal] DECIMAL(18,2) NOT NULL,
    [TaxAmount] DECIMAL(18,2) NOT NULL CONSTRAINT [DF_Invoices_TaxAmount] DEFAULT(0),
    [DiscountAmount] DECIMAL(18,2) NOT NULL CONSTRAINT [DF_Invoices_DiscountAmount] DEFAULT(0),
    [TotalAmount] DECIMAL(18,2) NOT NULL,
    [IssuedAt] DATETIME2 NOT NULL,
    [DueAt] DATETIME2 NULL,
    [PaidAt] DATETIME2 NULL,
    [CancelledAt] DATETIME2 NULL,
    [PdfUrl] NVARCHAR(500) NULL,
    [Notes] NVARCHAR(2000) NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Invoices_CreatedAt] DEFAULT(SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [FK_Invoices_Order] FOREIGN KEY ([OrderId]) REFERENCES [tenant].[Orders]([Id]),
    CONSTRAINT [FK_Invoices_Customer] FOREIGN KEY ([CustomerId]) REFERENCES [tenant].[Customers]([Id])
);
GO

CREATE UNIQUE INDEX [UX_Invoices_InvoiceNumber] ON [tenant].[Invoices]([InvoiceNumber]);
CREATE INDEX [IX_Invoices_OrderId] ON [tenant].[Invoices]([OrderId]);
CREATE INDEX [IX_Invoices_CustomerId] ON [tenant].[Invoices]([CustomerId]);
GO

CREATE TABLE [tenant].[InvoiceItems]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_InvoiceItems] PRIMARY KEY,
    [InvoiceId] UNIQUEIDENTIFIER NOT NULL,
    [Description] NVARCHAR(500) NOT NULL,
    [Quantity] INT NOT NULL,
    [UnitPrice] DECIMAL(18,2) NOT NULL,
    [TaxAmount] DECIMAL(18,2) NOT NULL CONSTRAINT [DF_InvoiceItems_TaxAmount] DEFAULT(0),
    [LineTotal] DECIMAL(18,2) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_InvoiceItems_CreatedAt] DEFAULT(SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [FK_InvoiceItems_Invoice] FOREIGN KEY ([InvoiceId]) REFERENCES [tenant].[Invoices]([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_InvoiceItems_InvoiceId] ON [tenant].[InvoiceItems]([InvoiceId]);
GO
