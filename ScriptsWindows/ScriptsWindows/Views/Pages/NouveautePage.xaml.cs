using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Pages;

public partial class NouveautePage : Page, ISectionPage
{
    private SectionController? _controller;

    public NouveautePage()
    {
        InitializeComponent();
        LoadContent();
    }

    private void LoadContent()
    {
        var tools = ToolRegistry.GetByCategory("NOUVEAUTE").ToList();
        if (tools.Count == 0)
        {
            ToolsPanel.Children.Clear();

            var emptyBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0x1F, 0x29, 0x37)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0x37, 0x41, 0x51)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(32),
                Margin = new Thickness(0, 20, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            var stack = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var iconTb = new TextBlock
            {
                Text = "✨",
                FontSize = 42,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 12)
            };

            var titleTb = new TextBlock
            {
                Text = "Catégorie Nouveautés prête",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(0xF9, 0xFA, 0xFB)),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 8)
            };

            var descTb = new TextBlock
            {
                Text = "Les 4 premiers scripts ont été intégrés dans leurs catégories permanentes (Sécurité, Matériel, Diagnostic et Réseau).\nLes futurs scripts et outils innovants apparaîtront automatiquement ici dès leur ajout !",
                FontSize = 13,
                Foreground = new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF)),
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 550,
                LineHeight = 20
            };

            stack.Children.Add(iconTb);
            stack.Children.Add(titleTb);
            stack.Children.Add(descTb);
            emptyBorder.Child = stack;

            ToolsPanel.Children.Add(emptyBorder);
        }
        else
        {
            _controller = SectionViewBuilder.PopulateSections(ToolsPanel, tools);
        }
    }

    public void ResetSections() => _controller?.ExpandAll();
}
