using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ScriptsWindows.Models;
using ScriptsWindows.Views.Controls;

namespace ScriptsWindows.Views;

/// <summary>
/// Contrôleur de gestion des sous-sections pour une catégorie donnée.
/// Garantit qu'une seule sous-catégorie est ouverte à la fois et permet de tout replier.
/// </summary>
public class SectionController
{
    private readonly List<Action<bool>> _expandActions = new();
    private int _expandedIndex = -1;

    public void RegisterSection(Action<bool> setExpanded)
    {
        _expandActions.Add(setExpanded);
    }

    public void Toggle(int index)
    {
        if (_expandedIndex == index)
        {
            // Déjà ouvert : on replie tout
            CollapseAll();
        }
        else
        {
            // Ouvre seulement la section demandée et ferme toutes les autres
            ExpandOnly(index);
        }
    }

    public void ExpandOnly(int index)
    {
        for (int i = 0; i < _expandActions.Count; i++)
        {
            if (i == index)
            {
                _expandActions[i](true);
            }
            else
            {
                _expandActions[i](false);
            }
        }
        _expandedIndex = index;
    }

    public void CollapseAll()
    {
        for (int i = 0; i < _expandActions.Count; i++)
        {
            _expandActions[i](false);
        }
        _expandedIndex = -1;
    }
}

