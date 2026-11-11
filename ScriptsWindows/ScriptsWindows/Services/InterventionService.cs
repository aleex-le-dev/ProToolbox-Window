using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using ScriptsWindows.Core;

namespace ScriptsWindows.Services;

/// <summary>Photographie de l'état du PC (avant / après intervention).</summary>
public class PcSnapshot
{
    public string ComputerName { get; set; } = "";
    public string Manufacturer { get; set; } = "";
    public string Model { get; set; } = "";
    public string Serial { get; set; } = "";
    public string Os { get; set; } = "";
    public string OsVersion { get; set; } = "";
    public string InstallDate { get; set; } = "";
    public string Uptime { get; set; } = "";
    public string Cpu { get; set; } = "";
    public double RamGb { get; set; }
    public string Activation { get; set; } = "";
    public double SysFreeGb { get; set; }
    public double SysTotalGb { get; set; }
    public List<string> Disks { get; set; } = new();
    public bool DefenderOn { get; set; }
    public int SignatureAgeDays { get; set; } = -1;
    public int QuickScanAgeDays { get; set; } = -1;
    public bool RebootPending { get; set; }
    public int ProblemDevices { get; set; }
    public int Crashes30 { get; set; }
    public int UnexpectedShutdowns30 { get; set; }
    public int BatteryWearPct { get; set; } = -1;
    public bool HasBattery { get; set; }
    public DateTime TakenAt { get; set; } = DateTime.Now;
}

/// <summary>Résultat d'une opération de l'intervention (pour le rapport).</summary>
public class StepResult
{
    public string Label { get; set; } = "";
    public string Status { get; set; } = "";   // OK, Avertissements, Échec, Annulé
    public TimeSpan Duration { get; set; }
}

