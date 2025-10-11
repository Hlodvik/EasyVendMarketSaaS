using System;

namespace EasyVend.Models
{
    public class Listing
    {
        public Guid ListingId { get; set; }
        public Guid ProductId { get; set; }
        public Guid IntegrationId { get; set; }
        public string ExternalListingId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
