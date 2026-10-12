using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ScriptsWindows.Models;
using ScriptsWindows.Views.Controls;

namespace ScriptsWindows.Views;

/// <summary>
/// Construit la vue partitionnée par sous-titres/composants pour chaque catégorie.
/// Les cartes de scripts sont segmentées sous des en-têtes visuels avec séparateurs.
/// </summary>
public static class SectionViewBuilder
{
    public static void PopulateSections(Panel container, IEnumerable<ScriptTool> tools)
    {
        container.Children.Clear();

        // Grouper les outils par SubHeading dans l'ordre d'insertion
        var groups = tools
            .GroupBy(t => string.IsNullOrWhiteSpace(t.SubHeading) ? "Général" : t.SubHeading);

        bool firstGroup = true;

        foreach (var group in groups)
        {
            var sectionBox = new StackPanel
            {
                Margin = new Thickness(0, firstGroup ? 4 : 22, 0, 10)
            };
            firstGroup = false;

            // En-tête de la sous-section
            var headerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(2, 0, 0, 6)
            };

            // Barre d'accent visuelle
            var accentBar = new Border
            {
                Width = 4,
                Height = 16,
                CornerRadius = new CornerRadius(2),
                Margin = new Thickness(0, 0, 10, 0)
            };
            accentBar.SetResourceReference(Border.BackgroundProperty, "Accent");

            // Titre de sous-section (subheading)
            var titleBlock = new TextBlock
            {
                Text = group.Key,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                VerticalAlignment = VerticalAlignment.Center
            };
            titleBlock.SetResourceReference(TextBlock.ForegroundProperty, "TextPrimary");

            // Badge discret du compte d'outils
            var countBadge = new Border
            {
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(7, 2, 7, 2),
                Margin = new Thickness(10, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            countBadge.SetResourceReference(Border.BackgroundProperty, "BgInput");

            var countText = new TextBlock
            {
                Text = $"{group.Count()} outil{(group.Count() > 1 ? "s" : "")}",
                FontSize = 11,
                FontWeight = FontWeights.Medium
            };
            countText.SetResourceReference(TextBlock.ForegroundProperty, "TextMuted");
            countBadge.Child = countText;

            headerPanel.Children.Add(accentBar);
            headerPanel.Children.Add(titleBlock);
            headerPanel.Children.Add(countBadge);

            // Séparateur fin
            var separator = new Border
            {
                Height = 1,
                Margin = new Thickness(0, 4, 0, 12),
                Opacity = 0.4
            };
            separator.SetResourceReference(Border.BackgroundProperty, "Border");

            // Grille des cartes de cette section
            var wrapPanel = new WrapPanel
            {
                Margin = new Thickness(0, 0, 0, 4)
            };

            foreach (var tool in group)
            {
                var card = new ToolCard();
                card.BindTool(tool);
                wrapPanel.Children.Add(card);
            }

            sectionBox.Children.Add(headerPanel);
            sectionBox.Children.Add(separator);
            sectionBox.Children.Add(wrapPanel);

            container.Children.Add(sectionBox);
        }
    }
}
