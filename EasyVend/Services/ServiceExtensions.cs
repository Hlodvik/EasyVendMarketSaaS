using EasyVend.Services;
using EasyVend.Services.Integrations;

namespace EasyVend.Services;
public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<UserService>();
        services.AddScoped<ListingPublisher>();
        services.AddHttpClient<IEtsyClient, EtsyClient>();
        return services;
    }
}