using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EasyVend.Services.Integrations;

/// <summary>
/// Minimal Etsy API client for creating a listing. Uses OAuth access token stored per-integration.
/// NOTE: This is a simplified example and may need additional required fields depending on your shop setup.
/// </summary>
public class EtsyClient(HttpClient http) : IEtsyClient
{
    // HttpClient injected by DI; typed client recommended so base address/config can be applied centrally.
    private readonly HttpClient _http = http;

    // Use System.Text.Json default web options for compact JSON formatting and correct naming policy.
    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Creates a new listing in the given Etsy shop using the provided access token.
    /// Returns (true, listingId, empty) on success, or (false, empty, errorMessage) on failure.
    /// </summary>
    public async Task<(bool Ok, string ListingId, string Error)> CreateListingAsync(
        long shopId,
        string accessToken,
        string title,
        string description,
        decimal price,
        int quantity,
        string sku,
        CancellationToken ct = default)
    {
        // Etsy V3 OpenAPI endpoint for creating a listing (draft by default) under shop
        // POST https://openapi.etsy.com/v3/application/shops/{shop_id}/listings
        var url = $"https://openapi.etsy.com/v3/application/shops/{shopId}/listings";

        using var req = new HttpRequestMessage(HttpMethod.Post, url);

        // Attach OAuth bearer token to authorize the request.
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // Minimal required payload. In practice you will need taxonomy_id, shipping_profile_id, who_made, when_made, is_supply, etc.
        var payload = new
        {
            title,
            description,
            who_made = "i_did",
            when_made = "made_to_order",
            is_supply = false,
            type = "physical",
            price = price.ToString("0.00"),
            quantity,
            skus = new[] { sku },
            state = "draft"
        };

        req.Content = new StringContent(JsonSerializer.Serialize(payload, _json), Encoding.UTF8, "application/json");

        using var resp = await _http.SendAsync(req, ct);
        var body = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode)
        {
            // Return full response body to aid diagnostics in logs/UI (be careful not to leak sensitive data).
            return (false, string.Empty, $"HTTP {(int)resp.StatusCode}: {body}");
        }

        try
        {
            using var doc = JsonDocument.Parse(body);
            var id = doc.RootElement.GetProperty("listing_id").GetRawText();

            // listing_id might be number; normalize to string
            var listingId = doc.RootElement.TryGetProperty("listing_id", out var idProp)
                ? idProp.ValueKind switch
                {
                    JsonValueKind.String => idProp.GetString()!,
                    JsonValueKind.Number => idProp.GetInt64().ToString(),
                    _ => string.Empty
                }
                : string.Empty;

            return (true, listingId, string.Empty);
        }
        catch (Exception ex)
        {
            // Parsing failed: return error with original body for troubleshooting.
            return (false, string.Empty, $"Parse error: {ex.Message}: {body}");
        }
    }
}
