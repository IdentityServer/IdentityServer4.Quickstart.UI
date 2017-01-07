# Quickstart UI for an in-memory IdentityServer4

This repo contains an MVC based UI for login, logout and consent that supplements an IdentityServer4 configured for in-memory clients, users, and scopes.

## Instructions

The assumption is that you started with an empty web application, added identityserver and configured the resources, clients and users. 

### Adding MVC
The quickstart UI uses MVC. Before you can add the UI you need to add the following packages to project.json:

```
"Microsoft.AspNetCore.Mvc": "1.1.0",
"Microsoft.AspNetCore.StaticFiles": "1.1.0"
```

...and add MVC and static files to your pipeline:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc();

        // rest omitted
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

Alternatively you can run this powershell script from your web project directory to download them automatically:

```
iex ((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/IdentityServer/IdentityServer4.Quickstart.UI/release/get.ps1'))
``` 

### Adding support for external authentication

You can add support for external authentication providers by adding additional authentication middleware to your pipeline.
For this example we are adding support for a cloud hosted identityserver3 instance via the OpenID Connect protocol and Google authentication.

Add the following packages to project.json:

```
"Microsoft.AspNetCore.Authentication.Cookies": "1.1.0",
"Microsoft.AspNetCore.Authentication.OpenIdConnect": "1.1.0",
"Microsoft.AspNetCore.Authentication.Google": "1.1.0"
```

Next you need to configure the authentication middleware in your pipeline. As always - order is important - the additional authentication middleware must run **after** identityserver, but **before** MVC:

```csharp
public class Startup
{
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
            ClientId = "708996912208-9m4dkjb5hscn7cjrn5u0r4tbgkbj1fko.apps.googleusercontent.com",
            ClientSecret = "wdfPY6t8H8cecgjlxud__4Gh"
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
