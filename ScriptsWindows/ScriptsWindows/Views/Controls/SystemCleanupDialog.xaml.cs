using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ScriptsWindows.Core;
using ScriptsWindows.Services;

namespace ScriptsWindows.Views.Controls;

public partial class SystemCleanupDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;
    private bool _isBusy;

    public SystemCleanupDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput ?? (_ => { });
        _ct = ct;

        Loaded += async (s, e) => await ScanSizesAsync();
    }

    private void SetBusy(bool busy, string status)
    {
        _isBusy = busy;
        ProgBar.Visibility = busy ? Visibility.Visible : Visibility.Collapsed;
        ProgBar.IsIndeterminate = busy;
        TxtStatus.Text = status;

        BtnCleanSelected.IsEnabled = !busy;
    }

    private async void BtnAnalyze_Click(object sender, RoutedEventArgs e)
    {
        if (_isBusy) return;
        await ScanSizesAsync();
    }

    private async Task ScanSizesAsync()
    {
        SetBusy(true, "Analyse de l'espace récupérable en cours...");
        TxtTotalSizeFound.Text = "Analyse en cours...";

        try
        {
            long tempBytes = 0;
            long wuBytes = 0;
            long browsersBytes = 0;
            long recycleBytes = 0;

            await Task.Run(() =>
            {
                // 1. Temp
                var tempPaths = new[] { Path.GetTempPath(), @"C:\Windows\Temp" };
                foreach (var path in tempPaths)
                {
                    tempBytes += MeasureDirSize(path);
                }

                // 2. Windows Update
                wuBytes = MeasureDirSize(@"C:\Windows\SoftwareDistribution\Download");

                // 3. Browsers & Thumbnails
                var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var browserPaths = new[]
                {
                    Path.Combine(userProfile, @"AppData\Local\Google\Chrome\User Data\Default\Cache"),
                    Path.Combine(userProfile, @"AppData\Local\Microsoft\Edge\User Data\Default\Cache"),
                    Path.Combine(userProfile, @"AppData\Local\BraveSoftware\Brave-Browser\User Data\Default\Cache"),
                    Path.Combine(userProfile, @"AppData\Local\Mozilla\Firefox\Profiles")
                };

                foreach (var bp in browserPaths)
                {
                    browsersBytes += MeasureDirSize(bp);
                }

                // Thumbnails
                var thumbDir = Path.Combine(userProfile, @"AppData\Local\Microsoft\Windows\Explorer");
                if (Directory.Exists(thumbDir))
                {
                    try
                    {
                        var di = new DirectoryInfo(thumbDir);
                        foreach (var file in di.EnumerateFiles("thumbcache_*.db", SearchOption.TopDirectoryOnly))
                        {
                            try { browsersBytes += file.Length; } catch { }
                        }
                    }
                    catch { }
                }

                // 4. Recycle Bin
                foreach (var drive in DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed && d.IsReady))
                {
                    var rbPath = Path.Combine(drive.RootDirectory.FullName, "$Recycle.Bin");
                    recycleBytes += MeasureDirSize(rbPath);
                }
            });

            TxtTempSize.Text = FormatBytes(tempBytes);
            TxtWuSize.Text = FormatBytes(wuBytes);
            TxtBrowsersSize.Text = FormatBytes(browsersBytes);
            TxtRecycleBinSize.Text = FormatBytes(recycleBytes);

            long total = tempBytes + wuBytes + browsersBytes + recycleBytes;
            TxtTotalSizeFound.Text = total > 0 ? $"~ {FormatBytes(total)} récupérables" : "Système déjà propre";
            SetBusy(false, $"Analyse terminée. {FormatBytes(total)} d'espace détectés.");
        }
        catch (Exception ex)
        {
            TxtTotalSizeFound.Text = "Erreur de calcul";
            SetBusy(false, $"Erreur lors de l'analyse : {ex.Message}");
        }
    }

    private static long MeasureDirSize(string dirPath)
    {
        if (string.IsNullOrWhiteSpace(dirPath) || !Directory.Exists(dirPath))
            return 0;

        long size = 0;
        try
        {
            var di = new DirectoryInfo(dirPath);
            var options = new EnumerationOptions
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = true
            };

            foreach (var file in di.EnumerateFiles("*", options))
            {
                try { size += file.Length; } catch { }
            }
        }
        catch { }

        return size;
    }

    private static string FormatBytes(long bytes)
    {
        if (bytes <= 0) return "0 MB";
        double mb = bytes / (1024.0 * 1024.0);
        if (mb >= 1024)
        {
            double gb = mb / 1024.0;
            return $"{gb:F2} GB";
        }
        return $"{mb:F1} MB";
    }

    private void BtnSelectAll_Click(object sender, RoutedEventArgs e)
    {
        ChkTemp.IsChecked = true;
        ChkWu.IsChecked = true;
        ChkRecycleBin.IsChecked = true;
        ChkBrowsers.IsChecked = true;
        ChkDnsAndClip.IsChecked = true;
        ChkWinSxS.IsChecked = true;
    }

    private void BtnDeselectAll_Click(object sender, RoutedEventArgs e)
    {
        ChkTemp.IsChecked = false;
        ChkWu.IsChecked = false;
        ChkRecycleBin.IsChecked = false;
        ChkBrowsers.IsChecked = false;
        ChkDnsAndClip.IsChecked = false;
        ChkWinSxS.IsChecked = false;
    }

    private void BtnLaunchCleanmgr_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _onOutput("[i] Lancement de l'assistant natif Windows Cleanmgr...");
            Process.Start(new ProcessStartInfo
            {
                FileName = "cleanmgr.exe",
                Arguments = "/d C:",
                UseShellExecute = true
            });
            TxtStatus.Text = "Assistant de nettoyage de disque (Cleanmgr) lancé.";
            _onOutput("[OK] Cleanmgr lancé avec succès");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Impossible de lancer Cleanmgr : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BtnLaunchStorageSense_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _onOutput("[i] Ouverture des paramètres d'assistant de stockage Windows...");
            Process.Start(new ProcessStartInfo
            {
                FileName = "ms-settings:storagesense",
                UseShellExecute = true
            });
            TxtStatus.Text = "Paramètres de stockage ouverts.";
            _onOutput("[OK] Paramètres de stockage ouverts.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Impossible d'ouvrir les paramètres : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void BtnCleanAllUnified_Click(object sender, RoutedEventArgs e)
    {
        if (_isBusy) return;

        var result = MessageBox.Show(
            "Voulez-vous lancer le nettoyage complet automatique (temporaires, cache WU, corbeille, navigateurs, DNS et presse-papiers) ?",
            "Confirmation du nettoyage complet",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        SetBusy(true, "Nettoyage complet en cours...");
        _onOutput("[i] === NETTOYAGE COMPLET UNIFIÉ LANCÉ ===");

        try
        {
            await CleaningService.CleanAll(_onOutput, _ct);
            await EmptyRecycleBinAsync();
            SetBusy(false, "Nettoyage complet terminé avec succès !");
            await ScanSizesAsync();
        }
        catch (Exception ex)
        {
            _onOutput($"[!] Erreur lors du nettoyage complet : {ex.Message}");
            SetBusy(false, $"Erreur : {ex.Message}");
        }
    }

    private async void BtnCleanSelected_Click(object sender, RoutedEventArgs e)
    {
        if (_isBusy) return;

        bool doTemp = ChkTemp.IsChecked == true;
        bool doWu = ChkWu.IsChecked == true;
        bool doRecycle = ChkRecycleBin.IsChecked == true;
        bool doBrowsers = ChkBrowsers.IsChecked == true;
        bool doDnsAndClip = ChkDnsAndClip.IsChecked == true;
        bool doWinSxS = ChkWinSxS.IsChecked == true;

        if (!doTemp && !doWu && !doRecycle && !doBrowsers && !doDnsAndClip && !doWinSxS)
        {
            MessageBox.Show("Veuillez sélectionner au moins un élément à nettoyer.", "Aucun élément coché", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        SetBusy(true, "Exécution du nettoyage sélectionné...");
        _onOutput("==================================================");
        _onOutput("  NETTOYAGE SYSTÈME ET DISQUE PERSONNALISÉ");
        _onOutput("==================================================");

        try
        {
            if (doTemp)
            {
                TxtStatus.Text = "Nettoyage des dossiers temporaires...";
                _onOutput("\n[1] Nettoyage des dossiers temporaires (%TEMP% et Windows\\Temp)...");
                await CleaningService.CleanTemp(_onOutput, _ct);
            }

            if (doWu)
            {
                TxtStatus.Text = "Purge du cache des téléchargements Windows Update...";
                _onOutput("\n[2] Purge du dossier de téléchargement Windows Update...");
                await CleaningService.CleanWindowsUpdate(_onOutput, _ct);
            }

            if (doRecycle)
            {
                TxtStatus.Text = "Vidage de la corbeille Windows...";
                _onOutput("\n[3] Vidage de la corbeille Windows...");
                await EmptyRecycleBinAsync();
            }

            if (doBrowsers)
            {
                TxtStatus.Text = "Nettoyage des caches navigateurs et miniatures...";
                _onOutput("\n[4] Nettoyage des caches navigateurs et miniatures...");
                await CleaningService.CleanBrowsers(_onOutput, _ct);
            }

            if (doDnsAndClip)
            {
                TxtStatus.Text = "Vidage du cache DNS et du presse-papiers...";
                _onOutput("\n[5] Vidage du cache DNS et du presse-papiers...");
                await CleaningService.FlushDns(_onOutput, _ct);
                await CleaningService.ClearClipboard(_onOutput, _ct);
            }

            if (doWinSxS)
            {
                TxtStatus.Text = "Nettoyage approfondi du magasin WinSxS (DISM)...";
                _onOutput("\n[6] Optimisation du magasin de composants WinSxS (DISM)...");
                await CleaningService.CleanWinSxS(_onOutput, _ct);
            }

            _onOutput("\n[OK] Nettoyage sélectionné terminé avec succès.");
            SetBusy(false, "Nettoyage sélectionné terminé avec succès !");

            // Recalculer l'espace libéré
            await ScanSizesAsync();
        }
        catch (Exception ex)
        {
            _onOutput($"[!] Erreur pendant le nettoyage : {ex.Message}");
            SetBusy(false, $"Erreur : {ex.Message}");
        }
    }

    private async Task EmptyRecycleBinAsync()
    {
        await PowerShellRunner.RunAsync(@"
try {
    Clear-RecycleBin -Force -ErrorAction SilentlyContinue
    Write-Output '[OK] Corbeille Windows vidée'
} catch {
    Write-Output '[i] Corbeille déjà vide ou accès restreint'
}
", _onOutput, _ct);
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
