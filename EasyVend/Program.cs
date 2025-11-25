using EasyVend.Auth;         // AddEasyVendAuthentication, AddEasyVendAuthorization
using EasyVend.Data;         // AddDatabase
using EasyVend.Services;     // AddApplicationServices
using EasyVend.Presentation; // AddBlazorUi
using EasyVend.Pipeline;     // UseWebPipeline
using EasyVend.Endpoints;    // MapAuthEndpoints, MapComponentEndpoints
using Microsoft.EntityFrameworkCore; // Database.Migrate
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Load optional local, gitignored secrets file (developer convenience)
    builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

    // Use Serilog for logging (console + file via configuration)
    builder.Logging.ClearProviders();
    builder.Host.UseSerilog((ctx, services, cfg) => cfg
        .ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // Authentication (Entra ID + OIDC Code+PKCE via OpenIdConnectOptionsSetup)
    // NOTE: This registers the Microsoft.Identity.Web handlers and configures
    // OpenIdConnect options. See Auth/OpenIdConnectOptionsSetup for details.
    builder.Services.AddEasyVendAuthentication(builder.Configuration);

    // Authorization (modern AddAuthorizationBuilder with fallback policy)
    builder.Services.AddEasyVendAuthorization();

    // Data layer (EF Core SQL Server using "DefaultConnection")
    builder.Services.AddDatabase(builder.Configuration);

    // App services (UserService, etc.)
    builder.Services.AddApplicationServices();

    // Blazor UI (server + WASM interactivity)
    builder.Services.AddBlazorUi();

    var app = builder.Build();

    // Apply EF Core migrations at startup (uses configured connection string)
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        try
        {
            Log.Information("Applying database migrations…");
            db.Database.Migrate();
            Log.Information("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Database migration failed");
            throw;
        }
    }

    // Request logging
    app.UseSerilogRequestLogging();

    // Middleware pipeline (HTTPS, static, auth/authorize, antiforgery, prod error handling)
    app.UseWebPipeline();

    // Minimal auth endpoints (/signin, /signout) and Razor components
    app.MapAuthEndpoints()
       .MapComponentEndpoints()
       .MapListingEndpoints(); // Listings API endpoints (publish to Etsy)

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
