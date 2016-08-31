$source = "https://github.com/IdentityServer/IdentityServer4.Quickstart.UI/archive/dev.zip"
Invoke-WebRequest $source -OutFile ui.zip

Expand-Archive ui.zip

move .\ui\IdentityServer4.Quickstart.UI-dev\Controllers\
move .\ui\IdentityServer4.Quickstart.UI-dev\Models\
move .\ui\IdentityServer4.Quickstart.UI-dev\Views\
move .\ui\IdentityServer4.Quickstart.UI-dev\wwwroot\

del ui.zip
del ui -Recurse
