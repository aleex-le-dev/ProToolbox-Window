using System.IO;
using System.Windows;
using Microsoft.Win32;
using ScriptsWindows.Core;

namespace ScriptsWindows.Services;

/// <summary>Service personnalisation — context_menu, god_mode, gaming_mode, photo_viewer.</summary>
public class PersonalizationService
{
    // Menu contextuel classique  (activate_classic)
    public static async Task SetClassicMenu(Action<string> onOutput, CancellationToken ct = default)
    {
        await CommandRunner.RunAsync("reg add \"HKCU\\Software\\Classes\\CLSID\\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\\InprocServer32\" /f /ve", onOutput, ct);
        onOutput("[OK] Menu contextuel classique activé");
    }

    // Menu contextuel moderne  (restore_modern)
    public static async Task SetModernMenu(Action<string> onOutput, CancellationToken ct = default)
    {
        await CommandRunner.RunAsync("reg delete \"HKCU\\Software\\Classes\\CLSID\\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\" /f", onOutput, ct);
        onOutput("[OK] Menu contextuel moderne restauré");
    }

    // Redémarrage de l'Explorateur Windows
    public static async Task RestartExplorer(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Redémarrage de l'Explorateur Windows...");
        await CommandRunner.RunAsync("taskkill /f /im explorer.exe & start explorer.exe", onOutput, ct);
        onOutput("[OK] Explorateur Windows redémarré");
    }

