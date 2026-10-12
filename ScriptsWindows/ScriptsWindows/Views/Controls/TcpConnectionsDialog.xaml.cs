using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Controls;

public class TcpConnectionItem
{
    public string ProcessName { get; set; } = "Inconnu";
    public int Pid { get; set; } = 0;
    public string LocalAddress { get; set; } = "—";
    public string RemoteAddress { get; set; } = "—";
    public string State { get; set; } = "Established";

    public string PidText => Pid > 0 ? $"PID: {Pid}" : "Système";

    public SolidColorBrush StateColor => State.ToUpperInvariant() switch
    {
        "ESTABLISHED" => new SolidColorBrush(Color.FromRgb(16, 185, 129)), // Vert #10B981
        "LISTEN"      => new SolidColorBrush(Color.FromRgb(59, 130, 246)), // Bleu #3B82F6
        "TIMEWAIT"    => new SolidColorBrush(Color.FromRgb(107, 114, 128)),// Gris #6B7280
        "CLOSEWAIT"   => new SolidColorBrush(Color.FromRgb(245, 158, 11)), // Orange #F59E0B
        _             => new SolidColorBrush(Color.FromRgb(75, 85, 99))
    };
}

public partial class TcpConnectionsDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;

    private readonly List<TcpConnectionItem> _allConnections = new();
    private readonly ObservableCollection<TcpConnectionItem> _filteredConnections = new();

    public TcpConnectionsDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput;
        _ct = ct;
        ItemsTcpList.ItemsSource = _filteredConnections;

        Loaded += async (_, _) => await LoadConnectionsAsync();
    }

    private async Task LoadConnectionsAsync()
    {
        BtnRefresh.IsEnabled = false;
        TxtStatus.Text = "Interrogation des connexions TCP actives...";
        TxtFeedback.Text = "Lecture de la table TCP Windows...";
        _allConnections.Clear();
        _filteredConnections.Clear();

        try
        {
            string ps = @"
$conns = Get-NetTCPConnection -ErrorAction SilentlyContinue | Where-Object { $_.State -ne 'Listen' } | Select-Object -First 60
$procTable = @{}
Get-Process -ErrorAction SilentlyContinue | ForEach-Object { $procTable[$_.Id] = $_.ProcessName }

$list = foreach ($c in $conns) {
    $pName = if ($procTable.ContainsKey($c.OwningProcess)) { ""$($procTable[$c.OwningProcess]).exe"" } else { 'Système' }
    [PSCustomObject]@{
        ProcessName  = $pName
        Pid          = $c.OwningProcess
        LocalAddress = ""$($c.LocalAddress):$($c.LocalPort)""
        RemoteAddress = ""$($c.RemoteAddress):$($c.RemotePort)""
        State        = ""$($c.State)""
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
                        _allConnections.Add(new TcpConnectionItem
                        {
                            ProcessName = el.GetProperty("ProcessName").GetString() ?? "Inconnu",
                            Pid = el.GetProperty("Pid").GetInt32(),
                            LocalAddress = el.GetProperty("LocalAddress").GetString() ?? "—",
                            RemoteAddress = el.GetProperty("RemoteAddress").GetString() ?? "—",
                            State = el.GetProperty("State").GetString() ?? "Established"
                        });
                    }
                }
                else if (json.StartsWith("{"))
                {
                    using var doc = JsonDocument.Parse(json);
                    var el = doc.RootElement;
                    _allConnections.Add(new TcpConnectionItem
                    {
                        ProcessName = el.GetProperty("ProcessName").GetString() ?? "Inconnu",
                        Pid = el.GetProperty("Pid").GetInt32(),
                        LocalAddress = el.GetProperty("LocalAddress").GetString() ?? "—",
                        RemoteAddress = el.GetProperty("RemoteAddress").GetString() ?? "—",
                        State = el.GetProperty("State").GetString() ?? "Established"
                    });
                }
            }

            ApplyFilter();
            TxtStatus.Text = $"{_allConnections.Count} connexions chargées.";
            TxtFeedback.Text = "Flux réseau actualisés avec succès.";
            _onOutput($"[OK] Moniteur TCP : {_allConnections.Count} connexions actives identifiées");
        }
        catch (Exception ex)
        {
            TxtStatus.Text = "Erreur de lecture.";
            TxtFeedback.Text = ex.Message;
            _onOutput($"[ERR] Échec du moniteur TCP : {ex.Message}");
        }
        finally
        {
            BtnRefresh.IsEnabled = true;
        }
    }

    private void ApplyFilter()
    {
        string query = TxtSearch.Text.Trim().ToLowerInvariant();
        _filteredConnections.Clear();

        foreach (var c in _allConnections)
        {
            if (string.IsNullOrWhiteSpace(query) ||
                c.ProcessName.ToLowerInvariant().Contains(query) ||
                c.LocalAddress.ToLowerInvariant().Contains(query) ||
                c.RemoteAddress.ToLowerInvariant().Contains(query) ||
                c.State.ToLowerInvariant().Contains(query))
            {
                _filteredConnections.Add(c);
            }
        }

        TxtCountBadge.Text = $"{_filteredConnections.Count} connexion(s)";
    }

    private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        ApplyFilter();
    }

    private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
    {
        await LoadConnectionsAsync();
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
