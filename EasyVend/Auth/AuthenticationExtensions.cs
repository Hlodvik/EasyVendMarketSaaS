using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace EasyVend.Auth
{
    /// <summary>
    /// Extension methods to register authentication services for the EasyVend application.
    ///
    /// This sets up Entra ID (Azure AD / Microsoft Identity Platform) authentication using
    /// Microsoft.Identity.Web and configures OpenID Connect options via a named options
    /// setup class. Consumers should provide configuration in the "AzureAd" section of
    /// configuration (client id, tenant, etc.).
    /// </summary>
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Registers Entra ID (Azure AD) authentication and applies custom OpenIdConnect
        /// options configuration. This method wires up Microsoft.Identity.Web to handle
        /// the web app OIDC flow and also registers <see cref="OpenIdConnectOptionsSetup"/>
        /// to configure response type, PKCE, scopes and events used by EasyVend.
        ///
        /// Important security notes:
        /// - Keep client secrets and other sensitive settings in secure configuration stores
        ///   (Key Vault / environment variables) rather than checked-in files.
        /// - The application requests the "offline_access" scope so refresh tokens can be
        ///   issued for long-lived sessions; store tokens securely if you persist them.
        /// </summary>
        /// <param name="services">The service collection to register authentication into.</param>
        /// <param name="config">Application configuration (expects an "AzureAd" section).</param>
        /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
        public static IServiceCollection AddEasyVendAuthentication(
            this IServiceCollection services, IConfiguration config)
        {
            services
                .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(config.GetSection("AzureAd"));

            // Apply OpenIdConnectOptionsSetup to the default scheme so we centralize
            // PKCE/code flow, scopes and events in one place.
            services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, OpenIdConnectOptionsSetup>();

            return services;
        }
    }
}
