# Quickstart UI for IdentityServer4
This repo contains a sample MVC based UI for login, logout, grant management and consent.

Note that the repo doesn't include solution and project files, but should be copied to your project as described below.

**note** This branch contains the files to go with the latest release of IdentityServer4. If you are looking for older versions, try [this](https://github.com/IdentityServer/IdentityServer4.Quickstart.UI/releases) page.

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
