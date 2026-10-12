using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ScriptsWindows.Services;

namespace ScriptsWindows.Views.Controls;

public partial class SystemIntegrityDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;
    private bool _isBusy;

    public SystemIntegrityDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput ?? (_ => { });
        _ct = ct;
    }

    private void SetBusy(bool busy, string status)
    {
        _isBusy = busy;
        ProgBar.Visibility = busy ? Visibility.Visible : Visibility.Collapsed;
        ProgBar.IsIndeterminate = busy;
        TxtStatus.Text = status;

        BtnRunPipeline.IsEnabled = !busy;
        BtnRunScanHealth.IsEnabled = !busy;
        BtnRunRestoreHealth.IsEnabled = !busy;
        BtnRunSfc.IsEnabled = !busy;
    }

    private async void BtnRunPipeline_Click(object sender, RoutedEventArgs e)
    {
        if (_isBusy) return;

        SetBusy(true, "Exécution du pipeline curatif complet (DISM + SFC)...");
        _onOutput("\n🚀 [LANCEMENT] Pipeline curatif global (DISM RestoreHealth + SFC scannow)...");

        try
        {
            await RepairService.RunCurativePipeline(_onOutput, _ct);
            SetBusy(false, "Pipeline curatif complet terminé avec succès.");
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] {ex.Message}");
            SetBusy(false, $"Erreur : {ex.Message}");
        }
    }

    private async void BtnRunScanHealth_Click(object sender, RoutedEventArgs e)
    {
        if (_isBusy) return;

        SetBusy(true, "Vérification de l'image (DISM /ScanHealth) en cours...");
        _onOutput("\n🚀 [LANCEMENT] DISM /Online /Cleanup-Image /ScanHealth...");

        try
        {
            await RepairService.RunDismCheck(_onOutput, _ct);
            SetBusy(false, "Vérification DISM terminée.");
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] {ex.Message}");
            SetBusy(false, $"Erreur : {ex.Message}");
        }
    }

    private async void BtnRunRestoreHealth_Click(object sender, RoutedEventArgs e)
    {
        if (_isBusy) return;

        SetBusy(true, "Restauration en ligne de l'image (DISM /RestoreHealth) en cours...");
        _onOutput("\n🚀 [LANCEMENT] DISM /Online /Cleanup-Image /RestoreHealth...");

        try
        {
            await RepairService.RunDismRestore(_onOutput, _ct);
            SetBusy(false, "Restauration de l'image DISM terminée.");
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] {ex.Message}");
            SetBusy(false, $"Erreur : {ex.Message}");
        }
    }

    private async void BtnRunSfc_Click(object sender, RoutedEventArgs e)
    {
        if (_isBusy) return;

        SetBusy(true, "Vérification des fichiers système (SFC /scannow) en cours...");
        _onOutput("\n🚀 [LANCEMENT] SFC /scannow...");

        try
        {
            await RepairService.RunSfc(_onOutput, _ct);
            SetBusy(false, "Analyse SFC terminée.");
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] {ex.Message}");
            SetBusy(false, $"Erreur : {ex.Message}");
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
