@echo off
chcp 65001 >nul
echo ========================================================
echo   Nettoyage des fichiers temporaires et caches de build
echo ========================================================
echo.

echo Suppression des dossiers de compilation bin/ et obj/...
if exist "%~dp0ScriptsWindows\ScriptsWindows\bin" rmdir /s /q "%~dp0ScriptsWindows\ScriptsWindows\bin"
if exist "%~dp0ScriptsWindows\ScriptsWindows\obj" rmdir /s /q "%~dp0ScriptsWindows\ScriptsWindows\obj"

echo Suppression des rapports et logs locaux...
if exist "%~dp0Intelligence_Report.txt" del /f /q "%~dp0Intelligence_Report.txt"
if exist "%~dp0crash_log.txt" del /f /q "%~dp0crash_log.txt"
if exist "%~dp0arborescence_scripts.txt" del /f /q "%~dp0arborescence_scripts.txt"
if exist "%~dp0*.log" del /f /q "%~dp0*.log"

echo.
echo ========================================================
echo   Nettoyage termine avec succes !
echo ========================================================
pause
