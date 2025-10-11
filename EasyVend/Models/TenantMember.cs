using System;

namespace EasyVend.Models
{
    public enum TenantMemberStatus { Invited = 0, Active = 1, Removed = 2 }

    public class TenantMember
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }

        public string Role { get; set; } = "Vendor"; // e.g., Admin, Vendor
        public TenantMemberStatus Status { get; set; } = TenantMemberStatus.Invited;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ActivatedAt { get; set; }
        public DateTime? RemovedAt { get; set; }
         
        public Tenant? Tenant { get; set; }
        public User? User { get; set; }
    }
}