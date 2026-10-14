using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ScriptsWindows.Services;

namespace ScriptsWindows.Views.Controls;

public partial class BootRecoveryDialog : Window
{
    private readonly Action<string>? _externalOutput;
    private readonly CancellationToken _cancellationToken;
    private readonly StringBuilder _logBuffer = new();
    private bool _isBusy;

    public BootRecoveryDialog(Action<string>? onOutput = null, CancellationToken ct = default)
    {
        InitializeComponent();
        _externalOutput = onOutput;
        _cancellationToken = ct;

        Loaded += async (_, _) => await RefreshStatusAsync();
    }

    private void Log(string message)
    {
        Dispatcher.InvokeAsync(() =>
        {
            _logBuffer.AppendLine(message);
            TxtConsole.Text = _logBuffer.ToString();
            TxtConsole.ScrollToEnd();
            _externalOutput?.Invoke(message);
        });
    }

    private async Task RefreshStatusAsync()
    {
        TxtSafeModeStatus.Text = "Analyse...";
        TxtFirmwareStatus.Text = "Analyse...";

        var safeMode = await BootService.GetCurrentSafeModeStatusAsync();
        bool isUefi = BootService.IsUefiSystem();

        Dispatcher.Invoke(() =>
        {
            switch (safeMode)
            {
                case SafeModeType.Minimal:
                    TxtSafeModeStatus.Text = "Mode sans échec (Minimal) ACTIF";
                    TxtSafeModeStatus.Foreground = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#F59E0B")!;
                    break;
                case SafeModeType.Network:
                    TxtSafeModeStatus.Text = "Mode sans échec (Réseau) ACTIF";
                    TxtSafeModeStatus.Foreground = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#F59E0B")!;
                    break;
                default:
                    TxtSafeModeStatus.Text = "Mode normal (Aucun sans échec)";
                    TxtSafeModeStatus.Foreground = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#10B981")!;
                    break;
            }

            TxtFirmwareStatus.Text = isUefi ? "UEFI (Recommandé)" : "BIOS Hérité (Legacy)";
            TxtFirmwareStatus.Foreground = isUefi
                ? (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#34D399")!
                : (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#F87171")!;
        });
    }

    private async void BtnRefreshStatus_Click(object sender, RoutedEventArgs e)
    {
        if (_isBusy) return;
        await RefreshStatusAsync();
        Log("[OK] Statut système actualisé.");
    }

    private async void BtnSafeModeMinimal_Click(object sender, RoutedEventArgs e)
    {
        if (_isBusy) return;

        bool reboot = ChkRebootSafeMode.IsChecked == true;
        if (reboot)
        {
            var res = MessageBox.Show(
                "Le mode sans échec Minimal va être configuré et l'ordinateur va redémarrer dans 5 secondes.\n\nAssurez-vous d'avoir sauvegardé tous vos documents ouverts.\nVoulez-vous continuer ?",
                "Confirmation du redémarrage",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (res != MessageBoxResult.Yes) return;
        }

        await ExecuteActionAsync(async () =>
        {
            await BootService.ConfigureSafeMode(SafeModeType.Minimal, reboot, Log, _cancellationToken);
            await RefreshStatusAsync();
        });
    }

    private async void BtnSafeModeNetwork_Click(object sender, RoutedEventArgs e)
    {
        if (_isBusy) return;

        bool reboot = ChkRebootSafeMode.IsChecked == true;
        if (reboot)
        {
            var res = MessageBox.Show(
                "Le mode sans échec Réseau (avec pilotes Internet) va être configuré et l'ordinateur va redémarrer dans 5 secondes.\n\nAssurez-vous d'avoir sauvegardé tous vos documents ouverts.\nVoulez-vous continuer ?",
                "Confirmation du redémarrage",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (res != MessageBoxResult.Yes) return;
        }

        await ExecuteActionAsync(async () =>
        {
            await BootService.ConfigureSafeMode(SafeModeType.Network, reboot, Log, _cancellationToken);
            await RefreshStatusAsync();
        });
    }

    private async void BtnDisableSafeMode_Click(object sender, RoutedEventArgs e)
    {
        if (_isBusy) return;

        await ExecuteActionAsync(async () =>
        {
            await BootService.ConfigureSafeMode(SafeModeType.Normal, false, Log, _cancellationToken);
            await RefreshStatusAsync();
        });
    }

    private async void BtnRebootWinRE_Click(object sender, RoutedEventArgs e)
    {
        if (_isBusy) return;

        var res = MessageBox.Show(
            "Votre ordinateur va redémarrer immédiatement pour accéder aux options avancées de récupération (WinRE).\n\nAssurez-vous d'avoir sauvegardé tous vos travaux en cours.\nConfirmez-vous le redémarrage vers WinRE ?",
            "Accès WinRE — Confirmation",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (res != MessageBoxResult.Yes) return;

        await ExecuteActionAsync(async () =>
        {
            await BootService.RebootToWinRE(true, Log, _cancellationToken);
        });
    }

    private async void BtnRebootBios_Click(object sender, RoutedEventArgs e)
    {
        if (_isBusy) return;

        if (!BootService.IsUefiSystem())
        {
            var warnRes = MessageBox.Show(
                "Votre ordinateur semble être configuré en BIOS Hérité (Legacy) et non en UEFI.\nLe redémarrage direct vers le firmware risque d'échouer ou de redémarrer normalement.\n\nVoulez-vous quand même tenter l'opération ?",
                "Avertissement Micrologiciel",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (warnRes != MessageBoxResult.Yes) return;
        }
        else
        {
            var res = MessageBox.Show(
                "Votre ordinateur va redémarrer directement dans les réglages du BIOS / UEFI.\n\nAssurez-vous d'avoir fermé vos applications et sauvegardé vos fichiers.\nConfirmez-vous le redémarrage vers le BIOS / UEFI ?",
                "Accès BIOS / UEFI — Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (res != MessageBoxResult.Yes) return;
        }

        await ExecuteActionAsync(async () =>
        {
            await BootService.RebootToBios(true, Log, _cancellationToken);
        });
    }

    private async void BtnRepairBcd_Click(object sender, RoutedEventArgs e)
    {
        if (_isBusy) return;

        var res = MessageBox.Show(
            "Cette opération va reconstruire les fichiers de démarrage Windows (BCDBoot) et supprimer les éventuelles boucles de démarrage sans échec.\n\nVoulez-vous lancer la réparation du magasin BCD ?",
            "Réparation BCD — Confirmation",
            MessageBoxButton.YesNo,
            MessageBoxImage.Information);

        if (res != MessageBoxResult.Yes) return;

        await ExecuteActionAsync(async () =>
        {
            await BootService.RepairBcd(Log, _cancellationToken);
            await RefreshStatusAsync();
        });
    }

    private async Task ExecuteActionAsync(Func<Task> action)
    {
        _isBusy = true;
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            Log($"\n[ERREUR] Une exception s'est produite : {ex.Message}");
        }
        finally
        {
            _isBusy = false;
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
