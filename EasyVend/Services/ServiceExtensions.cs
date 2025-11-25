using EasyVend.Services;
using EasyVend.Services.Integrations;

namespace EasyVend.Services;
public static class ServiceExtensions
{
    /// <summary>
    /// Registers application-specific services into the DI container.
    ///
    /// Add new services here to make them available via constructor injection
    /// throughout the application.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Core application services
        services.AddScoped<UserService>();

        // Listing publication coordination service
        services.AddScoped<ListingPublisher>();

        // Typed HttpClient for Etsy API integration. This registers an IEtsyClient
        // that uses an HttpClient supplied by the factory. You can configure the
        // client (timeouts, base address) where services are added if needed.
        services.AddHttpClient<IEtsyClient, EtsyClient>();

        return services;
    }
}