    /// <summary>Ouvre la boîte de dialogue de configuration du menu contextuel (Windows 10 / Windows 11).</summary>
    public static async Task OpenContextMenuDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var dialog = new Views.Controls.ContextMenuDialog(onOutput, ct)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        });
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

    // ─── Raccourcis Bureau : Arrêter et Redémarrer avec icônes officielles ───
    public const string ShutdownShortcutName = "Arrêter le PC.lnk";
    public const string RestartShortcutName = "Redémarrer le PC.lnk";

    /// <summary>
    /// Ouvre l'interface moderne de création des boutons d'arrêt et de redémarrage sur le Bureau.
    /// </summary>
    public static async Task OpenPowerShortcutsDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var dialog = new Views.Controls.PowerShortcutsDialog(onOutput, ct)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        });
    }

    /// <summary>
    /// Vérifie si un raccourci est présent sur le Bureau.
    /// </summary>
    public static bool IsShortcutPresent(string shortcutName)
    {
        try
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            return File.Exists(Path.Combine(desktop, shortcutName));
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Supprime un raccourci du Bureau.
    /// </summary>
    public static bool RemoveShortcut(string shortcutName)
    {
        try
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var file = Path.Combine(desktop, shortcutName);
            if (File.Exists(file))
            {
                File.Delete(file);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Crée le bouton "Arrêter le PC" avec l'icône rouge officielle Windows Power (immédiat).
    /// </summary>
    public static bool CreateShutdownShortcut()
    {
        var target = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "shutdown.exe");
        var icon = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "shell32.dll") + ",27";
        return CreateDesktopLnk(ShutdownShortcutName, target, "/s /t 0", icon, "Éteindre complètement l'ordinateur immédiatement");
    }

    /// <summary>
    /// Crée le bouton "Redémarrer le PC" avec l'icône de redémarrage officielle Windows (immédiat).
    /// </summary>
    public static bool CreateRestartShortcut()
    {
        var target = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "shutdown.exe");
        var icon = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "shell32.dll") + ",238";
        return CreateDesktopLnk(RestartShortcutName, target, "/r /t 0", icon, "Redémarrer l'ordinateur immédiatement");
    }

    /// <summary>
    /// Crée un raccourci Windows .lnk sur le Bureau avec cible, arguments et icône personnalisée.
    /// </summary>
    public static bool CreateDesktopLnk(string shortcutFileName, string targetPath, string arguments, string iconLocation, string description)
    {
        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        if (!Directory.Exists(desktop))
        {
            Directory.CreateDirectory(desktop);
        }
        var shortcutPath = Path.Combine(desktop, shortcutFileName);

        // 1. Création via COM WScript.Shell
        try
        {
            Type? shellType = Type.GetTypeFromProgID("WScript.Shell");
            if (shellType != null)
            {
                dynamic shell = Activator.CreateInstance(shellType)!;
                dynamic shortcut = shell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = targetPath;
                shortcut.Arguments = arguments;
                shortcut.IconLocation = iconLocation;
                shortcut.Description = description;
                shortcut.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System);
                shortcut.Save();
                if (File.Exists(shortcutPath)) return true;
            }
        }
        catch
        {
            // Secours ci-dessous
        }

        // 2. Secours via commande PowerShell
        try
        {
            var psCode = $"$ws = New-Object -ComObject WScript.Shell; $s = $ws.CreateShortcut('{shortcutPath.Replace("'", "''")}'); $s.TargetPath = '{targetPath.Replace("'", "''")}'; $s.Arguments = '{arguments.Replace("'", "''")}'; $s.IconLocation = '{iconLocation.Replace("'", "''")}'; $s.Description = '{description.Replace("'", "''")}'; $s.Save()";
            using var proc = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{psCode}\"",
                CreateNoWindow = true,
                UseShellExecute = false
            });
            proc?.WaitForExit(3000);
            return File.Exists(shortcutPath);
        }
        catch
        {
            return false;
        }
    }

    // Raccourcis bureau hérités (sys_shortcuts_bureau)
    public static Task CreateShortcuts(Action<string> onOutput, CancellationToken ct = default)
    {
        bool ok1 = CreateShutdownShortcut();
        bool ok2 = CreateRestartShortcut();
        if (ok1 && ok2)
        {
            onOutput("[OK] Raccourcis 'Arrêter le PC' et 'Redémarrer le PC' créés sur le Bureau avec leurs icônes.");
        }
        else
        {
            onOutput("[INFO] Création des raccourcis terminée.");
        }
        return Task.CompletedTask;
    }

    // ─── Personnalisation des éléments du Menu Contextuel (Clic Droit) ───
    private static readonly string[] ContextMenuCmdAdminPaths = new[]
    {
        @"Software\Classes\DesktopBackground\shell\OpenCmdAdmin",
        @"Software\Classes\Directory\Background\shell\OpenCmdAdmin",
        @"Software\Classes\Directory\shell\OpenCmdAdmin",
        @"Software\Classes\Drive\shell\OpenCmdAdmin",
        @"Software\Classes\LibraryFolder\Background\shell\OpenCmdAdmin"
    };

    private const string RegContentCmdAdmin = @"Windows Registry Editor Version 5.00

[HKEY_CURRENT_USER\Software\Classes\DesktopBackground\shell\OpenCmdAdmin]
@=""Ouvrir CMD (Admin)""
""Icon""=""cmd.exe""
""HasLUAShield""=""""
""Position""=""Top""

[HKEY_CURRENT_USER\Software\Classes\DesktopBackground\shell\OpenCmdAdmin\command]
@=""powershell.exe -windowstyle hidden -command \""Start-Process cmd.exe -ArgumentList '/s /k pushd \\\""%V\\\""' -Verb RunAs\""""

[HKEY_CURRENT_USER\Software\Classes\Directory\Background\shell\OpenCmdAdmin]
@=""Ouvrir CMD (Admin)""
""Icon""=""cmd.exe""
""HasLUAShield""=""""
""Position""=""Top""

[HKEY_CURRENT_USER\Software\Classes\Directory\Background\shell\OpenCmdAdmin\command]
@=""powershell.exe -windowstyle hidden -command \""Start-Process cmd.exe -ArgumentList '/s /k pushd \\\""%V\\\""' -Verb RunAs\""""

