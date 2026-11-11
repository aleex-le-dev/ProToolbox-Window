using System.Windows;
using ScriptsWindows.Services;

namespace ScriptsWindows.Views.Controls;

/// <summary>Suppression du bloatware (applications Store préinstallées), pré-sélection du bloatware connu uniquement.</summary>
public partial class DebloatDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;
    private List<AppxApp> _apps = new();
    private bool _busy;

    public DebloatDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput ?? (_ => { });
        _ct = ct;
        Loaded += async (_, _) => await LoadAsync();
    }

    private async Task LoadAsync()
    {
        SetBusy(true, "Chargement des applications…");
        try
        {
            _apps = await DebloatService.GetAppsAsync(_ct);
            foreach (var a in _apps) a.PropertyChanged += (_, _) => UpdateStatus();
            ApplyFilter();
            SetBusy(false, "");
            UpdateStatus();
        }
        catch (Exception ex)
        {
            SetBusy(false, $"Impossible de lister les applications : {ex.Message}");
        }
    }

    private void ApplyFilter()
    {
        var shown = ChkOnlyRecommended.IsChecked == true ? _apps.Where(a => a.IsRecommended).ToList() : _apps;
        ListApps.ItemsSource = shown;
        PanelEmpty.Visibility = shown.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void UpdateStatus()
    {
        if (_busy) return;
        int sel = _apps.Count(a => a.IsSelected);
        int reco = _apps.Count(a => a.IsRecommended);
        TxtStatus.Text = $"{sel} sélectionnée(s) • {reco} bloatware connu(s) sur {_apps.Count} application(s) supprimable(s)";
        BtnRemove.IsEnabled = sel > 0;
    }

    private void SetBusy(bool busy, string status)
    {
        _busy = busy;
        ProgBar.Visibility = busy ? Visibility.Visible : Visibility.Collapsed;
        BtnRemove.IsEnabled = !busy;
        ListApps.IsEnabled = !busy;
        if (status.Length > 0) TxtStatus.Text = status;
    }

    private void ChkOnlyRecommended_Changed(object sender, RoutedEventArgs e) { if (IsLoaded) ApplyFilter(); }
    private void BtnSelectRecommended_Click(object sender, RoutedEventArgs e) { foreach (var a in _apps) a.IsSelected = a.IsRecommended; }
    private void BtnSelectNone_Click(object sender, RoutedEventArgs e) { foreach (var a in _apps) a.IsSelected = false; }

    private async void BtnRemove_Click(object sender, RoutedEventArgs e)
    {
        var selected = _apps.Where(a => a.IsSelected).ToList();
        if (selected.Count == 0) return;
        bool allUsers = ChkAllUsers.IsChecked == true;
        string list = string.Join("\n", selected.Take(15).Select(a => "• " + a.FriendlyName)) + (selected.Count > 15 ? $"\n… et {selected.Count - 15} autre(s)" : "");
        if (MessageBox.Show(this, $"Supprimer {selected.Count} application(s) ?\n\n{list}\n\n{(allUsers ? "Pour tous les comptes, sans réinstallation automatique." : "Pour le compte actuel uniquement.")}\nElles restent réinstallables depuis le Microsoft Store.",
                            "Supprimer les applications", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

        SetBusy(true, $"Suppression de {selected.Count} application(s)…");
        _onOutput($"\n🚀 [LANCEMENT] Suppression de {selected.Count} application(s) préinstallée(s)");
        try { await DebloatService.RemoveAsync(selected, allUsers, _onOutput, _ct); }
        catch (Exception ex) { _onOutput($"[ERR] {ex.Message}"); }
        _busy = false;
        await LoadAsync();
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
