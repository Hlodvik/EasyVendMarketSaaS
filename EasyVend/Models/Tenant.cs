using System;
using System.Collections.Generic;

namespace EasyVend.Models
{
    public class Tenant
    {
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public bool IsMultiVendor { get; set; }
        public Guid? PrimaryVendorId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Vendor> Vendors { get; set; } = [];
        // join table 
        public ICollection<TenantMember> Members { get; set; } = [];
    }
}