[HKEY_CURRENT_USER\Software\Classes\Directory\shell\OpenCmdAdmin]
@=""Ouvrir CMD (Admin)""
""Icon""=""cmd.exe""
""HasLUAShield""=""""
""Position""=""Top""

[HKEY_CURRENT_USER\Software\Classes\Directory\shell\OpenCmdAdmin\command]
@=""powershell.exe -windowstyle hidden -command \""Start-Process cmd.exe -ArgumentList '/s /k pushd \\\""%V\\\""' -Verb RunAs\""""

[HKEY_CURRENT_USER\Software\Classes\Drive\shell\OpenCmdAdmin]
@=""Ouvrir CMD (Admin)""
""Icon""=""cmd.exe""
""HasLUAShield""=""""
""Position""=""Top""

[HKEY_CURRENT_USER\Software\Classes\Drive\shell\OpenCmdAdmin\command]
@=""powershell.exe -windowstyle hidden -command \""Start-Process cmd.exe -ArgumentList '/s /k pushd \\\""%V\\\""' -Verb RunAs\""""

[HKEY_CURRENT_USER\Software\Classes\LibraryFolder\Background\shell\OpenCmdAdmin]
@=""Ouvrir CMD (Admin)""
""Icon""=""cmd.exe""
""HasLUAShield""=""""
""Position""=""Top""

[HKEY_CURRENT_USER\Software\Classes\LibraryFolder\Background\shell\OpenCmdAdmin\command]
@=""powershell.exe -windowstyle hidden -command \""Start-Process cmd.exe -ArgumentList '/s /k pushd \\\""%V\\\""' -Verb RunAs\""""
";

    /// <summary>
    /// Ouvre l'interface de personnalisation du menu contextuel (CMD Admin sur Bureau & dossiers).
    /// </summary>
    public static async Task OpenContextMenuItemsDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var dialog = new Views.Controls.ContextMenuItemsDialog(onOutput, ct)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        });
    }

    private static readonly string[] ContextMenuSubKeyNames = new[]
    {
        "OpenCmdAdmin",
        "runas_cmd",
        "OpenCmdHere"
    };

    private static readonly string[] ContextMenuContainerPaths = new[]
    {
        @"Software\Classes\DesktopBackground\shell",
        @"Software\Classes\Directory\Background\shell",
        @"Software\Classes\Directory\shell",
        @"Software\Classes\Drive\shell",
        @"Software\Classes\LibraryFolder\Background\shell"
    };

    /// <summary>
    /// Recherche toutes les sous-clés correspondantes à CMD Admin (noms connus ou libellés contenant CMD + Admin).
    /// </summary>
    private static HashSet<string> GetMatchingSubKeyNames(RegistryKey rootKey, string containerPath)
    {
        var keys = new HashSet<string>(ContextMenuSubKeyNames, StringComparer.OrdinalIgnoreCase);
        try
        {
            using var container = rootKey.OpenSubKey(containerPath);
            if (container != null)
            {
                foreach (var name in container.GetSubKeyNames())
                {
                    using var sub = container.OpenSubKey(name);
                    if (sub != null)
                    {
                        var defVal = sub.GetValue("") as string ?? "";
                        if (defVal.Contains("cmd", StringComparison.OrdinalIgnoreCase) &&
                            (defVal.Contains("admin", StringComparison.OrdinalIgnoreCase) || defVal.Contains("administrateur", StringComparison.OrdinalIgnoreCase)))
                        {
                            keys.Add(name);
                        }
                    }
                }
            }
        }
        catch { }
        return keys;
    }

    /// <summary>
    /// Vérifie si une entrée "Ouvrir CMD (Admin)" est activée dans le menu contextuel (HKCU ou HKLM).
    /// </summary>
    public static bool IsCmdAdminContextMenuEnabled()
    {
        try
        {
            foreach (var container in ContextMenuContainerPaths)
            {
                // Vérification HKCU
                foreach (var subKey in GetMatchingSubKeyNames(Registry.CurrentUser, container))
                {
                    using var k1 = Registry.CurrentUser.OpenSubKey($@"{container}\{subKey}");
                    if (k1 != null) return true;
                }

                // Vérification HKLM
                foreach (var subKey in GetMatchingSubKeyNames(Registry.LocalMachine, container))
                {
                    using var k2 = Registry.LocalMachine.OpenSubKey($@"{container}\{subKey}");
                    if (k2 != null) return true;
                }
            }
        }
        catch { }
        return false;
    }

    /// <summary>
    /// Génère un fichier .reg, l'ouvre via regedit pour confirmation utilisateur, puis supprime le fichier .reg après validation.
    /// </summary>
    public static bool InstallCmdAdminViaRegFile()
    {
        // Nettoyage préalable des éventuelles anciennes clés parasites
        RemoveCmdAdminContextMenu();

        string regPath = Path.Combine(Path.GetTempPath(), "Activer_Ouvrir_CMD_Admin.reg");
        try
        {
            // Écriture du fichier .reg avec encodage Unicode officiel Windows
            File.WriteAllText(regPath, RegContentCmdAdmin, System.Text.Encoding.Unicode);

            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "regedit.exe",
                Arguments = $"\"{regPath}\"",
                UseShellExecute = true
            };

            using var proc = System.Diagnostics.Process.Start(psi);
            proc?.WaitForExit();

            return IsCmdAdminContextMenuEnabled();
        }
        finally
        {
            // Suppression immédiate du fichier .reg temporaire après validation
            if (File.Exists(regPath))
            {
                try { File.Delete(regPath); } catch { }
            }
        }
    }

    /// <summary>
    /// Supprime proprement et définitivement toutes les entrées de CMD Admin du menu contextuel (HKCU et HKLM).
    /// </summary>
    public static bool RemoveCmdAdminContextMenu()
    {
        // 1. Suppression directe via l'API .NET Registry (HKCU et HKLM)
        foreach (var container in ContextMenuContainerPaths)
        {
            var keysCu = GetMatchingSubKeyNames(Registry.CurrentUser, container);
            foreach (var subKey in keysCu)
            {
                try
                {
                    using var parentCu = Registry.CurrentUser.OpenSubKey(container, true);
                    parentCu?.DeleteSubKeyTree(subKey, false);
                }
                catch { }
            }

            var keysLm = GetMatchingSubKeyNames(Registry.LocalMachine, container);
            foreach (var subKey in keysLm)
            {
                try
                {
                    using var parentLm = Registry.LocalMachine.OpenSubKey(container, true);
                    parentLm?.DeleteSubKeyTree(subKey, false);
                }
                catch { }
            }
        }

        // 2. Nettoyage de sécurité renforcé via reg.exe
        try
        {
            foreach (var container in ContextMenuContainerPaths)
            {
                foreach (var subKey in ContextMenuSubKeyNames)
                {
                    var cmdCu = $"/c reg delete \"HKCU\\{container}\\{subKey}\" /f >nul 2>&1";
                    var cmdLm = $"/c reg delete \"HKLM\\{container}\\{subKey}\" /f >nul 2>&1";
                    using var p1 = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd.exe", cmdCu) { CreateNoWindow = true, UseShellExecute = false });
                    p1?.WaitForExit(1000);
                    using var p2 = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd.exe", cmdLm) { CreateNoWindow = true, UseShellExecute = false });
                    p2?.WaitForExit(1000);
                }
            }
        }
        catch { }

        // 3. Si une clé persistait (ex: restriction HKLM), création d'un fichier .reg de désinstallation
        if (IsCmdAdminContextMenuEnabled())
        {
            string delRegPath = Path.Combine(Path.GetTempPath(), "Supprimer_Ouvrir_CMD_Admin.reg");
            try
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("Windows Registry Editor Version 5.00");
                sb.AppendLine();
                foreach (var container in ContextMenuContainerPaths)
                {
                    var keysToDel = new HashSet<string>(ContextMenuSubKeyNames, StringComparer.OrdinalIgnoreCase);
                    foreach (var k in GetMatchingSubKeyNames(Registry.CurrentUser, container)) keysToDel.Add(k);
                    foreach (var k in GetMatchingSubKeyNames(Registry.LocalMachine, container)) keysToDel.Add(k);

                    foreach (var subKey in keysToDel)
                    {
                        sb.AppendLine($"[-HKEY_CURRENT_USER\\{container}\\{subKey}]");
                        sb.AppendLine($"[-HKEY_LOCAL_MACHINE\\{container}\\{subKey}]");
                    }
                }

                File.WriteAllText(delRegPath, sb.ToString(), System.Text.Encoding.Unicode);
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "regedit.exe",
                    Arguments = $"\"{delRegPath}\"",
                    UseShellExecute = true
                };
                using var pReg = System.Diagnostics.Process.Start(psi);
                pReg?.WaitForExit();
            }
            catch { }
            finally
            {
                if (File.Exists(delRegPath))
                {
                    try { File.Delete(delRegPath); } catch { }
                }
            }
        }

        return !IsCmdAdminContextMenuEnabled();
    }

    // ─── Personnalisation : Ouvrir PowerShell (Admin) au Clic Droit ───
    private static readonly string[] ContextMenuPowerShellSubKeyNames = new[]
    {
        "OpenPowerShellAdmin",
        "PowerShellAdmin",
        "runas_ps",
        "OpenPsAdmin",
        "PowerShellHereAdmin"
    };

    private const string RegContentPowerShellAdmin = @"Windows Registry Editor Version 5.00

