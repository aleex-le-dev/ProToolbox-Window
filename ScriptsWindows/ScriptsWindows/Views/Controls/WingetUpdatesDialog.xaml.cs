using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Controls;

public class WingetUpdateItem : INotifyPropertyChanged
{
    private bool _isSelected = true;

    public string Name { get; set; } = "";
    public string Id { get; set; } = "";
    public string CurrentVersion { get; set; } = "";
    public string AvailableVersion { get; set; } = "";
    public string Source { get; set; } = "winget";

    public string VersionDisplay => $"Actuelle : {CurrentVersion}";
    public string AvailableDisplay => $"Nouvelle : {AvailableVersion}";

    public bool IsSelected
    {
        get => _isSelected;
        set { _isSelected = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class WingetInstalledItem
{
    public string Name { get; set; } = "";
    public string Id { get; set; } = "";
    public string Version { get; set; } = "";
}

public partial class WingetUpdatesDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;

    public ObservableCollection<WingetUpdateItem> Updates { get; } = new();
    private readonly List<WingetInstalledItem> _allInstalled = new();

    public WingetUpdatesDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput;
        _ct = ct;

        LstUpdates.ItemsSource = Updates;

        Loaded += async (_, _) => await CheckUpdatesAsync();
    }

    private async Task CheckUpdatesAsync()
    {
        TxtFeedback.Text = "Recherche des mises à jour applicatives via Winget...";
        TxtNoUpdates.Text = "Analyse en cours...";
        TxtNoUpdates.Visibility = Visibility.Visible;
        Updates.Clear();

        try
        {
            string output = await CommandRunner.RunAsync("winget upgrade", _ct);
            var lines = output.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            int headerIdx = -1;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("Nom", StringComparison.OrdinalIgnoreCase) || lines[i].StartsWith("Name", StringComparison.OrdinalIgnoreCase))
                {
                    headerIdx = i;
                    break;
                }
            }

            if (headerIdx >= 0 && headerIdx + 2 < lines.Length)
            {
                string header = lines[headerIdx];
                int idPos   = header.IndexOf("ID", StringComparison.OrdinalIgnoreCase);
                int verPos  = header.IndexOf("Version", StringComparison.OrdinalIgnoreCase);
                int dispPos = header.IndexOf("Disponible", StringComparison.OrdinalIgnoreCase);
                if (dispPos < 0) dispPos = header.IndexOf("Available", StringComparison.OrdinalIgnoreCase);
                int srcPos  = header.IndexOf("Source", StringComparison.OrdinalIgnoreCase);

                for (int i = headerIdx + 2; i < lines.Length; i++)
                {
                    string line = lines[i].TrimEnd();
                    if (string.IsNullOrWhiteSpace(line) || line.Contains("mises à niveau", StringComparison.OrdinalIgnoreCase) || line.Contains("upgrades available", StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (dispPos > 0 && line.Length > dispPos)
                    {
                        string name = idPos > 0 && line.Length > idPos ? line.Substring(0, idPos).Trim() : line.Trim();
                        string id   = verPos > 0 && line.Length > verPos ? line.Substring(idPos, verPos - idPos).Trim() : "";
                        string ver  = dispPos > 0 && line.Length > dispPos ? line.Substring(verPos, dispPos - verPos).Trim() : "";
                        string disp = srcPos > 0 && line.Length > srcPos ? line.Substring(dispPos, srcPos - dispPos).Trim() : line.Substring(dispPos).Trim();
                        string src  = srcPos > 0 && line.Length > srcPos ? line.Substring(srcPos).Trim() : "winget";

                        if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(id))
                        {
                            Updates.Add(new WingetUpdateItem
                            {
                                Name = name,
                                Id = id,
                                CurrentVersion = ver,
                                AvailableVersion = disp,
                                Source = src
                            });
                        }
                    }
                }
            }

            if (Updates.Count > 0)
            {
                TxtNoUpdates.Visibility = Visibility.Collapsed;
                TxtFeedback.Text = $"{Updates.Count} mise(s) à jour trouvée(s).";
            }
            else
            {
                TxtNoUpdates.Text = "Toutes vos applications sont à jour ! Aucune mise à niveau requise.";
                TxtFeedback.Text = "Système et applications à jour.";
            }
        }
        catch (Exception ex)
        {
            TxtNoUpdates.Text = $"Erreur lors de la détection : {ex.Message}";
            TxtFeedback.Text = $"Erreur : {ex.Message}";
        }
    }

