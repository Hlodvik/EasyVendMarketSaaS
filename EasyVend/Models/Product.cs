using System;
using System.ComponentModel.DataAnnotations;

namespace EasyVend.Models
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public Guid TenantId { get; set; }
        public Guid VendorId { get; set; }

        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        // Pricing
        public decimal Price { get; set; }
        public string Currency { get; set; } = "USD";
        public decimal CostPerUnit { get; set; }

        // Inventory
        public int QuantityAvailable { get; set; }
        public int ReorderThreshold { get; set; }

        // Identification
        [MaxLength(64)]
        public string SKU { get; set; } = string.Empty;
        [MaxLength(64)]
        public string? Barcode { get; set; }

        // Listing detail / SEO
        [MaxLength(800)]
        public string? Tags { get; set; }
        public string? Description { get; set; }
        [MaxLength(800)]
        public string? Keywords { get; set; }

        // Lifecycle
        public bool IsActive { get; set; } = true;
        public bool IsArchived { get; set; } = false;

        // Analytics/Metrics
        public int TotalSold { get; set; }
        public int TotalViews { get; set; }
        public DateTime? LastSoldAt { get; set; }

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public string ImageUrl { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
    }
}
