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
set "SLN=%ROOT%\OpenReportViewer.sln"

echo [1/4] Restore modules individually...
"%DOTNET%" restore "%ROOT%\src\OpenReportViewer.Core\OpenReportViewer.Core.csproj" -m:1 || exit /b 1
"%DOTNET%" restore "%ROOT%\src\OpenReportViewer.Parsers\OpenReportViewer.Parsers.csproj" -m:1 || exit /b 1
"%DOTNET%" restore "%ROOT%\src\OpenReportViewer.Reporting\OpenReportViewer.Reporting.csproj" -m:1 || exit /b 1
"%DOTNET%" restore "%ROOT%\src\OpenReportViewer.AI\OpenReportViewer.AI.csproj" -m:1 || exit /b 1
"%DOTNET%" restore "%ROOT%\src\OpenReportViewer.Tests\OpenReportViewer.Tests.csproj" -m:1 || exit /b 1
"%DOTNET%" restore "%ROOT%\src\OpenReportViewer.UI.Wpf\OpenReportViewer.UI.Wpf.csproj" -m:1 || exit /b 1

echo [2/4] Build solution --no-restore...
"%DOTNET%" build "%SLN%" --no-restore -m:1 || exit /b 1

echo [3/4] Test...
"%DOTNET%" test "%ROOT%\src\OpenReportViewer.Tests\OpenReportViewer.Tests.csproj" --no-build || exit /b 1

echo.
echo Verification complete.
endlocal
