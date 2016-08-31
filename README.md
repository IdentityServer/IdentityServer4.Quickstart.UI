# Quickstart UI for an in-memory IdentityServer4

This repo contains a minimal UI that supplements an IdentityServer4 configured for in-memory clients, users, and scopes.

You typically start out with a startup file that looks similar to this:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc();

        services.AddIdentityServerQuickstart()
            .AddInMemoryClients(Config.GetClient())
            .AddInMemoryScopes(Config.GetScopes())
            .AddInMemoryUsers(Config.GetUsers());
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
        loggerFactory.AddConsole();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseIdentityServer();

        app.UseStaticFiles();
        app.UseMvcWithDefaultRoute();
    }
}
```

To add a simple UI for login, logout, consent and errors, clone this repo and drop the folders into your ASP.NET Core project.
