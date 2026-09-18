@echo off
setlocal enabledelayedexpansion

echo ================================================
echo OpenReportViewer Report Generator - Portable Setup
echo ================================================
echo.

set "APP_NAME=OpenReportViewer Report Generator"
set "EXE_NAME=OpenReportViewer.UI.Wpf.exe"
set "INSTALL_DIR=%LOCALAPPDATA%\OpenReportViewerReportGenerator"
set "SHORTCUT_PATH=%USERPROFILE%\Desktop\%APP_NAME%.lnk"

echo Creating installation directory...
if not exist "%INSTALL_DIR%" (
    mkdir "%INSTALL_DIR%"
    echo Directory created: %INSTALL_DIR%
) else (
    echo Directory already exists: %INSTALL_DIR%
)

echo.
echo Copying files...
copy /Y "%EXE_NAME%" "%INSTALL_DIR%\%EXE_NAME%" > nul
if %errorlevel% equ 0 (
    echo Successfully copied %EXE_NAME%
) else (
    echo ERROR: Failed to copy %EXE_NAME%
    pause
    exit /b 1
)

echo.
echo Creating desktop shortcut...
powershell -Command "$ws = New-Object -ComObject WScript.Shell; $s = $ws.CreateShortcut('%SHORTCUT_PATH%'); $s.TargetPath = '%INSTALL_DIR%\%EXE_NAME%'; $s.WorkingDirectory = '%INSTALL_DIR%'; $s.Description = '%APP_NAME%'; $s.Save()"

if %errorlevel% equ 0 (
    echo Shortcut created: %SHORTCUT_PATH%
) else (
    echo WARNING: Failed to create shortcut
)

echo.
echo ================================================
echo Installation Complete!
echo ================================================
echo.
echo Application installed to: %INSTALL_DIR%
echo Desktop shortcut: %SHORTCUT_PATH%
echo.
echo Press any key to launch the application...
pause > nul

start "" "%INSTALL_DIR%\%EXE_NAME%"

echo.
echo Setup complete! You can close this window.

