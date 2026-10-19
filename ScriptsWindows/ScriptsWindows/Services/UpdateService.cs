using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ScriptsWindows.Models;

namespace ScriptsWindows.Services;

/// <summary>
/// Service officiel de mise à jour pour Alex Toolkit.
/// Gère la détection en ligne, le téléchargement direct, l'auto-remplacement et le redémarrage.
/// </summary>
public static class UpdateService
{
    private static readonly HttpClient _httpClient = new(new HttpClientHandler
    {
        AllowAutoRedirect = true
    })
    {
        Timeout = TimeSpan.FromSeconds(20)
    };

    private static readonly string SettingsFile = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ScriptsWindows",
        "settings_update.json"
    );

    // Endpoints GitHub de releases
    private static readonly string[] ReleaseEndpoints = new[]
    {
        "https://api.github.com/repos/aleex-le-dev/ProToolbox-Window/releases",
        "https://api.github.com/repos/aleex-le-dev/SCRIPT-WINDOWS-ALEEXLEDEV/releases"
    };

    static UpdateService()
    {
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("AlexToolkit-Updater/4.1.0 (Windows NT 10.0; Win64; x64)");
        _httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github.v3+json");
    }

    /// <summary>
    /// Récupère la version actuelle locale d'Alex Toolkit.
    /// </summary>
    public static Version GetCurrentVersion()
    {
        var ver = typeof(UpdateService).Assembly.GetName().Version;
        return ver != null ? new Version(ver.Major, ver.Minor, Math.Max(0, ver.Build)) : new Version(4, 1, 0);
    }

    /// <summary>
    /// Charge les préférences de mise à jour depuis le stockage local.
    /// </summary>
    public static UpdateSettings LoadSettings()
    {
        try
        {
            if (File.Exists(SettingsFile))
            {
                string json = File.ReadAllText(SettingsFile);
                var settings = JsonSerializer.Deserialize<UpdateSettings>(json);
                if (settings != null) return settings;
            }
        }
        catch { }

        return new UpdateSettings();
    }

    /// <summary>
    /// Sauvegarde les préférences de mise à jour.
    /// </summary>
    public static void SaveSettings(UpdateSettings settings)
    {
        try
        {
            string dir = Path.GetDirectoryName(SettingsFile)!;
            Directory.CreateDirectory(dir);
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsFile, json);
        }
        catch { }
    }

    /// <summary>
    /// Vérifie en ligne les mises à jour disponibles via GitHub Releases.
    /// </summary>
    public static async Task<UpdateCheckResult> CheckForUpdatesAsync(CancellationToken ct = default)
    {
        var currentVersion = GetCurrentVersion();
        Exception? lastException = null;

        foreach (var endpoint in ReleaseEndpoints)
        {
            try
            {
                using var response = await _httpClient.GetAsync(endpoint, ct).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode) continue;

                string json = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                using var doc = JsonDocument.Parse(json);

                if (doc.RootElement.ValueKind != JsonValueKind.Array) continue;

                var releases = new List<ReleaseInfo>();

                foreach (var el in doc.RootElement.EnumerateArray())
                {
                    bool isDraft = el.TryGetProperty("draft", out var draftProp) && draftProp.GetBoolean();
                    if (isDraft) continue;

                    string tagName = el.TryGetProperty("tag_name", out var tagProp) ? tagProp.GetString() ?? "" : "";
                    if (string.IsNullOrWhiteSpace(tagName)) continue;

                    string title = el.TryGetProperty("name", out var nameProp) ? nameProp.GetString() ?? tagName : tagName;
                    string body = el.TryGetProperty("body", out var bodyProp) ? bodyProp.GetString() ?? "" : "";
                    string htmlUrl = el.TryGetProperty("html_url", out var htmlProp) ? htmlProp.GetString() ?? "" : "";

                    DateTime publishedAt = DateTime.MinValue;
                    if (el.TryGetProperty("published_at", out var pubProp) && pubProp.GetString() is string pubStr)
                    {
                        DateTime.TryParse(pubStr, out publishedAt);
                    }

                    // Recherche de l'asset AlexToolkit.exe
                    string downloadUrl = string.Empty;
                    long fileSize = 0;

                    if (el.TryGetProperty("assets", out var assetsProp) && assetsProp.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var asset in assetsProp.EnumerateArray())
                        {
                            string assetName = asset.TryGetProperty("name", out var aNameProp) ? aNameProp.GetString() ?? "" : "";
                            if (assetName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                            {
                                downloadUrl = asset.TryGetProperty("browser_download_url", out var bUrlProp) ? bUrlProp.GetString() ?? "" : "";
                                if (asset.TryGetProperty("size", out var sizeProp))
                                {
                                    fileSize = sizeProp.GetInt64();
                                }
                                if (assetName.Equals("AlexToolkit.exe", StringComparison.OrdinalIgnoreCase))
                                {
                                    break; // Priorité exacte à AlexToolkit.exe
                                }
                            }
                        }
                    }

                    // Fallback URL directe si non listé explicitement
                    if (string.IsNullOrEmpty(downloadUrl))
                    {
                        downloadUrl = $"https://github.com/aleex-le-dev/ProToolbox-Window/releases/download/{tagName}/AlexToolkit.exe";
                    }

                    var parsedVersion = ParseVersion(tagName);
                    bool isNewer = parsedVersion != null && parsedVersion > currentVersion;
                    bool isCurrent = parsedVersion != null && parsedVersion == currentVersion;

                    releases.Add(new ReleaseInfo
                    {
                        TagName = tagName,
                        Title = title,
                        Description = body,
                        HtmlUrl = htmlUrl,
                        DownloadUrl = downloadUrl,
                        FileSizeBytes = fileSize,
                        PublishedAt = publishedAt,
                        Version = parsedVersion,
                        IsNewer = isNewer,
                        IsCurrent = isCurrent
                    });
                }

                if (releases.Count > 0)
                {
                    // Trier par version descendante puis par date
                    var sortedReleases = releases
                        .OrderByDescending(r => r.Version ?? new Version(0, 0, 0))
                        .ThenByDescending(r => r.PublishedAt)
                        .ToList();

                    var latest = sortedReleases.FirstOrDefault();

                    // Mise à jour des paramètres de dernière vérification
                    var settings = LoadSettings();
                    settings.LastCheckDate = DateTime.Now;
                    settings.LastKnownLatestVersion = latest?.TagName;
                    SaveSettings(settings);

                    return new UpdateCheckResult
                    {
                        Success = true,
                        LatestRelease = latest,
                        AllReleases = sortedReleases,
                        CurrentVersion = currentVersion
                    };
                }
            }
            catch (Exception ex)
            {
                lastException = ex;
            }
        }

        return new UpdateCheckResult
        {
            Success = false,
            ErrorMessage = lastException != null ? lastException.Message : "Impossible de contacter les serveurs de mise à jour GitHub.",
            CurrentVersion = currentVersion
        };
    }

    /// <summary>
    /// Vérification silencieuse au démarrage pour alerter l'utilisateur de manière non intrusive.
    /// </summary>
    public static async Task<ReleaseInfo?> CheckSilentlyOnStartupAsync()
    {
        var settings = LoadSettings();
        if (!settings.AutoCheckOnStartup) return null;

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(8));
            var result = await CheckForUpdatesAsync(cts.Token).ConfigureAwait(false);
            if (result.Success && result.HasUpdate)
            {
                return result.LatestRelease;
            }
        }
        catch { }

        return null;
    }

    /// <summary>
    /// Télécharge le nouvel exécutable avec suivi précis de la progression (octets, total, pourcentage).
    /// </summary>
    public static async Task<string> DownloadUpdateAsync(
        string downloadUrl,
        string? targetPath = null,
        IProgress<(long bytesDownloaded, long totalBytes, double percent)>? progress = null,
        CancellationToken ct = default)
    {
        string destinationFile = targetPath ?? Path.Combine(
            Path.GetTempPath(),
            $"AlexToolkit_Update_{Guid.NewGuid():N}.exe"
        );

        using var response = await _httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        long totalBytes = response.Content.Headers.ContentLength ?? -1;

        await using var contentStream = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
        await using var fileStream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write, FileShare.None, 81920, true);

        var buffer = new byte[81920];
        long totalRead = 0;
        int read;

        while ((read = await contentStream.ReadAsync(buffer, 0, buffer.Length, ct).ConfigureAwait(false)) > 0)
        {
            await fileStream.WriteAsync(buffer, 0, read, ct).ConfigureAwait(false);
            totalRead += read;

            if (totalBytes > 0 && progress != null)
            {
                double percent = (double)totalRead / totalBytes * 100.0;
                progress.Report((totalRead, totalBytes, percent));
            }
        }

        return destinationFile;
    }

    /// <summary>
    /// Remplace le binaire actuel par le nouveau et redémarre instantanément l'application.
    /// </summary>
    public static void ApplyUpdateAndRestart(string tempDownloadedExePath)
    {
        string currentExePath = Environment.ProcessPath ?? Process.GetCurrentProcess().MainModule?.FileName ?? "";
        if (string.IsNullOrEmpty(currentExePath) || !File.Exists(tempDownloadedExePath))
        {
            throw new InvalidOperationException("Fichiers de mise à jour introuvables ou chemin d'exécution non résolu.");
        }

        int currentPid = Process.GetCurrentProcess().Id;
        string updaterBatPath = Path.Combine(Path.GetTempPath(), $"alextoolkit_updater_{Guid.NewGuid():N}.cmd");

        // Script batch autonome natif : attend l'arrêt du PID, remplace le binaire, relance et s'auto-supprime
        string batchScript = $@"@echo off
setlocal
set ""TARGET={currentExePath}""
set ""SOURCE={tempDownloadedExePath}""
set ""PID={currentPid}""

:: 1. Attente de la libération complète du processus
:wait_loop
tasklist /fi ""PID eq %PID%"" 2>nul | find ""%PID%"" >nul
if not errorlevel 1 (
    timeout /t 1 /nobreak >nul
    goto wait_loop
)

:: Pause de sécurité pour libérer les handles de fichier
timeout /t 1 /nobreak >nul

:: 2. Remplacement du binaire (avec stratégie .old en cas de verrou récalcitrant)
copy /Y ""%SOURCE%"" ""%TARGET%"" >nul 2>&1
if errorlevel 1 (
    move /Y ""%TARGET%"" ""%TARGET%.old"" >nul 2>&1
    copy /Y ""%SOURCE%"" ""%TARGET%"" >nul 2>&1
)

:: Nettoyage du fichier temporaire source
del /f /q ""%SOURCE%"" >nul 2>&1
if exist ""%TARGET%.old"" del /f /q ""%TARGET%.old"" >nul 2>&1

:: 3. Relance de la nouvelle version
start """" ""%TARGET%""

:: Auto-suppression du script updater
del ""%~f0"" >nul 2>&1
";

        File.WriteAllText(updaterBatPath, batchScript, System.Text.Encoding.ASCII);

        var psi = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c \"{updaterBatPath}\"",
            UseShellExecute = true,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        Process.Start(psi);

        // Fermeture immédiate et totale garantie du processus actuel pour libérer le fichier
        try
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Application.Current.Shutdown();
            });
        }
        catch { }

        Environment.Exit(0);
    }

    /// <summary>
    /// Ouvre la boîte de dialogue de mise à jour dans le thread UI.
    /// </summary>
    public static Task OpenUpdateDialog(Action<string>? onOutput = null, CancellationToken ct = default)
    {
        return Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var win = new Views.Controls.UpdateDialog();
            win.Owner = Application.Current.MainWindow;
            win.ShowDialog();
        }).Task;
    }

    /// <summary>
    /// Parse un tag de version (ex: 'v4.1.0' -> Version(4, 1, 0)).
    /// </summary>
    public static Version? ParseVersion(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag)) return null;

        string clean = tag.Trim().TrimStart('v', 'V');
        // Extraire uniquement les chiffres et points initiaux (ex: 4.1.0-beta -> 4.1.0)
        int dashIdx = clean.IndexOfAny(new[] { '-', '+', ' ' });
        if (dashIdx > 0) clean = clean.Substring(0, dashIdx);

        if (Version.TryParse(clean, out var v))
        {
            return v;
        }

        // Compléter si format 4 ou 4.1
        var parts = clean.Split('.');
        if (parts.Length == 1 && int.TryParse(parts[0], out int major))
        {
            return new Version(major, 0);
        }
        if (parts.Length == 2 && int.TryParse(parts[0], out major) && int.TryParse(parts[1], out int minor))
        {
            return new Version(major, minor);
        }

        return null;
    }
}
