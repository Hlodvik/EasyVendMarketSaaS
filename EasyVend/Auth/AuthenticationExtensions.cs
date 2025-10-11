using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace EasyVend.Auth
{
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Registers Entra ID auth and applies our OpenIdConnect options via named options.
        /// </summary>
        public static IServiceCollection AddEasyVendAuthentication(
            this IServiceCollection services, IConfiguration config)
        {
            services
                .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(config.GetSection("AzureAd"));

            // Apply OpenIdConnectOptionsSetup to the default scheme
            services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, OpenIdConnectOptionsSetup>();

            return services;
        }
    }
}
