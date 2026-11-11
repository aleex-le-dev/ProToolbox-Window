using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ShapePath = System.Windows.Shapes.Path;
using System.Windows.Threading;
using ScriptsWindows.Core;
using ScriptsWindows.Services;

namespace ScriptsWindows.Views.Controls;

/// <summary>
/// Banc d'essai visuel (WinSAT) : jauge de la note globale, barres par composant avec repère de comparaison,
/// mesures réelles comparées (profils types ou évaluation précédente) et historique.
/// </summary>
public partial class BenchmarkDialog : Window
{
    private static readonly CultureInfo Fr = CultureInfo.GetCultureInfo("fr-FR");
    private const double MaxScore = 9.9;

    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;
    private List<BenchmarkResult> _history = new();
    private BenchmarkResult? _current;
    private BenchmarkHardware _hw = new();
    private CancellationTokenSource? _runCts;
    private DispatcherTimer? _timer;
    private DateTime _runStart;

    // Composants notés : libellé, note, matériel associé
    private static readonly (string Label, Func<BenchmarkResult, double> Score, Func<BenchmarkHardware, string> Hw)[] Components =
    {
        ("Processeur",    r => r.CpuScore,      h => h.Cpu),
        ("Mémoire vive",  r => r.MemoryScore,   h => h.Ram),
        ("Graphismes",    r => r.GraphicsScore, h => h.Gpu),
        ("Graphismes 3D", r => r.GamingScore,   h => "Jeux et applications 3D"),
        ("Disque",        r => r.DiskScore,     h => h.Disk),
    };

    // Mesures brutes : libellé, explication, valeur, conversion d'affichage
    private static readonly (string Label, string Hint, Func<BenchmarkResult, double> Value, Func<double, string> Format)[] Metrics =
    {
        ("Processeur (multi-cœur)",  "Compression de données, tous les cœurs",   r => r.CpuMultiMBs,     v => $"{v.ToString("N0", Fr)} Mo/s"),
        ("Processeur (mono-cœur)",   "Réactivité d'une application seule",       r => r.CpuSingleMBs,    v => $"{v.ToString("N0", Fr)} Mo/s"),
        ("Mémoire vive",             "Bande passante",                           r => r.MemBandwidthMBs, v => $"{(v / 1000).ToString("N1", Fr)} Go/s"),
        ("Disque — lecture séquentielle", "Copie de gros fichiers",              r => r.DiskSeqReadMBs,  v => $"{v.ToString("N0", Fr)} Mo/s"),
        ("Disque — lecture aléatoire",    "Démarrage de Windows et des logiciels", r => r.DiskRandReadMBs, v => $"{v.ToString(v < 10 ? "N1" : "N0", Fr)} Mo/s"),
        ("Graphismes du bureau",     "Fluidité des fenêtres et animations",      r => r.DesktopFps,      v => $"{v.ToString("N0", Fr)} images/s"),
    };

    public BenchmarkDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput ?? (_ => { });
        _ct = ct;
        Loaded += async (_, _) => await LoadAsync();
        Closing += (_, e) =>
        {
            if (_runCts == null) return;
            if (MessageBox.Show(this, "L'évaluation est en cours. L'arrêter et fermer ?", "Évaluation en cours",
                                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) e.Cancel = true;
            else _runCts.Cancel();
        };
    }

    private static Brush B(string key) => ThemeManager.GetBrush(key);
    private static string S(double v) => v.ToString("0.0", Fr);

    // ─────────────────────────────────────────────── Chargement

    private async Task LoadAsync(BenchmarkResult? select = null)
    {
        TxtStatus.Text = "Lecture des résultats…";
        _hw = await BenchmarkService.GetHardwareAsync(_ct);
        PanelBattery.Visibility = _hw.OnBattery ? Visibility.Visible : Visibility.Collapsed;

        _history = BenchmarkService.LoadHistory();
        _current = (select == null ? null : _history.FirstOrDefault(h => Math.Abs((h.Date - select.Date).TotalSeconds) < 1)) ?? _history.FirstOrDefault();

        // Comparatifs : profils types + évaluations précédentes de ce PC
        var choices = BenchmarkService.References.Concat(_history.Where(h => h != _current)).ToList();
        CmbCompare.ItemsSource = choices;
        CmbCompare.SelectedItem = _history.Count > 1 ? _history[1] : BenchmarkService.References[2];

        if (_current == null)
        {
            TxtStatus.Text = "Aucune évaluation sur ce PC : lancez-en une (2 à 5 minutes).";
            TxtVerdict.Text = "Non évalué";
            TxtVerdictDetail.Text = "Cliquez sur « Lancer l'évaluation » pour mesurer les performances.";
            DrawGauge(0, "TextMuted");
            BuildHardware();
            return;
        }
        Render();
        TxtStatus.Text = $"{_history.Count} évaluation(s) enregistrée(s).";
    }

