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
        onOutput("--- MOTS DE PASSE WI-FI ---");
        onOutput("[i] Recherche des profils Wi-Fi enregistrés...");

        var rawProfiles = await CommandRunner.RunAsync("netsh wlan show profiles", ct);
        var profiles = new List<string>();

        foreach (var line in rawProfiles.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var idx = line.IndexOf(':');
            if (idx >= 0)
            {
                var prefix = line.Substring(0, idx);
                if (prefix.Contains("Profil", StringComparison.OrdinalIgnoreCase) ||
                    prefix.Contains("Profile", StringComparison.OrdinalIgnoreCase))
                {
                    var name = line.Substring(idx + 1).Trim();
                    if (!string.IsNullOrWhiteSpace(name) &&
                        !name.Contains("<") &&
                        !name.Equals("lecture seule", StringComparison.OrdinalIgnoreCase))
                    {
                        profiles.Add(name);
                    }
                }
            }
        }

        if (profiles.Count == 0)
        {
            onOutput("[i] Aucun profil Wi-Fi enregistré trouvé sur cette machine.");
            return;
        }

        onOutput($"[OK] {profiles.Count} profil(s) Wi-Fi détecté(s) :\n");

        foreach (var ssid in profiles)
        {
            ct.ThrowIfCancellationRequested();

            var details = await CommandRunner.RunAsync($"netsh wlan show profile name=\"{ssid}\" key=clear", ct);
            string? password = null;

            foreach (var dLine in details.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (dLine.Contains("Contenu de la cl", StringComparison.OrdinalIgnoreCase) ||
                    dLine.Contains("Key Content", StringComparison.OrdinalIgnoreCase))
                {
                    var cIdx = dLine.IndexOf(':');
                    if (cIdx >= 0)
                    {
                        password = dLine.Substring(cIdx + 1).Trim();
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(password))
            {
                onOutput($"📶 [SSID] {ssid}");
                onOutput($"   🔑 Mot de passe : {password}");
            }
            else
            {
                onOutput($"📶 [SSID] {ssid}");
                onOutput($"   🔓 Mot de passe : (Réseau ouvert ou clé entreprise)");
            }
            onOutput("--------------------------------------------------");
        }
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
