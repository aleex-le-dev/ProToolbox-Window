using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ScriptsWindows.Core;
using ScriptsWindows.Views.Pages;

namespace ScriptsWindows;

public partial class MainWindow : Window
{
    private Button? _activeNavBtn;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Appliquer le thème actif et initialiser l'état du bouton
        ThemeManager.ApplyTheme(ThemeManager.CurrentTheme);
        UpdateThemeButtonState();

        // Démarrer sur la page Favoris
        NavigateTo(NavFavoris, new FavorisPage(), "⭐ Mes Favoris", "Vos outils préférés accessibles rapidement");
    }

    // ─── Thème ───────────────────────────────────────────────────────

    private void BtnTheme_Click(object sender, RoutedEventArgs e)
    {
        var newTheme = ThemeManager.NextTheme();
        UpdateThemeButtonState();
        // Aucun log dans la console
    }

    private void UpdateThemeButtonState()
    {
        var current = ThemeManager.CurrentTheme;
        TxtThemeIcon.Text = current.Icon;
        BtnTheme.ToolTip = $"Thème actif : {current.Name} ({current.VisualStyle})\nCliquez pour changer de style";
    }

    // ─── Console Terminal Latéral ────────────────────────────────────

    private void BtnCloseOutput_Click(object sender, RoutedEventArgs e)
    {
        OutputPanel.Visibility = Visibility.Collapsed;
    }

    private void BtnClearOutput_Click(object sender, RoutedEventArgs e)
    {
        OutputBox.Document.Blocks.Clear();
    }

    // ─── Redimensionnement de la console (Splitter) ──────────────────

    private bool _isResizingTerminal;
    private Point _resizeStartPoint;
    private double _startTerminalWidth;

    private void TerminalResizeGrip_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isResizingTerminal = true;
        _resizeStartPoint = e.GetPosition(this);
        _startTerminalWidth = OutputPanel.ActualWidth;
        TerminalResizeGrip.CaptureMouse();
    }

    private void TerminalResizeGrip_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_isResizingTerminal) return;

        var currentPoint = e.GetPosition(this);
        double deltaX = _resizeStartPoint.X - currentPoint.X; // Glisser à gauche = agrandir
        double newWidth = _startTerminalWidth + deltaX;

        // Limites : min 200px, max 70% de la largeur de fenêtre
        double maxWidth = Math.Max(300, ActualWidth * 0.7);
        OutputPanel.Width = Math.Clamp(newWidth, 200, maxWidth);
    }

    private void TerminalResizeGrip_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_isResizingTerminal)
        {
            _isResizingTerminal = false;
            TerminalResizeGrip.ReleaseMouseCapture();
        }
    }

    // ─── Navigation sidebar ──────────────────────────────────────────

    private void NavFavoris_Click(object sender, RoutedEventArgs e)
        => NavigateTo((Button)sender, new FavorisPage(), "⭐ Mes Favoris", "Vos outils préférés accessibles rapidement");

    private void NavCategory_Click(object sender, RoutedEventArgs e)
    {
        var btn = (Button)sender;
        var tag = btn.Tag?.ToString() ?? "";

        Page page = tag switch
        {
            "DIAGNOSTIC"       => new DiagnosticPage(),
            "REPARATION"       => new ReparationPage(),
            "NETTOYAGE"        => new NettoyagePage(),
            "RESEAU"           => new ReseauPage(),
            "DISQUE"           => new DisquePage(),
            "APPLICATIONS"     => new ApplicationsPage(),
            "SECURITE"         => new SecuritePage(),
            "EXTRACTION"       => new ExtractionPage(),
            "PERSONNALISATION" => new PersonnalisationPage(),
            "MATERIEL"         => new MaterielPage(),
            _                  => new FavorisPage()
        };

        var (title, subtitle) = tag switch
        {
            "DIAGNOSTIC"       => ("🔍 Diagnostic", "Analyse approfondie de la configuration et des composants système"),
            "REPARATION"       => ("🔧 Réparation", "Intégrité système SFC, DISM, CHKDSK et redémarrages de secours"),
            "NETTOYAGE"        => ("🧹 Nettoyage", "Optimisation, suppression des fichiers temporaires et caches"),
            "RESEAU"           => ("🌐 Réseau", "Configuration DNS, audit Wi-Fi, réinitialisation de pile réseau"),
            "DISQUE"           => ("💽 Disque", "Formatage sécurisé, état SMART et gestion des disques"),
            "APPLICATIONS"     => ("📦 Applications", "Gestion, mise à jour et installation de logiciels"),
            "SECURITE"         => ("🔐 Sécurité", "Comptes d'utilisateurs, Defender, mots de passe Wi-Fi et audit"),
            "EXTRACTION"       => ("📤 Extraction", "Récupération de licences, exports et sauvegardes"),
            "PERSONNALISATION" => ("🎨 Personnalisation", "God Mode, menu contextuel classique et réglages Windows"),
            "MATERIEL"         => ("🖥️ Matériel", "Écran tactile, imprimantes, performances et alimentation"),
            _                  => ("⭐ Mes Favoris", "Vos outils préférés accessibles rapidement")
        };

        NavigateTo(btn, page, title, subtitle);
    }

    private void NavigateTo(Button navBtn, Page page, string title, string subtitle)
    {
        // Réinitialiser le bouton actif
        if (_activeNavBtn != null)
            _activeNavBtn.Style = (Style)FindResource("SidebarItem");

        // Activer le nouveau bouton
        navBtn.Style = (Style)FindResource("SidebarItemActive");
        _activeNavBtn = navBtn;

        // Naviguer
        MainFrame.Navigate(page);
        PageTitle.Text    = title;
        PageSubtitle.Text = subtitle;
    }

    // ─── Recherche ───────────────────────────────────────────────────

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
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
                PageTitle.Text = "🔍 Recherche";
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

    /// <summary>Affiche une ligne dans le terminal output avec coloration.</summary>
    public void AppendOutput(string line)
    {
        Dispatcher.Invoke(() =>
        {
            // S'assurer que la console est visible lors de l'exécution
            OutputPanel.Visibility = Visibility.Visible;
            RunningIndicator.Visibility = Visibility.Visible;

            var para = new Paragraph { Margin = new Thickness(0, 1, 0, 1) };
            var run  = new Run(line);

            // Coloration syntaxique moderne
            run.Foreground = (line.StartsWith("[ERR]") || line.Contains("[ERREUR]") || line.Contains("❌"))
                           ? new SolidColorBrush(Color.FromRgb(239, 68, 68))
                           : (line.StartsWith("[OK]") || line.Contains("✅"))
                           ? new SolidColorBrush(Color.FromRgb(16, 185, 129))
                           : (line.StartsWith("[!]") || line.Contains("⚠️"))
                           ? new SolidColorBrush(Color.FromRgb(245, 158, 11))
                           : (line.StartsWith("[i]") || line.Contains("🚀") || line.Contains("🎨"))
                           ? new SolidColorBrush(Color.FromRgb(129, 140, 248))
                           : new SolidColorBrush(Color.FromRgb(209, 213, 219));

            para.Inlines.Add(run);
            OutputBox.Document.Blocks.Add(para);
            OutputBox.ScrollToEnd();
        });
    }

    public void SetRunning(bool isRunning)
    {
        Dispatcher.Invoke(() =>
        {
            RunningIndicator.Visibility = isRunning ? Visibility.Visible : Visibility.Collapsed;
        });
    }
}
