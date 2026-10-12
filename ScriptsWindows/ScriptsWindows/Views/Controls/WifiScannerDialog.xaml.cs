using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Controls;

public class WifiNetworkItem
{
    public string Ssid { get; set; } = "Réseau masqué";
    public string Bssid { get; set; } = "—";
    public int SignalPercent { get; set; } = 0;
    public string Channel { get; set; } = "Auto";
    public string AuthShort { get; set; } = "WPA2";

    public string SignalPercentText => $"{SignalPercent} %";

    public string SignalIcon => SignalPercent switch
    {
        >= 75 => "📶",
        >= 45 => "📶",
        >= 20 => "📳",
        _     => "⚠️"
    };

    public SolidColorBrush SignalColor => SignalPercent switch
    {
        >= 70 => new SolidColorBrush(Color.FromRgb(16, 185, 129)),  // #10B981 (Vert)
        >= 40 => new SolidColorBrush(Color.FromRgb(245, 158, 11)),  // #F59E0B (Jaune/Orange)
        _     => new SolidColorBrush(Color.FromRgb(239, 68, 68))   // #EF4444 (Rouge)
    };

    public Visibility TagVisibility => string.IsNullOrWhiteSpace(AuthShort) ? Visibility.Collapsed : Visibility.Visible;
}

public partial class WifiScannerDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;
    private readonly ObservableCollection<WifiNetworkItem> _networks = new();

    public WifiScannerDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput;
        _ct = ct;
        ItemsWifiList.ItemsSource = _networks;

        Loaded += async (_, _) => await ScanWifiAsync();
    }

    private async Task ScanWifiAsync()
    {
        BtnRefresh.IsEnabled = false;
        TxtSummary.Text = "Scan des réseaux sans fil en cours...";
        TxtFeedback.Text = "Interrogation de la carte Wi-Fi...";
        _networks.Clear();

        try
        {
            string ps = @"
$raw = netsh wlan show networks mode=Bssid
$list = [System.Collections.Generic.List[PSCustomObject]]::new()
$currentSsid = ''
$currentAuth = ''

foreach ($line in ($raw -split '`r?`n')) {
    $trimmed = $line.Trim()
    if ($trimmed -match '^SSID\s+\d+\s*:\s*(.*)$') {
        $name = $matches[1].Trim()
        $currentSsid = if ($name) { $name } else { 'Réseau masqué' }
    }
    elseif ($trimmed -match '^Authentification\s*:\s*(.*)$' -or $trimmed -match '^Authentication\s*:\s*(.*)$') {
        $currentAuth = $matches[1].Trim()
    }
    elseif ($trimmed -match '^BSSID\s+\d+\s*:\s*(.*)$') {
        $bssid = $matches[1].Trim()
        $signal = 0
        $channel = '—'
    }
    elseif ($trimmed -match '^Signal\s*:\s*(\d+)%') {
        $signal = [int]$matches[1]
    }
    elseif ($trimmed -match '^Canal\s*:\s*(\d+)' -or $trimmed -match '^Channel\s*:\s*(\d+)') {
        $channel = $matches[1]
        
        $authLabel = if ($currentAuth -match 'WPA3') { 'WPA3' }
                     elseif ($currentAuth -match 'WPA2') { 'WPA2' }
                     elseif ($currentAuth -match 'Ouvert' -or $currentAuth -match 'Open') { 'Ouvert' }
                     else { $currentAuth }

        $list.Add([PSCustomObject]@{
            Ssid = $currentSsid
            Bssid = $bssid
            SignalPercent = $signal
            Channel = $channel
            AuthShort = $authLabel
        })
    }
}
$list | Sort-Object SignalPercent -Descending | ConvertTo-Json -Compress
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
                        _networks.Add(new WifiNetworkItem
                        {
                            Ssid = el.TryGetProperty("Ssid", out var s) ? s.GetString() ?? "Réseau sans fil" : "Réseau sans fil",
                            Bssid = el.TryGetProperty("Bssid", out var b) ? b.GetString() ?? "—" : "—",
                            SignalPercent = el.TryGetProperty("SignalPercent", out var sp) ? sp.GetInt32() : 0,
                            Channel = el.TryGetProperty("Channel", out var ch) ? ch.GetString() ?? "—" : "—",
                            AuthShort = el.TryGetProperty("AuthShort", out var a) ? a.GetString() ?? "WPA2" : "WPA2"
                        });
                    }
                }
                else if (json.StartsWith("{"))
                {
                    using var doc = JsonDocument.Parse(json);
                    var el = doc.RootElement;
                    _networks.Add(new WifiNetworkItem
                    {
                        Ssid = el.TryGetProperty("Ssid", out var s) ? s.GetString() ?? "Réseau sans fil" : "Réseau sans fil",
                        Bssid = el.TryGetProperty("Bssid", out var b) ? b.GetString() ?? "—" : "—",
                        SignalPercent = el.TryGetProperty("SignalPercent", out var sp) ? sp.GetInt32() : 0,
                        Channel = el.TryGetProperty("Channel", out var ch) ? ch.GetString() ?? "—" : "—",
                        AuthShort = el.TryGetProperty("AuthShort", out var a) ? a.GetString() ?? "WPA2" : "WPA2"
                    });
                }
            }

            TxtCount.Text = $"{_networks.Count} réseau(x)";
            TxtSummary.Text = _networks.Count > 0 
                ? $"Détection terminée : {_networks.Count} borne(s) Wi-Fi à portée."
                : "Aucun réseau Wi-Fi détecté. Vérifiez que le Wi-Fi est activé.";
            TxtFeedback.Text = "Scan terminé avec succès.";
            _onOutput($"[OK] Scan Wi-Fi graphique : {_networks.Count} réseau(x) trouvé(s)");
        }
        catch (Exception ex)
        {
            TxtSummary.Text = "Erreur lors du scan Wi-Fi.";
            TxtFeedback.Text = ex.Message;
            _onOutput($"[ERR] Échec du scan Wi-Fi : {ex.Message}");
        }
        finally
        {
            BtnRefresh.IsEnabled = true;
        }
    }

    private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
    {
        await ScanWifiAsync();
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
