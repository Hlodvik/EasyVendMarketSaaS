using System;
using System.ComponentModel.DataAnnotations;

namespace EasyVend.Models
{
    public class MarketplaceIntegration
    {
        [Key]
        public Guid IntegrationId { get; set; }
        public Guid TenantId { get; set; }
        public Guid? VendorId { get; set; }
        public string Platform { get; set; } = string.Empty;
        public string SyncStatus { get; set; } = string.Empty;
    }
}
