@echo off
setlocal
rem NuGet.Configuration fails when ProgramFiles vars are missing (agent hosts).
if not defined ProgramFiles set "ProgramFiles=C:\Program Files"
if not defined "ProgramFiles(x86)" set "ProgramFiles(x86)=C:\Program Files (x86)"
if not defined ProgramW6432 set "ProgramW6432=C:\Program Files"
if not defined NUGET_PACKAGES set "NUGET_PACKAGES=%USERPROFILE%\.nuget\packages"
if not defined DOTNET_CLI_HOME set "DOTNET_CLI_HOME=%USERPROFILE%"

set "DOTNET=C:\Program Files\dotnet\dotnet.exe"
set "ROOT=%~dp0.."
set "SLN=%ROOT%\LiveOptics.sln"

echo [1/3] Restore projects individually...
"%DOTNET%" restore "%ROOT%\src\OpenReportViewer.Core\LiveOptics.Core.csproj" || exit /b 1
"%DOTNET%" restore "%ROOT%\src\OpenReportViewer.Tests\LiveOptics.Tests.csproj" || exit /b 1
"%DOTNET%" restore "%ROOT%\src\OpenReportViewer.UI.Wpf\LiveOptics.UI.Wpf.csproj" || exit /b 1

echo [2/3] Build solution --no-restore...
"%DOTNET%" build "%SLN%" --no-restore || exit /b 1

echo [3/3] Test...
"%DOTNET%" test "%ROOT%\src\OpenReportViewer.Tests\LiveOptics.Tests.csproj" --no-build || exit /b 1

echo.
echo Sprint 0 verification complete.
endlocal
