using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace EasyVend.Endpoints;
public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/signin", async ctx =>
        {
            var returnUrl = ctx.Request.Query["returnUrl"].ToString();
            if (string.IsNullOrWhiteSpace(returnUrl) || !Uri.IsWellFormedUriString(returnUrl, UriKind.Relative) || !returnUrl.StartsWith('/'))
                returnUrl = "/setup-profile";
            var props = new AuthenticationProperties { RedirectUri = returnUrl };
            await ctx.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, props);
        }).AllowAnonymous();

        app.MapGet("/signout", async ctx =>
        {
            var props = new AuthenticationProperties { RedirectUri = "/" };
            await ctx.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, props);
            await ctx.SignOutAsync(); // cookie
        }).AllowAnonymous();

        return app;
    }
}