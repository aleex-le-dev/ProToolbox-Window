using System.Windows;
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

        // Extraction en parallèle ultra-rapide de l'ensemble des clés Wi-Fi
        var wifiResults = new (string Ssid, string? Password)[profiles.Count];
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = 10,
            CancellationToken = ct
        };

        await Parallel.ForEachAsync(Enumerable.Range(0, profiles.Count), parallelOptions, async (i, token) =>
        {
            var ssid = profiles[i];
            try
            {
                var details = await CommandRunner.RunAsync($"netsh wlan show profile name=\"{ssid}\" key=clear", token);
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

                wifiResults[i] = (ssid, password);
            }
            catch
            {
                wifiResults[i] = (ssid, null);
            }
        });

        // Affichage groupé et instantané dans la console (sans défilement lent un par un)
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"[OK] {profiles.Count} profil(s) Wi-Fi détecté(s) :\n");

        foreach (var (ssid, password) in wifiResults)
        {
            sb.AppendLine($"📶 [SSID] {ssid}");
            if (!string.IsNullOrEmpty(password))
            {
                sb.AppendLine($"   🔑 Mot de passe : {password}");
            }
            else
            {
                sb.AppendLine($"   🔓 Mot de passe : (Réseau ouvert ou clé entreprise)");
            }
            sb.AppendLine("--------------------------------------------------");
        }

        onOutput(sb.ToString().TrimEnd());

        // Boîte de dialogue pour proposer la sauvegarde directe du fichier texte sans sous-dossier
        if (wifiResults.Length > 0)
        {
            var targetFilePath = FolderDialogHelper.AskSaveFilePath(
                title: "Enregistrer les mots de passe Wi-Fi",
                defaultFileName: "Mots_de_passe_WiFi.txt",
                confirmQuestion: $"{wifiResults.Length} profil(s) Wi-Fi et clés ont été analysés.\n\nSouhaitez-vous enregistrer l'ensemble de ces mots de passe dans un fichier texte à l'emplacement de votre choix ?",
                confirmTitle: "Sauvegarde des mots de passe Wi-Fi — Alex Toolkit"
            );

            if (!string.IsNullOrEmpty(targetFilePath))
            {
                onOutput("\n═══════════════════════════════════════════════════════════════");
                onOutput("  📁 EXPORTATION DES MOTS DE PASSE WI-FI");
                onOutput("═══════════════════════════════════════════════════════════════");

                var reportLines = new List<string>
                {
                    "===========================================================",
                    $" INVENTAIRE DES MOTS DE PASSE WI-FI — {Environment.MachineName}",
                    $" Date d'export : {DateTime.Now:dd/MM/yyyy HH:mm:ss}",
                    "===========================================================",
                    ""
                };

                foreach (var item in wifiResults)
                {
                    reportLines.Add($"Réseau (SSID) : {item.Ssid}");
                    reportLines.Add($"Mot de passe  : {(string.IsNullOrEmpty(item.Password) ? "(Réseau ouvert ou clé entreprise)" : item.Password)}");
                    reportLines.Add("-----------------------------------------------------------");
                }

                await System.IO.File.WriteAllLinesAsync(targetFilePath, reportLines, ct);

                onOutput($"✅ [SUCCÈS] Mots de passe enregistrés directement dans : {targetFilePath}");
                FolderDialogHelper.OpenInExplorer(targetFilePath);
            }
            else
            {
                onOutput("\n[i] Sauvegarde des mots de passe Wi-Fi ignorée ou annulée par l'utilisateur.");
            }
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

    /// <summary>Ouvre l'interface interactive unifiée du Centre de Surveillance et Sécurité.</summary>
    public static async Task OpenSecurityCenterDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var dialog = new Views.Controls.SecurityCenterDialog(onOutput, ct)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        });
    }
}
