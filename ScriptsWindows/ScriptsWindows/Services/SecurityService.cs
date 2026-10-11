using ScriptsWindows.Core;

namespace ScriptsWindows.Services;

/// <summary>Service sécurité — dump_*, wd_*, sys_av_* du .bat.</summary>
public class SecurityService
{
    // Credential Manager  (dump_credman)
    public static async Task DumpCredManager(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
Write-Output '--- CREDENTIAL MANAGER ---'
$creds = cmdkey /list
Write-Output $creds
", onOutput, ct);
    }

    // Mots de passe Wi-Fi  (dump_wifi)
    public static async Task DumpWifiPasswords(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
Write-Output '--- MOTS DE PASSE WI-FI ---'
$profiles = netsh wlan show profiles | Select-String 'Profil\s+utilisateur\s+:' | ForEach-Object {
    ($_ -split ':')[1].Trim()
}
foreach ($p in $profiles) {
    $details = netsh wlan show profile name=""$p"" key=clear
    $pass = ($details | Select-String 'Contenu de la clé\s+:') -replace '.*:\s*', ''
    if (-not $pass) { $pass = ($details | Select-String 'Key Content\s+:') -replace '.*:\s*', '' }
    Write-Output ""[SSID] $p — MDP: $($pass.Trim())""
}
", onOutput, ct);
    }

    // Windows Defender — Scan rapide  (wd_quick)
    public static async Task DefenderQuickScan(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Lancement du scan rapide Windows Defender...");
        await PowerShellRunner.RunAsync("Start-MpScan -ScanType QuickScan; Write-Output '[OK] Scan rapide terminé'", onOutput, ct);
    }

    // Windows Defender — Scan complet  (wd_full)
    public static async Task DefenderFullScan(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Lancement du scan complet — peut prendre 30-60 min...");
        await PowerShellRunner.RunAsync("Start-MpScan -ScanType FullScan; Write-Output '[OK] Scan complet terminé'", onOutput, ct);
    }

    // Mettre à jour Defender  (wd_update)
    public static async Task UpdateDefender(Action<string> onOutput, CancellationToken ct = default)
        => await PowerShellRunner.RunAsync("Update-MpSignature; Write-Output '[OK] Signatures Defender mises à jour'", onOutput, ct);

    // Voir les menaces  (wd_threats)
    public static async Task GetDefenderThreats(Action<string> onOutput, CancellationToken ct = default)
        => await PowerShellRunner.RunAsync("Get-MpThreatDetection | Format-List *", onOutput, ct);

    // Test EICAR  (av_launcher_eicar)
    public static async Task TestEicar(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
$path = ""$env:TEMP\eicar_test.txt""
$eicar = 'X5O!P%@AP[4\PZX54(P^)7CC)7}$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!$H+H*'
Set-Content -Path $path -Value $eicar -NoNewline
Write-Output ""[i] Fichier EICAR créé : $path""
Write-Output '[i] Votre antivirus devrait le détecter et le supprimer immédiatement'
", onOutput, ct);
    }
}
