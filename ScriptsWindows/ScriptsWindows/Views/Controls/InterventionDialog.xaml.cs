using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using ScriptsWindows.Core;
using ScriptsWindows.Services;

namespace ScriptsWindows.Views.Controls;

/// <summary>Opération de maintenance proposée dans l'intervention.</summary>
public class InterventionStep : INotifyPropertyChanged
{
    private bool _isSelected;
    private bool _isEditable = true;
    private string _status = "";
    private Brush _statusBrush = Brushes.Transparent;

    public required string Label { get; init; }
    public required string Description { get; init; }
    public required int EstimatedMinutes { get; init; }
    public required Func<Action<string>, CancellationToken, Task> Run { get; init; }

    public bool IsSelected { get => _isSelected; set => Set(ref _isSelected, value); }
    public bool IsEditable { get => _isEditable; set => Set(ref _isEditable, value); }
    public string Status { get => _status; set => Set(ref _status, value); }
    public Brush StatusBrush { get => _statusBrush; set => Set(ref _statusBrush, value); }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

/// <summary>
/// Intervention technicien : état avant → opérations choisies → état après → rapport client HTML.
/// </summary>
public partial class InterventionDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;
    private readonly List<InterventionStep> _steps;
    private CancellationTokenSource? _runCts;
    private bool _running;

    private static readonly string TechnicianFile = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ScriptsWindows", "technicien.txt");

    public InterventionDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput ?? (_ => { });
        _ct = ct;

        _steps = new List<InterventionStep>
        {
            new() { Label = "Point de restauration", Description = "Filet de sécurité avant toute modification (1 point par 24 h maximum).",
                    EstimatedMinutes = 1, IsSelected = true, Run = RepairService.CreateRestorePoint },
            new() { Label = "Nettoyage des fichiers temporaires", Description = "Fichiers temporaires utilisateur et système.",
                    EstimatedMinutes = 1, IsSelected = true, Run = CleaningService.CleanTemp },
            new() { Label = "Vider le cache DNS", Description = "Corrige les sites qui ne se chargent plus après un changement de réseau.",
                    EstimatedMinutes = 0, IsSelected = true, Run = CleaningService.FlushDns },
            new() { Label = "Antivirus : mise à jour et analyse rapide", Description = "Met à jour les définitions Microsoft Defender puis lance une analyse rapide.",
                    EstimatedMinutes = 8, IsSelected = true,
                    Run = async (log, ct) => { await SecurityService.UpdateDefender(log, ct); await SecurityService.DefenderQuickScan(log, ct); } },
            new() { Label = "Réparation des fichiers système (DISM + SFC)", Description = "Restaure l'image Windows puis répare les fichiers système corrompus.",
                    EstimatedMinutes = 25, IsSelected = true, Run = RepairService.RunCurativePipeline },
            new() { Label = "Mise à jour des logiciels (Winget)", Description = "Met à jour tous les programmes installés compatibles. Peut fermer des applications ouvertes.",
                    EstimatedMinutes = 10, IsSelected = false, Run = AppService.UpdateAll },
        };
        foreach (var s in _steps) s.PropertyChanged += (_, e) => { if (e.PropertyName == nameof(InterventionStep.IsSelected)) UpdateTotal(); };
        ListSteps.ItemsSource = _steps;
        UpdateTotal();

        try { if (File.Exists(TechnicianFile)) TxtTechnician.Text = File.ReadAllText(TechnicianFile).Trim(); } catch { }

        Closing += (_, e) =>
        {
            if (!_running) return;
            if (MessageBox.Show(this, "L'intervention est en cours. L'arrêter et fermer ?", "Intervention en cours",
                                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) e.Cancel = true;
            else _runCts?.Cancel();
        };
    }

    private void UpdateTotal()
    {
        int n = _steps.Count(s => s.IsSelected);
        int min = _steps.Where(s => s.IsSelected).Sum(s => s.EstimatedMinutes);
        TxtTotal.Text = n == 0 ? "Aucune opération" : $"{n} opération(s) • environ {Math.Max(1, min)} min";
    }

    private void SetRunning(bool running)
    {
        _running = running;
        BtnStart.Visibility = running ? Visibility.Collapsed : Visibility.Visible;
        BtnCancel.Visibility = running ? Visibility.Visible : Visibility.Collapsed;
        BtnReportOnly.IsEnabled = !running;
        BtnClose.IsEnabled = !running;
        ProgBar.Visibility = running ? Visibility.Visible : Visibility.Collapsed;
        foreach (var s in _steps) s.IsEditable = !running;
    }

    private async void BtnStart_Click(object sender, RoutedEventArgs e)
    {
        var selected = _steps.Where(s => s.IsSelected).ToList();
        if (selected.Count == 0) { TxtStatus.Text = "Cochez au moins une opération, ou utilisez « Rapport seul »."; return; }
        await RunInterventionAsync(selected);
    }

