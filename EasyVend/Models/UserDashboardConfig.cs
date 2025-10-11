using System;
using System.ComponentModel.DataAnnotations;

namespace EasyVend.Models
{
    public class UserDashboardConfig
    {
        // Composite key: UserId + WidgetKey (configured via Fluent API)
        public Guid UserId { get; set; }

        [MaxLength(100)]
        public string WidgetKey { get; set; } = string.Empty; // e.g., "SalesChart"

        // Layout
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        // Visibility
        public bool Visible { get; set; } = true;

        // Arbitrary settings per widget (JSON)
        public string? SettingsJson { get; set; }
    }
}
