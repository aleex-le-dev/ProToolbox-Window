using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ScriptsWindows.Core;
using ScriptsWindows.Models;

namespace ScriptsWindows.Views.Controls;

public partial class ToolCard : UserControl
{

    // ── Champs ───────────────────────────────────────────────────────
    private ScriptTool? _tool;
    private Func<ScriptTool, Action<string>, Task>? _executeAction;
    private Action<string>? _outputCallback;
    private bool _isExecuting; // cette carte est celle qui exécute le script en cours

    public ToolCard()
    {
        InitializeComponent();

        // Verrouillage global : abonnement uniquement quand la carte est affichée
        // (évite qu'un événement statique garde en vie les cartes des pages reconstruites)
        Loaded   += OnCardLoaded;
        Unloaded += OnCardUnloaded;
        ToolTipService.SetShowOnDisabled(BtnRun, true); // explique pourquoi le bouton est grisé
    }

    private void OnCardLoaded(object sender, RoutedEventArgs e)
    {
        ScriptExecutionCoordinator.StateChanged -= OnExecutionStateChanged; // Loaded peut se répéter
        ScriptExecutionCoordinator.StateChanged += OnExecutionStateChanged;
        OnExecutionStateChanged(ScriptExecutionCoordinator.IsRunning);
    }

    private void OnCardUnloaded(object sender, RoutedEventArgs e)
        => ScriptExecutionCoordinator.StateChanged -= OnExecutionStateChanged;

    /// <summary>Désactive le bouton « Exécuter » des autres cartes pendant qu'un script tourne.</summary>
    private void OnExecutionStateChanged(bool isRunning)
    {
        if (_isExecuting) return; // la carte active gère elle-même son bouton
        BtnRun.IsEnabled = !isRunning;
        BtnRun.ToolTip   = isRunning
            ? $"Un script est en cours : {ScriptExecutionCoordinator.ActiveToolTitle}"
            : null;
    }

    public void BindTool(
        ScriptTool tool,
        Func<ScriptTool, Action<string>, Task>? executeAction = null,
        Action<string>? outputCallback = null)
    {
        _tool           = tool;
        _executeAction  = executeAction;
        _outputCallback = outputCallback;

        TxtIcon.Text        = IconGlyphs.ForCategory(tool.Category);
        TxtTitle.Text       = tool.Title;
        TxtDescription.Text = tool.Description;

        UpdateFavoriteState();
        PopulateFeatures(tool);

        BtnDesktop.Visibility = tool.DesktopAction != null ? Visibility.Visible : Visibility.Collapsed;
    }

