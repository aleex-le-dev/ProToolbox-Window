using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using ScriptsWindows.Services;

using ScriptsWindows.Core;

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
            badge.Background = (SolidColorBrush)ThemeManager.GetBrush("SuccessBg"); // Vert foncé
            badge.BorderBrush = (SolidColorBrush)ThemeManager.GetBrush("Success"); // Vert
            textBlock.Text = "● Sur le Bureau";
            textBlock.Foreground = (SolidColorBrush)ThemeManager.GetBrush("SuccessText");
        }
        else
        {
            badge.Background = (SolidColorBrush)ThemeManager.GetBrush("BgInput"); // Gris foncé
            badge.BorderBrush = (SolidColorBrush)ThemeManager.GetBrush("Border");
            textBlock.Text = "○ Non installé";
            textBlock.Foreground = (SolidColorBrush)ThemeManager.GetBrush("TextSecondary");
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
                TxtStatus.Foreground = (SolidColorBrush)ThemeManager.GetBrush("SuccessText");
                _onOutput("[OK] Raccourci 'Arrêter le PC' créé sur le Bureau avec son icône officielle.");
            }
            else
            {
                TxtStatus.Text = "⚠️ Impossible de créer le raccourci sur le Bureau.";
                TxtStatus.Foreground = (SolidColorBrush)ThemeManager.GetBrush("DangerText");
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
                TxtStatus.Foreground = (SolidColorBrush)ThemeManager.GetBrush("SuccessText");
                _onOutput("[OK] Raccourci 'Redémarrer le PC' créé sur le Bureau avec son icône officielle.");
            }
            else
            {
                TxtStatus.Text = "⚠️ Impossible de créer le raccourci sur le Bureau.";
                TxtStatus.Foreground = (SolidColorBrush)ThemeManager.GetBrush("DangerText");
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
        TxtStatus.Foreground = (SolidColorBrush)ThemeManager.GetBrush("TextSecondary");
        _onOutput("[INFO] Raccourci 'Arrêter le PC' supprimé du Bureau.");
        RefreshStatus();
    }

    private void BtnDeleteRestart_Click(object sender, RoutedEventArgs e)
    {
        PersonalizationService.RemoveShortcut(PersonalizationService.RestartShortcutName);
        TxtStatus.Text = "Raccourci 'Redémarrer le PC' retiré du Bureau.";
        TxtStatus.Foreground = (SolidColorBrush)ThemeManager.GetBrush("TextSecondary");
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
                TxtStatus.Foreground = (SolidColorBrush)ThemeManager.GetBrush("SuccessText");
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
