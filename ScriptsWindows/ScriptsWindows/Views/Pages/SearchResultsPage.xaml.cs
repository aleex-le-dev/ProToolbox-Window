using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ScriptsWindows.Core;
using ScriptsWindows.Views.Controls;

namespace ScriptsWindows.Views.Pages;

public partial class SearchResultsPage : Page
{
    public SearchResultsPage(string query)
    {
        InitializeComponent();
        UpdateSearch(query);
    }

    public void UpdateSearch(string query)
    {
        TxtHeader.Text = $"🔍 Recherche : \"{query}\"";
        ResultsPanel.Children.Clear();

        var matches = ToolRegistry.Search(query).ToList();
        TxtCount.Text = $"{matches.Count} outil(s) trouvé(s)";

        if (matches.Count == 0)
        {
            EmptyPanel.Visibility = Visibility.Visible;
            ResultsPanel.Visibility = Visibility.Collapsed;
        }
        else
        {
            EmptyPanel.Visibility = Visibility.Collapsed;
            ResultsPanel.Visibility = Visibility.Visible;

            foreach (var tool in matches)
            {
                var card = new ToolCard();
                card.BindTool(tool);
                ResultsPanel.Children.Add(card);
            }
        }
    }
}
