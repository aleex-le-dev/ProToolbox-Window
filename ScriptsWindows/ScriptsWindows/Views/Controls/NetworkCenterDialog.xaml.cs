using System.Net.NetworkInformation;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ScriptsWindows.Core;
using ScriptsWindows.Services;

namespace ScriptsWindows.Views.Controls;

public partial class NetworkCenterDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;

    private readonly SolidColorBrush _brushTabActive = new(Color.FromRgb(79, 70, 229));    // #4F46E5
    private readonly SolidColorBrush _brushTabInactive = new(Colors.Transparent);
    private readonly SolidColorBrush _brushTextActive = new(Color.FromRgb(249, 250, 251));  // #F9FAFB
    private readonly SolidColorBrush _brushTextInactive = new(Color.FromRgb(156, 163, 175)); // #9CA3AF

    private readonly SolidColorBrush _brushSuccess = new(Color.FromRgb(16, 185, 129));      // #10B981
    private readonly SolidColorBrush _brushError = new(Color.FromRgb(239, 68, 68));          // #EF4444
    private readonly SolidColorBrush _brushNeutral = new(Color.FromRgb(55, 65, 81));        // #374151

    public NetworkCenterDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput;
        _ct = ct;

        Loaded += async (_, _) =>
        {
            await LoadDiagnosticsAsync();
            await LoadDnsAdaptersAsync();
        };
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GESTION DES ONGLETS
    // ─────────────────────────────────────────────────────────────────────────
    private void TabButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        string tag = btn.Tag?.ToString() ?? "";

        // Réinitialiser les boutons d'onglets
        BtnTabDiag.Background = _brushTabInactive;
        BtnTabDiag.Foreground = _brushTextInactive;
        BtnTabDns.Background = _brushTabInactive;
        BtnTabDns.Foreground = _brushTextInactive;
        BtnTabReset.Background = _brushTabInactive;
        BtnTabReset.Foreground = _brushTextInactive;
        BtnTabTools.Background = _brushTabInactive;
        BtnTabTools.Foreground = _brushTextInactive;

        // Activer le bouton sélectionné
        btn.Background = _brushTabActive;
        btn.Foreground = _brushTextActive;

        // Commuter la visibilité des panneaux
        PanelDiag.Visibility  = (tag == "TabDiag")  ? Visibility.Visible : Visibility.Collapsed;
        PanelDns.Visibility   = (tag == "TabDns")   ? Visibility.Visible : Visibility.Collapsed;
        PanelReset.Visibility = (tag == "TabReset") ? Visibility.Visible : Visibility.Collapsed;
        PanelTools.Visibility = (tag == "TabTools") ? Visibility.Visible : Visibility.Collapsed;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // ONGLET 1 : DIAGNOSTIC & TRIAGE TEMPS RÉEL
    // ─────────────────────────────────────────────────────────────────────────
    private async Task LoadDiagnosticsAsync()
    {
        TxtFeedback.Text = "Diagnostic de connectivité en cours...";
        TxtDiagAdapter.Text = "Interrogation...";
        TxtDiagIp.Text = "—";
        TxtDiagGateway.Text = "—";
        TxtDiagDns.Text = "—";

        TxtTestGwStatus.Text = "Test...";
        BadgeTestGw.Background = _brushNeutral;
        TxtTestDnsStatus.Text = "Test...";
        BadgeTestDns.Background = _brushNeutral;
        TxtTestInetStatus.Text = "Test...";
        BadgeTestInet.Background = _brushNeutral;

        try
        {
            string ps = @"
$adapter = Get-NetAdapter | Where-Object { $_.Status -eq 'Up' -and $_.Virtual -eq $false } | Sort-Object LinkSpeed -Descending | Select-Object -First 1
$alias = if ($adapter) { $adapter.Name } else { (Get-NetAdapter | Where-Object { $_.Status -eq 'Up' } | Select-Object -First 1).Name }
$speed = if ($adapter) { $adapter.LinkSpeed } else { '' }

$ip = (Get-NetIPAddress -InterfaceAlias $alias -AddressFamily IPv4 -ErrorAction SilentlyContinue | Select-Object -First 1).IPAddress
$gw = (Get-NetRoute -DestinationPrefix '0.0.0.0/0' -InterfaceAlias $alias -ErrorAction SilentlyContinue | Sort-Object RouteMetric | Select-Object -First 1).NextHop
$dns = (Get-DnsClientServerAddress -InterfaceAlias $alias -AddressFamily IPv4 -ErrorAction SilentlyContinue).ServerAddresses -join ', '

# Tests connectivité
$pingGw = $false
$gwMs = 0
if ($gw) {
    $t = Test-Connection -TargetName $gw -Count 1 -TimeoutSeconds 2 -ErrorAction SilentlyContinue
    if ($t) { $pingGw = $true; $gwMs = $t.Latency }
}

$dnsOk = $false
try {
    $resolved = [System.Net.Dns]::GetHostAddresses('cloudflare.com')
    if ($resolved.Count -gt 0) { $dnsOk = $true }
} catch {}

$inetOk = $false
$inetT = Test-Connection -TargetName '1.1.1.1' -Count 1 -TimeoutSeconds 2 -ErrorAction SilentlyContinue
if ($inetT) { $inetOk = $true }

[PSCustomObject]@{
    Adapter = if ($adapter) { ""$($adapter.Name) ($($adapter.InterfaceDescription) - $speed)"" } else { 'Aucun adaptateur actif' }
    Ip = if ($ip) { $ip } else { 'Non configuré' }
    Gateway = if ($gw) { $gw } else { 'Non joignable' }
    Dns = if ($dns) { $dns } else { 'Automatique' }
    PingGw = $pingGw
    GwLatency = $gwMs
    DnsOk = $dnsOk
    InetOk = $inetOk
} | ConvertTo-Json -Compress
";
            string json = await PowerShellRunner.RunAsync(ps, _ct);
            json = json.Trim();

            if (!string.IsNullOrWhiteSpace(json) && json.StartsWith("{"))
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                TxtDiagAdapter.Text = root.TryGetProperty("Adapter", out var a) ? a.GetString() ?? "—" : "—";
                TxtDiagIp.Text      = root.TryGetProperty("Ip", out var ip) ? ip.GetString() ?? "—" : "—";
                TxtDiagGateway.Text = root.TryGetProperty("Gateway", out var gw) ? gw.GetString() ?? "—" : "—";
                TxtDiagDns.Text     = root.TryGetProperty("Dns", out var d) ? d.GetString() ?? "—" : "—";

                bool pingGw = root.TryGetProperty("PingGw", out var pgw) && pgw.GetBoolean();
                int gwLatency = root.TryGetProperty("GwLatency", out var l) ? l.GetInt32() : 0;
                bool dnsOk = root.TryGetProperty("DnsOk", out var dok) && dok.GetBoolean();
                bool inetOk = root.TryGetProperty("InetOk", out var iok) && iok.GetBoolean();

                // Badges
                if (pingGw)
                {
                    TxtTestGwStatus.Text = $"OK ({gwLatency} ms)";
                    BadgeTestGw.Background = _brushSuccess;
                }
                else
                {
                    TxtTestGwStatus.Text = "Échec ping";
                    BadgeTestGw.Background = _brushError;
                }

                if (dnsOk)
                {
                    TxtTestDnsStatus.Text = "Opérationnel";
                    BadgeTestDns.Background = _brushSuccess;
                }
                else
                {
                    TxtTestDnsStatus.Text = "Échec résolution";
                    BadgeTestDns.Background = _brushError;
                }

                if (inetOk)
                {
                    TxtTestInetStatus.Text = "Connecté";
                    BadgeTestInet.Background = _brushSuccess;
                }
                else
                {
                    TxtTestInetStatus.Text = "Pas d'accès Internet";
                    BadgeTestInet.Background = _brushError;
                }

                TxtFeedback.Text = "Diagnostic terminé.";
            }
        }
        catch (Exception ex)
        {
            TxtFeedback.Text = $"Erreur diagnostic : {ex.Message}";
        }
    }

    private async void BtnRunDiag_Click(object sender, RoutedEventArgs e)
    {
        await LoadDiagnosticsAsync();
    }

    private async void BtnConsoleTriage_Click(object sender, RoutedEventArgs e)
    {
        TxtFeedback.Text = "Exécution du triage dans la console...";
        await NetworkService.RunTriage(_onOutput, _ct);
        TxtFeedback.Text = "Triage réseau affiché dans la console.";
    }

    // ─────────────────────────────────────────────────────────────────────────
    // ONGLET 2 : GESTIONNAIRE DNS
    // ─────────────────────────────────────────────────────────────────────────
    private async Task LoadDnsAdaptersAsync()
    {
        CmbDnsAdapters.Items.Clear();
        try
        {
            string ps = @"
Get-NetAdapter | Where-Object { $_.Status -eq 'Up' } | Select-Object Name, InterfaceDescription, Status | ConvertTo-Json
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
                        CmbDnsAdapters.Items.Add(new NetworkAdapterDnsItem
                        {
                            Name = el.GetProperty("Name").GetString() ?? "",
                            InterfaceDescription = el.GetProperty("InterfaceDescription").GetString() ?? "",
                            Status = el.GetProperty("Status").GetString() ?? ""
                        });
                    }
                }
                else if (json.StartsWith("{"))
                {
                    using var doc = JsonDocument.Parse(json);
                    var el = doc.RootElement;
                    CmbDnsAdapters.Items.Add(new NetworkAdapterDnsItem
                    {
                        Name = el.GetProperty("Name").GetString() ?? "",
                        InterfaceDescription = el.GetProperty("InterfaceDescription").GetString() ?? "",
                        Status = el.GetProperty("Status").GetString() ?? ""
                    });
                }
            }

            if (CmbDnsAdapters.Items.Count > 0)
            {
                CmbDnsAdapters.SelectedIndex = 0;
            }
        }
        catch (Exception ex)
        {
            TxtFeedback.Text = $"Erreur adaptateurs DNS : {ex.Message}";
        }
    }

    private async void CmbDnsAdapters_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        await RefreshSelectedAdapterDnsAsync();
    }

    private async void BtnDnsRefresh_Click(object sender, RoutedEventArgs e)
    {
        await LoadDnsAdaptersAsync();
    }

    private async Task RefreshSelectedAdapterDnsAsync()
    {
        if (CmbDnsAdapters.SelectedItem is not NetworkAdapterDnsItem adapter) return;

        TxtDnsCurrentVal.Text = "Lecture...";
        TxtDnsCurrentIp.Text = "Lecture...";

        try
        {
            string ps = $@"
$alias = '{adapter.Name}'
$dns = (Get-DnsClientServerAddress -InterfaceAlias $alias -AddressFamily IPv4 -ErrorAction SilentlyContinue).ServerAddresses -join ', '
$ip = (Get-NetIPAddress -InterfaceAlias $alias -AddressFamily IPv4 -ErrorAction SilentlyContinue | Select-Object -First 1).IPAddress
[PSCustomObject]@{{
    Dns = if ($dns) {{ $dns }} else {{ 'Automatique (DHCP)' }}
    Ip = if ($ip) {{ $ip }} else {{ '—' }}
}} | ConvertTo-Json -Compress
";
            string json = await PowerShellRunner.RunAsync(ps, _ct);
            json = json.Trim();

            if (!string.IsNullOrWhiteSpace(json) && json.StartsWith("{"))
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (root.TryGetProperty("Dns", out var d)) TxtDnsCurrentVal.Text = d.GetString() ?? "Automatique";
                if (root.TryGetProperty("Ip", out var ip)) TxtDnsCurrentIp.Text = ip.GetString() ?? "—";
            }
        }
        catch (Exception ex)
        {
            TxtDnsCurrentVal.Text = "Erreur lecture";
            TxtFeedback.Text = ex.Message;
        }
    }

    private async Task ApplyDnsAsync(string primaryDns, string secondaryDns, string providerName)
    {
        if (CmbDnsAdapters.SelectedItem is not NetworkAdapterDnsItem adapter) return;

        _onOutput($"\n[i] Configuration DNS {providerName} ({primaryDns}, {secondaryDns}) sur '{adapter.Name}'...");
        TxtFeedback.Text = $"Application DNS {providerName}...";

        try
        {
            string ps = $"Set-DnsClientServerAddress -InterfaceAlias '{adapter.Name}' -ServerAddresses ('{primaryDns}','{secondaryDns}')";
            await PowerShellRunner.RunAsync(ps, _onOutput, _ct);
            await CommandRunner.RunAsync("ipconfig /flushdns", _onOutput, _ct);

            _onOutput($"[OK] DNS {providerName} appliqués et cache DNS vidé");
            TxtFeedback.Text = $"DNS {providerName} appliqués.";
            await RefreshSelectedAdapterDnsAsync();
            await LoadDiagnosticsAsync();
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] Échec : {ex.Message}");
            TxtFeedback.Text = $"Erreur : {ex.Message}";
        }
    }

    private async void BtnApplyCloudflare_Click(object sender, RoutedEventArgs e)
        => await ApplyDnsAsync("1.1.1.1", "1.0.0.1", "Cloudflare");

    private async void BtnApplyGoogle_Click(object sender, RoutedEventArgs e)
        => await ApplyDnsAsync("8.8.8.8", "8.8.4.4", "Google");

    private async void BtnApplyQuad9_Click(object sender, RoutedEventArgs e)
        => await ApplyDnsAsync("9.9.9.9", "149.112.112.112", "Quad9");

    private async void BtnRestoreDhcp_Click(object sender, RoutedEventArgs e)
    {
        if (CmbDnsAdapters.SelectedItem is not NetworkAdapterDnsItem adapter) return;

        _onOutput($"\n[i] Restauration DNS automatique (DHCP) sur '{adapter.Name}'...");
        TxtFeedback.Text = "Restauration DNS DHCP...";

        try
        {
            string ps = $"Set-DnsClientServerAddress -InterfaceAlias '{adapter.Name}' -ResetServerAddresses";
            await PowerShellRunner.RunAsync(ps, _onOutput, _ct);
            await CommandRunner.RunAsync("ipconfig /flushdns", _onOutput, _ct);

            _onOutput("[OK] DNS DHCP automatique restauré et cache vidé");
            TxtFeedback.Text = "DNS automatique restauré.";
            await RefreshSelectedAdapterDnsAsync();
            await LoadDiagnosticsAsync();
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] Échec : {ex.Message}");
            TxtFeedback.Text = $"Erreur : {ex.Message}";
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // ONGLET 3 : DÉPANNAGE & RESET RÉSEAU
    // ─────────────────────────────────────────────────────────────────────────
    private async void BtnRestartAdapters_Click(object sender, RoutedEventArgs e)
    {
        TxtFeedback.Text = "Redémarrage des adaptateurs réseau...";
        await NetworkService.RestartAdapters(_onOutput, _ct);
        TxtFeedback.Text = "Adaptateurs réseau redémarrés.";
        await LoadDiagnosticsAsync();
    }

    private async void BtnFlushDnsArp_Click(object sender, RoutedEventArgs e)
    {
        TxtFeedback.Text = "Purge DNS & ARP en cours...";
        _onOutput("\n[i] Purge du cache DNS et de la table ARP...");
        await NetworkService.FlushDns(_onOutput, _ct);
        await NetworkService.ClearArp(_onOutput, _ct);
        _onOutput("[OK] Cache DNS vidé et table ARP purgée");
        TxtFeedback.Text = "Cache DNS & table ARP purgés.";
        await LoadDiagnosticsAsync();
    }

    private async void BtnRenewDhcp_Click(object sender, RoutedEventArgs e)
    {
        TxtFeedback.Text = "Renouvellement du bail DHCP en cours...";
        _onOutput("\n[i] Renouvellement de l'adresse IP...");
        await NetworkService.RenewIp(_onOutput, _ct);
        _onOutput("[OK] Nouvelle adresse IP obtenue");
        TxtFeedback.Text = "Bail DHCP renouvelé.";
        await LoadDiagnosticsAsync();
    }

    private async void BtnResetWinsockTcp_Click(object sender, RoutedEventArgs e)
    {
        TxtFeedback.Text = "Réinitialisation TCP/IP & Winsock...";
        _onOutput("\n[i] Réinitialisation de la pile Winsock et TCP/IP...");
        await NetworkService.ResetWinsock(_onOutput, _ct);
        await NetworkService.ResetTcpIp(_onOutput, _ct);
        _onOutput("[OK] Winsock et TCP/IP réinitialisés — un redémarrage PC est recommandé");
        TxtFeedback.Text = "Winsock et TCP/IP réinitialisés.";
    }

    private async void BtnGrandReset_Click(object sender, RoutedEventArgs e)
    {
        var res = MessageBox.Show(
            "Cette opération va purger le cache DNS, vider la table ARP, réinitialiser Winsock, la pile TCP/IP et renouveler votre adresse IP.\n\nVoulez-vous continuer ?",
            "Confirmation Grand Reset Réseau",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (res != MessageBoxResult.Yes) return;

        TxtFeedback.Text = "Grand reset réseau en cours...";
        await NetworkService.ResetAll(_onOutput, _ct);
        TxtFeedback.Text = "Grand reset réseau terminé. Redémarrage conseillé.";
        await LoadDiagnosticsAsync();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // ONGLET 4 : SCANNERS & OUTILS
    // ─────────────────────────────────────────────────────────────────────────
    private async void BtnScanWifi_Click(object sender, RoutedEventArgs e)
    {
        TxtFeedback.Text = "Scan Wi-Fi en cours...";
        _onOutput("\n[i] Analyse des réseaux sans fil environnants (netsh wlan)...");
        await NetworkService.ScanWifi(_onOutput, _ct);
        TxtFeedback.Text = "Scan Wi-Fi terminé (voir console).";
    }

    private async void BtnReportWifi_Click(object sender, RoutedEventArgs e)
    {
        TxtFeedback.Text = "Génération du rapport Wi-Fi...";
        await DiagnosticService.GetWifiReport(_onOutput, _ct);
        TxtFeedback.Text = "Rapport Wi-Fi généré et ouvert.";
    }

    private async void BtnScanLan_Click(object sender, RoutedEventArgs e)
    {
        TxtFeedback.Text = "Scan du réseau local LAN en cours...";
        await NetworkService.ScanLan(_onOutput, _ct);
        TxtFeedback.Text = "Scan LAN terminé (voir console).";
    }

    private async void BtnAnalyzeFlux_Click(object sender, RoutedEventArgs e)
    {
        TxtFeedback.Text = "Analyse des connexions TCP en cours...";
        await NetworkService.AnalyzeFlux(_onOutput, _ct);
        TxtFeedback.Text = "Connexions TCP actives affichées dans la console.";
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
