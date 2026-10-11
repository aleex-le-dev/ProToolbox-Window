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
}
