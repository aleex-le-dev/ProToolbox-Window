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
/// <summary>
/// Contrôleur de gestion des sous-sections pour une catégorie donnée.
/// Toutes les sous-catégories sont dépliées par défaut afin qu'aucun script ne soit masqué.
/// Permet de déplier/replier individuellement chaque sous-catégorie par simple clic sur son en-tête.
/// </summary>
public class SectionController
{
    private readonly List<Action<bool>> _expandActions = new();
    private readonly List<bool> _isExpanded = new();

    public void RegisterSection(Action<bool> setExpanded, bool initialExpanded = true)
    {
        _expandActions.Add(setExpanded);
        _isExpanded.Add(initialExpanded);
        setExpanded(initialExpanded);
    }

    public void Toggle(int index)
    {
        if (index >= 0 && index < _expandActions.Count)
        {
            _isExpanded[index] = !_isExpanded[index];
            _expandActions[index](_isExpanded[index]);
        }
    }

    public void ExpandOnly(int index)
    {
        for (int i = 0; i < _expandActions.Count; i++)
        {
            bool expand = (i == index);
            _isExpanded[i] = expand;
            _expandActions[i](expand);
        }
    }

    public void ExpandAll()
    {
        for (int i = 0; i < _expandActions.Count; i++)
        {
            _isExpanded[i] = true;
            _expandActions[i](true);
        }
    }

    public void CollapseAll()
    {
        // Maintient toutes les sous-catégories ouvertes par défaut pour ne masquer aucun script
        ExpandAll();
    }
}

/// <summary>
/// Construit la vue partitionnée par sous-titres/composants pour chaque catégorie.
/// Les sous-sections sont repliées par défaut en mode accordéon exclusif (une seule sous-catégorie ouverte à la fois).
/// </summary>
public static class SectionViewBuilder
{
    private static readonly Dictionary<string, string> ProperTerms = new(StringComparer.OrdinalIgnoreCase)
    {
        ["SSD"] = "SSD",
        ["HDD"] = "HDD",
        ["RAM"] = "RAM",
        ["CPU"] = "CPU",
        ["GPU"] = "GPU",
        ["BIOS"] = "BIOS",
        ["UEFI"] = "UEFI",
        ["DNS"] = "DNS",
        ["ARP"] = "ARP",
        ["IP"] = "IP",
        ["IPv4"] = "IPv4",
        ["IPv6"] = "IPv6",
        ["TCP"] = "TCP",
        ["UDP"] = "UDP",
        ["DHCP"] = "DHCP",
        ["OS"] = "OS",
        ["UWP"] = "UWP",
        ["SFC"] = "SFC",
        ["DISM"] = "DISM",
        ["CHKDSK"] = "CHKDSK",
        ["SMART"] = "SMART",
        ["S.M.A.R.T."] = "S.M.A.R.T.",
        ["TRIM"] = "TRIM",
        ["WMI"] = "WMI",
        ["CIM"] = "CIM",
        ["CMD"] = "CMD",
        ["GUI"] = "GUI",
        ["USB"] = "USB",
        ["Wi-Fi"] = "Wi-Fi",
        ["WiFi"] = "Wi-Fi",
        ["WinSxS"] = "WinSxS",
        ["Windows"] = "Windows",
        ["Winget"] = "Winget",
        ["Defender"] = "Defender",
        ["Microsoft"] = "Microsoft",
        ["Store"] = "Store",
        ["Google"] = "Google",
        ["Chrome"] = "Chrome",
        ["VLC"] = "VLC",
        ["Media"] = "Media",
        ["Player"] = "Player",
        ["PDF"] = "PDF",
        ["Sumatra"] = "Sumatra",
        ["7-Zip"] = "7-Zip",
        ["PowerShell"] = "PowerShell",
        ["Office"] = "Office",
        ["BitLocker"] = "BitLocker",
        ["Cloudflare"] = "Cloudflare",
        ["Winsock"] = "Winsock",
        ["DxDiag"] = "DxDiag",
        ["WinSAT"] = "WinSAT",
        ["Resmon"] = "Resmon",
        ["Perfmon"] = "Perfmon",
        ["Msinfo32"] = "Msinfo32",
        ["Devmgmt"] = "Devmgmt",
        ["Cleanmgr"] = "Cleanmgr",
        ["DISKPART"] = "DISKPART",
        ["BSOD"] = "BSOD",
        ["God"] = "God",
        ["Mode"] = "Mode",
        ["ScanHealth"] = "ScanHealth",
        ["RestoreHealth"] = "RestoreHealth",
        ["CheckHealth"] = "CheckHealth",
        ["QuickScan"] = "QuickScan",
        ["WinRE"] = "WinRE",
        ["BCD"] = "BCD",
        ["BCDBoot"] = "BCDBoot"
    };

