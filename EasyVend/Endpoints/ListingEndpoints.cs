using EasyVend.Services;
using Microsoft.AspNetCore.Authorization;

namespace EasyVend.Endpoints;
public static class ListingEndpoints
{
    public static IEndpointRouteBuilder MapListingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/listings").RequireAuthorization();

        // POST /api/listings/{productId}/publish/etsy
        group.MapPost("{productId:guid}/publish/etsy", async (Guid productId, ListingPublisher publisher, CancellationToken ct) =>
        {
            var result = await publisher.PublishToEtsyAsync(productId, ct);
            return result.Ok ? Results.Ok(new { listingId = result.Msg }) : Results.BadRequest(new { error = result.Msg });
        });

        return app;
    }
}