    private async void BtnCheckUpdates_Click(object sender, RoutedEventArgs e)
        => await CheckUpdatesAsync();

    private void BtnSelectAll_Click(object sender, RoutedEventArgs e)
    {
        foreach (var u in Updates) u.IsSelected = true;
    }

    private void BtnDeselectAll_Click(object sender, RoutedEventArgs e)
    {
        foreach (var u in Updates) u.IsSelected = false;
    }

    private async void BtnUpdateSelected_Click(object sender, RoutedEventArgs e)
    {
        var selected = Updates.Where(u => u.IsSelected).ToList();
        if (selected.Count == 0)
        {
            MessageBox.Show("Veuillez cocher au moins une application à mettre à jour.", "Aucune sélection", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        BtnUpdateSelected.IsEnabled = false;
        _onOutput($"\n🚀 [LANCEMENT] Mise à jour de {selected.Count} application(s) sélectionnée(s)...");

        try
        {
            int count = 0;
            foreach (var app in selected)
            {
                await UpgradeApplicationAsync(app);
                count++;
            }

            _onOutput($"\n✅ [TERMINÉ] {count} application(s) traitée(s).");
            TxtFeedback.Text = "Mises à jour terminées.";
            await CheckUpdatesAsync();
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] Échec mise à jour : {ex.Message}");
            TxtFeedback.Text = $"Erreur : {ex.Message}";
        }
        finally
        {
            BtnUpdateSelected.IsEnabled = true;
        }
    }

    private async void BtnUpdateSingle_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn || btn.Tag is not WingetUpdateItem app) return;

