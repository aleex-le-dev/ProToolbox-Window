using ScriptsWindows.Core;

namespace ScriptsWindows.Services;

/// <summary>Service applications — winget + installations (app_install_*, update_*).</summary>
public class AppService
{
    public static async Task ListUpdates(Action<string> onOutput, CancellationToken ct = default)
        => await CommandRunner.RunAsync("winget upgrade", onOutput, ct);

    public static async Task UpdateSingle(string appId, Action<string> onOutput, CancellationToken ct = default)
        => await CommandRunner.RunAsync($"winget upgrade --id {appId} --accept-package-agreements --accept-source-agreements", onOutput, ct);

    public static async Task UpdateAll(Action<string> onOutput, CancellationToken ct = default)
        => await CommandRunner.RunAsync("winget upgrade --all --accept-package-agreements --accept-source-agreements", onOutput, ct);

    public static async Task Install(string wingetId, string name, Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput($"[i] Installation de {name}...");
        await CommandRunner.RunAsync($"winget install --id {wingetId} --accept-package-agreements --accept-source-agreements -e", onOutput, ct);
        onOutput($"[OK] {name} installé");
    }

    public static Task InstallChrome(Action<string> onOutput, CancellationToken ct = default)
        => Install("Google.Chrome", "Google Chrome", onOutput, ct);

    public static Task InstallVlc(Action<string> onOutput, CancellationToken ct = default)
        => Install("VideoLAN.VLC", "VLC Media Player", onOutput, ct);

    public static Task InstallSumatraPdf(Action<string> onOutput, CancellationToken ct = default)
        => Install("SumatraPDF.SumatraPDF", "Sumatra PDF", onOutput, ct);

    public static Task Install7Zip(Action<string> onOutput, CancellationToken ct = default)
        => Install("7zip.7zip", "7-Zip", onOutput, ct);

    public static Task InstallPs7(Action<string> onOutput, CancellationToken ct = default)
        => Install("Microsoft.PowerShell", "PowerShell 7", onOutput, ct);

    public static async Task InstallOffice(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Ouverture du lien de téléchargement Microsoft Office...");
        CommandRunner.OpenUrl("https://drive.google.com/file/d/1wf1eqVkXIM0f0R2963Ee4RKK-a_rgWld/view?usp=drive_link");
        onOutput("[OK] Lien Google Drive ouvert dans le navigateur");
        await Task.CompletedTask;
    }

    public static async Task GetInstalledApps(Action<string> onOutput, CancellationToken ct = default)
        => await CommandRunner.RunAsync("winget list", onOutput, ct);
}
