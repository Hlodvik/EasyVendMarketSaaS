using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Note: MSAL authentication removed - was causing startup failures
// TODO: Re-implement authentication after basic app is working

await builder.Build().RunAsync();
