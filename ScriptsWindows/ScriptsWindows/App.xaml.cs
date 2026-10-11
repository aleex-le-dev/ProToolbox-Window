using System.Diagnostics;
using System.Security.Principal;
using System.Windows;

namespace ScriptsWindows;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        // Auto-élévation admin au démarrage (équivalent du bloc fsutil du .bat)
        if (!AdminElevation.IsAdmin())
        {
            AdminElevation.RelaunchAsAdmin();
            return;
        }

        base.OnStartup(e);
    }
}
