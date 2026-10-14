using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Controls;

public partial class PrivacyShieldDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;

    private static readonly SolidColorBrush GreenBrush = new(Color.FromRgb(16, 185, 129));
    private static readonly SolidColorBrush OrangeBrush = new(Color.FromRgb(245, 158, 11));

    public PrivacyShieldDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput;
        _ct = ct;
        Loaded += async (s, e) => await RefreshStatusAsync();
    }

    private async Task RefreshStatusAsync()
    {
        TxtStatus.Text = "Vérification des paramètres système...";

        await Task.Run(() =>
        {
            // 1. DiagTrack
            bool diagTrackDisabled = false;
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\DiagTrack");
                var startVal = key?.GetValue("Start");
                if (startVal != null && (int)startVal == 4) diagTrackDisabled = true;
            }
            catch { }

            // 2. Advertising ID
            bool adDisabled = false;
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo");
                var enabledVal = key?.GetValue("Enabled");
                if (enabledVal != null && (int)enabledVal == 0) adDisabled = true;
            }
            catch { }

            // 3. Typing tracking
            bool typingRestricted = false;
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\InputPersonalization");
                var val = key?.GetValue("RestrictImplicitInkCollection");
                if (val != null && (int)val == 1) typingRestricted = true;
            }
            catch { }

            // 4. Start ads
            bool startAdsDisabled = false;
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager");
                var val = key?.GetValue("SystemPaneSuggestionsEnabled");
                if (val != null && (int)val == 0) startAdsDisabled = true;
            }
            catch { }

            Dispatcher.Invoke(() =>
            {
                UpdateLabel(TxtDiagTrackStatus, diagTrackDisabled, "Protégé (Désactivé)", "Actif (Non protégé)");
                UpdateLabel(TxtAdStatus, adDisabled, "Protégé (Désactivé)", "Actif (Pistage publicitaire)");
                UpdateLabel(TxtTypingStatus, typingRestricted, "Protégé (Bloqué)", "Actif (Enregistrement autorisé)");
                UpdateLabel(TxtStartAdsStatus, startAdsDisabled, "Protégé (Désactivé)", "Actif (Suggestions présentes)");

                bool fullProtected = diagTrackDisabled && adDisabled && typingRestricted && startAdsDisabled;
                bool allDefault = !diagTrackDisabled && !adDisabled && !typingRestricted && !startAdsDisabled;

                if (fullProtected)
                {
                    CardEnableShield.Visibility = Visibility.Collapsed;
                    CardRestoreDefaults.Visibility = Visibility.Visible;
                    TxtStatus.Text = "✅ Bouclier actif : votre système est entièrement sécurisé.";
                }
                else if (allDefault)
                {
                    CardEnableShield.Visibility = Visibility.Visible;
                    CardRestoreDefaults.Visibility = Visibility.Collapsed;
                    TxtStatus.Text = "ℹ️ Votre système partage actuellement des données avec Microsoft. Cliquez pour activer le bouclier.";
                }
                else
                {
                    CardEnableShield.Visibility = Visibility.Visible;
                    CardRestoreDefaults.Visibility = Visibility.Visible;
                    TxtStatus.Text = "ℹ️ Protection partielle : activez le bouclier complet ou rétablissez les valeurs par défaut.";
                }
            });
        });
    }

    private static void UpdateLabel(System.Windows.Controls.TextBlock tb, bool isProtected, string textProtected, string textUnprotected)
    {
        tb.Text = isProtected ? $"✅ {textProtected}" : $"⚠️ {textUnprotected}";
        tb.Foreground = isProtected ? GreenBrush : OrangeBrush;
    }

    private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
    {
        await RefreshStatusAsync();
    }

    private async void BtnEnableShield_Click(object sender, RoutedEventArgs e)
    {
        TxtStatus.Text = "Application du bouclier de confidentialité...";
        BtnEnableShield.IsEnabled = false;
        BtnRestoreDefaults.IsEnabled = false;

        _onOutput("🛡️ [CONFIDENTIALITÉ] Activation du bouclier de confidentialité...");

        try
        {
            await Task.Run(() =>
            {
                // Services (arrêt et désactivation)
                try { Process.Start(new ProcessStartInfo("sc.exe", "stop DiagTrack") { CreateNoWindow = true, UseShellExecute = false })?.WaitForExit(1000); } catch { }
                try { using var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\DiagTrack"); key?.SetValue("Start", 4, RegistryValueKind.DWord); } catch { }

                try { Process.Start(new ProcessStartInfo("sc.exe", "stop dmwappushservice") { CreateNoWindow = true, UseShellExecute = false })?.WaitForExit(1000); } catch { }
                try { using var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\dmwappushservice"); key?.SetValue("Start", 4, RegistryValueKind.DWord); } catch { }

                // Registres
                try
                {
                    using var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\DataCollection");
                    key?.SetValue("AllowTelemetry", 0, RegistryValueKind.DWord);
                    key?.SetValue("MaxTelemetryAllowed", 0, RegistryValueKind.DWord);
                }
                catch { }

                try
                {
                    using var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo");
                    key?.SetValue("Enabled", 0, RegistryValueKind.DWord);
                }
                catch { }

                try
                {
                    using var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\InputPersonalization");
                    key?.SetValue("RestrictImplicitInkCollection", 1, RegistryValueKind.DWord);
                    key?.SetValue("RestrictImplicitTextCollection", 1, RegistryValueKind.DWord);
                }
                catch { }

                try
                {
                    using var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager");
                    key?.SetValue("SystemPaneSuggestionsEnabled", 0, RegistryValueKind.DWord);
                    key?.SetValue("SubscribedContent-338388Enabled", 0, RegistryValueKind.DWord);
                    key?.SetValue("SubscribedContent-338389Enabled", 0, RegistryValueKind.DWord);
                }
                catch { }
            });

            _onOutput("[OK] Bouclier de confidentialité appliqué avec succès !");
            await RefreshStatusAsync();
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] Erreur lors de l'application : {ex.Message}");
            TxtStatus.Text = $"Erreur : {ex.Message}";
        }
        finally
        {
            BtnEnableShield.IsEnabled = true;
            BtnRestoreDefaults.IsEnabled = true;
        }
    }

    private async void BtnRestoreDefaults_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(this,
            "Voulez-vous rétablir les paramètres de télémétrie et de diagnostic par défaut de Windows ?",
            "Revenir en arrière", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        TxtStatus.Text = "Restauration des réglages par défaut de Windows...";
        BtnEnableShield.IsEnabled = false;
        BtnRestoreDefaults.IsEnabled = false;

        _onOutput("🔄 [RETOUR EN ARRIÈRE] Rétablissement des paramètres par défaut Windows...");

        try
        {
            await Task.Run(() =>
            {
                // Service DiagTrack remis en automatique
                try { using var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\DiagTrack"); key?.SetValue("Start", 2, RegistryValueKind.DWord); } catch { }
                try { using var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\dmwappushservice"); key?.SetValue("Start", 3, RegistryValueKind.DWord); } catch { }

                // Réinitialisation des registres
                try
                {
                    using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\DataCollection", true);
                    key?.DeleteValue("AllowTelemetry", false);
                    key?.DeleteValue("MaxTelemetryAllowed", false);
                }
                catch { }

                try
                {
                    using var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo");
                    key?.SetValue("Enabled", 1, RegistryValueKind.DWord);
                }
                catch { }

                try
                {
                    using var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\InputPersonalization");
                    key?.SetValue("RestrictImplicitInkCollection", 0, RegistryValueKind.DWord);
                    key?.SetValue("RestrictImplicitTextCollection", 0, RegistryValueKind.DWord);
                }
                catch { }

                try
                {
                    using var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager");
                    key?.SetValue("SystemPaneSuggestionsEnabled", 1, RegistryValueKind.DWord);
                }
                catch { }
            });

            _onOutput("[OK] Paramètres par défaut de Windows restaurés.");
            await RefreshStatusAsync();
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] Erreur : {ex.Message}");
            TxtStatus.Text = $"Erreur : {ex.Message}";
        }
        finally
        {
            BtnEnableShield.IsEnabled = true;
            BtnRestoreDefaults.IsEnabled = true;
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
