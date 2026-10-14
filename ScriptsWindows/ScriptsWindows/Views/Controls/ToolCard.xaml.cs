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
    // ── Brushes précalculés pour l'étoile des favoris ──────────
    private static readonly SolidColorBrush BrushFavGold  = new(Color.FromRgb(251, 191, 36)); // Or éclatant #FBBF24
    private static readonly SolidColorBrush BrushFavMuted = new(Color.FromRgb(148, 163, 184)); // Gris ardoise épuré

    static ToolCard()
    {
        BrushFavGold.Freeze();
        BrushFavMuted.Freeze();
    }

    // ── Champs ───────────────────────────────────────────────────────
    private ScriptTool? _tool;
    private Func<ScriptTool, Action<string>, Task>? _executeAction;
    private Action<string>? _outputCallback;

    public ToolCard()
    {
        InitializeComponent();
    }

    public void BindTool(
        ScriptTool tool,
        Func<ScriptTool, Action<string>, Task>? executeAction = null,
        Action<string>? outputCallback = null)
    {
        _tool           = tool;
        _executeAction  = executeAction;
        _outputCallback = outputCallback;

        TxtIcon.Text        = string.IsNullOrEmpty(tool.Icon) ? "⚡" : tool.Icon;
        TxtTitle.Text       = tool.Title;
        TxtDescription.Text = tool.Description;

        UpdateFavoriteState();
        PopulateFeatures(tool);
    }

    private void PopulateFeatures(ScriptTool tool)
    {
        PanelFeatures.Children.Clear();

        var features = tool.Features;
        if (features != null && features.Length > 0)
        {
            PanelFeatures.Visibility = Visibility.Visible;
            TxtDescription.MaxHeight = 34;

            foreach (var feat in features.Take(3))
            {
                var itemDock = new DockPanel { Margin = new Thickness(0, 1.5, 0, 1.5) };

                var bullet = new TextBlock
                {
                    Text = "•",
                    FontWeight = FontWeights.Bold,
                    FontSize = 11,
                    Margin = new Thickness(2, 0, 6, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };
                bullet.SetResourceReference(TextBlock.ForegroundProperty, "Accent");
                DockPanel.SetDock(bullet, Dock.Left);
                itemDock.Children.Add(bullet);

                var txt = new TextBlock
                {
                    Text = feat,
                    FontSize = 11,
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    VerticalAlignment = VerticalAlignment.Center
                };
                txt.SetResourceReference(TextBlock.ForegroundProperty, "TextPrimary");
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

        var (icon, name) = SectionViewBuilder.GetCategoryMeta(_tool.Category);
        TxtCategory.Text = $"{icon} {name}";
        BadgeCategory.Visibility = Visibility.Visible;
    }

    private void UpdateFavoriteState()
    {
        if (_tool == null) return;
        bool isFav = FavoritesManager.IsFavorite(_tool.Id);

        if (isFav)
        {
            // Étoile pleine officielle Windows (Segoe MDL2 Assets \uE735 = FavoriteStar)
            TxtFavIcon.Text       = "\uE735";
            TxtFavIcon.Foreground = BrushFavGold;
            BtnFavorite.ToolTip   = "⭐ Retirer des favoris";
        }
        else
        {
            // Étoile contour officielle Windows (Segoe MDL2 Assets \uE734 = FavoriteStarOutline)
            TxtFavIcon.Text       = "\uE734";
            TxtFavIcon.Foreground = BrushFavMuted;
            BtnFavorite.ToolTip   = "☆ Ajouter aux favoris";
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

        var mainWin = Application.Current.MainWindow as MainWindow;
        Action<string> log = _outputCallback ?? (msg => mainWin?.AppendOutput(msg));

        BtnRun.IsEnabled = false;
        BtnRun.Content   = "⏳ ...";
        mainWin?.SetRunning(true);

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
            ScriptExecutionCoordinator.EndExecution(_tool.Id);
            mainWin?.SetRunning(false);
            BtnRun.IsEnabled = true;
            BtnRun.Content   = "▶ Exécuter";
        }
    }
}
