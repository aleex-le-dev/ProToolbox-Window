using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Controls;

public class LanDeviceItem
{
    public string IpAddress { get; set; } = "";
    public string HostName { get; set; } = "Inconnu";
    public string DeviceType { get; set; } = "Appareil réseau";

    public string DeviceIcon => DeviceType switch
    {
        "Passerelle / Box" => "🌐",
        "Cet ordinateur"   => "💻",
        _                  => "📱"
    };
}

public partial class LanScannerDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;
    private readonly ObservableCollection<LanDeviceItem> _devices = new();
    private string _detectedSubnet = "";

    public LanScannerDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput;
        _ct = ct;
        ItemsLanList.ItemsSource = _devices;

        Loaded += async (_, _) => await DetectSubnetAsync();
    }

    private async Task DetectSubnetAsync()
    {
        TxtSubnetInfo.Text = "Détection de la passerelle par défaut...";
        try
        {
            string ps = @"
$gw = (Get-NetRoute -DestinationPrefix '0.0.0.0/0' | Sort-Object RouteMetric | Select-Object -First 1).NextHop
if ($gw) {
    $sub = ($gw -replace '\.\d+$', '')
    [PSCustomObject]@{ Gateway = $gw; Subnet = $sub } | ConvertTo-Json -Compress
} else {
    '{}'
}
";
            string json = await PowerShellRunner.RunAsync(ps, _ct);
            json = json.Trim();

            if (!string.IsNullOrWhiteSpace(json) && json.StartsWith("{"))
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (root.TryGetProperty("Subnet", out var sub))
                {
                    _detectedSubnet = sub.GetString() ?? "";
                    string gw = root.TryGetProperty("Gateway", out var g) ? g.GetString() ?? "" : "";
                    TxtSubnetInfo.Text = $"Plage : {_detectedSubnet}.1 - 254 (Passerelle : {gw})";
                }
            }
            if (string.IsNullOrWhiteSpace(_detectedSubnet))
            {
                _detectedSubnet = "192.168.1";
                TxtSubnetInfo.Text = "Plage par défaut : 192.168.1.1 - 254";
            }
        }
        catch (Exception ex)
        {
            _detectedSubnet = "192.168.1";
            TxtSubnetInfo.Text = $"Sous-réseau estimé : 192.168.1.1 - 254 ({ex.Message})";
        }
    }

    private async void BtnStartScan_Click(object sender, RoutedEventArgs e)
    {
        BtnStartScan.IsEnabled = false;
        ProgressScan.IsIndeterminate = true;
        TxtFeedback.Text = $"Scan du sous-réseau {_detectedSubnet}.1-254 en cours...";
        _devices.Clear();
        TxtHostCount.Text = "0 appareil(s)";

        _onOutput($"\n[i] Démarrage du scan LAN graphique sur {_detectedSubnet}.1-254...");

        try
        {
            string ps = $@"
$subnet = '{_detectedSubnet}'
$gw = (Get-NetRoute -DestinationPrefix '0.0.0.0/0' | Sort-Object RouteMetric | Select-Object -First 1).NextHop
$myIp = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object {{ $_.IPAddress -like ""$subnet.*"" }} | Select-Object -First 1).IPAddress

$results = 1..254 | ForEach-Object -Parallel {{
    $ip = ""$using:subnet.$_""
    if (Test-Connection $ip -Count 1 -TimeoutSeconds 1 -Quiet) {{
        $name = try {{ [System.Net.Dns]::GetHostEntry($ip).HostName }} catch {{ 'Hôte sans nom DNS' }}
        $type = if ($ip -eq $using:gw) {{ 'Passerelle / Box' }}
                elseif ($ip -eq $using:myIp) {{ 'Cet ordinateur' }}
                else {{ 'Appareil connecté' }}

        [PSCustomObject]@{{
            IpAddress = $ip
            HostName  = $name
            DeviceType = $type
        }}
    }}
}} -ThrottleLimit 60

$results | Sort-Object {{ [version]$_.IpAddress }} | ConvertTo-Json -Compress
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
                        _devices.Add(new LanDeviceItem
                        {
                            IpAddress = el.GetProperty("IpAddress").GetString() ?? "",
                            HostName = el.GetProperty("HostName").GetString() ?? "Inconnu",
                            DeviceType = el.GetProperty("DeviceType").GetString() ?? "Appareil"
                        });
                    }
                }
                else if (json.StartsWith("{"))
                {
                    using var doc = JsonDocument.Parse(json);
                    var el = doc.RootElement;
                    _devices.Add(new LanDeviceItem
                    {
                        IpAddress = el.GetProperty("IpAddress").GetString() ?? "",
                        HostName = el.GetProperty("HostName").GetString() ?? "Inconnu",
                        DeviceType = el.GetProperty("DeviceType").GetString() ?? "Appareil"
                    });
                }
            }

            TxtHostCount.Text = $"{_devices.Count} appareil(s)";
            TxtFeedback.Text = $"Scan terminé : {_devices.Count} machine(s) active(s) trouvée(s).";
            _onOutput($"[OK] Scan LAN terminé : {_devices.Count} appareil(s) identifié(s) sur le réseau");
        }
        catch (Exception ex)
        {
            TxtFeedback.Text = $"Erreur lors du scan : {ex.Message}";
            _onOutput($"[ERR] Échec du scan LAN : {ex.Message}");
        }
        finally
        {
            ProgressScan.IsIndeterminate = false;
            ProgressScan.Value = 100;
            BtnStartScan.IsEnabled = true;
        }
    }

    private void BtnCopyIp_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string ip)
        {
            Clipboard.SetText(ip);
            TxtFeedback.Text = $"Adresse IP '{ip}' copiée dans le presse-papier.";
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
