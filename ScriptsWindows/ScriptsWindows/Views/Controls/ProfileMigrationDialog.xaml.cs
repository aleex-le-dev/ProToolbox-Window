using System.IO;
using System.Windows;
using Microsoft.Win32;
using ScriptsWindows.Core;
using ScriptsWindows.Services;

namespace ScriptsWindows.Views.Controls;

/// <summary>Sauvegarde du profil utilisateur vers un support externe et restauration sur un autre PC.</summary>
public partial class ProfileMigrationDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;
    private readonly List<MigrationItem> _items = ProfileMigrationService.GetItems();
    private CancellationTokenSource? _runCts;
    private bool _running;

    public ProfileMigrationDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput ?? (_ => { });
        _ct = ct;
        TxtSubtitle.Text = $"Profil de « {Environment.UserName} » sur {Environment.MachineName} : fichiers personnels, favoris, Wi-Fi et logiciels.";
        ListItems.ItemsSource = _items;

        Loaded += async (_, _) =>
        {
            await ProfileMigrationService.ComputeSizesAsync(_items);
            UpdateTotal();
            TxtStatus.Text = "Cochez les éléments puis sauvegardez ou restaurez.";
        };
        foreach (var i in _items) i.PropertyChanged += (_, _) => UpdateTotal();

        Closing += (_, e) =>
        {
            if (!_running) return;
            if (MessageBox.Show(this, "Une copie est en cours. L'arrêter et fermer ?", "Copie en cours", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                e.Cancel = true;
            else _runCts?.Cancel();
        };
    }

    private void UpdateTotal()
    {
        long total = _items.Where(i => i.IsSelected && i.Kind == MigrationKind.Folder).Sum(i => Math.Max(0, i.SizeBytes));
        TxtTotal.Text = total > 0 ? $"≈ {ProfileMigrationService.FormatSize(total)} de fichiers" : "";
    }

    private void SetRunning(bool running, string status)
    {
        _running = running;
        BtnBackup.IsEnabled = BtnRestore.IsEnabled = BtnClose.IsEnabled = !running;
        BtnCancel.Visibility = ProgBar.Visibility = running ? Visibility.Visible : Visibility.Collapsed;
        ListItems.IsEnabled = !running;
        TxtStatus.Text = status;
    }

    private async void BtnBackup_Click(object sender, RoutedEventArgs e)
    {
        if (!_items.Any(i => i.IsSelected && i.IsAvailable)) { TxtStatus.Text = "Cochez au moins un élément."; return; }

        var dlg = new OpenFolderDialog { Title = "Choisissez où enregistrer la sauvegarde (disque externe, clé USB…)" };
        if (dlg.ShowDialog(this) != true) return;

        // Contrôle de l'espace disponible sur la destination
        long needed = _items.Where(i => i.IsSelected && i.Kind == MigrationKind.Folder).Sum(i => Math.Max(0, i.SizeBytes));
        try
        {
            var drive = new DriveInfo(Path.GetPathRoot(dlg.FolderName)!);
            if (drive.IsReady && drive.AvailableFreeSpace < needed)
            {
                MessageBox.Show(this, $"Espace insuffisant sur {drive.Name} : {ProfileMigrationService.FormatSize(drive.AvailableFreeSpace)} libres pour ≈ {ProfileMigrationService.FormatSize(needed)} à copier.",
                                "Espace insuffisant", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }
        catch { /* chemin réseau : pas de contrôle */ }

        await RunAsync("Sauvegarde en cours…", async (log, ct) =>
        {
            string root = await ProfileMigrationService.BackupAsync(_items, dlg.FolderName, log, ct);
            Dispatcher.Invoke(() => TxtStatus.Text = $"Sauvegarde terminée : {root}");
            FolderDialogHelper.OpenInExplorer(root);
        });
    }

    private async void BtnRestore_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFolderDialog { Title = "Sélectionnez le dossier « Sauvegarde_… » à restaurer" };
        if (dlg.ShowDialog(this) != true) return;

        var manifest = ProfileMigrationService.ReadManifest(dlg.FolderName);
        if (manifest == null)
        {
            MessageBox.Show(this, $"Ce dossier n'est pas une sauvegarde Alex Toolkit (fichier {ProfileMigrationService.ManifestName} absent).",
                            "Sauvegarde introuvable", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var toRestore = _items.Where(i => i.IsSelected && manifest.Items.Contains(i.Key)).ToList();
        if (toRestore.Count == 0) { TxtStatus.Text = "Aucun élément coché n'est présent dans cette sauvegarde."; return; }

        bool reinstall = false;
        if (toRestore.Any(i => i.Kind == MigrationKind.Apps))
            reinstall = MessageBox.Show(this, "Réinstaller automatiquement les logiciels de l'ancien PC via Winget ?\n(Peut prendre longtemps ; les logiciels indisponibles sont ignorés.)",
                                        "Logiciels", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

        string summary = string.Join(", ", toRestore.Select(i => i.Label));
        if (MessageBox.Show(this, $"Sauvegarde de « {manifest.User} » ({manifest.Computer}) du {manifest.Date:dd/MM/yyyy HH:mm}.\n\nRestaurer : {summary} ?",
                            "Restaurer", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

        await RunAsync("Restauration en cours…", (log, ct) => ProfileMigrationService.RestoreAsync(toRestore, dlg.FolderName, reinstall, log, ct));
    }

    private async Task RunAsync(string status, Func<Action<string>, CancellationToken, Task> work)
    {
        _runCts = CancellationTokenSource.CreateLinkedTokenSource(_ct);
        SetRunning(true, status);
        _onOutput($"\n🚀 [LANCEMENT] {status.TrimEnd('…')}");
        try
        {
            await work(_onOutput, _runCts.Token);
            if (TxtStatus.Text == status) TxtStatus.Text = "Terminé.";
        }
        catch (OperationCanceledException) { TxtStatus.Text = "Opération arrêtée."; _onOutput("🛑 [ANNULÉ] Copie interrompue."); }
        catch (Exception ex) { TxtStatus.Text = $"Erreur : {ex.Message}"; _onOutput($"[ERR] {ex.Message}"); }
        finally { _running = false; SetRunning(false, TxtStatus.Text); }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e) => _runCts?.Cancel();
    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
