using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ScriptsWindows.Core;
using ScriptsWindows.Services;

namespace ScriptsWindows.Views.Controls;

public class CriticalEventItem
{
    public string Time { get; set; } = "—";
    public string Provider { get; set; } = "Système";
    public string Message { get; set; } = "";
}

public class RamProcessItem
{
    public int Rank { get; set; } = 1;
    public string Name { get; set; } = "";
    public int Pid { get; set; } = 0;
    public double MemoryMb { get; set; } = 0;

    public string MemoryMbText => MemoryMb >= 1024 
        ? $"{(MemoryMb / 1024):F2} Go" 
        : $"{MemoryMb:F0} Mo";
}

public partial class SecurityCenterDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;

    private readonly SolidColorBrush _brushTabActive = new(Color.FromRgb(79, 70, 229));    // #4F46E5
    private readonly SolidColorBrush _brushTabInactive = new(Colors.Transparent);
    private readonly SolidColorBrush _brushTextActive = new(Color.FromRgb(249, 250, 251));  // #F9FAFB
    private readonly SolidColorBrush _brushTextInactive = new(Color.FromRgb(156, 163, 175)); // #9CA3AF

    private readonly SolidColorBrush _brushSuccess = new(Color.FromRgb(16, 185, 129));      // #10B981
    private readonly SolidColorBrush _brushError = new(Color.FromRgb(239, 68, 68));          // #EF4444

    private readonly ObservableCollection<CriticalEventItem> _events = new();
    private readonly ObservableCollection<RamProcessItem> _ramProcesses = new();

    public SecurityCenterDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput;
        _ct = ct;

        ItemsEventsList.ItemsSource = _events;
        ItemsRamList.ItemsSource = _ramProcesses;

        Loaded += async (_, _) =>
        {
            await LoadDefenderStatusAsync();
        };
    }

    // ─────────────────────────────────────────────────────────────────────────
    // NAVIGATION PAR ONGLETS
    // ─────────────────────────────────────────────────────────────────────────
    private async void TabButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        string tag = btn.Tag?.ToString() ?? "";

        BtnTabDefender.Background = _brushTabInactive;
        BtnTabDefender.Foreground = _brushTextInactive;
        BtnTabEvents.Background = _brushTabInactive;
        BtnTabEvents.Foreground = _brushTextInactive;
        BtnTabRam.Background = _brushTabInactive;
        BtnTabRam.Foreground = _brushTextInactive;

        btn.Background = _brushTabActive;
        btn.Foreground = _brushTextActive;

        PanelDefender.Visibility = (tag == "TabDefender") ? Visibility.Visible : Visibility.Collapsed;
        PanelEvents.Visibility   = (tag == "TabEvents")   ? Visibility.Visible : Visibility.Collapsed;
        PanelRam.Visibility      = (tag == "TabRam")      ? Visibility.Visible : Visibility.Collapsed;

        if (tag == "TabEvents" && _events.Count == 0)
        {
            await LoadCriticalEventsAsync();
        }
        else if (tag == "TabRam" && _ramProcesses.Count == 0)
        {
            await LoadRamProcessesAsync();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // ONGLET 1 : PROTECTION DEFENDER
    // ─────────────────────────────────────────────────────────────────────────
    private async Task LoadDefenderStatusAsync()
    {
        TxtFeedback.Text = "Interrogation de Windows Defender...";
        TxtRealtimeStatus.Text = "Vérification...";
        BadgeRealtime.Background = new SolidColorBrush(Color.FromRgb(55, 65, 81));
        TxtSignaturesDate.Text = "Lecture...";
        TxtEngineVersion.Text = "Lecture...";
        TxtScanStatus.Text = "Lecture...";

        try
        {
            string ps = @"
try {
    $def = Get-MpComputerStatus -ErrorAction Stop
    [PSCustomObject]@{
        RealTime = $def.RealTimeProtectionEnabled
        Signatures = $def.AntivirusSignatureLastUpdated.ToString('dd/MM/yyyy HH:mm')
        Engine = $def.AMEngineVersion
        Overdue = $def.QuickScanOverdue
        Available = $true
    } | ConvertTo-Json -Compress
} catch {
    [PSCustomObject]@{ Available = $false } | ConvertTo-Json -Compress
}
";
            string json = await PowerShellRunner.RunAsync(ps, _ct);
            json = json.Trim();

            if (!string.IsNullOrWhiteSpace(json) && json.StartsWith("{"))
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                bool available = root.TryGetProperty("Available", out var av) && av.GetBoolean();

                if (available)
                {
                    bool rt = root.TryGetProperty("RealTime", out var r) && r.GetBoolean();
                    string sigs = root.TryGetProperty("Signatures", out var s) ? s.GetString() ?? "—" : "—";
                    string eng = root.TryGetProperty("Engine", out var eg) ? eg.GetString() ?? "—" : "—";
                    bool overdue = root.TryGetProperty("Overdue", out var od) && od.GetBoolean();

                    TxtRealtimeStatus.Text = rt ? "ACTIVÉE (Protégé)" : "DÉSACTIVÉE (Risque)";
                    BadgeRealtime.Background = rt ? _brushSuccess : _brushError;
                    TxtSignaturesDate.Text = sigs;
                    TxtEngineVersion.Text = eng;
                    TxtScanStatus.Text = overdue ? "Analyse recommandée (en retard)" : "Système analysé récemment";

                    TxtFeedback.Text = "État Windows Defender actualisé.";
                }
                else
                {
                    TxtRealtimeStatus.Text = "Non accessible";
                    BadgeRealtime.Background = _brushError;
                    TxtFeedback.Text = "Defender est géré par un tiers ou désactivé.";
                }
            }
        }
        catch (Exception ex)
        {
            TxtFeedback.Text = $"Erreur Defender : {ex.Message}";
        }
    }

    private async void BtnRefreshDefender_Click(object sender, RoutedEventArgs e)
    {
        await LoadDefenderStatusAsync();
    }

    private async void BtnQuickScan_Click(object sender, RoutedEventArgs e)
    {
        TxtFeedback.Text = "Scan rapide Defender lancé (voir console)...";
        await SecurityService.DefenderQuickScan(_onOutput, _ct);
        TxtFeedback.Text = "Scan rapide terminé.";
        await LoadDefenderStatusAsync();
    }

    private async void BtnUpdateSigs_Click(object sender, RoutedEventArgs e)
    {
        TxtFeedback.Text = "Mise à jour des signatures Defender...";
        await SecurityService.UpdateDefender(_onOutput, _ct);
        TxtFeedback.Text = "Signatures antivirus mises à jour.";
        await LoadDefenderStatusAsync();
    }

    private async void BtnThreats_Click(object sender, RoutedEventArgs e)
    {
        TxtFeedback.Text = "Recherche des menaces détectées...";
        await SecurityService.GetDefenderThreats(_onOutput, _ct);
        TxtFeedback.Text = "Historique des menaces affiché dans la console.";
    }

    private async void BtnEicar_Click(object sender, RoutedEventArgs e)
    {
        TxtFeedback.Text = "Création du fichier test EICAR...";
        await SecurityService.TestEicar(_onOutput, _ct);
        TxtFeedback.Text = "Fichier EICAR créé — vérifiez la réaction de l'antivirus.";
    }

    private void BtnOpenWindowsSecurity_Click(object sender, RoutedEventArgs e)
    {
        CommandRunner.StartProcess("windowsdefender:");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // ONGLET 2 : ERREURS & PLANTAGES (BSOD)
    // ─────────────────────────────────────────────────────────────────────────
    private async Task LoadCriticalEventsAsync()
    {
        TxtFeedback.Text = "Analyse du journal des erreurs Windows...";
        _events.Clear();
        TxtEventsCount.Text = "Recherche en cours...";

        try
        {
            string ps = @"
$since = (Get-Date).AddDays(-1)
$events = Get-WinEvent -FilterHashtable @{
    LogName   = 'System','Application'
    Level     = 1,2
    StartTime = $since
} -MaxEvents 25 -ErrorAction SilentlyContinue

$list = foreach ($e in $events) {
    [PSCustomObject]@{
        Time     = $e.TimeCreated.ToString('dd/MM HH:mm')
        Provider = $e.ProviderName
        Message  = ($e.Message -replace '\r?\n', ' ')
    }
}
$list | ConvertTo-Json -Compress
";
            string json = await PowerShellRunner.RunAsync(ps, _ct);
            json = json.Trim();

            if (!string.IsNullOrWhiteSpace(json))
            {
                if (json.StartsWith("["))
                {
                    using var doc = JsonDocument.Parse(json);
                    foreach (var el in doc.RootElement.EnumerateArray())
                    {
                        _events.Add(new CriticalEventItem
                        {
                            Time = el.GetProperty("Time").GetString() ?? "",
                            Provider = el.GetProperty("Provider").GetString() ?? "Système",
                            Message = el.GetProperty("Message").GetString() ?? ""
                        });
                    }
                }
                else if (json.StartsWith("{"))
                {
                    using var doc = JsonDocument.Parse(json);
                    var el = doc.RootElement;
                    _events.Add(new CriticalEventItem
                    {
                        Time = el.GetProperty("Time").GetString() ?? "",
                        Provider = el.GetProperty("Provider").GetString() ?? "Système",
                        Message = el.GetProperty("Message").GetString() ?? ""
                    });
                }
            }

            TxtEventsCount.Text = _events.Count > 0 
                ? $"{_events.Count} événement(s) critique(s) sur les dernières 24h"
                : "Aucune erreur critique détectée sur les dernières 24h [Sain]";
            TxtFeedback.Text = "Analyse des événements terminée.";
            _onOutput($"[OK] Journal des erreurs : {_events.Count} incident(s) répertorié(s)");
        }
        catch (Exception ex)
        {
            TxtEventsCount.Text = "Erreur lors de la lecture des événements.";
            TxtFeedback.Text = ex.Message;
        }
    }

    private async void BtnRefreshEvents_Click(object sender, RoutedEventArgs e)
    {
        await LoadCriticalEventsAsync();
    }

    private void BtnPerfmon_Click(object sender, RoutedEventArgs e)
    {
        CommandRunner.StartProcess("perfmon", "/rel");
    }

    private void BtnEventvwr_Click(object sender, RoutedEventArgs e)
    {
        CommandRunner.StartProcess("eventvwr.msc");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // ONGLET 3 : TOP PROCESSUS & CHARGE RAM
    // ─────────────────────────────────────────────────────────────────────────
    private async Task LoadRamProcessesAsync()
    {
        TxtFeedback.Text = "Analyse des processus et de la charge mémoire...";
        _ramProcesses.Clear();
        TxtRamTotalUsage.Text = "Mesure de la charge RAM...";

        try
        {
            string ps = @"
$os = Get-CimInstance Win32_OperatingSystem
$totalMb = [math]::Round($os.TotalVisibleMemorySize / 1024, 0)
$freeMb  = [math]::Round($os.FreePhysicalMemory / 1024, 0)
$usedMb  = $totalMb - $freeMb
$pct     = [math]::Round(($usedMb / $totalMb) * 100, 1)

$procs = Get-Process | Sort-Object WorkingSet64 -Descending | Select-Object -First 15
$list = foreach ($p in $procs) {
    [PSCustomObject]@{
        Name     = ""$($p.ProcessName).exe""
        Pid      = $p.Id
        MemoryMb = [math]::Round($p.WorkingSet64 / 1MB, 1)
    }
}

[PSCustomObject]@{
    TotalMb   = $totalMb
    UsedMb    = $usedMb
    Percent   = $pct
    Processes = $list
} | ConvertTo-Json -Compress
";
            string json = await PowerShellRunner.RunAsync(ps, _ct);
            json = json.Trim();

            if (!string.IsNullOrWhiteSpace(json) && json.StartsWith("{"))
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                double usedMb = root.TryGetProperty("UsedMb", out var u) ? u.GetDouble() : 0;
                double totalMb = root.TryGetProperty("TotalMb", out var t) ? t.GetDouble() : 0;
                double pct = root.TryGetProperty("Percent", out var p) ? p.GetDouble() : 0;

                TxtRamTotalUsage.Text = $"{(usedMb / 1024):F1} Go / {(totalMb / 1024):F1} Go utilisés ({pct} % de charge)";

                if (root.TryGetProperty("Processes", out var pArray) && pArray.ValueKind == JsonValueKind.Array)
                {
                    int rank = 1;
                    foreach (var el in pArray.EnumerateArray())
                    {
                        _ramProcesses.Add(new RamProcessItem
                        {
                            Rank = rank++,
                            Name = el.GetProperty("Name").GetString() ?? "",
                            Pid = el.GetProperty("Pid").GetInt32(),
                            MemoryMb = el.GetProperty("MemoryMb").GetDouble()
                        });
                    }
                }

                TxtFeedback.Text = "Top 15 processus RAM chargé.";
                _onOutput($"[OK] Charge mémoire : {(usedMb / 1024):F1} Go / {(totalMb / 1024):F1} Go ({pct} %)");
            }
        }
        catch (Exception ex)
        {
            TxtRamTotalUsage.Text = "Erreur lecture RAM";
            TxtFeedback.Text = ex.Message;
        }
    }

    private async void BtnRefreshRam_Click(object sender, RoutedEventArgs e)
    {
        await LoadRamProcessesAsync();
    }

    private void BtnTaskMgr_Click(object sender, RoutedEventArgs e)
    {
        CommandRunner.StartProcess("taskmgr");
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
