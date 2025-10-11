using EasyVend.Services;

namespace EasyVend.Services;
public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<UserService>();
        // add app services here later
        return services;
    }
}