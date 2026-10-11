using System.Diagnostics;
using System.Security.Principal;
using System.Windows;

namespace ScriptsWindows;

/// <summary>
/// Gestion de l'élévation des droits administrateur.
/// Équivalent du bloc "fsutil dirty query + powershell RunAs" du .bat original.
/// </summary>
public static class AdminElevation
{
    /// <summary>Vérifie si l'application tourne avec les droits admin.</summary>
    public static bool IsAdmin()
    {
        using var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    /// <summary>
    /// Relance le processus avec les droits admin (UAC prompt).
    /// Puis ferme l'instance actuelle sans droits.
    /// </summary>
    public static void RelaunchAsAdmin()
    {
        try
        {
            var exePath = Environment.ProcessPath
                          ?? System.IO.Path.Combine(AppContext.BaseDirectory, "ScriptsWindows.exe");

            var psi = new ProcessStartInfo(exePath)
            {
                UseShellExecute = true,
                Verb = "runas",
                Arguments = string.Join(" ", Environment.GetCommandLineArgs()[1..])
            };

            Process.Start(psi);
        }
        catch (System.ComponentModel.Win32Exception)
        {
            // L'utilisateur a refusé l'UAC — on affiche un message et on quitte
            MessageBox.Show(
                "Cette application nécessite les droits administrateur pour fonctionner.\n\nVeuillez relancer en tant qu'administrateur.",
                "Droits insuffisants",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
        finally
        {
            Application.Current?.Shutdown();
        }
    }
}
