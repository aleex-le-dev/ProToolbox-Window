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
    /// <summary>
    /// Fournit l'icône et le nom d'affichage français pour une catégorie donnée.
    /// </summary>
    public static (string Icon, string Name) GetCategoryMeta(string category) => (category ?? "").ToUpperInvariant() switch
    {
        "DIAGNOSTIC"       => ("🔍", "Diagnostic"),
        "MATERIEL"         => ("🖥️", "Matériel"),
        "REPARATION"       => ("🔧", "Réparation"),
        "NETTOYAGE"        => ("🧹", "Nettoyage"),
        "SECRETS"          => ("🔑", "Secrets & Clés"),
        "RESEAU"           => ("🌐", "Réseau"),
        "DISQUE"           => ("💽", "Disque"),
        "APPLICATIONS"     => ("📦", "Applications"),
        "MISES_A_JOUR"     => ("🔄", "Mises à Jour"),
        "SECURITE"         => ("🛡️", "Sécurité"),
        "PERSONNALISATION" => ("🎨", "Personnalisation"),
        _                  => ("⚙️", string.IsNullOrWhiteSpace(category) ? "Général" : category)
    };

    public static SectionController PopulateSections(Panel container, IEnumerable<ScriptTool> tools, bool showCategory = false)
    {
        container.Children.Clear();
        var controller = new SectionController();

        var toolList = tools.ToList();
        if (toolList.Count == 0) return controller;

        // Grouper les outils : si showCategory est activé, grouper par Catégorie + Sous-titre
        var groups = showCategory
            ? toolList
                .GroupBy(t => new { Category = t.Category ?? "AUTRE", SubHeading = string.IsNullOrWhiteSpace(t.SubHeading) ? "Général" : t.SubHeading })
                .Select(g => new
                {
                    Category = g.Key.Category,
                    SubHeading = g.Key.SubHeading,
                    Tools = g.ToList()
                })
                .ToList()
            : toolList
                .GroupBy(t => string.IsNullOrWhiteSpace(t.SubHeading) ? "Général" : t.SubHeading)
                .Select(g => new
                {
                    Category = g.First().Category ?? "",
                    SubHeading = g.Key,
                    Tools = g.ToList()
                })
                .ToList();

        // Barre d'information discrète
        if (groups.Count > 1 || showCategory)
        {
            var topBar = new DockPanel
            {
                Margin = new Thickness(0, 0, 0, 12)
            };

            var catCount = toolList.Select(t => t.Category).Distinct().Count();
            var infoText = showCategory
                ? $"{catCount} catégorie{(catCount > 1 ? "s" : "")} • {groups.Count} sous-catégorie{(groups.Count > 1 ? "s" : "")} ({toolList.Count} outil{(toolList.Count > 1 ? "s" : "")} favori{(toolList.Count > 1 ? "s" : "")})"
                : $"{groups.Count} sous-catégories ({toolList.Count} outils au total)";

            var countInfo = new TextBlock
            {
                Text = infoText,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center
            };
            countInfo.SetResourceReference(TextBlock.ForegroundProperty, "TextMuted");
            DockPanel.SetDock(countInfo, Dock.Left);
            topBar.Children.Add(countInfo);

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

            // En-tête de sous-catégorie (carte interactive)
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

            // Côté droit : badge du nombre d'outils
            var countBadge = new Border
            {
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(8, 2, 8, 2),
                VerticalAlignment = VerticalAlignment.Center
            };
            countBadge.SetResourceReference(Border.BackgroundProperty, "BgInput");
            DockPanel.SetDock(countBadge, Dock.Right);

            var countText = new TextBlock
            {
                Text = $"{group.Tools.Count} outil{(group.Tools.Count > 1 ? "s" : "")}",
                FontSize = 11,
                FontWeight = FontWeights.Medium
            };
            countText.SetResourceReference(TextBlock.ForegroundProperty, "TextMuted");
            countBadge.Child = countText;

            dock.Children.Add(countBadge);

            // Côté gauche : chevron + barre d'accent + badge catégorie (si actif) + titre de la sous-catégorie
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

            leftPanel.Children.Add(chevron);
            leftPanel.Children.Add(accentBar);

            if (showCategory)
            {
                var (catIcon, catName) = GetCategoryMeta(group.Category);

                var catBadge = new Border
                {
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(7, 2, 7, 2),
                    Margin = new Thickness(0, 0, 8, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };
                catBadge.SetResourceReference(Border.BackgroundProperty, "AccentDim");

                var catText = new TextBlock
                {
                    Text = $"{catIcon} {catName}",
                    FontSize = 11.5,
                    FontWeight = FontWeights.SemiBold
                };
                catText.SetResourceReference(TextBlock.ForegroundProperty, "Accent");
                catBadge.Child = catText;
                leftPanel.Children.Add(catBadge);

                var sep = new TextBlock
                {
                    Text = "›",
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 8, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };
                sep.SetResourceReference(TextBlock.ForegroundProperty, "TextMuted");
                leftPanel.Children.Add(sep);
            }

            var titleBlock = new TextBlock
            {
                Text = group.SubHeading,
                FontSize = 13.5,
                FontWeight = FontWeights.SemiBold,
                VerticalAlignment = VerticalAlignment.Center
            };
            titleBlock.SetResourceReference(TextBlock.ForegroundProperty, "TextPrimary");

            leftPanel.Children.Add(titleBlock);
            dock.Children.Add(leftPanel);

            headerBorder.Child = dock;

            // Grille des cartes (repliée par défaut)
            var wrapPanel = new WrapPanel
            {
                Margin = new Thickness(6, 10, 0, 6),
                Visibility = Visibility.Collapsed
            };

            foreach (var tool in group.Tools)
            {
                var card = new ToolCard();
                card.BindTool(tool);
                if (showCategory)
                {
                    card.ShowCategoryBadge(true);
                }
                wrapPanel.Children.Add(card);
            }

            // Action d'affichage de cette section
            void SetExpanded(bool expand)
            {
                wrapPanel.Visibility = expand ? Visibility.Visible : Visibility.Collapsed;
                chevron.Text = expand ? "▼" : "▶";

                if (expand)
                {
                    headerBorder.SetResourceReference(Border.BorderBrushProperty, "AccentDim");
                }
                else
                {
                    headerBorder.SetResourceReference(Border.BorderBrushProperty, "Border");
                }
            }

            // Enregistrer dans le contrôleur exclusif
            controller.RegisterSection(SetExpanded);

            // Effet hover visuel sur l'en-tête
            headerBorder.MouseEnter += (s, e) =>
            {
                headerBorder.SetResourceReference(Border.BackgroundProperty, "BgCardHover");
            };

            headerBorder.MouseLeave += (s, e) =>
            {
                headerBorder.SetResourceReference(Border.BackgroundProperty, "BgCard");
            };

            // Clic sur l'en-tête pour basculer (mode accordéon exclusif : ouvre la section et ferme les autres)
            headerBorder.MouseLeftButtonDown += (s, e) =>
            {
                e.Handled = true;
                controller.Toggle(currentIndex);
            };

            sectionBox.Children.Add(headerBorder);
            sectionBox.Children.Add(wrapPanel);

            container.Children.Add(sectionBox);
        }

        return controller;
    }
}
