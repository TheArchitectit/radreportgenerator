@echo off
setlocal enabledelayedexpansion

echo ================================================
echo Live Optics Report Generator - Uninstall
echo ================================================
echo.

set "APP_NAME=Live Optics Report Generator"
set "INSTALL_DIR=%LOCALAPPDATA%\LiveOpticsReportGenerator"
set "SHORTCUT_PATH=%USERPROFILE%\Desktop\%APP_NAME%.lnk"

echo This will remove the Live Optics Report Generator.
echo.

if exist "%INSTALL_DIR%" (
    echo Removing installation directory...
    rmdir /s /q "%INSTALL_DIR%"
    echo Directory removed: %INSTALL_DIR%
) else (
    echo Installation directory not found: %INSTALL_DIR%
)

if exist "%SHORTCUT_PATH%" (
    echo Removing desktop shortcut...
    del /q "%SHORTCUT_PATH%"
    echo Shortcut removed
) else (
    echo Desktop shortcut not found
)

echo.
echo ================================================
echo Uninstallation Complete!
echo ================================================
echo.
pause
