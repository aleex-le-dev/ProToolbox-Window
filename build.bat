@echo off
chcp 65001 >nul
echo ========================================================
echo   Compilation de Boîte à Scripts Windows v4.0 (C# / WPF)
echo ========================================================
echo.

:: Vérifier si dotnet est disponible
where dotnet >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    if exist "%LOCALAPPDATA%\Microsoft\dotnet\dotnet.exe" (
        set "DOTNET_CMD=%LOCALAPPDATA%\Microsoft\dotnet\dotnet.exe"
    ) else (
        echo [ERREUR] .NET SDK 8.0 n'est pas détecté.
        echo Lancez dans PowerShell :
        echo   & ([scriptblock]::Create((Invoke-RestMethod -Uri https://dot.net/v1/dotnet-install.ps1))) -Channel 8.0 -InstallDir "$env:LOCALAPPDATA\Microsoft\dotnet"
        pause
        exit /b 1
    )
) else (
    set "DOTNET_CMD=dotnet"
)

echo [1/2] Compilation et publication en fichier unique .exe...
"%DOTNET_CMD%" publish "%~dp0ScriptsWindows\ScriptsWindows\ScriptsWindows.csproj" -c Release -r win-x64 --no-self-contained -p:PublishSingleFile=true

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERREUR] Échec de la compilation.
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo [2/2] Copie de ScriptsWindows.exe à la racine du projet...
copy /Y "%~dp0ScriptsWindows\ScriptsWindows\bin\Release\net8.0-windows\win-x64\publish\ScriptsWindows.exe" "%~dp0ScriptsWindows.exe" >nul

echo.
echo ========================================================
echo   [SUCCÈS] Exécutable généré : %~dp0ScriptsWindows.exe
echo ========================================================
echo.
pause
