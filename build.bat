@echo off
setlocal
echo ========================================================
echo   Compilation de Alex Toolkit
echo ========================================================
echo.

:: Priorite au SDK installe localement dans LocalAppData
if exist "%LOCALAPPDATA%\Microsoft\dotnet\dotnet.exe" (
    set "DOTNET_ROOT=%LOCALAPPDATA%\Microsoft\dotnet"
    set "DOTNET_MULTILEVEL_LOOKUP=0"
    set "PATH=%LOCALAPPDATA%\Microsoft\dotnet;%PATH%"
    set "DOTNET_CMD=%LOCALAPPDATA%\Microsoft\dotnet\dotnet.exe"
) else (
    set "DOTNET_CMD=dotnet"
)

echo [1/2] Compilation et publication en fichier unique .exe...
"%DOTNET_CMD%" publish "%~dp0ScriptsWindows\ScriptsWindows\ScriptsWindows.csproj" -c Release -r win-x64 --no-self-contained -p:PublishSingleFile=true

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERREUR] Echec de la compilation.
    echo Verifiez que le SDK .NET 8.0 est bien present.
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo [2/2] Mise a jour de AlexToolkit.exe a la racine...
copy /Y "%~dp0ScriptsWindows\ScriptsWindows\bin\Release\net8.0-windows\win-x64\publish\AlexToolkit.exe" "%~dp0AlexToolkit.exe" >nul 2>&1

if %ERRORLEVEL% NEQ 0 (
    :: Si le fichier est ouvert, on le renomme temporairement pour permettre la copie
    move /Y "%~dp0AlexToolkit.exe" "%~dp0AlexToolkit.old.exe" >nul 2>&1
    copy /Y "%~dp0ScriptsWindows\ScriptsWindows\bin\Release\net8.0-windows\win-x64\publish\AlexToolkit.exe" "%~dp0AlexToolkit.exe" >nul
)
if exist "%~dp0AlexToolkit.old.exe" del /f /q "%~dp0AlexToolkit.old.exe" >nul 2>&1

:: Nettoyage de l'ancien binaire ScriptsWindows.exe s'il existe
if exist "%~dp0ScriptsWindows.exe" del /f /q "%~dp0ScriptsWindows.exe" >nul 2>&1

echo.
echo ========================================================
echo   [SUCCES] Executable genere : %~dp0AlexToolkit.exe
echo ========================================================
echo.
pause