using System.IO;
using ScriptsWindows.Core;

namespace ScriptsWindows.Services;

/// <summary>Service export — sys_win_key, sys_drivers, gather_browser_history, search_sensitive_docs.</summary>
public class ExportService
{
    // Clé Windows  (sys_win_key)
    public static async Task GetWindowsKey(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
$key = (Get-WmiObject -query 'select * from SoftwareLicensingService').OA3xOriginalProductKey
if ($key) {
    Write-Output ""[OK] Clé Windows : $key""
} else {
    # Méthode alternative via registre
    $regPath = 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\SoftwareProtectionPlatform'
    $key2 = (Get-ItemProperty -Path $regPath -Name BackupProductKeyDefault -ErrorAction SilentlyContinue).BackupProductKeyDefault
    if ($key2) { Write-Output ""[OK] Clé Windows (backup) : $key2"" }
    else { Write-Output '[!] Clé non récupérable (OEM ou numérique)' }
}
", onOutput, ct);
    }

    // Export pilotes  (sys_drivers)
    public static async Task ExportDrivers(Action<string> onOutput, CancellationToken ct = default)
    {
        var dest = @"C:\DriversBackup";
        try
        {
            if (!Directory.Exists(dest))
                Directory.CreateDirectory(dest);
        }
        catch
        {
            dest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "DriversBackup");
            if (!Directory.Exists(dest))
                Directory.CreateDirectory(dest);
        }

        onOutput($"[i] Dossier cible vérifié : {dest}");
        onOutput("[i] Sauvegarde et extraction de tous les pilotes système tiers via DISM...");
        await CommandRunner.RunAsync($"DISM /Online /Export-Driver /Destination:\"{dest}\"", onOutput, ct);
        onOutput($"[OK] Pilotes exportés avec succès dans {dest}");
    }

    // Liste logiciels  (sys_export_software)
    public static async Task ExportSoftwareList(Action<string> onOutput, CancellationToken ct = default)
    {
        var dest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "liste_logiciels.txt");
        onOutput("[i] Export de la liste des logiciels via winget...");
        await CommandRunner.RunAsync($"winget list > \"{dest}\"", onOutput, ct);
        onOutput($"[OK] Liste exportée : {dest}");
    }

    // Historique navigateurs  (gather_browser_history)
    public static async Task AnalyzeBrowserHistory(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
Write-Output '--- ANALYSE HISTORIQUE NAVIGATEURS ---'
$profile = $env:USERPROFILE

# Chrome
$chromeHist = ""$profile\AppData\Local\Google\Chrome\User Data\Default\History""
if (Test-Path $chromeHist) {
    Write-Output '[Chrome] Fichier historique trouvé'
    Write-Output ""  Taille : $([math]::Round((Get-Item $chromeHist).Length/1KB, 1)) KB""
    Write-Output ""  Path   : $chromeHist""
}

# Edge
$edgeHist = ""$profile\AppData\Local\Microsoft\Edge\User Data\Default\History""
if (Test-Path $edgeHist) {
    Write-Output '[Edge] Fichier historique trouvé'
    Write-Output ""  Taille : $([math]::Round((Get-Item $edgeHist).Length/1KB, 1)) KB""
}

Write-Output '[i] Historiques SQLite — utilisez DB Browser for SQLite pour les lire'
", onOutput, ct);
    }

    // Recherche documents sensibles  (search_sensitive_docs)
    public static async Task ScanSensitiveDocs(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Scan des documents sensibles (SSH, IBAN, mots de passe...)");
        await PowerShellRunner.RunAsync(@"
$patterns = @('password', 'mot de passe', 'IBAN', 'ssh', 'id_rsa', 'AWS', 'secret', 'token', 'credential')
$searchDirs = @($env:USERPROFILE + '\Documents', $env:USERPROFILE + '\Desktop', $env:USERPROFILE + '\Downloads')

foreach ($dir in $searchDirs) {
    if (-not (Test-Path $dir)) { continue }
    foreach ($pat in $patterns) {
        Get-ChildItem $dir -Recurse -File -ErrorAction SilentlyContinue |
            Where-Object { $_.Name -like ""*$pat*"" -or $_.Extension -in '.txt','.env','.ini','.cfg','.json','.xml','.key','.pem' } |
            Select-Object -First 5 |
            ForEach-Object {
                Write-Output ""[!] $($_.FullName) ($([math]::Round($_.Length/1KB,1)) KB)""
            }
    }
}
Write-Output '[OK] Scan terminé'
", onOutput, ct);
    }

    // Export Wi-Fi + Apps  (sys_export_wifi_apps)
    public static async Task ExportWifiAndApps(Action<string> onOutput, CancellationToken ct = default)
    {
        var dest = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        onOutput("[i] Export des profils Wi-Fi...");
        await CommandRunner.RunAsync($"netsh wlan export profile key=clear folder=\"{dest}\"", onOutput, ct);
        onOutput("[i] Export de la liste des apps...");
        await CommandRunner.RunAsync($"winget list > \"{dest}\\apps_list.txt\"", onOutput, ct);
        onOutput("[OK] Export terminé sur le Bureau");
    }
}
