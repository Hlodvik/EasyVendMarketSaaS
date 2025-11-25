using EasyVend.Data;
using EasyVend.Models;
using EasyVend.Services.Integrations;
using Microsoft.EntityFrameworkCore;

namespace EasyVend.Services
{
    /// <summary>
    /// Coordinates publishing a product listing to an external marketplace (Etsy for now).
    ///
    /// This service is responsible for:
    ///  - resolving the marketplace integration for a product's tenant,
    ///  - loading integration credentials (access token, shop id),
    ///  - calling the marketplace client, and
    ///  - recording sync logs and created listing records.
    /// </summary>
    public class ListingPublisher(AppDbContext db, IEtsyClient etsy)
    {
        private readonly AppDbContext _db = db;
        private readonly IEtsyClient _etsy = etsy;

        /// <summary>
        /// Publish the specified product to Etsy. The tenant is inferred from the product row.
        /// Returns (true, externalListingId) on success or (false, errorMessage) on failure.
        /// </summary>
        /// <param name="productId">Local product identifier to publish.</param>
        /// <param name="ct">Optional cancellation token.</param>
        public async Task<(bool Ok, string Msg)> PublishToEtsyAsync(Guid productId, CancellationToken ct = default)
        {
            var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == productId, ct);
            if (product == null) return (false, "Product not found.");

            // Find the Etsy integration for this product's tenant
            var integration = await _db.MarketplaceIntegrations
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TenantId == product.TenantId && x.Platform == "Etsy", ct);

            if (integration == null)
                return (false, "Etsy is not connected for this tenant.");

            // Load required credentials: access_token and shop_id
            var creds = await _db.IntegrationCredentials
                .Where(c => c.IntegrationId == integration.IntegrationId)
                .ToListAsync(ct);

            var accessToken = creds.FirstOrDefault(c => c.KeyName == "access_token")?.KeyValue;
            var shopIdStr = creds.FirstOrDefault(c => c.KeyName == "shop_id")?.KeyValue;

            if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(shopIdStr) || !long.TryParse(shopIdStr, out var shopId))
                return (false, "Missing Etsy access token or shop id.");

            var (ok, listingId, err) = await _etsy.CreateListingAsync(
                shopId,
                accessToken,
                product.Title,
                product.Description ?? string.Empty,
                product.Price,
                Math.Max(1, product.QuantityAvailable),
                product.SKU,
                ct);

            if (!ok)
            {
                // Record a failed sync attempt with error details for troubleshooting.
                _db.IntegrationSyncLogs.Add(new IntegrationSyncLog
                {
                    SyncLogId = Guid.NewGuid(),
                    IntegrationId = integration.IntegrationId,
                    Timestamp = DateTime.UtcNow,
                    Action = "CreateListing",
                    Result = "Failed",
                    ErrorMessage = err
                });
                await _db.SaveChangesAsync(ct);
                return (false, err);
            }

            // Persist listing record and successful sync log
            _db.Listings.Add(new Listing
            {
                ListingId = Guid.NewGuid(),
                ProductId = product.ProductId,
                IntegrationId = integration.IntegrationId,
                ExternalListingId = listingId,
                Status = "Created"
            });

            _db.IntegrationSyncLogs.Add(new IntegrationSyncLog
            {
                SyncLogId = Guid.NewGuid(),
                IntegrationId = integration.IntegrationId,
                Timestamp = DateTime.UtcNow,
                Action = "CreateListing",
                Result = "Success",
                ErrorMessage = null
            });

            await _db.SaveChangesAsync(ct);
            return (true, listingId);
        }
    }
}