    private async void BtnReportOnly_Click(object sender, RoutedEventArgs e) => await RunInterventionAsync(new List<InterventionStep>());

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        _runCts?.Cancel();
        TxtStatus.Text = "Arrêt demandé : fin de l'opération en cours…";
    }

    private async Task RunInterventionAsync(List<InterventionStep> selected)
    {
        SaveTechnician();
        _runCts = CancellationTokenSource.CreateLinkedTokenSource(_ct);
        var token = _runCts.Token;

        // Journal de l'intervention : copie de chaque ligne envoyée à la console (pour le rapport)
        var log = new List<string>();
        void Tee(string line) { lock (log) log.Add(line); _onOutput(line); }

        foreach (var s in _steps) { s.Status = s.IsSelected && selected.Contains(s) ? "En attente" : ""; s.StatusBrush = ThemeManager.GetBrush("TextMuted"); }
        SetRunning(true);
        int total = selected.Count + 2;
        int done = 0;
        void Progress() => ProgBar.Value = 100.0 * ++done / total;

        var results = new List<StepResult>();
        PcSnapshot? before = null, after = null;
        try
        {
            Tee($"\n🚀 [LANCEMENT] Intervention technicien ({DateTime.Now:dd/MM/yyyy HH:mm})");
            TxtStatus.Text = "Relevé de l'état initial du PC…";
            before = await InterventionService.TakeSnapshotAsync(token);
            Progress();

            bool cancelled = false;
            foreach (var step in selected)
            {
                if (cancelled || token.IsCancellationRequested)
                {
                    cancelled = true;
                    step.Status = "Non exécuté";
                    results.Add(new StepResult { Label = step.Label, Status = "Annulé" });
                    continue;
                }

                TxtStatus.Text = $"{step.Label}…";
                step.Status = "En cours…";
                step.StatusBrush = ThemeManager.GetBrush("AccentText");
                Tee($"\n[i] ── {step.Label} ──");

                int logStart; lock (log) logStart = log.Count;
                var sw = Stopwatch.StartNew();
                string status;
                try
                {
                    await step.Run(Tee, token);
                    bool hasErrors; lock (log) hasErrors = log.Skip(logStart).Any(l => l.StartsWith("[ERR]") || l.Contains("[ERREUR]"));
                    status = hasErrors ? "Avertissements" : "OK";
                }
                catch (OperationCanceledException) { status = "Annulé"; cancelled = true; }
                catch (Exception ex) { Tee($"[ERR] {ex.Message}"); status = "Échec"; }
                sw.Stop();

                results.Add(new StepResult { Label = step.Label, Status = status, Duration = sw.Elapsed });
                step.Status = status == "OK" ? $"Terminé • {Format(sw.Elapsed)}" : status;
                step.StatusBrush = ThemeManager.GetBrush(status == "OK" ? "SuccessText" : status == "Avertissements" ? "WarningText" : "DangerText");
                Progress();
            }

            TxtStatus.Text = "Relevé de l'état final du PC…";
            after = selected.Count > 0
                ? await InterventionService.TakeSnapshotAsync(CancellationToken.None) // toujours mesurer l'état final, même après un arrêt
                : null;
            Progress();
        }
        catch (OperationCanceledException) when (before == null)
        {
            TxtStatus.Text = "Intervention annulée.";
            SetRunning(false);
            return;
        }
        catch (Exception ex)
        {
            Tee($"[ERR] {ex.Message}");
            if (before == null) { TxtStatus.Text = $"Impossible de relever l'état du PC : {ex.Message}"; SetRunning(false); return; }
        }

        // Rapport
        try
        {
            string html;
            lock (log)
                html = InterventionService.BuildReportHtml(TxtClient.Text.Trim(), TxtTechnician.Text.Trim(), TxtReason.Text.Trim(),
                                                           TxtConclusion.Text.Trim(), before!, after, results, log.ToList());
            string path = InterventionService.SaveReport(html, before!.ComputerName);
            Tee($"[OK] Rapport d'intervention : {path}");
            TxtStatus.Text = $"Rapport enregistré : {Path.GetFileName(path)}";
            CommandRunner.OpenUrl(path);
        }
        catch (Exception ex)
        {
            Tee($"[ERR] Rapport : {ex.Message}");
            TxtStatus.Text = $"Erreur lors de la création du rapport : {ex.Message}";
        }
        finally
        {
            SetRunning(false);
            ProgBar.Value = 0;
        }
    }

    private static string Format(TimeSpan t) => t.TotalMinutes >= 1 ? $"{(int)t.TotalMinutes} min {t.Seconds:00} s" : $"{Math.Max(1, t.Seconds)} s";

    private void SaveTechnician()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(TechnicianFile)!);
            File.WriteAllText(TechnicianFile, TxtTechnician.Text.Trim());
        }
        catch { }
    }

    private void BtnOpenReports_Click(object sender, RoutedEventArgs e)
    {
        Directory.CreateDirectory(InterventionService.ReportsFolder);
        CommandRunner.StartProcess("explorer.exe", $"\"{InterventionService.ReportsFolder}\"");
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
