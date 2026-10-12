using System.IO;
using System.Windows;
using System.Windows.Controls;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Controls;

public class ChkdskDriveItem
{
    public string Letter { get; set; } = "";
    public string Label { get; set; } = "";
    public string SizeInfo { get; set; } = "";

    public override string ToString()
    {
        string labelPart = string.IsNullOrWhiteSpace(Label) ? "" : $" [{Label}]";
        return $"Lecteur {Letter}:{labelPart} ({SizeInfo})";
    }
}

public partial class ChkdskDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;

    public ChkdskDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput;
        _ct = ct;

        Loaded += (_, _) => LoadDrives();
    }

    private void LoadDrives()
    {
        CmbDrives.Items.Clear();
        var fixedDrives = DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed && d.IsReady);

        foreach (var d in fixedDrives)
        {
            string letter = d.Name.TrimEnd('\\').TrimEnd(':');
            double totalGb = Math.Round(d.TotalSize / (1024.0 * 1024.0 * 1024.0), 1);
            double freeGb  = Math.Round(d.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0), 1);

            CmbDrives.Items.Add(new ChkdskDriveItem
            {
                Letter = letter,
                Label = d.VolumeLabel,
                SizeInfo = $"{freeGb} Go libres sur {totalGb} Go"
            });
        }

        if (CmbDrives.Items.Count > 0)
        {
            CmbDrives.SelectedIndex = 0;
        }
    }

    private async void CmbDrives_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CmbDrives.SelectedItem is not ChkdskDriveItem item) return;

        TxtDriveDirtyStatus.Text = $"Vérification de l'intégrité de {item.Letter}:...";
        try
        {
            string res = await CommandRunner.RunAsync($"fsutil dirty query {item.Letter}:", _ct);
            if (res.Contains("NON intègre", StringComparison.OrdinalIgnoreCase) || res.Contains("is dirty", StringComparison.OrdinalIgnoreCase))
            {
                TxtDriveDirtyStatus.Text = $"⚠️ Le lecteur {item.Letter}: est marqué comme endommagé (dirty bit actif). Analyse recommandée.";
                TxtDriveDirtyStatus.Foreground = System.Windows.Media.Brushes.Orange;
            }
            else
            {
                TxtDriveDirtyStatus.Text = $"✅ Le volume {item.Letter}: est marqué comme intègre par Windows.";
                TxtDriveDirtyStatus.Foreground = System.Windows.Media.Brushes.LightGreen;
            }
        }
        catch
        {
            TxtDriveDirtyStatus.Text = $"État du volume {item.Letter}: prêt pour analyse.";
            TxtDriveDirtyStatus.Foreground = System.Windows.Media.Brushes.LightGray;
        }
    }

    private async void BtnRunChkdsk_Click(object sender, RoutedEventArgs e)
    {
        if (CmbDrives.SelectedItem is not ChkdskDriveItem item) return;

        BtnRunChkdsk.IsEnabled = false;
        TxtFeedback.Text = "Lancement de la commande CHKDSK...";

        try
        {
            if (RbReadOnly.IsChecked == true)
            {
                _onOutput($"\n[i] Lancement de l'analyse en lecture seule sur {item.Letter}: (sans modification)...");
                await CommandRunner.RunAsync($"chkdsk {item.Letter}:", _onOutput, _ct);
                _onOutput($"[OK] Analyse en lecture seule sur {item.Letter}: terminée.");
                TxtFeedback.Text = $"Analyse de {item.Letter}: terminée.";
            }
            else if (RbFix.IsChecked == true)
            {
                _onOutput($"\n[i] Planification de la réparation des erreurs (CHKDSK /f) sur {item.Letter}:...");
                await CommandRunner.RunAsync($"echo O | chkdsk {item.Letter}: /f", _onOutput, _ct);
                _onOutput($"[OK] Réparation programmée sur {item.Letter}: pour le prochain redémarrage.");
                TxtFeedback.Text = $"Réparation /f programmée sur {item.Letter}: au redémarrage.";
            }
            else
            {
                _onOutput($"\n[i] Planification de la réparation complète et récupération des secteurs (CHKDSK /f /r) sur {item.Letter}:...");
                await CommandRunner.RunAsync($"echo O | chkdsk {item.Letter}: /f /r", _onOutput, _ct);
                _onOutput($"[OK] Scan complet et récupération programmés sur {item.Letter}: pour le prochain redémarrage.");
                TxtFeedback.Text = $"Scan complet /f /r programmé sur {item.Letter}:.";
            }
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] Échec CHKDSK : {ex.Message}");
            TxtFeedback.Text = $"Erreur : {ex.Message}";
        }
        finally
        {
            BtnRunChkdsk.IsEnabled = true;
        }
    }

    private async void BtnCancelSchedule_Click(object sender, RoutedEventArgs e)
    {
        if (CmbDrives.SelectedItem is not ChkdskDriveItem item) return;

        _onOutput($"\n[i] Annulation de la planification de vérification au démarrage pour {item.Letter}:...");
        try
        {
            await CommandRunner.RunAsync($"chkntfs /d", _onOutput, _ct);
            _onOutput($"[OK] Planification réinitialisée aux paramètres par défaut (analyse annulée si volume intègre).");
            TxtFeedback.Text = "Planification CHKDSK réinitialisée.";
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] {ex.Message}");
            TxtFeedback.Text = $"Erreur : {ex.Message}";
        }
    }
}
