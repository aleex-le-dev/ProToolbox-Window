using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ScriptsWindows.Core;
using ScriptsWindows.Views.Controls;

namespace ScriptsWindows.Views.Pages;

public partial class FavorisPage : Page, ISectionPage
{
    private SectionController? _controller;

    public FavorisPage()
    {
        InitializeComponent();
        Loaded += FavorisPage_Loaded;
        FavoritesManager.FavoriteChanged += OnFavoriteChanged;
    }

    private void FavorisPage_Loaded(object sender, RoutedEventArgs e)
    {
        RefreshFavoris();
    }

    private void OnFavoriteChanged(string toolId, bool isFav)
    {
        // Rafraîchir dynamiquement quand un favori est ajouté ou retiré
        Dispatcher.InvokeAsync(RefreshFavoris);
    }

    public void RefreshFavoris()
    {
        ToolsPanel.Children.Clear();
        var favTools = ToolRegistry.GetFavorites().ToList();

        if (favTools.Count == 0)
        {
            EmptyState.Visibility = Visibility.Visible;
            ToolsPanel.Visibility = Visibility.Collapsed;
            _controller = null;
        }
        else
        {
            EmptyState.Visibility = Visibility.Collapsed;
            ToolsPanel.Visibility = Visibility.Visible;
            _controller = SectionViewBuilder.PopulateFavoritesSections(ToolsPanel, favTools);
        }
    }

    public void ResetSections() => _controller?.CollapseAll();
}
