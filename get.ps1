$source = "https://github.com/IdentityServer/IdentityServer4.Quickstart.UI/archive/dev.zip"
Invoke-WebRequest $source -OutFile ui.zip

Expand-Archive ui.zip

if (!(Test-Path -Path Controllers)) { mkdir Controllers }
if (!(Test-Path -Path Models))      { mkdir Models }
if (!(Test-Path -Path Views))       { mkdir Views }
if (!(Test-Path -Path wwwroot))     { mkdir wwwroot }

copy .\ui\IdentityServer4.Quickstart.UI-dev\Controllers\* Controllers -recurse -force
copy .\ui\IdentityServer4.Quickstart.UI-dev\Models\* Models -recurse -force
copy .\ui\IdentityServer4.Quickstart.UI-dev\Views\* Views -recurse -force
copy .\ui\IdentityServer4.Quickstart.UI-dev\wwwroot\* wwwroot -recurse -force

del ui.zip
del ui -recurse
