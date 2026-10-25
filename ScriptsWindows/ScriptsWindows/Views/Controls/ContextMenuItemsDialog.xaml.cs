using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using ScriptsWindows.Services;

namespace ScriptsWindows.Views.Controls;

public partial class ContextMenuItemsDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;

    public ContextMenuItemsDialog(Action<string>? onOutput = null, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput ?? (_ => { });
        _ct = ct;

        Loaded += (s, e) => RefreshStatus();
    }

    private void RefreshStatus()
    {
        bool isClassic = IsClassicMenuEnabled();
        bool isCmdAdminActive = PersonalizationService.IsCmdAdminContextMenuEnabled();
        bool isPsAdminActive = PersonalizationService.IsPowerShellAdminContextMenuEnabled();

        // 1. Statut Menu Classique Windows 10
        if (isClassic)
        {
            BadgeClassicStatus.Background = new SolidColorBrush(Color.FromRgb(0x1E, 0x1B, 0x4B)); // Indigo foncé
            BadgeClassicStatus.BorderBrush = new SolidColorBrush(Color.FromRgb(0x63, 0x66, 0xF1));
            TxtClassicStatus.Text = "● Windows 10 Classique Actif";
            TxtClassicStatus.Foreground = new SolidColorBrush(Color.FromRgb(0x81, 0x8C, 0xF8));

            BtnToggleClassicMenu.Content = "↩ Rétablir menu Win 11";
            BtnToggleClassicMenu.Style = (Style)FindResource("DangerActionButton");
        }
        else
        {
            BadgeClassicStatus.Background = new SolidColorBrush(Color.FromRgb(0x11, 0x18, 0x27));
            BadgeClassicStatus.BorderBrush = new SolidColorBrush(Color.FromRgb(0x37, 0x41, 0x51));
            TxtClassicStatus.Text = "○ Windows 11 Moderne (Par défaut)";
            TxtClassicStatus.Foreground = new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF));

            BtnToggleClassicMenu.Content = "✓ Passer au menu Win 10";
            BtnToggleClassicMenu.Style = (Style)FindResource("IndigoActionButton");
        }

        // 2. Statut CMD Admin
        UpdateBadge(BadgeCmdAdminStatus, TxtCmdAdminStatus, isCmdAdminActive);
        if (isCmdAdminActive)
        {
            BtnToggleCmdAdmin.Content = "🗑️ Retirer du menu contextuel";
            BtnToggleCmdAdmin.Style = (Style)FindResource("DangerActionButton");
        }
        else
        {
            BtnToggleCmdAdmin.Content = "✓ Activer au clic droit (via .reg)";
            BtnToggleCmdAdmin.Style = (Style)FindResource("SuccessActionButton");
        }

        // 3. Statut PowerShell Admin
        UpdateBadge(BadgePowerShellAdminStatus, TxtPowerShellAdminStatus, isPsAdminActive);
        if (isPsAdminActive)
        {
            BtnTogglePowerShellAdmin.Content = "🗑️ Retirer du menu contextuel";
            BtnTogglePowerShellAdmin.Style = (Style)FindResource("DangerActionButton");
        }
        else
        {
            BtnTogglePowerShellAdmin.Content = "✓ Activer au clic droit (via .reg)";
            BtnTogglePowerShellAdmin.Style = (Style)FindResource("SuccessActionButton");
        }
    }

    private static bool IsClassicMenuEnabled()
    {
        try
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32");
            return key != null;
        }
        catch
        {
            return false;
        }
    }

    private async void BtnToggleClassicMenu_Click(object sender, RoutedEventArgs e)
    {
        BtnToggleClassicMenu.IsEnabled = false;
        try
        {
            bool isClassic = IsClassicMenuEnabled();
            if (isClassic)
            {
                TxtStatus.Text = "Rétablissement du menu moderne Windows 11...";
                _onOutput("[i] Rétablissement du menu contextuel moderne Windows 11...");
                await PersonalizationService.SetModernMenu(_onOutput, _ct);
                TxtStatus.Text = "Redémarrage de l'Explorateur Windows...";
                await PersonalizationService.RestartExplorer(_onOutput, _ct);
                TxtStatus.Text = "✓ Menu moderne Windows 11 d'origine restauré.";
                _onOutput("[OK] Menu moderne Windows 11 restauré.");
            }
            else
            {
                TxtStatus.Text = "Activation du menu classique Windows 10...";
                _onOutput("[i] Activation du grand menu classique Windows 10...");
                await PersonalizationService.SetClassicMenu(_onOutput, _ct);
                TxtStatus.Text = "Redémarrage de l'Explorateur Windows...";
                await PersonalizationService.RestartExplorer(_onOutput, _ct);
                TxtStatus.Text = "✓ Menu contextuel classique Windows 10 activé avec succès !";
                _onOutput("[OK] Menu classique Windows 10 activé.");
            }
            RefreshStatus();
        }
        catch (Exception ex)
        {
            TxtStatus.Text = $"Erreur : {ex.Message}";
            _onOutput($"[ERR] {ex.Message}");
        }
        finally
        {
            BtnToggleClassicMenu.IsEnabled = true;
        }
    }

    private async void BtnRestartExplorer_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            TxtStatus.Text = "Redémarrage de l'Explorateur Windows...";
            await PersonalizationService.RestartExplorer(_onOutput, _ct);
            TxtStatus.Text = "✓ Explorateur Windows redémarré avec succès.";
            RefreshStatus();
        }
        catch (Exception ex)
        {
            TxtStatus.Text = $"Erreur : {ex.Message}";
            _onOutput($"[ERR] {ex.Message}");
        }
    }

    private static void UpdateBadge(System.Windows.Controls.Border badge, System.Windows.Controls.TextBlock textBlock, bool exists)
    {
        if (exists)
        {
            badge.Background = new SolidColorBrush(Color.FromRgb(0x06, 0x5F, 0x46)); // Vert foncé
            badge.BorderBrush = new SolidColorBrush(Color.FromRgb(0x10, 0xB9, 0x81)); // Vert
            textBlock.Text = "● Actif dans le menu";
            textBlock.Foreground = new SolidColorBrush(Color.FromRgb(0x34, 0xD3, 0x99));
        }
        else
        {
            badge.Background = new SolidColorBrush(Color.FromRgb(0x11, 0x18, 0x27)); // Gris foncé
            badge.BorderBrush = new SolidColorBrush(Color.FromRgb(0x37, 0x41, 0x51));
            textBlock.Text = "○ Non actif";
            textBlock.Foreground = new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF));
        }
    }

    private void BtnToggleCmdAdmin_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            bool isAlreadyActive = PersonalizationService.IsCmdAdminContextMenuEnabled();

            if (isAlreadyActive)
            {
                // Suppression directe via l'interface
                bool removed = PersonalizationService.RemoveCmdAdminContextMenu();
                if (removed)
                {
                    TxtStatus.Text = "« Ouvrir CMD (Admin) » a été retiré de votre menu contextuel avec succès.";
                    TxtStatus.Foreground = new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF));
                    _onOutput("[INFO] « Ouvrir CMD (Admin) » retiré du menu contextuel (Bureau, dossiers, disques et bibliothèques).");
                }
                else
                {
                    TxtStatus.Text = "Opération incomplète : certaines clés nécessitent les droits administrateur.";
                    TxtStatus.Foreground = new SolidColorBrush(Color.FromRgb(0xEF, 0x44, 0x44));
                    _onOutput("[WARN] Échec du retrait de certaines clés du menu contextuel.");
                }
            }
            else
            {
                // Création du fichier .reg, ouverture via regedit pour confirmation, puis suppression automatique
                TxtStatus.Text = "Ouverture du fichier .reg dans l'Éditeur du Registre... Veuillez valider la fusion.";
                TxtStatus.Foreground = new SolidColorBrush(Color.FromRgb(0x38, 0xBD, 0xF8));

                bool success = PersonalizationService.InstallCmdAdminViaRegFile();

                if (success)
                {
                    TxtStatus.Text = "✓ Fichier .reg validé et supprimé ! « Ouvrir CMD (Admin) » est actif au clic droit.";
                    TxtStatus.Foreground = new SolidColorBrush(Color.FromRgb(0x34, 0xD3, 0x99));
                    _onOutput("[SUCCÈS] Fichier .reg importé et supprimé. « Ouvrir CMD (Admin) » configuré sur le Bureau et tous les dossiers.");
                }
                else
                {
                    TxtStatus.Text = "Opération annulée ou non validée dans l'Éditeur du Registre.";
                    TxtStatus.Foreground = new SolidColorBrush(Color.FromRgb(0xEF, 0x44, 0x44));
                }
            }

            RefreshStatus();
        }
        catch (Exception ex)
        {
            TxtStatus.Text = $"Erreur : {ex.Message}";
            _onOutput($"[ERR] {ex.Message}");
        }
    }

    private void BtnTogglePowerShellAdmin_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            bool isAlreadyActive = PersonalizationService.IsPowerShellAdminContextMenuEnabled();

            if (isAlreadyActive)
            {
                // Suppression directe via l'interface
                bool removed = PersonalizationService.RemovePowerShellAdminContextMenu();
                if (removed)
                {
                    TxtStatus.Text = "« Ouvrir PowerShell (Admin) » a été retiré de votre menu contextuel avec succès.";
                    TxtStatus.Foreground = new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF));
                    _onOutput("[INFO] « Ouvrir PowerShell (Admin) » retiré du menu contextuel (Bureau, dossiers, disques et bibliothèques).");
                }
                else
                {
                    TxtStatus.Text = "Opération incomplète : certaines clés nécessitent les droits administrateur.";
                    TxtStatus.Foreground = new SolidColorBrush(Color.FromRgb(0xEF, 0x44, 0x44));
                    _onOutput("[WARN] Échec du retrait de certaines clés du menu contextuel.");
                }
            }
            else
            {
                // Création du fichier .reg, ouverture via regedit pour confirmation, puis suppression automatique
                TxtStatus.Text = "Ouverture du fichier .reg dans l'Éditeur du Registre... Veuillez valider la fusion.";
                TxtStatus.Foreground = new SolidColorBrush(Color.FromRgb(0x38, 0xBD, 0xF8));

                bool success = PersonalizationService.InstallPowerShellAdminViaRegFile();

                if (success)
                {
                    TxtStatus.Text = "✓ Fichier .reg validé et supprimé ! « Ouvrir PowerShell (Admin) » est actif au clic droit.";
                    TxtStatus.Foreground = new SolidColorBrush(Color.FromRgb(0x34, 0xD3, 0x99));
                    _onOutput("[SUCCÈS] Fichier .reg importé et supprimé. « Ouvrir PowerShell (Admin) » configuré sur le Bureau et tous les dossiers.");
                }
                else
                {
                    TxtStatus.Text = "Opération annulée ou non validée dans l'Éditeur du Registre.";
                    TxtStatus.Foreground = new SolidColorBrush(Color.FromRgb(0xEF, 0x44, 0x44));
                }
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
