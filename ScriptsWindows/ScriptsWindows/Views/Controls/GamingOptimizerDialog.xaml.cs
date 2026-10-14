using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Controls;

public partial class GamingOptimizerDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;

    private static readonly SolidColorBrush GreenBrush = new(Color.FromRgb(16, 185, 129));
    private static readonly SolidColorBrush OrangeBrush = new(Color.FromRgb(245, 158, 11));

    public GamingOptimizerDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput;
        _ct = ct;
        Loaded += async (s, e) => await RefreshStatusAsync();
    }

    private async Task RefreshStatusAsync()
    {
        TxtStatus.Text = "Analyse des réglages de performances...";

        await Task.Run(() =>
        {
            // 1. Mode Jeu
            bool isGameMode = false;
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\GameBar");
                var val = key?.GetValue("AutoGameModeEnabled");
                if (val != null && (int)val == 1) isGameMode = true;
            }
            catch { }

            // 2. CPU / SystemProfile
            bool isMaxCpu = false;
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile");
                var val = key?.GetValue("SystemResponsiveness");
                if (val != null && (int)val == 0) isMaxCpu = true;
            }
            catch { }

            // 3. Nagle / Latence
            bool isLowLatencyTcp = false;
            try
            {
                using var parent = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces");
                if (parent != null)
                {
                    foreach (var sub in parent.GetSubKeyNames())
                    {
                        using var iface = parent.OpenSubKey(sub);
                        var ack = iface?.GetValue("TcpAckFrequency");
                        if (ack != null && (int)ack == 1)
                        {
                            isLowLatencyTcp = true;
                            break;
                        }
                    }
                }
            }
            catch { }

            // 4. HAGS
            bool isHags = false;
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\GraphicsDrivers");
                var val = key?.GetValue("HwSchMode");
                if (val != null && (int)val == 2) isHags = true;
            }
            catch { }

            Dispatcher.Invoke(() =>
            {
                UpdateLabel(TxtGameModeStatus, isGameMode, "Activé (Optimisé)", "Désactivé / Standard");
                UpdateLabel(TxtCpuPriorityStatus, isMaxCpu, "100% Jeux (Zéro bride)", "Standard (Réservé 20% tâches de fond)");
                UpdateLabel(TxtNagleStatus, isLowLatencyTcp, "Ultra-rapide (Nagle désactivé)", "Standard (Délai d'accusé actif)");
                UpdateLabel(TxtHagsStatus, isHags, "Activé (Matériel GPU)", "Standard / Par défaut");

                bool fullOptimized = isGameMode && isMaxCpu && isLowLatencyTcp;
                bool allDefault = !isGameMode && !isMaxCpu && !isLowLatencyTcp;

                if (fullOptimized)
                {
                    CardEnableGaming.Visibility = Visibility.Collapsed;
                    CardRestoreDefaults.Visibility = Visibility.Visible;
                    TxtStatus.Text = "🚀 Mode Gaming Ultime actif : réactivité et performances maximales.";
                }
                else if (allDefault)
                {
                    CardEnableGaming.Visibility = Visibility.Visible;
                    CardRestoreDefaults.Visibility = Visibility.Collapsed;
                    TxtStatus.Text = "ℹ️ Configuration standard Windows active. Cliquez pour activer le boost.";
                }
                else
                {
                    CardEnableGaming.Visibility = Visibility.Visible;
                    CardRestoreDefaults.Visibility = Visibility.Visible;
                    TxtStatus.Text = "ℹ️ Optimisation partielle. Activez le boost complet ou rétablissez les valeurs d'origine.";
                }
            });
        });
    }

    private static void UpdateLabel(System.Windows.Controls.TextBlock tb, bool isBoosted, string textBoosted, string textDefault)
    {
        tb.Text = isBoosted ? $"✅ {textBoosted}" : $"⚠️ {textDefault}";
        tb.Foreground = isBoosted ? GreenBrush : OrangeBrush;
    }

    private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
    {
        await RefreshStatusAsync();
    }

    private async void BtnEnableGaming_Click(object sender, RoutedEventArgs e)
    {
        TxtStatus.Text = "Application des optimisations gaming...";
        BtnEnableGaming.IsEnabled = false;
        BtnRestoreDefaults.IsEnabled = false;

        _onOutput("⚡ [GAMING] Application du Mode Gaming Ultime & Réduction de latence...");

        try
        {
            await Task.Run(() =>
            {
                // Game Mode
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\GameBar"))
                {
                    key?.SetValue("AutoGameModeEnabled", 1, RegistryValueKind.DWord);
                    key?.SetValue("AllowAutoGameMode", 1, RegistryValueKind.DWord);
                }

                // System Profile
                using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile"))
                {
                    key?.SetValue("SystemResponsiveness", 0, RegistryValueKind.DWord);
                    key?.SetValue("NetworkThrottlingIndex", unchecked((int)0xffffffff), RegistryValueKind.DWord);
                }

                using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games"))
                {
                    key?.SetValue("GPU Priority", 8, RegistryValueKind.DWord);
                    key?.SetValue("Priority", 6, RegistryValueKind.DWord);
                    key?.SetValue("Scheduling Category", "High", RegistryValueKind.String);
                }

                // TCP Nagle Disable
                using (var parent = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces", true))
                {
                    if (parent != null)
                    {
                        foreach (var sub in parent.GetSubKeyNames())
                        {
                            using var iface = parent.OpenSubKey(sub, true);
                            if (iface?.GetValue("DhcpIPAddress") != null || iface?.GetValue("IPAddress") != null)
                            {
                                iface.SetValue("TcpAckFrequency", 1, RegistryValueKind.DWord);
                                iface.SetValue("TCPNoDelay", 1, RegistryValueKind.DWord);
                            }
                        }
                    }
                }

                // HAGS
                using (var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\GraphicsDrivers"))
                {
                    key?.SetValue("HwSchMode", 2, RegistryValueKind.DWord);
                }
            });

            _onOutput("[OK] Mode Gaming Ultime configuré avec succès !");
            await RefreshStatusAsync();
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] Erreur : {ex.Message}");
            TxtStatus.Text = $"Erreur : {ex.Message}";
        }
        finally
        {
            BtnEnableGaming.IsEnabled = true;
            BtnRestoreDefaults.IsEnabled = true;
        }
    }

    private async void BtnRestoreDefaults_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(this,
            "Souhaitez-vous restaurer les paramètres standards de Windows pour la pile réseau et le planificateur multimédia ?",
            "Revenir en arrière", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        TxtStatus.Text = "Restauration des réglages par défaut...";
        BtnEnableGaming.IsEnabled = false;
        BtnRestoreDefaults.IsEnabled = false;

        _onOutput("🔄 [RETOUR EN ARRIÈRE] Rétablissement des réglages de latence par défaut Windows...");

        try
        {
            await Task.Run(() =>
            {
                // System Profile par défaut (20%)
                using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile"))
                {
                    key?.SetValue("SystemResponsiveness", 20, RegistryValueKind.DWord);
                    key?.DeleteValue("NetworkThrottlingIndex", false);
                }

                using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games"))
                {
                    key?.SetValue("GPU Priority", 8, RegistryValueKind.DWord);
                    key?.SetValue("Priority", 2, RegistryValueKind.DWord);
                    key?.SetValue("Scheduling Category", "Medium", RegistryValueKind.String);
                }

                // Supprimer les overrides Nagle
                using (var parent = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces", true))
                {
                    if (parent != null)
                    {
                        foreach (var sub in parent.GetSubKeyNames())
                        {
                            using var iface = parent.OpenSubKey(sub, true);
                            iface?.DeleteValue("TcpAckFrequency", false);
                            iface?.DeleteValue("TCPNoDelay", false);
                        }
                    }
                }
            });

            _onOutput("[OK] Réglages par défaut de Windows restaurés.");
            await RefreshStatusAsync();
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] Erreur : {ex.Message}");
            TxtStatus.Text = $"Erreur : {ex.Message}";
        }
        finally
        {
            BtnEnableGaming.IsEnabled = true;
            BtnRestoreDefaults.IsEnabled = true;
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
