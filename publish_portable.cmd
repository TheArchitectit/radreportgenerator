@echo off
echo Building Portable Single-File Executable for Live Optics Report Generator...

cd src\OpenReportViewer.UI.Wpf

dotnet publish LiveOptics.UI.Wpf.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ..\..\PortableBuild

echo.
echo Build complete!
echo You can find the portable executable in the "PortableBuild" folder.
pause
