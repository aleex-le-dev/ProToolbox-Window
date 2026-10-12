using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ScriptsWindows.Core;
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
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        ThemeManager.ApplyTheme(ThemeManager.CurrentTheme);
        UpdateThemeButtonState();

        // Démarrer sur la page Favoris
        NavigateTo(NavFavoris, GetOrCreatePage("FAVORIS"), "⭐ Mes Favoris", "Vos outils préférés accessibles rapidement");

        // Préchauffage asynchrone de toutes les pages (0 ms de délai au clic)
        Dispatcher.InvokeAsync(WarmupPages, DispatcherPriority.Background);
    }

    private void WarmupPages()
    {
        try
        {
            string[] categories =
            {
                "DIAGNOSTIC", "MATERIEL", "REPARATION", "NETTOYAGE", "SECRETS", "RESEAU",
                "DISQUE", "APPLICATIONS", "MISES_A_JOUR", "SECURITE", "PERSONNALISATION"
            };
            foreach (var cat in categories)
            {
                if (!_pageCache.ContainsKey(cat))
                    GetOrCreatePage(cat);
            }
        }
        catch { }
    }

    // ─── Thème ───────────────────────────────────────────────────────

    private void BtnTheme_Click(object sender, RoutedEventArgs e)
    {
        ThemeManager.NextTheme();
        UpdateThemeButtonState();
    }

    private void UpdateThemeButtonState()
    {
        var current = ThemeManager.CurrentTheme;
        TxtThemeIcon.Text = current.Icon;
        TxtThemeName.Text = current.Name;
        BtnTheme.ToolTip  = $"Thème actif : {current.Name} ({current.VisualStyle})\nCliquez pour basculer vers le thème suivant";
    }

    // ─── Console Terminal Latéral ────────────────────────────────────

    private void BtnToggleConsole_Click(object sender, RoutedEventArgs e)
    {
        if (OutputPanel.Visibility == Visibility.Visible)
        {
            OutputPanel.Visibility = Visibility.Collapsed;
        }
        else
        {
            AdjustConsoleWidth();
            OutputPanel.Visibility = Visibility.Visible;
        }
    }

    /// <summary>
    /// Ajuste la largeur de la console pour qu'elle ait la même largeur que la zone de scripts (partage 50/50 de l'espace disponible).
    /// </summary>
    private void AdjustConsoleWidth()
    {
        double available = ActualWidth - 220; // 220px = sidebar gauche
        if (available > 600)
        {
            double halfWidth = Math.Round(available / 2.0);
            OutputPanel.Width = Math.Clamp(halfWidth, 380, available * 0.7);
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
        => OutputBox.Document.Blocks.Clear();

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
        OutputPanel.Width   = Math.Clamp(newWidth, 300, maxWidth);
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
            "DIAGNOSTIC"       => new DiagnosticPage(),
            "MATERIEL"         => new MaterielPage(),
            "REPARATION"       => new ReparationPage(),
            "NETTOYAGE"        => new NettoyagePage(),
            "SECRETS"          => new SecretsPage(),
            "RESEAU"           => new ReseauPage(),
            "DISQUE"           => new DisquePage(),
            "APPLICATIONS"     => new ApplicationsPage(),
            "MISES_A_JOUR"     => new MisesAJourPage(),
            "SECURITE"         => new SecuritePage(),
            "PERSONNALISATION" => new PersonnalisationPage(),
            _                  => new FavorisPage()
        };

        _pageCache[tag] = newPage;
        return newPage;
    }

    private void NavFavoris_Click(object sender, RoutedEventArgs e)
        => NavigateTo((Button)sender, GetOrCreatePage("FAVORIS"), "⭐ Mes Favoris", "Vos outils préférés accessibles rapidement");

    private void NavCategory_Click(object sender, RoutedEventArgs e)
    {
        var btn = (Button)sender;
        var tag = btn.Tag?.ToString() ?? "";
        var page = GetOrCreatePage(tag);

        var (title, subtitle) = tag switch
        {
            "DIAGNOSTIC"       => ("🔍 Diagnostic Matériel & Intégrité", "Analyse carte mère, CPU, RAM, disques physiques, batterie et réseau"),
            "MATERIEL"         => ("🖥️ Matériel & Périphériques", "Outils de diagnostic mémoire, GPU/DxDiag, périphériques et benchmarks"),
            "REPARATION"       => ("🔧 Réparation Intégrale", "CHKDSK, spouleur, pipeline DISM+SFC, cache icônes et Windows Update"),
            "NETTOYAGE"        => ("🧹 Nettoyage & Optimisation Espace", "Nettoyage unifié, magasins WinSxS, caches navigateurs, temp et cleanmgr"),
            "SECRETS"          => ("🔑 Récupération d'Identifiants & Secrets", "Mots de passe Wi-Fi, clés Windows, documents sensibles et profils web"),
            "RESEAU"           => ("🌐 Réseau & Configuration", "DNS Cloudflare/Google, purge ARP, rapport Wi-Fi et audit des connexions"),
            "DISQUE"           => ("💽 Disque Dur & Stockage", "Formatage Diskpart, défragmentation, TRIM SSD, gestionnaire et BitLocker"),
            "APPLICATIONS"     => ("📦 Installation d'Applications", "Installation automatisée de navigateurs, lecteurs et utilitaires via winget"),
            "MISES_A_JOUR"     => ("🔄 Mises à Jour & Inventaire Logiciel", "Inventaire applicatif, mise à niveau winget et signatures Defender"),
            "SECURITE"         => ("🛡️ Surveillance, Sécurité & Événements", "Scan Defender, journaux d'erreurs, fiabilité perfmon et monitoring des ressources"),
            "PERSONNALISATION" => ("🎨 Personnalisation & Système", "God Mode et rétablissement du menu contextuel classique Windows 10"),
            _                  => ("⭐ Mes Favoris", "Vos outils préférés accessibles rapidement")
        };

        NavigateTo(btn, page, title, subtitle);
    }

    private void NavigateTo(Button navBtn, Page page, string title, string subtitle)
    {
        if (_activeNavBtn != null)
            _activeNavBtn.Style = (Style)FindResource("SidebarItem");

        navBtn.Style    = (Style)FindResource("SidebarItemActive");
        _activeNavBtn   = navBtn;

        // Replier toutes les sous-catégories de toutes les pages mises en cache
        foreach (var cachedPage in _pageCache.Values)
        {
            if (cachedPage is ISectionPage sp)
            {
                sp.ResetSections();
            }
        }

        // S'assurer que la page affichée a également toutes ses sous-catégories fermées
        if (page is ISectionPage currentSectionPage)
        {
            currentSectionPage.ResetSections();
        }

        MainFrame.Navigate(page);
        PageTitle.Text    = title;
        PageSubtitle.Text = subtitle;
    }

    // ─── Recherche avec debounce 200 ms ──────────────────────────────

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
                NavCategory_Click(_activeNavBtn, new RoutedEventArgs());
            else
                NavigateTo(NavFavoris, new FavorisPage(), "⭐ Mes Favoris", "Vos outils préférés accessibles rapidement");
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
        }, DispatcherPriority.Background);
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
