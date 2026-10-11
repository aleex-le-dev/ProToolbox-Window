using ScriptsWindows.Core;

namespace ScriptsWindows.Services;

/// <summary>Service personnalisation — context_menu, god_mode, gaming_mode, photo_viewer.</summary>
public class PersonalizationService
{
    // Menu contextuel classique  (activate_classic)
    public static async Task SetClassicMenu(Action<string> onOutput, CancellationToken ct = default)
    {
        await CommandRunner.RunAsync("reg add \"HKCU\\Software\\Classes\\CLSID\\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\\InprocServer32\" /f /ve", onOutput, ct);
        onOutput("[OK] Menu contextuel classique activé — redémarrez l'Explorateur");
    }

    // Menu contextuel moderne  (restore_modern)
    public static async Task SetModernMenu(Action<string> onOutput, CancellationToken ct = default)
    {
        await CommandRunner.RunAsync("reg delete \"HKCU\\Software\\Classes\\CLSID\\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\" /f", onOutput, ct);
        onOutput("[OK] Menu contextuel moderne restauré — redémarrez l'Explorateur");
    }

    // God Mode  (sys_god_mode)
    public static Task CreateGodMode(Action<string> onOutput, CancellationToken ct = default)
    {
        var dest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                "GodMode.{ED7BA470-8E54-465E-825C-99712043E01C}");
        if (!Directory.Exists(dest))
            Directory.CreateDirectory(dest);
        onOutput("[OK] Dossier God Mode créé sur le Bureau");
        return Task.CompletedTask;
    }

    // Mode Gaming  (sys_gaming_mode)
    public static async Task ToggleGamingMode(bool enable, Action<string> onOutput, CancellationToken ct = default)
    {
        var val = enable ? "1" : "0";
        await CommandRunner.RunAsync($"reg add \"HKCU\\Software\\Microsoft\\GameBar\" /v AutoGameModeEnabled /t REG_DWORD /d {val} /f", onOutput, ct);
        onOutput($"[OK] Mode Gaming {(enable ? "activé" : "désactivé")}");
    }

    // Visionneuse de photos Windows  (photo_viewer_toggle)
    public static async Task EnablePhotoViewer(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
$cmd = '%SystemRoot%\System32\rundll32.exe ""%ProgramFiles%\Windows Photo Viewer\PhotoViewer.dll"", ImageView_Fullscreen %1'
reg add 'HKCR\Applications\photoviewer.dll\shell\open' /v 'MuiVerb' /t REG_SZ /d '@photoviewer.dll,-3043' /f | Out-Null
reg add 'HKCR\Applications\photoviewer.dll\shell\open\command' /ve /t REG_EXPAND_SZ /d $cmd /f | Out-Null
Write-Output '[OK] Visionneuse de photos Windows activée'
", onOutput, ct);
    }

    public static async Task DisablePhotoViewer(Action<string> onOutput, CancellationToken ct = default)
    {
        await CommandRunner.RunAsync("reg delete \"HKCR\\Applications\\photoviewer.dll\" /f", onOutput, ct);
        onOutput("[OK] Visionneuse de photos Windows désactivée");
    }

    // Raccourcis bureau  (sys_shortcuts_bureau)
    public static Task CreateShortcuts(Action<string> onOutput, CancellationToken ct = default)
    {
        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        // Raccourci Redémarrer
        CreateShortcutFile(Path.Combine(desktop, "Redémarrer.bat"),
            "@echo off\nshutdown /r /t 5 /c \"Redémarrage dans 5s...\"");

        // Raccourci Éteindre
        CreateShortcutFile(Path.Combine(desktop, "Éteindre.bat"),
            "@echo off\nshutdown /s /t 5 /c \"Extinction dans 5s...\"");

        onOutput("[OK] Raccourcis créés sur le Bureau");
        return Task.CompletedTask;
    }

    private static void CreateShortcutFile(string path, string content)
    {
        File.WriteAllText(path, content);
    }
}
