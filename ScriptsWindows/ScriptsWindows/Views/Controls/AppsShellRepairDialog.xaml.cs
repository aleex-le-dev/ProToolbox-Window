using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ScriptsWindows.Services;

namespace ScriptsWindows.Views.Controls;

public partial class AppsShellRepairDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;
    private bool _isBusy;

    public AppsShellRepairDialog(Action<string> onOutput, CancellationToken ct = default)
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

        BtnRepairStore.IsEnabled = !busy;
        BtnRepairIcons.IsEnabled = !busy;
        BtnRepairAll.IsEnabled = !busy;
    }

    private async void BtnRepairStore_Click(object sender, RoutedEventArgs e)
    {
        if (_isBusy) return;

        SetBusy(true, "Réinitialisation du Microsoft Store et des packages UWP...");
        _onOutput("\n🚀 [LANCEMENT] Réparation Microsoft Store (wsreset & AppX)...");

        try
        {
            await RepairService.RepairWindowsStore(_onOutput, _ct);
            SetBusy(false, "Microsoft Store et packages UWP réparés.");
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] {ex.Message}");
            SetBusy(false, $"Erreur : {ex.Message}");
        }
    }

    private async void BtnRepairIcons_Click(object sender, RoutedEventArgs e)
    {
        if (_isBusy) return;

        SetBusy(true, "Reconstruction du cache d'icônes et de miniatures...");
        _onOutput("\n🚀 [LANCEMENT] Purge iconcache / thumbcache et relance d'Explorer...");

        try
        {
            await RepairService.RepairIcons(_onOutput, _ct);
            SetBusy(false, "Cache d'icônes reconstruit avec succès.");
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] {ex.Message}");
            SetBusy(false, $"Erreur : {ex.Message}");
        }
    }

    private async void BtnRepairAll_Click(object sender, RoutedEventArgs e)
    {
        if (_isBusy) return;

        SetBusy(true, "Cure complète Store & Interface en cours...");
        _onOutput("==================================================");
        _onOutput("  RÉPARATION COMPLÈTE STORE & INTERFACE");
        _onOutput("==================================================");

        try
        {
            _onOutput("\n[1/2] Réparation du Microsoft Store & UWP...");
            await RepairService.RepairWindowsStore(_onOutput, _ct);

            _onOutput("\n[2/2] Reconstruction du cache d'icônes & Explorateur...");
            await RepairService.RepairIcons(_onOutput, _ct);

            _onOutput("\n[OK] Cure complète Store & Interface terminée.");
            SetBusy(false, "Cure complète terminée avec succès !");
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