/// <summary>
/// Construit la vue partitionnée par sous-titres/composants pour chaque catégorie.
/// Les sous-sections sont repliées par défaut en mode accordéon exclusif (une seule sous-catégorie ouverte à la fois).
/// </summary>
public static class SectionViewBuilder
{
    public static SectionController PopulateSections(Panel container, IEnumerable<ScriptTool> tools)
    {
        container.Children.Clear();
        var controller = new SectionController();

        var toolList = tools.ToList();
        if (toolList.Count == 0) return controller;

        // Grouper les outils par SubHeading dans l'ordre d'insertion
        var groups = toolList
            .GroupBy(t => string.IsNullOrWhiteSpace(t.SubHeading) ? "Général" : t.SubHeading)
            .ToList();

        // Barre d'information et bouton "Tout replier"
        if (groups.Count > 1)
        {
            var topBar = new DockPanel
            {
                Margin = new Thickness(0, 0, 0, 12)
            };

            var countInfo = new TextBlock
            {
                Text = $"{groups.Count} sous-catégories ({toolList.Count} outils au total)",
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center
            };
            countInfo.SetResourceReference(TextBlock.ForegroundProperty, "TextMuted");
            DockPanel.SetDock(countInfo, Dock.Left);
            topBar.Children.Add(countInfo);

            var buttonGroup = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            DockPanel.SetDock(buttonGroup, Dock.Right);

            var btnCollapseAll = CreateMiniButton("▸ Tout replier", () => controller.CollapseAll());

            buttonGroup.Children.Add(btnCollapseAll);
            topBar.Children.Add(buttonGroup);

            container.Children.Add(topBar);
        }

        for (int groupIndex = 0; groupIndex < groups.Count; groupIndex++)
        {
            var group = groups[groupIndex];
            int currentIndex = groupIndex;

            var sectionBox = new StackPanel
            {
                Margin = new Thickness(0, 0, 0, 10)
            };

            // En-tête repliable interactif (carte cliquable)
            var headerBorder = new Border
            {
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(14, 10, 14, 10),
                Cursor = Cursors.Hand,
                BorderThickness = new Thickness(1)
            };
            headerBorder.SetResourceReference(Border.BackgroundProperty, "BgCard");
            headerBorder.SetResourceReference(Border.BorderBrushProperty, "Border");

            var dock = new DockPanel();

            // Côté droit : indicateur texte et badge
            var rightPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center
            };
            DockPanel.SetDock(rightPanel, Dock.Right);

            var countBadge = new Border
            {
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(8, 2, 8, 2),
                Margin = new Thickness(0, 0, 8, 0),
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

            var toggleHint = new TextBlock
            {
                Text = "Déplier",
                FontSize = 11.5,
                FontWeight = FontWeights.Medium,
                VerticalAlignment = VerticalAlignment.Center
            };
            toggleHint.SetResourceReference(TextBlock.ForegroundProperty, "TextMuted");

            rightPanel.Children.Add(countBadge);
            rightPanel.Children.Add(toggleHint);
            dock.Children.Add(rightPanel);

            // Côté gauche : chevron + barre d'accent + titre de la sous-catégorie
            var leftPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center
            };

            var chevron = new TextBlock
            {
                Text = "▶",
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            chevron.SetResourceReference(TextBlock.ForegroundProperty, "Accent");

            var accentBar = new Border
            {
                Width = 3.5,
                Height = 15,
                CornerRadius = new CornerRadius(2),
                Margin = new Thickness(0, 0, 10, 0)
            };
            accentBar.SetResourceReference(Border.BackgroundProperty, "Accent");

            var titleBlock = new TextBlock
            {
                Text = group.Key,
                FontSize = 13.5,
                FontWeight = FontWeights.SemiBold,
                VerticalAlignment = VerticalAlignment.Center
            };
            titleBlock.SetResourceReference(TextBlock.ForegroundProperty, "TextPrimary");

            leftPanel.Children.Add(chevron);
            leftPanel.Children.Add(accentBar);
            leftPanel.Children.Add(titleBlock);
            dock.Children.Add(leftPanel);

            headerBorder.Child = dock;

            // Grille des cartes (repliée par défaut)
            var wrapPanel = new WrapPanel
            {
                Margin = new Thickness(6, 10, 0, 6),
                Visibility = Visibility.Collapsed
            };

            foreach (var tool in group)
            {
                var card = new ToolCard();
                card.BindTool(tool);
                wrapPanel.Children.Add(card);
            }

            // Action de bascule de cette section
            void SetExpanded(bool expand)
            {
                wrapPanel.Visibility = expand ? Visibility.Visible : Visibility.Collapsed;
                chevron.Text = expand ? "▼" : "▶";
                toggleHint.Text = expand ? "Replier" : "Déplier";

                if (expand)
                {
                    toggleHint.SetResourceReference(TextBlock.ForegroundProperty, "Accent");
                    headerBorder.SetResourceReference(Border.BorderBrushProperty, "AccentDim");
                }
                else
                {
                    toggleHint.SetResourceReference(TextBlock.ForegroundProperty, "TextMuted");
                    headerBorder.SetResourceReference(Border.BorderBrushProperty, "Border");
                }
            }

            // Enregistrer dans le contrôleur exclusif
            controller.RegisterSection(SetExpanded);

            // Clic sur l'en-tête pour basculer en mode exclusif
            headerBorder.MouseLeftButtonDown += (s, e) =>
            {
                e.Handled = true;
                controller.Toggle(currentIndex);
            };

            // Effet hover subtil sur l'en-tête
            headerBorder.MouseEnter += (s, e) =>
            {
                headerBorder.SetResourceReference(Border.BackgroundProperty, "BgCardHover");
            };
            headerBorder.MouseLeave += (s, e) =>
            {
                headerBorder.SetResourceReference(Border.BackgroundProperty, "BgCard");
            };

            sectionBox.Children.Add(headerBorder);
            sectionBox.Children.Add(wrapPanel);

            container.Children.Add(sectionBox);
        }

        return controller;
    }

    private static Button CreateMiniButton(string text, Action onClick)
    {
        var btn = new Button
        {
            Content = text,
            Padding = new Thickness(8, 3, 8, 3),
            FontSize = 11,
            Cursor = Cursors.Hand,
            BorderThickness = new Thickness(1)
        };
        btn.SetResourceReference(Button.BackgroundProperty, "BgCard");
        btn.SetResourceReference(Button.BorderBrushProperty, "Border");
        btn.SetResourceReference(Button.ForegroundProperty, "TextSecondary");

        var style = new Style(typeof(Border));
        style.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(6)));
        btn.Resources.Add(typeof(Border), style);

        btn.Click += (s, e) => onClick();
        return btn;
    }
}
