namespace EasyVend.Presentation;
public static class UiExtensions
{
    public static IServiceCollection AddBlazorUi(this IServiceCollection services)
    {
        services.AddRazorComponents()
            // Enable detailed circuit errors to diagnose interactivity crashes (turn off after debugging)
            .AddInteractiveServerComponents(options => options.DetailedErrors = true)
            .AddInteractiveWebAssemblyComponents();
        return services;
    }
}