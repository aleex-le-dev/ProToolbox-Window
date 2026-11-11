using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ScriptsWindows.Core;
using ScriptsWindows.Views.Controls;

namespace ScriptsWindows.Views.Pages;

public partial class FavorisPage : Page, ISectionPage
{
    private SectionController? _controller;
    private bool _isDirty = true; // la liste doit être (re)construite au prochain affichage

    public FavorisPage()
    {
        InitializeComponent();
        Loaded += FavorisPage_Loaded;
        FavoritesManager.FavoriteChanged += OnFavoriteChanged;
    }

    private void FavorisPage_Loaded(object sender, RoutedEventArgs e)
    {
        // Reconstruit uniquement si les favoris ont changé depuis le dernier affichage
        if (_isDirty) RefreshFavoris();
    }

    private void OnFavoriteChanged(string toolId, bool isFav)
    {
        _isDirty = true;

        // Page visible → rafraîchissement immédiat ; sinon il sera fait au prochain Loaded
        Dispatcher.InvokeAsync(() =>
        {
            if (IsLoaded && _isDirty) RefreshFavoris();
        });
    }

    public void RefreshFavoris()
    {
        _isDirty = false;
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

    public void ResetSections() => _controller?.ExpandAll();
}
