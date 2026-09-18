@echo off
rem Agent/host environments may omit ProgramFiles vars required by NuGet.Configuration.
if not defined ProgramFiles set "ProgramFiles=C:\Program Files"
if not defined ProgramFiles(x86) set "ProgramFiles(x86)=C:\Program Files (x86)"
if not defined ProgramW6432 set "ProgramW6432=C:\Program Files"
if not defined NUGET_PACKAGES set "NUGET_PACKAGES=%USERPROFILE%\.nuget\packages"
if not defined DOTNET_CLI_HOME set "DOTNET_CLI_HOME=%USERPROFILE%"
"C:\Program Files\dotnet\dotnet.exe" %*

