using EasyVend.Services;
using Microsoft.AspNetCore.Authorization;

namespace EasyVend.Endpoints;
public static class ListingEndpoints
{
    /// <summary>
    /// Registers minimal API routes related to listing operations (publish to marketplaces).
    /// Routes are grouped under /api/listings and require an authenticated caller.
    /// </summary>
    public static IEndpointRouteBuilder MapListingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/listings").RequireAuthorization();

        // POST /api/listings/{productId}/publish/etsy
        // Body: none. The service infers tenant from the product row and reads stored
        // integration credentials (access token + shop id) to call Etsy.
        group.MapPost("{productId:guid}/publish/etsy", async (Guid productId, ListingPublisher publisher, CancellationToken ct) =>
        {
            var result = await publisher.PublishToEtsyAsync(productId, ct);
            return result.Ok ? Results.Ok(new { listingId = result.Msg }) : Results.BadRequest(new { error = result.Msg });
        });

        return app;
    }
}
