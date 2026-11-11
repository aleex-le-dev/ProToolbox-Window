using System.Collections.ObjectModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;

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

/// <summary>
/// Scanner LAN natif (.NET) : ping ICMP parallèle sur le /24 local + résolution DNS inverse.
/// Aucun processus PowerShell lancé → compatible Windows PowerShell 5.1 / 7 et ~1-2 s par scan.
/// </summary>
public partial class LanScannerDialog : Window
{
    private const int PingTimeoutMs     = 1000;
    private const int MaxParallelPings  = 64;
    private const int DnsLookupTimeoutMs = 1500;

    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;
    private readonly ObservableCollection<LanDeviceItem> _devices = new();
    private readonly CancellationTokenSource _windowCts = new();

    private string _detectedSubnet = "";
    private string _gatewayIp = "";
    private string _localIp = "";

    public LanScannerDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput;
        _ct = ct;
        ItemsLanList.ItemsSource = _devices;

        Loaded += (_, _) => DetectSubnet();
        Closed += (_, _) =>
        {
            try { _windowCts.Cancel(); } catch { }
            _windowCts.Dispose();
        };
    }

    /// <summary>
    /// Détecte l'IP locale et la passerelle de l'interface active (celle qui possède une passerelle IPv4).
    /// </summary>
    private void DetectSubnet()
    {
        try
        {
            var candidates = NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.OperationalStatus == OperationalStatus.Up
                          && ni.NetworkInterfaceType != NetworkInterfaceType.Loopback
                          && ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
                .Select(ni => ni.GetIPProperties())
                .Select(props => new
                {
                    Gateway = props.GatewayAddresses
                        .Select(g => g.Address)
                        .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork && !a.Equals(IPAddress.Any)),
                    Local = props.UnicastAddresses
                        .Select(u => u.Address)
                        .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(a))
                })
                .Where(x => x.Local != null)
                .OrderByDescending(x => x.Gateway != null) // l'interface avec passerelle en priorité
                .ToList();

            var best = candidates.FirstOrDefault();
            if (best?.Local != null)
            {
                _localIp   = best.Local.ToString();
                _gatewayIp = best.Gateway?.ToString() ?? "";
                _detectedSubnet = _localIp[.._localIp.LastIndexOf('.')];
                TxtSubnetInfo.Text = string.IsNullOrEmpty(_gatewayIp)
                    ? $"Plage : {_detectedSubnet}.1 - 254 (IP locale : {_localIp})"
                    : $"Plage : {_detectedSubnet}.1 - 254 (Passerelle : {_gatewayIp})";
                return;
            }
        }
        catch (Exception ex)
        {
            _onOutput($"[!] Détection réseau impossible : {ex.Message}");
        }

        _detectedSubnet = "192.168.1";
        TxtSubnetInfo.Text = "Plage par défaut : 192.168.1.1 - 254";
    }

    private async void BtnStartScan_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_detectedSubnet)) DetectSubnet();

        BtnStartScan.IsEnabled = false;
        ProgressScan.IsIndeterminate = false;
        ProgressScan.Minimum = 0;
        ProgressScan.Maximum = 254;
        ProgressScan.Value = 0;
        TxtFeedback.Text = $"Scan du sous-réseau {_detectedSubnet}.1-254 en cours...";
        _devices.Clear();
        TxtHostCount.Text = "0 appareil(s)";

        _onOutput($"\n[i] Démarrage du scan LAN sur {_detectedSubnet}.1-254...");

        using var linked = CancellationTokenSource.CreateLinkedTokenSource(_ct, _windowCts.Token);
        var token = linked.Token;

        // Progress<T> créé sur le thread UI → callbacks marshalés automatiquement sur le Dispatcher
        int done = 0;
        var progress = new Progress<LanDeviceItem?>(found =>
        {
            ProgressScan.Value = ++done;
            if (found != null)
            {
                InsertSorted(found);
                TxtHostCount.Text = $"{_devices.Count} appareil(s)";
            }
        });

        try
        {
            await ScanSubnetAsync(_detectedSubnet, progress, token);

            TxtFeedback.Text = $"Scan terminé : {_devices.Count} machine(s) active(s) trouvée(s).";
            _onOutput($"[OK] Scan LAN terminé : {_devices.Count} appareil(s) identifié(s) sur le réseau");
        }
        catch (OperationCanceledException)
        {
            TxtFeedback.Text = "Scan annulé.";
            _onOutput("🛑 [ANNULÉ] Scan LAN interrompu.");
        }
        catch (Exception ex)
        {
            TxtFeedback.Text = $"Erreur lors du scan : {ex.Message}";
            _onOutput($"[ERR] Échec du scan LAN : {ex.Message}");
        }
        finally
        {
            ProgressScan.Value = ProgressScan.Maximum;
            if (IsLoaded) BtnStartScan.IsEnabled = true;
        }
    }

    private async Task ScanSubnetAsync(string subnet, IProgress<LanDeviceItem?> progress, CancellationToken ct)
    {
        using var throttle = new SemaphoreSlim(MaxParallelPings);

        var tasks = Enumerable.Range(1, 254).Select(async host =>
        {
            await throttle.WaitAsync(ct);
            try
            {
                string ip = $"{subnet}.{host}";
                progress.Report(await ProbeHostAsync(ip, ct));
            }
            finally
            {
                throttle.Release();
            }
        });

        await Task.WhenAll(tasks);
    }

    private async Task<LanDeviceItem?> ProbeHostAsync(string ip, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        bool alive;
        try
        {
            using var ping = new Ping();
            var reply = await ping.SendPingAsync(ip, PingTimeoutMs);
            alive = reply.Status == IPStatus.Success;
        }
        catch (PingException)
        {
            alive = false;
        }

        if (!alive) return null;

        string type = ip == _gatewayIp ? "Passerelle / Box"
                    : ip == _localIp   ? "Cet ordinateur"
                    : "Appareil connecté";

        return new LanDeviceItem
        {
            IpAddress  = ip,
            HostName   = await ResolveHostNameAsync(ip, ct),
            DeviceType = type
        };
    }

    /// <summary>Résolution DNS inverse bornée dans le temps (certaines box ne répondent jamais).</summary>
    private static async Task<string> ResolveHostNameAsync(string ip, CancellationToken ct)
    {
        try
        {
            var lookup = Dns.GetHostEntryAsync(ip);
            var winner = await Task.WhenAny(lookup, Task.Delay(DnsLookupTimeoutMs, ct));
            if (winner == lookup)
            {
                var name = (await lookup).HostName;
                if (!string.IsNullOrWhiteSpace(name) && name != ip) return name;
            }
        }
        catch (OperationCanceledException) { throw; }
        catch { }

        return "Hôte sans nom DNS";
    }

    /// <summary>Insère un appareil en conservant l'ordre numérique du dernier octet.</summary>
    private void InsertSorted(LanDeviceItem item)
    {
        int key = LastOctet(item.IpAddress);
        int index = 0;
        while (index < _devices.Count && LastOctet(_devices[index].IpAddress) < key) index++;
        _devices.Insert(index, item);
    }

    private static int LastOctet(string ip) =>
        int.TryParse(ip.AsSpan(ip.LastIndexOf('.') + 1), out int n) ? n : 0;

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
