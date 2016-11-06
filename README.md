# Quickstart UI for an in-memory IdentityServer4

This repo contains an MVC based UI for login, logout and consent that supplements an IdentityServer4 configured for in-memory clients, users, and scopes.

## Instructions

The assumption is that you started with an empty web application, added identityserver and configured the in-memory scopes, clients and users. Your startup file will look similar to this at this point:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // configure clients, scopes and users in-memory
        services.AddIdentityServer()
            .AddTemporarySigningCredential()
            .AddInMemoryPersistedGrants()
            .AddInMemoryClients(Config.GetClients())
            .AddInMemoryScopes(Config.GetScopes())
            .AddInMemoryUsers(Config.GetUsers());
    }

    public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
    {
        loggerFactory.AddConsole();
        app.UseDeveloperExceptionPage();
      
        app.UseIdentityServer();
    }
}
```

### Adding MVC
Before you can add the quickstart UI you need to add the following packages to project.json:

```
"Microsoft.AspNetCore.Mvc": "1.0.0",
"Microsoft.AspNetCore.StaticFiles": "1.0.0"
```

...and add MVC and static files to your pipeline:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc();

        services.AddIdentityServer()
            .AddTemporarySigningCredential()
            .AddInMemoryPersistedGrants()
            .AddInMemoryClients(Config.GetClient())
            .AddInMemoryScopes(Config.GetScopes())
            .AddInMemoryUsers(Config.GetUsers());
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
        loggerFactory.AddConsole();
        app.UseDeveloperExceptionPage();

        app.UseIdentityServer();

        app.UseStaticFiles();
        app.UseMvcWithDefaultRoute();
    }
}
```

### Adding the quickstart UI

This repo contains the controllers, models, views and CSS files needed for the UI. Simply download/clone it and copy the folders into the web project.

Alternatively you can run this powershell script from your web project folder to download them automatically:

```
iex ((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/IdentityServer/IdentityServer4.Quickstart.UI/dev/get.ps1'))
``` 

### Adding support for external authentication

You can add support for external authentication providers by adding additional authentication middleware to your pipeline.
For this example we are adding support for a cloud hosted identityserver3 instance via the OpenID Connect protocol and Google authentication.

Add the following packages to project.json:

```
"Microsoft.AspNetCore.Authentication.Cookies": "1.0.0",
"Microsoft.AspNetCore.Authentication.OpenIdConnect": "1.0.0",
"Microsoft.AspNetCore.Authentication.Google": "1.0.0"
```

Next you need to configure the authentication middleware in your pipeline. As always - order is important - the additional authentication middleware must run **after** identityserver, but **before** MVC:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc();

        services.AddIdentityServer()
            .AddTemporarySigningCredential()
            .AddInMemoryPersistedGrants()
            .AddInMemoryClients(Config.GetClient())
            .AddInMemoryScopes(Config.GetScopes())
            .AddInMemoryUsers(Config.GetUsers());
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
        loggerFactory.AddConsole();
        app.UseDeveloperExceptionPage();

        app.UseIdentityServer();

        // cookie middleware for temporarily storing the outcome of the external authentication
        app.UseCookieAuthentication(new CookieAuthenticationOptions
        {
            AuthenticationScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
            AutomaticAuthenticate = false,
            AutomaticChallenge = false
        });

        // middleware for google authentication
        app.UseGoogleAuthentication(new GoogleOptions
        {
            AuthenticationScheme = "Google",
            SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
            ClientId = "434483408261-55tc8n0cs4ff1fe21ea8df2o443v2iuc.apps.googleusercontent.com",
            ClientSecret = "3gcoTrEDPPJ0ukn_aYYT6PWo"
        });
        
        // middleware for external openid connect authentication
        app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
        {
            SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
            SignOutScheme = IdentityServerConstants.SignoutScheme,

            DisplayName = "OpenID Connect",
            Authority = "https://demo.identityserver.io/",
            ClientId = "implicit",
                
            TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "name",
                RoleClaimType = "role"
            }
        });

        app.UseStaticFiles();
        app.UseMvcWithDefaultRoute();
    }
}
```

**Note** for Google authentication you need to register your local quickstart identityserver using the Google developer [console](https://console.developers.google.com). As a redirect URL, use the URL of your local identityserver and add `/signin-google`.
If your IdentityServer is running on port 5000 - you can use the above client id/secret which is pre-registered.
