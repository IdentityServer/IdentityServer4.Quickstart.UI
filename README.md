# Quickstart UI for an in-memory IdentityServer4

This repo contains a minimal UI that supplements an IdentityServer4 configured for in-memory clients, users, and scopes.

You typically start out with an empty web project and a startup file that looks similar to this:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc();

        // configure clients, scopes and users in-memory
        services.AddIdentityServerQuickstart()
            .AddInMemoryClients(Config.GetClients())
            .AddInMemoryScopes(Config.GetScopes())
            .AddInMemoryUsers(Config.GetUsers());
    }

    public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
    {
        loggerFactory.AddConsole();
        app.UseDeveloperExceptionPage();
      
        app.UseIdentityServer();

        app.UseStaticFiles();
        app.UseMvcWithDefaultRoute();
    }
}
```

To add a simple UI for login, logout, consent and errors, clone this repo and drop the folders into your ASP.NET Core project.

`git clone https://github.com/IdentityServer/IdentityServer4.Quickstart.UI`
