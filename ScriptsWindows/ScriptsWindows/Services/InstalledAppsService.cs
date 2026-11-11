using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using ScriptsWindows.Core;

namespace ScriptsWindows.Services;

/// <summary>Logiciel installé (registre de désinstallation ou Microsoft Store).</summary>
public class InstalledApp
{
    public string Name { get; set; } = "";
    public string Version { get; set; } = "";
    public string Publisher { get; set; } = "";
    public string InstallDateRaw { get; set; } = "";
    public double SizeMB { get; set; }
    public string Location { get; set; } = "";
    public string Uninstall { get; set; } = "";
    public string Source { get; set; } = "";
    public string PackageFullName { get; set; } = "";

    public DateTime? InstallDate =>
        DateTime.TryParseExact(InstallDateRaw, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d) ? d
        : DateTime.TryParse(InstallDateRaw, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d2) ? d2 : null;

    public bool IsStore => Source == "Microsoft Store";
    public string SizeText => SizeMB <= 0 ? "—" : SizeMB >= 1024 ? $"{SizeMB / 1024:0.0} Go" : $"{SizeMB:0} Mo";
    public string DateText => InstallDate?.ToString("dd/MM/yyyy") ?? "—";
    public string Details => string.Join(" • ", new[]
    {
        string.IsNullOrWhiteSpace(Publisher) ? null : Publisher,
        string.IsNullOrWhiteSpace(Version) ? null : $"v{Version}",
        InstallDate == null ? null : $"installé le {DateText}",
        Source
    }.Where(s => s != null));
    public bool HasLocation => !string.IsNullOrWhiteSpace(Location) && Directory.Exists(Location);
    public bool CanUninstall => IsStore ? PackageFullName.Length > 0 : Uninstall.Length > 0;
}

