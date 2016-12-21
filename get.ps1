$source = "https://github.com/IdentityServer/IdentityServer4.Quickstart.UI/archive/release.zip"
Invoke-WebRequest $source -OutFile ui.zip

Expand-Archive ui.zip

if (!(Test-Path -Path Quickstart))  { mkdir Quickstart }
if (!(Test-Path -Path Views))       { mkdir Views }
if (!(Test-Path -Path wwwroot))     { mkdir wwwroot }

copy .\ui\IdentityServer4.Quickstart.UI-release\Quickstart\* Quickstart -recurse -force
copy .\ui\IdentityServer4.Quickstart.UI-release\Views\* Views -recurse -force
copy .\ui\IdentityServer4.Quickstart.UI-release\wwwroot\* wwwroot -recurse -force

del ui.zip
del ui -recurse