    private void CmbCompare_SelectionChanged(object sender, SelectionChangedEventArgs e) { if (_current != null) Render(); }

    // ─────────────────────────────────────────────── Rendu

    private void Render()
    {
        if (_current == null) return;
        var cmp = CmbCompare.SelectedItem as BenchmarkResult;
        var r = _current;

        var (title, detail, brushKey) = BenchmarkService.Verdict(r.SystemScore);
        TxtGlobalScore.Text = S(r.SystemScore);
        TxtGlobalScore.Foreground = B(brushKey);
        TxtVerdict.Text = title;
        TxtVerdict.Foreground = B(brushKey);
        TxtVerdictDetail.Text = detail;
        DrawGauge(r.SystemScore, brushKey);

        var weakest = Components.OrderBy(c => c.Score(r)).First();
        TxtBottleneck.Text = $"Composant limitant : {weakest.Label} ({S(weakest.Score(r))})";
        TxtAdvice.Text = BenchmarkService.Advice(weakest.Label, weakest.Score(r));
        TxtDate.Text = $"Évaluation du {r.Date:dddd d MMMM yyyy à HH:mm}";
        BuildHardware();

        // Notes par composant
        PanelScores.Children.Clear();
        foreach (var c in Components)
        {
            double v = c.Score(r);
            double? refV = cmp != null ? c.Score(cmp) : null;
            PanelScores.Children.Add(ScoreRow(c.Label, c.Hw(_hw), v, refV, c.Label == weakest.Label));
        }

        // Mesures
        TxtMetricsLegend.Text = cmp == null ? "" : $"Barre bleue : ce PC • barre grise : {cmp.DisplayName}";
        PanelMetrics.Children.Clear();
        foreach (var m in Metrics)
        {
            double v = m.Value(r);
            if (v <= 0) continue;
            PanelMetrics.Children.Add(MetricRow(m.Label, m.Hint, v, cmp != null ? m.Value(cmp) : 0, m.Format));
        }

        BuildHistory();
    }

    private void BuildHardware()
    {
        PanelHardware.Children.Clear();
        foreach (var (label, value) in new[] { ("Processeur", _hw.Cpu), ("Graphismes", _hw.Gpu), ("Mémoire", _hw.Ram), ("Disque", _hw.Disk) })
        {
            if (string.IsNullOrWhiteSpace(value)) continue;
            var row = new DockPanel { Margin = new Thickness(0, 2, 0, 2) };
            var l = new TextBlock { Text = label, Width = 80, FontSize = 11.5, Foreground = B("TextMuted") };
            DockPanel.SetDock(l, Dock.Left);
            row.Children.Add(l);
            row.Children.Add(new TextBlock { Text = value, FontSize = 11.5, Foreground = B("TextSecondary"), TextWrapping = TextWrapping.Wrap });
            PanelHardware.Children.Add(row);
        }
    }

