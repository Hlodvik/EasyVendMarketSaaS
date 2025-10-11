using System;

namespace EasyVend.Models
{
    public class Vendor
    {
        public Guid VendorId { get; set; }
        public Guid TenantId { get; set; }
        public Guid? UserId { get; set; }
        public User? User { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }
}
