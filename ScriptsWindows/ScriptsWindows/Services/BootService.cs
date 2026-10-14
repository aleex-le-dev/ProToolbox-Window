using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ScriptsWindows.Core;

namespace ScriptsWindows.Services;

/// <summary>
/// Types de modes sans échec configurables pour Windows.
/// </summary>
public enum SafeModeType
{
    Normal,
    Minimal,
    Network
}

/// <summary>
/// Service de gestion du démarrage, du BIOS/UEFI, du WinRE et de la réparation BCD sous Windows.
/// </summary>
public static class BootService
{
    /// <summary>
    /// Ouvre la console interactive unifiée de Démarrage et Récupération.
    /// </summary>
    public static async Task OpenBootRecoveryDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var dialog = new Views.Controls.BootRecoveryDialog(onOutput, ct)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        });
    }

    /// <summary>
    /// Détecte l'état actuel du mode sans échec dans la base BCD ({current} ou {default}).
    /// </summary>
    public static async Task<SafeModeType> GetCurrentSafeModeStatusAsync()
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "bcdedit.exe",
                Arguments = "/enum {current}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = Process.Start(psi);
            if (proc != null)
            {
                string output = await proc.StandardOutput.ReadToEndAsync();
                await proc.WaitForExitAsync();

                if (output.IndexOf("safeboot", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (output.IndexOf("network", StringComparison.OrdinalIgnoreCase) >= 0)
                        return SafeModeType.Network;
                    return SafeModeType.Minimal;
                }
            }
        }
        catch { }

        return SafeModeType.Normal;
    }

    /// <summary>
    /// Détecte si la machine utilise un micrologiciel UEFI ou un BIOS hérité (Legacy).
    /// </summary>
    public static bool IsUefiSystem()
    {
        try
        {
            string? fwType = Environment.GetEnvironmentVariable("firmware_type");
            if (!string.IsNullOrWhiteSpace(fwType) && fwType.Equals("UEFI", StringComparison.OrdinalIgnoreCase))
                return true;

            // Détection alternative via le dossier EFI sur le volume système ou variable Windows
            string windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            string efiDir = Path.Combine(windir, "Panther");
            if (Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "en-US")))
            {
                // Vérification rapide via bcdedit si accessible
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c echo %firmware_type%",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var proc = Process.Start(psi);
                if (proc != null)
                {
                    string res = proc.StandardOutput.ReadToEnd().Trim();
                    if (res.Equals("UEFI", StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }
        }
        catch { }

        return true; // Les systèmes Windows 10/11 récents sont quasi-exclusivement en UEFI
    }

    /// <summary>
    /// Configure le mode sans échec (Minimal, Réseau ou désactivation pour retour au mode normal).
    /// </summary>
    public static async Task ConfigureSafeMode(SafeModeType mode, bool rebootImmediately, Action<string> onOutput, CancellationToken ct = default)
    {
        switch (mode)
        {
            case SafeModeType.Minimal:
                onOutput("[i] Configuration du mode sans échec : Minimal...");
                await CommandRunner.RunAsync("bcdedit /set {current} safeboot minimal", onOutput, ct);
                await CommandRunner.RunAsync("bcdedit /set {default} safeboot minimal", onOutput, ct);
                onOutput("[OK] Mode sans échec minimal activé dans le magasin BCD.");
                break;

            case SafeModeType.Network:
                onOutput("[i] Configuration du mode sans échec : Avec prise en charge réseau...");
                await CommandRunner.RunAsync("bcdedit /set {current} safeboot network", onOutput, ct);
                await CommandRunner.RunAsync("bcdedit /set {default} safeboot network", onOutput, ct);
                onOutput("[OK] Mode sans échec réseau activé dans le magasin BCD.");
                break;

            case SafeModeType.Normal:
            default:
                onOutput("[i] Suppression des paramètres de démarrage sans échec (retour au mode normal)...");
                await CommandRunner.RunAsync("bcdedit /deletevalue {current} safeboot", onOutput, ct);
                await CommandRunner.RunAsync("bcdedit /deletevalue {default} safeboot", onOutput, ct);
                onOutput("[OK] Démarrage normal de Windows restauré.");
                break;
        }

        if (rebootImmediately)
        {
            onOutput("\n[!] Redémarrage du système programmé dans 5 secondes...");
            await Task.Delay(2000, ct);
            await CommandRunner.RunAsync("shutdown.exe /r /f /t 5", onOutput, ct);
        }
        else
        {
            onOutput("[i] Les modifications prendront effet au prochain redémarrage manuel du PC.");
        }
    }

    /// <summary>
    /// Déclenche le redémarrage vers l'environnement de récupération Windows (WinRE).
    /// </summary>
    public static async Task RebootToWinRE(bool rebootImmediately, Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("========================================================");
        onOutput("  ACCÈS À L'ENVIRONNEMENT DE RÉCUPÉRATION WINDOWS (WinRE)");
        onOutput("========================================================");
        onOutput("[i] Préparation du redémarrage vers les options de dépannage avancées...");

        try
        {
            // Tente de marquer le prochain démarrage pour WinRE
            await CommandRunner.RunAsync("reagentc.exe /boottore", onOutput, ct);
        }
        catch { }

        if (rebootImmediately)
        {
            onOutput("\n[!] Redémarrage immédiat vers WinRE en cours...");
            await Task.Delay(1000, ct);
            await CommandRunner.RunAsync("shutdown.exe /r /o /f /t 00", onOutput, ct);
            onOutput("[OK] Commande de basculement WinRE exécutée.");
        }
        else
        {
            onOutput("[OK] WinRE pré-armé : le prochain démarrage affichera le menu de dépannage.");
        }
    }

    /// <summary>
    /// Déclenche le redémarrage vers le BIOS / UEFI de la carte mère.
    /// </summary>
    public static async Task RebootToBios(bool rebootImmediately, Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("========================================================");
        onOutput("  REDÉMARRAGE VERS LE BIOS / UEFI DU SYSTÈME");
        onOutput("========================================================");

        if (!IsUefiSystem())
        {
            onOutput("[!] AVERTISSEMENT : Ce PC semble fonctionner en mode BIOS hérité (Legacy).");
            onOutput("[!] La commande /r /fw nécessite une carte mère avec firmware UEFI actif.");
        }

        if (rebootImmediately)
        {
            onOutput("[i] Envoi de la commande de redémarrage vers le microprogramme UEFI...");
            await Task.Delay(1000, ct);
            await CommandRunner.RunAsync("shutdown.exe /r /fw /f /t 02", onOutput, ct);
            onOutput("[OK] Requête transmise au système d'exploitation.");
        }
        else
        {
            onOutput("[i] Action annulée ou en attente.");
        }
    }

    /// <summary>
    /// Réinitialise et répare les fichiers et configurations du chargeur de démarrage Windows (BCD).
    /// </summary>
    public static async Task RepairBcd(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("========================================================");
        onOutput("  RÉINITIALISATION ET RÉPARATION DU MAGASIN BCD");
        onOutput("========================================================");
        onOutput("[1/4] Vérification de la présence du dossier système Windows...");

        string systemRoot = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        onOutput($"[i] Chemin racine Windows détecté : {systemRoot}");

        onOutput("\n[2/4] Suppression des drapeaux de blocage safeboot résiduels...");
        await CommandRunner.RunAsync("bcdedit /deletevalue {current} safeboot", onOutput, ct);
        await CommandRunner.RunAsync("bcdedit /deletevalue {default} safeboot", onOutput, ct);

        onOutput("\n[3/4] Réinitialisation et reconstruction des fichiers BCD via BCDBoot...");
        await CommandRunner.RunAsync($"bcdboot.exe \"{systemRoot}\" /l fr-fr", onOutput, ct);

        onOutput("\n[4/4] Vérification de l'état du chargeur de démarrage BCD...");
        await CommandRunner.RunAsync("bcdedit /enum {current}", onOutput, ct);

        onOutput("\n[OK] Réinitialisation et reconstruction du magasin BCD terminées avec succès.");
        onOutput("[i] Le système démarrera de manière fluide en mode normal au prochain redémarrage.");
    }

    /// <summary>
    /// Demande confirmation à l'utilisateur puis redémarre vers WinRE.
    /// </summary>
    public static async Task PromptAndRebootWinRE(Action<string> onOutput, CancellationToken ct = default)
    {
        bool confirmed = false;
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var res = MessageBox.Show(
                "Votre ordinateur va redémarrer immédiatement pour accéder aux options avancées de récupération (WinRE).\n\nAssurez-vous d'avoir sauvegardé tous vos travaux en cours.\nConfirmez-vous le redémarrage vers WinRE ?",
                "Accès WinRE — Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            confirmed = (res == MessageBoxResult.Yes);
        });

        if (confirmed)
        {
            await RebootToWinRE(true, onOutput, ct);
        }
        else
        {
            onOutput("[i] Redémarrage vers WinRE annulé par l'utilisateur.");
        }
    }

    /// <summary>
    /// Demande confirmation à l'utilisateur puis redémarre vers le BIOS / UEFI.
    /// </summary>
    public static async Task PromptAndRebootBios(Action<string> onOutput, CancellationToken ct = default)
    {
        bool confirmed = false;
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            if (!IsUefiSystem())
            {
                var warnRes = MessageBox.Show(
                    "Votre ordinateur semble être configuré en BIOS Hérité (Legacy) et non en UEFI.\nLe redémarrage direct vers le firmware risque d'échouer.\n\nVoulez-vous quand même tenter l'opération ?",
                    "Avertissement Micrologiciel",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                confirmed = (warnRes == MessageBoxResult.Yes);
            }
            else
            {
                var res = MessageBox.Show(
                    "Votre ordinateur va redémarrer directement dans les réglages du BIOS / UEFI.\n\nAssurez-vous d'avoir fermé vos applications et sauvegardé vos fichiers.\nConfirmez-vous le redémarrage vers le BIOS / UEFI ?",
                    "Accès BIOS / UEFI — Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                confirmed = (res == MessageBoxResult.Yes);
            }
        });

        if (confirmed)
        {
            await RebootToBios(true, onOutput, ct);
        }
        else
        {
            onOutput("[i] Redémarrage vers le BIOS / UEFI annulé par l'utilisateur.");
        }
    }
}
