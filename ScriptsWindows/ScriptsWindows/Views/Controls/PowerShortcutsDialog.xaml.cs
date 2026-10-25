using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using ScriptsWindows.Services;

namespace ScriptsWindows.Views.Controls;

public partial class PowerShortcutsDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;

    public PowerShortcutsDialog(Action<string>? onOutput = null, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput ?? (_ => { });
        _ct = ct;

        Loaded += (s, e) => RefreshStatus();
    }

    private void RefreshStatus()
    {
        bool shutdownExists = PersonalizationService.IsShortcutPresent(PersonalizationService.ShutdownShortcutName);
        bool restartExists = PersonalizationService.IsShortcutPresent(PersonalizationService.RestartShortcutName);

        // Arrêter
        UpdateBadge(BadgeShutdownStatus, TxtShutdownStatus, shutdownExists);
        BtnDeleteShutdown.Visibility = shutdownExists ? Visibility.Visible : Visibility.Collapsed;
        BtnCreateShutdown.Content = shutdownExists ? "🔄 Remplacer sur le Bureau" : "📌 Mettre sur le Bureau";

        // Redémarrer
        UpdateBadge(BadgeRestartStatus, TxtRestartStatus, restartExists);
        BtnDeleteRestart.Visibility = restartExists ? Visibility.Visible : Visibility.Collapsed;
        BtnCreateRestart.Content = restartExists ? "🔄 Remplacer sur le Bureau" : "📌 Mettre sur le Bureau";
    }

    private static void UpdateBadge(System.Windows.Controls.Border badge, System.Windows.Controls.TextBlock textBlock, bool exists)
    {
        if (exists)
        {
            badge.Background = new SolidColorBrush(Color.FromRgb(0x06, 0x5F, 0x46)); // Vert foncé
            badge.BorderBrush = new SolidColorBrush(Color.FromRgb(0x10, 0xB9, 0x81)); // Vert
            textBlock.Text = "● Sur le Bureau";
            textBlock.Foreground = new SolidColorBrush(Color.FromRgb(0x34, 0xD3, 0x99));
        }
        else
        {
            badge.Background = new SolidColorBrush(Color.FromRgb(0x11, 0x18, 0x27)); // Gris foncé
            badge.BorderBrush = new SolidColorBrush(Color.FromRgb(0x37, 0x41, 0x51));
            textBlock.Text = "○ Non installé";
            textBlock.Foreground = new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF));
        }
    }

    private void BtnCreateShutdown_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            bool ok = PersonalizationService.CreateShutdownShortcut();
            if (ok)
            {
                TxtStatus.Text = "✓ Raccourci 'Arrêter le PC' créé avec succès sur votre Bureau !";
                TxtStatus.Foreground = new SolidColorBrush(Color.FromRgb(0x34, 0xD3, 0x99));
                _onOutput("[OK] Raccourci 'Arrêter le PC' créé sur le Bureau avec son icône officielle.");
            }
            else
            {
                TxtStatus.Text = "⚠️ Impossible de créer le raccourci sur le Bureau.";
                TxtStatus.Foreground = new SolidColorBrush(Color.FromRgb(0xEF, 0x44, 0x44));
            }
            RefreshStatus();
        }
        catch (Exception ex)
        {
            TxtStatus.Text = $"Erreur : {ex.Message}";
            _onOutput($"[ERR] {ex.Message}");
        }
    }

    private void BtnCreateRestart_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            bool ok = PersonalizationService.CreateRestartShortcut();
            if (ok)
            {
                TxtStatus.Text = "✓ Raccourci 'Redémarrer le PC' créé avec succès sur votre Bureau !";
                TxtStatus.Foreground = new SolidColorBrush(Color.FromRgb(0x34, 0xD3, 0x99));
                _onOutput("[OK] Raccourci 'Redémarrer le PC' créé sur le Bureau avec son icône officielle.");
            }
            else
            {
                TxtStatus.Text = "⚠️ Impossible de créer le raccourci sur le Bureau.";
                TxtStatus.Foreground = new SolidColorBrush(Color.FromRgb(0xEF, 0x44, 0x44));
            }
            RefreshStatus();
        }
        catch (Exception ex)
        {
            TxtStatus.Text = $"Erreur : {ex.Message}";
            _onOutput($"[ERR] {ex.Message}");
        }
    }

    private void BtnDeleteShutdown_Click(object sender, RoutedEventArgs e)
    {
        PersonalizationService.RemoveShortcut(PersonalizationService.ShutdownShortcutName);
        TxtStatus.Text = "Raccourci 'Arrêter le PC' retiré du Bureau.";
        TxtStatus.Foreground = new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF));
        _onOutput("[INFO] Raccourci 'Arrêter le PC' supprimé du Bureau.");
        RefreshStatus();
    }

    private void BtnDeleteRestart_Click(object sender, RoutedEventArgs e)
    {
        PersonalizationService.RemoveShortcut(PersonalizationService.RestartShortcutName);
        TxtStatus.Text = "Raccourci 'Redémarrer le PC' retiré du Bureau.";
        TxtStatus.Foreground = new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF));
        _onOutput("[INFO] Raccourci 'Redémarrer le PC' supprimé du Bureau.");
        RefreshStatus();
    }

    private void BtnCreateAll_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            bool ok1 = PersonalizationService.CreateShutdownShortcut();
            bool ok2 = PersonalizationService.CreateRestartShortcut();

            if (ok1 && ok2)
            {
                TxtStatus.Text = "🎉 Boutons 'Arrêter' et 'Redémarrer' créés avec succès sur votre Bureau !";
                TxtStatus.Foreground = new SolidColorBrush(Color.FromRgb(0x34, 0xD3, 0x99));
                _onOutput("[SUCCÈS] Boutons Arrêter et Redémarrer installés sur le Bureau avec leurs icônes.");
            }
            else
            {
                TxtStatus.Text = "Création effectuée.";
            }
            RefreshStatus();
        }
        catch (Exception ex)
        {
            TxtStatus.Text = $"Erreur : {ex.Message}";
            _onOutput($"[ERR] {ex.Message}");
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
