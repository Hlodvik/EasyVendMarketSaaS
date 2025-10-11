using EasyVend;
using EasyVend.FrontEnd.Components;

namespace EasyVend.Endpoints;
public static class ComponentEndpoints
{
    public static IEndpointRouteBuilder MapComponentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapRazorComponents<App>()
           .AddInteractiveServerRenderMode()
           .AddInteractiveWebAssemblyRenderMode();
        return app;
    }
}