using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Controls;

public class StartupItemModel
{
    public string Name { get; set; } = "";
    public string Command { get; set; } = "";
    public string Location { get; set; } = "";
    public string User { get; set; } = "";

    public string LocationShort => Location.Contains("HKLM", StringComparison.OrdinalIgnoreCase) ? "Système (HKLM)" : "Utilisateur (HKCU / Dossier)";
}

public partial class StartupManagerDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;

    public StartupManagerDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput;
        _ct = ct;

        Loaded += async (_, _) => await LoadStartupItemsAsync();
    }

    private async Task LoadStartupItemsAsync()
    {
        TxtFeedback.Text = "Analyse des applications au démarrage...";
        LstStartupItems.Items.Clear();

        try
        {
            string ps = @"
Get-CimInstance Win32_StartupCommand -ErrorAction SilentlyContinue | Select-Object Name, Command, Location, User | ConvertTo-Json
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
                        LstStartupItems.Items.Add(new StartupItemModel
                        {
                            Name = el.TryGetProperty("Name", out var n) ? (n.GetString() ?? "") : "",
                            Command = el.TryGetProperty("Command", out var c) ? (c.GetString() ?? "") : "",
                            Location = el.TryGetProperty("Location", out var l) ? (l.GetString() ?? "") : "",
                            User = el.TryGetProperty("User", out var u) ? (u.GetString() ?? "") : ""
                        });
                    }
                }
                else if (json.StartsWith("{"))
                {
                    using var doc = JsonDocument.Parse(json);
                    var el = doc.RootElement;
                    LstStartupItems.Items.Add(new StartupItemModel
                    {
                        Name = el.TryGetProperty("Name", out var n) ? (n.GetString() ?? "") : "",
                        Command = el.TryGetProperty("Command", out var c) ? (c.GetString() ?? "") : "",
                        Location = el.TryGetProperty("Location", out var l) ? (l.GetString() ?? "") : "",
                        User = el.TryGetProperty("User", out var u) ? (u.GetString() ?? "") : ""
                    });
                }
            }

            TxtFeedback.Text = $"{LstStartupItems.Items.Count} application(s) au démarrage détectée(s).";
        }
        catch (Exception ex)
        {
            TxtFeedback.Text = $"Erreur détection : {ex.Message}";
        }
    }

    private async void BtnDisableItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn || btn.Tag is not StartupItemModel item) return;

        var confirm = MessageBox.Show(
            $"Désactiver '{item.Name}' du démarrage de Windows ?\n\nCommande : {item.Command}",
            "Désactiver du démarrage",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirm != MessageBoxResult.Yes) return;

        _onOutput($"\n[i] Désactivation de l'application '{item.Name}' du démarrage...");
        TxtFeedback.Text = $"Désactivation de '{item.Name}'...";

        try
        {
            string ps = $@"
Remove-ItemProperty -Path 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Run' -Name '{item.Name}' -ErrorAction SilentlyContinue
Remove-ItemProperty -Path 'HKLM:\Software\Microsoft\Windows\CurrentVersion\Run' -Name '{item.Name}' -ErrorAction SilentlyContinue
Write-Output '[OK] Entrée de démarrage retirée'
";
            await PowerShellRunner.RunAsync(ps, _onOutput, _ct);
            _onOutput($"[OK] Application '{item.Name}' retirée du démarrage.");
            TxtFeedback.Text = $"'{item.Name}' désactivée avec succès.";
            await LoadStartupItemsAsync();
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] {ex.Message}");
            TxtFeedback.Text = $"Erreur : {ex.Message}";
        }
    }

    private void BtnOpenTaskmgr_Click(object sender, RoutedEventArgs e)
    {
        _onOutput("\n[i] Ouverture du Gestionnaire des tâches sur l'onglet Démarrage...");
        CommandRunner.StartProcess("taskmgr.exe", "/0 /startup");
    }

    private void BtnOpenStartupFolder_Click(object sender, RoutedEventArgs e)
    {
        _onOutput("\n[i] Ouverture du dossier de démarrage utilisateur (shell:startup)...");
        CommandRunner.StartProcess("explorer.exe", "shell:startup");
    }

    private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
    {
        await LoadStartupItemsAsync();
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
