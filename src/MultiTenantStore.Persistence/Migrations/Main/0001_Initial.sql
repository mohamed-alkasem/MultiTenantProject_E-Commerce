/*
    Main (Platform) database schema for MultiTenantStore.
    Target: SQL Server
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'main')
BEGIN
    EXEC('CREATE SCHEMA [main]');
END
GO

CREATE TABLE [main].[ApplicationUsers]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_ApplicationUsers] PRIMARY KEY,
    [UserName] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL,
    [PhoneNumber] NVARCHAR(30) NULL,
    [PasswordHash] NVARCHAR(MAX) NOT NULL,
    [IsEmailVerified] BIT NOT NULL CONSTRAINT [DF_ApplicationUsers_IsEmailVerified] DEFAULT(0),
    [IsActive] BIT NOT NULL CONSTRAINT [DF_ApplicationUsers_IsActive] DEFAULT(1),
    [LastLoginAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_ApplicationUsers_CreatedAt] DEFAULT(SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NULL
);
GO

CREATE UNIQUE INDEX [UX_ApplicationUsers_UserName] ON [main].[ApplicationUsers]([UserName]);
CREATE UNIQUE INDEX [UX_ApplicationUsers_Email] ON [main].[ApplicationUsers]([Email]);
GO

CREATE TABLE [main].[Stores]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_Stores] PRIMARY KEY,
    [OwnerUserId] UNIQUEIDENTIFIER NOT NULL,
    [StoreName] NVARCHAR(200) NOT NULL,
    [Slug] NVARCHAR(100) NOT NULL,
    [Status] NVARCHAR(30) NOT NULL,
    [SubscriptionStatus] NVARCHAR(30) NOT NULL,
    [ActivatedAt] DATETIME2 NULL,
    [SuspendedAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Stores_CreatedAt] DEFAULT(SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [FK_Stores_OwnerUser] FOREIGN KEY ([OwnerUserId]) REFERENCES [main].[ApplicationUsers]([Id])
);
GO

CREATE UNIQUE INDEX [UX_Stores_Slug] ON [main].[Stores]([Slug]);
CREATE INDEX [IX_Stores_OwnerUserId] ON [main].[Stores]([OwnerUserId]);
GO

CREATE TABLE [main].[StoreUsers]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_StoreUsers] PRIMARY KEY,
    [StoreId] UNIQUEIDENTIFIER NOT NULL,
    [ApplicationUserId] UNIQUEIDENTIFIER NOT NULL,
    [Role] NVARCHAR(30) NOT NULL,
    [IsActive] BIT NOT NULL CONSTRAINT [DF_StoreUsers_IsActive] DEFAULT(1),
    [InvitedAt] DATETIME2 NULL,
    [AcceptedAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_StoreUsers_CreatedAt] DEFAULT(SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [FK_StoreUsers_Store] FOREIGN KEY ([StoreId]) REFERENCES [main].[Stores]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_StoreUsers_ApplicationUser] FOREIGN KEY ([ApplicationUserId]) REFERENCES [main].[ApplicationUsers]([Id])
);
GO

CREATE UNIQUE INDEX [UX_StoreUsers_Store_User] ON [main].[StoreUsers]([StoreId], [ApplicationUserId]);
CREATE INDEX [IX_StoreUsers_ApplicationUserId] ON [main].[StoreUsers]([ApplicationUserId]);
GO

CREATE TABLE [main].[StoreDomains]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_StoreDomains] PRIMARY KEY,
    [StoreId] UNIQUEIDENTIFIER NOT NULL,
    [Domain] NVARCHAR(255) NOT NULL,
    [IsPrimary] BIT NOT NULL CONSTRAINT [DF_StoreDomains_IsPrimary] DEFAULT(0),
    [IsVerified] BIT NOT NULL CONSTRAINT [DF_StoreDomains_IsVerified] DEFAULT(0),
    [VerificationToken] NVARCHAR(100) NULL,
    [VerifiedAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_StoreDomains_CreatedAt] DEFAULT(SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [FK_StoreDomains_Store] FOREIGN KEY ([StoreId]) REFERENCES [main].[Stores]([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [UX_StoreDomains_Domain] ON [main].[StoreDomains]([Domain]);
CREATE INDEX [IX_StoreDomains_StoreId] ON [main].[StoreDomains]([StoreId]);
GO

CREATE TABLE [main].[StoreDatabases]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_StoreDatabases] PRIMARY KEY,
    [StoreId] UNIQUEIDENTIFIER NOT NULL,
    [ServerName] NVARCHAR(255) NOT NULL,
    [DatabaseName] NVARCHAR(255) NOT NULL,
    [Username] NVARCHAR(100) NULL,
    [PasswordEncrypted] NVARCHAR(MAX) NULL,
    [ConnectionStringEncrypted] NVARCHAR(MAX) NULL,
    [ProvisioningStatus] NVARCHAR(30) NOT NULL,
    [LastMigrationAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_StoreDatabases_CreatedAt] DEFAULT(SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [FK_StoreDatabases_Store] FOREIGN KEY ([StoreId]) REFERENCES [main].[Stores]([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [UX_StoreDatabases_StoreId] ON [main].[StoreDatabases]([StoreId]);
GO

CREATE TABLE [main].[StoreBrandings]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_StoreBrandings] PRIMARY KEY,
    [StoreId] UNIQUEIDENTIFIER NOT NULL,
    [LogoUrl] NVARCHAR(500) NULL,
    [FaviconUrl] NVARCHAR(500) NULL,
    [PrimaryColor] NVARCHAR(20) NULL,
    [SecondaryColor] NVARCHAR(20) NULL,
    [AccentColor] NVARCHAR(20) NULL,
    [Theme] NVARCHAR(20) NULL,
    [FontFamily] NVARCHAR(100) NULL,
    [CustomCss] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_StoreBrandings_CreatedAt] DEFAULT(SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [FK_StoreBrandings_Store] FOREIGN KEY ([StoreId]) REFERENCES [main].[Stores]([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [UX_StoreBrandings_StoreId] ON [main].[StoreBrandings]([StoreId]);
GO

CREATE TABLE [main].[SubscriptionPlans]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_SubscriptionPlans] PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [Code] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(1000) NULL,
    [PriceMonthly] DECIMAL(18,2) NOT NULL CONSTRAINT [DF_SubscriptionPlans_PriceMonthly] DEFAULT(0),
    [PriceYearly] DECIMAL(18,2) NULL,
    [TrialDays] INT NOT NULL CONSTRAINT [DF_SubscriptionPlans_TrialDays] DEFAULT(0),
    [MaxProducts] INT NULL,
    [MaxOrdersPerMonth] INT NULL,
    [MaxStorageMb] INT NULL,
    [MaxStaffUsers] INT NULL,
    [IsActive] BIT NOT NULL CONSTRAINT [DF_SubscriptionPlans_IsActive] DEFAULT(1),
    [SortOrder] INT NOT NULL CONSTRAINT [DF_SubscriptionPlans_SortOrder] DEFAULT(0),
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_SubscriptionPlans_CreatedAt] DEFAULT(SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NULL
);
GO

CREATE UNIQUE INDEX [UX_SubscriptionPlans_Code] ON [main].[SubscriptionPlans]([Code]);
GO

CREATE TABLE [main].[StoreSubscriptions]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_StoreSubscriptions] PRIMARY KEY,
    [StoreId] UNIQUEIDENTIFIER NOT NULL,
    [PlanId] UNIQUEIDENTIFIER NOT NULL,
    [BillingCycle] NVARCHAR(20) NOT NULL,
    [Status] NVARCHAR(30) NOT NULL,
    [AutoRenew] BIT NOT NULL CONSTRAINT [DF_StoreSubscriptions_AutoRenew] DEFAULT(1),
    [StartDate] DATETIME2 NOT NULL,
    [EndDate] DATETIME2 NULL,
    [TrialEndsAt] DATETIME2 NULL,
    [CancelledAt] DATETIME2 NULL,
    [RenewedAt] DATETIME2 NULL,
    [NextBillingAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_StoreSubscriptions_CreatedAt] DEFAULT(SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [FK_StoreSubscriptions_Store] FOREIGN KEY ([StoreId]) REFERENCES [main].[Stores]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_StoreSubscriptions_Plan] FOREIGN KEY ([PlanId]) REFERENCES [main].[SubscriptionPlans]([Id])
);
GO

CREATE INDEX [IX_StoreSubscriptions_StoreId] ON [main].[StoreSubscriptions]([StoreId]);
CREATE INDEX [IX_StoreSubscriptions_PlanId] ON [main].[StoreSubscriptions]([PlanId]);
GO

CREATE TABLE [main].[SubscriptionPayments]
(
    [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_SubscriptionPayments] PRIMARY KEY,
    [StoreSubscriptionId] UNIQUEIDENTIFIER NOT NULL,
    [Amount] DECIMAL(18,2) NOT NULL,
    [Currency] NVARCHAR(10) NOT NULL,
    [Status] NVARCHAR(30) NOT NULL,
    [Provider] NVARCHAR(50) NULL,
    [ProviderPaymentId] NVARCHAR(255) NULL,
    [FailureReason] NVARCHAR(1000) NULL,
    [PaidAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_SubscriptionPayments_CreatedAt] DEFAULT(SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [FK_SubscriptionPayments_StoreSubscription] FOREIGN KEY ([StoreSubscriptionId]) REFERENCES [main].[StoreSubscriptions]([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_SubscriptionPayments_StoreSubscriptionId] ON [main].[SubscriptionPayments]([StoreSubscriptionId]);
GO
