using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Controls;

public class NetworkAdapterDnsItem
{
    public string Name { get; set; } = "";
    public string InterfaceDescription { get; set; } = "";
    public string Status { get; set; } = "";

    public override string ToString() => $"{Name} ({InterfaceDescription})";
}

public partial class DnsManagerDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;

    public DnsManagerDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput;
        _ct = ct;

        Loaded += async (_, _) => await LoadAdaptersAsync();
    }

    private async Task LoadAdaptersAsync()
    {
        TxtFeedback.Text = "Détection des adaptateurs réseau actifs...";
        CmbAdapters.Items.Clear();

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
                        CmbAdapters.Items.Add(new NetworkAdapterDnsItem
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
                    CmbAdapters.Items.Add(new NetworkAdapterDnsItem
                    {
                        Name = el.GetProperty("Name").GetString() ?? "",
                        InterfaceDescription = el.GetProperty("InterfaceDescription").GetString() ?? "",
                        Status = el.GetProperty("Status").GetString() ?? ""
                    });
                }
            }

            if (CmbAdapters.Items.Count > 0)
            {
                CmbAdapters.SelectedIndex = 0;
            }
            else
            {
                TxtCurrentDns.Text = "Aucune carte réseau active";
                TxtFeedback.Text = "Vérifiez votre connexion réseau.";
            }
        }
        catch (Exception ex)
        {
            TxtFeedback.Text = $"Erreur détection : {ex.Message}";
        }
    }

    private async void CmbAdapters_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        await RefreshCurrentDnsAsync();
    }

    private async Task RefreshCurrentDnsAsync()
    {
        if (CmbAdapters.SelectedItem is not NetworkAdapterDnsItem adapter) return;

        TxtCurrentDns.Text = "Lecture...";
        TxtCurrentIp.Text = "Lecture...";

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
                if (root.TryGetProperty("Dns", out var d)) TxtCurrentDns.Text = d.GetString() ?? "Automatique";
                if (root.TryGetProperty("Ip", out var ip)) TxtCurrentIp.Text = ip.GetString() ?? "—";
            }
            TxtFeedback.Text = $"Paramètres chargés pour {adapter.Name}.";
        }
        catch (Exception ex)
        {
            TxtCurrentDns.Text = "Erreur lecture";
            TxtFeedback.Text = ex.Message;
        }
    }

    private async Task ApplyDnsAsync(string primaryDns, string secondaryDns, string providerName)
    {
        if (CmbAdapters.SelectedItem is not NetworkAdapterDnsItem adapter) return;

        _onOutput($"\n[i] Application des serveurs DNS {providerName} ({primaryDns}, {secondaryDns}) sur '{adapter.Name}'...");
        TxtFeedback.Text = $"Configuration des DNS {providerName}...";

        try
        {
            string ps = $"Set-DnsClientServerAddress -InterfaceAlias '{adapter.Name}' -ServerAddresses ('{primaryDns}','{secondaryDns}')";
            await PowerShellRunner.RunAsync(ps, _onOutput, _ct);
            await CommandRunner.RunAsync("ipconfig /flushdns", _onOutput, _ct);

            _onOutput($"[OK] DNS {providerName} appliqués avec succès et cache DNS vidé.");
            TxtFeedback.Text = $"DNS {providerName} appliqués avec succès.";
            await RefreshCurrentDnsAsync();
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] Échec d'application des DNS : {ex.Message}");
            TxtFeedback.Text = $"Erreur : {ex.Message}";
        }
    }

    private async void BtnSetCloudflare_Click(object sender, RoutedEventArgs e)
        => await ApplyDnsAsync("1.1.1.1", "1.0.0.1", "Cloudflare");

    private async void BtnSetGoogle_Click(object sender, RoutedEventArgs e)
        => await ApplyDnsAsync("8.8.8.8", "8.8.4.4", "Google");

    private async void BtnSetQuad9_Click(object sender, RoutedEventArgs e)
        => await ApplyDnsAsync("9.9.9.9", "149.112.112.112", "Quad9 Sécurisé");

    private async void BtnSetDhcp_Click(object sender, RoutedEventArgs e)
    {
        if (CmbAdapters.SelectedItem is not NetworkAdapterDnsItem adapter) return;

        _onOutput($"\n[i] Rétablissement des DNS en mode Automatique (DHCP) sur '{adapter.Name}'...");
        TxtFeedback.Text = "Réinitialisation vers DHCP...";

        try
        {
            string ps = $"Set-DnsClientServerAddress -InterfaceAlias '{adapter.Name}' -ResetServerAddresses";
            await PowerShellRunner.RunAsync(ps, _onOutput, _ct);
            await CommandRunner.RunAsync("ipconfig /flushdns", _onOutput, _ct);

            _onOutput($"[OK] Mode automatique (DHCP) restauré et cache vidé.");
            TxtFeedback.Text = "Mode automatique (DHCP) restauré.";
            await RefreshCurrentDnsAsync();
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] {ex.Message}");
            TxtFeedback.Text = $"Erreur : {ex.Message}";
        }
    }

    private async void BtnFlushDns_Click(object sender, RoutedEventArgs e)
    {
        _onOutput("\n[i] Vidage du cache du résolveur DNS client Windows...");
        await CommandRunner.RunAsync("ipconfig /flushdns", _onOutput, _ct);
        _onOutput("[OK] Cache DNS vidé.");
        TxtFeedback.Text = "Cache DNS vidé avec succès.";
    }

    private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        => await LoadAdaptersAsync();

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