        btn.IsEnabled = false;
        try
        {
            await UpgradeApplicationAsync(app);
            TxtFeedback.Text = $"{app.Name} mis à jour.";
            await CheckUpdatesAsync();
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] {ex.Message}");
            TxtFeedback.Text = $"Erreur : {ex.Message}";
        }
        finally
        {
            btn.IsEnabled = true;
        }
    }

    private async Task UpgradeApplicationAsync(WingetUpdateItem app)
    {
        bool force = ChkForceUpgrade.IsChecked == true;
        _onOutput($"\n[i] Mise à niveau de {app.Name} ({app.Id})...");
        TxtFeedback.Text = $"Mise à niveau de {app.Name}...";

        string srcArg = !string.IsNullOrWhiteSpace(app.Source) ? $"--source {app.Source}" : "";
        string upgradeCmd = $"winget upgrade --exact --id \"{app.Id}\" {srcArg} --include-unknown --accept-package-agreements --accept-source-agreements";

        bool scopeOrReqMismatch = false;
        Action<string> captureOutput = line =>
        {
            _onOutput(line);
            if (!string.IsNullOrEmpty(line))
            {
                if (line.Contains("ne s’applique pas à votre système", StringComparison.OrdinalIgnoreCase) ||
                    line.Contains("ne s'applique pas", StringComparison.OrdinalIgnoreCase) ||
                    line.Contains("Aucune mise à niveau applicable trouvée", StringComparison.OrdinalIgnoreCase))
                {
                    scopeOrReqMismatch = true;
                }
            }
        };

        await CommandRunner.RunAsync(upgradeCmd, captureOutput, _ct);

        if (scopeOrReqMismatch && force)
        {
            _onOutput($"\n[!] Conflit de portée (scope user vs machine) ou de type de package détecté pour {app.Name}.");
            _onOutput($"[⚡ MODE FORCÉ] Contournement via install --force : winget install --exact --id \"{app.Id}\" --force ...");
            string installForceCmd = $"winget install --exact --id \"{app.Id}\" {srcArg} --force --accept-package-agreements --accept-source-agreements";
            await CommandRunner.RunAsync(installForceCmd, _onOutput, _ct);
        }

        _onOutput($"[OK] Traitement de {app.Name} terminé.");
    }

    private async void BtnUpdateAll_Click(object sender, RoutedEventArgs e)
    {
        var confirm = MessageBox.Show(
            "Voulez-vous lancer la mise à niveau automatique de toutes les applications disponibles ?\n\n(Commande : winget upgrade --all --include-unknown)",
            "Tout mettre à jour",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirm != MessageBoxResult.Yes) return;

        _onOutput("\n🚀 [LANCEMENT] Mise à niveau de toutes les applications (winget upgrade --all --include-unknown)...");
        TxtFeedback.Text = "Mise à jour globale en cours...";

        try
        {
            await CommandRunner.RunAsync("winget upgrade --all --include-unknown --accept-package-agreements --accept-source-agreements", _onOutput, _ct);
            _onOutput("\n✅ [TERMINÉ] Mise à jour globale terminée.");
            TxtFeedback.Text = "Mise à jour globale terminée.";
            await CheckUpdatesAsync();
        }
        catch (Exception ex)
        {
            _onOutput($"[ERR] {ex.Message}");
            TxtFeedback.Text = $"Erreur : {ex.Message}";
        }
    }


    private async void BtnLoadInstalledApps_Click(object sender, RoutedEventArgs e)
    {
        TxtFeedback.Text = "Inventaire complet des logiciels installés via winget list...";
        TxtNoInstalled.Text = "Chargement en cours...";
        TxtNoInstalled.Visibility = Visibility.Visible;
        _allInstalled.Clear();
        LstInstalledApps.Items.Clear();

        try
        {
            string output = await CommandRunner.RunAsync("winget list", _ct);
            var lines = output.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            int headerIdx = -1;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("Nom", StringComparison.OrdinalIgnoreCase) || lines[i].StartsWith("Name", StringComparison.OrdinalIgnoreCase))
                {
                    headerIdx = i;
                    break;
                }
            }

            if (headerIdx >= 0 && headerIdx + 2 < lines.Length)
            {
                string header = lines[headerIdx];
                int idPos  = header.IndexOf("ID", StringComparison.OrdinalIgnoreCase);
                int verPos = header.IndexOf("Version", StringComparison.OrdinalIgnoreCase);

                for (int i = headerIdx + 2; i < lines.Length; i++)
                {
                    string line = lines[i].TrimEnd();
                    if (string.IsNullOrWhiteSpace(line) || idPos < 0) continue;

                    string name = line.Length > idPos ? line.Substring(0, idPos).Trim() : line.Trim();
                    string id   = verPos > 0 && line.Length > verPos ? line.Substring(idPos, verPos - idPos).Trim() : "";
                    string ver  = verPos > 0 && line.Length > verPos ? line.Substring(verPos).Trim() : "";

                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        var item = new WingetInstalledItem { Name = name, Id = id, Version = ver };
                        _allInstalled.Add(item);
                        LstInstalledApps.Items.Add(item);
                    }
                }
            }

            TxtNoInstalled.Visibility = Visibility.Collapsed;
            TxtFeedback.Text = $"{_allInstalled.Count} logiciel(s) installé(s) répertorié(s).";
        }
        catch (Exception ex)
        {
            TxtNoInstalled.Text = $"Erreur inventaire : {ex.Message}";
            TxtFeedback.Text = $"Erreur : {ex.Message}";
        }
    }

    private void TxtSearchApp_TextChanged(object sender, TextChangedEventArgs e)
    {
        string filter = TxtSearchApp.Text.Trim().ToLowerInvariant();
        LstInstalledApps.Items.Clear();

        var matches = string.IsNullOrWhiteSpace(filter)
            ? _allInstalled
            : _allInstalled.Where(a => a.Name.ToLowerInvariant().Contains(filter) || a.Id.ToLowerInvariant().Contains(filter));

        foreach (var m in matches)
        {
            LstInstalledApps.Items.Add(m);
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
