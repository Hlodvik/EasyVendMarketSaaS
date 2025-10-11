using EasyVend.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace EasyVend.Auth
{
    /// <summary>
    /// Centralizes OpenIdConnect configuration (code+PKCE, scopes, events).
    /// </summary>
    public sealed class OpenIdConnectOptionsSetup : IConfigureNamedOptions<OpenIdConnectOptions>
    {
        public void Configure(OpenIdConnectOptions options) =>
            Configure(Options.DefaultName, options);

        public void Configure(string? name, OpenIdConnectOptions options)
        {
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.UsePkce = true;
            options.SaveTokens = true;

            // Minimal scopes + email
            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");
            options.Scope.Add("offline_access");  

            // Events (create local user on first sign-in)
            options.Events ??= new OpenIdConnectEvents();
            options.Events.OnTokenValidated = async ctx =>
            {
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
