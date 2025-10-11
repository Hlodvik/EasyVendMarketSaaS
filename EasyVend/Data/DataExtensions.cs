using EasyVend.Data;
using Microsoft.EntityFrameworkCore;

namespace EasyVend.Data;
public static class DataExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        // EF Core migrations are used for schema changes; no runtime schema patching.
        return services;
    }
}
