using System;
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
    // ── Brushes précalculés (zéro allocation à l'exécution) ──────────
    private static readonly SolidColorBrush BrushFavOn  = new(Color.FromRgb(250, 204, 21));
    private static readonly SolidColorBrush BrushFavOff = new(Color.FromRgb(226, 232, 240));

    static ToolCard()
    {
        BrushFavOn.Freeze();
        BrushFavOff.Freeze();
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
    }

    private void UpdateFavoriteState()
    {
        if (_tool == null) return;
        bool isFav = FavoritesManager.IsFavorite(_tool.Id);
        TxtFavIcon.Text       = isFav ? "⭐" : "☆";
        TxtFavIcon.Foreground = isFav ? BrushFavOn : BrushFavOff;
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
