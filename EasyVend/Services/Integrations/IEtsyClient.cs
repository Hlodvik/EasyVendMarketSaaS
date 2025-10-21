using System.Threading;
using System.Threading.Tasks;

namespace EasyVend.Services.Integrations
{
    public interface IEtsyClient
    {
        /// <summary>
        /// Creates a draft or active listing in the specified Etsy shop.
        /// Returns the created Etsy listing id as a string.
        /// </summary>
        Task<(bool Ok, string ListingId, string Error)> CreateListingAsync(
            long shopId,
            string accessToken,
            string title,
            string description,
            decimal price,
            int quantity,
            string sku,
            CancellationToken ct = default);
    }
}
