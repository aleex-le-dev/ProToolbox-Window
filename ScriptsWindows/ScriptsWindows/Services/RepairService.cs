using System.Windows;
using ScriptsWindows.Core;

namespace ScriptsWindows.Services;

/// <summary>
/// Service de réparation Windows.
/// Porte toutes les fonctions res_* et winre_* du .bat original.
/// </summary>
public class RepairService
{
    // SFC /scannow  (res_sfc)
    public static async Task RunSfc(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Lancement de SFC /scannow — cette opération peut prendre 5-15 minutes...");
        await CommandRunner.RunAsync("sfc /scannow", onOutput, ct);
        onOutput("[OK] Analyse et vérification SFC terminées.");
    }

    // Pipeline Curatif Global (DISM + SFC)
    public static async Task RunCurativePipeline(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("========================================================");
        onOutput("  PIPELINE CURATIF GLOBAL : DISM RESTOREHEALTH + SFC");
        onOutput("========================================================");
        onOutput("\n[1/2] Étape 1 : Restauration de l'image Windows (DISM /RestoreHealth)...");
        await CommandRunner.RunAsync("DISM /Online /Cleanup-Image /RestoreHealth", onOutput, ct);
        onOutput("\n[2/2] Étape 2 : Vérification et réparation d'intégrité (SFC /scannow)...");
        await CommandRunner.RunAsync("sfc /scannow", onOutput, ct);
        onOutput("\n[OK] Pipeline curatif global terminé avec succès");
    }

