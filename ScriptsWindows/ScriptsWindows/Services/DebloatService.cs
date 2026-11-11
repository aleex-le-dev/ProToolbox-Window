using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using ScriptsWindows.Core;

namespace ScriptsWindows.Services;

/// <summary>Application du Store installée (candidate à la suppression).</summary>
public class AppxApp : INotifyPropertyChanged
{
    private bool _isSelected;
    public string Name { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Publisher { get; set; } = "";
    public string Version { get; set; } = "";
    public string FriendlyName { get; set; } = "";
    public bool IsRecommended { get; set; }
    public string Category { get; set; } = "";
    public string Subtitle => string.IsNullOrEmpty(Category) ? Name : $"{Category} • {Name}";
    public bool IsSelected { get => _isSelected; set { if (_isSelected == value) return; _isSelected = value; OnChanged(); } }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnChanged([CallerMemberName] string? n = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}

/// <summary>
/// Suppression des applications préinstallées inutiles (bloatware). Les composants système et utiles
/// ne sont jamais proposés ; les applications supprimées restent réinstallables depuis le Microsoft Store.
/// </summary>
public static class DebloatService
{
    // Bloatware connu : préfixe du nom de paquet → (nom lisible, catégorie)
    private static readonly (string Prefix, string Friendly, string Category)[] Known =
    {
        ("king.com.", "Jeux King (Candy Crush…)", "Jeu publicitaire"),
        ("Microsoft.MicrosoftSolitaireCollection", "Solitaire Collection", "Jeu publicitaire"),
        ("Microsoft.BingNews", "Actualités", "Microsoft"),
        ("Microsoft.BingWeather", "Météo", "Microsoft"),
        ("Microsoft.BingSearch", "Recherche Bing", "Microsoft"),
        ("Microsoft.GetHelp", "Obtenir de l'aide", "Microsoft"),
        ("Microsoft.Getstarted", "Astuces", "Microsoft"),
        ("Microsoft.WindowsFeedbackHub", "Hub de commentaires", "Microsoft"),
        ("Microsoft.MicrosoftOfficeHub", "Microsoft 365 (raccourci promotionnel)", "Microsoft"),
        ("Microsoft.People", "Contacts", "Microsoft"),
        ("Microsoft.ZuneMusic", "Lecteur multimédia", "Microsoft"),
        ("Microsoft.ZuneVideo", "Films et TV", "Microsoft"),
        ("Microsoft.Microsoft3DViewer", "Visionneuse 3D", "Microsoft"),
        ("Microsoft.MixedReality.Portal", "Portail de réalité mixte", "Microsoft"),
        ("Microsoft.SkypeApp", "Skype", "Microsoft"),
        ("Microsoft.WindowsMaps", "Cartes", "Microsoft"),
        ("Microsoft.Todos", "Microsoft To Do", "Microsoft"),
        ("Microsoft.PowerAutomateDesktop", "Power Automate", "Microsoft"),
        ("Microsoft.549981C3F5F10", "Cortana", "Microsoft"),
        ("Clipchamp.Clipchamp", "Clipchamp", "Microsoft"),
        ("MicrosoftTeams", "Teams (personnel)", "Microsoft"),
        ("MSTeams", "Teams (personnel)", "Microsoft"),
        ("Microsoft.YourPhone", "Mobile connecté", "Microsoft"),
        ("Microsoft.Wallet", "Portefeuille", "Microsoft"),
        ("Microsoft.Messaging", "Messagerie", "Microsoft"),
        ("Microsoft.OneConnect", "Forfaits mobiles", "Microsoft"),
        ("Microsoft.Print3D", "Print 3D", "Microsoft"),
        ("SpotifyAB.SpotifyMusic", "Spotify (préinstallé)", "Promotionnel"),
        ("Disney.", "Disney+", "Promotionnel"),
        ("BytedancePte.Ltd.TikTok", "TikTok", "Promotionnel"),
        ("Facebook.", "Facebook / Instagram", "Promotionnel"),
        ("AmazonVideo.PrimeVideo", "Prime Video", "Promotionnel"),
        ("5A894077.McAfeeSecurity", "McAfee (essai)", "Promotionnel"),
        ("4DF9E0F8.Netflix", "Netflix", "Promotionnel"),
        ("PricelinePartnerNetwork.Booking", "Booking.com", "Promotionnel"),
        ("7EE7776C.LinkedInforWindows", "LinkedIn", "Promotionnel"),
        ("Microsoft.Xbox.TCUI", "Xbox (composant)", "Jeux Xbox"),
        ("Microsoft.XboxApp", "Xbox Console Companion", "Jeux Xbox"),
        ("Microsoft.GamingApp", "Xbox", "Jeux Xbox"),
        ("Microsoft.XboxGamingOverlay", "Xbox Game Bar", "Jeux Xbox"),
    };

    // Ne jamais proposer (système, sécurité, outils utiles, dépendances)
    private static readonly string[] Protected =
    {
        "Microsoft.WindowsStore", "Microsoft.DesktopAppInstaller", "Microsoft.StorePurchaseApp", "Microsoft.SecHealthUI",
        "Microsoft.WindowsCalculator", "Microsoft.WindowsNotepad", "Microsoft.WindowsTerminal", "Microsoft.Paint",
        "Microsoft.Windows.Photos", "Microsoft.ScreenSketch", "Microsoft.WindowsCamera", "Microsoft.WindowsAlarms",
        "Microsoft.HEIFImageExtension", "Microsoft.HEVCVideoExtension", "Microsoft.VP9VideoExtensions", "Microsoft.WebMediaExtensions",
        "Microsoft.WebpImageExtension", "Microsoft.RawImageExtension", "Microsoft.AV1VideoExtension", "Microsoft.MPEG2VideoExtension",
        "Microsoft.VCLibs", "Microsoft.UI.Xaml", "Microsoft.NET", "Microsoft.WindowsAppRuntime", "Microsoft.Services.Store.Engagement",
        "Microsoft.Winget", "Microsoft.ApplicationCompatibilityEnhancements", "Microsoft.Windows.DevHome", "MicrosoftWindows.Client",
        "Microsoft.Windows.", "windows.", "Microsoft.AAD", "Microsoft.AccountsControl", "Microsoft.LockApp", "Microsoft.ECApp",
        "Microsoft.CredDialogHost", "Microsoft.MicrosoftEdge", "Microsoft.OneDriveSync", "NcsiUwpApp", "Microsoft.BioEnrollment",
        "Microsoft.XboxIdentityProvider", "Microsoft.XboxSpeechToTextOverlay", "Microsoft.Office.OneNote", "Microsoft.OutlookForWindows",
        "Microsoft.MicrosoftStickyNotes", "Microsoft.WindowsSoundRecorder", "Microsoft.Copilot", "Microsoft.Edge",
        "MicrosoftCorporationII.QuickAssist", "MicrosoftCorporationII.WinAppRuntime", "Microsoft.StartExperiencesApp",
        "Microsoft.LanguageExperiencePack", "Microsoft.Ink.Handwriting", "Microsoft.WidgetsPlatformRuntime", "MicrosoftWindows.",
        "WindowsWorkload.", "Microsoft.Winget.", "Microsoft.MicrosoftEdgeDevToolsClient", "Microsoft.Windows.CloudExperienceHost"
    };

    // Familles protégées par mot-clé : codecs, packs de langue / saisie, dépendances d'exécution
    private static readonly string[] ProtectedKeywords =
        { "VideoExtension", "ImageExtension", "AudioExtension", "LanguageExperience", "Handwriting", "Speech", "Runtime", "Framework" };

    private static bool IsProtected(string name)
        => Protected.Any(p => name.StartsWith(p, StringComparison.OrdinalIgnoreCase))
        || ProtectedKeywords.Any(k => name.Contains(k, StringComparison.OrdinalIgnoreCase));

    public static async Task<List<AppxApp>> GetAppsAsync(CancellationToken ct = default)
    {
        string json = await PowerShellRunner.RunAsync(@"
$list = Get-AppxPackage -ErrorAction SilentlyContinue | Where-Object { -not $_.IsFramework -and -not $_.NonRemovable -and $_.SignatureKind -ne 'System' } |
    ForEach-Object { [PSCustomObject]@{ Name = $_.Name; FullName = $_.PackageFullName; Publisher = (""$($_.Publisher)"" -replace '^CN=([^,]+).*$', '$1'); Version = ""$($_.Version)"" } }
ConvertTo-Json -InputObject @($list) -Compress
", ct);

        var raw = JsonSerializer.Deserialize<List<AppxApp>>(json.Trim()) ?? new();
        var result = new List<AppxApp>();
        foreach (var app in raw.GroupBy(a => a.Name).Select(g => g.First()))
        {
            if (IsProtected(app.Name)) continue;
            var known = Known.FirstOrDefault(k => app.Name.StartsWith(k.Prefix, StringComparison.OrdinalIgnoreCase));
            app.IsRecommended = known.Prefix != null;
            app.FriendlyName = known.Friendly ?? PrettyName(app.Name);
            app.Category = known.Category ?? (app.Publisher.Contains("Microsoft", StringComparison.OrdinalIgnoreCase) ? "Microsoft" : app.Publisher);
            app.IsSelected = app.IsRecommended;
            result.Add(app);
        }
        return result.OrderByDescending(a => a.IsRecommended).ThenBy(a => a.FriendlyName).ToList();
    }

    /// <summary>
    /// Nom lisible de repli : on retire l'éditeur (premier segment) et on sépare les mots.
    /// « Microsoft.BingWeather » → « Bing Weather », « PythonSoftwareFoundation.Python.3.12 » → « Python 3.12 ».
    /// </summary>
    public static string FriendlyName(string packageName)
    {
        var known = Known.FirstOrDefault(k => packageName.StartsWith(k.Prefix, StringComparison.OrdinalIgnoreCase));
        return known.Prefix != null ? known.Friendly : PrettyName(packageName);
    }

    private static string PrettyName(string name)
    {
        var parts = name.Split('.');
        var words = parts.Length > 1 ? parts.Skip(1).ToList() : parts.ToList();
        // Les segments numériques consécutifs forment une version : « 3 » + « 12 » → « 3.12 »
        var merged = new List<string>();
        foreach (var w in words)
        {
            if (merged.Count > 0 && w.All(char.IsDigit) && merged[^1].All(c => char.IsDigit(c) || c == '.'))
                merged[^1] += "." + w;
            else
                merged.Add(System.Text.RegularExpressions.Regex.Replace(w, "(?<=[a-z])(?=[A-Z0-9])", " "));
        }
        return string.Join(" ", merged).Trim();
    }

    /// <summary>Supprime les applications ; allUsers : tous les comptes + empêche la réinstallation pour les nouveaux comptes.</summary>
    public static Task RemoveAsync(IEnumerable<AppxApp> apps, bool allUsers, Action<string> log, CancellationToken ct = default)
    {
        string names = string.Join(",", apps.Select(a => "'" + a.Name.Replace("'", "''") + "'"));
        return PowerShellRunner.RunAsync($@"
$names = @({names})
$ok = 0; $ko = 0
foreach ($n in $names) {{
    try {{
        $pkgs = @(Get-AppxPackage -Name $n {(allUsers ? "-AllUsers" : "")} -ErrorAction SilentlyContinue)
        foreach ($p in $pkgs) {{ Remove-AppxPackage -Package $p.PackageFullName {(allUsers ? "-AllUsers" : "")} -ErrorAction Stop }}
        {(allUsers ? "Get-AppxProvisionedPackage -Online -ErrorAction SilentlyContinue | Where-Object { $_.DisplayName -eq $n } | ForEach-Object { Remove-AppxProvisionedPackage -Online -PackageName $_.PackageName -ErrorAction SilentlyContinue | Out-Null }" : "")}
        Write-Output ""[OK] $n supprimé""
        $ok++
    }} catch {{
        Write-Output ""[ERR] $n : $($_.Exception.Message)""
        $ko++
    }}
}}
Write-Output ""[i] $ok application(s) supprimée(s), $ko échec(s). Elles restent réinstallables depuis le Microsoft Store.""
", log, ct);
    }
}
