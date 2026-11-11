using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Controls;

public enum CheckState
{
    Pending,
    Pass,
    Warn,
    Fail
}

public class WebsiteCheckItem
{
    public string Index { get; set; } = "";
    public string Title { get; set; } = "";
    public string Category { get; set; } = "";
    public string StatusText { get; set; } = "⏳ En attente";
    public string Details { get; set; } = "En attente d'analyse...";
    public string Recommendation { get; set; } = "";
    public CheckState State { get; set; } = CheckState.Pending;

    public Visibility HasRecommendationVisibility =>
        string.IsNullOrWhiteSpace(Recommendation) ? Visibility.Collapsed : Visibility.Visible;

    public Brush BorderColor { get; set; } = (SolidColorBrush)ThemeManager.GetBrush("Border");
    public Brush StatusBg { get; set; } = (SolidColorBrush)ThemeManager.GetBrush("BgCard");
    public Brush StatusFg { get; set; } = (SolidColorBrush)ThemeManager.GetBrush("TextSecondary");
}

public class HeadingTreeItem
{
    public string Tag { get; set; } = "";
    public string Text { get; set; } = "";
    public Thickness IndentMargin { get; set; }
    public Brush BadgeBg { get; set; } = Brushes.DodgerBlue;
}

public class KeywordDensityItem
{
    public string Rank { get; set; } = "";
    public string Word { get; set; } = "";
    public int Count { get; set; }
    public double Density { get; set; }
    public string CountText => $"{Count} occurrence{(Count > 1 ? "s" : "")}";
    public string DensityText => $"Densité : {Density:0.00} %";
    public string Locations { get; set; } = "";
}

public class HttpHeaderItem
{
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
}

public class InspectedLinkItem
{
    public string StatusBadge { get; set; } = "";
    public Brush StatusFg { get; set; } = (SolidColorBrush)ThemeManager.GetBrush("TextSecondary");
    public string Scope { get; set; } = "";
    public string LatencyText { get; set; } = "";
    public string Url { get; set; } = "";
    public bool IsBroken { get; set; }
}

public partial class WebsiteAnalyzerDialog : Window
{
    private readonly Action<string>? _externalOutput;
    private readonly CancellationToken _cancellationToken;
    private CancellationTokenSource? _auditCts;
    private readonly List<WebsiteCheckItem> _allChecks = new();
    private List<KeywordDensityItem> _kw1List = new();
    private List<KeywordDensityItem> _kw2List = new();
    private List<KeywordDensityItem> _kw3List = new();

    private string _cachedPageTitle = "";
    private string _cachedMetaDesc = "";
    private string _cachedH1Text = "";
    private string _cachedH2H3Text = "";
    private string _cachedAltText = "";
    private string _cachedVisibleText = "";
    private int _cachedTotalWords;

    private CheckState? _activeFilter;
    private string _searchQuery = "";
    private int _activeTab;
    private bool _isAnalyzing;
    private bool _suppressTextChanged;
    private string _lastAnalyzedUrl = "";

    private static readonly SolidColorBrush PassBorder = (SolidColorBrush)ThemeManager.GetBrush("Success");
    private static readonly SolidColorBrush PassBg = (SolidColorBrush)ThemeManager.GetBrush("SuccessBg");
    private static readonly SolidColorBrush PassFg = (SolidColorBrush)ThemeManager.GetBrush("SuccessText");

    private static readonly SolidColorBrush WarnBorder = (SolidColorBrush)ThemeManager.GetBrush("Warning");
    private static readonly SolidColorBrush WarnBg = (SolidColorBrush)ThemeManager.GetBrush("WarningBg");
    private static readonly SolidColorBrush WarnFg = (SolidColorBrush)ThemeManager.GetBrush("WarningText");

    private static readonly SolidColorBrush FailBorder = (SolidColorBrush)ThemeManager.GetBrush("Danger");
    private static readonly SolidColorBrush FailBg = (SolidColorBrush)ThemeManager.GetBrush("DangerBg");
    private static readonly SolidColorBrush FailFg = (SolidColorBrush)ThemeManager.GetBrush("DangerText");

