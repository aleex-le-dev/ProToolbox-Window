using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using ScriptsWindows.Models;
using ScriptsWindows.Services;

namespace ScriptsWindows.Views.Controls;

public partial class UpdateDialog : Window
{
    private UpdateCheckResult? _lastResult;
    private CancellationTokenSource? _downloadCts;
    private bool _isUpdating = false;

    public UpdateDialog()
    {
        InitializeComponent();
        Loaded += UpdateDialog_Loaded;
        Closing += UpdateDialog_Closing;
    }

    private async void UpdateDialog_Loaded(object sender, RoutedEventArgs e)
    {
        var settings = UpdateService.LoadSettings();
        ChkAutoCheck.IsChecked = settings.AutoCheckOnStartup;

        var curVer = UpdateService.GetCurrentVersion();
        TxtCurrentVersion.Text = $"v{curVer.Major}.{curVer.Minor}.{curVer.Build}";

        await RunCheckAsync();
    }

    private void UpdateDialog_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (_isUpdating)
        {
            var res = MessageBox.Show(
                "Le téléchargement de la mise à jour est en cours. Souhaitez-vous vraiment l'interrompre ?",
                "Téléchargement en cours",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );
            if (res != MessageBoxResult.Yes)
            {
                e.Cancel = true;
                return;
            }
            _downloadCts?.Cancel();
        }
    }

    // ─── Vérification en ligne ─────────────────────────────────────────
    private async Task RunCheckAsync()
    {
        BtnRefresh.IsEnabled = false;
        BtnApplyUpdate.IsEnabled = false;
        BtnDownloadAs.IsEnabled = false;

        SetStatusSearching();

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            _lastResult = await UpdateService.CheckForUpdatesAsync(cts.Token);
            ApplyCheckResult(_lastResult);
        }
        catch (Exception ex)
        {
            SetStatusError("Erreur lors de la vérification : " + ex.Message);
        }
        finally
        {
            BtnRefresh.IsEnabled = true;
        }
    }

    private void ApplyCheckResult(UpdateCheckResult result)
    {
        if (!result.Success || result.LatestRelease == null)
        {
            SetStatusError(result.ErrorMessage ?? "Impossible de récupérer les informations de version.");
            return;
        }

        var latest = result.LatestRelease;
        TxtRemoteVersion.Text = latest.TagName;
        TxtRemoteDetails.Text = $"Sortie le {latest.PublishedAtFormatted} • {latest.FileSizeFormatted}";

        TxtChangelog.Text = !string.IsNullOrWhiteSpace(latest.Description)
            ? latest.Description.Replace("\r\n", "\n").Replace("\n", Environment.NewLine)
            : "Aucune note de publication détaillée n'a été fournie pour cette version.";

        // Remplir l'historique des versions
        ListReleases.ItemsSource = result.AllReleases;

        if (latest.IsNewer)
        {
            SetStatusUpdateAvailable(latest);
            BtnApplyUpdate.IsEnabled = true;
            BtnDownloadAs.IsEnabled = true;
        }
        else
        {
            SetStatusUpToDate(result.CurrentVersion);
            BtnApplyUpdate.IsEnabled = false;
            BtnDownloadAs.IsEnabled = true; // Permet de télécharger le binaire pour une clé USB
        }
    }

    // ─── États visuels de la bannière ──────────────────────────────────
    private void SetStatusSearching()
    {
        BannerStatus.Background = new SolidColorBrush(Color.FromRgb(30, 41, 59));
        BannerStatus.BorderBrush = new SolidColorBrush(Color.FromRgb(51, 65, 85));
        TxtBannerIcon.Text = "🔄";
        TxtBannerTitle.Text = "Recherche des mises à jour...";
        TxtBannerSubtitle.Text = "Connexion aux serveurs GitHub & Miroir d'Alex Toolkit...";
        TxtRemoteVersion.Text = "Recherche...";
        TxtRemoteDetails.Text = "Veuillez patienter";
        TxtChangelog.Text = "Récupération du journal des modifications en ligne...";
    }

    private void SetStatusUpToDate(Version current)
    {
        BannerStatus.Background = new SolidColorBrush(Color.FromRgb(16, 185, 129));
        BannerStatus.Background.Opacity = 0.15;
        BannerStatus.BorderBrush = new SolidColorBrush(Color.FromRgb(16, 185, 129));
        TxtBannerIcon.Text = "✅";
        TxtBannerTitle.Text = "Alex Toolkit est parfaitement à jour !";
        TxtBannerSubtitle.Text = $"Vous utilisez déjà la version la plus récente (v{current.Major}.{current.Minor}.{current.Build}).";
    }

    private void SetStatusUpdateAvailable(ReleaseInfo latest)
    {
        BannerStatus.Background = new SolidColorBrush(Color.FromRgb(99, 102, 241));
        BannerStatus.Background.Opacity = 0.20;
        BannerStatus.BorderBrush = new SolidColorBrush(Color.FromRgb(129, 140, 248));
        TxtBannerIcon.Text = "🚀";
        TxtBannerTitle.Text = $"Une nouvelle version est disponible ({latest.TagName}) !";
        TxtBannerSubtitle.Text = "Cliquez sur 'Mettre à jour et redémarrer' pour l'installer instantanément sans perte de données.";
    }

    private void SetStatusError(string msg)
    {
        BannerStatus.Background = new SolidColorBrush(Color.FromRgb(239, 68, 68));
        BannerStatus.Background.Opacity = 0.15;
        BannerStatus.BorderBrush = new SolidColorBrush(Color.FromRgb(239, 68, 68));
        TxtBannerIcon.Text = "⚠️";
        TxtBannerTitle.Text = "Échec de vérification des mises à jour";
        TxtBannerSubtitle.Text = msg;
        TxtChangelog.Text = "Impossible d'afficher les notes de version car le serveur n'a pas pu être contacté.";
    }

    // ─── Actions : Mettre à jour et redémarrer ───────────────────────
    private async void BtnApplyUpdate_Click(object sender, RoutedEventArgs e)
    {
        if (_lastResult?.LatestRelease == null) return;
        var release = _lastResult.LatestRelease;

        var confirm = MessageBox.Show(
            $"Le nouvel exécutable d'Alex Toolkit ({release.TagName}) va être téléchargé.\n" +
            "Dès la fin du téléchargement, l'application se fermera, remplacera le binaire existant et redémarrera automatiquement.\n\n" +
            "Voulez-vous continuer ?",
            "Mise à jour d'Alex Toolkit",
            MessageBoxButton.YesNo,
            MessageBoxImage.Information
        );

        if (confirm != MessageBoxResult.Yes) return;

        _isUpdating = true;
        _downloadCts = new CancellationTokenSource();

        BtnApplyUpdate.IsEnabled = false;
        BtnDownloadAs.IsEnabled = false;
        BtnRefresh.IsEnabled = false;

        DownloadProgressPanel.Visibility = Visibility.Visible;
        ProgDownload.Value = 0;
        TxtDownloadProgressStatus.Text = "Téléchargement du nouvel exécutable...";

        var progress = new Progress<(long downloaded, long total, double pct)>(info =>
        {
            ProgDownload.Value = info.pct;
            TxtDownloadProgressPct.Text = $"{info.pct:F0}%";
            double dMb = info.downloaded / (1024.0 * 1024.0);
            double tMb = info.total > 0 ? info.total / (1024.0 * 1024.0) : 0;
            TxtDownloadSpeed.Text = tMb > 0
                ? $"{dMb:F1} Mo / {tMb:F1} Mo"
                : $"{dMb:F1} Mo téléchargés";
        });

        try
        {
            string tempFile = await UpdateService.DownloadUpdateAsync(
                release.DownloadUrl,
                null,
                progress,
                _downloadCts.Token
            );

            TxtDownloadProgressStatus.Text = "Téléchargement terminé ! Remplacement et redémarrage...";
            TxtDownloadProgressPct.Text = "100%";
            ProgDownload.Value = 100;

            await Task.Delay(800);

            // Permutation & relance
            UpdateService.ApplyUpdateAndRestart(tempFile);
        }
        catch (OperationCanceledException)
        {
            MessageBox.Show("Téléchargement de la mise à jour interrompu.", "Mise à jour annulée", MessageBoxButton.OK, MessageBoxImage.Information);
            DownloadProgressPanel.Visibility = Visibility.Collapsed;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Erreur lors du téléchargement de la mise à jour :\n{ex.Message}\n\nVous pouvez télécharger manuellement le binaire via le bouton 'Voir sur GitHub'.",
                "Erreur de mise à jour",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
            DownloadProgressPanel.Visibility = Visibility.Collapsed;
        }
        finally
        {
            _isUpdating = false;
            BtnApplyUpdate.IsEnabled = true;
            BtnDownloadAs.IsEnabled = true;
            BtnRefresh.IsEnabled = true;
        }
    }

    // ─── Actions : Enregistrer sous... (Téléchargement binaire manuel) ─
    private async void BtnDownloadAs_Click(object sender, RoutedEventArgs e)
    {
        if (_lastResult?.LatestRelease == null) return;
        var release = _lastResult.LatestRelease;

        var sfd = new SaveFileDialog
        {
            Title = "Enregistrer la nouvelle version d'Alex Toolkit",
            FileName = "AlexToolkit.exe",
            Filter = "Exécutable Windows (*.exe)|*.exe"
        };

        if (sfd.ShowDialog() != true) return;

        await DownloadToPathAsync(release.DownloadUrl, sfd.FileName);
    }

    private async void BtnDownloadMirrorItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is ReleaseInfo release)
        {
            var sfd = new SaveFileDialog
            {
                Title = $"Enregistrer Alex Toolkit {release.TagName}",
                FileName = $"AlexToolkit_{release.TagName}.exe",
                Filter = "Exécutable Windows (*.exe)|*.exe"
            };

            if (sfd.ShowDialog() != true) return;

            // Basculer sur la vue principale pour visualiser la progression
            SwitchToDirectUpdateTab();
            await DownloadToPathAsync(release.DownloadUrl, sfd.FileName);
        }
    }

    private async Task DownloadToPathAsync(string url, string targetPath)
    {
        _isUpdating = true;
        _downloadCts = new CancellationTokenSource();

        DownloadProgressPanel.Visibility = Visibility.Visible;
        ProgDownload.Value = 0;
        TxtDownloadProgressStatus.Text = $"Téléchargement vers {Path.GetFileName(targetPath)}...";

        var progress = new Progress<(long downloaded, long total, double pct)>(info =>
        {
            ProgDownload.Value = info.pct;
            TxtDownloadProgressPct.Text = $"{info.pct:F0}%";
            double dMb = info.downloaded / (1024.0 * 1024.0);
            double tMb = info.total > 0 ? info.total / (1024.0 * 1024.0) : 0;
            TxtDownloadSpeed.Text = tMb > 0
                ? $"{dMb:F1} Mo / {tMb:F1} Mo"
                : $"{dMb:F1} Mo";
        });

        try
        {
            await UpdateService.DownloadUpdateAsync(url, targetPath, progress, _downloadCts.Token);
            TxtDownloadProgressStatus.Text = "Téléchargement terminé avec succès !";
            ProgDownload.Value = 100;
            TxtDownloadProgressPct.Text = "100%";

            var choice = MessageBox.Show(
                $"Le fichier exécutable a été enregistré avec succès dans :\n{targetPath}\n\nSouhaitez-vous ouvrir le dossier contenant le fichier ?",
                "Téléchargement réussi",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information
            );

            if (choice == MessageBoxResult.Yes)
            {
                Process.Start("explorer.exe", $"/select,\"{targetPath}\"");
            }
        }
        catch (OperationCanceledException)
        {
            MessageBox.Show("Téléchargement interrompu.", "Annulé", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Échec du téléchargement :\n{ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            _isUpdating = false;
        }
    }

    // ─── Navigation entre onglets ──────────────────────────────────────
    private void TabBtnDirectUpdate_Click(object sender, RoutedEventArgs e)
    {
        SwitchToDirectUpdateTab();
    }

    private void TabBtnMirror_Click(object sender, RoutedEventArgs e)
    {
        SwitchToMirrorTab();
    }

    private void SwitchToDirectUpdateTab()
    {
        ViewDirectUpdate.Visibility = Visibility.Visible;
        ViewMirror.Visibility = Visibility.Collapsed;

        TabBtnDirectUpdate.Background = new SolidColorBrush(Color.FromRgb(55, 65, 81));
        TabBtnDirectUpdate.Foreground = Brushes.White;

        TabBtnMirror.Background = Brushes.Transparent;
        TabBtnMirror.Foreground = new SolidColorBrush(Color.FromRgb(156, 163, 175));
    }

    private void SwitchToMirrorTab()
    {
        ViewDirectUpdate.Visibility = Visibility.Collapsed;
        ViewMirror.Visibility = Visibility.Visible;

        TabBtnMirror.Background = new SolidColorBrush(Color.FromRgb(55, 65, 81));
        TabBtnMirror.Foreground = Brushes.White;

        TabBtnDirectUpdate.Background = Brushes.Transparent;
        TabBtnDirectUpdate.Foreground = new SolidColorBrush(Color.FromRgb(156, 163, 175));
    }

    // ─── Autres boutons & Préférences ─────────────────────────────────
    private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
    {
        await RunCheckAsync();
    }

    private void BtnOpenGitHub_Click(object sender, RoutedEventArgs e)
    {
        string url = _lastResult?.LatestRelease?.HtmlUrl ?? "https://github.com/aleex-le-dev/ProToolbox-Window/releases";
        OpenBrowser(url);
    }

    private void BtnOpenMirrorUrl_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string url && !string.IsNullOrEmpty(url))
        {
            OpenBrowser(url);
        }
    }

    private static void OpenBrowser(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        catch { }
    }

    private void ChkAutoCheck_Changed(object sender, RoutedEventArgs e)
    {
        var settings = UpdateService.LoadSettings();
        settings.AutoCheckOnStartup = ChkAutoCheck.IsChecked == true;
        UpdateService.SaveSettings(settings);
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