/// <summary>
/// Inventaire complet des logiciels installés, à partir des mêmes sources que « Applications et fonctionnalités » :
/// clés de désinstallation (machine 64 / 32 bits, utilisateur) + applications du Microsoft Store.
/// </summary>
public static class InstalledAppsService
{
    public static async Task<List<InstalledApp>> GetAppsAsync(CancellationToken ct = default)
    {
        string json = await PowerShellRunner.RunAsync(@"
$sources = @(
    @{ Path = 'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*';             Label = 'Bureau 64 bits' },
    @{ Path = 'HKLM:\Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*'; Label = 'Bureau 32 bits' },
    @{ Path = 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*';             Label = 'Utilisateur' }
)
$list = New-Object System.Collections.Generic.List[object]
foreach ($s in $sources) {
    Get-ItemProperty $s.Path -ErrorAction SilentlyContinue | Where-Object {
        $_.DisplayName -and $_.SystemComponent -ne 1 -and -not $_.ParentKeyName -and
        $_.ReleaseType -notin @('Update','Hotfix','Security Update') -and $_.DisplayName -notmatch '\(KB\d+\)'
    } | ForEach-Object {
        $list.Add([PSCustomObject]@{
            Name = ""$($_.DisplayName)"".Trim(); Version = ""$($_.DisplayVersion)""; Publisher = ""$($_.Publisher)"".Trim()
            InstallDateRaw = ""$($_.InstallDate)""; SizeMB = if ($_.EstimatedSize) { [math]::Round($_.EstimatedSize / 1024, 1) } else { 0 }
            Location = ""$($_.InstallLocation)"".Trim('""', ' ')
            Uninstall = if ($_.QuietUninstallString) { ""$($_.QuietUninstallString)"" } else { ""$($_.UninstallString)"" }
            Source = $s.Label; PackageFullName = ''
        })
    }
}
# Applications du Microsoft Store installées par l'utilisateur (hors composants système)
Get-AppxPackage -ErrorAction SilentlyContinue | Where-Object { -not $_.IsFramework -and $_.SignatureKind -eq 'Store' -and -not $_.NonRemovable } | ForEach-Object {
    $date = try { (Get-Item $_.InstallLocation -ErrorAction Stop).CreationTime.ToString('yyyyMMdd') } catch { '' }
    $list.Add([PSCustomObject]@{
        Name = $_.Name; Version = ""$($_.Version)""; Publisher = (""$($_.Publisher)"" -replace '^CN=([^,]+).*$', '$1')
        InstallDateRaw = $date; SizeMB = 0; Location = ""$($_.InstallLocation)""; Uninstall = ''
        Source = 'Microsoft Store'; PackageFullName = $_.PackageFullName
    })
}
ConvertTo-Json -InputObject $list.ToArray() -Compress
", ct);

        json = json.Trim();
        if (json.Length == 0)
            throw new InvalidOperationException(PowerShellRunner.LastErrors.FirstOrDefault(e => !string.IsNullOrWhiteSpace(e)) ?? "Aucune donnée renvoyée.");
        if (json.StartsWith("{")) json = "[" + json + "]";
        var apps = JsonSerializer.Deserialize<List<InstalledApp>>(json) ?? new();
        foreach (var a in apps.Where(a => a.IsStore)) a.Name = DebloatService.FriendlyName(a.Name);

        // Doublons (même logiciel déclaré en 32 et 64 bits ou machine + utilisateur) : on garde la ligne la plus complète
        return apps.GroupBy(a => (a.Name.ToLowerInvariant(), a.Version))
                   .Select(g => g.OrderByDescending(a => a.SizeMB).ThenByDescending(a => a.Uninstall.Length).First())
                   .OrderBy(a => a.Name, StringComparer.CurrentCultureIgnoreCase)
                   .ToList();
    }

    /// <summary>Désinstalle : programme de désinstallation du logiciel (MSI → /X) ou Remove-AppxPackage (Store).</summary>
    public static Task UninstallAsync(InstalledApp app, Action<string> log, CancellationToken ct = default)
    {
        if (app.IsStore)
            return PowerShellRunner.RunAsync($"Remove-AppxPackage -Package '{app.PackageFullName.Replace("'", "''")}' -ErrorAction Stop; Write-Output '[OK] {app.Name.Replace("'", "''")} désinstallé.'", log, ct);

        string cmd = app.Uninstall.Trim();
        // « MsiExec.exe /I{GUID} » ouvre parfois l'assistant de modification : /X force la désinstallation
        var msi = System.Text.RegularExpressions.Regex.Match(cmd, @"msiexec(\.exe)?\s+/[IX]\s*(\{[0-9A-Fa-f\-]+\})", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        if (msi.Success) cmd = $"msiexec.exe /X{msi.Groups[2].Value}";
        log($"[i] Lancement du désinstalleur : {cmd}");
        // Le désinstalleur du logiciel s'affiche normalement (il peut poser des questions à l'utilisateur)
        return CommandRunner.RunAsync($"start \"\" /wait {cmd}", log, ct);
    }

    public static string ExportFolder => InterventionService.ReportsFolder;

    /// <summary>Export CSV (séparateur « ; » + BOM UTF-8 : s'ouvre directement dans Excel en français).</summary>
    public static string ExportCsv(IEnumerable<InstalledApp> apps)
    {
        Directory.CreateDirectory(ExportFolder);
        string path = Path.Combine(ExportFolder, $"Logiciels_{Environment.MachineName}_{DateTime.Now:yyyy-MM-dd_HH-mm}.csv");
        static string C(string s) => "\"" + (s ?? "").Replace("\"", "\"\"") + "\"";
        var sb = new StringBuilder("Nom;Éditeur;Version;Date d'installation;Taille (Mo);Origine;Emplacement\r\n");
        foreach (var a in apps)
            sb.Append(string.Join(";", C(a.Name), C(a.Publisher), C(a.Version), C(a.DateText == "—" ? "" : a.DateText),
                                       a.SizeMB > 0 ? a.SizeMB.ToString("0.#", CultureInfo.GetCultureInfo("fr-FR")) : "", C(a.Source), C(a.Location))).Append("\r\n");
        File.WriteAllText(path, sb.ToString(), new UTF8Encoding(true));
        return path;
    }

    /// <summary>Rapport HTML imprimable (même présentation que le rapport d'intervention).</summary>
    public static string ExportHtml(IReadOnlyList<InstalledApp> apps)
    {
        Directory.CreateDirectory(ExportFolder);
        string path = Path.Combine(ExportFolder, $"Logiciels_{Environment.MachineName}_{DateTime.Now:yyyy-MM-dd_HH-mm}.html");
        static string E(string? s) => WebUtility.HtmlEncode(s ?? "");
        double totalGb = apps.Sum(a => a.SizeMB) / 1024;
        var sb = new StringBuilder();
        sb.Append("<!doctype html><html lang=\"fr\"><head><meta charset=\"utf-8\"><title>Logiciels installés — ").Append(E(Environment.MachineName)).Append("</title><style>")
          .Append("body{margin:0;background:#f5f6f8;color:#1f2328;font:13px/1.45 'Segoe UI',sans-serif}.page{max-width:1000px;margin:24px auto;background:#fff;border:1px solid #e2e5ea;border-radius:10px;padding:36px 44px}")
          .Append("h1{font-size:21px;margin:0}.sub{color:#59636e;margin:4px 0 20px}table{width:100%;border-collapse:collapse}th,td{text-align:left;padding:6px 8px;border-bottom:1px solid #e2e5ea;vertical-align:top}")
          .Append("th{background:#f5f6f8;font-weight:600}td.n{text-align:right;white-space:nowrap}.src{color:#59636e;font-size:12px}@media print{body{background:#fff}.page{border:0;margin:0;padding:0}}</style></head><body><div class=\"page\">")
          .Append($"<h1>Logiciels installés — {E(Environment.MachineName)}</h1><div class=\"sub\">{apps.Count} logiciels • {totalGb:0.0} Go déclarés • inventaire du {DateTime.Now:dd/MM/yyyy à HH:mm} (Alex Toolkit)</div>")
          .Append("<table><thead><tr><th>Nom</th><th>Éditeur</th><th>Version</th><th>Installé le</th><th>Taille</th><th>Origine</th></tr></thead><tbody>");
        foreach (var a in apps)
            sb.Append($"<tr><td>{E(a.Name)}</td><td>{E(a.Publisher)}</td><td>{E(a.Version)}</td><td>{E(a.DateText)}</td><td class=\"n\">{E(a.SizeText)}</td><td class=\"src\">{E(a.Source)}</td></tr>");
        sb.Append("</tbody></table></div></body></html>");
        File.WriteAllText(path, sb.ToString(), new UTF8Encoding(true));
        return path;
    }
}
