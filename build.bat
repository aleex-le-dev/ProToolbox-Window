@echo off
setlocal
echo ========================================================
echo   Compilation de Scripts Windows v4.0 (C# / WPF)
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
echo [2/2] Mise a jour de ScriptsWindows.exe a la racine...
copy /Y "%~dp0ScriptsWindows\ScriptsWindows\bin\Release\net8.0-windows\win-x64\publish\ScriptsWindows.exe" "%~dp0ScriptsWindows.exe" >nul

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [INFO] ScriptsWindows.exe a la racine est actuellement ouvert.
    echo Fermez l'application ouverte pour mettre a jour le fichier a la racine.
    echo Le nouvel executable est pret dans :
    echo   ScriptsWindows\ScriptsWindows\bin\Release\net8.0-windows\win-x64\publish\ScriptsWindows.exe
    echo.
    pause
    exit /b 0
)

echo.
echo ========================================================
echo   [SUCCES] Executable genere : %~dp0ScriptsWindows.exe
echo ========================================================
echo.
pause