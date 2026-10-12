using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ScriptsWindows.Core;
using ScriptsWindows.Views.Controls;

namespace ScriptsWindows.Views.Pages;

public partial class FavorisPage : Page
{
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
        // Rafraîchir dynamiquement quand la page est active
        Dispatcher.InvokeAsync(RefreshFavoris);
    }

    public void RefreshFavoris()
    {
        FavorisGrid.Items.Clear();
        var favTools = ToolRegistry.GetFavorites().ToList();

        if (favTools.Count == 0)
        {
            HeaderPanel.Visibility = Visibility.Collapsed;
            EmptyState.Visibility  = Visibility.Visible;
            FavorisGrid.Visibility = Visibility.Collapsed;
        }
        else
        {
            HeaderPanel.Visibility = Visibility.Visible;
            EmptyState.Visibility  = Visibility.Collapsed;
            FavorisGrid.Visibility = Visibility.Visible;

            TxtFavCount.Text = $"{favTools.Count} outil{(favTools.Count > 1 ? "s" : "")} favori{(favTools.Count > 1 ? "s" : "")} enregistré{(favTools.Count > 1 ? "s" : "")}";

            foreach (var tool in favTools)
            {
                var card = new ToolCard();
                card.BindTool(tool);
                FavorisGrid.Items.Add(card);
            }
        }
    }
}
