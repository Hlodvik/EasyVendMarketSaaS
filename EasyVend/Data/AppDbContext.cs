using Microsoft.EntityFrameworkCore;
using EasyVend.Models;

namespace EasyVend.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Tenant> Tenants { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Vendor> Vendors { get; set; } = null!;
        public DbSet<PayoutAccount> PayoutAccounts { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<MarketplaceIntegration> MarketplaceIntegrations { get; set; } = null!;
        public DbSet<IntegrationCredential> IntegrationCredentials { get; set; } = null!;
        public DbSet<IntegrationSyncLog> IntegrationSyncLogs { get; set; } = null!;
        public DbSet<Listing> Listings { get; set; } = null!;
        public DbSet<Subscription> Subscriptions { get; set; } = null!;
        public DbSet<BillingTransaction> BillingTransactions { get; set; } = null!;
        public DbSet<TenantMember> TenantMembers { get; set; } = null!;
        public DbSet<UserDashboardConfig> UserDashboardConfigs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Tenants ---
            modelBuilder.Entity<Tenant>()
                .HasIndex(t => t.Domain)
                .IsUnique();


            modelBuilder.Entity<User>()
                .HasIndex(u => u.EntraObjectId)
                .IsUnique();                // every Entra user once
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email);   // non-unique: same email could exist across identity providers/tenants

            // --- TenantMembers (join table: Tenant / User) ---
            modelBuilder.Entity<TenantMember>()
                .HasIndex(m => new { m.TenantId, m.UserId })
                .IsUnique();

            modelBuilder.Entity<TenantMember>()
                .HasOne(m => m.Tenant)
                .WithMany(t => t.Members)
                .HasForeignKey(m => m.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TenantMember>()
                .HasOne(m => m.User)
                .WithMany(u => u.Memberships)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Money precision ---
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);
            modelBuilder.Entity<Product>()
                .Property(p => p.CostPerUnit)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.PriceAtPurchase)
                .HasPrecision(18, 2);

            modelBuilder.Entity<BillingTransaction>()
                .Property(b => b.Amount)
                .HasPrecision(18, 2);

            // --- Vendor relationships & indexes ---
            modelBuilder.Entity<Vendor>()
                .HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Vendor>()
                .HasIndex(v => new { v.TenantId, v.IsPrimary });

            // --- Product indexes for sorting/filtering/lookup ---
            modelBuilder.Entity<Product>()
                .HasIndex(p => new { p.TenantId, p.CreatedAt });
            modelBuilder.Entity<Product>()
                .HasIndex(p => new { p.TenantId, p.UpdatedAt });
            modelBuilder.Entity<Product>()
                .HasIndex(p => new { p.TenantId, p.SKU });
            modelBuilder.Entity<Product>()
                .HasIndex(p => new { p.TenantId, p.Barcode });
            modelBuilder.Entity<Product>()
                .HasIndex(p => new { p.TenantId, p.IsActive, p.IsArchived });
            modelBuilder.Entity<Product>()
                .HasIndex(p => new { p.TenantId, p.TotalSold, p.TotalViews });

            // --- UserDashboardConfig ---
            modelBuilder.Entity<UserDashboardConfig>()
                .HasKey(c => new { c.UserId, c.WidgetKey });

            modelBuilder.Entity<UserDashboardConfig>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserDashboardConfig>()
                .Property(c => c.WidgetKey)
                .HasMaxLength(100);

            modelBuilder.Entity<UserDashboardConfig>()
                .Property(c => c.SettingsJson)
                .HasColumnType("nvarchar(max)");
        }
    }
}
