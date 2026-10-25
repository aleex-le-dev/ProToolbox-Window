using System.IO;
using System.Windows;
using ScriptsWindows.Core;

namespace ScriptsWindows.Services;

/// <summary>Service de nettoyage — porte cl_*, sys_clean_unified, res_clean_browsers.</summary>
public class CleaningService
{
    // Vider les temporaires  (cl_temp)
    public static async Task CleanTemp(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
$dirs = @($env:TEMP, $env:TMP, 'C:\Windows\Temp')
foreach ($d in $dirs) {
    if (Test-Path $d) {
        $count = (Get-ChildItem $d -Recurse -ErrorAction SilentlyContinue | Measure-Object).Count
        Remove-Item ""$d\*"" -Recurse -Force -ErrorAction SilentlyContinue
        Write-Output ""[OK] Nettoyé : $d ($count fichiers supprimés)""
    }
}
", onOutput, ct);
    }

    // Purger Windows Update  (cl_wu)
    public static async Task CleanWindowsUpdate(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Arrêt du service Windows Update...");
        await CommandRunner.RunAsync("net stop wuauserv /y", onOutput, ct);
        await PowerShellRunner.RunAsync(@"
$dir = 'C:\Windows\SoftwareDistribution\Download'
if (Test-Path $dir) {
    $size = (Get-ChildItem $dir -Recurse -ErrorAction SilentlyContinue | Measure-Object -Property Length -Sum).Sum
    $sizeMB = [math]::Round($size / 1MB, 1)
    Remove-Item ""$dir\*"" -Recurse -Force -ErrorAction SilentlyContinue
    Write-Output ""[OK] Cache Windows Update purgé ($sizeMB MB libérés)""
}
", onOutput, ct);
        await CommandRunner.RunAsync("net start wuauserv", onOutput, ct);
    }

    // Vider DNS  (cl_dns)
    public static async Task FlushDns(Action<string> onOutput, CancellationToken ct = default)
    {
        await CommandRunner.RunAsync("ipconfig /flushdns", onOutput, ct);
        onOutput("[OK] Cache DNS vidé");
    }

    // Nettoyage disque  (cl_disk)
    public static Task CleanDisk(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Lancement de cleanmgr...");
        CommandRunner.StartProcess("cleanmgr.exe", "/sagerun:1");
        onOutput("[OK] Nettoyage disque lancé");
        return Task.CompletedTask;
    }

    // Nettoyer registre orphelin  (cl_registry)
    public static async Task CleanRegistry(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
Write-Output '[i] Recherche de clés orphelines dans le registre...'

# Nettoyage des programmes désinstallés dans CurrentVersion\Uninstall
$key = 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\*'
$apps = Get-ItemProperty $key -ErrorAction SilentlyContinue
$removed = 0
foreach ($app in $apps) {
    if ($app.InstallLocation -and -not (Test-Path $app.InstallLocation)) {
        Write-Output ""[!] Orphelin trouvé : $($app.DisplayName) — $($app.InstallLocation)""
        $removed++
    }
}
Write-Output ""[OK] $removed entrées orphelines trouvées""
Write-Output '[i] Pour un nettoyage complet, utilisez un outil dédié (CCleaner, etc.)'
", onOutput, ct);
    }

    // Vider presse-papiers  (cl_clipboard)
    public static async Task ClearClipboard(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync("Set-Clipboard -Value $null; Write-Output '[OK] Presse-papiers vidé'", onOutput, ct);
    }

    // Nettoyer caches navigateurs  (res_clean_browsers)
    public static async Task CleanBrowsers(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
$profile = $env:USERPROFILE
$browsers = @{
    'Chrome'  = ""$profile\AppData\Local\Google\Chrome\User Data\Default\Cache""
    'Edge'    = ""$profile\AppData\Local\Microsoft\Edge\User Data\Default\Cache""
    'Firefox' = ""$profile\AppData\Local\Mozilla\Firefox\Profiles""
    'Brave'   = ""$profile\AppData\Local\BraveSoftware\Brave-Browser\User Data\Default\Cache""
}
foreach ($b in $browsers.GetEnumerator()) {
    if (Test-Path $b.Value) {
        $size = (Get-ChildItem $b.Value -Recurse -ErrorAction SilentlyContinue | Measure-Object -Property Length -Sum).Sum
        $sizeMB = [math]::Round($size / 1MB, 1)
        Remove-Item ""$($b.Value)\*"" -Recurse -Force -ErrorAction SilentlyContinue
        Write-Output ""[OK] $($b.Key) : $sizeMB MB supprimés""
    } else {
        Write-Output ""[i] $($b.Key) : non installé""
    }
}

# Miniatures Windows
$thumbDir = ""$profile\AppData\Local\Microsoft\Windows\Explorer""
Get-ChildItem $thumbDir -Filter 'thumbcache_*.db' -ErrorAction SilentlyContinue | Remove-Item -Force -ErrorAction SilentlyContinue
Write-Output '[OK] Cache miniatures Windows supprimé'
", onOutput, ct);
    }

    // Nettoyage complet unifié  (cl_all / sys_clean_unified)
    public static async Task CleanAll(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] === NETTOYAGE COMPLET UNIFIÉ ===");
        onOutput("");
        onOutput("[1/5] Nettoyage des temporaires...");
        await CleanTemp(onOutput, ct);
        onOutput("[2/5] Purge Windows Update...");
        await CleanWindowsUpdate(onOutput, ct);
        onOutput("[3/5] Vidage cache DNS...");
        await FlushDns(onOutput, ct);
        onOutput("[4/5] Nettoyage caches navigateurs...");
        await CleanBrowsers(onOutput, ct);
        onOutput("[5/5] Vidage presse-papiers...");
        await ClearClipboard(onOutput, ct);
        onOutput("");
        onOutput("[OK] === NETTOYAGE COMPLET TERMINÉ ===");
    }

