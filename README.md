# Quickstart UI for IdentityServer4
This repo contains a sample MVC based UI for login, logout, grant management and consent.

Note that the repo doesn't include solution and project files, but should be copied to your project as described below.

**note** This branch contains the files to go with the latest release of IdentityServer4. If you are looking for older versions, try [this](https://github.com/IdentityServer/IdentityServer4.Quickstart.UI/releases) page.

## Important
This organization is not maintained anymore besides critical security bugfixes (if feasible). This organization will be archived when .NET Core 3.1 end of support is reached (3rd Dec 2022). All new development is happening in the new [Duende Software](https://github.com/duendesoftware) organization. 

The new [Duende IdentityServer](https://duendesoftware.com/products/identityserver) comes with a commercial license but is [free](https://blog.duendesoftware.com/posts/20220111_fair_trade/) for dev/testing/personal projects and companies or individuals making less than 1M USD gross annnual revenue. Please [get in touch with us](https://duendesoftware.com/contact) if you have any question.

## Adding the quickstart UI
This repo contains the controllers, models, views and CSS files needed for the UI. Simply download/clone it and copy the folders into the web project.

Alternatively you can run this powershell script from your web project directory to download them automatically:

```
iex ((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/IdentityServer/IdentityServer4.Quickstart.UI/main/getmain.ps1'))
``` 

Or using bash one-liner on macOS or Linux:

```bash
curl -L https://raw.githubusercontent.com/IdentityServer/IdentityServer4.Quickstart.UI/main/getmain.sh | bash
```

Or you can use our [templates](https://github.com/IdentityServer/IdentityServer4.Templates):

```
dotnet new -i identityserver4.templates
dotnet new is4ui
```

## Issues
For issues, use the [consolidated IdentityServer4 issue tracker](https://github.com/IdentityServer/IdentityServer4/issues).
