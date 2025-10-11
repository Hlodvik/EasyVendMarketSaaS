using System;

namespace EasyVend.Models
{
    public class Customer
    {
        public Guid CustomerId { get; set; }
        public Guid TenantId { get; set; }
        public Guid? UserId { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string BillingInfo { get; set; } = string.Empty;
    }
}