    // Nettoyage composants WinSxS (cl_winsxs)
    public static async Task CleanWinSxS(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Nettoyage approfondi du magasin de composants WinSxS (DISM)...");
        await CommandRunner.RunAsync("DISM /Online /Cleanup-Image /StartComponentCleanup /ResetBase", onOutput, ct);
        onOutput("[OK] Nettoyage WinSxS terminé");
    }

    /// <summary>Ouvre l'interface interactive de nettoyage système et disque unifié.</summary>
    public static async Task OpenSystemCleanupDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var dialog = new Views.Controls.SystemCleanupDialog(onOutput, ct)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        });
    }

    /// <summary>
    /// Génère un script PowerShell professionnel et autonome (avec lanceur batch auto-élévateur UAC sur le Bureau)
    /// qui affiche en temps réel les explications, les étapes en cours, le calcul des gains d'espace disque et un bilan récapitulatif complet.
    /// </summary>
    public static string CreateDesktopCleanupScript(Action<string>? onOutput = null)
    {
        // 1. Emplacement du script PowerShell dans %LOCALAPPDATA%\ScriptsWindows
        var appDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ScriptsWindows");
        if (!Directory.Exists(appDataDir))
        {
            Directory.CreateDirectory(appDataDir);
        }
        var psScriptPath = Path.Combine(appDataDir, "Nettoyage_Systeme_Et_Disque.ps1");

        // 2. Contenu PowerShell pur, riche, aéré et détaillé
        var psContent = @"# Auto-élévation administrateur native et sécurisée (anti-boucle)
$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    try {
        Start-Process powershell.exe -ArgumentList ""-NoProfile -ExecutionPolicy Bypass -File `""$PSCommandPath`"""" -Verb RunAs
    } catch { }
    exit
}

# ==============================================================================
# Script de Maintenance et Nettoyage Avancé — Alex Toolkit
# ==============================================================================
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$OutputEncoding = [System.Text.Encoding]::UTF8
$host.UI.RawUI.WindowTitle = ""Nettoyage Système et Disque — Alex Toolkit""
Clear-Host

$chrono = [System.Diagnostics.Stopwatch]::StartNew()

function Get-FolderSize([string]$path) {
    if (-not (Test-Path -LiteralPath $path)) { return 0 }
    try {
        $measure = Get-ChildItem -LiteralPath $path -Recurse -Force -File -ErrorAction SilentlyContinue |
                   Measure-Object -Property Length -Sum
        if ($measure -and $null -ne $measure.Sum) {
            return [long]$measure.Sum
        }
    } catch { }
    return 0
}

function Format-Size([long]$bytes) {
    if ($bytes -le 0) { return ""0 Mo"" }
    $mb = $bytes / 1MB
    if ($mb -ge 1024) {
        return (""{0:N2} Go"" -f ($mb / 1024.0))
    }
    return (""{0:N1} Mo"" -f $mb)
}

function Write-StepHeader([string]$step, [string]$title, [string]$description) {
    Write-Host """"
    Write-Host """"
    Write-Host ""━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"" -ForegroundColor DarkGray
    Write-Host "" [$step] $title"" -ForegroundColor Cyan
    Write-Host ""━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"" -ForegroundColor DarkGray
    Write-Host """"
    Write-Host ""   ℹ️  $description"" -ForegroundColor Gray
    Write-Host """"
    Write-Host ""   ⏳ Analyse et nettoyage en cours..."" -ForegroundColor Yellow
    Write-Host """"
}

function Write-SubResult([string]$label, [string]$value, [string]$color = ""Green"") {
    Write-Host ""   ▪ "" -ForegroundColor DarkGray -NoNewline
    Write-Host ""$label : "" -ForegroundColor Gray -NoNewline
    Write-Host ""$value"" -ForegroundColor $color
}

# ------------------------------------------------------------------------------
# Bannière de démarrage
# ------------------------------------------------------------------------------
Write-Host ""╔══════════════════════════════════════════════════════════════════════════════╗"" -ForegroundColor Cyan
Write-Host ""║              🧹 ALEX TOOLKIT — NETTOYAGE SYSTÈME & DISQUE                   ║"" -ForegroundColor Cyan
Write-Host ""║                 Maintenance, Purge & Récupération d'Espace                  ║"" -ForegroundColor Cyan
Write-Host ""╚══════════════════════════════════════════════════════════════════════════════╝"" -ForegroundColor Cyan
Write-Host "" 💻 PC : $env:COMPUTERNAME  |  👤 Session : $env:USERNAME  |  🛡️ Mode : Administrateur"" -ForegroundColor DarkGray
Write-Host "" 📅 Date : $(Get-Date -Format 'dd/MM/yyyy HH:mm:ss')"" -ForegroundColor DarkGray

$freedTemp = 0
$freedWu = 0
$freedRecycle = 0
$freedBrowsers = 0

# ------------------------------------------------------------------------------
# ÉTAPE 1/5 : Fichiers temporaires
# ------------------------------------------------------------------------------
Write-StepHeader ""1/5"" ""FICHIERS TEMPORAIRES WINDOWS & UTILISATEUR"" ""Supprime les fichiers temporaires et résidus d'installations laissés dans %TEMP% et Windows\Temp.""

$tempPaths = @($env:TEMP, $env:TMP, 'C:\Windows\Temp') | Select-Object -Unique

foreach ($p in $tempPaths) {
    if (Test-Path -LiteralPath $p) {
        $before = Get-FolderSize $p
        $items = Get-ChildItem -LiteralPath $p -Force -ErrorAction SilentlyContinue
        foreach ($it in $items) {
            try {
                Remove-Item -LiteralPath $it.FullName -Recurse -Force -ErrorAction SilentlyContinue
            } catch { }
        }
        $after = Get-FolderSize $p
        $diff = [math]::Max(0, ($before - $after))
        $freedTemp += $diff
        Write-SubResult ""$p"" (""{0} libérés"" -f (Format-Size $diff))
    }
}
Write-Host """"
Write-Host ""   ✅ [TERMINÉ] Fichiers temporaires purgés (Total : $(Format-Size $freedTemp) libérés)"" -ForegroundColor Green

# ------------------------------------------------------------------------------
# ÉTAPE 2/5 : Cache Windows Update
# ------------------------------------------------------------------------------
Write-StepHeader ""2/5"" ""CACHE DES TÉLÉCHARGEMENTS WINDOWS UPDATE"" ""Arrête le service Windows Update pour vider les paquets déjà installés dans SoftwareDistribution\Download.""

Stop-Service -Name wuauserv -Force -ErrorAction SilentlyContinue
$wuDir = 'C:\Windows\SoftwareDistribution\Download'
if (Test-Path -LiteralPath $wuDir) {
    $wuBefore = Get-FolderSize $wuDir
    $wuItems = Get-ChildItem -LiteralPath $wuDir -Force -ErrorAction SilentlyContinue
    foreach ($w in $wuItems) {
        try { Remove-Item -LiteralPath $w.FullName -Recurse -Force -ErrorAction SilentlyContinue } catch { }
    }
    $wuAfter = Get-FolderSize $wuDir
    $freedWu = [math]::Max(0, ($wuBefore - $wuAfter))
    Write-SubResult ""Dossier SoftwareDistribution\Download"" (""{0} libérés"" -f (Format-Size $freedWu))
}
Start-Service -Name wuauserv -ErrorAction SilentlyContinue
Write-Host """"
Write-SubResult ""Service Windows Update (wuauserv)"" ""Redémarré avec succès"" ""White""
Write-Host """"
Write-Host ""   ✅ [TERMINÉ] Cache Windows Update purgé (Total : $(Format-Size $freedWu) libérés)"" -ForegroundColor Green

# ------------------------------------------------------------------------------
# ÉTAPE 3/5 : Vidage de la Corbeille
# ------------------------------------------------------------------------------
Write-StepHeader ""3/5"" ""VIDAGE DE LA CORBEILLE (TOUS LES LECTEURS)"" ""Supprime définitivement les fichiers dans la Corbeille de tous les disques pour récupérer l'espace.""

try {
    $drives = [System.IO.DriveInfo]::GetDrives() | Where-Object { $_.DriveType -eq 'Fixed' -and $_.IsReady }
    foreach ($d in $drives) {
        $rbPath = Join-Path $d.RootDirectory.FullName '$Recycle.Bin'
        if (Test-Path -LiteralPath $rbPath) {
            $freedRecycle += (Get-FolderSize $rbPath)
        }
    }
    Clear-RecycleBin -Force -ErrorAction SilentlyContinue
    Write-SubResult ""Corbeille Windows"" (""{0} vidés"" -f (Format-Size $freedRecycle))
} catch {
    Write-SubResult ""Corbeille Windows"" ""Déjà vide ou accès restreint"" ""Gray""
}
Write-Host """"
Write-Host ""   ✅ [TERMINÉ] Corbeille vidée avec succès"" -ForegroundColor Green

# ------------------------------------------------------------------------------
# ÉTAPE 4/5 : Navigateurs & Miniatures
# ------------------------------------------------------------------------------
Write-StepHeader ""4/5"" ""CACHES NAVIGATEURS WEB & MINIATURES WINDOWS"" ""Allège les caches de Chrome, Edge, Firefox, Brave et les miniatures sans toucher à vos identifiants.""

$userProf = $env:USERPROFILE
$browsers = [ordered]@{
    ""Google Chrome""   = ""$userProf\AppData\Local\Google\Chrome\User Data\Default\Cache""
    ""Microsoft Edge""  = ""$userProf\AppData\Local\Microsoft\Edge\User Data\Default\Cache""
    ""Mozilla Firefox"" = ""$userProf\AppData\Local\Mozilla\Firefox\Profiles""
    ""Brave Browser""   = ""$userProf\AppData\Local\BraveSoftware\Brave-Browser\User Data\Default\Cache""
}

foreach ($bName in $browsers.Keys) {
    $bPath = $browsers[$bName]
    if (Test-Path -LiteralPath $bPath) {
        $bBefore = Get-FolderSize $bPath
        $bItems = Get-ChildItem -LiteralPath $bPath -Force -ErrorAction SilentlyContinue
        foreach ($bi in $bItems) {
            try { Remove-Item -LiteralPath $bi.FullName -Recurse -Force -ErrorAction SilentlyContinue } catch { }
        }
        $bAfter = Get-FolderSize $bPath
        $diff = [math]::Max(0, ($bBefore - $bAfter))
        $freedBrowsers += $diff
        Write-SubResult $bName (""{0} libérés"" -f (Format-Size $diff))
    } else {
        Write-SubResult $bName ""Non installé ou dossier vide"" ""DarkGray""
    }
}

$thumbDir = ""$userProf\AppData\Local\Microsoft\Windows\Explorer""
if (Test-Path -LiteralPath $thumbDir) {
    $tBefore = Get-FolderSize $thumbDir
    Get-ChildItem -LiteralPath $thumbDir -Filter 'thumbcache_*.db' -Force -ErrorAction SilentlyContinue |
        ForEach-Object { try { Remove-Item -LiteralPath $_.FullName -Force -ErrorAction SilentlyContinue } catch { } }
    $tAfter = Get-FolderSize $thumbDir
    $tDiff = [math]::Max(0, ($tBefore - $tAfter))
    $freedBrowsers += $tDiff
    Write-SubResult ""Miniatures Windows (thumbcache)"" (""{0} libérés"" -f (Format-Size $tDiff))
}
Write-Host """"
Write-Host ""   ✅ [TERMINÉ] Caches navigateurs et miniatures purgés (Total : $(Format-Size $freedBrowsers) libérés)"" -ForegroundColor Green

# ------------------------------------------------------------------------------
# ÉTAPE 5/5 : DNS & Presse-papiers
# ------------------------------------------------------------------------------
Write-StepHeader ""5/5"" ""OPTIMISATION RÉSEAU (DNS) & PRESSE-PAPIERS"" ""Vide le cache de résolution DNS (ipconfig /flushdns) et réinitialise la mémoire vive du presse-papiers.""

ipconfig /flushdns | Out-Null
Write-SubResult ""Cache DNS (ipconfig /flushdns)"" ""Mémoire tampon réinitialisée""
Set-Clipboard -Value $null -ErrorAction SilentlyContinue
Write-SubResult ""Presse-papiers Windows"" ""Mémoire vive libérée""
Write-Host """"
Write-Host ""   ✅ [TERMINÉ] Optimisation réseau et mémoire effectuée"" -ForegroundColor Green

# ------------------------------------------------------------------------------
# BILAN RÉCAPITULATIF FINAL
# ------------------------------------------------------------------------------
$chrono.Stop()
$elapsedSec = [math]::Round($chrono.Elapsed.TotalSeconds, 1)
$totalFreedBytes = $freedTemp + $freedWu + $freedRecycle + $freedBrowsers

Write-Host """"
Write-Host """"
Write-Host ""╔══════════════════════════════════════════════════════════════════════════════╗"" -ForegroundColor Green
Write-Host ""║                        📊 BILAN DU NETTOYAGE EFFECTUÉ                        ║"" -ForegroundColor Green
Write-Host ""╠══════════════════════════════════════════════════════════════════════════════╣"" -ForegroundColor Green
Write-Host (""║  📁 Fichiers temporaires (%TEMP% & Windows)   │ {0,-28} ║"" -f (Format-Size $freedTemp)) -ForegroundColor Green
Write-Host (""║  📦 Cache des mises à jour Windows Update     │ {0,-28} ║"" -f (Format-Size $freedWu)) -ForegroundColor Green
Write-Host (""║  🗑️  Corbeille Windows (Tous les disques)     │ {0,-28} ║"" -f (Format-Size $freedRecycle)) -ForegroundColor Green
Write-Host (""║  🌐 Caches navigateurs & Miniatures           │ {0,-28} ║"" -f (Format-Size $freedBrowsers)) -ForegroundColor Green
Write-Host (""║  🔌 Cache DNS & Presse-papiers                │ {0,-28} ║"" -f ""Purge effectuée (OK)"") -ForegroundColor Green
Write-Host ""╠══════════════════════════════════════════════════════════════════════════════╣"" -ForegroundColor Green
Write-Host (""║  🎉 TOTAL ESPACE DISQUE RÉCUPÉRÉ              │ {0,-28} ║"" -f (Format-Size $totalFreedBytes)) -ForegroundColor Yellow
Write-Host (""║  ⏱️  Durée du nettoyage                        │ {0,-28} ║"" -f (""{0} secondes"" -f $elapsedSec)) -ForegroundColor White
Write-Host ""╚══════════════════════════════════════════════════════════════════════════════╝"" -ForegroundColor Green
Write-Host """"
Write-Host ""  ✨ Nettoyage système et disque terminé avec succès !"" -ForegroundColor Cyan
Write-Host """"
Write-Host ""━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"" -ForegroundColor DarkGray
Write-Host "" Appuyez sur [Entrée] pour quitter cette fenêtre..."" -ForegroundColor Yellow
Write-Host ""━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"" -ForegroundColor DarkGray
Write-Host """"
$null = Read-Host
exit
";

        // Écriture du fichier PowerShell
        File.WriteAllText(psScriptPath, psContent, System.Text.Encoding.UTF8);

        // 3. Lanceur batch sans BOM sur le Bureau
        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        if (!Directory.Exists(desktop))
        {
            Directory.CreateDirectory(desktop);
        }
        var batPath = Path.Combine(desktop, "Nettoyage_Systeme_Et_Disque.bat");

        var batContent = @"@echo off
setlocal
title Nettoyage Systeme et Disque - Alex Toolkit

set ""PS_SCRIPT=%LOCALAPPDATA%\ScriptsWindows\Nettoyage_Systeme_Et_Disque.ps1""
if not exist ""%PS_SCRIPT%"" (
    echo.
    echo [ERREUR] Le script PowerShell est introuvable :
    echo %PS_SCRIPT%
    echo.
    pause
    exit /b 1
)

:: Lance PowerShell en mode Administrateur et ferme immediatement ce lanceur .bat
start """" powershell.exe -NoProfile -ExecutionPolicy Bypass -WindowStyle Hidden -Command ""Start-Process powershell.exe -ArgumentList '-NoProfile -ExecutionPolicy Bypass -File \""""%PS_SCRIPT%\""""' -Verb RunAs""
exit
";

        // Encodage UTF-8 SANS BOM impératif pour éviter toute erreur de syntaxe CMD
        File.WriteAllText(batPath, batContent, new System.Text.UTF8Encoding(false));
        onOutput?.Invoke($"[OK] Script de nettoyage créé sur le Bureau : {batPath}");
        return batPath;
    }
}

