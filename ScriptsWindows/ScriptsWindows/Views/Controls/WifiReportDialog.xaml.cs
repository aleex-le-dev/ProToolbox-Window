using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Controls;

public partial class WifiReportDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;

    private readonly string _sourceReportPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        @"Microsoft\Windows\WlanReport\wlan-report-latest.html");

    public WifiReportDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput;
        _ct = ct;

        Loaded += async (_, _) => await LoadOrGenerateReportAsync();
    }

    private async Task LoadOrGenerateReportAsync(bool forceNew = false)
    {
        PanelLoading.Visibility = Visibility.Visible;
        ReportWebBrowser.Visibility = Visibility.Collapsed;
        PanelInfo.Visibility = Visibility.Collapsed;
        BtnRegenerate.IsEnabled = false;
        TxtFeedback.Text = "Analyse des événements sans fil en cours...";

        try
        {
            // Si le rapport existe déjà et date de moins de 10 minutes, on peut l'afficher direct à moins d'un forçage
            bool needGeneration = forceNew || !File.Exists(_sourceReportPath) ||
                                  (DateTime.Now - File.GetLastWriteTime(_sourceReportPath)).TotalMinutes > 10;

            if (needGeneration)
            {
                _onOutput("\n[i] Génération en cours du rapport de diagnostic Wi-Fi complet...");
                // Exécution discrète sans inonder la console de caractères Mojibake
                await Task.Run(() =>
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "netsh",
                        Arguments = "wlan show wlanreport",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    using var proc = Process.Start(psi);
                    proc?.WaitForExit();
                }, _ct);
            }

            if (File.Exists(_sourceReportPath))
            {
                var fileInfo = new FileInfo(_sourceReportPath);
                TxtReportDate.Text = $"Généré le {fileInfo.LastWriteTime:dd/MM/yyyy à HH:mm:ss} ({fileInfo.Length / 1024} Ko)";
                TxtFeedback.Text = "Rapport chargé avec succès.";
                _onOutput($"[OK] Rapport Wi-Fi officiel prêt : {_sourceReportPath}");

                try
                {
                    ReportWebBrowser.Navigate(new Uri(_sourceReportPath));
                    ReportWebBrowser.Visibility = Visibility.Visible;
                    PanelLoading.Visibility = Visibility.Collapsed;
                }
                catch
                {
                    // Fallback vers le panneau info
                    PanelInfo.Visibility = Visibility.Visible;
                    PanelLoading.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                PanelLoading.Visibility = Visibility.Collapsed;
                PanelInfo.Visibility = Visibility.Visible;
                TxtFeedback.Text = "Impossible de localiser le rapport généré.";
                _onOutput("[!] Rapport introuvable dans C:\\ProgramData\\Microsoft\\Windows\\WlanReport");
            }
        }
        catch (Exception ex)
        {
            PanelLoading.Visibility = Visibility.Collapsed;
            PanelInfo.Visibility = Visibility.Visible;
            TxtFeedback.Text = $"Erreur : {ex.Message}";
            _onOutput($"[ERR] Échec du rapport Wi-Fi : {ex.Message}");
        }
        finally
        {
            BtnRegenerate.IsEnabled = true;
        }
    }

    private void BtnOpenBrowser_Click(object sender, RoutedEventArgs e)
    {
        if (File.Exists(_sourceReportPath))
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = _sourceReportPath,
                    UseShellExecute = true
                });
                TxtFeedback.Text = "Rapport ouvert dans votre navigateur par défaut.";
            }
            catch (Exception ex)
            {
                TxtFeedback.Text = $"Erreur d'ouverture : {ex.Message}";
            }
        }
        else
        {
            TxtFeedback.Text = "Le fichier de rapport n'existe pas encore.";
        }
    }

    private void BtnExport_Click(object sender, RoutedEventArgs e)
    {
        if (!File.Exists(_sourceReportPath))
        {
            MessageBox.Show("Veuillez d'abord générer le rapport Wi-Fi.", "Export", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var sfd = new SaveFileDialog
        {
            Title = "Enregistrer une copie du rapport Wi-Fi",
            FileName = $"Rapport-WiFi-{DateTime.Now:yyyyMMdd-HHmm}.html",
            Filter = "Fichier HTML (*.html)|*.html|Tous les fichiers (*.*)|*.*",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        };

        if (sfd.ShowDialog() == true)
        {
            try
            {
                File.Copy(_sourceReportPath, sfd.FileName, true);
                TxtFeedback.Text = $"Rapport exporté : {Path.GetFileName(sfd.FileName)}";
                _onOutput($"[OK] Rapport Wi-Fi exporté vers : {sfd.FileName}");

                var res = MessageBox.Show(
                    $"Le rapport a été copié vers :\n{sfd.FileName}\n\nSouhaitez-vous l'ouvrir maintenant ?",
                    "Rapport exporté avec succès",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);

                if (res == MessageBoxResult.Yes)
                {
                    Process.Start(new ProcessStartInfo { FileName = sfd.FileName, UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la copie : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void BtnRegenerate_Click(object sender, RoutedEventArgs e)
    {
        await LoadOrGenerateReportAsync(forceNew: true);
    }

    private void BtnOpenFolder_Click(object sender, RoutedEventArgs e)
    {
        string dir = Path.GetDirectoryName(_sourceReportPath) ?? "";
        if (Directory.Exists(dir))
        {
            Process.Start("explorer.exe", $"/select,\"{_sourceReportPath}\"");
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
