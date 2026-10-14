using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using ScriptsWindows.Core;
using ScriptsWindows.Views;
using ScriptsWindows.Views.Pages;

namespace ScriptsWindows;

public partial class MainWindow : Window
{
    private Button? _activeNavBtn;

    // ── Brushes statiques (Freeze = zéro GC pressure sur chaque ligne de log) ──
    private static readonly SolidColorBrush BrushError   = Freeze(Color.FromRgb(239, 68,  68));
    private static readonly SolidColorBrush BrushSuccess = Freeze(Color.FromRgb(16,  185, 129));
    private static readonly SolidColorBrush BrushWarning = Freeze(Color.FromRgb(245, 158, 11));
    private static readonly SolidColorBrush BrushInfo    = Freeze(Color.FromRgb(129, 140, 248));
    private static readonly SolidColorBrush BrushDefault = Freeze(Color.FromRgb(209, 213, 219));

    private static SolidColorBrush Freeze(Color c) { var b = new SolidColorBrush(c); b.Freeze(); return b; }

    // ── Debounce pour la recherche (évite de chercher à chaque frappe) ──
    private DispatcherTimer? _searchDebounce;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
        OutputBox.PreviewMouseWheel += OutputBox_PreviewMouseWheel;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        ThemeManager.ApplyTheme(ThemeManager.CurrentTheme);

        // Initialisation des compteurs de scripts pour chaque catégorie et les favoris
        UpdateCategoryCounts();
        FavoritesManager.FavoriteChanged += (_, _) =>
        {
            Dispatcher.InvokeAsync(UpdateFavoritesCount);
        };

        // Démarrer sur la page Favoris
        NavigateTo(NavFavoris, GetOrCreatePage("FAVORIS"), "Mes favoris", "Vos outils préférés accessibles rapidement");

