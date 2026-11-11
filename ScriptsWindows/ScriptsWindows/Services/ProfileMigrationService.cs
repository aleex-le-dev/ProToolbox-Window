using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Win32;
using ScriptsWindows.Core;

namespace ScriptsWindows.Services;

public enum MigrationKind { Folder, Bookmarks, Wifi, Apps }

/// <summary>Élément sauvegardable du profil (dossier, favoris, Wi-Fi, logiciels).</summary>
public class MigrationItem : INotifyPropertyChanged
{
    private bool _isSelected = true;
    private string _sizeText = "Calcul…";
    private bool _isAvailable = true;

    public required string Key { get; init; }
    public required string Label { get; init; }
    public required MigrationKind Kind { get; init; }
    public string SourcePath { get; init; } = "";
    public string Hint { get; init; } = "";
    /// <summary>Taille en octets (dossiers), -1 si inconnue.</summary>
    public long SizeBytes { get; set; } = -1;

    public bool IsSelected { get => _isSelected; set => Set(ref _isSelected, value); }
    public string SizeText { get => _sizeText; set => Set(ref _sizeText, value); }
    public bool IsAvailable { get => _isAvailable; set => Set(ref _isAvailable, value); }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

/// <summary>Manifeste écrit à la racine de chaque sauvegarde (sauvegarde.json).</summary>
public class BackupManifest
{
    public string Computer { get; set; } = "";
    public string User { get; set; } = "";
    public DateTime Date { get; set; }
    public List<string> Items { get; set; } = new();
    public string ToolVersion { get; set; } = "";
}

/// <summary>
/// Sauvegarde et migration du profil de l'utilisateur connecté vers un disque externe / une clé USB,
/// puis restauration sur un autre PC. La restauration n'efface jamais rien et n'écrase pas un fichier plus récent.
/// </summary>
public static class ProfileMigrationService
{
    public const string ManifestName = "sauvegarde.json";
    private static string Local => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    private static string Roaming => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    private static string ChromeUserData => Path.Combine(Local, @"Google\Chrome\User Data");
    private static string EdgeUserData => Path.Combine(Local, @"Microsoft\Edge\User Data");

    /// <summary>Profils d'un navigateur Chromium possédant des favoris (Default, Profile 1, Profile 2…).</summary>
    private static IEnumerable<string> BookmarkProfiles(string userData)
        => Directory.Exists(userData)
            ? Directory.GetDirectories(userData).Where(d => File.Exists(Path.Combine(d, "Bookmarks"))).Select(d => Path.GetFileName(d))
            : Enumerable.Empty<string>();
    private static string FirefoxProfiles => Path.Combine(Roaming, @"Mozilla\Firefox\Profiles");

