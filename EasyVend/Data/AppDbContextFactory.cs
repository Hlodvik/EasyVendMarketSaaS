using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace EasyVend.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();
            var env = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                      ?? System.Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                      ?? "Production";

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .AddJsonFile("appsettings.Local.json", optional: true)
                .AddJsonFile($"appsettings.{env}.Local.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connection = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Missing DefaultConnection.");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            optionsBuilder.UseSqlServer(connection, sqlOptions =>
            {
                // retry transient connection issues automatically
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            });

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
