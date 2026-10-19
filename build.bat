@echo off
setlocal EnableExtensions EnableDelayedExpansion
title Compilation automatique - Alex Toolkit

echo ========================================================
echo   Compilation automatique de Alex Toolkit
echo ========================================================
echo.

set "CSPROJ=%~dp0ScriptsWindows\ScriptsWindows\ScriptsWindows.csproj"
set "PUBLISH_EXE=%~dp0ScriptsWindows\ScriptsWindows\bin\Release\net8.0-windows\win-x64\publish\AlexToolkit.exe"

:: 1. Detection du SDK .NET 8
if exist "%LOCALAPPDATA%\Microsoft\dotnet\dotnet.exe" (
    set "DOTNET_ROOT=%LOCALAPPDATA%\Microsoft\dotnet"
    set "DOTNET_MULTILEVEL_LOOKUP=0"
    set "PATH=%LOCALAPPDATA%\Microsoft\dotnet;%PATH%"
    set "DOTNET_CMD=%LOCALAPPDATA%\Microsoft\dotnet\dotnet.exe"
) else (
    set "DOTNET_CMD=dotnet"
)

:: 2. Auto-increment de la version (+0.0.1) sans intervention manuelle
for /f "tokens=1,2 delims=;" %%A in ('powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0ScriptsWindows\bump_version.ps1"') do (
    set "OLD_VER=%%A"
    set "NEW_VER=%%B"
)

if "%NEW_VER%"=="" (
    echo [ERREUR] Impossible de determiner la nouvelle version.
    pause
    exit /b 1
)

echo Version precedente : %OLD_VER%
echo Nouvelle version   : %NEW_VER%  (+0.0.1 automatique)
echo.

set "FINAL_EXE=%~dp0AlexToolkit_v%NEW_VER%.exe"

:: 3. Compilation et publication en fichier unique autonome .exe
echo [1/2] Compilation en cours...
"%DOTNET_CMD%" publish "%CSPROJ%" -c Release -r win-x64 --no-self-contained -p:PublishSingleFile=true

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ========================================================
    echo   [ERREUR] Echec de la compilation.
    echo   Verifiez que le SDK .NET 8.0 est bien present.
    echo ========================================================
    pause
    exit /b %ERRORLEVEL%
)

:: 4. Suppression des anciens executables a la racine
echo.
echo [2/2] Nettoyage des anciens executables et renommage...

for %%F in ("%~dp0AlexToolkit*.exe") do (
    if exist "%%F" del /f /q "%%F" >nul 2>&1
    if exist "%%F" (
        move /Y "%%F" "%%F.old" >nul 2>&1
        del /f /q "%%F.old" >nul 2>&1
    )
)

if exist "%~dp0ScriptsWindows.exe" del /f /q "%~dp0ScriptsWindows.exe" >nul 2>&1

:: 5. Copie du nouveau binaire renomme a la racine
copy /Y "%PUBLISH_EXE%" "%FINAL_EXE%" >nul

echo.
echo ======================================================================
echo   [SUCCES] Build termine avec succes !
echo ======================================================================
echo.
echo   * NOUVEAU FICHIER A LA RACINE :
echo     AlexToolkit_v%NEW_VER%.exe
echo.
echo   Pour votre mise a jour GitHub :
echo   1. Creez la Release sur GitHub avec le tag : v%NEW_VER%
echo   2. Glissez-deposez ce fichier : AlexToolkit_v%NEW_VER%.exe
echo   3. Cliquez sur "Publish release".
echo ======================================================================
echo.
pause