[HKEY_CURRENT_USER\Software\Classes\DesktopBackground\shell\OpenPowerShellAdmin]
@=""Ouvrir PowerShell (Admin)""
""Icon""=""powershell.exe""
""HasLUAShield""=""""
""Position""=""Top""

[HKEY_CURRENT_USER\Software\Classes\DesktopBackground\shell\OpenPowerShellAdmin\command]
@=""powershell.exe -windowstyle hidden -command \""$d = '%V'; if (-not $d) { $d = [Environment]::GetFolderPath('Desktop') }; Start-Process powershell.exe -ArgumentList '-NoExit', '-Command', ('Set-Location -LiteralPath ''{0}''' -f $d) -Verb RunAs\""""

[HKEY_CURRENT_USER\Software\Classes\Directory\Background\shell\OpenPowerShellAdmin]
@=""Ouvrir PowerShell (Admin)""
""Icon""=""powershell.exe""
""HasLUAShield""=""""
""Position""=""Top""

[HKEY_CURRENT_USER\Software\Classes\Directory\Background\shell\OpenPowerShellAdmin\command]
@=""powershell.exe -windowstyle hidden -command \""Start-Process powershell.exe -ArgumentList '-NoExit', '-Command', 'Set-Location -LiteralPath ''%V''' -Verb RunAs\""""