    /// <summary>
    /// Formate un titre selon les règles typographiques françaises :
    /// Seul le premier mot commence par une majuscule, tous les mots suivants sont en minuscules,
    /// sauf les sigles, acronymes et noms propres technologiques préservés (SSD, RAM, CPU, GPU, BIOS, Windows...).
    /// </summary>
    public static string FormatFrenchTitle(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        var tokens = System.Text.RegularExpressions.Regex.Split(input.Trim(), @"(\s+|[()&/,›\-:])");
        var sb = new System.Text.StringBuilder();
        bool isFirstWord = true;

        foreach (var token in tokens)
        {
            if (string.IsNullOrEmpty(token)) continue;

            if (char.IsWhiteSpace(token[0]) || token == "(" || token == ")" || token == "&" || token == "/" || token == "," || token == "›" || token == "-" || token == ":")
            {
                sb.Append(token);
                continue;
            }

            if (ProperTerms.TryGetValue(token, out var proper))
            {
                sb.Append(proper);
            }
            else if (token.Length > 1 && token.All(char.IsUpper))
            {
                sb.Append(token);
            }
            else if (isFirstWord)
            {
                sb.Append(char.ToUpper(token[0]) + (token.Length > 1 ? token.Substring(1).ToLower() : ""));
            }
            else
            {
                sb.Append(token.ToLower());
            }

            isFirstWord = false;
        }

        return sb.ToString();
    }

    /// <summary>
    /// Fournit l'icône et le nom d'affichage français complet pour une catégorie donnée.
    /// </summary>
    public static (string Icon, string Name) GetCategoryMeta(string category) => (category ?? "").ToUpperInvariant() switch
    {
        "APPLICATIONS"     => ("📦", "Installer vos logiciels"),
        "DEMARRAGE"        => ("⚡", "Démarrage & Récupération"),
        "DIAGNOSTIC"       => ("🔍", "Bilan & Santé du PC"),
        "DISQUE"           => ("💽", "Disques et stockage"),
        "MATERIEL"         => ("🖥️", "Performances & Énergie"),
        "MISES_A_JOUR"     => ("🔄", "Mises à jour des programmes"),
        "NETTOYAGE"        => ("🧹", "Nettoyage & Espace disque"),
        "PERSONNALISATION" => ("🎨", "Personnalisation de Windows"),
        "REPARATION"       => ("🔧", "Dépannage & Réparation"),
        "RESEAU"           => ("🌐", "Internet & Réseau"),
        "SECRETS"          => ("🔑", "Mots de passe & Clés de licence"),
        "NOUVEAUTE"        => ("✨", "Nouveautés & Innovations"),
        "SECURITE"         => ("🛡️", "Sécurité & Antivirus"),
        _                  => ("⚙️", string.IsNullOrWhiteSpace(category) ? "Général" : FormatFrenchTitle(category))
    };

    /// <summary>
    /// Fournit le sous-titre explicatif officiel pour chaque catégorie.
    /// </summary>
    public static string GetCategorySubtitle(string category) => (category ?? "").ToUpperInvariant() switch
    {
        "APPLICATIONS"     => "Installez ou mettez à jour en 1 clic vos applications indispensables du quotidien",
        "DEMARRAGE"        => "Gérez le démarrage en mode sans échec, l'accès au BIOS/UEFI, la réparation du menu BCD et WinRE",
        "DIAGNOSTIC"       => "Vérifiez l'état de votre ordinateur, son matériel et accélérez son démarrage",
        "DISQUE"           => "Gérez vos disques durs et clés USB, boostez leur vitesse et protégez vos dossiers",
        "MATERIEL"         => "Ajustez la puissance du processeur (normal ou boost) et testez la rapidité du PC",
        "MISES_A_JOUR"     => "Recherchez et installez les dernières versions de tous vos programmes en un clic",
        "NETTOYAGE"        => "Libérez de l'espace disque en supprimant les fichiers temporaires et inutiles en toute sécurité",
        "NOUVEAUTE"        => "Découvrez les derniers scripts et outils innovants : confidentialité, débloat, optimisation gaming et audit IA",
        "PERSONNALISATION" => "Simplifiez votre usage en retrouvant le clic droit classique ou le dossier Tous les Réglages",
        "REPARATION"       => "Réparez automatiquement les bugs de Windows, les blocages d'impression et les erreurs",
        "RESEAU"           => "Résolvez les coupures de connexion, optimisez votre Wi-Fi et inspectez vos équipements",
        "SECRETS"          => "Retrouvez vos clés Wi-Fi, votre licence Windows originale et créez des mots de passe incassables",
        "SECURITE"         => "Protégez votre ordinateur des virus, surveillez les pannes récentes et les logiciels lourds",
        _                  => "Outils pratiques pour prendre soin de votre PC"
    };

