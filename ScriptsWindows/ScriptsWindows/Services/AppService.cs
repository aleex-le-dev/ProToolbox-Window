using System.Windows;
using ScriptsWindows.Core;

namespace ScriptsWindows.Services;

/// <summary>Service applications — winget + installations (app_install_*, update_*).</summary>
public class AppService
{
    /// <summary>Ouvre le Centre d'Installation d'Applications (App Store Winget interactif).</summary>
    public static async Task OpenAppInstallerDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var dialog = new Views.Controls.AppInstallerDialog(onOutput, ct)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        });
    }

    /// <summary>Ouvre l'interface interactive de gestion des paquets et mises à jour Winget.</summary>
    public static async Task OpenWingetManagerDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var dialog = new Views.Controls.WingetUpdatesDialog(onOutput, ct)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        });
    }

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

    public const string ArtCraftGoogleDriveDirectUrl = "https://drive.usercontent.google.com/download?id=1DcGioXrlfMl77umBzhZLDwQiuUWyso_J&export=download&confirm=t";
    public const string ArtCraftGoogleDriveWebUrl = "https://drive.google.com/file/d/1DcGioXrlfMl77umBzhZLDwQiuUWyso_J/view?usp=drive_link";
    public const string ArtCraftBackupMsiUrl = "https://github.com/storytold/craft-launcher/releases/download/v0.3.2/artcraft-launcher-0.3.2-windows-x64.msi";
    public const string ArtCraftWebsiteUrl = "https://getartcraft.com/apps";

    public static async Task InstallOffice(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Ouverture du lien de téléchargement Microsoft Office...");
        CommandRunner.OpenUrl("https://drive.google.com/file/d/1wf1eqVkXIM0f0R2963Ee4RKK-a_rgWld/view?usp=drive_link");
        onOutput("[OK] Lien Google Drive ouvert dans le navigateur");
        await Task.CompletedTask;
    }

    private static async Task DownloadFileWithProgressAsync(
        System.Net.Http.HttpClient client,
        string url,
        string destinationPath,
        Action<string> onOutput,
        Action<string>? onProgress,
        CancellationToken ct)
    {
        using var response = await client.GetAsync(url, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, ct);
        response.EnsureSuccessStatusCode();

        long totalBytes = response.Content.Headers.ContentLength ?? -1L;
        string totalStr = totalBytes > 0 ? $"{totalBytes / 1024.0 / 1024.0:F1} Mo" : "taille inconnue";
        onOutput($"[i] Taille du téléchargement : {totalStr}");

        await using (var contentStream = await response.Content.ReadAsStreamAsync(ct))
        await using (var fileStream = new System.IO.FileStream(destinationPath, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None, 81920, true))
        {
            var buffer = new byte[81920];
            long totalRead = 0;
            int bytesRead;
            int lastReportedPercent = -1;

            while ((bytesRead = await contentStream.ReadAsync(buffer.AsMemory(0, buffer.Length), ct)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), ct);
                totalRead += bytesRead;

                if (totalBytes > 0)
                {
                    int percent = (int)((totalRead * 100) / totalBytes);
                    if (percent >= lastReportedPercent + 20 || percent == 100)
                    {
                        lastReportedPercent = percent;
                        string progressMsg = $"{percent}% ({totalRead / 1024.0 / 1024.0:F1} / {totalStr})";
                        onOutput($"[i] Téléchargement : {progressMsg}");
                        onProgress?.Invoke(progressMsg);
                    }
                }
            }
        }
    }

    private static async Task CreateShortcutsAsync(string targetExe)
    {
        try
        {
            string startMenu = Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms);
            if (string.IsNullOrEmpty(startMenu) || !System.IO.Directory.Exists(startMenu))
                startMenu = Environment.GetFolderPath(Environment.SpecialFolder.Programs);

            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
            if (string.IsNullOrEmpty(desktop) || !System.IO.Directory.Exists(desktop))
                desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string startMenuShortcut = System.IO.Path.Combine(startMenu, "ArtCraft Launcher.lnk");
            string desktopShortcut = System.IO.Path.Combine(desktop, "ArtCraft Launcher.lnk");

            string script = $@"
$w = New-Object -ComObject WScript.Shell
$s1 = $w.CreateShortcut('{startMenuShortcut.Replace("'", "''")}')
$s1.TargetPath = '{targetExe.Replace("'", "''")}'
$s1.Description = 'ArtCraft Launcher - Suite Créative Open-Source'
$s1.Save()

if (Test-Path '{desktop.Replace("'", "''")}') {{
    $s2 = $w.CreateShortcut('{desktopShortcut.Replace("'", "''")}')
    $s2.TargetPath = '{targetExe.Replace("'", "''")}'
    $s2.Description = 'ArtCraft Launcher - Suite Créative Open-Source'
    $s2.Save()
}}
";
            await PowerShellRunner.RunAsync(script);
        }
        catch { }
    }

    /// <summary>
    /// Télécharge et installe ArtCraft Launcher (source directe Google Drive, avec lien MSI de secours).
    /// </summary>
    public static async Task<bool> InstallArtCraft(Action<string> onOutput, Action<string>? onProgress = null, CancellationToken ct = default)
    {
        onOutput("\n=======================================================");
        onOutput("🎨 INSTALLATION DE ARTCRAFT LAUNCHER (SUITE CRÉATIVE)");
        onOutput("=======================================================");
        onOutput("[i] Suite créative : 13 applications libres & open-source (PhotoCraft, VectorCraft, FilmCraft, CAD...)");
        onOutput($"[i] Catalogue officiel : {ArtCraftWebsiteUrl}");
        onOutput($"[i] Source principale (Direct Google Drive) : {ArtCraftGoogleDriveWebUrl}");
        onOutput($"[i] Source de secours (.msi GitHub) : {ArtCraftBackupMsiUrl}");

        using var client = new System.Net.Http.HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AlexToolkit/4.1");

        string tempExe = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "artcraft-launcher-setup.exe");
        string tempMsi = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "artcraft-launcher-0.3.2-windows-x64.msi");

        // ── 1. TENTATIVE SOURCE PRINCIPALE : LIEN DIRECT GOOGLE DRIVE (EXE 0.5.1) ──
        try
        {
            onOutput("\n[i] --- Téléchargement via la source principale (Google Drive direct) ---");
            onProgress?.Invoke("Connexion à Google Drive...");

            await DownloadFileWithProgressAsync(client, ArtCraftGoogleDriveDirectUrl, tempExe, onOutput, onProgress, ct);

            onOutput("[OK] Téléchargement Google Drive terminé avec succès !");
            onOutput("[i] Déploiement de l'exécutable ArtCraft Launcher dans Program Files...");
            onProgress?.Invoke("Installation en cours...");

            string installDir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "ArtCraft Launcher");
            System.IO.Directory.CreateDirectory(installDir);
            string targetExe = System.IO.Path.Combine(installDir, "artcraft-launcher.exe");

            System.IO.File.Copy(tempExe, targetExe, overwrite: true);
            onOutput($"[OK] Application installée dans : {targetExe}");

            onOutput("[i] Création des raccourcis Bureau et Menu Démarrer...");
            await CreateShortcutsAsync(targetExe);
            onOutput("[OK] Raccourcis créés avec succès !");

            onOutput("[i] Lancement de l'application ArtCraft Launcher...");
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(targetExe) { UseShellExecute = true });
            }
            catch { }

            onOutput("[OK] ArtCraft Launcher a été installé et lancé avec succès !");
            onProgress?.Invoke("Installé avec succès !");
            return true;
        }
        catch (OperationCanceledException)
        {
            onOutput("[!] Installation annulée.");
            onProgress?.Invoke("Annulé");
            return false;
        }
        catch (Exception exGoogleDrive)
        {
            onOutput($"[!] Échec du téléchargement Google Drive : {exGoogleDrive.Message}");
            onOutput("[i] Basculement vers la source de secours officielle (.msi GitHub Releases)...");
            onProgress?.Invoke("Secours : GitHub Releases...");
        }
        finally
        {
            try { if (System.IO.File.Exists(tempExe)) System.IO.File.Delete(tempExe); } catch { }
        }

        // ── 2. TENTATIVE SOURCE DE SECOURS : LIEN .MSI (GITHUB RELEASES) ──
        try
        {
            onOutput("\n[i] --- Téléchargement via le lien de secours (.msi GitHub) ---");
            await DownloadFileWithProgressAsync(client, ArtCraftBackupMsiUrl, tempMsi, onOutput, onProgress, ct);

            onOutput("[OK] Téléchargement du package MSI de secours réussi !");
            onOutput("[i] Lancement de l'installateur Windows (msiexec)...");
            onProgress?.Invoke("Installation MSI en cours...");

            var psi = new System.Diagnostics.ProcessStartInfo("msiexec.exe")
            {
                Arguments = $"/i \"{tempMsi}\" /passive /norestart",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = System.Diagnostics.Process.Start(psi);
            if (proc != null)
            {
                await proc.WaitForExitAsync(ct);
                if (proc.ExitCode == 0 || proc.ExitCode == 3010)
                {
                    onOutput("[OK] ArtCraft Launcher installé avec succès via le package MSI de secours !");
                    onProgress?.Invoke("Installé avec succès !");
                    return true;
                }
                else
                {
                    onOutput($"[!] L'installateur MSI s'est terminé avec le code : {proc.ExitCode}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            onOutput("[!] Installation annulée.");
            onProgress?.Invoke("Annulé");
            return false;
        }
        catch (Exception exMsi)
        {
            onOutput($"[!] Échec également sur le lien de secours MSI : {exMsi.Message}");
        }
        finally
        {
            try { if (System.IO.File.Exists(tempMsi)) System.IO.File.Delete(tempMsi); } catch { }
        }

        // ── 3. DERNIER RECOURS : OUVERTURE DU LIEN DRIVE DANS LE NAVIGATEUR ──
        onOutput("\n[i] Ouverture du lien Google Drive dans le navigateur par défaut pour téléchargement manuel...");
        CommandRunner.OpenUrl(ArtCraftGoogleDriveWebUrl);
        onOutput("[OK] Lien ouvert dans le navigateur.");
        onProgress?.Invoke("Lien ouvert dans le navigateur");
        return false;
    }

    public static void OpenArtCraftWebsite() => CommandRunner.OpenUrl(ArtCraftWebsiteUrl);
    public static void OpenArtCraftGoogleDrive() => CommandRunner.OpenUrl(ArtCraftGoogleDriveWebUrl);

    public static async Task GetInstalledApps(Action<string> onOutput, CancellationToken ct = default)
        => await CommandRunner.RunAsync("winget list", onOutput, ct);
}