[HKEY_CURRENT_USER\Software\Classes\Directory\shell\OpenPowerShellAdmin]
@=""Ouvrir PowerShell (Admin)""
""Icon""=""powershell.exe""
""HasLUAShield""=""""
""Position""=""Top""

[HKEY_CURRENT_USER\Software\Classes\Directory\shell\OpenPowerShellAdmin\command]
@=""powershell.exe -windowstyle hidden -command \""Start-Process powershell.exe -ArgumentList '-NoExit', '-Command', 'Set-Location -LiteralPath ''%V''' -Verb RunAs\""""

[HKEY_CURRENT_USER\Software\Classes\Drive\shell\OpenPowerShellAdmin]
@=""Ouvrir PowerShell (Admin)""
""Icon""=""powershell.exe""
""HasLUAShield""=""""
""Position""=""Top""

[HKEY_CURRENT_USER\Software\Classes\Drive\shell\OpenPowerShellAdmin\command]
@=""powershell.exe -windowstyle hidden -command \""Start-Process powershell.exe -ArgumentList '-NoExit', '-Command', 'Set-Location -LiteralPath ''%V''' -Verb RunAs\""""

[HKEY_CURRENT_USER\Software\Classes\LibraryFolder\Background\shell\OpenPowerShellAdmin]
@=""Ouvrir PowerShell (Admin)""
""Icon""=""powershell.exe""
""HasLUAShield""=""""
""Position""=""Top""

[HKEY_CURRENT_USER\Software\Classes\LibraryFolder\Background\shell\OpenPowerShellAdmin\command]
@=""powershell.exe -windowstyle hidden -command \""Start-Process powershell.exe -ArgumentList '-NoExit', '-Command', 'Set-Location -LiteralPath ''%V''' -Verb RunAs\""""
";

    /// <summary>
    /// Recherche toutes les sous-clés correspondantes à PowerShell Admin (noms connus ou libellés contenant PowerShell + Admin).
    /// </summary>
    private static HashSet<string> GetMatchingPowerShellSubKeyNames(RegistryKey rootKey, string containerPath)
    {
        var keys = new HashSet<string>(ContextMenuPowerShellSubKeyNames, StringComparer.OrdinalIgnoreCase);
        try
        {
            using var container = rootKey.OpenSubKey(containerPath);
            if (container != null)
            {
                foreach (var name in container.GetSubKeyNames())
                {
                    using var sub = container.OpenSubKey(name);
                    if (sub != null)
                    {
                        var defVal = sub.GetValue("") as string ?? "";
                        if (defVal.Contains("powershell", StringComparison.OrdinalIgnoreCase) &&
                            (defVal.Contains("admin", StringComparison.OrdinalIgnoreCase) || defVal.Contains("administrateur", StringComparison.OrdinalIgnoreCase)))
                        {
                            keys.Add(name);
                        }
                    }
                }
            }
        }
        catch { }
        return keys;
    }

    /// <summary>
    /// Vérifie si une entrée "Ouvrir PowerShell (Admin)" est activée dans le menu contextuel (HKCU ou HKLM).
    /// </summary>
    public static bool IsPowerShellAdminContextMenuEnabled()
    {
        try
        {
            foreach (var container in ContextMenuContainerPaths)
            {
                // Vérification HKCU
                foreach (var subKey in GetMatchingPowerShellSubKeyNames(Registry.CurrentUser, container))
                {
                    using var k1 = Registry.CurrentUser.OpenSubKey($@"{container}\{subKey}");
                    if (k1 != null) return true;
                }

                // Vérification HKLM
                foreach (var subKey in GetMatchingPowerShellSubKeyNames(Registry.LocalMachine, container))
                {
                    using var k2 = Registry.LocalMachine.OpenSubKey($@"{container}\{subKey}");
                    if (k2 != null) return true;
                }
            }
        }
        catch { }
        return false;
    }

    /// <summary>
    /// Génère un fichier .reg pour PowerShell Admin, l'ouvre via regedit pour confirmation utilisateur, puis supprime le fichier .reg après validation.
    /// </summary>
    public static bool InstallPowerShellAdminViaRegFile()
    {
        // Nettoyage préalable des éventuelles anciennes clés parasites
        RemovePowerShellAdminContextMenu();

        string regPath = Path.Combine(Path.GetTempPath(), "Activer_Ouvrir_PowerShell_Admin.reg");
        try
        {
            // Écriture du fichier .reg avec encodage Unicode officiel Windows
            File.WriteAllText(regPath, RegContentPowerShellAdmin, System.Text.Encoding.Unicode);

            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "regedit.exe",
                Arguments = $"\"{regPath}\"",
                UseShellExecute = true
            };

            using var proc = System.Diagnostics.Process.Start(psi);
            proc?.WaitForExit();

            return IsPowerShellAdminContextMenuEnabled();
        }
        finally
        {
            // Suppression immédiate du fichier .reg temporaire après validation
            if (File.Exists(regPath))
            {
                try { File.Delete(regPath); } catch { }
            }
        }
    }

    /// <summary>
    /// Supprime proprement et définitivement toutes les entrées de PowerShell Admin du menu contextuel (HKCU et HKLM).
    /// </summary>
    public static bool RemovePowerShellAdminContextMenu()
    {
        // 1. Suppression directe via l'API .NET Registry (HKCU et HKLM)
        foreach (var container in ContextMenuContainerPaths)
        {
            var keysCu = GetMatchingPowerShellSubKeyNames(Registry.CurrentUser, container);
            foreach (var subKey in keysCu)
            {
                try
                {
                    using var parentCu = Registry.CurrentUser.OpenSubKey(container, true);
                    parentCu?.DeleteSubKeyTree(subKey, false);
                }
                catch { }
            }

            var keysLm = GetMatchingPowerShellSubKeyNames(Registry.LocalMachine, container);
            foreach (var subKey in keysLm)
            {
                try
                {
                    using var parentLm = Registry.LocalMachine.OpenSubKey(container, true);
                    parentLm?.DeleteSubKeyTree(subKey, false);
                }
                catch { }
            }
        }

        // 2. Nettoyage de sécurité renforcé via reg.exe
        try
        {
            foreach (var container in ContextMenuContainerPaths)
            {
                foreach (var subKey in ContextMenuPowerShellSubKeyNames)
                {
                    var cmdCu = $"/c reg delete \"HKCU\\{container}\\{subKey}\" /f >nul 2>&1";
                    var cmdLm = $"/c reg delete \"HKLM\\{container}\\{subKey}\" /f >nul 2>&1";
                    using var p1 = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd.exe", cmdCu) { CreateNoWindow = true, UseShellExecute = false });
                    p1?.WaitForExit(1000);
                    using var p2 = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd.exe", cmdLm) { CreateNoWindow = true, UseShellExecute = false });
                    p2?.WaitForExit(1000);
                }
            }
        }
        catch { }

        // 3. Si une clé persistait (ex: restriction HKLM), création d'un fichier .reg de désinstallation
        if (IsPowerShellAdminContextMenuEnabled())
        {
            string delRegPath = Path.Combine(Path.GetTempPath(), "Supprimer_Ouvrir_PowerShell_Admin.reg");
            try
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("Windows Registry Editor Version 5.00");
                sb.AppendLine();
                foreach (var container in ContextMenuContainerPaths)
                {
                    var keysToDel = new HashSet<string>(ContextMenuPowerShellSubKeyNames, StringComparer.OrdinalIgnoreCase);
                    foreach (var k in GetMatchingPowerShellSubKeyNames(Registry.CurrentUser, container)) keysToDel.Add(k);
                    foreach (var k in GetMatchingPowerShellSubKeyNames(Registry.LocalMachine, container)) keysToDel.Add(k);

                    foreach (var subKey in keysToDel)
                    {
                        sb.AppendLine($"[-HKEY_CURRENT_USER\\{container}\\{subKey}]");
                        sb.AppendLine($"[-HKEY_LOCAL_MACHINE\\{container}\\{subKey}]");
                    }
                }

                File.WriteAllText(delRegPath, sb.ToString(), System.Text.Encoding.Unicode);
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "regedit.exe",
                    Arguments = $"\"{delRegPath}\"",
                    UseShellExecute = true
                };
                using var pReg = System.Diagnostics.Process.Start(psi);
                pReg?.WaitForExit();
            }
            catch { }
            finally
            {
                if (File.Exists(delRegPath))
                {
                    try { File.Delete(delRegPath); } catch { }
                }
            }
        }

        return !IsPowerShellAdminContextMenuEnabled();
    }
}