    private void PopulateFeatures(ScriptTool tool)
    {
        PanelFeatures.Children.Clear();

        var features = tool.Features;
        if (features != null && features.Length > 0)
        {
            PanelFeatures.Visibility = Visibility.Visible;
            TxtDescription.MaxHeight = 34; // 2 lignes

            foreach (var feat in features.Take(3))
            {
                var itemDock = new DockPanel { Margin = new Thickness(0, 1, 0, 1) };

                var bullet = new TextBlock
                {
                    Text = "\uE73E", // CheckMark
                    FontFamily = (FontFamily)FindResource("FontIcon"),
                    FontSize = 9,
                    Margin = new Thickness(0, 0, 8, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };
                bullet.SetResourceReference(TextBlock.ForegroundProperty, "SuccessText");
                DockPanel.SetDock(bullet, Dock.Left);
                itemDock.Children.Add(bullet);

                var txt = new TextBlock
                {
                    Text = feat,
                    FontSize = 11.5,
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    VerticalAlignment = VerticalAlignment.Center
                };
                txt.SetResourceReference(TextBlock.ForegroundProperty, "TextSecondary");
                itemDock.Children.Add(txt);

                PanelFeatures.Children.Add(itemDock);
            }
        }
        else
        {
            PanelFeatures.Visibility = Visibility.Collapsed;
            TxtDescription.MaxHeight = 70; // Permet à la description de s'étaler si pas de liste
        }
    }

    /// <summary>
    /// Affiche ou masque le badge de la catégorie dans le pied de carte (utile dans la vue Favoris).
    /// </summary>
    public void ShowCategoryBadge(bool show)
    {
        if (!show || _tool == null)
        {
            BadgeCategory.Visibility = Visibility.Collapsed;
            return;
        }

        var (_, name) = SectionViewBuilder.GetCategoryMeta(_tool.Category);
        TxtCategory.Text = name;
        BadgeCategory.Visibility = Visibility.Visible;
    }

    private void UpdateFavoriteState()
    {
        if (_tool == null) return;
        bool isFav = FavoritesManager.IsFavorite(_tool.Id);

        if (isFav)
        {
            TxtFavIcon.Text = IconGlyphs.FavoriteFilled;
            TxtFavIcon.SetResourceReference(TextBlock.ForegroundProperty, "WarningText");
            BtnFavorite.ToolTip = "Retirer des favoris";
        }
        else
        {
            TxtFavIcon.Text = IconGlyphs.Favorite;
            TxtFavIcon.SetResourceReference(TextBlock.ForegroundProperty, "TextMuted");
            BtnFavorite.ToolTip = "Ajouter aux favoris";
        }
    }

    private void BtnFavorite_Click(object sender, RoutedEventArgs e)
    {
        if (_tool == null) return;
        FavoritesManager.ToggleFavorite(_tool.Id);
        UpdateFavoriteState();
    }

    private async void BtnRun_Click(object sender, RoutedEventArgs e)
    {
        if (_tool == null) return;

        // ── Exécution exclusive via le coordinateur global ──────────
        if (!ScriptExecutionCoordinator.TryBeginExecution(_tool, out var ct))
        {
            MessageBox.Show(
                $"Un script est déjà en cours : {ScriptExecutionCoordinator.ActiveToolTitle}\n\nAttendez qu'il se termine avant d'en lancer un autre.",
                "Script en cours",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            return;
        }

        _isExecuting = true;

        var mainWin = Application.Current.MainWindow as MainWindow;
        Action<string> log = _outputCallback ?? (msg => mainWin?.AppendOutput(msg));

        BtnRun.IsEnabled = false;
        BtnRun.Content   = "En cours…";

        try
        {
            log($"\n🚀 [LANCEMENT] {_tool.Title} ({DateTime.Now:HH:mm:ss})");

            if (_executeAction != null)
                await _executeAction(_tool, log);
            else if (_tool.Action != null)
                await _tool.Action(log, ct);
            else
                log($"⚠️ [INFO] Aucun script associé à {_tool.Title}.");

            log($"✅ [TERMINÉ] {_tool.Title}");
        }
        catch (OperationCanceledException)
        {
            log($"🛑 [ANNULÉ] {_tool.Title}");
        }
        catch (Exception ex)
        {
            log($"❌ [ERREUR] {ex.Message}");
        }
        finally
        {
            _isExecuting = false;
            ScriptExecutionCoordinator.EndExecution(_tool.Id);
            BtnRun.IsEnabled = true;
            BtnRun.Content   = "Exécuter";
        }
    }

    private void BtnDesktop_Click(object sender, RoutedEventArgs e)
    {
        if (_tool?.DesktopAction == null) return;

        try
        {
            var mainWin = Application.Current.MainWindow as MainWindow;
            Action<string> log = _outputCallback ?? (msg => mainWin?.AppendOutput(msg));

            _tool.DesktopAction(log);

            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var scriptPath = System.IO.Path.Combine(desktop, "Nettoyage_Systeme_Et_Disque.bat");

            MessageBox.Show(
                $"Le script de nettoyage automatique a été créé avec succès sur votre Bureau :\n\n" +
                $"📁 {scriptPath}\n\n" +
                "Il vous suffit désormais de double-cliquer dessus à tout moment pour lancer le nettoyage complet en PowerShell avec affichage détaillé des gains d'espace !",
                "Script ajouté au Bureau — Alex Toolkit",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Impossible de créer le script sur le Bureau : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
