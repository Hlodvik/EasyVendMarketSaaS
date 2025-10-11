using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace EasyVend.Auth
{
    public static class AuthorizationExtensions
    {
        /// Registers authorization services with the modern builder API and sets policies.
        public static IServiceCollection AddEasyVendAuthorization(this IServiceCollection services)
        {
            var authz = services.AddAuthorizationBuilder();

            // Require auth by default; public pages/components use [AllowAnonymous]
            authz.SetFallbackPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());

            // Examples (delete if unused):
            // authz.AddPolicy("RequireAdmin", p => p.RequireRole("Admin"));
            // authz.AddPolicy("CompletedProfile", p => p.RequireClaim("profile_complete", "true"));

            return services;
        }
    }
}