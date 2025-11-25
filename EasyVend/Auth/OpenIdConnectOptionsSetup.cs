using EasyVend.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace EasyVend.Auth
{
    /// <summary>
    /// Centralizes OpenIdConnect configuration (code+PKCE, scopes, events).
    ///
    /// This class is registered as a named options configurator so the same settings are
    /// consistently applied to the app's OpenIdConnect scheme. It sets response type to
    /// the authorization code flow with PKCE (recommended for public clients) and requests
    /// essential scopes required by the app.
    ///
    /// The OnTokenValidated event creates a local user row on first successful sign-in.
    /// </summary>
    public sealed class OpenIdConnectOptionsSetup : IConfigureNamedOptions<OpenIdConnectOptions>
    {
        /// <summary>
        /// Configure for the default name.
        /// </summary>
        public void Configure(OpenIdConnectOptions options) =>
            Configure(Options.DefaultName, options);

        /// <summary>
        /// Apply centralized OpenIdConnect settings.
        /// </summary>
        public void Configure(string? name, OpenIdConnectOptions options)
        {
            // Use authorization code flow with PKCE for security.
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.UsePkce = true;

            // Persist tokens on sign-in for downstream API calls (refresh token granted with offline_access).
            options.SaveTokens = true;

            // Scopes: minimal identity + email + offline access for refresh tokens.
            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");
            options.Scope.Add("offline_access");  

            // Events (create local user on first sign-in)
            options.Events ??= new OpenIdConnectEvents();
            options.Events.OnTokenValidated = async ctx =>
            {
                // Ensure we have a local user record for the authenticated principal.
                var userSvc = ctx.HttpContext.RequestServices.GetRequiredService<UserService>();
                await userSvc.EnsureUserExistsAsync(ctx.Principal!);
            };

            // Optional diagnostics (uncomment while debugging)
            // options.Events.OnRemoteFailure = ctx => {
            //     ctx.Response.Redirect("/auth-error?msg=" + Uri.EscapeDataString(ctx.Failure?.Message ?? "unknown"));
            //     ctx.HandleResponse();
            //     return Task.CompletedTask;
            // };
            // options.Events.OnAuthenticationFailed = ctx => {
            //     ctx.Response.Redirect("/auth-error?msg=" + Uri.EscapeDataString(ctx.Exception?.Message ?? "unknown"));
            //     ctx.HandleResponse();
            //     return Task.CompletedTask;
            // };
        }
    }
}
