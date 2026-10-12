using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ScriptsWindows.Core;
using ScriptsWindows.Services;

namespace ScriptsWindows.Views.Controls;

public class ServicePresetItem
{
    public string Name { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Description { get; set; } = "";
    public string Status { get; set; } = "Inconnu";
    public string StartType { get; set; } = "—";

    public override string ToString() => $"{DisplayName} ({Name})";
}

public partial class ServicesManagerDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;

    private readonly List<ServicePresetItem> _services = new()
    {
        new() { Name = "spooler", DisplayName = "Spouleur d'impression", Description = "Gère la mise en mémoire tampon des tâches d'impression. Indispensable pour imprimer." },
        new() { Name = "wuauserv", DisplayName = "Windows Update", Description = "Télécharge et installe les correctifs de sécurité et mises à jour du système." },
        new() { Name = "SysMain", DisplayName = "SysMain (Superfetch)", Description = "Maintient et optimise les performances du système au fil du temps." },
        new() { Name = "WSearch", DisplayName = "Windows Search", Description = "Fournit l'indexation de contenu, la mise en cache des propriétés et la recherche de fichiers." },
        new() { Name = "DiagTrack", DisplayName = "Télémétrie Windows", Description = "Collecte et envoie les données de télémétrie et d'expérience utilisateur à Microsoft." },
        new() { Name = "DoSvc", DisplayName = "Optimisation de livraison", Description = "Gère le téléchargement des mises à jour Windows et applications du Store en réseau local / P2P." },
        new() { Name = "BITS", DisplayName = "Service BITS", Description = "Transfère des fichiers en arrière-plan en utilisant la bande passante réseau inactive." },
        new() { Name = "LanmanServer", DisplayName = "Serveur (Partage de fichiers)", Description = "Prend en charge le partage de fichiers, d'imprimantes et de canaux nommés sur le réseau." }
    };

    public ServicesManagerDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput;
        _ct = ct;

