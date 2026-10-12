using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Controls;

public class DriveBitLockerItem
{
    public string Letter { get; set; } = "";
    public string Label { get; set; } = "";
    public string TotalSize { get; set; } = "";
    public string FreeSize { get; set; } = "";
    public string ProtectionStatus { get; set; } = "Inconnu";
    public string EncryptionMethod { get; set; } = "—";
    public string PercentEncrypted { get; set; } = "—";
    public string VolumeType { get; set; } = "Données";

    public override string ToString()
    {
        string labelInfo = string.IsNullOrWhiteSpace(Label) ? "" : $" [{Label}]";
        return $"Lecteur {Letter}:{labelInfo} ({TotalSize}) — {ProtectionStatus}";
    }
}

public partial class BitLockerDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;

    public BitLockerDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput;
        _ct = ct;

        Loaded += async (_, _) => await LoadDrivesAsync();
    }

    private async Task LoadDrivesAsync()
    {
        TxtFeedback.Text = "Détection des disques et analyse BitLocker en cours...";
        CmbDrives.IsEnabled = false;
        BtnRefresh.IsEnabled = false;

        try
        {
            var drives = await QueryBitLockerDrivesAsync();
            CmbDrives.Items.Clear();

            foreach (var d in drives)
            {
                CmbDrives.Items.Add(d);
            }

            if (CmbDrives.Items.Count > 0)
            {
                CmbDrives.SelectedIndex = 0;
            }
            else
            {
                TxtProtectionStatus.Text = "Aucun lecteur détecté";
                TxtFeedback.Text = "Aucun disque fixe avec lettre de lecteur n'a été trouvé.";
            }
        }
        catch (Exception ex)
        {
            TxtFeedback.Text = $"Erreur d'analyse : {ex.Message}";
        }
        finally
        {
            CmbDrives.IsEnabled = true;
            BtnRefresh.IsEnabled = true;
        }
    }

    private async Task<List<DriveBitLockerItem>> QueryBitLockerDrivesAsync()
    {
        var items = new List<DriveBitLockerItem>();

        // 1. Détection des lecteurs fixes
        var driveDrives = DriveInfo.GetDrives()
            .Where(d => d.DriveType == DriveType.Fixed && d.IsReady)
            .ToList();

        foreach (var d in driveDrives)
        {
            string letter = d.Name.TrimEnd('\\').TrimEnd(':');
            double totalGb = Math.Round(d.TotalSize / (1024.0 * 1024.0 * 1024.0), 1);
            double freeGb  = Math.Round(d.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0), 1);

            var item = new DriveBitLockerItem
            {
                Letter = letter,
                Label = d.VolumeLabel,
                TotalSize = $"{totalGb} Go",
                FreeSize = $"{freeGb} Go",
                VolumeType = letter.Equals("C", StringComparison.OrdinalIgnoreCase) ? "Système d'exploitation" : "Volume de données fixe"
            };

            // 2. Interrogation PowerShell du statut BitLocker pour ce lecteur
            try
            {
                string script = $@"
$v = try {{ Get-BitLockerVolume -MountPoint '{letter}:' -ErrorAction Stop }} catch {{ $null }}
if ($v) {{
    $prot = if ($v.ProtectionStatus -eq 'On') {{ 'ACTIVÉ' }} elseif ($v.ProtectionStatus -eq 'Off') {{ 'DÉSACTIVÉ' }} else {{ ""$($v.ProtectionStatus)"" }}
    [PSCustomObject]@{{
        Prot = $prot
        Method = ""$($v.EncryptionMethod)""
        Percent = ""$($v.EncryptionPercentage)%""
        Type = ""$($v.VolumeType)""
    }} | ConvertTo-Json -Compress
}} else {{
    # Fallback manage-bde
    $mbde = try {{ (manage-bde -status '{letter}:' | Out-String) }} catch {{ '' }}
    if ($mbde -match 'Protection\s*:\s*Activer|Protection Status:\s*Protection On') {{
        [PSCustomObject]@{{ Prot = 'ACTIVÉ'; Method = 'Chiffré'; Percent = '100%'; Type = 'Fixe' }} | ConvertTo-Json -Compress
    }} elseif ($mbde -match 'Protection\s*:\s*Désactiver|Protection Status:\s*Protection Off') {{
        [PSCustomObject]@{{ Prot = 'DÉSACTIVÉ'; Method = 'Non chiffré'; Percent = '0%'; Type = 'Fixe' }} | ConvertTo-Json -Compress
    }} else {{
        '{{}}'
    }}
}}";
                string json = await PowerShellRunner.RunAsync(script, _ct);
                json = json.Trim();

                if (!string.IsNullOrWhiteSpace(json) && json.StartsWith("{"))
                {
                    using var doc = JsonDocument.Parse(json);
                    var root = doc.RootElement;
                    if (root.TryGetProperty("Prot", out var p) && p.GetString() != null)
                        item.ProtectionStatus = p.GetString()!;
                    if (root.TryGetProperty("Method", out var m) && m.GetString() != null)
                        item.EncryptionMethod = m.GetString()!;
                    if (root.TryGetProperty("Percent", out var pct) && pct.GetString() != null)
                        item.PercentEncrypted = pct.GetString()!;
                    if (root.TryGetProperty("Type", out var t) && !string.IsNullOrWhiteSpace(t.GetString()))
                        item.VolumeType = t.GetString()!;
                }
            }
            catch
            {
                item.ProtectionStatus = "Non vérifié";
            }

            items.Add(item);
        }

        return items;
    }

    private void CmbDrives_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateSelectedDriveDisplay();
    }

    private void UpdateSelectedDriveDisplay()
    {
        if (CmbDrives.SelectedItem is not DriveBitLockerItem item)
        {
            TxtProtectionStatus.Text = "—";
            TxtEncryptionMethod.Text = "—";
            TxtPercentEncrypted.Text = "—";
            TxtVolumeType.Text       = "—";
            return;
        }

        bool isProtected = item.ProtectionStatus.Contains("ACTIVÉ", StringComparison.OrdinalIgnoreCase) ||
                           item.ProtectionStatus.Contains("On", StringComparison.OrdinalIgnoreCase);

        TxtProtectionStatus.Text = isProtected ? "ACTIVÉ (Protégé)" : "DÉSACTIVÉ (Non protégé)";
        BadgeProtection.Background = isProtected ? new SolidColorBrush(Color.FromRgb(16, 185, 129)) : new SolidColorBrush(Color.FromRgb(239, 68, 68));

        TxtEncryptionMethod.Text = item.EncryptionMethod;
        TxtPercentEncrypted.Text = item.PercentEncrypted;
        TxtVolumeType.Text       = item.VolumeType;

        BtnEnable.IsEnabled  = !isProtected;
        BtnDisable.IsEnabled = isProtected;
        BtnSuspend.Content   = isProtected ? "⏸️ Suspendre protection" : "▶️ Reprendre protection";
        TxtFeedback.Text     = $"Lecteur {item.Letter}: sélectionné ({item.TotalSize}).";
    }

    private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
    {
        await LoadDrivesAsync();
    }

    private async void BtnDisable_Click(object sender, RoutedEventArgs e)
    {
        if (CmbDrives.SelectedItem is not DriveBitLockerItem item) return;

        var confirm = MessageBox.Show(
            $"Êtes-vous sûr de vouloir DÉSACTIVER BitLocker et déchiffrer le lecteur {item.Letter}: ?\n\n" +
            "Le déchiffrement va s'exécuter en arrière-plan sans bloquer votre système.",
            $"Désactiver BitLocker sur {item.Letter}:",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (confirm != MessageBoxResult.Yes) return;

        _onOutput($"\n[i] Désactivation de BitLocker demandée pour le lecteur {item.Letter}:...");
        TxtFeedback.Text = $"Lancement du déchiffrement de {item.Letter}:...";

        try
        {
            await CommandRunner.RunAsync($"manage-bde -off {item.Letter}:", _onOutput, _ct);
            _onOutput($"[OK] Déchiffrement du lecteur {item.Letter}: initialisé.");
            TxtFeedback.Text = $"Déchiffrement de {item.Letter}: lancé avec succès.";
            await LoadDrivesAsync();
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] Échec lors de la désactivation : {ex.Message}");
            TxtFeedback.Text = $"Erreur : {ex.Message}";
        }
    }

    private async void BtnEnable_Click(object sender, RoutedEventArgs e)
    {
        if (CmbDrives.SelectedItem is not DriveBitLockerItem item) return;

        _onOutput($"\n[i] Activation de BitLocker sur le lecteur {item.Letter}:...");
        TxtFeedback.Text = $"Activation de BitLocker sur {item.Letter}:...";

        try
        {
            // Tente l'activation directe avec protecteur automatique ou ouvre l'interface certifiée si prérequis requis
            await CommandRunner.RunAsync($"manage-bde -on {item.Letter}: -UsedSpaceOnly", _onOutput, _ct);
            _onOutput($"[i] Vérification de l'assistant de configuration BitLocker Windows...");
            CommandRunner.StartProcess("control.exe", "/name Microsoft.BitLockerDriveEncryption");
            TxtFeedback.Text = $"Assistant BitLocker lancé pour le lecteur {item.Letter}:.";
            await LoadDrivesAsync();
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] Échec de l'activation : {ex.Message}");
            TxtFeedback.Text = $"Erreur : {ex.Message}";
        }
    }

    private async void BtnSuspend_Click(object sender, RoutedEventArgs e)
    {
        if (CmbDrives.SelectedItem is not DriveBitLockerItem item) return;

        bool isProtected = item.ProtectionStatus.Contains("ACTIVÉ", StringComparison.OrdinalIgnoreCase);

        if (isProtected)
        {
            _onOutput($"\n[i] Suspension temporaire de la protection BitLocker sur {item.Letter}: (pour maintenance ou flash BIOS)...");
            await CommandRunner.RunAsync($"manage-bde -protectors -disable {item.Letter}: -RebootCount 1", _onOutput, _ct);
            _onOutput($"[OK] Protection suspendue sur {item.Letter}: pour le prochain redémarrage.");
        }
        else
        {
            _onOutput($"\n[i] Reprise de la protection BitLocker sur {item.Letter}:...");
            await CommandRunner.RunAsync($"manage-bde -protectors -enable {item.Letter}:", _onOutput, _ct);
            _onOutput($"[OK] Protection réactivée sur {item.Letter}:.");
        }

        await LoadDrivesAsync();
    }

    private void BtnOpenPanel_Click(object sender, RoutedEventArgs e)
    {
        _onOutput("\n[i] Ouverture du Panneau de configuration Chiffrement de lecteur BitLocker...");
        CommandRunner.StartProcess("control.exe", "/name Microsoft.BitLockerDriveEncryption");
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
