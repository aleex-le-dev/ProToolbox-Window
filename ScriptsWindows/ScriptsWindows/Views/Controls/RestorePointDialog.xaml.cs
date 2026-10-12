using System.Text.Json;
using System.Windows;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Controls;

public class RestorePointViewItem
{
    public int SequenceNumber { get; set; }
    public string DateStr { get; set; } = "";
    public string Description { get; set; } = "";
}

public partial class RestorePointDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;

    public RestorePointDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput;
        _ct = ct;

        Loaded += async (_, _) => await LoadRestorePointsAsync();
    }

    private async Task LoadRestorePointsAsync()
    {
        TxtFeedback.Text = "Lecture des points de restauration existants...";
        LstRestorePoints.Items.Clear();

        try
        {
            string ps = @"
$pts = try { Get-ComputerRestorePoint -ErrorAction Stop } catch { @() }
if ($pts) {
    $pts | ForEach-Object {
        $d = try { [Management.ManagementDateTimeConverter]::ToDateTime($_.CreationTime).ToString('dd/MM/yyyy HH:mm') } catch { $_.CreationTime }
        [PSCustomObject]@{
            SequenceNumber = $_.SequenceNumber
            DateStr = $d
            Description = $_.Description
        }
    } | ConvertTo-Json
} else {
    '[]'
}";
            string json = await PowerShellRunner.RunAsync(ps, _ct);
            json = json.Trim();

            if (!string.IsNullOrWhiteSpace(json))
            {
                if (json.StartsWith("["))
                {
                    using var doc = JsonDocument.Parse(json);
                    foreach (var el in doc.RootElement.EnumerateArray())
                    {
                        LstRestorePoints.Items.Add(new RestorePointViewItem
                        {
                            SequenceNumber = el.TryGetProperty("SequenceNumber", out var seq) ? seq.GetInt32() : 0,
                            DateStr = el.TryGetProperty("DateStr", out var ds) ? (ds.GetString() ?? "") : "",
                            Description = el.TryGetProperty("Description", out var desc) ? (desc.GetString() ?? "") : ""
                        });
                    }
                }
                else if (json.StartsWith("{"))
                {
                    using var doc = JsonDocument.Parse(json);
                    var el = doc.RootElement;
                    LstRestorePoints.Items.Add(new RestorePointViewItem
                    {
                        SequenceNumber = el.TryGetProperty("SequenceNumber", out var seq) ? seq.GetInt32() : 0,
                        DateStr = el.TryGetProperty("DateStr", out var ds) ? (ds.GetString() ?? "") : "",
                        Description = el.TryGetProperty("Description", out var desc) ? (desc.GetString() ?? "") : ""
                    });
                }
            }

            TxtFeedback.Text = LstRestorePoints.Items.Count > 0
                ? $"{LstRestorePoints.Items.Count} point(s) de restauration trouvé(s)."
                : "Aucun point de restauration trouvé sur ce système.";
        }
        catch (Exception ex)
        {
            TxtFeedback.Text = $"Erreur lecture : {ex.Message}";
        }
    }

    private async void BtnCreatePoint_Click(object sender, RoutedEventArgs e)
    {
        string desc = TxtPointDescription.Text.Trim();
        if (string.IsNullOrWhiteSpace(desc)) desc = "Point de restauration AlexToolkit";

        BtnCreatePoint.IsEnabled = false;
        TxtFeedback.Text = $"Création du point '{desc}' en cours...";
        _onOutput($"\n[i] Création d'un point de restauration système : '{desc}'...");

        try
        {
            string ps = $@"
Enable-ComputerRestore -Drive ""$env:SystemDrive"" -ErrorAction SilentlyContinue
Checkpoint-Computer -Description ""{desc}"" -RestorePointType MODIFY_SETTINGS -ErrorAction Stop
Write-Output '[OK] Point de restauration créé avec succès'
";
            await PowerShellRunner.RunAsync(ps, _onOutput, _ct);
            TxtFeedback.Text = "Point de restauration créé avec succès !";
            await LoadRestorePointsAsync();
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] Échec de la création : {ex.Message}");
            TxtFeedback.Text = $"Erreur : {ex.Message}";
        }
        finally
        {
            BtnCreatePoint.IsEnabled = true;
        }
    }

    private void BtnOpenRstrui_Click(object sender, RoutedEventArgs e)
    {
        _onOutput("\n[i] Ouverture de l'assistant de restauration système Windows (rstrui.exe)...");
        CommandRunner.StartProcess("rstrui.exe");
    }

    private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
    {
        await LoadRestorePointsAsync();
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
