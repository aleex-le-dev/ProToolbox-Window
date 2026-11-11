using System.Text.Json;
using System.Text.RegularExpressions;
using ScriptsWindows.Core;

namespace ScriptsWindows.Services;

/// <summary>Écran bleu (BugCheck) relevé dans le journal Windows.</summary>
public class CrashEvent
{
    public string Date { get; set; } = "";
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string Cause { get; set; } = "";
    public string DumpPath { get; set; } = "";
    public string Title => string.IsNullOrEmpty(Name) ? Code : $"{Name}  ({Code})";
}

/// <summary>Périphérique signalé en erreur par le Gestionnaire de périphériques.</summary>
public class ProblemDevice
{
    public string Name { get; set; } = "";
    public string Class { get; set; } = "";
    public string InstanceId { get; set; } = "";
    public int ErrorCode { get; set; }
    public string Explanation => TroubleshootService.DescribeDeviceError(ErrorCode);
    public string Subtitle => $"{(string.IsNullOrEmpty(Class) ? "Périphérique" : Class)} • Code {ErrorCode}";
}

/// <summary>
/// Analyse des pannes : écrans bleus (journal + minidumps) et périphériques en erreur,
/// avec une explication en français et la piste de résolution la plus probable.
/// </summary>
public static class TroubleshootService
{
    // Codes d'arrêt les plus fréquents → nom + cause probable
    private static readonly Dictionary<uint, (string Name, string Cause)> BugChecks = new()
    {
        [0x0A]  = ("IRQL_NOT_LESS_OR_EQUAL", "Pilote défectueux ou incompatible (souvent après une mise à jour de pilote)."),
        [0x19]  = ("BAD_POOL_HEADER", "Pilote ou mémoire vive (RAM) défaillante."),
        [0x1A]  = ("MEMORY_MANAGEMENT", "Mémoire vive (RAM) défaillante : lancer un test mémoire."),
        [0x1E]  = ("KMODE_EXCEPTION_NOT_HANDLED", "Pilote défectueux."),
        [0x3B]  = ("SYSTEM_SERVICE_EXCEPTION", "Pilote (souvent graphique) ou fichiers système : SFC/DISM puis mise à jour des pilotes."),
        [0x50]  = ("PAGE_FAULT_IN_NONPAGED_AREA", "RAM, pilote ou antivirus tiers."),
        [0x7A]  = ("KERNEL_DATA_INPAGE_ERROR", "Disque ou câble défaillant : vérifier la santé S.M.A.R.T. et lancer CHKDSK."),
        [0x7E]  = ("SYSTEM_THREAD_EXCEPTION_NOT_HANDLED", "Pilote défectueux."),
        [0x9F]  = ("DRIVER_POWER_STATE_FAILURE", "Pilote qui gère mal la veille / reprise (réseau, graphique, USB)."),
        [0xBE]  = ("ATTEMPTED_WRITE_TO_READONLY_MEMORY", "Pilote défectueux."),
        [0xC2]  = ("BAD_POOL_CALLER", "Pilote défectueux."),
        [0xD1]  = ("DRIVER_IRQL_NOT_LESS_OR_EQUAL", "Pilote défectueux (très souvent réseau / Wi-Fi)."),
        [0xEF]  = ("CRITICAL_PROCESS_DIED", "Fichiers système ou disque : SFC/DISM et santé du disque."),
        [0xF4]  = ("CRITICAL_OBJECT_TERMINATION", "Disque défaillant ou débranché."),
        [0x101] = ("CLOCK_WATCHDOG_TIMEOUT", "Processeur : overclocking, BIOS obsolète ou surchauffe."),
        [0x116] = ("VIDEO_TDR_FAILURE", "Pilote ou carte graphique : réinstaller le pilote graphique."),
        [0x119] = ("VIDEO_SCHEDULER_INTERNAL_ERROR", "Pilote graphique."),
        [0x124] = ("WHEA_UNCORRECTABLE_ERROR", "Erreur matérielle : surchauffe, overclocking, RAM, CPU ou alimentation."),
        [0x133] = ("DPC_WATCHDOG_VIOLATION", "Pilote de stockage ou firmware SSD obsolète."),
        [0x139] = ("KERNEL_SECURITY_CHECK_FAILURE", "Pilote incompatible ou RAM."),
        [0x13A] = ("KERNEL_MODE_HEAP_CORRUPTION", "Pilote défectueux (souvent graphique)."),
        [0x154] = ("UNEXPECTED_STORE_EXCEPTION", "Disque / SSD ou antivirus tiers."),
        [0x1E0] = ("ILLEGAL_IOMMU_PAGE_FAULT", "Pilote ou firmware (BIOS)."),
    };

