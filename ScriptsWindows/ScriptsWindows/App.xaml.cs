using System.Diagnostics;
using System.Security.Principal;
using System.Windows;

namespace ScriptsWindows;

public partial class App : Application
{
    public App()
    {
        DispatcherUnhandledException += (s, e) =>
        {
            LogCrash("DispatcherUnhandledException", e.Exception);
            MessageBox.Show(
                $"Une erreur est survenue :\n\n{e.Exception.Message}\n\nUn fichier crash_log.txt a été généré.",
                "Erreur Alex Toolkit",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            e.Handled = true;
        };

        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            if (e.ExceptionObject is Exception ex)
            {
                LogCrash("AppDomainUnhandledException", ex);
                MessageBox.Show(
                    $"Erreur critique :\n\n{ex.Message}\n\nConsultez crash_log.txt",
                    "Erreur critique",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        };
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

    protected override void OnStartup(StartupEventArgs e)
    {
        // Auto-élévation admin au démarrage si nécessaire
        if (!AdminElevation.IsAdmin())
        {
            AdminElevation.RelaunchAsAdmin();
            return;
        }

        base.OnStartup(e);
    }
}
