using System;

namespace EasyVend.Models
{
    public class BillingTransaction
    {
        public Guid BillingTransactionId { get; set; }
        public Guid TenantId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string Type { get; set; } = string.Empty; // Subscription, Commission, One-off
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