    /// <summary>Dossier « Téléchargements » réel (peut être déplacé par l'utilisateur).</summary>
    private static string DownloadsFolder()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders");
            if (key?.GetValue("{374DE290-123F-4565-9164-39C4925E467B}") is string p && p.Length > 0)
                return Environment.ExpandEnvironmentVariables(p);
        }
        catch { }
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
    }

    /// <summary>Éléments du profil actuel (chemins réels, y compris redirections OneDrive).</summary>
    public static List<MigrationItem> GetItems() => new()
    {
        new() { Key = "Bureau",            Label = "Bureau",            Kind = MigrationKind.Folder, SourcePath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) },
        new() { Key = "Documents",         Label = "Documents",         Kind = MigrationKind.Folder, SourcePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) },
        new() { Key = "Images",            Label = "Images",            Kind = MigrationKind.Folder, SourcePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) },
        new() { Key = "Videos",            Label = "Vidéos",            Kind = MigrationKind.Folder, SourcePath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) },
        new() { Key = "Musique",           Label = "Musique",           Kind = MigrationKind.Folder, SourcePath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) },
        new() { Key = "Telechargements",   Label = "Téléchargements",   Kind = MigrationKind.Folder, SourcePath = DownloadsFolder(), IsSelected = false },
        new() { Key = "Favoris",           Label = "Favoris des navigateurs", Kind = MigrationKind.Bookmarks, Hint = "Chrome, Edge, Firefox" },
        new() { Key = "WiFi",              Label = "Réseaux Wi-Fi enregistrés", Kind = MigrationKind.Wifi, Hint = "Clés incluses : protégez la sauvegarde" },
        new() { Key = "Applications",      Label = "Liste des logiciels installés", Kind = MigrationKind.Apps, Hint = "Réinstallation automatique via Winget" },
    };

    public static string FormatSize(long bytes) => bytes switch
    {
        >= 1L << 30 => $"{bytes / (double)(1L << 30):0.0} Go",
        >= 1L << 20 => $"{bytes / (double)(1L << 20):0} Mo",
        >= 1L << 10 => $"{bytes / (double)(1L << 10):0} Ko",
        _ => $"{bytes} o"
    };

    public static long FolderSize(string path)
    {
        if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return -1;
        long size = 0;
        var opts = new EnumerationOptions { RecurseSubdirectories = true, IgnoreInaccessible = true, AttributesToSkip = FileAttributes.ReparsePoint };
        try { foreach (var f in new DirectoryInfo(path).EnumerateFiles("*", opts)) { try { size += f.Length; } catch { } } } catch { }
        return size;
    }

    /// <summary>Calcule la taille de chaque élément (hors thread UI).</summary>
    public static Task ComputeSizesAsync(IEnumerable<MigrationItem> items) => Task.Run(() =>
    {
        foreach (var item in items)
        {
            string text; bool available = true; long bytes = -1;
            switch (item.Kind)
            {
                case MigrationKind.Folder:
                    bytes = FolderSize(item.SourcePath);
                    available = bytes >= 0;
                    text = available ? FormatSize(bytes) : "Introuvable";
                    break;
                case MigrationKind.Bookmarks:
                    var found = new List<string>();
                    if (BookmarkProfiles(ChromeUserData).Any()) found.Add("Chrome");
                    if (BookmarkProfiles(EdgeUserData).Any()) found.Add("Edge");
                    if (Directory.Exists(FirefoxProfiles)) found.Add("Firefox");
                    available = found.Count > 0;
                    text = available ? string.Join(", ", found) : "Aucun navigateur";
                    break;
                default:
                    text = item.Hint;
                    break;
            }
            System.Windows.Application.Current?.Dispatcher.Invoke(() => { item.SizeBytes = bytes; item.SizeText = text; item.IsAvailable = available; if (!available) item.IsSelected = false; });
        }
    });

    private static string Q(string s) => "'" + s.Replace("'", "''") + "'";

    /// <summary>Copie robocopy silencieuse ; restore=true : n'écrase pas les fichiers plus récents de la destination.</summary>
    private static Task Robocopy(string src, string dst, bool restore, Action<string> log, CancellationToken ct)
        => PowerShellRunner.RunAsync($@"
$src = {Q(src)}; $dst = {Q(dst)}
New-Item -ItemType Directory -Force -Path $dst | Out-Null
& robocopy $src $dst /E /R:1 /W:1 /XJ /MT:8 /NFL /NDL /NJH /NJS /NP /XF desktop.ini thumbs.db {(restore ? "/XO" : "")} | Out-Null
$code = $LASTEXITCODE
$n = @(Get-ChildItem $dst -Recurse -File -Force -ErrorAction SilentlyContinue).Count
if ($code -lt 8) {{ Write-Output ""[OK] $(Split-Path $src -Leaf) → $dst ($n fichiers)"" }}
else {{ Write-Output ""[ERR] Copie incomplète de $src (code robocopy $code : fichiers verrouillés ou accès refusé)"" }}
", log, ct);

    public static async Task<string> BackupAsync(IEnumerable<MigrationItem> items, string destinationRoot, Action<string> log, CancellationToken ct)
    {
        string user = Environment.UserName;
        string root = Path.Combine(destinationRoot, $"Sauvegarde_{user}_{DateTime.Now:yyyy-MM-dd_HH-mm}");
        Directory.CreateDirectory(root);
        var done = new List<string>();

        foreach (var item in items.Where(i => i.IsSelected && i.IsAvailable))
        {
            ct.ThrowIfCancellationRequested();
            log($"\n[i] ── {item.Label} ──");
            switch (item.Kind)
            {
                case MigrationKind.Folder:
                    await Robocopy(item.SourcePath, Path.Combine(root, "Fichiers", item.Key), false, log, ct);
                    break;

                case MigrationKind.Bookmarks:
                    string nav = Path.Combine(root, "Navigateurs");
                    void CopyBrowser(string userData, string name)
                    {
                        foreach (var profile in BookmarkProfiles(userData))
                        {
                            Directory.CreateDirectory(Path.Combine(nav, name, profile));
                            File.Copy(Path.Combine(userData, profile, "Bookmarks"), Path.Combine(nav, name, profile, "Bookmarks"), true);
                            log($"[OK] Favoris {name} ({profile}) sauvegardés");
                        }
                    }
                    CopyBrowser(ChromeUserData, "Chrome");
                    CopyBrowser(EdgeUserData, "Edge");
                    if (Directory.Exists(FirefoxProfiles))
                        foreach (var backups in Directory.GetDirectories(FirefoxProfiles).Select(p => Path.Combine(p, "bookmarkbackups")).Where(Directory.Exists))
                            await Robocopy(backups, Path.Combine(nav, "Firefox", Path.GetFileName(Path.GetDirectoryName(backups)!)), false, log, ct);
                    break;

                case MigrationKind.Wifi:
                    string wifi = Path.Combine(root, "WiFi");
                    Directory.CreateDirectory(wifi);
                    await PowerShellRunner.RunAsync($@"
$root = {Q(wifi)}
# netsh tronque le chemin du fichier exporté (~128 caractères) : sur une destination profonde, deux réseaux
# finissent dans le même fichier tronqué. On exporte donc profil par profil dans un dossier temporaire au chemin court.
$tmp = Join-Path $env:PUBLIC ('ATK' + [guid]::NewGuid().ToString('N').Substring(0, 6))
$names = netsh wlan show profiles | ForEach-Object {{ if ($_ -match '^\s+[^:]+\s:\s(.+)$') {{ $Matches[1].Trim() }} }}
$i = 0
try {{
    foreach ($name in $names) {{
        $i++; $sub = Join-Path $tmp $i
        New-Item -ItemType Directory -Force -Path $sub | Out-Null
        & netsh wlan export profile name=""$name"" key=clear folder=""$sub"" | Out-Null
        $dest = Join-Path $root $i
        New-Item -ItemType Directory -Force -Path $dest | Out-Null
        Get-ChildItem $sub -Filter *.xml -ErrorAction SilentlyContinue | Move-Item -Destination $dest -Force
    }}
}} finally {{
    Remove-Item $tmp -Recurse -Force -ErrorAction SilentlyContinue   # contenait des clés en clair
}}
$n = @(Get-ChildItem $root -Recurse -Filter *.xml -ErrorAction SilentlyContinue).Count
Write-Output ""[OK] $n profil(s) Wi-Fi exporté(s) (clés en clair : sauvegarde à protéger)""
", log, ct);
                    break;

                case MigrationKind.Apps:
                    string apps = Path.Combine(root, "Applications");
                    Directory.CreateDirectory(apps);
                    await PowerShellRunner.RunAsync($@"
$dir = {Q(apps)}
& winget export -o (Join-Path $dir 'winget.json') --accept-source-agreements --disable-interactivity 2>$null | Out-Null
$keys = 'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*','HKLM:\Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*','HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*'
$list = Get-ItemProperty $keys -ErrorAction SilentlyContinue | Where-Object {{ $_.DisplayName -and -not $_.SystemComponent }} |
    Sort-Object DisplayName -Unique | ForEach-Object {{ ""{{0}}  {{1}}  {{2}}"" -f $_.DisplayName, $_.DisplayVersion, $_.Publisher }}
$list | Out-File (Join-Path $dir 'logiciels_installes.txt') -Encoding utf8
$w = if (Test-Path (Join-Path $dir 'winget.json')) {{ 'winget.json créé' }} else {{ 'winget indisponible' }}
Write-Output ""[OK] $(@($list).Count) logiciels listés ($w)""
", log, ct);
                    break;
            }
            done.Add(item.Key);
        }

        var manifest = new BackupManifest
        {
            Computer = Environment.MachineName, User = user, Date = DateTime.Now, Items = done,
            ToolVersion = UpdateService.GetCurrentVersion().ToString()
        };
        File.WriteAllText(Path.Combine(root, ManifestName), JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true }));
        log($"[OK] Sauvegarde terminée : {root}");
        return root;
    }

    public static BackupManifest? ReadManifest(string folder)
    {
        try
        {
            string path = Path.Combine(folder, ManifestName);
            return File.Exists(path) ? JsonSerializer.Deserialize<BackupManifest>(File.ReadAllText(path)) : null;
        }
        catch { return null; }
    }

    public static async Task RestoreAsync(IEnumerable<MigrationItem> items, string backupRoot, bool reinstallApps, Action<string> log, CancellationToken ct)
    {
        foreach (var item in items.Where(i => i.IsSelected))
        {
            ct.ThrowIfCancellationRequested();
            switch (item.Kind)
            {
                case MigrationKind.Folder:
                    string src = Path.Combine(backupRoot, "Fichiers", item.Key);
                    if (!Directory.Exists(src)) continue;
                    log($"\n[i] ── {item.Label} ──");
                    await Robocopy(src, item.SourcePath, true, log, ct);
                    break;

                case MigrationKind.Bookmarks:
                    log($"\n[i] ── {item.Label} ──");
                    void RestoreBookmarks(string name, string userData, string process)
                    {
                        string fromRoot = Path.Combine(backupRoot, "Navigateurs", name);
                        if (!Directory.Exists(fromRoot)) return;
                        if (System.Diagnostics.Process.GetProcessesByName(process).Length > 0)
                        {
                            log($"[!] {name} est ouvert : fermez-le puis relancez la restauration des favoris.");
                            return;
                        }
                        foreach (var profileDir in Directory.GetDirectories(fromRoot))
                        {
                            string from = Path.Combine(profileDir, "Bookmarks");
                            if (!File.Exists(from)) continue;
                            string target = Path.Combine(userData, Path.GetFileName(profileDir), "Bookmarks");
                            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
                            if (File.Exists(target)) File.Copy(target, target + ".avant_restauration", true);
                            File.Copy(from, target, true);
                            log($"[OK] Favoris {name} ({Path.GetFileName(profileDir)}) restaurés (ancien fichier conservé en .avant_restauration)");
                        }
                    }
                    RestoreBookmarks("Chrome", ChromeUserData, "chrome");
                    RestoreBookmarks("Edge", EdgeUserData, "msedge");
                    if (Directory.Exists(Path.Combine(backupRoot, "Navigateurs", "Firefox")))
                        log("[i] Firefox : copiez une sauvegarde de « Navigateurs\\Firefox » puis, dans Firefox, Bibliothèque > Importation et sauvegarde > Restaurer.");
                    break;

                case MigrationKind.Wifi:
                    string wifi = Path.Combine(backupRoot, "WiFi");
                    if (!Directory.Exists(wifi)) continue;
                    log($"\n[i] ── {item.Label} ──");
                    await PowerShellRunner.RunAsync($@"
$ok = 0
Get-ChildItem {Q(wifi)} -Recurse -Filter *.xml | ForEach-Object {{
    $r = & netsh wlan add profile filename=""$($_.FullName)"" user=all
    if ($LASTEXITCODE -eq 0) {{ $ok++ }}
}}
Write-Output ""[OK] $ok profil(s) Wi-Fi importé(s)""
", log, ct);
                    break;

                case MigrationKind.Apps:
                    string json = Path.Combine(backupRoot, "Applications", "winget.json");
                    if (!reinstallApps || !File.Exists(json)) continue;
                    log($"\n[i] ── Réinstallation des logiciels (peut prendre longtemps) ──");
                    await CommandRunner.RunAsync($"winget import -i \"{json}\" --accept-package-agreements --accept-source-agreements --ignore-unavailable --disable-interactivity", log, ct);
                    break;
            }
        }
        log("[OK] Restauration terminée.");
    }
}