    /// <summary>
    /// Affiche directement les cartes d'outils dans une grille fluide (WrapPanel) sans accordéon ni sous-titres.
    /// Utilisé lorsque les outils doivent être accessibles immédiatement sans clic intermédiaire.
    /// </summary>
    public static SectionController PopulateDirect(Panel container, IEnumerable<ScriptTool> tools)
    {
        container.Children.Clear();
        var controller = new SectionController();

        var wrapPanel = new WrapPanel
        {
            Margin = new Thickness(0, 4, 0, 0)
        };

        foreach (var tool in tools)
        {
            var card = new ToolCard();
            card.BindTool(tool);
            wrapPanel.Children.Add(card);
        }

        container.Children.Add(wrapPanel);
        return controller;
    }

    /// <summary>
    /// Construit la vue partitionnée par sous-titres/composants pour chaque page de catégorie.
    /// Les sous-sections sont triées par ordre alphabétique et repliées par défaut en mode accordéon exclusif.
    /// </summary>
    public static SectionController PopulateSections(Panel container, IEnumerable<ScriptTool> tools)
    {
        container.Children.Clear();
        var controller = new SectionController();

        var toolList = tools.ToList();
        if (toolList.Count == 0) return controller;

        // Si aucun outil n'a de sous-catégorie définie, affichage direct immédiat
        if (toolList.All(t => string.IsNullOrWhiteSpace(t.SubHeading)))
        {
            return PopulateDirect(container, toolList);
        }

        // Grouper par sous-catégorie et trier systématiquement par ordre alphabétique avec casse de phrase
        var groups = toolList
            .GroupBy(t => string.IsNullOrWhiteSpace(t.SubHeading) ? "Général" : FormatFrenchTitle(t.SubHeading))
            .Select(g => new
            {
                SubHeading = g.Key,
                Tools = g.OrderBy(t => t.Title, StringComparer.CurrentCultureIgnoreCase).ToList()
            })
            .OrderBy(g => g.SubHeading, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        // Barre d'information discrète
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
                BorderThickness = new Thickness(1),
                VerticalAlignment = VerticalAlignment.Center
            };
            countBadge.SetResourceReference(Border.BackgroundProperty, "BgInput");
            countBadge.SetResourceReference(Border.BorderBrushProperty, "Border");
            DockPanel.SetDock(countBadge, Dock.Right);

            var countText = new TextBlock
            {
                Text = $"{group.Tools.Count} outil{(group.Tools.Count > 1 ? "s" : "")}",
                FontSize = 11,
                FontWeight = FontWeights.SemiBold
            };
            countText.SetResourceReference(TextBlock.ForegroundProperty, "TextPrimary");
            countBadge.Child = countText;

            dock.Children.Add(countBadge);

            // Côté gauche : chevron + barre d'accent + titre de la sous-catégorie
            var leftPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center
            };