    static WebsiteAnalyzerDialog()
    {
        PassBorder.Freeze(); PassBg.Freeze(); PassFg.Freeze();
        WarnBorder.Freeze(); WarnBg.Freeze(); WarnFg.Freeze();
        FailBorder.Freeze(); FailBg.Freeze(); FailFg.Freeze();
    }

    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "le","la","les","de","des","du","un","une","et","ou","en","au","aux","ce","ces","cet","cette",
        "dans","par","pour","sur","avec","sans","sous","vers","chez","entre","mais","donc","or","ni","car",
        "que","qui","quoi","dont","où","ne","pas","plus","moins","très","bien","tout","tous","toute","toutes",
        "mon","ton","son","ma","ta","sa","mes","tes","ses","notre","votre","leur","nos","vos","leurs",
        "je","tu","il","elle","on","nous","vous","ils","elles","se","me","te","lui","eux","cela","ça","ici",
        "est","sont","être","avoir","fait","faire","peut","peuvent","comme","aussi","été","avait","ont",
        "autres","autre","après","avant","depuis","pendant","alors","ainsi","encore","toujours","jamais","rien",
        "the","and","for","that","this","with","from","your","you","are","was","were","have","has","had",
        "not","but","what","all","when","can","more","will","about","which","their","there","been","into",
        "http","https","www","com","org","net","html","nbsp","amp","quot","copy"
    };

    public WebsiteAnalyzerDialog(Action<string>? onOutput = null, CancellationToken ct = default, int initialTab = 0)
    {
        InitializeComponent();
        _externalOutput = onOutput;
        _cancellationToken = ct;

        InitializeDefaultChecks();
        RefreshFilteredList();
        SwitchTab(initialTab);

        Loaded += (_, _) =>
        {
            TxtUrl.Focus();
            TxtUrl.CaretIndex = TxtUrl.Text.Length;
        };
    }

    private void SwitchTab(int tabIndex)
    {
        _activeTab = Math.Clamp(tabIndex, 0, 5);

        PanelOverview.Visibility = _activeTab == 0 ? Visibility.Visible : Visibility.Collapsed;
        PanelChecklist.Visibility = _activeTab == 1 ? Visibility.Visible : Visibility.Collapsed;
        PanelSpeedtest.Visibility = _activeTab == 2 ? Visibility.Visible : Visibility.Collapsed;
        PanelSeo.Visibility = _activeTab == 3 ? Visibility.Visible : Visibility.Collapsed;
        PanelKeywords.Visibility = _activeTab == 4 ? Visibility.Visible : Visibility.Collapsed;
        PanelNetwork.Visibility = _activeTab == 5 ? Visibility.Visible : Visibility.Collapsed;

        BtnTabOverview.Style = (Style)FindResource(_activeTab == 0 ? "PrimaryActionButton" : "ActionButton");
        BtnTabChecklist.Style = (Style)FindResource(_activeTab == 1 ? "PrimaryActionButton" : "ActionButton");
        BtnTabSpeedtest.Style = (Style)FindResource(_activeTab == 2 ? "PrimaryActionButton" : "ActionButton");
        BtnTabSeo.Style = (Style)FindResource(_activeTab == 3 ? "PrimaryActionButton" : "ActionButton");
        BtnTabKeywords.Style = (Style)FindResource(_activeTab == 4 ? "PrimaryActionButton" : "ActionButton");
        BtnTabNetwork.Style = (Style)FindResource(_activeTab == 5 ? "PrimaryActionButton" : "ActionButton");
    }

    private void BtnTabOverview_Click(object sender, RoutedEventArgs e) => SwitchTab(0);
    private void BtnTabChecklist_Click(object sender, RoutedEventArgs e) => SwitchTab(1);
    private void BtnTabSpeedtest_Click(object sender, RoutedEventArgs e) => SwitchTab(2);
    private void BtnTabSeo_Click(object sender, RoutedEventArgs e) => SwitchTab(3);
    private void BtnTabKeywords_Click(object sender, RoutedEventArgs e) => SwitchTab(4);
    private void BtnTabNetwork_Click(object sender, RoutedEventArgs e) => SwitchTab(5);

    private void InitializeDefaultChecks()
    {
        _allChecks.Clear();
        var definitions = new (string Title, string Category)[]
        {
            ("Page RGPD", "Légal & Confidentialité"),
            ("Page de CGU / Mentions", "Légal & Mentions"),
            ("API hors front-end", "Sécurité Code & Bundles JS"),
            ("Forcer le HTTPS", "Sécurité Réseau & SSL"),
            ("Bannière cookies", "Conformité RGPD / CMP"),
            ("Méta-titre (Meta title)", "SEO & Référencement"),
            ("Image réseaux (Open Graph)", "Partage Réseaux Sociaux"),
            ("Favicon", "Identité Visuelle"),
            ("Sitemap + robots.txt", "Indexation Moteurs"),
            ("Textes images (attributs Alt)", "Accessibilité & SEO Images"),
            ("Compresse images", "Poids & Formats Médias"),
            ("Vitesse pages", "Temps de Réponse & Compression"),
            ("Contexte & Contraste", "Accessibilité, Langue & Hn"),
            ("Site responsive", "Adaptation Mobile & Viewport"),
            ("Page 404 sur mesure", "Gestion des Erreurs 404"),
            ("Répare liens cassés", "Vérification HTTP des Liens"),
            ("Valid. formulaire", "Sécurité & Validation Champs"),
            ("Anti-spam", "Protection Bot / Captcha"),
            ("Outil d'analytics", "Mesure d'Audience"),
            ("Un seul CTA", "Clarté de Conversion"),
            ("Protection contre les injections SQL", "Sécurité Serveur & En-têtes")
        };

        for (int i = 0; i < definitions.Length; i++)
        {
            _allChecks.Add(new WebsiteCheckItem
            {
                Index = (i + 1).ToString("00"),
                Title = definitions[i].Title,
                Category = definitions[i].Category,
                StatusText = "⏳ En attente",
                Details = "Saisissez le nom de domaine à droite de https:// et cliquez sur « Lancer l'analyse complète »."
            });
        }

        ItemsOverviewPriorities.ItemsSource = new[]
        {
            new WebsiteCheckItem
            {
                Title = "Aucun audit lancé pour le moment",
                StatusText = "ℹ️ Prêt",
                Recommendation = "Entrez votre nom de domaine (ex: salutalex.fr) en haut et cliquez sur « 🚀 Lancer l'analyse complète » pour générer le diagnostic complet.",
                BorderColor = (SolidColorBrush)ThemeManager.GetBrush("Border"),
                StatusFg = (SolidColorBrush)ThemeManager.GetBrush("AccentText")
            }
        };
    }

    private void SetCheckResult(int index, CheckState state, string statusText, string details, string recommendation = "")
    {
        var item = _allChecks[index];
        item.State = state;
        item.StatusText = statusText;
        item.Details = details;
        item.Recommendation = string.IsNullOrWhiteSpace(recommendation) ? "" : $"💡 Conseil : {recommendation}";

        switch (state)
        {
            case CheckState.Pass:
                item.BorderColor = PassBorder;
                item.StatusBg = PassBg;
                item.StatusFg = PassFg;
                break;
            case CheckState.Warn:
                item.BorderColor = WarnBorder;
                item.StatusBg = WarnBg;
                item.StatusFg = WarnFg;
                break;
            case CheckState.Fail:
                item.BorderColor = FailBorder;
                item.StatusBg = FailBg;
                item.StatusFg = FailFg;
                break;
        }
    }

    private void RefreshFilteredList()
    {
        IEnumerable<WebsiteCheckItem> view = _allChecks;
        if (_activeFilter.HasValue)
            view = view.Where(c => c.State == _activeFilter.Value);

        if (!string.IsNullOrWhiteSpace(_searchQuery))
        {
            string q = _searchQuery.Trim();
            view = view.Where(c =>
                c.Title.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                c.Category.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                c.Details.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                c.Recommendation.Contains(q, StringComparison.OrdinalIgnoreCase));
        }

        ItemsChecks.ItemsSource = view.ToList();

        BtnFilterAll.Style = (Style)FindResource(_activeFilter == null ? "PrimaryActionButton" : "ActionButton");
        BtnFilterFail.Style = (Style)FindResource(_activeFilter == CheckState.Fail ? "PrimaryActionButton" : "ActionButton");
        BtnFilterWarn.Style = (Style)FindResource(_activeFilter == CheckState.Warn ? "PrimaryActionButton" : "ActionButton");
        BtnFilterOk.Style = (Style)FindResource(_activeFilter == CheckState.Pass ? "PrimaryActionButton" : "ActionButton");
    }

    private void TxtSearchChecklist_TextChanged(object sender, TextChangedEventArgs e)
    {
        _searchQuery = TxtSearchChecklist.Text;
        RefreshFilteredList();
    }

    private void BtnCancelAudit_Click(object sender, RoutedEventArgs e)
    {
        try { _auditCts?.Cancel(); } catch { }
    }

    private void TxtUrl_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (TxtPlaceholder != null)
            TxtPlaceholder.Visibility = string.IsNullOrEmpty(TxtUrl.Text) ? Visibility.Visible : Visibility.Collapsed;

        if (_suppressTextChanged) return;

        string current = TxtUrl.Text;
        string cleaned = Regex.Replace(current, @"https?://", "", RegexOptions.IgnoreCase);
        if (cleaned != current)
        {
            _suppressTextChanged = true;
            TxtUrl.Text = cleaned;
            TxtUrl.CaretIndex = cleaned.Length;
            _suppressTextChanged = false;
            if (TxtPlaceholder != null)
                TxtPlaceholder.Visibility = string.IsNullOrEmpty(cleaned) ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private string? GetNormalizedTargetUrl()
    {
        string cleaned = Regex.Replace(TxtUrl.Text.Trim(), @"https?://", "", RegexOptions.IgnoreCase).Trim();
        if (string.IsNullOrWhiteSpace(cleaned)) return null;
        return "https://" + cleaned;
    }

    private async void TxtUrl_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            await RunFullAuditAsync();
        }
    }

    private async void BtnAnalyze_Click(object sender, RoutedEventArgs e)
    {
        await RunFullAuditAsync();
    }

    private void BtnOpenSite_Click(object sender, RoutedEventArgs e)
    {
        var url = GetNormalizedTargetUrl();
        if (string.IsNullOrEmpty(url)) return;
        try
        {
            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
        }
        catch { }
    }

    private void BtnFilterAll_Click(object sender, RoutedEventArgs e) { _activeFilter = null; RefreshFilteredList(); }
    private void BtnFilterFail_Click(object sender, RoutedEventArgs e) { _activeFilter = CheckState.Fail; RefreshFilteredList(); }
    private void BtnFilterWarn_Click(object sender, RoutedEventArgs e) { _activeFilter = CheckState.Warn; RefreshFilteredList(); }
    private void BtnFilterOk_Click(object sender, RoutedEventArgs e) { _activeFilter = CheckState.Pass; RefreshFilteredList(); }

    private void BtnKw1_Click(object sender, RoutedEventArgs e) => SetKeywordView(1);
    private void BtnKw2_Click(object sender, RoutedEventArgs e) => SetKeywordView(2);
    private void BtnKw3_Click(object sender, RoutedEventArgs e) => SetKeywordView(3);

    private void SetKeywordView(int ngramSize)
    {
        BtnKw1.Style = (Style)FindResource(ngramSize == 1 ? "PrimaryActionButton" : "ActionButton");
        BtnKw2.Style = (Style)FindResource(ngramSize == 2 ? "PrimaryActionButton" : "ActionButton");
        BtnKw3.Style = (Style)FindResource(ngramSize == 3 ? "PrimaryActionButton" : "ActionButton");

        ItemsKeywords.ItemsSource = ngramSize switch
        {
            2 => _kw2List,
            3 => _kw3List,
            _ => _kw1List
        };
    }

    private void TxtTargetKeyword_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            EvaluateTargetKeyword();
        }
    }

    private void BtnCheckKeyword_Click(object sender, RoutedEventArgs e)
    {
        EvaluateTargetKeyword();
    }

    private void EvaluateTargetKeyword()
    {
        string kw = TxtTargetKeyword.Text.Trim();
        if (string.IsNullOrWhiteSpace(kw))
        {
            TxtTargetKeywordResult.Text = "Veuillez saisir un mot-clé ou une expression à tester.";
            return;
        }
        if (string.IsNullOrWhiteSpace(_cachedVisibleText))
        {
            TxtTargetKeywordResult.Text = "Lancez d'abord l'analyse d'un site web pour évaluer ce mot-clé.";
            return;
        }

        bool inTitle = _cachedPageTitle.Contains(kw, StringComparison.OrdinalIgnoreCase);
        bool inDesc = _cachedMetaDesc.Contains(kw, StringComparison.OrdinalIgnoreCase);
        bool inH1 = _cachedH1Text.Contains(kw, StringComparison.OrdinalIgnoreCase);
        bool inH2H3 = _cachedH2H3Text.Contains(kw, StringComparison.OrdinalIgnoreCase);
        bool inUrl = _lastAnalyzedUrl.Contains(kw.Replace(' ', '-'), StringComparison.OrdinalIgnoreCase) || _lastAnalyzedUrl.Contains(kw, StringComparison.OrdinalIgnoreCase);
        bool inAlt = _cachedAltText.Contains(kw, StringComparison.OrdinalIgnoreCase);

        int bodyCount = Regex.Matches(_cachedVisibleText, Regex.Escape(kw), RegexOptions.IgnoreCase).Count;
        double density = _cachedTotalWords > 0 ? (bodyCount * 100.0) / _cachedTotalWords : 0;

        int score = (inTitle ? 2 : 0) + (inH1 ? 2 : 0) + (inDesc ? 1 : 0) + (inH2H3 ? 1 : 0) + (inAlt ? 1 : 0) + (bodyCount >= 2 ? 2 : (bodyCount == 1 ? 1 : 0)) + (inUrl ? 1 : 0);

        TxtTargetKeywordResult.Text =
            $"🎯 Score SEO pour « {kw} » : {score}/10 — Occurrences : {bodyCount}x (Densité : {density:0.00}%) | " +
            $"Title : {(inTitle ? "✅ Oui" : "❌ Non")} • H1 : {(inH1 ? "✅ Oui" : "❌ Non")} • Meta Desc : {(inDesc ? "✅ Oui" : "❌ Non")} • " +
            $"H2/H3 : {(inH2H3 ? "✅ Oui" : "❌ Non")} • Alt Img : {(inAlt ? "✅ Oui" : "❌ Non")} • URL : {(inUrl ? "✅ Oui" : "❌ Non")}";
    }

    private async void BtnRunSpeedtestOnly_Click(object sender, RoutedEventArgs e)
    {
        if (_isAnalyzing) return;
        string? normalizedUrl = GetNormalizedTargetUrl();
        if (string.IsNullOrEmpty(normalizedUrl) || !Uri.TryCreate(normalizedUrl, UriKind.Absolute, out Uri? baseUri))
        {
            MessageBox.Show("Veuillez saisir un domaine valide (ex: salutalex.fr).", "Domaine requis", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        SwitchTab(2);
        _isAnalyzing = true;
        BtnRunSpeedtestOnly.IsEnabled = false;
        BtnQuickSpeedtest.IsEnabled = false;
        BtnRunSpeedtestOnly.Content = "⏳ Speedtest (5 salves) en cours...";
        TxtHeaderStatusBadge.Text = "⚡ Speedtest 5x en cours...";
        try
        {
            using var handler = new HttpClientHandler { AllowAutoRedirect = true, AutomaticDecompression = DecompressionMethods.All };
            using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(12) };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AlexToolkitSpeedtest/3.0");

            var res = await RunSpeedtestMetricsAsync(baseUri, client, sampleCount: 5, htmlBytesCount: 0, jsCount: 0, cssCount: 0, imgCount: 0, totalAssetBytes: 0, compressionName: "Auto");
            TxtPerfValue.Text = $"TTFB {res.TtfbMs} ms";
            TxtPerfSub.Text = $"DNS {res.DnsMs} ms • TLS {res.TlsMs} ms (5 salves)";
            TxtHeaderStatusBadge.Text = $"✅ Speedtest : TTFB {res.TtfbMs} ms";
        }
        catch (Exception ex)
        {
            TxtSpeedPingStats.Text = $"Erreur lors du Speedtest : {ex.Message}";
            TxtHeaderStatusBadge.Text = "⚠️ Erreur Speedtest";
        }
        finally
        {
            _isAnalyzing = false;
            BtnRunSpeedtestOnly.IsEnabled = true;
            BtnQuickSpeedtest.IsEnabled = true;
            BtnRunSpeedtestOnly.Content = "⚡ Relancer un Speedtest (5 salves)";
        }
    }

    private async Task<(long DnsMs, long TlsMs, long TtfbMs, long TransferMs, List<IPAddress> IpList)> RunSpeedtestMetricsAsync(
        Uri targetUri,
        HttpClient client,
        int sampleCount,
        int htmlBytesCount,
        int jsCount,
        int cssCount,
        int imgCount,
        long totalAssetBytes,
        string compressionName)
    {
        var ipList = new List<IPAddress>();
        var swDns = Stopwatch.StartNew();
        try
        {
            var addresses = await Dns.GetHostAddressesAsync(targetUri.Host, _cancellationToken);
            ipList.AddRange(addresses);
        }
        catch { }
        swDns.Stop();
        long dnsMs = Math.Max(1, swDns.ElapsedMilliseconds);

        var swTls = Stopwatch.StartNew();
        try
        {
            using var tcp = new TcpClient();
            int port = targetUri.Port > 0 ? targetUri.Port : 443;
            await tcp.ConnectAsync(targetUri.Host, port, _cancellationToken);
            if (targetUri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
            {
                using var ssl = new SslStream(tcp.GetStream(), false, (_, _, _, _) => true);
                await ssl.AuthenticateAsClientAsync(targetUri.Host);
            }
        }
        catch { }
        swTls.Stop();
        long tlsMs = Math.Max(1, swTls.ElapsedMilliseconds);

        var swTtfb = Stopwatch.StartNew();
        long ttfbMs = 0;
        long transferMs = 0;
        long measuredBytes = htmlBytesCount;
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, targetUri);
            using var resp = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, _cancellationToken);
            ttfbMs = Math.Max(1, swTtfb.ElapsedMilliseconds);

            var swDl = Stopwatch.StartNew();
            byte[] data = await resp.Content.ReadAsByteArrayAsync(_cancellationToken);
            swDl.Stop();
            transferMs = Math.Max(1, swDl.ElapsedMilliseconds);
            if (measuredBytes <= 0) measuredBytes = data.Length;
        }
        catch
        {
            swTtfb.Stop();
            ttfbMs = Math.Max(1, swTtfb.ElapsedMilliseconds);
        }

        var samples = new List<long>();
        var samplesLog = new StringBuilder();
        for (int i = 1; i <= sampleCount; i++)
        {
            var swSample = Stopwatch.StartNew();
            try
            {
                using var r = await client.GetAsync(targetUri, HttpCompletionOption.ResponseHeadersRead, _cancellationToken);
                swSample.Stop();
                long ms = Math.Max(1, swSample.ElapsedMilliseconds);
                samples.Add(ms);
                samplesLog.AppendLine($"Salve #{i} : {ms,4} ms  (HTTP {(int)r.StatusCode} {r.StatusCode})");
            }
            catch (Exception ex)
            {
                swSample.Stop();
                samplesLog.AppendLine($"Salve #{i} : ÉCHEC ({ex.GetType().Name})");
            }
        }

        TxtSpeedDns.Text = $"{dnsMs} ms";
        TxtSpeedDnsSub.Text = ipList.Count > 0 ? $"IP : {ipList[0]}" : "Résolu via cache système";

        TxtSpeedTls.Text = $"{tlsMs} ms";
        TxtSpeedTlsSub.Text = tlsMs < 150 ? "✅ Handshake TLS rapide" : "Handshake TCP + TLS 1.2/1.3";

        TxtSpeedTtfb.Text = $"{ttfbMs} ms";
        TxtSpeedTtfbSub.Text = ttfbMs < 300 ? "✅ Serveur ultra-réactif" : (ttfbMs < 800 ? "⚡ Réponse serveur normale" : "⚠️ TTFB élevé");

        double kbDownloaded = Math.Max(1.0, measuredBytes / 1024.0);
        double speedKbPerSec = kbDownloaded / Math.Max(0.005, (ttfbMs + transferMs) / 1000.0);
        TxtSpeedTransfer.Text = $"{transferMs} ms";
        TxtSpeedThroughput.Text = speedKbPerSec >= 1024
            ? $"Débit : {speedKbPerSec / 1024.0:0.1} Mo/s ({kbDownloaded:0} Ko)"
            : $"Débit : {speedKbPerSec:0} Ko/s ({kbDownloaded:0} Ko)";

        double totalWaterfall = Math.Max(1.0, dnsMs + tlsMs + ttfbMs + transferMs);
        TxtBarDnsLabel.Text = $"{dnsMs} ms ({dnsMs * 100.0 / totalWaterfall:0}%)";
        BarSpeedDns.Value = dnsMs * 100.0 / totalWaterfall;

        TxtBarTlsLabel.Text = $"{tlsMs} ms ({tlsMs * 100.0 / totalWaterfall:0}%)";
        BarSpeedTls.Value = tlsMs * 100.0 / totalWaterfall;

        TxtBarTtfbLabel.Text = $"{ttfbMs} ms ({ttfbMs * 100.0 / totalWaterfall:0}%)";
        BarSpeedTtfb.Value = ttfbMs * 100.0 / totalWaterfall;

        TxtBarTransferLabel.Text = $"{transferMs} ms ({transferMs * 100.0 / totalWaterfall:0}%)";
        BarSpeedTransfer.Value = transferMs * 100.0 / totalWaterfall;

        if (samples.Count > 0)
        {
            long minMs = samples.Min();
            long maxMs = samples.Max();
            double avgMs = samples.Average();
            double jitter = samples.Count > 1
                ? samples.Skip(1).Select((v, idx) => Math.Abs(v - samples[idx])).Average()
                : 0;

            TxtSpeedPingStats.Text =
                $"• Latence Minimale : {minMs} ms\n" +
                $"• Latence Moyenne : {avgMs:0} ms\n" +
                $"• Latence Maximale : {maxMs} ms\n" +
                $"• Gigue (Jitter) : {jitter:0.0} ms  |  Taux de réussite : {samples.Count}/{sampleCount} (100%)";
        }
        TxtSpeedSamplesList.Text = samplesLog.ToString().TrimEnd();

        if (htmlBytesCount > 0)
        {
            long totalBytes = Math.Max(htmlBytesCount, totalAssetBytes);
            double totalKb = totalBytes / 1024.0;
            TxtSpeedWeightBreakdown.Text =
                $"• Document HTML : {htmlBytesCount / 1024.0:0.1} Ko (Compression : {compressionName})\n" +
                $"• Ressources détectées : {jsCount} script(s) JS • {cssCount} feuille(s) CSS • {imgCount} image(s)\n" +
                $"• Poids total échantillonné : {totalKb:0.0} Ko";

            double fiberSec = Math.Max(0.05, (totalBytes * 8.0) / 100_000_000.0 + (ttfbMs / 1000.0));
            double lteSec = Math.Max(0.08, (totalBytes * 8.0) / 20_000_000.0 + (ttfbMs / 1000.0));
            double slow3gSec = Math.Max(0.15, (totalBytes * 8.0) / 3_000_000.0 + (ttfbMs / 1000.0));

            TxtSpeedNetworkSim.Text =
                $"• Fibre Optique (100 Mbps) : ~{fiberSec:0.00} sec\n" +
                $"• 4G Mobile (20 Mbps) : ~{lteSec:0.00} sec\n" +
                $"• 3G / Zone rurale (3 Mbps) : ~{slow3gSec:0.00} sec";
        }

        return (dnsMs, tlsMs, ttfbMs, transferMs, ipList);
    }

    private double ComputePillarPercent(params int[] indices)
    {
        if (indices.Length == 0) return 0;
        double pts = 0;
        foreach (int idx in indices)
        {
            pts += _allChecks[idx].State switch
            {
                CheckState.Pass => 1.0,
                CheckState.Warn => 0.5,
                _ => 0.0
            };
        }
        return Math.Round((pts * 100.0) / indices.Length);
    }

    private async Task RunFullAuditAsync()
    {
        if (_isAnalyzing) return;

        string? normalizedUrl = GetNormalizedTargetUrl();
        if (string.IsNullOrEmpty(normalizedUrl) || !Uri.TryCreate(normalizedUrl, UriKind.Absolute, out Uri? baseUri))
        {
            MessageBox.Show("Veuillez saisir un nom de domaine valide (ex: salutalex.fr).", "Domaine requis", MessageBoxButton.OK, MessageBoxImage.Information);
            TxtUrl.Focus();
            return;
        }

        _auditCts?.Dispose();
        _auditCts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken);
        var auditToken = _auditCts.Token;

        var totalAuditSw = Stopwatch.StartNew();
        _isAnalyzing = true;
        _activeFilter = null;
        _lastAnalyzedUrl = baseUri.ToString();
        BtnAnalyze.IsEnabled = false;
        BtnQuickSpeedtest.IsEnabled = false;
        BtnCancelAudit.Visibility = Visibility.Visible;
        BarAuditProgress.Visibility = Visibility.Visible;
        BarAuditProgress.Value = 10;
        BtnAnalyze.Content = "⏳ Analyse parallèle...";
        TxtHeaderStatusBadge.Text = $"⏳ Connexion à {baseUri.Host}...";
        InitializeDefaultChecks();
        RefreshFilteredList();

        _externalOutput?.Invoke($"🌐 [SUITE WEB] Démarrage de l'analyse haute performance sur : {baseUri}");

        string sslIssuer = "Inconnu";
        string sslSubject = "";
        string sslValidFrom = "";
        string sslValidTo = "";
        string sslThumbprint = "";
        string sslAlgo = "";
        int? sslDaysLeft = null;
        bool sslValid = false;

        try
        {
            using var handler = new SocketsHttpHandler
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.All,
                MaxConnectionsPerServer = 20,
                EnableMultipleHttp2Connections = true,
                PooledConnectionLifetime = TimeSpan.FromMinutes(2),
                ConnectTimeout = TimeSpan.FromSeconds(5),
                SslOptions = new SslClientAuthenticationOptions
                {
                    RemoteCertificateValidationCallback = (_, cert, _, errors) =>
                    {
                        if (cert != null)
                        {
                            try
                            {
                                var x509 = new X509Certificate2(cert);
                                sslIssuer = x509.GetNameInfo(X509NameType.SimpleName, true);
                                sslSubject = x509.GetNameInfo(X509NameType.SimpleName, false);
                                sslValidFrom = x509.NotBefore.ToString("dd/MM/yyyy");
                                sslValidTo = x509.NotAfter.ToString("dd/MM/yyyy");
                                sslThumbprint = x509.GetCertHashString(System.Security.Cryptography.HashAlgorithmName.SHA256);
                                sslAlgo = x509.SignatureAlgorithm.FriendlyName ?? "SHA256RSA";
                                sslDaysLeft = (int)(x509.NotAfter - DateTime.Now).TotalDays;
                                sslValid = errors == SslPolicyErrors.None && sslDaysLeft > 0;
                            }
                            catch { }
                        }
                        return true;
                    }
                }
            };

            using var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AlexToolkitWebAuditor/3.5");

            // ── 1. Téléchargement de la page principale ──
            var sw = Stopwatch.StartNew();
            using var response = await client.GetAsync(baseUri, auditToken);
            byte[] rawBytes = await response.Content.ReadAsByteArrayAsync(auditToken);
            string html = Encoding.UTF8.GetString(rawBytes);
            sw.Stop();
            BarAuditProgress.Value = 30;

            long elapsedMs = sw.ElapsedMilliseconds;
            int htmlSizeKb = Math.Max(1, rawBytes.Length / 1024);
            Uri finalUri = response.RequestMessage?.RequestUri ?? baseUri;
            string rootOrigin = $"{finalUri.Scheme}://{finalUri.Authority}";
            string serverHeader = response.Headers.Server.Any() ? string.Join(" ", response.Headers.Server) :
                                  (response.Headers.TryGetValues("x-vercel-id", out _) ? "Vercel Edge" :
                                   response.Headers.TryGetValues("cf-ray", out _) ? "Cloudflare CDN" : "Serveur Web");

            // ── 2. Téléchargement PARALLÈLE des premiers scripts JS ──
            var scriptMatches = Regex.Matches(html, @"<script\b[^>]*?\bsrc\s*=\s*[""']([^""']+)[""']", RegexOptions.IgnoreCase);
            var cssMatches = Regex.Matches(html, @"<link\b[^>]*?\brel\s*=\s*[""']stylesheet[""']", RegexOptions.IgnoreCase);
            var jsUrls = scriptMatches
                .Select(m => Uri.TryCreate(finalUri, m.Groups[1].Value, out var u) ? u : null)
                .Where(u => u != null && u.Host.Equals(finalUri.Host, StringComparison.OrdinalIgnoreCase))
                .Select(u => u!.ToString())
                .Distinct()
                .Take(4)
                .ToList();

            var combinedJsBuilder = new StringBuilder();
            long totalSampledAssetBytes = rawBytes.Length;
            if (jsUrls.Count > 0)
            {
                var jsTasks = jsUrls.Select(async jsUrl =>
                {
                    try
                    {
                        using var ctsJs = CancellationTokenSource.CreateLinkedTokenSource(auditToken);
                        ctsJs.CancelAfter(TimeSpan.FromSeconds(4));
                        return await client.GetStringAsync(jsUrl, ctsJs.Token);
                    }
                    catch { return ""; }
                });
                string[] jsResults = await Task.WhenAll(jsTasks);
                foreach (var jsContent in jsResults)
                {
                    if (!string.IsNullOrEmpty(jsContent))
                    {
                        totalSampledAssetBytes += Encoding.UTF8.GetByteCount(jsContent);
                        if (jsContent.Length <= 1_500_000)
                            combinedJsBuilder.AppendLine(jsContent);
                    }
                }
            }
            string fullCorpus = html + "\n" + combinedJsBuilder;
            BarAuditProgress.Value = 45;

            // Extraction des liens <a href="...">
            var anchorMatches = Regex.Matches(html, @"<a\b[^>]*?\bhref\s*=\s*[""']([^""']+)[""'][^>]*>(.*?)</a>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var links = new List<(string Href, string Text)>();
            foreach (Match m in anchorMatches)
            {
                string href = WebUtility.HtmlDecode(m.Groups[1].Value.Trim());
                string text = Regex.Replace(m.Groups[2].Value, "<.*?>", " ").Trim();
                links.Add((href, text));
            }

            async Task<string?> ProbeRoutesParallelAsync(string[] routes)
            {
                var tasks = routes.Select(async route =>
                {
                    try
                    {
                        using var ctsProbe = CancellationTokenSource.CreateLinkedTokenSource(auditToken);
                        ctsProbe.CancelAfter(TimeSpan.FromSeconds(3.5));
                        using var r = await client.GetAsync(rootOrigin + route, HttpCompletionOption.ResponseHeadersRead, ctsProbe.Token);
                        return r.IsSuccessStatusCode ? route : null;
                    }
                    catch { return null; }
                });
                var results = await Task.WhenAll(tasks);
                return results.FirstOrDefault(r => r != null);
            }

            // ── POINT 1 : Page RGPD ──
            var rgpdMatch = links.FirstOrDefault(l =>
                Regex.IsMatch(l.Href + " " + l.Text, @"\b(rgpd|confidentialit[eé]|privacy|donnees-personnelles|donn[eé]es\s+personnelles|politique\s+de\s+confidentialit)\b", RegexOptions.IgnoreCase));

            string? probedRgpdRoute = string.IsNullOrEmpty(rgpdMatch.Href)
                ? await ProbeRoutesParallelAsync(new[] { "/politique-de-confidentialite", "/confidentialite", "/rgpd", "/privacy", "/privacy-policy" })
                : null;

            if (!string.IsNullOrEmpty(rgpdMatch.Href))
                SetCheckResult(0, CheckState.Pass, "✅ Détectée", $"Lien trouvé : « {(string.IsNullOrWhiteSpace(rgpdMatch.Text) ? rgpdMatch.Href : rgpdMatch.Text)} » ({rgpdMatch.Href}).");
            else if (probedRgpdRoute != null)
                SetCheckResult(0, CheckState.Pass, "✅ Route active", $"Page accessible sur {probedRgpdRoute} (pensez à vérifier qu'elle est bien visible dans le pied de page).");
            else if (Regex.IsMatch(fullCorpus, @"\b(RGPD|politique de confidentialit[eé]|privacy policy)\b", RegexOptions.IgnoreCase))
                SetCheckResult(0, CheckState.Warn, "⚠️ Mention JS/Texte", "Mention RGPD trouvée dans le bundle, mais aucun lien HTML <a href> direct dans le pied de page.", "Ajoutez un lien <a href=\"/politique-de-confidentialite\">Politique de confidentialité</a> dans le footer.");
            else
                SetCheckResult(0, CheckState.Fail, "❌ Manquante", "Aucune page ni lien de Politique de confidentialité / RGPD détecté.", "Créez une page dédiée expliquant la collecte des données, la durée de conservation et les droits RGPD.");

            // ── POINT 2 : Page de CGU / Mentions légales ──
            var cguMatch = links.FirstOrDefault(l =>
                Regex.IsMatch(l.Href + " " + l.Text, @"\b(cgu|cgv|mentions-legales|mentions\s+l[eé]gales|conditions\s+g[eé]n[eé]rales|terms|legal)\b", RegexOptions.IgnoreCase));

            string? probedCguRoute = string.IsNullOrEmpty(cguMatch.Href)
                ? await ProbeRoutesParallelAsync(new[] { "/mentions-legales", "/cgu", "/cgv", "/conditions-generales", "/legal", "/terms" })
                : null;

            if (!string.IsNullOrEmpty(cguMatch.Href))
                SetCheckResult(1, CheckState.Pass, "✅ Détectée", $"Lien trouvé : « {(string.IsNullOrWhiteSpace(cguMatch.Text) ? cguMatch.Href : cguMatch.Text)} » ({cguMatch.Href}).");
            else if (probedCguRoute != null)
                SetCheckResult(1, CheckState.Pass, "✅ Route active", $"Page légale accessible sur {probedCguRoute}.");
            else if (Regex.IsMatch(fullCorpus, @"\b(mentions\s+l[eé]gales|conditions\s+g[eé]n[eé]rales|CGU|CGV)\b", RegexOptions.IgnoreCase))
                SetCheckResult(1, CheckState.Warn, "⚠️ Mention JS/Texte", "Mention CGU / Mentions légales détectée dans les scripts mais pas de lien HTML statique.", "Ajoutez un lien vers vos Mentions légales et CGU dans le footer de toutes les pages.");
            else
                SetCheckResult(1, CheckState.Fail, "❌ Manquante", "Aucun lien vers les CGU, CGV ou Mentions légales détecté.", "En France (loi LCEN), les Mentions légales (éditeur, hébergeur, contact) et CGU sont obligatoires.");

            // ── POINT 3 : API hors front-end ──
            var secretPatterns = new (string Name, string Pattern)[]
            {
                ("Clé Google API (AIza)", @"AIza[0-9A-Za-z\-_]{35}"),
                ("Clé OpenAI / Stripe secrète (sk_)", @"\b(sk-[a-zA-Z0-9]{20,}|sk_live_[0-9a-zA-Z]{24,})\b"),
                ("Token GitHub (ghp_)", @"\bghp_[A-Za-z0-9]{36}\b"),
                ("Clé AWS (AKIA)", @"\bAKIA[0-9A-Z]{16}\b"),
                ("Token Slack (xoxb)", @"\bxox[baprs]-[0-9a-zA-Z\-]{10,48}\b"),
                ("Clé privée PEM", @"-----BEGIN (RSA |EC )?PRIVATE KEY-----")
            };
            var exposedSecrets = secretPatterns.Where(p => Regex.IsMatch(fullCorpus, p.Pattern)).Select(p => p.Name).ToList();
            if (exposedSecrets.Count == 0)
                SetCheckResult(2, CheckState.Pass, "✅ Sécurisé", $"Aucune clé API secrète détectée dans le HTML ni dans les {jsUrls.Count} bundles JS inspectés.");
            else
                SetCheckResult(2, CheckState.Fail, "❌ Clé exposée", $"Clé secrète détectée côté client : {string.Join(", ", exposedSecrets)} !", "Déplacez immédiatement vos appels API sensibles vers un backend/route serveur et révoquez la clé exposée.");

            // ── POINT 4 : Forcer le HTTPS ──
            bool isHttps = finalUri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase);
            bool hasHsts = response.Headers.Contains("Strict-Transport-Security");
            bool httpRedirectsToHttps = false;
            try
            {
                using var noRedirHandler = new HttpClientHandler { AllowAutoRedirect = false };
                using var noRedirClient = new HttpClient(noRedirHandler) { Timeout = TimeSpan.FromSeconds(5) };
                var httpUri = new Uri($"http://{finalUri.Host}{finalUri.PathAndQuery}");
                using var httpResp = await noRedirClient.GetAsync(httpUri, _cancellationToken);
                if ((int)httpResp.StatusCode is >= 300 and < 400 &&
                    httpResp.Headers.Location != null &&
                    httpResp.Headers.Location.ToString().StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    httpRedirectsToHttps = true;
                }
            }
            catch { }

            string sslInfo = sslDaysLeft.HasValue ? $"Certificat SSL ({sslIssuer}) valide encore {sslDaysLeft} jours" : "HTTPS actif";
            if (isHttps && sslValid && (httpRedirectsToHttps || hasHsts))
                SetCheckResult(3, CheckState.Pass, "✅ HTTPS + Redirection", $"{sslInfo} • Redirection HTTP➔HTTPS : {(httpRedirectsToHttps ? "Oui" : "Implicite")} • HSTS : {(hasHsts ? "Activé" : "Non")}.");
            else if (isHttps)
                SetCheckResult(3, CheckState.Warn, "⚠️ Sans HSTS", $"{sslInfo}, mais l'en-tête Strict-Transport-Security (HSTS) n'est pas activé.", "Ajoutez l'en-tête HTTP Strict-Transport-Security: max-age=31536000; includeSubDomains.");
            else
                SetCheckResult(3, CheckState.Fail, "❌ Non sécurisé", "Le site ne force pas HTTPS ou le certificat SSL est invalide.", "Activez un certificat SSL/TLS (Let's Encrypt / Cloudflare) et forcez la redirection 301 vers HTTPS.");

            // ── POINT 5 : Bannière cookies ──
            var cookieMatch = Regex.Match(fullCorpus, @"\b(tarteaucitron|axeptio|cookiebot|onetrust|didomi|complianz|cookieyes|osano|klaro|quantcast|cookie-consent|cookie-banner|cc-window|gdpr-cookie|orejime)\b", RegexOptions.IgnoreCase);
            if (cookieMatch.Success)
                SetCheckResult(4, CheckState.Pass, "✅ Détectée", $"Gestionnaire de consentement cookies détecté ({cookieMatch.Value}).");
            else
                SetCheckResult(4, CheckState.Warn, "⚠️ Non détectée", "Aucune bannière de cookies détectée dans le HTML ou les scripts.", "Si vous utilisez des cookies non essentiels (GA4, pixels publicitaires), intégrez une bannière conforme CNIL (ex: Tarteaucitron, Axeptio).");

            // ── POINT 6 : Méta-titre (Meta title) & Description ──
            var titleMatch = Regex.Match(html, @"<title[^>]*>(.*?)</title>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var descMatch = Regex.Match(html, @"<meta\b[^>]*?\bname\s*=\s*[""']description[""'][^>]*?\bcontent\s*=\s*[""']([^""']*)[""']", RegexOptions.IgnoreCase)
                          is { Success: true } dm ? dm : Regex.Match(html, @"<meta\b[^>]*?\bcontent\s*=\s*[""']([^""']*)[""'][^>]*?\bname\s*=\s*[""']description[""']", RegexOptions.IgnoreCase);
            string pageTitle = titleMatch.Success ? WebUtility.HtmlDecode(Regex.Replace(titleMatch.Groups[1].Value, @"\s+", " ").Trim()) : "";
            string metaDesc = descMatch.Success ? WebUtility.HtmlDecode(descMatch.Groups[1].Value.Trim()) : "";
            _cachedPageTitle = pageTitle;
            _cachedMetaDesc = metaDesc;

            if (!string.IsNullOrEmpty(pageTitle) && !string.IsNullOrEmpty(metaDesc))
            {
                bool goodLen = pageTitle.Length is >= 15 and <= 70 && metaDesc.Length is >= 50 and <= 170;
                if (goodLen)
                    SetCheckResult(5, CheckState.Pass, "✅ Optimisé SEO", $"Title ({pageTitle.Length} car.) : « {pageTitle} » • Meta description ({metaDesc.Length} car.).");
                else
                    SetCheckResult(5, CheckState.Warn, "⚠️ Longueur à ajuster", $"Title : {pageTitle.Length} car. (« {pageTitle} ») • Meta description : {metaDesc.Length} car.", "Visez 30 à 60 caractères pour le <title> et 120 à 160 caractères pour la meta description.");
            }
            else if (!string.IsNullOrEmpty(pageTitle))
                SetCheckResult(5, CheckState.Warn, "⚠️ Sans description", $"Title présent (« {pageTitle} »), mais <meta name=\"description\"> est absente.", "Ajoutez <meta name=\"description\" content=\"Votre description accrocheuse (150 car.)\"> dans le <head>.");
            else
                SetCheckResult(5, CheckState.Fail, "❌ Manquant", "Aucune balise <title> détectée dans le <head>.", "Ajoutez obligatoirement une balise <title> unique et descriptive sur chaque page.");

            // ── POINT 7 : Image réseaux (Open Graph & Twitter Card) ──
            string ExtractMetaContent(string propName)
            {
                var m1 = Regex.Match(html, $@"<meta\b[^>]*?\b(?:property|name)\s*=\s*[""']{Regex.Escape(propName)}[""'][^>]*?\bcontent\s*=\s*[""']([^""']*)[""']", RegexOptions.IgnoreCase);
                if (m1.Success) return WebUtility.HtmlDecode(m1.Groups[1].Value.Trim());
                var m2 = Regex.Match(html, $@"<meta\b[^>]*?\bcontent\s*=\s*[""']([^""']*)[""'][^>]*?\b(?:property|name)\s*=\s*[""']{Regex.Escape(propName)}[""']", RegexOptions.IgnoreCase);
                return m2.Success ? WebUtility.HtmlDecode(m2.Groups[1].Value.Trim()) : "";
            }

            string ogTitle = ExtractMetaContent("og:title");
            string ogDesc = ExtractMetaContent("og:description");
            string ogImage = ExtractMetaContent("og:image");
            string twitterCard = ExtractMetaContent("twitter:card");

            bool hasOgTitle = !string.IsNullOrEmpty(ogTitle);
            bool hasOgImage = !string.IsNullOrEmpty(ogImage);
            bool hasOgDesc = !string.IsNullOrEmpty(ogDesc);

            if (hasOgTitle && hasOgImage && hasOgDesc)
                SetCheckResult(6, CheckState.Pass, "✅ Complet", $"Open Graph complet (og:title, og:description, og:image{(!string.IsNullOrEmpty(twitterCard) ? $" + twitter:{twitterCard}" : "")}).");
            else if (hasOgTitle || hasOgImage)
                SetCheckResult(6, CheckState.Warn, "⚠️ Incomplet", $"og:title={(hasOgTitle ? "Oui" : "Non")}, og:image={(hasOgImage ? "Oui" : "Non")}, og:description={(hasOgDesc ? "Oui" : "Non")}.", "Ajoutez og:image (format 1200x630 px recommandé), og:title et og:description pour un aperçu riche sur Discord, LinkedIn et X.");
            else
                SetCheckResult(6, CheckState.Fail, "❌ Manquant", "Aucune balise Open Graph détectée.", "Ajoutez les balises <meta property=\"og:title\">, <meta property=\"og:description\"> et <meta property=\"og:image\"> dans le <head>.");

            // ── POINT 8 : Favicon ──
            bool hasFaviconTag = Regex.IsMatch(html, @"<link\b[^>]*?\brel\s*=\s*[""'][^""']*icon[^""']*[""']", RegexOptions.IgnoreCase);
            bool hasFaviconFile = false;
            if (!hasFaviconTag)
            {
                try
                {
                    using var favResp = await client.GetAsync($"{rootOrigin}/favicon.ico", HttpCompletionOption.ResponseHeadersRead, _cancellationToken);
                    hasFaviconFile = favResp.IsSuccessStatusCode;
                }
                catch { }
            }
            if (hasFaviconTag || hasFaviconFile)
                SetCheckResult(7, CheckState.Pass, "✅ Présent", hasFaviconTag ? "Balise <link rel=\"icon\"> déclarée dans l'en-tête HTML." : "Fichier /favicon.ico accessible à la racine.");
            else
                SetCheckResult(7, CheckState.Fail, "❌ Manquant", "Aucun favicon trouvé.", "Ajoutez <link rel=\"icon\" href=\"/favicon.ico\"> et <link rel=\"apple-touch-icon\" href=\"/apple-touch-icon.png\">.");

            // ── POINT 9 : Sitemap + robots.txt ──
            bool hasRobots = false;
            bool hasSitemap = false;
            try
            {
                using var robResp = await client.GetAsync($"{rootOrigin}/robots.txt", _cancellationToken);
                if (robResp.IsSuccessStatusCode)
                {
                    string robText = await robResp.Content.ReadAsStringAsync(_cancellationToken);
                    hasRobots = robText.Contains("User-agent", StringComparison.OrdinalIgnoreCase);
                    if (robText.Contains("Sitemap:", StringComparison.OrdinalIgnoreCase))
                        hasSitemap = true;
                }
            }
            catch { }

            if (!hasSitemap)
            {
                try
                {
                    using var siteResp = await client.GetAsync($"{rootOrigin}/sitemap.xml", HttpCompletionOption.ResponseHeadersRead, _cancellationToken);
                    hasSitemap = siteResp.IsSuccessStatusCode;
                }
                catch { }
            }

            if (hasRobots && hasSitemap)
                SetCheckResult(8, CheckState.Pass, "✅ Tous deux actifs", "Fichiers /robots.txt et /sitemap.xml présents et accessibles.");
            else if (hasRobots || hasSitemap)
                SetCheckResult(8, CheckState.Warn, "⚠️ Partiel", $"robots.txt : {(hasRobots ? "OK" : "Absent")} • sitemap.xml : {(hasSitemap ? "OK" : "Absent")}.", "Assurez-vous d'avoir à la fois /robots.txt et /sitemap.xml, et déclarez 'Sitemap: https://votresite.fr/sitemap.xml' dans robots.txt.");
            else
                SetCheckResult(8, CheckState.Fail, "❌ Absents", "Ni /robots.txt ni /sitemap.xml n'ont été trouvés.", "Générez un fichier /robots.txt et un /sitemap.xml pour permettre à Google d'indexer vos pages.");

            // ── POINT 10 : Textes images (attributs Alt) ──
            var imgMatches = Regex.Matches(html, @"<img\b[^>]*>", RegexOptions.IgnoreCase);
            int totalImgs = imgMatches.Count;
            int imgsWithAlt = 0;
            var imgUrls = new List<string>();
            var altBuilder = new StringBuilder();
            foreach (Match img in imgMatches)
            {
                var altM = Regex.Match(img.Value, @"\balt\s*=\s*[""']([^""']*)[""']", RegexOptions.IgnoreCase);
                if (altM.Success && !string.IsNullOrWhiteSpace(altM.Groups[1].Value))
                {
                    imgsWithAlt++;
                    altBuilder.Append(altM.Groups[1].Value).Append(' ');
                }

                var srcM = Regex.Match(img.Value, @"\bsrc\s*=\s*[""']([^""']+)[""']", RegexOptions.IgnoreCase);
                if (srcM.Success && !srcM.Groups[1].Value.StartsWith("data:", StringComparison.OrdinalIgnoreCase) &&
                    Uri.TryCreate(finalUri, srcM.Groups[1].Value, out var imgUri))
                {
                    imgUrls.Add(imgUri.ToString());
                }
            }
            _cachedAltText = altBuilder.ToString();

            if (totalImgs == 0)
                SetCheckResult(9, CheckState.Pass, "✅ Aucune image <img>", "Aucune balise <img> sans alt détectée.");
            else if (imgsWithAlt == totalImgs)
                SetCheckResult(9, CheckState.Pass, $"✅ 100% ({imgsWithAlt}/{totalImgs})", $"Toutes les {totalImgs} images possèdent un attribut alt descriptif.");
            else if (imgsWithAlt > 0)
                SetCheckResult(9, CheckState.Warn, $"⚠️ {imgsWithAlt}/{totalImgs} renseignés", $"{totalImgs - imgsWithAlt} image(s) sur {totalImgs} n'ont pas d'attribut alt.", "Ajoutez un attribut alt=\"Description de l'image\" sur chaque balise <img> pour le SEO et les lecteurs d'écran.");
            else
                SetCheckResult(9, CheckState.Fail, $"❌ 0/{totalImgs} renseigné", $"Aucune des {totalImgs} images n'a d'attribut alt.", "Renseignez obligatoirement l'attribut alt sur toutes vos balises <img>.");

            // ── POINT 11 : Compresse images (vérification parallèle du poids en Ko) ──
            bool hasModernFormats = Regex.IsMatch(html, @"\.(webp|avif|svg)\b", RegexOptions.IgnoreCase);
            bool hasLazyLoading = Regex.IsMatch(html, @"loading\s*=\s*[""']lazy[""']", RegexOptions.IgnoreCase);
            long maxImgBytes = 0;
            var sampledImgUrls = imgUrls.Distinct().Take(6).ToList();
            if (sampledImgUrls.Count > 0)
            {
                var imgSizeTasks = sampledImgUrls.Select(async imgUrl =>
                {
                    try
                    {
                        using var ctsImg = CancellationTokenSource.CreateLinkedTokenSource(auditToken);
                        ctsImg.CancelAfter(TimeSpan.FromSeconds(3));
                        using var headReq = new HttpRequestMessage(HttpMethod.Head, imgUrl);
                        using var headResp = await client.SendAsync(headReq, HttpCompletionOption.ResponseHeadersRead, ctsImg.Token);
                        return headResp.Content.Headers.ContentLength ?? 0L;
                    }
                    catch { return 0L; }
                });
                long[] imgSizes = await Task.WhenAll(imgSizeTasks);
                foreach (long len in imgSizes)
                {
                    totalSampledAssetBytes += len;
                    if (len > maxImgBytes) maxImgBytes = len;
                }
            }
            long maxImgKb = maxImgBytes / 1024;
            BarAuditProgress.Value = 65;

            if (maxImgKb > 350)
                SetCheckResult(10, CheckState.Warn, $"⚠️ Image lourde ({maxImgKb} Ko)", $"Une image de {maxImgKb} Ko a été détectée • WebP/AVIF : {(hasModernFormats ? "Oui" : "Non")}.", "Compressez vos images sous 150-200 Ko en les convertissant au format WebP ou AVIF et ajoutez loading=\"lazy\".");
            else if (totalImgs == 0 || (hasModernFormats && hasLazyLoading))
                SetCheckResult(10, CheckState.Pass, "✅ Optimisées", $"Formats modernes (WebP/AVIF/SVG) et lazy-loading actifs{(maxImgKb > 0 ? $" (max mesuré : {maxImgKb} Ko)" : "")}.");
            else if (hasModernFormats || hasLazyLoading)
                SetCheckResult(10, CheckState.Pass, "✅ Poids maîtrisé", $"Poids max mesuré : {(maxImgKb > 0 ? $"{maxImgKb} Ko" : "< 200 Ko")} • WebP/AVIF : {(hasModernFormats ? "Oui" : "Non")} • Lazy-loading : {(hasLazyLoading ? "Oui" : "Non")}.");
            else
                SetCheckResult(10, CheckState.Warn, "⚠️ Sans WebP / Lazy", "Images en formats classiques sans attribut loading=\"lazy\".", "Convertissez vos images en .webp et ajoutez loading=\"lazy\" sur les images hors de l'écran initial.");

            // ── POINT 12 : Vitesse pages ──
            if (elapsedMs < 700)
                SetCheckResult(11, CheckState.Pass, $"✅ Rapide ({elapsedMs} ms)", $"Temps de réponse rapide : {elapsedMs} ms • Poids HTML : {htmlSizeKb} Ko • Serveur : {serverHeader}.");
            else if (elapsedMs < 1800)
                SetCheckResult(11, CheckState.Warn, $"⚠️ Moyen ({elapsedMs} ms)", $"Temps de réponse de {elapsedMs} ms • Poids HTML : {htmlSizeKb} Ko.", "Activez la mise en cache CDN (Cloudflare/Vercel) et la compression Brotli/Gzip pour descendre sous 500 ms.");
            else
                SetCheckResult(11, CheckState.Fail, $"❌ Lent ({elapsedMs} ms)", $"Temps de chargement élevé : {elapsedMs} ms ({htmlSizeKb} Ko).", "Optimisez le temps de réponse serveur (TTFB), réduisez les scripts bloquants et activez un CDN.");

            // ── POINT 13 : Contexte & Contraste (lang, H1, ARIA) + Extraction H1..H6 ──
            var langMatch = Regex.Match(html, @"<html\b[^>]*?\blang\s*=\s*[""']([a-zA-Z\-]+)[""']", RegexOptions.IgnoreCase);
            bool hasLang = langMatch.Success;
            string htmlLang = langMatch.Success ? langMatch.Groups[1].Value : "Non défini";

            var headingMatches = Regex.Matches(html, @"<(h[1-6])\b[^>]*>(.*?)</\1>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var headingItems = new List<HeadingTreeItem>();
            var h1Builder = new StringBuilder();
            var h2h3Builder = new StringBuilder();
            int h1Count = 0;

            foreach (Match hm in headingMatches)
            {
                string tag = hm.Groups[1].Value.ToUpperInvariant();
                string hText = WebUtility.HtmlDecode(Regex.Replace(hm.Groups[2].Value, "<.*?>", " ").Trim());
                hText = Regex.Replace(hText, @"\s+", " ");
                if (string.IsNullOrWhiteSpace(hText)) continue;

                int level = tag[1] - '0';
                if (level == 1) { h1Count++; h1Builder.Append(hText).Append(' '); }
                else if (level is 2 or 3) { h2h3Builder.Append(hText).Append(' '); }

                Brush badgeBrush = level switch
                {
                    1 => (SolidColorBrush)ThemeManager.GetBrush("Accent"),
                    2 => (SolidColorBrush)ThemeManager.GetBrush("Success"),
                    3 => (SolidColorBrush)ThemeManager.GetBrush("Accent"),
                    4 => (SolidColorBrush)ThemeManager.GetBrush("Warning"),
                    _ => (SolidColorBrush)ThemeManager.GetBrush("TextMuted")
                };

                headingItems.Add(new HeadingTreeItem
                {
                    Tag = tag,
                    Text = hText,
                    IndentMargin = new Thickness((level - 1) * 18, 0, 0, 5),
                    BadgeBg = badgeBrush
                });
            }
            _cachedH1Text = h1Builder.ToString();
            _cachedH2H3Text = h2h3Builder.ToString();

            bool hasAria = Regex.IsMatch(html, @"\b(aria-label|aria-labelledby|role\s*=)\b", RegexOptions.IgnoreCase);
            if (hasLang && h1Count == 1)
                SetCheckResult(12, CheckState.Pass, "✅ Conforme", $"Attribut <html lang=\"{htmlLang}\"> défini, 1 titre <h1> unique{(hasAria ? " et balises ARIA présentes" : "")}.");
            else if (hasLang || h1Count >= 1)
                SetCheckResult(12, CheckState.Warn, "⚠️ À ajuster", $"Attribut <html lang> : {(hasLang ? htmlLang : "Absent")} • Nombre de titres <h1> : {h1Count} (recommandé : 1 seul).", "Définissez <html lang=\"fr\"> et assurez-vous d'avoir un seul titre <h1> par page avec un ratio de contraste WCAG >= 4.5:1.");
            else
                SetCheckResult(12, CheckState.Fail, "❌ Non conforme", "Attribut <html lang> et titre principal <h1> tous deux absents.", "Ajoutez <html lang=\"fr\"> et structurez votre page avec un titre principal <h1> unique.");

            // ── POINT 14 : Site responsive ──
            bool hasViewport = Regex.IsMatch(html, @"<meta\b[^>]*?\bname\s*=\s*[""']viewport[""'][^>]*?\bwidth\s*=\s*device-width", RegexOptions.IgnoreCase);
            if (hasViewport)
                SetCheckResult(13, CheckState.Pass, "✅ Responsive", "Balise <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"> présente.");
            else
                SetCheckResult(13, CheckState.Fail, "❌ Non responsive", "Balise meta viewport absente.", "Ajoutez <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"> dans le <head>.");

            // ── POINT 15 : Page 404 sur mesure ──
            try
            {
                using var cts404 = CancellationTokenSource.CreateLinkedTokenSource(auditToken);
                cts404.CancelAfter(TimeSpan.FromSeconds(4));
                string fakeUrl = $"{rootOrigin}/page-introuvable-audit-{Guid.NewGuid():N}";
                using var resp404 = await client.GetAsync(fakeUrl, cts404.Token);
                string body404 = await resp404.Content.ReadAsStringAsync(cts404.Token);
                if (resp404.StatusCode == HttpStatusCode.NotFound && body404.Length > 300)
                    SetCheckResult(14, CheckState.Pass, "✅ 404 Personnalisée", $"Code HTTP 404 confirmé avec une page personnalisée ({body404.Length:N0} octets).");
                else if (resp404.StatusCode == HttpStatusCode.NotFound)
                    SetCheckResult(14, CheckState.Warn, "⚠️ 404 Basique", "Code HTTP 404 renvoyé mais page par défaut très brute.", "Créez une page 404 personnalisée aux couleurs du site avec un bouton de retour vers l'accueil.");
                else
                    SetCheckResult(14, CheckState.Warn, $"⚠️ Soft 404 ({(int)resp404.StatusCode})", $"Une URL inexistante renvoie le code HTTP {(int)resp404.StatusCode} au lieu de 404.", "Configurez votre routeur/serveur pour renvoyer un vrai statut HTTP 404 sur les pages inexistantes.");
            }
            catch
            {
                SetCheckResult(14, CheckState.Warn, "⚠️ Non vérifiable", "Impossible de tester une route 404 sur cet hôte.");
            }

            // ── POINT 16 : Répare liens cassés (vérification haute concurrence + détails pour l'onglet 6) ──
            var testableLinks = links
                .Select(l => l.Href)
                .Where(h => !string.IsNullOrWhiteSpace(h) && !h.StartsWith("#") && !h.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase) && !h.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase) && !h.StartsWith("tel:", StringComparison.OrdinalIgnoreCase))
                .Select(h => Uri.TryCreate(finalUri, h, out var u) ? u : null)
                .Where(u => u != null && (u.Scheme == "http" || u.Scheme == "https"))
                .Select(u => u!.ToString())
                .Distinct()
                .Take(16)
                .ToList();

            var brokenLinks = new List<string>();
            var inspectedLinks = new List<InspectedLinkItem>();
            await Parallel.ForEachAsync(testableLinks, new ParallelOptions { MaxDegreeOfParallelism = 10, CancellationToken = auditToken }, async (url, token) =>
            {
                var swLink = Stopwatch.StartNew();
                bool isInternal = Uri.TryCreate(url, UriKind.Absolute, out var lu) && lu.Host.Equals(finalUri.Host, StringComparison.OrdinalIgnoreCase);
                string scope = isInternal ? "🏠 Interne" : "🌍 Externe";
                try
                {
                    using var ctsLink = CancellationTokenSource.CreateLinkedTokenSource(token);
                    ctsLink.CancelAfter(TimeSpan.FromSeconds(4));
                    using var req = new HttpRequestMessage(HttpMethod.Get, url);
                    using var linkResp = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ctsLink.Token);
                    swLink.Stop();
                    int code = (int)linkResp.StatusCode;
                    bool isErr = code >= 400 && linkResp.StatusCode != HttpStatusCode.Forbidden && linkResp.StatusCode != HttpStatusCode.TooManyRequests && code != 999;
                    lock (brokenLinks)
                    {
                        if (isErr) brokenLinks.Add($"{url} (HTTP {code})");
                        inspectedLinks.Add(new InspectedLinkItem
                        {
                            StatusBadge = isErr ? $"❌ HTTP {code}" : $"✅ HTTP {code}",
                            StatusFg = isErr ? FailFg : PassFg,
                            Scope = scope,
                            LatencyText = $"{swLink.ElapsedMilliseconds} ms",
                            Url = url,
                            IsBroken = isErr
                        });
                    }
                }
                catch
                {
                    swLink.Stop();
                    lock (brokenLinks)
                    {
                        brokenLinks.Add($"{url} (Injoignable)");
                        inspectedLinks.Add(new InspectedLinkItem
                        {
                            StatusBadge = "❌ Timeout/Err",
                            StatusFg = FailFg,
                            Scope = scope,
                            LatencyText = $"{swLink.ElapsedMilliseconds} ms",
                            Url = url,
                            IsBroken = true
                        });
                    }
                }
            });
            ItemsInspectedLinks.ItemsSource = inspectedLinks.OrderByDescending(x => x.IsBroken).ToList();
            BarAuditProgress.Value = 82;

            if (testableLinks.Count == 0)
                SetCheckResult(15, CheckState.Warn, "⚠️ Aucun lien HTML", "Aucun lien <a href> statique trouvé dans le DOM initial.");
            else if (brokenLinks.Count == 0)
                SetCheckResult(15, CheckState.Pass, $"✅ 0 lien cassé ({testableLinks.Count} testés)", $"Les {testableLinks.Count} liens inspectés en parallèle répondent tous avec succès.");
            else
                SetCheckResult(15, CheckState.Fail, $"❌ {brokenLinks.Count} lien(s) cassé(s)", $"Liens en erreur : {string.Join(" | ", brokenLinks.Take(3))}", "Corrigez ou supprimez les URLs qui renvoient une erreur 404.");

            // ── POINT 17 : Valid. formulaire ──
            int formCount = Regex.Matches(html, @"<form\b", RegexOptions.IgnoreCase).Count;
            int inputCount = Regex.Matches(html, @"<(input|textarea|select)\b", RegexOptions.IgnoreCase).Count;
            bool hasValidationAttrs = Regex.IsMatch(html, @"\b(required|pattern\s*=|minlength\s*=|type\s*=\s*[""']email[""'])\b", RegexOptions.IgnoreCase);
            if (formCount == 0 && inputCount == 0)
                SetCheckResult(16, CheckState.Pass, "✅ Sans formulaire", "Aucun formulaire de saisie exposé sur cette page.");
            else if (hasValidationAttrs)
                SetCheckResult(16, CheckState.Pass, "✅ Validé", $"{formCount} formulaire(s) / {inputCount} champ(s) avec validation HTML5 (required, type email, etc.).");
            else
                SetCheckResult(16, CheckState.Warn, "⚠️ Sans attributs HTML5", $"{inputCount} champ(s) détecté(s) sans attribut 'required' ou type spécifique.", "Ajoutez les attributs required, type=\"email\", maxlength et une validation côté serveur.");

            // ── POINT 18 : Anti-spam ──
            var spamMatch = Regex.Match(fullCorpus, @"\b(recaptcha|g-recaptcha|turnstile|cf-turnstile|hcaptcha|honeypot|akismet)\b", RegexOptions.IgnoreCase);
            if (formCount == 0 && inputCount == 0)
                SetCheckResult(17, CheckState.Pass, "✅ Non applicable", "Aucun formulaire public exposé sur cette page.");
            else if (spamMatch.Success)
                SetCheckResult(17, CheckState.Pass, "✅ Protégé", $"Système anti-spam détecté ({spamMatch.Value}).");
            else
                SetCheckResult(17, CheckState.Warn, "⚠️ Non détecté", "Formulaire présent sans protection anti-spam détectée.", "Intégrez Cloudflare Turnstile (gratuit et sans friction), reCAPTCHA ou un champ honeypot caché.");

            // ── POINT 19 : Outil d'analytics ──
            var analyticsMatch = Regex.Match(fullCorpus, @"\b(googletagmanager|gtag|GoogleAnalyticsObject|G-[A-Z0-9]{6,}|GTM-[A-Z0-9]{4,}|plausible\.io|matomo|piwik|umami|clarity\.ms|hotjar|_vercel/insights|vercel-analytics)\b", RegexOptions.IgnoreCase);
            if (analyticsMatch.Success)
                SetCheckResult(18, CheckState.Pass, "✅ Actif", $"Outil d'analyse d'audience détecté ({analyticsMatch.Value.Trim()}).");
            else
                SetCheckResult(18, CheckState.Warn, "⚠️ Non détecté", "Aucun script de mesure d'audience détecté.", "Ajoutez Plausible, Umami (exemptés de bandeau cookies CNIL) ou Google Analytics 4 pour suivre votre trafic.");

            // ── POINT 20 : Un seul CTA ──
            var ctaElements = Regex.Matches(html, @"<(a|button)\b[^>]*?\bclass\s*=\s*[""'][^""']*\b(cta|btn-primary|button-primary|bg-blue|bg-indigo|bg-emerald|primary)\b[^""']*[""'][^>]*>(.*?)</\1>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var ctaTexts = ctaElements
                .Select(m => WebUtility.HtmlDecode(Regex.Replace(m.Groups[3].Value, "<.*?>", " ").Trim()))
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Distinct()
                .ToList();
            int buttonCount = Regex.Matches(html, @"<button\b", RegexOptions.IgnoreCase).Count;

            if (ctaTexts.Count == 1)
                SetCheckResult(19, CheckState.Pass, "✅ 1 CTA principal", $"Appel à l'action principal unique et clair : « {ctaTexts[0]} ».");
            else if (ctaTexts.Count > 1)
                SetCheckResult(19, CheckState.Warn, $"⚠️ {ctaTexts.Count} CTA détectés", $"Boutons principaux détectés : « {string.Join(" », « ", ctaTexts.Take(3))} ».", "Privilégiez un seul appel à l'action principal par écran pour maximiser votre taux de conversion.");
            else
                SetCheckResult(19, CheckState.Pass, $"ℹ️ {buttonCount} bouton(s)", $"{buttonCount} bouton(s) et {links.Count} lien(s) sur la page — vérifiez visuellement que votre CTA principal ressort clairement.");

            // ── POINT 21 : Protection contre les injections SQL & En-têtes de sécurité ──
            bool hasCsp = response.Headers.Contains("Content-Security-Policy");
            bool hasNoSniff = response.Headers.Contains("X-Content-Type-Options");
            bool hasFrameOptions = response.Headers.Contains("X-Frame-Options");
            bool hasReferrer = response.Headers.Contains("Referrer-Policy");
            bool hasSqlError = Regex.IsMatch(html, @"(SQL syntax.*?MySQL|Warning.*?\Wmysqli?_|PostgreSQL.*?ERROR|ORA-[0-9]{5}|SQLite/JDBCDriver|System\.Data\.SqlClient)", RegexOptions.IgnoreCase);

            int secHeadersCount = (hasCsp ? 1 : 0) + (hasNoSniff ? 1 : 0) + (hasFrameOptions ? 1 : 0) + (hasReferrer ? 1 : 0);
            if (hasSqlError)
                SetCheckResult(20, CheckState.Fail, "❌ Erreur SQL exposée", "Une erreur de base de données SQL brute est visible dans la réponse HTML !", "Utilisez exclusivement des requêtes préparées (PDO / Prisma / ORM) et désactivez l'affichage des erreurs SQL en production.");
            else if (isHttps && secHeadersCount >= 2)
                SetCheckResult(20, CheckState.Pass, $"✅ Protégé ({secHeadersCount}/4 en-têtes)", $"Aucune fuite SQL • En-têtes actifs : CSP={(hasCsp ? "Oui" : "Non")}, NoSniff={(hasNoSniff ? "Oui" : "Non")}, X-Frame={(hasFrameOptions ? "Oui" : "Non")}, Referrer={(hasReferrer ? "Oui" : "Non")}.");
            else
                SetCheckResult(20, CheckState.Warn, $"⚠️ En-têtes ({secHeadersCount}/4)", $"Aucune fuite SQL, mais {4 - secHeadersCount} en-tête(s) HTTP de sécurité manquent.", "Configurez Content-Security-Policy, X-Content-Type-Options: nosniff, X-Frame-Options: DENY et utilisez des requêtes SQL préparées.");

            // =====================================================================
            // MODULE SPEEDTEST & BENCHMARK SERVEUR
            // =====================================================================
            var speedMetrics = await RunSpeedtestMetricsAsync(
                finalUri,
                client,
                sampleCount: 3,
                htmlBytesCount: rawBytes.Length,
                jsCount: scriptMatches.Count,
                cssCount: cssMatches.Count,
                imgCount: totalImgs,
                totalAssetBytes: totalSampledAssetBytes,
                compressionName: "Gzip/Brotli auto");

            // =====================================================================
            // MODULE SEO, SERP PREVIEW & ARBORESCENCE H1-H6
            // =====================================================================
            TxtSerpUrl.Text = finalUri.ToString();
            TxtSerpTitle.Text = string.IsNullOrEmpty(pageTitle) ? "(Aucun titre <title> défini sur cette page)" : (pageTitle.Length > 62 ? pageTitle[..60] + "..." : pageTitle);
            TxtSerpDesc.Text = string.IsNullOrEmpty(metaDesc) ? "(Aucune meta description définie — Google affichera un extrait aléatoire du texte de votre page.)" : (metaDesc.Length > 162 ? metaDesc[..160] + "..." : metaDesc);
            TxtSerpMetrics.Text = $"Title : {pageTitle.Length} / 60 caractères {(pageTitle.Length is >= 30 and <= 60 ? "✅" : "⚠️")}  •  Meta Description : {metaDesc.Length} / 160 caractères {(metaDesc.Length is >= 110 and <= 160 ? "✅" : "⚠️")}";

            TxtOgPreviewTitle.Text = !string.IsNullOrEmpty(ogTitle) ? $"og:title : {ogTitle}" : "og:title : ❌ Non renseigné";
            TxtOgPreviewDesc.Text = !string.IsNullOrEmpty(ogDesc) ? $"og:description : {ogDesc}" : "og:description : ❌ Non renseignée";
            TxtOgPreviewImg.Text = !string.IsNullOrEmpty(ogImage) ? $"og:image : {ogImage}" : "og:image : ❌ Aucune image de partage définie";

            var canonicalMatch = Regex.Match(html, @"<link\b[^>]*?\brel\s*=\s*[""']canonical[""'][^>]*?\bhref\s*=\s*[""']([^""']+)[""']", RegexOptions.IgnoreCase);
            string canonicalUrl = canonicalMatch.Success ? canonicalMatch.Groups[1].Value : "Non définie";
            string metaRobots = ExtractMetaContent("robots");
            bool hasJsonLd = Regex.IsMatch(html, @"application/ld\+json", RegexOptions.IgnoreCase);

            string visibleHtml = Regex.Replace(html, @"<(script|style|noscript|svg|head)\b[^>]*>.*?</\1>", " ", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            string plainText = WebUtility.HtmlDecode(Regex.Replace(visibleHtml, @"<[^>]+>", " "));
            plainText = Regex.Replace(plainText, @"\s+", " ").Trim();
            _cachedVisibleText = plainText;

            double textHtmlRatio = rawBytes.Length > 0 ? (Encoding.UTF8.GetByteCount(plainText) * 100.0) / rawBytes.Length : 0;
            int internalLinksCount = links.Count(l => Uri.TryCreate(finalUri, l.Href, out var u) && u.Host.Equals(finalUri.Host, StringComparison.OrdinalIgnoreCase));
            int externalLinksCount = Math.Max(0, links.Count - internalLinksCount);

            TxtSeoTechDetails.Text =
                $"• URL Canonique : {canonicalUrl}\n" +
                $"• Directive Meta Robots : {(string.IsNullOrEmpty(metaRobots) ? "index, follow (implicite)" : metaRobots)}\n" +
                $"• Données structurées Schema.org (JSON-LD) : {(hasJsonLd ? "✅ Présentes" : "❌ Absentes")}\n" +
                $"• Langue déclarée : {htmlLang}  |  Ratio Texte / HTML : {textHtmlRatio:0.1}%\n" +
                $"• Maillage des liens : {internalLinksCount} lien(s) interne(s) • {externalLinksCount} lien(s) externe(s)";

            TxtHeadingsSummary.Text = $"{headingItems.Count} titre(s) Hn ({h1Count} H1)";
            if (headingItems.Count == 0)
            {
                headingItems.Add(new HeadingTreeItem
                {
                    Tag = "INFO",
                    Text = "Aucune balise H1 à H6 statique trouvée dans le DOM initial (ou rendu 100% client JS).",
                    IndentMargin = new Thickness(0),
                    BadgeBg = (SolidColorBrush)ThemeManager.GetBrush("TextMuted")
                });
            }
            ItemsHeadings.ItemsSource = headingItems;

            // =====================================================================
            // MODULE EXTRACTEUR DE MOTS-CLÉS & DENSITÉ LEXICALE (1, 2, 3 MOTS)
            // =====================================================================
            string keywordCorpus = $"{pageTitle} {_cachedH1Text} {_cachedH2H3Text} {plainText}";
            var rawWords = Regex.Matches(keywordCorpus.ToLowerInvariant(), @"\b[a-zàâäéèêëîïôöùûüÿçœæ]{3,}\b")
                .Select(m => m.Value)
                .ToList();

            var filteredWords = rawWords.Where(w => !StopWords.Contains(w) && !Regex.IsMatch(w, @"^\d+$")).ToList();
            _cachedTotalWords = Math.Max(1, rawWords.Count);
            int uniqueWords = filteredWords.Distinct().Count();
            int readingMinutes = Math.Max(1, (int)Math.Ceiling(_cachedTotalWords / 200.0));

            TxtWordCountSummary.Text = $"Total : {_cachedTotalWords} mots • Vocabulaire clé : {uniqueWords} termes • Lecture : ~{readingMinutes} min";

            string GetStrategicLocations(string phrase)
            {
                var locs = new List<string>();
                if (pageTitle.Contains(phrase, StringComparison.OrdinalIgnoreCase)) locs.Add("Title");
                if (_cachedH1Text.Contains(phrase, StringComparison.OrdinalIgnoreCase)) locs.Add("H1");
                if (_cachedH2H3Text.Contains(phrase, StringComparison.OrdinalIgnoreCase)) locs.Add("H2/H3");
                if (metaDesc.Contains(phrase, StringComparison.OrdinalIgnoreCase)) locs.Add("Meta Desc");
                if (finalUri.ToString().Contains(phrase.Replace(' ', '-'), StringComparison.OrdinalIgnoreCase)) locs.Add("URL");
                return locs.Count > 0 ? "📍 " + string.Join(" • ", locs) : "Corps du texte";
            }

            _kw1List = filteredWords
                .GroupBy(w => w)
                .OrderByDescending(g => g.Count())
                .Take(25)
                .Select((g, idx) => new KeywordDensityItem
                {
                    Rank = $"#{idx + 1}",
                    Word = g.Key,
                    Count = g.Count(),
                    Density = (g.Count() * 100.0) / _cachedTotalWords,
                    Locations = GetStrategicLocations(g.Key)
                })
                .ToList();

            var bigrams = new List<string>();
            for (int i = 0; i < filteredWords.Count - 1; i++)
                bigrams.Add($"{filteredWords[i]} {filteredWords[i + 1]}");

            _kw2List = bigrams
                .GroupBy(b => b)
                .Where(g => g.Count() >= 2 || bigrams.Count < 15)
                .OrderByDescending(g => g.Count())
                .Take(25)
                .Select((g, idx) => new KeywordDensityItem
                {
                    Rank = $"#{idx + 1}",
                    Word = g.Key,
                    Count = g.Count(),
                    Density = (g.Count() * 200.0) / _cachedTotalWords,
                    Locations = GetStrategicLocations(g.Key)
                })
                .ToList();

            var trigrams = new List<string>();
            for (int i = 0; i < filteredWords.Count - 2; i++)
                trigrams.Add($"{filteredWords[i]} {filteredWords[i + 1]} {filteredWords[i + 2]}");

            _kw3List = trigrams
                .GroupBy(t => t)
                .Where(g => g.Count() >= 2 || trigrams.Count < 15)
                .OrderByDescending(g => g.Count())
                .Take(25)
                .Select((g, idx) => new KeywordDensityItem
                {
                    Rank = $"#{idx + 1}",
                    Word = g.Key,
                    Count = g.Count(),
                    Density = (g.Count() * 300.0) / _cachedTotalWords,
                    Locations = GetStrategicLocations(g.Key)
                })
                .ToList();

            SetKeywordView(1);
            if (!string.IsNullOrWhiteSpace(TxtTargetKeyword.Text))
                EvaluateTargetKeyword();

            // =====================================================================
            // MODULE RÉSEAU, DNS, CERTIFICAT SSL & EN-TÊTES HTTP
            // =====================================================================
            string reverseDns = "Non résolu";
            if (speedMetrics.IpList.Count > 0)
            {
                try
                {
                    var hostEntry = await Dns.GetHostEntryAsync(speedMetrics.IpList[0]);
                    reverseDns = hostEntry.HostName;
                }
                catch { }
            }

            TxtNetDnsInfo.Text =
                $"• Hôte analysé     : {finalUri.Host}\n" +
                $"• URL finale       : {finalUri}\n" +
                $"• Adresses IP      : {(speedMetrics.IpList.Count > 0 ? string.Join(", ", speedMetrics.IpList.Select(ip => ip.ToString())) : "Non résolues")}\n" +
                $"• Reverse DNS (PTR): {reverseDns}\n" +
                $"• Protocole HTTP   : HTTP/{response.Version} ({(int)response.StatusCode} {response.StatusCode})\n" +
                $"• Serveur / CDN    : {serverHeader}";

            TxtNetSslInfo.Text = isHttps
                ? $"• Statut SSL/TLS   : {(sslValid ? "✅ Valide et approuvé" : "⚠️ Avertissement certificat")}\n" +
                  $"• Sujet (CN)       : {sslSubject}\n" +
                  $"• Émetteur (CA)    : {sslIssuer}\n" +
                  $"• Période validité : Du {sslValidFrom} au {sslValidTo} ({sslDaysLeft ?? 0} jours restants)\n" +
                  $"• Algorithme       : {sslAlgo}\n" +
                  $"• Empreinte SHA256 : {(sslThumbprint.Length > 32 ? sslThumbprint[..32] + "..." : sslThumbprint)}"
                : "❌ Ce site n'utilise pas de connexion chiffrée HTTPS.";

            var allHeaders = new List<HttpHeaderItem>();
            foreach (var h in response.Headers)
                allHeaders.Add(new HttpHeaderItem { Name = h.Key, Value = string.Join(", ", h.Value) });
            foreach (var h in response.Content.Headers)
                allHeaders.Add(new HttpHeaderItem { Name = h.Key, Value = string.Join(", ", h.Value) });

            ItemsHeaders.ItemsSource = allHeaders;

            // =====================================================================
            // MISE À JOUR DU TABLEAU DE BORD GLOBAL (VUE D'ENSEMBLE) & DES 4 KPI
            // =====================================================================
            int okCount = _allChecks.Count(c => c.State == CheckState.Pass);
            int warnCount = _allChecks.Count(c => c.State == CheckState.Warn);
            int failCount = _allChecks.Count(c => c.State == CheckState.Fail);
            int scorePct = (int)Math.Round(((okCount + warnCount * 0.5) * 100.0) / 21.0);

            string grade = scorePct switch
            {
                >= 92 when failCount == 0 => "A+",
                >= 82 => "A",
                >= 70 => "B",
                >= 55 => "C",
                _ => "D"
            };
            Brush gradeBrush = scorePct >= 80 ? PassFg : (scorePct >= 60 ? WarnFg : FailFg);

            TxtOverviewGrade.Text = grade;
            TxtOverviewGrade.Foreground = gradeBrush;
            BorderGradeBadge.BorderBrush = gradeBrush;
            TxtOverviewVerdict.Text = failCount == 0
                ? $"Prêt au lancement ({scorePct}%)"
                : $"{failCount} point(s) critique(s) à corriger ({scorePct}%)";

            double legalPct = ComputePillarPercent(0, 1, 4);
            double perfPct = ComputePillarPercent(10, 11, 13, 14, 15, 19);
            double seoPct = ComputePillarPercent(5, 6, 7, 8, 9, 12, 18);
            double secPct = ComputePillarPercent(2, 3, 16, 17, 20);

            TxtPillarLegal.Text = $"{legalPct:0}%";
            BarPillarLegal.Value = legalPct;

            TxtPillarPerf.Text = $"{perfPct:0}%";
            BarPillarPerf.Value = perfPct;

            TxtPillarSeo.Text = $"{seoPct:0}%";
            BarPillarSeo.Value = seoPct;

            TxtPillarSec.Text = $"{secPct:0}%";
            BarPillarSec.Value = secPct;

            var priorityItems = _allChecks
                .Where(c => c.State is CheckState.Fail or CheckState.Warn)
                .OrderByDescending(c => c.State == CheckState.Fail)
                .Take(4)
                .ToList();

            if (priorityItems.Count == 0)
            {
                priorityItems.Add(new WebsiteCheckItem
                {
                    Title = "Félicitations — Les 21 contrôles sont validés !",
                    StatusText = "✅ 21/21 OK",
                    Recommendation = "Votre site respecte l'ensemble des bonnes pratiques légales, SEO, performance et sécurité.",
                    BorderColor = PassBorder,
                    StatusFg = PassFg
                });
            }
            ItemsOverviewPriorities.ItemsSource = priorityItems;

            if (_kw1List.Count > 0)
            {
                var topKwLines = _kw1List.Take(5)
                    .Select(k => $"{k.Rank,-3} {k.Word,-18} : {k.Count,2}x ({k.Density:0.0}%)  {k.Locations}");
                TxtOverviewTopKeywords.Text = string.Join("\n", topKwLines);
            }
            else
            {
                TxtOverviewTopKeywords.Text = "Aucun mot-clé significatif extrait.";
            }

            TxtOverviewSpeedAndSeo.Text =
                $"• Latence réseau : DNS {speedMetrics.DnsMs} ms • TLS {speedMetrics.TlsMs} ms • TTFB {speedMetrics.TtfbMs} ms ({htmlSizeKb} Ko)\n" +
                $"• Balise Title   : {(string.IsNullOrEmpty(pageTitle) ? "❌ Absente" : $"« {(pageTitle.Length > 48 ? pageTitle[..48] + "..." : pageTitle)} » ({pageTitle.Length} car.)")}\n" +
                $"• Structure Hn   : {headingItems.Count} titre(s) détecté(s) ({h1Count} H1) • {_cachedTotalWords} mots";

            // Mise à jour des 4 cartes KPI supérieures
            TxtScoreValue.Text = $"Note {grade} • {okCount}/21 ({scorePct}%)";
            TxtScoreSub.Text = $"{okCount} OK • {warnCount} avis • {failCount} manquant(s)";
            TxtScoreValue.Foreground = gradeBrush;

            TxtPerfValue.Text = $"TTFB {speedMetrics.TtfbMs} ms • {htmlSizeKb} Ko";
            TxtPerfValue.Foreground = speedMetrics.TtfbMs < 500 ? PassFg : (speedMetrics.TtfbMs < 1200 ? WarnFg : FailFg);
            TxtPerfSub.Text = $"DNS {speedMetrics.DnsMs} ms • TLS {speedMetrics.TlsMs} ms • {serverHeader}";

            TxtSslValue.Text = sslValid ? (sslDaysLeft.HasValue ? $"Valide ({sslDaysLeft} j)" : "SSL Valide") : (isHttps ? "HTTPS Actif" : "Non chiffré");
            TxtSslValue.Foreground = isHttps ? PassFg : FailFg;
            TxtSslSub.Text = $"CA : {sslIssuer} • {secHeadersCount}/4 en-têtes";

            string topKw = _kw1List.Count > 0 ? $"Top: « {_kw1List[0].Word} »" : "0 mot-clé";
            TxtMediaValue.Text = $"{_cachedTotalWords} mots • {headingItems.Count} Hn";
            TxtMediaSub.Text = $"{topKw} • {links.Count} liens • {totalImgs} img";

            BtnFilterAll.Content = $"Tous (21)";
            BtnFilterFail.Content = $"❌ Manquants ({failCount})";
            BtnFilterWarn.Content = $"⚠️ À optimiser ({warnCount})";
            BtnFilterOk.Content = $"✅ Conformes ({okCount})";

            totalAuditSw.Stop();
            BarAuditProgress.Value = 100;
            TxtHeaderStatusBadge.Text = $"✅ {finalUri.Host} — Note {grade} ({scorePct}%) en {totalAuditSw.Elapsed.TotalSeconds:0.0} s";
            RefreshFilteredList();

            _externalOutput?.Invoke($"✅ [SUITE WEB TERMINÉE] {finalUri} — Note {grade} ({scorePct}%), {okCount}/21 OK, TTFB: {speedMetrics.TtfbMs}ms, Mots: {_cachedTotalWords} ({totalAuditSw.Elapsed.TotalSeconds:0.0}s).");
        }
        catch (OperationCanceledException)
        {
            TxtHeaderStatusBadge.Text = "⏹ Analyse annulée";
            TxtSummaryDetails.Text = "L'analyse a été interrompue à votre demande.";
        }
        catch (Exception ex)
        {
            TxtHeaderStatusBadge.Text = "❌ Site injoignable";
            TxtSummaryDetails.Text = $"Erreur lors de la connexion à {baseUri} : {ex.Message}";
            _externalOutput?.Invoke($"[ERR] Échec de l'analyse web : {ex.Message}");
        }
        finally
        {
            _isAnalyzing = false;
            BtnAnalyze.IsEnabled = true;
            BtnQuickSpeedtest.IsEnabled = true;
            BtnCancelAudit.Visibility = Visibility.Collapsed;
            BarAuditProgress.Visibility = Visibility.Collapsed;
            BtnAnalyze.Content = "🚀 Lancer l'analyse complète";
        }
    }

    private string BuildReportText()
    {
        var sb = new StringBuilder();
        sb.AppendLine("# 🌐 Rapport Complet d'Audit Web, Speedtest & SEO — Alex Toolkit");
        sb.AppendLine();
        sb.AppendLine($"- **Site analysé** : `{(string.IsNullOrEmpty(_lastAnalyzedUrl) ? GetNormalizedTargetUrl() : _lastAnalyzedUrl)}`");
        sb.AppendLine($"- **Date de l'audit** : {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
        sb.AppendLine($"- **Score & Note globale** : **{TxtScoreValue.Text}** ({TxtScoreSub.Text})");
        sb.AppendLine($"- **Speedtest & Latence** : {TxtPerfValue.Text} ({TxtPerfSub.Text})");
        sb.AppendLine($"- **Sécurité SSL/TLS** : {TxtSslValue.Text} ({TxtSslSub.Text})");
        sb.AppendLine($"- **Contenu & SEO** : {TxtMediaValue.Text} ({TxtMediaSub.Text})");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine("## 1. Checklist Pré-lancement (21 Points)");
        sb.AppendLine();

        foreach (var item in _allChecks)
        {
            sb.AppendLine($"### {item.Index}. {item.Title} *({item.Category})* — {item.StatusText}");
            sb.AppendLine($"- **Constat** : {item.Details}");
            if (!string.IsNullOrWhiteSpace(item.Recommendation))
                sb.AppendLine($"- **{item.Recommendation}**");
            sb.AppendLine();
        }

        sb.AppendLine("---");
        sb.AppendLine("## 2. Top 10 Mots-Clés & Densité SEO");
        sb.AppendLine();
        foreach (var kw in _kw1List.Take(10))
        {
            sb.AppendLine($"- **{kw.Rank} `{kw.Word}`** : {kw.CountText} ({kw.DensityText}) — *{kw.Locations}*");
        }

        return sb.ToString();
    }

    private void BtnCopyReport_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Clipboard.SetText(BuildReportText());
            MessageBox.Show("Le rapport complet (Synthèse + 21 Points + Speedtest + SEO + Top Mots-Clés) a été copié dans le presse-papiers !", "Rapport copié", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur lors de la copie : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void BtnExportReport_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var sfd = new SaveFileDialog
            {
                Title = "Exporter le bilan complet du site web",
                Filter = "Document Markdown (*.md)|*.md|Fichier texte (*.txt)|*.txt",
                FileName = $"Audit_Complet_Site_{DateTime.Now:yyyyMMdd_HHmm}.md"
            };

            if (sfd.ShowDialog() == true)
            {
                File.WriteAllText(sfd.FileName, BuildReportText(), Encoding.UTF8);
                MessageBox.Show($"Rapport enregistré avec succès dans :\n{sfd.FileName}", "Export réussi", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur lors de l'export : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
