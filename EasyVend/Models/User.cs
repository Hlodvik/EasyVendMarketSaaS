using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyVend.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        // Stable identity key (oid or iss|sub)
        [Required]
        public string EntraObjectId { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(320)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(200)]
        public string DisplayName { get; set; } = string.Empty;

        // Preferred contact channel; default this to Email on first login
        [EmailAddress, MaxLength(320)]
        public string? PreferredContactEmail { get; set; }

        // ---- New profile fields ----
        [MaxLength(200)]
        public string? CompanyName { get; set; }

        [MaxLength(200)]
        public string? AddressLine1 { get; set; }

        [MaxLength(200)]
        public string? AddressLine2 { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? Region { get; set; }     // State/Province/Region

        [MaxLength(32)]
        public string? PostalCode { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        // Comma-separated list of marketplace identifiers (e.g., "amazon,etsy,shopify")
        [MaxLength(800)]
        public string? PreferredMarketplaces { get; set; }

        // Onboarding gate
        public bool OnboardingComplete { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation: memberships across tenants
        public ICollection<TenantMember> Memberships { get; set; } = [];
    }
}