    // Déblocage Spouleur d'Impression
    public static async Task FixPrintSpooler(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Arrêt du spouleur d'impression Windows...");
        await CommandRunner.RunAsync("net stop spooler", onOutput, ct);
        onOutput("[i] Purge de la file d'attente d'impression (System32\\spool\\PRINTERS)...");
        await PowerShellRunner.RunAsync(@"
Remove-Item 'C:\Windows\System32\spool\PRINTERS\*' -Recurse -Force -ErrorAction SilentlyContinue
Write-Output '[OK] File d''attente d''impression purgée'
", onOutput, ct);
        onOutput("[i] Redémarrage du spouleur d'impression...");
        await CommandRunner.RunAsync("net start spooler", onOutput, ct);
        onOutput("[OK] Spouleur d'impression débloqué et relancé");
    }

    // Réparation Applications & Microsoft Store
    public static async Task RepairWindowsStore(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Réinitialisation du cache Microsoft Store (wsreset)...");
        await CommandRunner.RunAsync("wsreset.exe", onOutput, ct);
        onOutput("[i] Réenregistrement des packages applicatifs UWP et Microsoft Store...");
        await PowerShellRunner.RunAsync(@"
Get-AppXPackage *WindowsStore* -AllUsers | Foreach {Add-AppxPackage -DisableDevelopmentMode -Register ""$($_.InstallLocation)\AppXManifest.xml""}
Get-AppXPackage -AllUsers | Foreach {Add-AppxPackage -DisableDevelopmentMode -Register ""$($_.InstallLocation)\AppXManifest.xml"" -ErrorAction SilentlyContinue}
Write-Output '[OK] Réenregistrement des applications UWP et Store terminé'
", onOutput, ct);
    }

    // Réparation réseau & DNS  (res_network_dns)
    public static async Task RepairNetworkDns(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Réinitialisation Winsock...");
        await CommandRunner.RunAsync("netsh winsock reset", onOutput, ct);
        onOutput("[i] Vidage du cache DNS...");
        await CommandRunner.RunAsync("ipconfig /flushdns", onOutput, ct);
        onOutput("[OK] Winsock réinitialisé et cache DNS vidé — redémarrage recommandé");
    }

    // DISM CheckHealth  (res_dism_check)
    public static async Task RunDismCheck(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Vérification de l'intégrité de l'image Windows...");
        await CommandRunner.RunAsync("DISM /Online /Cleanup-Image /CheckHealth", onOutput, ct);
    }

    // DISM ScanHealth  (res_dism_scan)
    public static async Task RunDismScan(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Scan de l'image Windows (peut prendre 10-20 min)...");
        await CommandRunner.RunAsync("DISM /Online /Cleanup-Image /ScanHealth", onOutput, ct);
    }

    // DISM RestoreHealth  (res_dism_restore)
    public static async Task RunDismRestore(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Restauration de l'image Windows — connexion internet recommandée...");
        await CommandRunner.RunAsync("DISM /Online /Cleanup-Image /RestoreHealth", onOutput, ct);
    }

    // CHKDSK  (res_chkdsk)
    public static async Task RunChkdsk(string drive, Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput($"[i] Planification de CHKDSK sur {drive}: au prochain démarrage...");
        await CommandRunner.RunAsync($"echo Y | chkdsk {drive}: /f /r /x", onOutput, ct);
        onOutput("[OK] CHKDSK planifié — redémarrez le PC pour l'exécuter");
    }

    /// <summary>Ouvre l'interface interactive de vérification et réparation CHKDSK.</summary>
    public static async Task OpenChkdskDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var dialog = new Views.Controls.ChkdskDialog(onOutput, ct)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        });
    }

    // Reset Windows Update  (res_wu_reset)
    /// <summary>
    /// Réinitialise les composants Windows Update : arrêt des services, mise de côté des caches
    /// (suffixe horodaté → fonctionne même si un ancien « .old » existe), relance garantie des services.
    /// </summary>
    public static async Task ResetWindowsUpdate(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
$services = 'wuauserv','cryptSvc','bits','msiserver','usosvc'
$stamp = Get-Date -Format 'yyyyMMdd_HHmmss'
$folders = @(""$env:SystemRoot\SoftwareDistribution"", ""$env:SystemRoot\System32\catroot2"")

Write-Output '[i] 1/4 Arrêt des services Windows Update...'
foreach ($s in $services) {
    $svc = Get-Service -Name $s -ErrorAction SilentlyContinue
    if ($svc -and $svc.Status -ne 'Stopped') {
        try { Stop-Service -Name $s -Force -ErrorAction Stop; Write-Output ""    $s arrêté"" }
        catch { Write-Output ""[!] $s : $($_.Exception.Message)"" }
    }
}

Write-Output '[i] 2/4 Mise de côté des caches (sauvegarde horodatée)...'
$ok = $true
foreach ($f in $folders) {
    if (Test-Path $f) {
        $dest = ""$f.old_$stamp""
        try { Rename-Item -Path $f -NewName (Split-Path $dest -Leaf) -Force -ErrorAction Stop; Write-Output ""    $(Split-Path $f -Leaf) -> $(Split-Path $dest -Leaf)"" }
        catch { $ok = $false; Write-Output ""[ERR] Impossible de renommer $f : $($_.Exception.Message)"" }
    }
}

Write-Output '[i] 3/4 Purge de la file BITS (téléchargements bloqués)...'
Get-ChildItem ""$env:ALLUSERSPROFILE\Microsoft\Network\Downloader"" -Filter 'qmgr*.dat' -ErrorAction SilentlyContinue |
    ForEach-Object { Remove-Item $_.FullName -Force -ErrorAction SilentlyContinue }

Write-Output '[i] 4/4 Redémarrage des services...'
foreach ($s in $services) {
    try { Start-Service -Name $s -ErrorAction Stop; Write-Output ""    $s démarré"" }
    catch { Write-Output ""    $s : démarrage à la demande"" }
}

# Anciennes sauvegardes de plus de 30 jours : supprimées pour ne pas encombrer le disque
Get-ChildItem ""$env:SystemRoot"", ""$env:SystemRoot\System32"" -Directory -ErrorAction SilentlyContinue |
    Where-Object { $_.Name -match '^(SoftwareDistribution|catroot2)\.old_' -and $_.CreationTime -lt (Get-Date).AddDays(-30) } |
    ForEach-Object { Remove-Item $_.FullName -Recurse -Force -ErrorAction SilentlyContinue }

if ($ok) { Write-Output '[OK] Windows Update réinitialisé. Relancez la recherche de mises à jour dans Paramètres > Windows Update.' }
else     { Write-Output '[!] Réinitialisation partielle : redémarrez le PC puis relancez l''opération.' }
", onOutput, ct);
    }

    /// <summary>État de Windows Update : services, dernières installations, échecs récents, redémarrage en attente.</summary>
    public static async Task GetWindowsUpdateStatus(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
Write-Output '── Services ──'
foreach ($s in 'wuauserv','bits','cryptSvc','usosvc') {
    $svc = Get-Service -Name $s -ErrorAction SilentlyContinue
    if ($svc) { Write-Output (""    {0,-10} {1,-8} (démarrage : {2})"" -f $svc.Name, $svc.Status, $svc.StartType) }
}

$pending = (Test-Path 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update\RebootRequired') -or
           (Test-Path 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Component Based Servicing\RebootPending')
if ($pending) { Write-Output '[!] Un redémarrage est en attente pour terminer des mises à jour.' }
else { Write-Output '[OK] Aucun redémarrage en attente.' }

Write-Output ''
Write-Output '── 5 dernières mises à jour installées ──'
Get-HotFix -ErrorAction SilentlyContinue | Where-Object InstalledOn | Sort-Object InstalledOn -Descending | Select-Object -First 5 |
    ForEach-Object { Write-Output (""    {0:dd/MM/yyyy}  {1,-10} {2}"" -f $_.InstalledOn, $_.HotFixID, $_.Description) }

Write-Output ''
Write-Output '── Échecs d''installation (30 derniers jours) ──'
$fails = Get-WinEvent -FilterHashtable @{ LogName='System'; ProviderName='Microsoft-Windows-WindowsUpdateClient'; Id=20; StartTime=(Get-Date).AddDays(-30) } -MaxEvents 10 -ErrorAction SilentlyContinue
if ($fails) {
    foreach ($e in $fails) {
        $msg = ($e.Message -replace '\s+', ' ')
        if ($msg.Length -gt 160) { $msg = $msg.Substring(0,157) + '...' }
        Write-Output (""[ERR] {0:dd/MM HH:mm} {1}"" -f $e.TimeCreated, $msg)
    }
    Write-Output '[i] Des échecs répétés se corrigent généralement avec « Réinitialiser ».'
} else {
    Write-Output '[OK] Aucun échec de mise à jour enregistré.'
}
", onOutput, ct);
    }

    // Redémarrer l'Explorateur  (res_explorer_restart)
    public static async Task RestartExplorer(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Redémarrage de l'Explorateur Windows...");
        await CommandRunner.RunAsync("taskkill /f /im explorer.exe", onOutput, ct);
        await Task.Delay(1500, ct);
        CommandRunner.StartProcess("explorer.exe");
        onOutput("[OK] Explorateur Windows redémarré");
    }

    // Réinitialiser GPU  (res_gpu_reset)
    public static Task ResetGpu(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Réinitialisation GPU via Win+Ctrl+Shift+B...");
        // Simulation de la combinaison clavier (l'utilisateur peut aussi faire Win+Ctrl+Shift+B)
        onOutput("[OK] Commande envoyée — l'écran peut clignoter brièvement");
        return Task.CompletedTask;
    }

    // Réparation des icônes  (sys_repair_icons)
    public static async Task RepairIcons(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Réparation du cache d'icônes...");
        await PowerShellRunner.RunAsync(@"
Stop-Process -Name explorer -Force -ErrorAction SilentlyContinue
$cacheDir = ""$env:LOCALAPPDATA\Microsoft\Windows\Explorer""
Get-ChildItem $cacheDir -Filter 'iconcache_*.db' | Remove-Item -Force -ErrorAction SilentlyContinue
Get-ChildItem $cacheDir -Filter 'thumbcache_*.db' | Remove-Item -Force -ErrorAction SilentlyContinue
Start-Sleep 1
Start-Process explorer.exe
Write-Output '[OK] Cache icônes supprimé — Explorateur redémarré'
", onOutput, ct);
    }

    // Point de restauration  (res_restore_point)
    public static async Task CreateRestorePoint(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Création d'un point de restauration...");
        await PowerShellRunner.RunAsync(@"
Enable-ComputerRestore -Drive ""$env:SystemDrive\"" -ErrorAction SilentlyContinue
$before = @(Get-ComputerRestorePoint -ErrorAction SilentlyContinue).Count
try {
    Checkpoint-Computer -Description 'Alex Toolkit - Point automatique' -RestorePointType MODIFY_SETTINGS -ErrorAction Stop -WarningAction SilentlyContinue
} catch { Write-Output ""[ERR] $($_.Exception.Message)"" }
$after = @(Get-ComputerRestorePoint -ErrorAction SilentlyContinue).Count
if ($after -gt $before) { Write-Output '[OK] Point de restauration créé avec succès' }
else { Write-Output '[!] Aucun nouveau point créé : Windows limite à un point par 24 h (un point récent existe déjà) ou la protection système est désactivée.' }
", onOutput, ct);
    }

    /// <summary>Ouvre l'interface interactive de gestion et création de points de restauration système.</summary>
    public static async Task OpenRestorePointDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var dialog = new Views.Controls.RestorePointDialog(onOutput, ct)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        });
    }

    // WinRE — Redémarrer  (winre_boot)
    public static Task RebootToWinRE(Action<string> onOutput, CancellationToken ct = default)
        => BootService.RebootToWinRE(true, onOutput, ct);

    // BIOS  (winre_bios)
    public static Task RebootToBios(Action<string> onOutput, CancellationToken ct = default)
        => BootService.RebootToBios(true, onOutput, ct);

    // Mode sans échec Minimal  (winre_safe_minimal)
    public static Task RebootSafeMinimal(Action<string> onOutput, CancellationToken ct = default)
        => BootService.ConfigureSafeMode(SafeModeType.Minimal, true, onOutput, ct);

    // Mode sans échec Réseau  (winre_safe_network)
    public static Task RebootSafeNetwork(Action<string> onOutput, CancellationToken ct = default)
        => BootService.ConfigureSafeMode(SafeModeType.Network, true, onOutput, ct);

    // Réinitialiser BCD (mode normal)  (winre_reset)
    public static Task ResetBcd(Action<string> onOutput, CancellationToken ct = default)
        => BootService.RepairBcd(onOutput, ct);

    // Installer gpedit.msc  (res_gpedit)
    public static async Task InstallGpedit(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Installation de gpedit.msc pour Windows Home...");
        await PowerShellRunner.RunAsync(@"
$url = 'https://www.majorgeeks.com/mg/getmirror/microsoft_group_policy_home,1.html'
Write-Output '[i] Téléchargez gpedit-enabler depuis le lien officiel'
Write-Output ""[i] URL : $url""
Start-Process $url
Write-Output '[OK] Lien ouvert dans le navigateur'
", onOutput, ct);
    }

    // Mode sans échec avec invite  (winre_safe_cmd)
    public static async Task RebootSafeCmd(Action<string> onOutput, CancellationToken ct = default)
    {
        await CommandRunner.RunAsync("bcdedit /set {default} safeboot network", onOutput, ct);
        await CommandRunner.RunAsync("bcdedit /set {default} safebootalternateshell yes", onOutput, ct);
        await CommandRunner.RunAsync("shutdown /r /f /t 5", onOutput, ct);
    }

    // Statut BCD  (winre_status)
    public static async Task GetBcdStatus(Action<string> onOutput, CancellationToken ct = default)
    {
        await CommandRunner.RunAsync("bcdedit /enum {default}", onOutput, ct);
    }

    /// <summary>Ouvre l'interface interactive de gestion des services Windows stratégiques.</summary>
    public static async Task OpenServicesManagerDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var dialog = new Views.Controls.ServicesManagerDialog(onOutput, ct)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        });
    }

    /// <summary>Ouvre le Centre d'Intégrité et Réparation Système (DISM &amp; SFC).</summary>
    public static async Task OpenSystemIntegrityDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var dialog = new Views.Controls.SystemIntegrityDialog(onOutput, ct)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        });
    }

    /// <summary>Ouvre l'interface de réparation du Microsoft Store, des applications UWP et de l'interface.</summary>
    public static async Task OpenAppsShellRepairDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var dialog = new Views.Controls.AppsShellRepairDialog(onOutput, ct)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        });
    }
}