        // Préchauffage asynchrone de toutes les pages (0 ms de délai au clic)
        Dispatcher.InvokeAsync(WarmupPages, DispatcherPriority.Background);
    }

    private void UpdateCategoryCounts()
    {
        UpdateFavoritesCount();

        Badge_APPLICATIONS.Text     = ToolRegistry.GetByCategory("APPLICATIONS").Count().ToString();
        Badge_DEMARRAGE.Text        = ToolRegistry.GetByCategory("DEMARRAGE").Count().ToString();
        Badge_DIAGNOSTIC.Text       = ToolRegistry.GetByCategory("DIAGNOSTIC").Count().ToString();
        Badge_DISQUE.Text           = ToolRegistry.GetByCategory("DISQUE").Count().ToString();
        Badge_MATERIEL.Text         = ToolRegistry.GetByCategory("MATERIEL").Count().ToString();
        Badge_MISES_A_JOUR.Text     = ToolRegistry.GetByCategory("MISES_A_JOUR").Count().ToString();
        Badge_NETTOYAGE.Text        = ToolRegistry.GetByCategory("NETTOYAGE").Count().ToString();
        Badge_NOUVEAUTE.Text        = ToolRegistry.GetByCategory("NOUVEAUTE").Count().ToString();
        Badge_PERSONNALISATION.Text = ToolRegistry.GetByCategory("PERSONNALISATION").Count().ToString();
        Badge_REPARATION.Text       = ToolRegistry.GetByCategory("REPARATION").Count().ToString();
        Badge_RESEAU.Text           = ToolRegistry.GetByCategory("RESEAU").Count().ToString();
        Badge_SECRETS.Text          = ToolRegistry.GetByCategory("SECRETS").Count().ToString();
        Badge_SECURITE.Text         = ToolRegistry.GetByCategory("SECURITE").Count().ToString();
    }

    private void UpdateFavoritesCount()
    {
        Badge_FAVORIS.Text = ToolRegistry.GetFavorites().Count().ToString();
    }

    private void WarmupPages()
    {
        try
        {
            string[] categories =
            {
                "APPLICATIONS", "DEMARRAGE", "DIAGNOSTIC", "DISQUE", "MATERIEL", "MISES_A_JOUR",
                "NETTOYAGE", "NOUVEAUTE", "PERSONNALISATION", "REPARATION", "RESEAU", "SECRETS", "SECURITE", "PARAMETRES"
            };
            foreach (var cat in categories)
            {
                if (!_pageCache.ContainsKey(cat))
                    GetOrCreatePage(cat);
            }
        }
        catch { }
    }

    // ─── Console Terminal Latéral ────────────────────────────────────

    /// <summary>
    /// Ajuste la largeur de la console à l'ouverture pour être compacte et garantir que 2 cartes d'outils restent côte à côte.
    /// </summary>
    private void AdjustConsoleWidth()
    {
        double available = ActualWidth - 240; // 240px = largeur sidebar gauche
        if (available > 600)
        {
            // Réserve au moins 740px pour que 2 cartes (325px + marges et défilement) restent côte à côte
            double maxConsole = available - 740;
            OutputPanel.Width = Math.Clamp(maxConsole, 280, 360);
        }
        else
        {
            OutputPanel.Width = 280;
        }
    }

    private void BtnCloseOutput_Click(object sender, RoutedEventArgs e)
    {
        if (ScriptExecutionCoordinator.IsRunning)
        {
            AppendOutput("\n🛑 [INTERRUPTION] Console fermée : annulation du script...");
            ScriptExecutionCoordinator.StopActiveExecution();
        }
        OutputPanel.Visibility = Visibility.Collapsed;
    }


    private void BtnZoomIn_Click(object sender, RoutedEventArgs e)
        => AdjustOutputFontSize(1);

    private void BtnZoomOut_Click(object sender, RoutedEventArgs e)
        => AdjustOutputFontSize(-1);

    private void OutputBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
        {
            e.Handled = true;
            if (e.Delta > 0)
                AdjustOutputFontSize(1);
            else if (e.Delta < 0)
                AdjustOutputFontSize(-1);
        }
    }

    private void AdjustOutputFontSize(double delta)
    {
        double newSize = Math.Clamp(OutputBox.FontSize + delta, 10.0, 24.0);
        OutputBox.FontSize = newSize;
        if (OutputBox.Document != null)
        {
            OutputBox.Document.FontSize = newSize;
        }
    }

    // ─── Redimensionnement de la console (Splitter) ──────────────────

    private bool   _isResizingTerminal;
    private Point  _resizeStartPoint;
    private double _startTerminalWidth;

    private void TerminalResizeGrip_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isResizingTerminal = true;
        _resizeStartPoint   = e.GetPosition(this);
        _startTerminalWidth = OutputPanel.ActualWidth;
        TerminalResizeGrip.CaptureMouse();
    }

    private void TerminalResizeGrip_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_isResizingTerminal) return;
        var    currentPoint = e.GetPosition(this);
        double deltaX       = _resizeStartPoint.X - currentPoint.X;
        double newWidth     = _startTerminalWidth + deltaX;
        double maxWidth     = Math.Max(400, ActualWidth * 0.75);
        OutputPanel.Width   = Math.Clamp(newWidth, 260, maxWidth);
    }

    private void TerminalResizeGrip_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!_isResizingTerminal) return;
        _isResizingTerminal = false;
        TerminalResizeGrip.ReleaseMouseCapture();
    }

    // ─── Navigation sidebar avec mise en cache des pages ─────────────

    private readonly System.Collections.Generic.Dictionary<string, Page> _pageCache = new();

    private Page GetOrCreatePage(string tag)
    {
        if (_pageCache.TryGetValue(tag, out var cachedPage))
        {
            if (cachedPage is FavorisPage fav)
                fav.RefreshFavoris();
            return cachedPage;
        }

        Page newPage = tag switch
        {
            "APPLICATIONS"     => new ApplicationsPage(),
            "DEMARRAGE"        => new DemarragePage(),
            "DIAGNOSTIC"       => new DiagnosticPage(),
            "DISQUE"           => new DisquePage(),
            "MATERIEL"         => new MaterielPage(),
            "MISES_A_JOUR"     => new MisesAJourPage(),
            "NETTOYAGE"        => new NettoyagePage(),
            "NOUVEAUTE"        => new NouveautePage(),
            "PERSONNALISATION" => new PersonnalisationPage(),
            "REPARATION"       => new ReparationPage(),
            "RESEAU"           => new ReseauPage(),
            "SECRETS"          => new SecretsPage(),
            "SECURITE"         => new SecuritePage(),
            "PARAMETRES"       => new ParametresPage(),
            _                  => new FavorisPage()
        };

        _pageCache[tag] = newPage;
        return newPage;
    }

    // ─── Navigation au survol (hover) avec debounce 50 ms ───────────
    private DispatcherTimer? _hoverNavTimer;
    private Button?          _pendingHoverBtn;

    private void NavCategory_MouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is not Button btn || (btn == _activeNavBtn && MainFrame.Content is not SearchResultsPage))
            return;

        _pendingHoverBtn = btn;
        _hoverNavTimer?.Stop();
        _hoverNavTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
        _hoverNavTimer.Tick += (_, _) =>
        {
            _hoverNavTimer?.Stop();
            if (_pendingHoverBtn != null && _pendingHoverBtn.IsMouseOver)
            {
                TriggerCategoryNavigation(_pendingHoverBtn);
            }
        };
        _hoverNavTimer.Start();
    }

    // ─── Réduction / Dépliage de la colonne catégories ───────────────
    private bool             _isSidebarCollapsed;
    private DispatcherTimer? _collapseSidebarTimer;

    private void Sidebar_MouseEnter(object sender, MouseEventArgs e)
    {
        _collapseSidebarTimer?.Stop();
        if (_isSidebarCollapsed)
        {
            SetSidebarCollapsed(false);
        }
    }

    private void MainContent_MouseEnter(object sender, MouseEventArgs e)
    {
        _hoverNavTimer?.Stop();
        _pendingHoverBtn = null;

        // Si l'utilisateur est en train de taper dans la recherche, ne pas réduire
        if (SearchBox.IsFocused) return;

        // Déclencher la réduction vers le mode compact après 100 ms
        _collapseSidebarTimer?.Stop();
        _collapseSidebarTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
        _collapseSidebarTimer.Tick += (_, _) =>
        {
            _collapseSidebarTimer?.Stop();
            if (!SidebarBorder.IsMouseOver && !SearchBox.IsFocused)
            {
                SetSidebarCollapsed(true);
            }
        };
        _collapseSidebarTimer.Start();
    }

    private void Sidebar_MouseLeave(object sender, MouseEventArgs e)
    {
        _hoverNavTimer?.Stop();
        _pendingHoverBtn = null;
    }

    private void SearchBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_isSidebarCollapsed)
        {
            SetSidebarCollapsed(false);
            SearchBox.Focus();
        }
    }

    private void SetSidebarCollapsed(bool isCollapsed)
    {
        if (_isSidebarCollapsed == isCollapsed) return;
        _isSidebarCollapsed = isCollapsed;

        // 1. En-tête / Logo
        HeaderExpanded.Visibility   = isCollapsed ? Visibility.Collapsed : Visibility.Visible;
        HeaderCollapsed.Visibility  = isCollapsed ? Visibility.Visible : Visibility.Collapsed;
        SidebarHeaderBorder.Padding = isCollapsed ? new Thickness(0, 18, 0, 18) : new Thickness(16, 20, 16, 16);

        // 2. Recherche
        if (isCollapsed)
        {
            SearchExpandedGrid.Visibility       = Visibility.Collapsed;
            SearchIconCollapsed.Visibility      = Visibility.Visible;
            SidebarSearchBorder.Padding         = new Thickness(8, 10, 8, 10);
            SearchBoxBorder.Width               = 48;
            SearchBoxBorder.Height              = 36;
            SearchBoxBorder.HorizontalAlignment = HorizontalAlignment.Center;
        }
        else
        {
            SearchExpandedGrid.Visibility       = Visibility.Visible;
            SearchIconCollapsed.Visibility      = Visibility.Collapsed;
            SidebarSearchBorder.Padding         = new Thickness(12, 10, 12, 10);
            SearchBoxBorder.Width               = double.NaN;
            SearchBoxBorder.Height              = double.NaN;
            SearchBoxBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        // 3. Séparateur et titre "CATÉGORIES"
        CategoriesHeader.Visibility = isCollapsed ? Visibility.Collapsed : Visibility.Visible;
        CategoriesSeparator.Margin  = isCollapsed ? new Thickness(4, 6, 4, 6) : new Thickness(4, 8, 4, 8);

        // 4. Boutons de navigation (Favoris, Paramètres, et 13 catégories)
        UpdateButtonCollapsedState(NavFavoris, isCollapsed);
        UpdateButtonCollapsedState(NavParametres, isCollapsed);

        foreach (var child in CategoryStackPanel.Children)
        {
            if (child is Button catBtn)
            {
                UpdateButtonCollapsedState(catBtn, isCollapsed);
            }
        }

        // 5. Barre de défilement toujours masquée (défilement à la molette préservé)
        SidebarScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

        // 6. Animation fluide de largeur
        double targetWidth  = isCollapsed ? 64.0 : 240.0;
        double currentWidth = SidebarBorder.ActualWidth;
        if (double.IsNaN(currentWidth) || currentWidth == 0)
            currentWidth = isCollapsed ? 240.0 : 64.0;

        var anim = new DoubleAnimation
        {
            From           = currentWidth,
            To             = targetWidth,
            Duration       = TimeSpan.FromMilliseconds(180),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
        };
        SidebarBorder.BeginAnimation(FrameworkElement.WidthProperty, anim);
    }

    private void UpdateButtonCollapsedState(Button btn, bool isCollapsed)
    {
        btn.Padding = isCollapsed ? new Thickness(0, 8, 0, 8) : new Thickness(10, 8, 10, 8);

        if (btn.Content is DockPanel dock)
        {
            foreach (UIElement child in dock.Children)
            {
                if (child is Border badgeBorder)
                {
                    badgeBorder.Visibility = isCollapsed ? Visibility.Collapsed : Visibility.Visible;
                }
                else if (child is StackPanel sp)
                {
                    sp.HorizontalAlignment = isCollapsed ? HorizontalAlignment.Center : HorizontalAlignment.Left;
                    if (sp.Children.Count >= 2)
                    {
                        if (sp.Children[0] is TextBlock iconTb)
                        {
                            iconTb.Margin = isCollapsed ? new Thickness(0) : new Thickness(0, 0, 10, 0);
                        }
                        if (sp.Children[1] is TextBlock labelTb)
                        {
                            labelTb.Visibility = isCollapsed ? Visibility.Collapsed : Visibility.Visible;
                        }
                    }
                }
            }
        }
    }

    private void TriggerCategoryNavigation(Button btn, bool force = false)
    {
        if (!force && btn == _activeNavBtn && MainFrame.Content is not SearchResultsPage)
            return;

        if (btn == NavFavoris)
        {
            NavigateTo(btn, GetOrCreatePage("FAVORIS"), "Mes favoris", "Vos outils préférés accessibles rapidement");
        }
        else if (btn == NavParametres)
        {
            NavigateTo(btn, GetOrCreatePage("PARAMETRES"), "Paramètres", "Configuration générale et structure des outils");
        }
        else
        {
            var tag = btn.Tag?.ToString() ?? "";
            var page = GetOrCreatePage(tag);
            var (icon, name) = SectionViewBuilder.GetCategoryMeta(tag);
            string title = $"{icon} {name}";
            string subtitle = SectionViewBuilder.GetCategorySubtitle(tag);
            NavigateTo(btn, page, title, subtitle);
        }
    }

    private void NavFavoris_Click(object sender, RoutedEventArgs e)
    {
        _hoverNavTimer?.Stop();
        _pendingHoverBtn = null;
        if (sender is Button btn)
            TriggerCategoryNavigation(btn, force: true);
    }

    private void NavParametres_Click(object sender, RoutedEventArgs e)
    {
        _hoverNavTimer?.Stop();
        _pendingHoverBtn = null;
        if (sender is Button btn)
            TriggerCategoryNavigation(btn, force: true);
    }

    private void NavCategory_Click(object sender, RoutedEventArgs e)
    {
        _hoverNavTimer?.Stop();
        _pendingHoverBtn = null;
        if (sender is Button btn)
            TriggerCategoryNavigation(btn, force: true);
    }

    private void NavigateTo(Button navBtn, Page page, string title, string subtitle)
    {
        if (_activeNavBtn != null)
            _activeNavBtn.Style = (Style)FindResource("SidebarItem");

        navBtn.Style    = (Style)FindResource("SidebarItemActive");
        _activeNavBtn   = navBtn;

        // Déplier toutes les sous-catégories de la page affichée pour afficher directement tous les outils
        if (page is ISectionPage currentSectionPage)
        {
            currentSectionPage.ResetSections();
        }

        MainFrame.Navigate(page);
        PageTitle.Text    = title;
        PageSubtitle.Text = subtitle;
    }

    // ─── Recherche avec debounce 200 ms & gestion du focus ──────────

    private void ClearSearchFocus()
    {
        bool hasSearchFocus = SearchBox.IsKeyboardFocused ||
                              SearchBox.IsFocused ||
                              SearchBox.IsKeyboardFocusWithin ||
                              Keyboard.FocusedElement == SearchBox;

        if (hasSearchFocus)
        {
            var searchScope = FocusManager.GetFocusScope(SearchBox);
            if (searchScope != null)
            {
                FocusManager.SetFocusedElement(searchScope, null);
            }
            FocusManager.SetFocusedElement(this, MainRootGrid);
            Keyboard.ClearFocus();
            Keyboard.Focus(MainRootGrid);
        }
    }

    private static bool IsDescendantOf(DependencyObject? child, DependencyObject? parent)
    {
        while (child != null)
        {
            if (child == parent) return true;
            child = VisualTreeHelper.GetParent(child);
        }
        return false;
    }

    private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        bool isInsideSearch = SearchBoxBorder.IsMouseOver ||
                              SearchBox.IsMouseOver ||
                              (e.OriginalSource is DependencyObject d && IsDescendantOf(d, SearchBoxBorder));

        if (!isInsideSearch)
        {
            ClearSearchFocus();

            // Si le clic s'est produit en dehors de la barre latérale, lancer la réduction
            if (!SidebarBorder.IsMouseOver)
            {
                _collapseSidebarTimer?.Stop();
                SetSidebarCollapsed(true);
            }
        }
    }

    private void MainContent_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        ClearSearchFocus();
        if (!SidebarBorder.IsMouseOver)
        {
            _collapseSidebarTimer?.Stop();
            SetSidebarCollapsed(true);
        }
    }

    private void SearchBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            SearchBox.Text = "";
            ClearSearchFocus();
            e.Handled = true;
        }
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Réinitialiser le timer à chaque frappe
        _searchDebounce?.Stop();
        _searchDebounce = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
        _searchDebounce.Tick += (_, _) =>
        {
            _searchDebounce?.Stop();
            ApplySearch();
        };
        _searchDebounce.Start();
    }

    private void ApplySearch()
    {
        var query = SearchBox.Text.Trim();

        if (query.Length >= 2)
        {
            if (MainFrame.Content is SearchResultsPage searchPage)
            {
                searchPage.UpdateSearch(query);
            }
            else
            {
                MainFrame.Navigate(new SearchResultsPage(query));
                PageTitle.Text    = "🔍 Recherche";
                PageSubtitle.Text = $"Résultats pour \"{query}\"";
            }
        }
        else if (query.Length == 0 && MainFrame.Content is SearchResultsPage)
        {
            if (_activeNavBtn != null)
                TriggerCategoryNavigation(_activeNavBtn, force: true);
            else
                NavigateTo(NavFavoris, new FavorisPage(), "Mes Favoris", "Vos outils préférés accessibles rapidement");
        }
    }

    // ─── Terminal Output ─────────────────────────────────────────────

    /// <summary>Affiche une ligne dans le terminal output avec coloration (non-bloquant).</summary>
    public void AppendOutput(string line)
    {
        // InvokeAsync = non-bloquant, ne gèle pas le thread appelant
        Dispatcher.InvokeAsync(() =>
        {
            if (OutputPanel.Visibility != Visibility.Visible)
            {
                AdjustConsoleWidth();
                OutputPanel.Visibility = Visibility.Visible;
            }

            var brush = (line.StartsWith("[ERR]") || line.Contains("[ERREUR]") || line.Contains("❌"))
                      ? BrushError
                      : (line.StartsWith("[OK]") || line.Contains("✅"))
                      ? BrushSuccess
                      : (line.StartsWith("[!]") || line.Contains("⚠️"))
                      ? BrushWarning
                      : (line.StartsWith("[i]") || line.Contains("🚀") || line.Contains("🎨"))
                      ? BrushInfo
                      : BrushDefault;

            var para = new Paragraph { Margin = new Thickness(0, 1, 0, 1) };
            para.Inlines.Add(new Run(line) { Foreground = brush });
            OutputBox.Document.Blocks.Add(para);
            OutputBox.ScrollToEnd();
        }, DispatcherPriority.Normal);
    }

    public void SetRunning(bool isRunning)
    {
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        if (ScriptExecutionCoordinator.IsRunning)
        {
            ScriptExecutionCoordinator.StopActiveExecution();
        }
        base.OnClosing(e);
    }
}
