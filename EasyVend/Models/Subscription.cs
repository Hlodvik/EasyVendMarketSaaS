using System;

namespace EasyVend.Models
{
    public class Subscription
    {
        public Guid SubscriptionId { get; set; }
        public Guid TenantId { get; set; }
        public string SubPlan { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime RenewalDate { get; set; }
    }
}
