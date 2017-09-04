$source = "https://github.com/IdentityServer/IdentityServer4.Quickstart.UI/archive/aspnetcore1.zip"
Invoke-WebRequest $source -OutFile ui.zip

Expand-Archive ui.zip

if (!(Test-Path -Path Quickstart))  { mkdir Quickstart }
if (!(Test-Path -Path Views))       { mkdir Views }
if (!(Test-Path -Path wwwroot))     { mkdir wwwroot }

copy .\ui\IdentityServer4.Quickstart.UI-aspnetcore1\Quickstart\* Quickstart -recurse -force
copy .\ui\IdentityServer4.Quickstart.UI-aspnetcore1\Views\* Views -recurse -force
copy .\ui\IdentityServer4.Quickstart.UI-aspnetcore1\wwwroot\* wwwroot -recurse -force

del ui.zip
del ui -recurse
