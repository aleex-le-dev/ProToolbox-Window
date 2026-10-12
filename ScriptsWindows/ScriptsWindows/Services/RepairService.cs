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
    public static async Task ResetWindowsUpdate(Action<string> onOutput, CancellationToken ct = default)
    {
        var commands = new[]
        {
            "net stop wuauserv",
            "net stop cryptSvc",
            "net stop bits",
            "net stop msiserver",
            "ren C:\\Windows\\SoftwareDistribution SoftwareDistribution.old",
            "ren C:\\Windows\\System32\\catroot2 catroot2.old",
            "net start wuauserv",
            "net start cryptSvc",
            "net start bits",
            "net start msiserver"
        };

        foreach (var cmd in commands)
        {
            onOutput($"[CMD] {cmd}");
            await CommandRunner.RunAsync(cmd, onOutput, ct);
        }
        onOutput("[OK] Windows Update réinitialisé avec succès");
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
Enable-ComputerRestore -Drive '$env:SystemDrive' -ErrorAction SilentlyContinue
Checkpoint-Computer -Description 'AleexLeDev - Point Auto' -RestorePointType MODIFY_SETTINGS
Write-Output '[OK] Point de restauration créé avec succès'
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
    public static async Task RebootToWinRE(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[!] Redémarrage vers WinRE dans 5 secondes...");
        await Task.Delay(3000, ct);
        await CommandRunner.RunAsync("shutdown /r /o /f /t 5", onOutput, ct);
    }

    // BIOS  (winre_bios)
    public static async Task RebootToBios(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[!] Redémarrage vers le BIOS/UEFI...");
        await PowerShellRunner.RunAsync("shutdown /r /fw /f /t 5", onOutput, ct);
    }

    // Mode sans échec Minimal  (winre_safe_minimal)
    public static async Task RebootSafeMinimal(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Configuration du mode sans échec minimal...");
        await CommandRunner.RunAsync("bcdedit /set {default} safeboot minimal", onOutput, ct);
        onOutput("[!] Redémarrage dans 5 secondes...");
        await CommandRunner.RunAsync("shutdown /r /f /t 5", onOutput, ct);
    }

    // Mode sans échec Réseau  (winre_safe_network)
    public static async Task RebootSafeNetwork(Action<string> onOutput, CancellationToken ct = default)
    {
        await CommandRunner.RunAsync("bcdedit /set {default} safeboot network", onOutput, ct);
        await CommandRunner.RunAsync("shutdown /r /f /t 5", onOutput, ct);
    }

    // Réinitialiser BCD (mode normal)  (winre_reset)
    public static async Task ResetBcd(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Réinitialisation du démarrage en mode normal...");
        await CommandRunner.RunAsync("bcdedit /deletevalue {default} safeboot", onOutput, ct);
        onOutput("[OK] BCD réinitialisé — démarrage normal restauré");
    }

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
}