            var chevron = new TextBlock
            {
                Text = "▼",
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
                Text = group.SubHeading,
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
            headerBorder.SetResourceReference(Border.BorderBrushProperty, "AccentDim");

            // Grille des cartes (dépliée par défaut pour ne masquer aucun script)
            var wrapPanel = new WrapPanel
            {
                Margin = new Thickness(6, 10, 0, 6),
                Visibility = Visibility.Visible
            };

            foreach (var tool in group.Tools)
            {
                var card = new ToolCard();
                card.BindTool(tool);
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

            // Enregistrer dans le contrôleur (déplié par défaut)
            controller.RegisterSection(SetExpanded, initialExpanded: true);

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

    /// <summary>
    /// Construit la vue des favoris avec une ligne accordéon par catégorie (triée par ordre alphabétique).
    /// À l'ouverture d'une catégorie, les sous-catégories et leurs scripts favoris sont affichés.
    /// </summary>
    public static SectionController PopulateFavoritesSections(Panel container, IEnumerable<ScriptTool> tools)
    {
        container.Children.Clear();
        var controller = new SectionController();

        var toolList = tools.ToList();
        if (toolList.Count == 0) return controller;

        // Grouper par Catégorie et trier alphabétiquement par nom français
        var categoryGroups = toolList
            .GroupBy(t => t.Category ?? "AUTRE")
            .Select(cg =>
            {
                var meta = GetCategoryMeta(cg.Key);
                return new
                {
                    CategoryKey   = cg.Key,
                    CategoryIcon  = meta.Icon,
                    CategoryName  = meta.Name,
                    TotalCount    = cg.Count(),
                    SubCategories = cg
                        .GroupBy(t => string.IsNullOrWhiteSpace(t.SubHeading) ? "Général" : FormatFrenchTitle(t.SubHeading))
                        .Select(sg => new
                        {
                            SubHeading = sg.Key,
                            Tools      = sg.OrderBy(t => t.Title, StringComparer.CurrentCultureIgnoreCase).ToList()
                        })
                        .OrderBy(sg => sg.SubHeading, StringComparer.CurrentCultureIgnoreCase)
                        .ToList()
                };
            })
            .OrderBy(cg => cg.CategoryName, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        // Barre d'information discrète
        if (categoryGroups.Count > 0)
        {
            var topBar = new DockPanel
            {
                Margin = new Thickness(0, 0, 0, 14)
            };

            var countInfo = new TextBlock
            {
                Text = $"{categoryGroups.Count} catégorie{(categoryGroups.Count > 1 ? "s" : "")} avec des favoris ({toolList.Count} outil{(toolList.Count > 1 ? "s" : "")} au total)",
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center
            };
            countInfo.SetResourceReference(TextBlock.ForegroundProperty, "TextMuted");
            DockPanel.SetDock(countInfo, Dock.Left);
            topBar.Children.Add(countInfo);

            container.Children.Add(topBar);
        }

        for (int catIndex = 0; catIndex < categoryGroups.Count; catIndex++)
        {
            var catGroup = categoryGroups[catIndex];
            int currentIndex = catIndex;

            var sectionBox = new StackPanel
            {
                Margin = new Thickness(0, 0, 0, 10)
            };

            // En-tête de catégorie (carte interactive d'accordéon)
            var headerBorder = new Border
            {
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(14, 11, 14, 11),
                Cursor = Cursors.Hand,
                BorderThickness = new Thickness(1)
            };
            headerBorder.SetResourceReference(Border.BackgroundProperty, "BgCard");
            headerBorder.SetResourceReference(Border.BorderBrushProperty, "Border");

            var dock = new DockPanel();

            // Côté droit : badge du nombre d'outils favoris dans cette catégorie
            var countBadge = new Border
            {
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(9, 3, 9, 3),
                BorderThickness = new Thickness(1),
                VerticalAlignment = VerticalAlignment.Center
            };
            countBadge.SetResourceReference(Border.BackgroundProperty, "BgInput");
            countBadge.SetResourceReference(Border.BorderBrushProperty, "Border");
            DockPanel.SetDock(countBadge, Dock.Right);

            var countText = new TextBlock
            {
                Text = $"{catGroup.TotalCount} favori{(catGroup.TotalCount > 1 ? "s" : "")}",
                FontSize = 11.5,
                FontWeight = FontWeights.SemiBold
            };
            countText.SetResourceReference(TextBlock.ForegroundProperty, "TextPrimary");
            countBadge.Child = countText;
            dock.Children.Add(countBadge);

            // Côté gauche : chevron + barre d'accent + icône + nom de la catégorie
            var leftPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center
            };

            var chevron = new TextBlock
            {
                Text = "▼",
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            chevron.SetResourceReference(TextBlock.ForegroundProperty, "Accent");

            var accentBar = new Border
            {
                Width = 3.5,
                Height = 16,
                CornerRadius = new CornerRadius(2),
                Margin = new Thickness(0, 0, 10, 0)
            };
            accentBar.SetResourceReference(Border.BackgroundProperty, "Accent");

            var iconBlock = new TextBlock
            {
                Text = catGroup.CategoryIcon,
                FontSize = 15,
                Margin = new Thickness(0, 0, 8, 0),
                FontFamily = new FontFamily("Segoe UI Emoji, Segoe MDL2 Assets, Segoe UI Symbol, Segoe UI"),
                VerticalAlignment = VerticalAlignment.Center
            };
            iconBlock.SetResourceReference(TextBlock.ForegroundProperty, "TextPrimary");

            var titleBlock = new TextBlock
            {
                Text = catGroup.CategoryName,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                VerticalAlignment = VerticalAlignment.Center
            };
            titleBlock.SetResourceReference(TextBlock.ForegroundProperty, "TextPrimary");

            leftPanel.Children.Add(chevron);
            leftPanel.Children.Add(accentBar);
            leftPanel.Children.Add(iconBlock);
            leftPanel.Children.Add(titleBlock);
            dock.Children.Add(leftPanel);

            headerBorder.Child = dock;
            headerBorder.SetResourceReference(Border.BorderBrushProperty, "AccentDim");

            // Panneau déroulant : sous-catégories et outils de la catégorie (déplié par défaut)
            var contentPanel = new StackPanel
            {
                Margin = new Thickness(8, 12, 0, 8),
                Visibility = Visibility.Visible
            };

            foreach (var subCat in catGroup.SubCategories)
            {
                var subBox = new StackPanel
                {
                    Margin = new Thickness(0, 4, 0, 14)
                };

                // En-tête élégant de la sous-catégorie
                var subHeader = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(4, 0, 0, 8),
                    VerticalAlignment = VerticalAlignment.Center
                };

                var subBullet = new Border
                {
                    Width = 6,
                    Height = 6,
                    CornerRadius = new CornerRadius(3),
                    Margin = new Thickness(0, 0, 8, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };
                subBullet.SetResourceReference(Border.BackgroundProperty, "Accent");

                var subTitle = new TextBlock
                {
                    Text = subCat.SubHeading,
                    FontSize = 13,
                    FontWeight = FontWeights.SemiBold,
                    VerticalAlignment = VerticalAlignment.Center
                };
                subTitle.SetResourceReference(TextBlock.ForegroundProperty, "TextSecondary");

                var subBadge = new Border
                {
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(6, 1, 6, 1),
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(10, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };
                subBadge.SetResourceReference(Border.BackgroundProperty, "BgInput");
                subBadge.SetResourceReference(Border.BorderBrushProperty, "Border");

                var subBadgeText = new TextBlock
                {
                    Text = $"{subCat.Tools.Count} outil{(subCat.Tools.Count > 1 ? "s" : "")}",
                    FontSize = 10.5,
                    FontWeight = FontWeights.SemiBold
                };
                subBadgeText.SetResourceReference(TextBlock.ForegroundProperty, "TextPrimary");
                subBadge.Child = subBadgeText;

                subHeader.Children.Add(subBullet);
                subHeader.Children.Add(subTitle);
                subHeader.Children.Add(subBadge);
                subBox.Children.Add(subHeader);

                // Cartes d'outils
                var wrapPanel = new WrapPanel
                {
                    Margin = new Thickness(0, 2, 0, 0)
                };

                foreach (var tool in subCat.Tools)
                {
                    var card = new ToolCard();
                    card.BindTool(tool);
                    wrapPanel.Children.Add(card);
                }

                subBox.Children.Add(wrapPanel);
                contentPanel.Children.Add(subBox);
            }

            // Action d'affichage de cette catégorie
            void SetExpanded(bool expand)
            {
                contentPanel.Visibility = expand ? Visibility.Visible : Visibility.Collapsed;
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

            // Enregistrer dans le contrôleur (déplié par défaut)
            controller.RegisterSection(SetExpanded, initialExpanded: true);

            // Effet hover visuel sur l'en-tête
            headerBorder.MouseEnter += (s, e) =>
            {
                headerBorder.SetResourceReference(Border.BackgroundProperty, "BgCardHover");
            };

            headerBorder.MouseLeave += (s, e) =>
            {
                headerBorder.SetResourceReference(Border.BackgroundProperty, "BgCard");
            };

            // Clic sur l'en-tête pour basculer (mode accordéon exclusif : ouvre la catégorie et ferme les autres)
            headerBorder.MouseLeftButtonDown += (s, e) =>
            {
                e.Handled = true;
                controller.Toggle(currentIndex);
            };

            sectionBox.Children.Add(headerBorder);
            sectionBox.Children.Add(contentPanel);

            container.Children.Add(sectionBox);
        }

        return controller;
    }
}
