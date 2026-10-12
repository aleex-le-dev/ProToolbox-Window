using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using ScriptsWindows.Services;

namespace ScriptsWindows.Views.Controls;

public partial class ContextMenuDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;

    public ContextMenuDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput ?? (_ => { });
        _ct = ct;

        Loaded += (s, e) => RefreshStatus();
    }

    private void RefreshStatus()
    {
        bool isClassic = IsClassicMenuEnabled();
        if (isClassic)
        {
            TxtCurrentState.Text = "Menu Classique Windows 10 (Actif)";
            TxtCurrentState.Foreground = new SolidColorBrush(Color.FromRgb(0x63, 0x66, 0xF1)); // Indigo
            BtnApplyClassic.IsEnabled = false;
            BtnApplyModern.IsEnabled = true;
            TxtStatus.Text = "Le menu classique Windows 10 est actuellement activé.";
        }
        else
        {
            TxtCurrentState.Text = "Menu Moderne Windows 11 (Par défaut)";
            TxtCurrentState.Foreground = new SolidColorBrush(Color.FromRgb(0x10, 0xB9, 0x81)); // Vert
            BtnApplyClassic.IsEnabled = true;
            BtnApplyModern.IsEnabled = false;
            TxtStatus.Text = "Le menu moderne d'origine Windows 11 est actuellement activé.";
        }
    }

    public static bool IsClassicMenuEnabled()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32");
            return key != null;
        }
        catch
        {
            return false;
        }
    }

    private void BtnRefresh_Click(object sender, RoutedEventArgs e)
    {
        RefreshStatus();
    }

    private async void BtnApplyClassic_Click(object sender, RoutedEventArgs e)
    {
        BtnApplyClassic.IsEnabled = false;
        TxtStatus.Text = "Activation du menu classique Windows 10...";

        try
        {
            _onOutput("\n[i] Activation du menu contextuel classique Windows 10...");
            await PersonalizationService.SetClassicMenu(_onOutput, _ct);

            if (ChkRestartExplorer.IsChecked == true)
            {
                TxtStatus.Text = "Redémarrage de l'Explorateur Windows...";
                await PersonalizationService.RestartExplorer(_onOutput, _ct);
            }

            TxtStatus.Text = "Menu classique Windows 10 appliqué avec succès !";
            _onOutput("[OK] Menu classique Windows 10 configuré.");
            RefreshStatus();
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] {ex.Message}");
            TxtStatus.Text = $"Erreur : {ex.Message}";
            BtnApplyClassic.IsEnabled = true;
        }
    }

    private async void BtnApplyModern_Click(object sender, RoutedEventArgs e)
    {
        BtnApplyModern.IsEnabled = false;
        TxtStatus.Text = "Rétablissement du menu moderne Windows 11...";

        try
        {
            _onOutput("\n[i] Rétablissement du menu contextuel moderne Windows 11 (remise en arrière)...");
            await PersonalizationService.SetModernMenu(_onOutput, _ct);

            if (ChkRestartExplorer.IsChecked == true)
            {
                TxtStatus.Text = "Redémarrage de l'Explorateur Windows...";
                await PersonalizationService.RestartExplorer(_onOutput, _ct);
            }

            TxtStatus.Text = "Menu moderne Windows 11 rétabli avec succès !";
            _onOutput("[OK] Menu moderne Windows 11 d'origine restauré.");
            RefreshStatus();
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] {ex.Message}");
            TxtStatus.Text = $"Erreur : {ex.Message}";
            BtnApplyModern.IsEnabled = true;
        }
    }

    private async void BtnRestartExplorer_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            TxtStatus.Text = "Redémarrage de l'Explorateur...";
            await PersonalizationService.RestartExplorer(_onOutput, _ct);
            TxtStatus.Text = "Explorateur Windows redémarré.";
            RefreshStatus();
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] {ex.Message}");
            TxtStatus.Text = $"Erreur : {ex.Message}";
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
