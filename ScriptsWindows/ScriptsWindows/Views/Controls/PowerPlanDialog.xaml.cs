using System;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Controls;

public partial class PowerPlanDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;

    public class PowerPlanItem
    {
        public string Guid { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Icon { get; set; } = "⚡";
        public bool IsActive { get; set; }
        public Visibility IsActiveVisibility => IsActive ? Visibility.Visible : Visibility.Collapsed;
        public bool CanActivate => !IsActive;
    }

    public ObservableCollection<PowerPlanItem> Plans { get; } = new();

    public PowerPlanDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput;
        _ct = ct;
        PlansItemsControl.ItemsSource = Plans;

        Loaded += async (s, e) => await LoadPlansAsync();
    }

    private async Task LoadPlansAsync()
    {
        TxtStatus.Text = "Lecture des modes d'alimentation Windows...";
        Plans.Clear();

        try
        {
            string ps = @"
$output = powercfg /list
$plans = @()
foreach ($line in $output) {
    if ($line -match '([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})\s+\(([^)]+)\)') {
        $guid = $matches[1]
        $name = $matches[2]
        $isActive = $line.Contains('*')
        $plans += [PSCustomObject]@{
            Guid = $guid
            Name = $name
            IsActive = $isActive
        }
    }
}
$plans | ConvertTo-Json -Compress
";
            string json = await PowerShellRunner.RunAsync(ps, _ct);
            if (!string.IsNullOrWhiteSpace(json))
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    string activeName = "Non déterminé";
                    foreach (var elem in doc.RootElement.EnumerateArray())
                    {
                        var guid = elem.GetProperty("Guid").GetString() ?? "";
                        var name = elem.GetProperty("Name").GetString() ?? "";
                        var isActive = elem.GetProperty("IsActive").GetBoolean();

                        string icon = "⚡";
                        string desc = "Mode personnalisé Windows";
                        string lower = name.ToLowerInvariant();

                        if (lower.Contains("normal") || lower.Contains("équilibré") || lower.Contains("balanced"))
                        {
                            icon = "⚖️";
                            desc = "Équilibre optimal entre réactivité CPU et consommation énergétique.";
                        }
                        else if (lower.Contains("haute") || lower.Contains("high") || lower.Contains("élevé"))
                        {
                            icon = "🚀";
                            desc = "Maximise les fréquences du processeur et élimine les bridages de veille.";
                        }
                        else if (lower.Contains("ultime") || lower.Contains("ultimate"))
                        {
                            icon = "⚡";
                            desc = "Supprime toute latence et pousse le matériel à son potentiel absolu.";
                        }
                        else if (lower.Contains("éco") || lower.Contains("saver"))
                        {
                            icon = "🌱";
                            desc = "Réduit la luminosité et bride le processeur pour préserver la batterie.";
                        }

                        if (isActive)
                        {
                            activeName = name;
                        }

                        Plans.Add(new PowerPlanItem
                        {
                            Guid = guid,
                            Name = name,
                            Description = desc,
                            Icon = icon,
                            IsActive = isActive
                        });
                    }

                    TxtActivePlanBadge.Text = $"{activeName} (Actif)";
                }
            }

            TxtStatus.Text = "Prêt.";
        }
        catch (Exception ex)
        {
            TxtStatus.Text = $"Erreur lors du chargement : {ex.Message}";
        }
    }

    private async void BtnActivatePlan_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string guid)
        {
            await ApplyPlanAsync(guid, "Sélection personnalisée");
        }
    }

    private async void BtnSetBalanced_Click(object sender, RoutedEventArgs e)
    {
        // 381b4222-f694-41f0-9685-ff5bb260df2e = Équilibré / Utilisation normale
        await ApplyPlanAsync("381b4222-f694-41f0-9685-ff5bb260df2e", "Utilisation normale (Équilibré)");
    }

    private async void BtnSetHigh_Click(object sender, RoutedEventArgs e)
    {
        // 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c = Haute performance
        await ApplyPlanAsync("8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c", "Haute performance");
    }

    private async void BtnSetUltimate_Click(object sender, RoutedEventArgs e)
    {
        TxtStatus.Text = "Déblocage et activation du mode Performances Ultimes...";
        _onOutput("[i] Activation du plan d'alimentation Performances Ultimes...");
        // Dupliquer le scheme ultime s'il n'est pas encore installé
        await CommandRunner.RunAsync("powercfg -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61", _onOutput, _ct);
        await ApplyPlanAsync("e9a42b02-d5df-448d-aa00-03f14749eb61", "Performances Ultimes");
    }

    private async Task ApplyPlanAsync(string guid, string label)
    {
        TxtStatus.Text = $"Activation du plan {label}...";
        _onOutput($"[i] Application du plan d'alimentation : {label} ({guid})...");

        await CommandRunner.RunAsync($"powercfg /setactive {guid}", _onOutput, _ct);
        _onOutput($"[OK] Plan d'alimentation activé avec succès : {label}");

        await LoadPlansAsync();
        TxtStatus.Text = $"Plan activé : {label}";
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
