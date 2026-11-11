using System;
using System.Windows;

namespace ScriptsWindows;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        // Délai max par défaut de toutes les Regex de l'application : l'analyseur de sites web
        // applique des motifs à du HTML/JS externe arbitraire (protection contre le backtracking catastrophique).
        AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromSeconds(2));

        // ── Élévation admin avant tout affichage de fenêtre ──────────
        // (évite le "flash" de la fenêtre principale avant le redémarrage)
        if (!AdminElevation.IsAdmin())
        {
            AdminElevation.RelaunchAsAdmin();
            Shutdown();
            return;
        }

        // ── Gestionnaires d'exceptions non-gérées ────────────────────
        DispatcherUnhandledException += (s, ev) =>
        {
            LogCrash("DispatcherUnhandledException", ev.Exception);
            MessageBox.Show(
                $"Une erreur est survenue :\n\n{ev.Exception.Message}\n\nUn fichier crash_log.txt a été généré.",
                "Erreur Alex Toolkit",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            ev.Handled = true;
        };

        AppDomain.CurrentDomain.UnhandledException += (s, ev) =>
        {
            if (ev.ExceptionObject is Exception ex)
            {
                LogCrash("AppDomainUnhandledException", ex);
                MessageBox.Show(
                    $"Erreur critique :\n\n{ex.Message}\n\nConsultez crash_log.txt",
                    "Erreur critique",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        };

        base.OnStartup(e);
    }

    private static void LogCrash(string source, Exception ex)
    {
        try
        {
            var logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crash_log.txt");
            System.IO.File.AppendAllText(logPath, $"[{DateTime.Now}] [{source}]\n{ex}\n\n");
        }
        catch { }
    }
}