    public static (string Name, string Cause) DescribeBugCheck(string code)
    {
        var m = Regex.Match(code ?? "", "0x([0-9a-fA-F]+)");
        if (m.Success && uint.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.HexNumber, null, out uint v)
            && BugChecks.TryGetValue(v, out var d))
            return d;
        return ("", "Code peu courant : rechercher le code d'arrêt sur learn.microsoft.com (« Bug Check Code Reference »).");
    }

    public static string DescribeDeviceError(int code) => code switch
    {
        1  => "Mal configuré : réinstaller le pilote.",
        3  => "Pilote corrompu ou mémoire insuffisante : réinstaller le pilote.",
        10 => "Le périphérique ne peut pas démarrer : mettre à jour ou réinstaller le pilote.",
        12 => "Conflit de ressources : désactiver un autre périphérique ou mettre à jour le BIOS.",
        14 => "Redémarrage nécessaire pour terminer l'installation.",
        18 => "Pilotes à réinstaller.",
        19 => "Configuration du registre endommagée : désinstaller puis réinstaller le périphérique.",
        21 => "Suppression en cours : redémarrer.",
        22 => "Périphérique désactivé : l'activer dans le Gestionnaire de périphériques.",
        24 => "Absent ou mal installé : vérifier le branchement et le pilote.",
        28 => "Aucun pilote installé : rechercher le pilote du fabricant.",
        31 => "Ne fonctionne pas correctement : réinstaller le pilote.",
        32 => "Service du pilote désactivé.",
        37 => "Échec d'initialisation du pilote : réinstaller le pilote.",
        39 => "Pilote manquant ou endommagé : réinstaller le pilote.",
        43 => "Arrêté après avoir signalé un problème : pilote ou matériel défaillant (fréquent sur USB et cartes graphiques).",
        48 => "Pilote bloqué pour incompatibilité connue : installer une version plus récente.",
        52 => "Signature du pilote invalide : installer le pilote officiel signé.",
        _  => "Problème signalé par Windows : réinstaller ou mettre à jour le pilote."
    };

    /// <summary>Écrans bleus des 90 derniers jours (évènement 1001 BugCheck) + nombre d'arrêts brutaux (Kernel-Power 41).</summary>
    public static async Task<(List<CrashEvent> Crashes, int UnexpectedShutdowns, int DumpCount)> GetCrashesAsync(CancellationToken ct = default)
    {
        string json = await PowerShellRunner.RunAsync(@"
$since = (Get-Date).AddDays(-90)
$bc = @(Get-WinEvent -FilterHashtable @{ LogName='System'; ProviderName='Microsoft-Windows-WER-SystemErrorReporting'; Id=1001; StartTime=$since } -MaxEvents 30 -ErrorAction SilentlyContinue |
    ForEach-Object {
        $msg = $_.Message
        $code = if ($msg -match '0x[0-9a-fA-F]{8}') { $Matches[0] } else { '' }
        $dump = if ($msg -match '([A-Za-z]:\\[^\s]+\.dmp)') { $Matches[1] } else { '' }
        [PSCustomObject]@{ Date = $_.TimeCreated.ToString('dd/MM/yyyy HH:mm'); Code = $code; DumpPath = $dump }
    })
$kp = @(Get-WinEvent -FilterHashtable @{ LogName='System'; ProviderName='Microsoft-Windows-Kernel-Power'; Id=41; StartTime=$since } -ErrorAction SilentlyContinue).Count
$dumps = @(Get-ChildItem ""$env:SystemRoot\Minidump"" -Filter *.dmp -ErrorAction SilentlyContinue).Count
[PSCustomObject]@{ Crashes = $bc; Shutdowns = $kp; Dumps = $dumps } | ConvertTo-Json -Compress -Depth 3
", ct);

        var crashes = new List<CrashEvent>();
        int shutdowns = 0, dumps = 0;
        try
        {
            using var doc = JsonDocument.Parse(json.Trim());
            var root = doc.RootElement;
            shutdowns = root.TryGetProperty("Shutdowns", out var s) && s.TryGetInt32(out int si) ? si : 0;
            dumps = root.TryGetProperty("Dumps", out var d) && d.TryGetInt32(out int di) ? di : 0;
            if (root.TryGetProperty("Crashes", out var arr) && arr.ValueKind == JsonValueKind.Array)
            {
                foreach (var c in arr.EnumerateArray())
                {
                    string code = c.GetProperty("Code").GetString() ?? "";
                    var (name, cause) = DescribeBugCheck(code);
                    crashes.Add(new CrashEvent
                    {
                        Date = c.GetProperty("Date").GetString() ?? "",
                        Code = string.IsNullOrEmpty(code) ? "Code inconnu" : code,
                        Name = name,
                        Cause = cause,
                        DumpPath = c.GetProperty("DumpPath").GetString() ?? ""
                    });
                }
            }
        }
        catch (JsonException) { }

        return (crashes, shutdowns, dumps);
    }

    /// <summary>Périphériques en erreur (code ≠ 0), hors code 45 « non connecté actuellement » qui n'est pas une panne.</summary>
    public static async Task<List<ProblemDevice>> GetProblemDevicesAsync(CancellationToken ct = default)
    {
        string json = await PowerShellRunner.RunAsync(@"
@(Get-CimInstance Win32_PnPEntity -ErrorAction SilentlyContinue |
    Where-Object { $_.ConfigManagerErrorCode -ne 0 -and $_.ConfigManagerErrorCode -ne 45 } |
    ForEach-Object {
        [PSCustomObject]@{
            Name = if ($_.Name) { $_.Name } else { 'Périphérique inconnu' }
            Class = ""$($_.PNPClass)""
            InstanceId = $_.PNPDeviceID
            ErrorCode = [int]$_.ConfigManagerErrorCode
        }
    }) | ConvertTo-Json -Compress
", ct);

        json = json.Trim();
        if (string.IsNullOrEmpty(json)) return new();
        if (json.StartsWith("{")) json = "[" + json + "]";
        try { return JsonSerializer.Deserialize<List<ProblemDevice>>(json) ?? new(); }
        catch (JsonException) { return new(); }
    }

    /// <summary>Relance la détection matérielle et l'installation automatique des pilotes (pnputil /scan-devices).</summary>
    public static Task ScanForHardwareChanges(Action<string> onOutput, CancellationToken ct = default)
        => CommandRunner.RunAsync("pnputil /scan-devices", onOutput, ct);

    /// <summary>Réinstalle un périphérique : suppression puis nouvelle détection (Windows réinstalle le pilote).</summary>
    public static Task ReinstallDevice(string instanceId, Action<string> onOutput, CancellationToken ct = default)
        => PowerShellRunner.RunAsync($@"
$id = '{instanceId.Replace("'", "''")}'
& pnputil /remove-device ""$id""
Start-Sleep -Seconds 2
& pnputil /scan-devices
Write-Output '[OK] Périphérique réinstallé : vérifiez son état avec « Actualiser ».'
", onOutput, ct);
}
