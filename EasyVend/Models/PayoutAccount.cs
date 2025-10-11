using System;

namespace EasyVend.Models
{
    public class PayoutAccount
    {
        public Guid PayoutAccountId { get; set; }
        public Guid VendorId { get; set; }
        public string Provider { get; set; } = string.Empty;
        public string AccountIdentifier { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
