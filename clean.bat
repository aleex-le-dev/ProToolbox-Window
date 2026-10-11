@echo off
chcp 65001 >nul
echo ========================================================
echo   Nettoyage des fichiers temporaires et caches de build
echo ========================================================
echo.

echo Suppression des dossiers de compilation bin/ et obj/...
if exist "%~dp0ScriptsWindows\ScriptsWindows\bin" rmdir /s /q "%~dp0ScriptsWindows\ScriptsWindows\bin"
if exist "%~dp0ScriptsWindows\ScriptsWindows\obj" rmdir /s /q "%~dp0ScriptsWindows\ScriptsWindows\obj"

echo Suppression des dossiers de collecte temporaire...
if exist "%~dp0DiagNav" rmdir /s /q "%~dp0DiagNav"

echo Suppression des rapports et logs locaux...
if exist "%~dp0Intelligence_Report.txt" del /f /q "%~dp0Intelligence_Report.txt"
if exist "%~dp0*.log" del /f /q "%~dp0*.log"

echo.
echo [ATTENTION] Les fichiers suivants contiennent des données sensibles :
echo  - credentials.txt (mots de passe SMTP)
echo  - webhook_tokens.txt (tokens Discord)
echo Ils ne sont pas supprimés automatiquement mais sont protégés par le .gitignore.
echo.
echo ========================================================
echo   Nettoyage terminé avec succès !
echo ========================================================
pause