    /// <summary>Ligne : libellé + matériel | barre (note sur 9,9) avec repère de comparaison | note + écart.</summary>
    private UIElement ScoreRow(string label, string hw, double value, double? refValue, bool weakest)
    {
        var grid = new Grid { Margin = new Thickness(0, 0, 0, 12) };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(190) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(96) });

        var left = new StackPanel { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 12, 0) };
        var title = new StackPanel { Orientation = Orientation.Horizontal };
        title.Children.Add(new TextBlock { Text = label, FontWeight = FontWeights.SemiBold, FontSize = 13 });
        if (weakest)
            title.Children.Add(new Border
            {
                Background = B("WarningBg"), CornerRadius = new CornerRadius(4), Padding = new Thickness(5, 0, 5, 1), Margin = new Thickness(8, 0, 0, 0),
                Child = new TextBlock { Text = "limitant", FontSize = 10.5, Foreground = B("WarningText") }
            });
        left.Children.Add(title);
        left.Children.Add(new TextBlock { Text = hw, FontSize = 11, Foreground = B("TextMuted"), TextTrimming = TextTrimming.CharacterEllipsis });
        grid.Children.Add(left);

        var bar = Bar(value, MaxScore, weakest ? "Warning" : "Accent", 12, refValue);
        Grid.SetColumn(bar, 1);
        grid.Children.Add(bar);

        var right = new StackPanel { HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center };
        right.Children.Add(new TextBlock { Text = S(value), FontSize = 16, FontWeight = FontWeights.SemiBold, HorizontalAlignment = HorizontalAlignment.Right });
        if (refValue is double rv && rv > 0)
        {
            double d = Math.Round(value - rv, 1);
            right.Children.Add(new TextBlock
            {
                Text = d == 0 ? "= comparatif" : (d > 0 ? "+" : "−") + S(Math.Abs(d)),
                FontSize = 11, HorizontalAlignment = HorizontalAlignment.Right,
                Foreground = B(d > 0 ? "SuccessText" : d < 0 ? "DangerText" : "TextMuted")
            });
        }
        Grid.SetColumn(right, 2);
        grid.Children.Add(right);
        return grid;
    }

    /// <summary>Mesure brute : deux barres (ce PC / comparatif) à la même échelle + écart en %.</summary>
    private UIElement MetricRow(string label, string hint, double value, double refValue, Func<double, string> format)
    {
        var grid = new Grid { Margin = new Thickness(0, 0, 0, 14) };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(190) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(96) });

        var left = new StackPanel { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 12, 0) };
        left.Children.Add(new TextBlock { Text = label, FontWeight = FontWeights.SemiBold, FontSize = 12.5 });
        left.Children.Add(new TextBlock { Text = hint, FontSize = 11, Foreground = B("TextMuted") });
        grid.Children.Add(left);

        double max = Math.Max(value, refValue) * 1.05;
        var bars = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
        bars.Children.Add(BarWithLabel(value, max, "Accent", format(value), "TextPrimary"));
        if (refValue > 0) bars.Children.Add(BarWithLabel(refValue, max, "BorderStrong", format(refValue), "TextMuted"));
        Grid.SetColumn(bars, 1);
        grid.Children.Add(bars);

        if (refValue > 0)
        {
            double ratio = value / refValue;
            string text = ratio >= 2 ? $"×{ratio.ToString("0.0", Fr)}"
                        : ratio <= 0.5 ? $"÷{(1 / ratio).ToString("0.0", Fr)}"
                        : $"{(ratio >= 1 ? "+" : "−")}{Math.Abs((ratio - 1) * 100):0} %";
            var delta = new TextBlock
            {
                Text = Math.Abs(ratio - 1) < 0.03 ? "équivalent" : text, FontSize = 15, FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center,
                Foreground = B(Math.Abs(ratio - 1) < 0.03 ? "TextMuted" : ratio > 1 ? "SuccessText" : "DangerText")
            };
            Grid.SetColumn(delta, 2);
            grid.Children.Add(delta);
        }
        return grid;
    }

    private UIElement BarWithLabel(double value, double max, string brush, string text, string textBrush)
    {
        var g = new Grid { Margin = new Thickness(0, 1, 0, 1) };
        g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(110) });
        g.Children.Add(Bar(value, max, brush, 8, null));
        var t = new TextBlock { Text = text, FontSize = 11, Foreground = B(textBrush), Margin = new Thickness(10, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
        Grid.SetColumn(t, 1);
        g.Children.Add(t);
        return g;
    }

    /// <summary>Barre horizontale proportionnelle (colonnes en étoiles : aucune mesure de taille nécessaire).</summary>
    private static Grid Bar(double value, double max, string brush, double height, double? marker)
    {
        double ratio = max > 0 ? Math.Clamp(value / max, 0, 1) : 0;
        var root = new Grid { Height = Math.Max(height, 16), VerticalAlignment = VerticalAlignment.Center };

        var track = new Grid { Height = height, VerticalAlignment = VerticalAlignment.Center };
        track.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(Math.Max(ratio, 0.0001), GridUnitType.Star) });
        track.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(Math.Max(1 - ratio, 0.0001), GridUnitType.Star) });
        var bg = new Border { Background = B("BgInput"), CornerRadius = new CornerRadius(height / 2) };
        Grid.SetColumnSpan(bg, 2);
        track.Children.Add(bg);
        track.Children.Add(new Border { Background = B(brush), CornerRadius = new CornerRadius(height / 2) });
        root.Children.Add(track);

        if (marker is double m && m > 0 && max > 0)
        {
            double mr = Math.Clamp(m / max, 0, 1);
            var layer = new Grid();
            layer.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(Math.Max(mr, 0.0001), GridUnitType.Star) });
            layer.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(Math.Max(1 - mr, 0.0001), GridUnitType.Star) });
            layer.Children.Add(new Border { Width = 3, Height = height + 8, Background = B("WarningText"), HorizontalAlignment = HorizontalAlignment.Right, CornerRadius = new CornerRadius(1.5) });
            root.Children.Add(layer);
        }
        return root;
    }

    /// <summary>Jauge circulaire sur 270° (de 1,0 à 9,9).</summary>
    private void DrawGauge(double score, string brushKey)
    {
        GaugeCanvas.Children.Clear();
        const double cx = 100, cy = 95, radius = 78, thickness = 14, start = 135, sweep = 270;

        ShapePath Arc(double from, double angle, Brush stroke)
        {
            Point P(double deg) => new(cx + radius * Math.Cos(deg * Math.PI / 180), cy + radius * Math.Sin(deg * Math.PI / 180));
            var fig = new PathFigure { StartPoint = P(from), IsClosed = false };
            fig.Segments.Add(new ArcSegment(P(from + angle), new Size(radius, radius), 0, angle > 180, SweepDirection.Clockwise, true));
            return new ShapePath
            {
                Data = new PathGeometry(new[] { fig }), Stroke = stroke, StrokeThickness = thickness,
                StrokeStartLineCap = PenLineCap.Round, StrokeEndLineCap = PenLineCap.Round
            };
        }

        GaugeCanvas.Children.Add(Arc(start, sweep, B("BgInput")));
        double ratio = Math.Clamp((score - 1) / (MaxScore - 1), 0, 1);
        if (ratio > 0.005) GaugeCanvas.Children.Add(Arc(start, Math.Min(sweep * ratio, 269.9), B(brushKey)));

        // Graduations 1 · 5 · 9,9
        foreach (var (val, txt) in new[] { (1.0, "1"), (5.45, "5"), (MaxScore, "9,9") })
        {
            double deg = start + sweep * (val - 1) / (MaxScore - 1);
            double r2 = radius + 18;
            var t = new TextBlock { Text = txt, FontSize = 10, Foreground = B("TextMuted") };
            Canvas.SetLeft(t, cx + r2 * Math.Cos(deg * Math.PI / 180) - 8);
            Canvas.SetTop(t, cy + r2 * Math.Sin(deg * Math.PI / 180) - 7);
            GaugeCanvas.Children.Add(t);
        }
    }

    private void BuildHistory()
    {
        PanelHistory.Children.Clear();
        TxtNoHistory.Visibility = _history.Count <= 1 ? Visibility.Visible : Visibility.Collapsed;
        if (_history.Count == 0) return;

        // En-tête des colonnes
        var head = HistoryGrid();
        string[] cols = { "Date", "Note globale", "Processeur", "Mémoire", "Graphismes", "3D", "Disque", "" };
        for (int i = 0; i < cols.Length; i++) head.Children.Add(Cell(cols[i], i, "TextMuted", 11, FontWeights.SemiBold));
        PanelHistory.Children.Add(head);

        for (int i = 0; i < _history.Count; i++)
        {
            var h = _history[i];
            var prev = i + 1 < _history.Count ? _history[i + 1] : null;
            var row = HistoryGrid();
            row.Background = h == _current ? B("AccentDim") : Brushes.Transparent;

            row.Children.Add(Cell(h.Date.ToString("dd/MM/yyyy HH:mm", Fr), 0, "TextPrimary", 12, FontWeights.Normal));

            // Note globale : mini barre + valeur + évolution
            var global = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
            var mini = Bar(h.SystemScore, MaxScore, "Accent", 6, null);
            mini.Width = 70;
            global.Children.Add(mini);
            global.Children.Add(new TextBlock { Text = S(h.SystemScore), FontWeight = FontWeights.SemiBold, FontSize = 12.5, Margin = new Thickness(8, 0, 6, 0), VerticalAlignment = VerticalAlignment.Center });
            if (prev != null && Math.Abs(h.SystemScore - prev.SystemScore) >= 0.05)
            {
                double d = h.SystemScore - prev.SystemScore;
                global.Children.Add(new TextBlock
                {
                    Text = (d > 0 ? "▲ " : "▼ ") + S(Math.Abs(d)), FontSize = 11, VerticalAlignment = VerticalAlignment.Center,
                    Foreground = B(d > 0 ? "SuccessText" : "DangerText")
                });
            }
            Grid.SetColumn(global, 1);
            row.Children.Add(global);

            double[] subs = { h.CpuScore, h.MemoryScore, h.GraphicsScore, h.GamingScore, h.DiskScore };
            double[]? prevSubs = prev == null ? null : new[] { prev.CpuScore, prev.MemoryScore, prev.GraphicsScore, prev.GamingScore, prev.DiskScore };
            for (int k = 0; k < subs.Length; k++)
            {
                string brush = prevSubs == null || Math.Abs(subs[k] - prevSubs[k]) < 0.05 ? "TextSecondary" : subs[k] > prevSubs[k] ? "SuccessText" : "DangerText";
                row.Children.Add(Cell(S(subs[k]), 2 + k, brush, 12, FontWeights.Normal));
            }

            if (h != _current)
            {
                var actions = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
                var show = new Button { Content = "Afficher", Style = (Style)FindResource("ActionButton"), FontSize = 11, Padding = new Thickness(8, 2, 8, 2), MinHeight = 24, Margin = new Thickness(0, 0, 6, 0), Tag = h };
                show.Click += async (s, _) => await LoadAsync((BenchmarkResult)((Button)s).Tag);
                var cmp = new Button { Content = "Comparer", Style = (Style)FindResource("ActionButton"), FontSize = 11, Padding = new Thickness(8, 2, 8, 2), MinHeight = 24, Tag = h };
                cmp.Click += (s, _) => CmbCompare.SelectedItem = ((Button)s).Tag;
                actions.Children.Add(show);
                actions.Children.Add(cmp);
                Grid.SetColumn(actions, 7);
                row.Children.Add(actions);
            }
            PanelHistory.Children.Add(row);
        }
    }

    private static Grid HistoryGrid()
    {
        var g = new Grid { Margin = new Thickness(0, 0, 0, 2) };
        foreach (var w in new[] { 130.0, 190, 80, 80, 80, 60, 70 }) g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(w) });
        g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        return g;
    }

    private static TextBlock Cell(string text, int col, string brush, double size, FontWeight weight)
    {
        var t = new TextBlock { Text = text, FontSize = size, FontWeight = weight, Foreground = B(brush), VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(4, 6, 4, 6) };
        Grid.SetColumn(t, col);
        return t;
    }

    // ─────────────────────────────────────────────── Évaluation

    private async void BtnRun_Click(object sender, RoutedEventArgs e)
    {
        if (_runCts != null) { _runCts.Cancel(); return; }

        if (_hw.OnBattery && MessageBox.Show(this, "Le PC est sur batterie : les notes seront sous-estimées.\nBranchez le chargeur pour un résultat fiable.\n\nLancer quand même ?",
                                             "Sur batterie", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

        _runCts = CancellationTokenSource.CreateLinkedTokenSource(_ct);
        PanelProgress.Visibility = Visibility.Visible;
        TxtRunLabel.Text = "Arrêter";
        CmbCompare.IsEnabled = BtnClose.IsEnabled = false;
        ProgRun.Value = 0;
        _runStart = DateTime.Now;
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += (_, _) => TxtElapsed.Text = (DateTime.Now - _runStart).ToString(@"m\:ss");
        _timer.Start();
        _onOutput("\n🚀 [LANCEMENT] Évaluation des performances (winsat formal)");

        try
        {
            var result = await BenchmarkService.RunAsync(
                (p, label) => Dispatcher.Invoke(() => { ProgRun.Value = p; TxtStep.Text = $"{label}…"; }),
                _onOutput, _runCts.Token);

            if (result == null)
            {
                TxtStatus.Text = "L'évaluation n'a pas produit de résultat : consultez la console (droits administrateur, batterie…).";
            }
            else
            {
                var previous = _history.FirstOrDefault();
                await LoadAsync(result);
                if (previous != null) CmbCompare.SelectedItem = CmbCompare.Items.Cast<BenchmarkResult>().FirstOrDefault(c => c.Date == previous.Date && !c.IsReference) ?? CmbCompare.SelectedItem;
                TxtStatus.Text = $"Évaluation terminée en {(DateTime.Now - _runStart):m\\:ss} : note globale {S(result.SystemScore)}.";
                _onOutput($"[OK] Note globale {S(result.SystemScore)} — Processeur {S(result.CpuScore)}, Mémoire {S(result.MemoryScore)}, Graphismes {S(result.GraphicsScore)}, 3D {S(result.GamingScore)}, Disque {S(result.DiskScore)}");
            }
        }
        catch (OperationCanceledException) { TxtStatus.Text = "Évaluation arrêtée."; }
        catch (Exception ex) { TxtStatus.Text = $"Erreur : {ex.Message}"; _onOutput($"[ERR] {ex.Message}"); }
        finally
        {
            _timer?.Stop();
            _runCts?.Dispose();
            _runCts = null;
            PanelProgress.Visibility = Visibility.Collapsed;
            TxtRunLabel.Text = "Lancer l'évaluation";
            CmbCompare.IsEnabled = BtnClose.IsEnabled = true;
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
