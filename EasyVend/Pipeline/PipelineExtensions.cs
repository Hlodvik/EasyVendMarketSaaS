namespace EasyVend.Pipeline;
public static class PipelineExtensions
{
    public static WebApplication UseWebPipeline(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();

        // Redirect authenticated users from "/" to "/dashboard"
        app.Use(async (ctx, next) =>
        {
            if (HttpMethods.IsGet(ctx.Request.Method)
                && string.Equals(ctx.Request.Path.Value, "/", StringComparison.Ordinal)
                && ctx.User?.Identity?.IsAuthenticated == true)
            {
                ctx.Response.Redirect("/dashboard");
                return;
            }
            await next();
        });

        app.UseAntiforgery(); // after auth

        return app;
    }
}