/// <summary>
/// Intervention technicien : état du PC avant/après, exécution d'opérations de maintenance
/// et génération d'un rapport client HTML (imprimable / exportable en PDF depuis le navigateur).
/// </summary>
public static class InterventionService
{
    public static async Task<PcSnapshot> TakeSnapshotAsync(CancellationToken ct = default)
    {
        string json = await PowerShellRunner.RunAsync(@"
$cs = Get-CimInstance Win32_ComputerSystem
$os = Get-CimInstance Win32_OperatingSystem
$bios = Get-CimInstance Win32_BIOS
$cpu = Get-CimInstance Win32_Processor | Select-Object -First 1
$cv = Get-ItemProperty 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion' -ErrorAction SilentlyContinue
$sys = Get-CimInstance Win32_LogicalDisk -Filter ""DeviceID='$env:SystemDrive'""
$up = (Get-Date) - $os.LastBootUpTime

$lic = Get-CimInstance SoftwareLicensingProduct -Filter ""PartialProductKey IS NOT NULL AND Name LIKE 'Windows%'"" -ErrorAction SilentlyContinue | Select-Object -First 1
$activation = if (-not $lic) { 'Inconnue' } elseif ($lic.LicenseStatus -eq 1) { 'Activé' } else { 'Non activé' }

$mp = Get-MpComputerStatus -ErrorAction SilentlyContinue
$pending = (Test-Path 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update\RebootRequired') -or
           (Test-Path 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Component Based Servicing\RebootPending')
$since = (Get-Date).AddDays(-30)
$bsod = @(Get-WinEvent -FilterHashtable @{ LogName='System'; ProviderName='Microsoft-Windows-WER-SystemErrorReporting'; Id=1001; StartTime=$since } -ErrorAction SilentlyContinue).Count
$kp = @(Get-WinEvent -FilterHashtable @{ LogName='System'; ProviderName='Microsoft-Windows-Kernel-Power'; Id=41; StartTime=$since } -ErrorAction SilentlyContinue).Count
$dev = @(Get-CimInstance Win32_PnPEntity -ErrorAction SilentlyContinue | Where-Object { $_.ConfigManagerErrorCode -ne 0 -and $_.ConfigManagerErrorCode -ne 45 }).Count

$hasBattery = [bool](Get-CimInstance Win32_Battery -ErrorAction SilentlyContinue)
$wear = -1
try {
    $design = (Get-CimInstance -Namespace root\wmi -ClassName BatteryStaticData -ErrorAction Stop | Select-Object -First 1).DesignedCapacity
    $full = (Get-CimInstance -Namespace root\wmi -ClassName BatteryFullChargedCapacity -ErrorAction Stop | Select-Object -First 1).FullChargedCapacity
    if ($design -gt 0 -and $full -gt 0) { $wear = [math]::Max(0, [math]::Round(100 - ($full / $design * 100))) }
} catch { }

$disks = @(Get-PhysicalDisk -ErrorAction SilentlyContinue | ForEach-Object {
    $h = switch (""$($_.HealthStatus)"") { 'Healthy' { 'sain' } 'Warning' { 'à surveiller' } 'Unhealthy' { 'défaillant' } default { ""$($_.HealthStatus)"" } }
    ""$($_.FriendlyName) — $($_.MediaType) $([math]::Round($_.Size / 1GB)) Go — santé : $h""
})

[PSCustomObject]@{
    ComputerName = $env:COMPUTERNAME
    Manufacturer = ""$($cs.Manufacturer)""
    Model = ""$($cs.Model)""
    Serial = ""$($bios.SerialNumber)""
    Os = ""$($os.Caption)""
    OsVersion = ""$($cv.DisplayVersion) (build $($os.BuildNumber))""
    InstallDate = $os.InstallDate.ToString('dd/MM/yyyy')
    Uptime = ""$([int]$up.TotalDays) j $($up.Hours) h""
    Cpu = ""$($cpu.Name)"".Trim()
    RamGb = [math]::Round($cs.TotalPhysicalMemory / 1GB, 1)
    Activation = $activation
    SysFreeGb = [math]::Round($sys.FreeSpace / 1GB, 1)
    SysTotalGb = [math]::Round($sys.Size / 1GB, 1)
    Disks = $disks
    DefenderOn = [bool]($mp -and $mp.AntivirusEnabled -and $mp.RealTimeProtectionEnabled)
    SignatureAgeDays = if ($mp) { [int]$mp.AntivirusSignatureAge } else { -1 }
    QuickScanAgeDays = if ($mp -and $mp.QuickScanAge -lt 4000) { [int]$mp.QuickScanAge } else { -1 }
    RebootPending = [bool]$pending
    ProblemDevices = $dev
    Crashes30 = $bsod
    UnexpectedShutdowns30 = $kp
    BatteryWearPct = [int]$wear
    HasBattery = $hasBattery
} | ConvertTo-Json -Compress -Depth 3
", ct);

        var snap = JsonSerializer.Deserialize<PcSnapshot>(json.Trim()) ?? new PcSnapshot();
        snap.TakenAt = DateTime.Now;
        return snap;
    }

    /// <summary>Dossier des rapports : Documents\Alex Toolkit\Rapports.</summary>
    public static string ReportsFolder => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Alex Toolkit", "Rapports");

    public static string BuildReportHtml(
        string client, string technician, string reason, string conclusion,
        PcSnapshot before, PcSnapshot? after, IReadOnlyList<StepResult> steps, IReadOnlyList<string> log)
    {
        static string E(string? s) => WebUtility.HtmlEncode(s ?? "");
        var now = after?.TakenAt ?? before.TakenAt;
        var sb = new StringBuilder();

        string Row(string label, string value) => $"<tr><th>{E(label)}</th><td>{value}</td></tr>";
        string Badge(bool good, string text) => $"<span class=\"badge {(good ? "ok" : "ko")}\">{E(text)}</span>";
        string Days(int d) => d < 0 ? "—" : d == 0 ? "aujourd'hui" : $"il y a {d} j";

        // Comparaison avant / après
        string Compare(string label, string vBefore, string? vAfter, bool improved = false)
            => $"<tr><th>{E(label)}</th><td>{vBefore}</td><td>{(vAfter == null ? "—" : vAfter)}{(improved ? " <span class=\"up\">▲</span>" : "")}</td></tr>";

        sb.Append("<!doctype html><html lang=\"fr\"><head><meta charset=\"utf-8\"><title>Rapport d'intervention — ")
          .Append(E(before.ComputerName)).Append("</title><style>")
          .Append(@"
:root{--ink:#1f2328;--muted:#59636e;--line:#e2e5ea;--accent:#2563eb;--ok:#1a7f37;--ko:#cf222e;--warn:#9a6700;--bg:#f5f6f8}
*{box-sizing:border-box}body{margin:0;background:var(--bg);color:var(--ink);font:14px/1.5 'Segoe UI',system-ui,sans-serif}
.page{max-width:900px;margin:24px auto;background:#fff;border:1px solid var(--line);border-radius:10px;padding:40px 48px}
header{display:flex;justify-content:space-between;align-items:flex-start;border-bottom:2px solid var(--ink);padding-bottom:16px;margin-bottom:24px}
.brand{display:flex;gap:12px;align-items:center}.logo{width:40px;height:40px;border-radius:9px;background:var(--accent);color:#fff;font:700 20px 'Segoe UI';display:grid;place-items:center}
h1{font-size:22px;margin:0}.sub{color:var(--muted);font-size:13px}.meta{text-align:right;font-size:13px;color:var(--muted)}
h2{font-size:15px;text-transform:uppercase;letter-spacing:.04em;color:var(--muted);margin:28px 0 10px;border-bottom:1px solid var(--line);padding-bottom:6px}
table{width:100%;border-collapse:collapse}th,td{text-align:left;padding:7px 10px;border-bottom:1px solid var(--line);vertical-align:top}
th{width:34%;font-weight:600;color:var(--muted)}thead th{color:var(--ink);background:var(--bg)}
.grid{display:grid;grid-template-columns:1fr 1fr;gap:0 28px}
.badge{display:inline-block;padding:1px 8px;border-radius:999px;font-size:12px;font-weight:600}.ok{background:#dafbe1;color:var(--ok)}.ko{background:#ffebe9;color:var(--ko)}.wa{background:#fff8c5;color:var(--warn)}
.up{color:var(--ok);font-size:11px}.note{white-space:pre-wrap;border:1px solid var(--line);border-radius:8px;padding:12px 14px;min-height:48px}
.log{font:12px/1.45 Consolas,monospace;background:#16191d;color:#d4d9df;border-radius:8px;padding:12px 14px;white-space:pre-wrap;max-height:none}
.sign{display:grid;grid-template-columns:1fr 1fr;gap:28px;margin-top:36px}.sign div{border-top:1px solid var(--ink);padding-top:6px;font-size:13px;color:var(--muted);height:90px}
footer{margin-top:28px;font-size:11.5px;color:var(--muted);text-align:center}
@media print{body{background:#fff}.page{border:0;margin:0;padding:0;max-width:none}.log{max-height:none}h2{break-after:avoid}}
</style></head><body><div class=""page"">");

        sb.Append("<header><div class=\"brand\"><div class=\"logo\">A</div><div><h1>Rapport d'intervention</h1><div class=\"sub\">Maintenance informatique — Alex Toolkit</div></div></div>")
          .Append($"<div class=\"meta\">{now:dddd d MMMM yyyy}<br>{now:HH:mm}<br>Réf. {E(before.ComputerName)}-{now:yyyyMMdd-HHmm}</div></header>");

        sb.Append("<div class=\"grid\"><div><h2>Client</h2><table>")
          .Append(Row("Client", E(client).Length > 0 ? E(client) : "—"))
          .Append(Row("Technicien", E(technician).Length > 0 ? E(technician) : "—"))
          .Append("</table></div><div><h2>Motif</h2><div class=\"note\">").Append(E(string.IsNullOrWhiteSpace(reason) ? "Maintenance préventive" : reason)).Append("</div></div></div>");

        sb.Append("<h2>Machine</h2><div class=\"grid\"><table>")
          .Append(Row("Nom", E(before.ComputerName)))
          .Append(Row("Fabricant / modèle", E(before.Model.StartsWith(before.Manufacturer, StringComparison.OrdinalIgnoreCase) ? before.Model : $"{before.Manufacturer} {before.Model}")))
          .Append(Row("N° de série", E(before.Serial)))
          .Append(Row("Processeur", E(before.Cpu)))
          .Append(Row("Mémoire vive", $"{before.RamGb:0.#} Go"))
          .Append("</table><table>")
          .Append(Row("Système", E($"{before.Os} {before.OsVersion}")))
          .Append(Row("Installé le", E(before.InstallDate)))
          .Append(Row("Licence Windows", Badge(before.Activation == "Activé", before.Activation)))
          .Append(Row("Disques", string.Join("<br>", before.Disks.Select(E))))
          .Append(Row("Batterie (usure)", !before.HasBattery && before.BatteryWearPct < 0 ? "Pas de batterie"
                                         : before.BatteryWearPct < 0 ? "Non mesurable"
                                         : Badge(before.BatteryWearPct < 30, $"{before.BatteryWearPct} %")))
          .Append("</table></div>");

        sb.Append("<h2>État du système</h2><table><thead><tr><th>Indicateur</th><th>Avant</th><th>Après</th></tr></thead><tbody>");
        sb.Append(Compare("Espace libre sur le disque système",
            $"{before.SysFreeGb:0.#} Go / {before.SysTotalGb:0} Go",
            after == null ? null : $"{after.SysFreeGb:0.#} Go (+{Math.Max(0, after.SysFreeGb - before.SysFreeGb):0.##} Go)",
            after != null && after.SysFreeGb - before.SysFreeGb >= 0.1));
        sb.Append(Compare("Protection antivirus", Badge(before.DefenderOn, before.DefenderOn ? "Active" : "Inactive"),
            after == null ? null : Badge(after.DefenderOn, after.DefenderOn ? "Active" : "Inactive")));
        sb.Append(Compare("Définitions antivirus", Days(before.SignatureAgeDays), after == null ? null : Days(after.SignatureAgeDays),
            after != null && after.SignatureAgeDays >= 0 && after.SignatureAgeDays < before.SignatureAgeDays));
        sb.Append(Compare("Dernière analyse antivirus", Days(before.QuickScanAgeDays), after == null ? null : Days(after.QuickScanAgeDays),
            after != null && after.QuickScanAgeDays >= 0 && (before.QuickScanAgeDays < 0 || after.QuickScanAgeDays < before.QuickScanAgeDays)));
        sb.Append(Compare("Périphériques en erreur", Badge(before.ProblemDevices == 0, before.ProblemDevices.ToString()),
            after == null ? null : Badge(after.ProblemDevices == 0, after.ProblemDevices.ToString())));
        sb.Append(Compare("Écrans bleus (30 jours)", Badge(before.Crashes30 == 0, before.Crashes30.ToString()), null));
        sb.Append(Compare("Arrêts brutaux (30 jours)", Badge(before.UnexpectedShutdowns30 == 0, before.UnexpectedShutdowns30.ToString()), null));
        sb.Append(Compare("Redémarrage en attente", before.RebootPending ? Badge(false, "Oui") : "Non",
            after == null ? null : after.RebootPending ? Badge(false, "Oui — redémarrer le PC") : "Non"));
        sb.Append(Compare("Démarré depuis", E(before.Uptime), null));
        sb.Append("</tbody></table>");

        if (steps.Count > 0)
        {
            sb.Append("<h2>Opérations réalisées</h2><table><thead><tr><th>Opération</th><th>Résultat</th><th>Durée</th></tr></thead><tbody>");
            foreach (var s in steps)
            {
                string cls = s.Status == "OK" ? "ok" : s.Status == "Avertissements" ? "wa" : "ko";
                string dur = s.Duration.TotalMinutes >= 1 ? $"{(int)s.Duration.TotalMinutes} min {s.Duration.Seconds:00} s" : $"{s.Duration.Seconds} s";
                sb.Append($"<tr><th>{E(s.Label)}</th><td><span class=\"badge {cls}\">{E(s.Status)}</span></td><td>{dur}</td></tr>");
            }
            sb.Append("</tbody></table>");
        }

        sb.Append("<h2>Conclusion et recommandations</h2><div class=\"note\">")
          .Append(E(string.IsNullOrWhiteSpace(conclusion) ? "—" : conclusion)).Append("</div>");

        if (log.Count > 0)
        {
            sb.Append("<h2>Journal technique</h2><div class=\"log\">");
            foreach (var line in log.TakeLast(400)) sb.Append(E(line)).Append('\n');
            sb.Append("</div>");
        }

        sb.Append("<div class=\"sign\"><div>Signature du technicien</div><div>Signature du client</div></div>")
          .Append($"<footer>Rapport généré par Alex Toolkit v{UpdateService.GetCurrentVersion()} le {now:dd/MM/yyyy à HH:mm}. Pour un PDF : Ctrl + P puis « Enregistrer au format PDF ».</footer>")
          .Append("</div></body></html>");

        return sb.ToString();
    }

    public static string SaveReport(string html, string computerName)
    {
        Directory.CreateDirectory(ReportsFolder);
        string safe = string.Concat(computerName.Where(c => !Path.GetInvalidFileNameChars().Contains(c)));
        string path = Path.Combine(ReportsFolder, $"Intervention_{safe}_{DateTime.Now:yyyy-MM-dd_HH-mm}.html");
        File.WriteAllText(path, html, new UTF8Encoding(true));
        return path;
    }
}
