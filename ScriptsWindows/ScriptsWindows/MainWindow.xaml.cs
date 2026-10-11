using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
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
        // Démarrer sur la page Favoris
        NavigateTo(NavFavoris, new FavorisPage(), "⭐ Mes Favoris", "Vos outils préférés");
    }

    // ─── Navigation sidebar ──────────────────────────────────────────

    private void NavFavoris_Click(object sender, RoutedEventArgs e)
        => NavigateTo((Button)sender, new FavorisPage(), "⭐ Mes Favoris", "Vos outils préférés");

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
            "DIAGNOSTIC"       => ("🔍 Diagnostic", "Analyse et diagnostic système"),
            "REPARATION"       => ("🔧 Réparation", "Outils de réparation Windows"),
            "NETTOYAGE"        => ("🧹 Nettoyage", "Nettoyage et optimisation système"),
            "RESEAU"           => ("🌐 Réseau", "DNS, Wi-Fi, LAN et pentest réseau"),
            "DISQUE"           => ("💽 Disque", "Formatage et gestion des disques"),
            "APPLICATIONS"     => ("📦 Applications", "Winget, installations et mises à jour"),
            "SECURITE"         => ("🔐 Sécurité", "Comptes, Defender et extraction"),
            "EXTRACTION"       => ("📤 Extraction", "Export de données et clés"),
            "PERSONNALISATION" => ("🎨 Personnalisation", "Menu contextuel, God Mode, thèmes"),
            "MATERIEL"         => ("🖥️ Matériel", "Écran tactile, imprimantes, plan d'alimentation"),
            _                  => ("⭐ Favoris", "Vos outils préférés")
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
            // TODO: ouvrir SearchResultsPage filtrée
        }
    }

    // ─── Terminal Output ─────────────────────────────────────────────

    /// <summary>Affiche une ligne dans le terminal output avec coloration.</summary>
    public void AppendOutput(string line)
    {
        Dispatcher.Invoke(() =>
        {
            OutputPanel.Visibility = Visibility.Visible;
            RunningIndicator.Visibility = Visibility.Visible;

            var para = new Paragraph { Margin = new Thickness(0) };
            var run  = new Run(line);

            // Coloration syntaxique basique
            run.Foreground = line.StartsWith("[ERR]") ? new SolidColorBrush(Color.FromRgb(239, 68, 68))
                           : line.StartsWith("[OK]")  ? new SolidColorBrush(Color.FromRgb(16, 185, 129))
                           : line.StartsWith("[!]")   ? new SolidColorBrush(Color.FromRgb(245, 158, 11))
                           : new SolidColorBrush(Color.FromRgb(209, 213, 219));

            para.Inlines.Add(run);
            OutputBox.Document.Blocks.Add(para);
            OutputScroll.ScrollToEnd();
        });
    }

    public void SetRunning(bool isRunning)
    {
        Dispatcher.Invoke(() =>
        {
            RunningIndicator.Visibility = isRunning ? Visibility.Visible : Visibility.Collapsed;
        });
    }

    private void BtnClearOutput_Click(object sender, RoutedEventArgs e)
    {
        OutputBox.Document.Blocks.Clear();
    }

    private void BtnCloseOutput_Click(object sender, RoutedEventArgs e)
    {
        OutputPanel.Visibility = Visibility.Collapsed;
    }
}
