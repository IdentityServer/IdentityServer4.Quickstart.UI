# Quickstart UI for an in-memory IdentityServer4 v2

This repo contains an MVC based UI for login, logout and consent that supplements an IdentityServer4 configured for in-memory clients, users, and scopes. 
Note that the repo doesn't include solution and project files, but should be copied to your project as described below.

**note** This branch contains the files for IdentityServer4 v2 and ASP.NET Core / MVC 2. The files for IdentityServer 1.x and ASP.NET Core 1.x can be found [on this branch](https://github.com/IdentityServer/IdentityServer4.Quickstart.UI/tree/aspnetcore1). The documentation for 1.x can be found [here](http://docs.identityserver.io/en/aspnetcore1/).

## Issues

For issues, use the [consolidated IdentityServer4 issue tracker](https://github.com/IdentityServer/IdentityServer4/issues).

## Instructions

The assumption is that you started with an empty web application, added identityserver and configured the resources, clients and users. 

### Adding MVC

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc();

        // rest omitted
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseStaticFiles();

        app.UseIdentityServer();

        app.UseMvcWithDefaultRoute();
    }
}
```

### Adding the quickstart UI

This repo contains the controllers, models, views and CSS files needed for the UI. Simply download/clone it and copy the folders into the web project.

Alternatively you can run this powershell script from your web project directory to download them automatically:

```
iex ((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/IdentityServer/IdentityServer4.Quickstart.UI/master/getmaster.ps1'))
``` 

Or using bash one-liner on macOS or Linux:

```bash
\curl -L https://raw.githubusercontent.com/IdentityServer/IdentityServer4.Quickstart.UI/master/getmaster.sh | bash
```

### Adding support for external authentication

You can add support for external authentication providers by adding additional authentication handlers.
For this example we are adding support for a cloud hosted identityserver instance via the OpenID Connect protocol and Google authentication.

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc();
        
        // some details omitted
        services.AddIdentityServer();
        
          services.AddAuthentication()
            .AddGoogle("Google", options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                options.ClientId = "<insert here>";
                options.ClientSecret = "<inser here>";
            })
            .AddOpenIdConnect("demoidsrv", "IdentityServer", options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.SignOutScheme = IdentityServerConstants.SignoutScheme;

                options.Authority = "https://demo.identityserver.io/";
                options.ClientId = "implicit";
                options.ResponseType = "id_token";
                options.SaveTokens = true;
                options.CallbackPath = new PathString("/signin-idsrv");
                options.SignedOutCallbackPath = new PathString("/signout-callback-idsrv");
                options.RemoteSignOutPath = new PathString("/signout-idsrv");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            });
    }
}
```

**Note** for Google authentication you need to register your local quickstart identityserver using the Google developer [console](https://console.developers.google.com). As a redirect URL, use the URL of your local identityserver and add `/signin-google`.
