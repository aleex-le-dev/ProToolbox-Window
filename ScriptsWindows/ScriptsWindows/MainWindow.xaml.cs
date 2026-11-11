using System;
using System.Buffers;
using System.Linq;
using System.Threading;
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
    private static readonly SolidColorBrush BrushError   = Freeze(Color.FromRgb(0xFF, 0x7B, 0x72));
    private static readonly SolidColorBrush BrushSuccess = Freeze(Color.FromRgb(0x56, 0xD3, 0x64));
    private static readonly SolidColorBrush BrushWarning = Freeze(Color.FromRgb(0xE3, 0xB3, 0x41));
    private static readonly SolidColorBrush BrushInfo    = Freeze(Color.FromRgb(0x7E, 0xB0, 0xFF));
    private static readonly SolidColorBrush BrushDefault = Freeze(Color.FromRgb(0xD4, 0xD9, 0xDF));

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
        NavigateTo(NavFavoris, GetOrCreatePage("FAVORIS"), "Favoris", "Vos outils préférés accessibles rapidement");

        // Préchauffage asynchrone de toutes les pages (0 ms de délai au clic)
        Dispatcher.InvokeAsync(WarmupPages, DispatcherPriority.Background);

        // Vérification silencieuse et asynchrone des mises à jour en ligne
        _ = CheckForUpdatesSilentlyAsync();
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

    private static readonly string[] WarmupCategories =
    {
        "APPLICATIONS", "DEMARRAGE", "DIAGNOSTIC", "DISQUE", "MATERIEL", "MISES_A_JOUR",
        "NETTOYAGE", "NOUVEAUTE", "PERSONNALISATION", "REPARATION", "RESEAU", "SECRETS", "SECURITE", "PARAMETRES"
    };

    /// <summary>
    /// Préchauffe UNE page par passage du Dispatcher (priorité ApplicationIdle) puis se replanifie :
    /// l'UI reste réactive entre deux pages au lieu de geler le temps de construire les 14 d'un coup.
    /// </summary>
    private void WarmupPages() => WarmupNextPage(0);

    private void WarmupNextPage(int index)
    {
        while (index < WarmupCategories.Length && _pageCache.ContainsKey(WarmupCategories[index]))
            index++;

        if (index >= WarmupCategories.Length) return;

        try { GetOrCreatePage(WarmupCategories[index]); }
        catch { }

        int next = index + 1;
        Dispatcher.InvokeAsync(() => WarmupNextPage(next), DispatcherPriority.ApplicationIdle);
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


    private void BtnClearOutput_Click(object sender, RoutedEventArgs e)
    {
        OutputBox.Document.Blocks.Clear();
        _consoleBlockCount = 0;
        _lastProgressPara  = null;
        _lastProgressKey   = null;
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
        // FavorisPage se met à jour seule (Loaded + FavoriteChanged), seulement si nécessaire
        if (_pageCache.TryGetValue(tag, out var cachedPage))
            return cachedPage;

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
        SidebarHeaderBorder.Padding = isCollapsed ? new Thickness(0, 18, 0, 14) : new Thickness(16, 18, 16, 14);

        // 2. Recherche
        if (isCollapsed)
        {
            SearchExpandedGrid.Visibility       = Visibility.Collapsed;
            SearchIconCollapsed.Visibility      = Visibility.Visible;
            SidebarSearchBorder.Padding         = new Thickness(8, 0, 8, 10);
            SearchBoxBorder.Width               = 48;
            SearchBoxBorder.Height              = 34;
            SearchBoxBorder.HorizontalAlignment = HorizontalAlignment.Center;
        }
        else
        {
            SearchExpandedGrid.Visibility       = Visibility.Visible;
            SearchIconCollapsed.Visibility      = Visibility.Collapsed;
            SidebarSearchBorder.Padding         = new Thickness(12, 0, 12, 10);
            SearchBoxBorder.Width               = double.NaN;
            SearchBoxBorder.Height              = 34;
            SearchBoxBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        // 3. Séparateur et titre "CATÉGORIES"
        CategoriesHeader.Visibility = isCollapsed ? Visibility.Collapsed : Visibility.Visible;
        CategoriesSeparator.Margin  = isCollapsed ? new Thickness(4, 4, 4, 4) : new Thickness(4, 8, 4, 4);

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
        btn.Padding = isCollapsed ? new Thickness(0, 7, 0, 7) : new Thickness(10, 7, 10, 7);

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
                            iconTb.Margin = isCollapsed ? new Thickness(0) : new Thickness(0, 0, 12, 0);
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
            NavigateTo(btn, GetOrCreatePage("FAVORIS"), "Favoris", "Vos outils préférés accessibles rapidement");
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
            string title = name;
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
            // Le texte d'un RichTextBox (Paragraph, Run…) n'est pas un Visual : VisualTreeHelper lèverait une exception
            child = child is Visual or System.Windows.Media.Media3D.Visual3D
                ? VisualTreeHelper.GetParent(child)
                : LogicalTreeHelper.GetParent(child);
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
                PageTitle.Text    = "Recherche";
                PageSubtitle.Text = $"Résultats pour \"{query}\"";
            }
        }
        else if (query.Length == 0 && MainFrame.Content is SearchResultsPage)
        {
            if (_activeNavBtn != null)
                TriggerCategoryNavigation(_activeNavBtn, force: true);
            else
                NavigateTo(NavFavoris, GetOrCreatePage("FAVORIS"), "Favoris", "Vos outils préférés accessibles rapidement");
        }
    }

    // ─── Terminal Output ─────────────────────────────────────────────

    // Tampon de sortie : les lignes arrivent de n'importe quel thread et sont rendues par lots
    // (1 dispatch + 1 ScrollToEnd par lot au lieu d'1 par ligne → UI fluide même avec SFC/DISM/winget).
    private const int MaxConsoleBlocks   = 3000; // au-delà, les plus anciennes lignes sont supprimées
    private const int MaxLinesPerFlush   = 400;  // borne le travail par frame, le reste est replanifié

    private readonly System.Collections.Concurrent.ConcurrentQueue<string> _pendingOutput = new();
    private int _outputFlushScheduled;           // 0/1, manipulé via Interlocked
    private int _consoleBlockCount;
    private Paragraph? _lastProgressPara;         // dernière ligne de progression (remplaçable)
    private string?    _lastProgressKey;

    private static readonly SearchValues<char> ProgressNoiseChars =
        SearchValues.Create("0123456789.,%= []()|/\\-#>*█▓▒░■□▌▐ \t");

    private static readonly SearchValues<char> ProgressBarChars = SearchValues.Create("█▓▒░■");

    /// <summary>Affiche une ligne dans le terminal output avec coloration (non-bloquant, thread-safe).</summary>
    public void AppendOutput(string line)
    {
        _pendingOutput.Enqueue(line);

        if (Interlocked.Exchange(ref _outputFlushScheduled, 1) == 0)
            Dispatcher.InvokeAsync(FlushOutput, DispatcherPriority.Background);
    }

    private void FlushOutput()
    {
        // Relâcher le drapeau AVANT de vider : une ligne arrivée pendant le flush replanifie un passage
        Interlocked.Exchange(ref _outputFlushScheduled, 0);

        if (_pendingOutput.IsEmpty) return;

        if (OutputPanel.Visibility != Visibility.Visible)
        {
            AdjustConsoleWidth();
            OutputPanel.Visibility = Visibility.Visible;
        }

        var blocks = OutputBox.Document.Blocks;
        int processed = 0;

        while (processed < MaxLinesPerFlush && _pendingOutput.TryDequeue(out var line))
        {
            processed++;

            // Barres de progression (SFC « 12 % », DISM « [=== 12.3% ] », winget « ███▒▒ 1.2 MB ») :
            // on met à jour la ligne précédente au lieu d'en empiler des milliers.
            string? progressKey = GetProgressKey(line);
            if (progressKey != null && progressKey == _lastProgressKey && _lastProgressPara != null)
            {
                if (_lastProgressPara.Inlines.FirstInline is Run run)
                    run.Text = line;
                continue;
            }

            var para = new Paragraph { Margin = new Thickness(0, 0, 0, 2) };
            para.Inlines.Add(new Run(line) { Foreground = GetLineBrush(line) });
            blocks.Add(para);
            _consoleBlockCount++;

            _lastProgressPara = progressKey != null ? para : null;
            _lastProgressKey  = progressKey;
        }

        while (_consoleBlockCount > MaxConsoleBlocks && blocks.FirstBlock != null)
        {
            if (blocks.FirstBlock == _lastProgressPara) { _lastProgressPara = null; _lastProgressKey = null; }
            blocks.Remove(blocks.FirstBlock);
            _consoleBlockCount--;
        }

        OutputBox.ScrollToEnd();

        // Encore des lignes en attente (rafale) → nouveau passage après le rendu de cette frame
        if (!_pendingOutput.IsEmpty && Interlocked.Exchange(ref _outputFlushScheduled, 1) == 0)
            Dispatcher.InvokeAsync(FlushOutput, DispatcherPriority.Background);
    }

    private static SolidColorBrush GetLineBrush(string line) =>
        (line.StartsWith("[ERR]") || line.Contains("[ERREUR]") || line.Contains("❌")) ? BrushError
      : (line.StartsWith("[OK]")  || line.Contains("✅"))                              ? BrushSuccess
      : (line.StartsWith("[!]")   || line.Contains("⚠️"))                              ? BrushWarning
      : (line.StartsWith("[i]")   || line.Contains("🚀") || line.Contains("🎨"))       ? BrushInfo
      : BrushDefault;

    /// <summary>
    /// Retourne une « signature » de ligne de progression (texte sans chiffres ni caractères de barre),
    /// ou null si la ligne n'est pas une progression. Deux lignes de même signature se remplacent.
    /// </summary>
    private static string? GetProgressKey(string line)
    {
        if (line.Length > 300) return null;
        if (!line.Contains('%') && line.AsSpan().IndexOfAny(ProgressBarChars) < 0) return null;

        var sb = new System.Text.StringBuilder(line.Length);
        foreach (char c in line)
            if (!ProgressNoiseChars.Contains(c)) sb.Append(c);

        return sb.ToString();
    }

    private async System.Threading.Tasks.Task CheckForUpdatesSilentlyAsync()
    {
        try
        {
            await System.Threading.Tasks.Task.Delay(2500); // Ne pas ralentir le démarrage immédiat
            var update = await Services.UpdateService.CheckSilentlyOnStartupAsync();
            if (update != null)
            {
                Dispatcher.Invoke(() =>
                {
                    TxtUpdateNotificationLabel.Text = $"Mise à jour {update.TagName} disponible";
                    BtnUpdateNotification.Visibility = Visibility.Visible;
                });
            }
        }
        catch { }
    }

    private async void BtnUpdateNotification_Click(object sender, RoutedEventArgs e)
    {
        await Services.UpdateService.OpenUpdateDialog();
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
