using System;
using System.ComponentModel.DataAnnotations;

namespace EasyVend.Models
{
    public class IntegrationSyncLog
    {
        [Key]
        public Guid SyncLogId { get; set; }
        public Guid IntegrationId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }
}
