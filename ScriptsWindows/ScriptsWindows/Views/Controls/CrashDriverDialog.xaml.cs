using System.IO;
using System.Windows;
using System.Windows.Controls;
using ScriptsWindows.Core;
using ScriptsWindows.Services;

namespace ScriptsWindows.Views.Controls;

/// <summary>Écrans bleus (journal + minidumps) et périphériques en erreur, avec cause probable et actions.</summary>
public partial class CrashDriverDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;
    private bool _busy;

    public CrashDriverDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput ?? (_ => { });
        _ct = ct;
        Loaded += async (_, _) => await RefreshAsync();
    }

    private void SetBusy(bool busy, string status)
    {
        _busy = busy;
        ProgBar.Visibility = busy ? Visibility.Visible : Visibility.Collapsed;
        BtnRefresh.IsEnabled = !busy;
        TxtStatus.Text = status;
    }

    private async Task RefreshAsync()
    {
        if (_busy) return;
        SetBusy(true, "Analyse du journal Windows et des périphériques…");
        try
        {
            var crashesTask = TroubleshootService.GetCrashesAsync(_ct);
            var devicesTask = TroubleshootService.GetProblemDevicesAsync(_ct);
            await Task.WhenAll(crashesTask, devicesTask);

            var (crashes, shutdowns, dumps) = crashesTask.Result;
            var devices = devicesTask.Result;

            ListCrashes.ItemsSource = crashes;
            TxtNoCrash.Visibility = crashes.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            TxtCrashCount.Text = crashes.Count.ToString();
            TxtCrashCount.Foreground = ThemeManager.GetBrush(crashes.Count == 0 ? "SuccessText" : "DangerText");

            TxtShutdownCount.Text = shutdowns.ToString();
            TxtShutdownCount.Foreground = ThemeManager.GetBrush(shutdowns == 0 ? "SuccessText" : shutdowns > 3 ? "DangerText" : "WarningText");

            ListDevices.ItemsSource = devices;
            TxtNoDevice.Visibility = devices.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            TxtDeviceCount.Text = devices.Count.ToString();
            TxtDeviceCount.Foreground = ThemeManager.GetBrush(devices.Count == 0 ? "SuccessText" : "WarningText");

            // Synthèse : le code d'arrêt le plus fréquent oriente le diagnostic
            var top = crashes.GroupBy(c => c.Title).OrderByDescending(g => g.Count()).FirstOrDefault();
            string summary = top != null
                ? $"Code le plus fréquent : {top.Key} ({top.Count()}×). {top.First().Cause}"
                : shutdowns > 0
                    ? "Pas d'écran bleu, mais des arrêts brutaux : alimentation, surchauffe ou gel complet du système."
                    : "Aucune panne système détectée.";
            SetBusy(false, summary);

            _onOutput($"[i] Pannes & pilotes : {crashes.Count} écran(s) bleu(s), {shutdowns} arrêt(s) brutal(aux), {dumps} minidump(s), {devices.Count} périphérique(s) en erreur.");
            foreach (var d in devices) _onOutput($"[!] {d.Name} — code {d.ErrorCode} : {d.Explanation}");
        }
        catch (Exception ex)
        {
            SetBusy(false, $"Erreur d'analyse : {ex.Message}");
        }
    }

    private async void BtnRefresh_Click(object sender, RoutedEventArgs e) => await RefreshAsync();

    private void BtnOpenMinidump_Click(object sender, RoutedEventArgs e)
    {
        string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Minidump");
        if (Directory.Exists(dir)) CommandRunner.StartProcess("explorer.exe", $"\"{dir}\"");
        else TxtStatus.Text = "Aucun dossier Minidump : Windows n'a enregistré aucun vidage mémoire.";
    }

    private void BtnDevMgmt_Click(object sender, RoutedEventArgs e) => CommandRunner.StartProcess("devmgmt.msc");

    private async void BtnScanHardware_Click(object sender, RoutedEventArgs e)
    {
        if (_busy) return;
        SetBusy(true, "Recherche des modifications matérielles…");
        _onOutput("\n[i] Recherche des modifications matérielles (pnputil /scan-devices)");
        try { await TroubleshootService.ScanForHardwareChanges(_onOutput, _ct); }
        catch (Exception ex) { _onOutput($"[ERR] {ex.Message}"); }
        _busy = false;
        await RefreshAsync();
    }

    private async void BtnReinstall_Click(object sender, RoutedEventArgs e)
    {
        if (_busy || sender is not Button { Tag: ProblemDevice device }) return;
        var confirm = MessageBox.Show(this,
            $"Le périphérique « {device.Name} » va être supprimé puis redétecté : Windows réinstallera automatiquement son pilote.\n\nContinuer ?",
            "Réinstaller le périphérique", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirm != MessageBoxResult.Yes) return;

        SetBusy(true, $"Réinstallation de « {device.Name} »…");
        _onOutput($"\n[i] Réinstallation du périphérique : {device.Name}");
        try { await TroubleshootService.ReinstallDevice(device.InstanceId, _onOutput, _ct); }
        catch (Exception ex) { _onOutput($"[ERR] {ex.Message}"); }
        _busy = false;
        await RefreshAsync();
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
