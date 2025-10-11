IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [BillingTransactions] (
    [BillingTransactionId] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Currency] nvarchar(max) NOT NULL,
    [Type] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_BillingTransactions] PRIMARY KEY ([BillingTransactionId])
);
GO

CREATE TABLE [Customers] (
    [CustomerId] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NULL,
    [ShippingAddress] nvarchar(max) NOT NULL,
    [BillingInfo] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([CustomerId])
);
GO

CREATE TABLE [IntegrationCredentials] (
    [CredentialId] uniqueidentifier NOT NULL,
    [IntegrationId] uniqueidentifier NOT NULL,
    [KeyName] nvarchar(max) NOT NULL,
    [KeyValue] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_IntegrationCredentials] PRIMARY KEY ([CredentialId])
);
GO

CREATE TABLE [IntegrationSyncLogs] (
    [SyncLogId] uniqueidentifier NOT NULL,
    [IntegrationId] uniqueidentifier NOT NULL,
    [Timestamp] datetime2 NOT NULL,
    [Action] nvarchar(max) NOT NULL,
    [Result] nvarchar(max) NOT NULL,
    [ErrorMessage] nvarchar(max) NULL,
    CONSTRAINT [PK_IntegrationSyncLogs] PRIMARY KEY ([SyncLogId])
);
GO

CREATE TABLE [Listings] (
    [ListingId] uniqueidentifier NOT NULL,
    [ProductId] uniqueidentifier NOT NULL,
    [IntegrationId] uniqueidentifier NOT NULL,
    [ExternalListingId] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Listings] PRIMARY KEY ([ListingId])
);
GO

CREATE TABLE [MarketplaceIntegrations] (
    [IntegrationId] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NOT NULL,
    [VendorId] uniqueidentifier NULL,
    [Platform] nvarchar(max) NOT NULL,
    [SyncStatus] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_MarketplaceIntegrations] PRIMARY KEY ([IntegrationId])
);
GO

CREATE TABLE [Orders] (
    [OrderId] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NOT NULL,
    [VendorId] uniqueidentifier NOT NULL,
    [CustomerId] uniqueidentifier NOT NULL,
    [TotalPrice] decimal(18,2) NOT NULL,
    [Currency] nvarchar(max) NOT NULL,
    [PaymentStatus] nvarchar(max) NOT NULL,
    [ShippingStatus] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([OrderId])
);
GO

CREATE TABLE [PayoutAccounts] (
    [PayoutAccountId] uniqueidentifier NOT NULL,
    [VendorId] uniqueidentifier NOT NULL,
    [Provider] nvarchar(max) NOT NULL,
    [AccountIdentifier] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_PayoutAccounts] PRIMARY KEY ([PayoutAccountId])
);
GO

CREATE TABLE [Products] (
    [ProductId] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NOT NULL,
    [VendorId] uniqueidentifier NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Category] nvarchar(max) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Currency] nvarchar(max) NOT NULL,
    [QuantityAvailable] int NOT NULL,
    [ImageUrl] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([ProductId])
);
GO

CREATE TABLE [Subscriptions] (
    [SubscriptionId] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NOT NULL,
    [SubPlan] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [RenewalDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Subscriptions] PRIMARY KEY ([SubscriptionId])
);
GO

CREATE TABLE [Tenants] (
    [TenantId] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Domain] nvarchar(450) NOT NULL,
    [IsMultiVendor] bit NOT NULL,
    [PrimaryVendorId] uniqueidentifier NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Tenants] PRIMARY KEY ([TenantId])
);
GO

CREATE TABLE [OrderItems] (
    [OrderItemId] uniqueidentifier NOT NULL,
    [OrderId] uniqueidentifier NOT NULL,
    [ProductId] uniqueidentifier NOT NULL,
    [Quantity] int NOT NULL,
    [PriceAtPurchase] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_OrderItems] PRIMARY KEY ([OrderItemId]),
    CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([OrderId]) ON DELETE CASCADE
);
GO

CREATE TABLE [Users] (
    [UserId] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    [Email] nvarchar(450) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserId]),
    CONSTRAINT [FK_Users_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenants] ([TenantId]) ON DELETE CASCADE
);
GO

CREATE TABLE [Vendors] (
    [VendorId] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NULL,
    [DisplayName] nvarchar(max) NOT NULL,
    [Bio] nvarchar(max) NOT NULL,
    [IsPrimary] bit NOT NULL,
    CONSTRAINT [PK_Vendors] PRIMARY KEY ([VendorId]),
    CONSTRAINT [FK_Vendors_Tenants_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [Tenants] ([TenantId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Vendors_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE SET NULL
);
GO

CREATE INDEX [IX_OrderItems_OrderId] ON [OrderItems] ([OrderId]);
GO

CREATE UNIQUE INDEX [IX_Tenants_Domain] ON [Tenants] ([Domain]);
GO

CREATE UNIQUE INDEX [IX_Users_TenantId_Email] ON [Users] ([TenantId], [Email]);
GO

CREATE INDEX [IX_Vendors_TenantId_IsPrimary] ON [Vendors] ([TenantId], [IsPrimary]);
GO

CREATE INDEX [IX_Vendors_UserId] ON [Vendors] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251001224902_InitialCreate', N'8.0.12');
GO

COMMIT;
GO