        Loaded += async (_, _) => await LoadServicesAsync();
    }

    private async Task LoadServicesAsync()
    {
        TxtFeedback.Text = "Interrogation des services Windows...";
        CmbServices.Items.Clear();

        foreach (var s in _services)
        {
            CmbServices.Items.Add(s);
        }

        if (CmbServices.Items.Count > 0)
        {
            CmbServices.SelectedIndex = 0;
            await RefreshSelectedServiceStatusAsync();
        }
    }

    private async void CmbServices_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        await RefreshSelectedServiceStatusAsync();
    }

    private async Task RefreshSelectedServiceStatusAsync()
    {
        if (CmbServices.SelectedItem is not ServicePresetItem item) return;

        TxtServiceName.Text   = item.Name;
        TxtDescription.Text   = item.Description;
        TxtServiceStatus.Text = "Lecture...";
        TxtStartupType.Text   = "Lecture...";

        try
        {
            string ps = $@"
$s = Get-Service -Name '{item.Name}' -ErrorAction SilentlyContinue
if ($s) {{
    [PSCustomObject]@{{
        Status = ""$($s.Status)""
        StartType = ""$($s.StartType)""
    }} | ConvertTo-Json -Compress
}} else {{
    '{{}}'
}}";
            string json = await PowerShellRunner.RunAsync(ps, _ct);
            json = json.Trim();

            if (!string.IsNullOrWhiteSpace(json) && json.StartsWith("{"))
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (root.TryGetProperty("Status", out var st)) item.Status = st.GetString() ?? "Inconnu";
                if (root.TryGetProperty("StartType", out var ty)) item.StartType = ty.GetString() ?? "—";
            }

            bool isRunning = item.Status.Equals("Running", StringComparison.OrdinalIgnoreCase);

            TxtServiceStatus.Text = isRunning ? "En cours d'exécution (Running)" : "Arrêté (Stopped)";
            BadgeStatus.Background = isRunning ? new SolidColorBrush(Color.FromRgb(16, 185, 129)) : new SolidColorBrush(Color.FromRgb(239, 68, 68));
            TxtStartupType.Text = item.StartType;

            BtnStart.IsEnabled   = !isRunning;
            BtnStop.IsEnabled    = isRunning;
            BtnRestart.IsEnabled = isRunning;
            BtnPurgeSpooler.Visibility = item.Name.Equals("spooler", StringComparison.OrdinalIgnoreCase)
                ? Visibility.Visible
                : Visibility.Collapsed;
            TxtFeedback.Text     = $"État de {item.DisplayName} mis à jour.";
        }
        catch (Exception ex)
        {
            TxtServiceStatus.Text = "Erreur";
            TxtFeedback.Text = ex.Message;
        }
    }

    private async void BtnPurgeSpooler_Click(object sender, RoutedEventArgs e)
    {
        BtnPurgeSpooler.IsEnabled = false;
        TxtFeedback.Text = "Purge de la file d'attente d'impression en cours...";
        try
        {
            await RepairService.FixPrintSpooler(_onOutput, _ct);
            TxtFeedback.Text = "File d'attente purgée et spouleur relancé.";
            await RefreshSelectedServiceStatusAsync();
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] Échec de la purge de la file d'impression : {ex.Message}");
            TxtFeedback.Text = $"Erreur : {ex.Message}";
        }
        finally
        {
            BtnPurgeSpooler.IsEnabled = true;
        }
    }

    private async void BtnStart_Click(object sender, RoutedEventArgs e)
    {
        if (CmbServices.SelectedItem is not ServicePresetItem item) return;

        _onOutput($"\n[i] Démarrage du service '{item.DisplayName}' ({item.Name})...");
        TxtFeedback.Text = $"Démarrage de {item.Name}...";

        try
        {
            await PowerShellRunner.RunAsync($"Start-Service -Name '{item.Name}' -ErrorAction Stop", _onOutput, _ct);
            _onOutput($"[OK] Service '{item.DisplayName}' démarré avec succès.");
            TxtFeedback.Text = $"Service {item.Name} démarré.";
            await RefreshSelectedServiceStatusAsync();
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] Échec du démarrage : {ex.Message}");
            TxtFeedback.Text = $"Erreur : {ex.Message}";
        }
    }

    private async void BtnStop_Click(object sender, RoutedEventArgs e)
    {
        if (CmbServices.SelectedItem is not ServicePresetItem item) return;

        _onOutput($"\n[i] Arrêt du service '{item.DisplayName}' ({item.Name})...");
        TxtFeedback.Text = $"Arrêt de {item.Name}...";

        try
        {
            await PowerShellRunner.RunAsync($"Stop-Service -Name '{item.Name}' -Force -ErrorAction Stop", _onOutput, _ct);
            _onOutput($"[OK] Service '{item.DisplayName}' arrêté avec succès.");
            TxtFeedback.Text = $"Service {item.Name} arrêté.";
            await RefreshSelectedServiceStatusAsync();
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] Échec de l'arrêt : {ex.Message}");
            TxtFeedback.Text = $"Erreur : {ex.Message}";
        }
    }

    private async void BtnRestart_Click(object sender, RoutedEventArgs e)
    {
        if (CmbServices.SelectedItem is not ServicePresetItem item) return;

        _onOutput($"\n[i] Redémarrage du service '{item.DisplayName}' ({item.Name})...");
        TxtFeedback.Text = $"Redémarrage de {item.Name}...";

        try
        {
            await PowerShellRunner.RunAsync($"Restart-Service -Name '{item.Name}' -Force -ErrorAction Stop", _onOutput, _ct);
            _onOutput($"[OK] Service '{item.DisplayName}' redémarré avec succès.");
            TxtFeedback.Text = $"Service {item.Name} redémarré.";
            await RefreshSelectedServiceStatusAsync();
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] Échec du redémarrage : {ex.Message}");
            TxtFeedback.Text = $"Erreur : {ex.Message}";
        }
    }

    private void BtnOpenServicesMsc_Click(object sender, RoutedEventArgs e)
    {
        _onOutput("\n[i] Ouverture de la console MMC des Services Windows (services.msc)...");
        CommandRunner.StartProcess("services.msc");
    }

    private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
    {
        await RefreshSelectedServiceStatusAsync();
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
