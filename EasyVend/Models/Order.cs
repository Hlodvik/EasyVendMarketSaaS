using System;
using System.Collections.Generic;

namespace EasyVend.Models
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public Guid TenantId { get; set; }
        public Guid VendorId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public string Currency { get; set; } = "USD";
        public string PaymentStatus { get; set; } = "Pending";
        public string ShippingStatus { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
