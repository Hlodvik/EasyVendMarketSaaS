using System.Threading;
using System.Threading.Tasks;

namespace EasyVend.Services.Integrations
{
    /// <summary>
    /// Abstraction for communicating with Etsy's API. Implementations should handle
    /// authentication (bearer tokens) and make HTTP requests to Etsy's OpenAPI endpoints.
    /// </summary>
    public interface IEtsyClient
    {
        /// <summary>
        /// Creates a draft or active listing in the specified Etsy shop.
        /// Returns a tuple where <c>Ok</c> indicates success, <c>ListingId</c> contains
        /// the external Etsy listing identifier on success and <c>Error</c> contains an
        /// error message on failure.
        /// </summary>
        /// <param name="shopId">Numeric shop identifier for the Etsy shop.</param>
        /// <param name="accessToken">OAuth bearer access token for the Etsy API.</param>
        /// <param name="title">Listing title (short, human-readable).</param>
        /// <param name="description">Detailed listing description.</param>
        /// <param name="price">Listing price in the shop's currency.</param>
        /// <param name="quantity">Available quantity for the listing.</param>
        /// <param name="sku">Seller SKU for inventory matching (optional but recommended).</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>Tuple (Ok, ListingId, Error) describing result.</returns>
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
