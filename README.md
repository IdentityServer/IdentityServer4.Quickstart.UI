# Important update
This project is not maintained anymore. This repo will be archived when .NET Core 3.1 end of support is reached (13th Dec 2022). All new development is happening in the new [Duende Software](https://github.com/duendesoftware) organization. 

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
