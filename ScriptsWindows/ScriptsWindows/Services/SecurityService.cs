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

    // Générateur de mots de passe aléatoires sécurisés pour comptes Windows & Wi-Fi
    public static async Task GenerateSecurePasswords(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("═══════════════════════════════════════════════════════════════");
        onOutput("  🔑 GÉNÉRATEUR DE MOTS DE PASSE ROBUSTES WINDOWS");
        onOutput("═══════════════════════════════════════════════════════════════");
        onOutput("[i] Génération d'une série de mots de passe cryptographiquement forts...\n");

        var lengths = new[] { 16, 20, 24, 32 };
        var generated = new List<(string Label, string Password, int Length)>();

        for (int i = 0; i < lengths.Length; i++)
        {
            int len = lengths[i];
            string pwd = CreateRandomPassword(len);
            generated.Add(($"Profil Recommandé {i + 1} ({len} caractères)", pwd, len));
            onOutput($"[{i + 1}] Profil ({len} car.) : {pwd}");
        }

        for (int i = 0; i < 2; i++)
        {
            string ultraPwd = CreateRandomPassword(28);
            generated.Add(($"Super Administrateur / BitLocker {i + 1} (28 caractères)", ultraPwd, 28));
            onOutput($"[+] Haut Niveau (28 car.) : {ultraPwd}");
        }

        onOutput("\n═══════════════════════════════════════════════════════════════");

        var targetFilePath = FolderDialogHelper.AskSaveFilePath(
            title: "Enregistrer les mots de passe générés",
            defaultFileName: "Mots_de_passe_generes.txt",
            confirmQuestion: "Une série de mots de passe sécurisés vient d'être générée.\n\nSouhaitez-vous enregistrer ces mots de passe dans un fichier texte à l'emplacement de votre choix ?",
            confirmTitle: "Génération de mots de passe — Alex Toolkit"
        );

        if (!string.IsNullOrEmpty(targetFilePath))
        {
            var lines = new List<string>
            {
                "===========================================================",
                $" MOTS DE PASSE SÉCURISÉS GÉNÉRÉS — {Environment.MachineName}",
                $" Date de génération : {DateTime.Now:dd/MM/yyyy HH:mm:ss}",
                "===========================================================",
                "RECOMMANDATION : Conservez ce fichier dans un endroit sûr ou dans un coffre-fort.",
                ""
            };

            foreach (var item in generated)
            {
                lines.Add($"Label        : {item.Label}");
                lines.Add($"Mot de passe : {item.Password}");
                lines.Add($"Longueur     : {item.Length} caractères");
                lines.Add("-----------------------------------------------------------");
            }

            await System.IO.File.WriteAllLinesAsync(targetFilePath, lines, ct);
            onOutput($"\n✅ [SUCCÈS] Mots de passe enregistrés directement dans : {targetFilePath}");
            FolderDialogHelper.OpenInExplorer(targetFilePath);
        }
        else
        {
            onOutput("\n[i] Sauvegarde des mots de passe générés ignorée ou annulée par l'utilisateur.");
        }
    }

    private static string CreateRandomPassword(int length)
    {
        const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string lower = "abcdefghijkmnopqrstuvwxyz";
        const string digits = "23456789";
        const string specials = "!@#$%^&*()_+-=[]{}|";
        const string allChars = upper + lower + digits + specials;

        var bytes = new byte[length];
        System.Security.Cryptography.RandomNumberGenerator.Fill(bytes);

        var chars = new char[length];
        // Garantir au moins un caractère de chaque groupe
        chars[0] = upper[bytes[0] % upper.Length];
        chars[1] = lower[bytes[1] % lower.Length];
        chars[2] = digits[bytes[2] % digits.Length];
        chars[3] = specials[bytes[3] % specials.Length];

        for (int i = 4; i < length; i++)
        {
            chars[i] = allChars[bytes[i] % allChars.Length];
        }

        // Mélange (Fisher-Yates)
        var shuffleBytes = new byte[length];
        System.Security.Cryptography.RandomNumberGenerator.Fill(shuffleBytes);
        for (int i = length - 1; i > 0; i--)
        {
            int j = shuffleBytes[i] % (i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }

        return new string(chars);
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
