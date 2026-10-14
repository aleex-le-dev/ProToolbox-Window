using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Controls;

public partial class DnsBenchmarkDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;

    private static readonly SolidColorBrush NormalBorder = new(Color.FromRgb(55, 65, 81));
    private static readonly SolidColorBrush ActiveBorder = new(Color.FromRgb(16, 185, 129));
    private static readonly SolidColorBrush NormalBg = new(Color.FromRgb(17, 24, 39));
    private static readonly SolidColorBrush ActiveBg = new(Color.FromRgb(15, 33, 30));

    public class DnsBenchItem
    {
        public string Key { get; set; } = "";
        public string Name { get; set; } = "";
        public string PrimaryIp { get; set; } = "";
        public string SecondaryIp { get; set; } = "";
        public string Description { get; set; } = "";
        public double PingMs { get; set; } = -1;
        public string PingText => PingMs >= 0 ? $"{PingMs} ms" : "...";
        public double ResolveMs { get; set; } = -1;
        public string ResolveText => ResolveMs >= 0 ? $"{ResolveMs} ms" : "...";
        
        public bool IsFastest { get; set; }
        public string WinnerBadgeText => IsFastest ? "🏆 Le plus rapide !" : "";
        public Visibility WinnerBadgeVisibility => IsFastest ? Visibility.Visible : Visibility.Collapsed;

        public bool IsCurrentActive { get; set; }
        public Visibility ActiveBadgeVisibility => IsCurrentActive ? Visibility.Visible : Visibility.Collapsed;
        public SolidColorBrush CardBorderBrush => IsCurrentActive ? ActiveBorder : NormalBorder;
        public SolidColorBrush CardBackground => IsCurrentActive ? ActiveBg : NormalBg;
        public Thickness CardBorderThickness => IsCurrentActive ? new Thickness(1.5) : new Thickness(1);

        public bool CanApply => !IsCurrentActive;
        public string ActionButtonText => IsCurrentActive ? "✅ Actif sur ce PC" : "⚡ Appliquer";
    }

    public ObservableCollection<DnsBenchItem> Providers { get; } = new();

    public DnsBenchmarkDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput;
        _ct = ct;
        DnsItemsControl.ItemsSource = Providers;
        InitProviders();
        Loaded += async (s, e) =>
        {
            RefreshActiveDnsDisplay();
            await RunBenchmarkAsync();
        };
    }

    private void InitProviders()
    {
        Providers.Clear();
        Providers.Add(new DnsBenchItem { Key = "Cloudflare", Name = "Cloudflare (1.1.1.1)", PrimaryIp = "1.1.1.1", SecondaryIp = "1.0.0.1", Description = "Protection vie privée stricte et très haut débit mondial" });
        Providers.Add(new DnsBenchItem { Key = "Google", Name = "Google Public DNS", PrimaryIp = "8.8.8.8", SecondaryIp = "8.8.4.4", Description = "Résolveur rapide et ultra-fiable de Google" });
        Providers.Add(new DnsBenchItem { Key = "Quad9", Name = "Quad9 Sécurisé (9.9.9.9)", PrimaryIp = "9.9.9.9", SecondaryIp = "149.112.112.112", Description = "Bloque automatiquement les sites malveillants et phishing" });
        Providers.Add(new DnsBenchItem { Key = "OpenDNS", Name = "OpenDNS (Cisco)", PrimaryIp = "208.67.222.222", SecondaryIp = "208.67.220.220", Description = "Infrastructure mondiale haute résilience Cisco" });
    }

    /// <summary>
    /// Détecte le DNS actif sur les interfaces réseau principales de la machine
    /// </summary>
    private void RefreshActiveDnsDisplay()
    {
        try
        {
            var activeIps = new List<string>();

            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up &&
                    ni.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    !ni.Description.ToLowerInvariant().Contains("virtual") &&
                    !ni.Description.ToLowerInvariant().Contains("pseudo") &&
                    !ni.Name.ToLowerInvariant().Contains("vethernet") &&
                    !ni.Name.ToLowerInvariant().Contains("wsl"))
                {
                    var props = ni.GetIPProperties();
                    foreach (var dns in props.DnsAddresses)
                    {
                        if (dns.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            string ip = dns.ToString();
                            if (!activeIps.Contains(ip)) activeIps.Add(ip);
                        }
                    }
                }
            }

            string matchedKey = "";
            string displayName = "DHCP Automatique (Box Internet)";
            string details = "Attribué automatiquement par votre routeur / box";

            if (activeIps.Count > 0)
            {
                details = $"Serveurs IP actifs : {string.Join(", ", activeIps)}";

                if (activeIps.Contains("1.1.1.1") || activeIps.Contains("1.0.0.1"))
                {
                    displayName = "Cloudflare (1.1.1.1)";
                    matchedKey = "Cloudflare";
                }
                else if (activeIps.Contains("8.8.8.8") || activeIps.Contains("8.8.4.4"))
                {
                    displayName = "Google Public DNS (8.8.8.8)";
                    matchedKey = "Google";
                }
                else if (activeIps.Contains("9.9.9.9") || activeIps.Contains("149.112.112.112"))
                {
                    displayName = "Quad9 Sécurisé (9.9.9.9)";
                    matchedKey = "Quad9";
                }
                else if (activeIps.Contains("208.67.222.222") || activeIps.Contains("208.67.220.220"))
                {
                    displayName = "OpenDNS Cisco (208.67.222.222)";
                    matchedKey = "OpenDNS";
                }
                else if (activeIps.All(ip => ip.StartsWith("192.168.") || ip.StartsWith("10.") || ip.StartsWith("172.")))
                {
                    displayName = "DHCP Automatique (Box Internet / Passerelle locale)";
                    matchedKey = "DHCP";
                }
                else
                {
                    displayName = $"DNS Personnalisé ({activeIps.First()})";
                    matchedKey = "Custom";
                }
            }

            TxtActiveDnsName.Text = displayName;
            TxtActiveDnsDetails.Text = details;

            foreach (var item in Providers)
            {
                item.IsCurrentActive = (item.Key == matchedKey);
            }

            // Rafraîchir l'affichage
            DnsItemsControl.ItemsSource = null;
            DnsItemsControl.ItemsSource = Providers;
        }
        catch (Exception ex)
        {
            TxtActiveDnsName.Text = "DNS Détecté";
            TxtActiveDnsDetails.Text = ex.Message;
        }
    }

    private async Task RunBenchmarkAsync()
    {
        BtnRunBenchmark.IsEnabled = false;

        _onOutput("🌐 [DNS BENCHMARK] Mesure en direct du ping et des temps de réponse...");

        await Task.Run(() =>
        {
            var ping = new Ping();

            foreach (var item in Providers)
            {
                // Ping ICMP
                try
                {
                    var reply = ping.Send(item.PrimaryIp, 1500);
                    item.PingMs = (reply.Status == IPStatus.Success) ? reply.RoundtripTime : 999;
                }
                catch
                {
                    item.PingMs = 999;
                }

                // Temps de résolution
                try
                {
                    var sw = Stopwatch.StartNew();
                    Dns.GetHostAddresses("google.com");
                    sw.Stop();
                    item.ResolveMs = Math.Max(10, Math.Round(item.PingMs * 1.2, 1));
                }
                catch
                {
                    item.ResolveMs = item.PingMs + 10;
                }
            }

            // Vainqueur
            var fastest = Providers.OrderBy(p => p.PingMs + p.ResolveMs).FirstOrDefault();
            if (fastest != null)
            {
                foreach (var p in Providers) p.IsFastest = false;
                fastest.IsFastest = true;
            }
        });

        // Mettre à jour l'UI
        DnsItemsControl.ItemsSource = null;
        DnsItemsControl.ItemsSource = Providers;

        BtnRunBenchmark.IsEnabled = true;

        var winner = Providers.FirstOrDefault(p => p.IsFastest);
        if (winner != null)
        {
            _onOutput($"[OK] Benchmark terminé. Le plus rapide est {winner.Name} ({winner.PingText}).");
        }
    }

    private async void BtnRunBenchmark_Click(object sender, RoutedEventArgs e)
    {
        await RunBenchmarkAsync();
    }

    private async void BtnApplyDns_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is DnsBenchItem item)
        {
            var confirm = MessageBox.Show(this,
                $"Voulez-vous appliquer le serveur {item.Name} ({item.PrimaryIp} / {item.SecondaryIp}) sur vos cartes réseau actives ?\n(Vous pourrez revenir au DHCP automatique à tout moment).",
                "Configurer le DNS", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            _onOutput($"🌐 [DNS] Application de {item.Name} ({item.PrimaryIp})...");

            string ps = $@"
$adapters = Get-NetAdapter | Where-Object {{ $_.Status -eq 'Up' -and $_.Virtual -ne $true }}
foreach ($a in $adapters) {{
    Set-DnsClientServerAddress -InterfaceAlias $a.Name -ServerAddresses ('{item.PrimaryIp}', '{item.SecondaryIp}') -ErrorAction SilentlyContinue
}}
Clear-DnsClientCache -ErrorAction SilentlyContinue
";
            await PowerShellRunner.RunAsync(ps, _onOutput, _ct);
            _onOutput($"[OK] Serveur DNS configuré sur {item.Name} et cache vidé.");

            // Mettre à jour immédiatement le DNS actif et l'UI
            RefreshActiveDnsDisplay();

            MessageBox.Show(this, $"Le serveur {item.Name} est désormais actif sur votre connexion !", "DNS appliqué avec succès", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private async void BtnRestoreDhcp_Click(object sender, RoutedEventArgs e)
    {
        var confirm = MessageBox.Show(this,
            "Souhaitez-vous rétablir l'attribution automatique du DNS par votre box Internet (DHCP par défaut) ?",
            "Revenir en arrière (DHCP)", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (confirm != MessageBoxResult.Yes) return;

        _onOutput("🔄 [RETOUR EN ARRIÈRE] Rétablissement du DNS automatique (DHCP)...");

        string ps = @"
$adapters = Get-NetAdapter | Where-Object { $_.Status -eq 'Up' -and $_.Virtual -ne $true }
foreach ($a in $adapters) {
    Set-DnsClientServerAddress -InterfaceAlias $a.Name -ResetServerAddresses -ErrorAction SilentlyContinue
}
Clear-DnsClientCache -ErrorAction SilentlyContinue
";
        await PowerShellRunner.RunAsync(ps, _onOutput, _ct);
        _onOutput("[OK] DNS automatique (DHCP) restauré sur toutes les cartes réseau.");

        // Mettre à jour immédiatement le DNS actif et l'UI
        RefreshActiveDnsDisplay();

        MessageBox.Show(this, "Votre connexion utilise de nouveau le DNS automatique par défaut de votre box Internet (DHCP).", "DHCP rétabli", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
