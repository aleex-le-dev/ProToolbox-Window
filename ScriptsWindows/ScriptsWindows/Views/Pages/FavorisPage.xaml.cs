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
    }

    private void FavorisPage_Loaded(object sender, RoutedEventArgs e)
    {
        RefreshFavoris();
    }

    public void RefreshFavoris()
    {
        FavorisGrid.Items.Clear();
        var favTools = ToolRegistry.GetFavorites().ToList();

        if (favTools.Count == 0)
        {
            EmptyState.Visibility = Visibility.Visible;
            FavorisGrid.Visibility = Visibility.Collapsed;
        }
        else
        {
            EmptyState.Visibility = Visibility.Collapsed;
            FavorisGrid.Visibility = Visibility.Visible;

            foreach (var tool in favTools)
            {
                var card = new ToolCard();
                card.BindTool(tool);
                FavorisGrid.Items.Add(card);
            }
        }
    }
}
