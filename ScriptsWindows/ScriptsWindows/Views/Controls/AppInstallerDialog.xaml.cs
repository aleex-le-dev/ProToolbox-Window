using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ScriptsWindows.Core;
using ScriptsWindows.Services;

namespace ScriptsWindows.Views.Controls;

public partial class AppInstallerDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;

    public ObservableCollection<AppItemModel> Apps { get; } = new();

    public AppInstallerDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput;
        _ct = ct;

        ItemsAppsList.ItemsSource = Apps;

        InitApps();
        UpdateSelectedCount();
    }

    private void InitApps()
    {
        Apps.Clear();
        Apps.Add(new AppItemModel("Google.Chrome", "Google Chrome", "Navigateur web rapide et sécurisé par Google", "🌐"));
        Apps.Add(new AppItemModel("VideoLAN.VLC", "VLC Media Player", "Lecteur multimédia universel lisant tous les formats", "🎬"));
        Apps.Add(new AppItemModel("SumatraPDF.SumatraPDF", "Sumatra PDF", "Lecteur PDF et eBooks ultra-léger et instantané", "📄"));
        Apps.Add(new AppItemModel("7zip.7zip", "7-Zip", "Gestionnaire d'archives haute compression léger et rapide", "🗜️"));
        Apps.Add(new AppItemModel("Microsoft.PowerShell", "PowerShell 7", "Version moderne multiplateforme de la console PowerShell", "⚡"));
        Apps.Add(new AppItemModel("SPECIAL_OFFICE", "Microsoft Office", "Téléchargement de l'installateur Microsoft Office (Google Drive)", "💼", isSpecialOffice: true, actionButtonText: "Télécharger"));
    }

    private void BtnSelectAll_Click(object sender, RoutedEventArgs e)
    {
        foreach (var app in Apps) app.IsSelected = true;
        UpdateSelectedCount();
    }

    private void BtnDeselectAll_Click(object sender, RoutedEventArgs e)
    {
        foreach (var app in Apps) app.IsSelected = false;
        UpdateSelectedCount();
    }

    private void ItemCheck_Changed(object sender, RoutedEventArgs e)
    {
        UpdateSelectedCount();
    }

    private void UpdateSelectedCount()
    {
        int count = Apps.Count(a => a.IsSelected);
        if (TxtSelectedCount != null)
            TxtSelectedCount.Text = $"{count} sélectionné(s)";
    }

    private async void BtnInstallSingle_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn || btn.Tag is not AppItemModel app) return;
        btn.IsEnabled = false;

        app.StatusText = "Installation...";
        app.StatusColor = new SolidColorBrush(Color.FromRgb(0xF5, 0x9E, 0x0B));
        TxtFeedback.Text = $"Traitement de {app.Name}...";

        bool ok = await InstallAppAsync(app);
        if (ok)
        {
            app.StatusText = "Installé ✅";
            app.StatusColor = new SolidColorBrush(Color.FromRgb(0x34, 0xD3, 0x99));
            TxtFeedback.Text = $"{app.Name} traité avec succès.";
        }
        else
        {
            app.StatusText = "Échoué ❌";
            app.StatusColor = new SolidColorBrush(Color.FromRgb(0xF8, 0x71, 0x71));
            TxtFeedback.Text = $"Échec pour {app.Name}.";
        }

        btn.IsEnabled = true;
    }

    private async void BtnInstallSelected_Click(object sender, RoutedEventArgs e)
    {
        var selected = Apps.Where(a => a.IsSelected).ToList();
        if (selected.Count == 0)
        {
            MessageBox.Show("Veuillez cocher au moins une application.", "Aucune sélection", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        BtnInstallSelected.IsEnabled = false;
        ProgressBatch.Visibility = Visibility.Visible;
        ProgressBatch.Maximum = selected.Count;
        ProgressBatch.Value = 0;

        int done = 0;
        int success = 0;

        foreach (var app in selected)
        {
            if (_ct.IsCancellationRequested) break;

            done++;
            ProgressBatch.Value = done;
            app.StatusText = "Installation...";
            app.StatusColor = new SolidColorBrush(Color.FromRgb(0xF5, 0x9E, 0x0B));
            TxtFeedback.Text = $"[{done}/{selected.Count}] Traitement de {app.Name}...";

            bool ok = await InstallAppAsync(app);
            if (ok)
            {
                success++;
                app.StatusText = "Installé ✅";
                app.StatusColor = new SolidColorBrush(Color.FromRgb(0x34, 0xD3, 0x99));
            }
            else
            {
                app.StatusText = "Échoué ❌";
                app.StatusColor = new SolidColorBrush(Color.FromRgb(0xF8, 0x71, 0x71));
            }
        }

        TxtFeedback.Text = $"Terminé : {success}/{selected.Count} application(s) traitée(s) avec succès.";
        BtnInstallSelected.IsEnabled = true;
        ProgressBatch.Visibility = Visibility.Collapsed;
    }

    private async Task<bool> InstallAppAsync(AppItemModel app)
    {
        try
        {
            if (app.IsSpecialOffice)
            {
                await AppService.InstallOffice(_onOutput, _ct);
                return true;
            }

            _onOutput($"[i] Installation de {app.Name} (Winget ID: {app.Id})...");
            string cmd = $"winget install --id {app.Id} --accept-package-agreements --accept-source-agreements --silent -e";
            string output = await CommandRunner.RunAsync(cmd, _ct);

            if (output.Contains("correctement") || output.Contains("succès") || output.Contains("Successfully") || output.Contains("already installed") || output.Contains("déjà installé"))
            {
                _onOutput($"[OK] {app.Name} installé avec succès");
                return true;
            }

            // Fallback sans mode silencieux si nécessaire
            if (output.Contains("Code de sortie : 1") || output.Contains("0x8007") || output.Contains("erreur"))
            {
                _onOutput($"[i] Nouvelle tentative standard pour {app.Name}...");
                string cmdFallback = $"winget install --id {app.Id} --accept-package-agreements --accept-source-agreements -e";
                string outFallback = await CommandRunner.RunAsync(cmdFallback, _ct);
                return !outFallback.Contains("Échec");
            }

            return true;
        }
        catch (Exception ex)
        {
            _onOutput($"[!] Erreur sur {app.Name} : {ex.Message}");
            return false;
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}

public class AppItemModel : INotifyPropertyChanged
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
    public bool IsSpecialOffice { get; set; }
    public string ActionButtonText { get; set; }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set { _isSelected = value; OnPropertyChanged(); }
    }

    private string _statusText = "Disponible";
    public string StatusText
    {
        get => _statusText;
        set { _statusText = value; OnPropertyChanged(); }
    }

    private Brush _statusColor = new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF));
    public Brush StatusColor
    {
        get => _statusColor;
        set { _statusColor = value; OnPropertyChanged(); }
    }

    public AppItemModel(string id, string name, string description, string icon, bool isSpecialOffice = false, string actionButtonText = "Installer")
    {
        Id = id;
        Name = name;
        Description = description;
        Icon = icon;
        IsSpecialOffice = isSpecialOffice;
        ActionButtonText = actionButtonText;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? prop = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
}
