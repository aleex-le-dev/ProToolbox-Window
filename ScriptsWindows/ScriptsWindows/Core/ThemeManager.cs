using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace ScriptsWindows.Core;

/// <summary>
/// Définition d'un thème : un nom et une palette de jetons de couleur.
/// Chaque jeton « X » est publié dans les ressources de l'application sous deux clés :
/// « XColor » (Color) et « X » (SolidColorBrush figé).
/// </summary>
public class AppTheme
{
    public required string Id { get; init; }
    public required string Name { get; init; }

    /// <summary>Glyphe Segoe Fluent Icons / MDL2 représentant le thème.</summary>
    public required string Icon { get; init; }

    public required string VisualStyle { get; init; }
    public required bool IsDark { get; init; }
    public required IReadOnlyDictionary<string, Color> Colors { get; init; }
}

public static class ThemeManager
{
    private static Color Hex(string hex) => (Color)ColorConverter.ConvertFromString(hex);

    public static readonly List<AppTheme> AvailableThemes = new()
    {
        // ── Sombre (par défaut) : gris neutres, accent bleu sobre ──
        new AppTheme
        {
            Id = "dark",
            Name = "Sombre",
            Icon = "", // QuietHours (lune)
            VisualStyle = "Gris neutres profonds, contraste confortable",
            IsDark = true,
            Colors = new Dictionary<string, Color>
            {
                ["BgDeep"]        = Hex("#0F1115"),
                ["BgSidebar"]     = Hex("#14161B"),
                ["BgCard"]        = Hex("#1A1D23"),
                ["BgCardHover"]   = Hex("#20242B"),
                ["BgInput"]       = Hex("#23272E"),
                ["BgSurface"]     = Hex("#23272E"),
                ["Border"]        = Hex("#2A2E36"),
                ["BorderStrong"]  = Hex("#3A3F48"),
                ["BorderAccent"]  = Hex("#3B82F6"),
                ["Accent"]        = Hex("#3B82F6"),
                ["AccentHover"]   = Hex("#5B95F8"),
                ["AccentPressed"] = Hex("#2F6FD8"),
                ["AccentDim"]     = Hex("#263B82F6"),
                ["AccentText"]    = Hex("#7EB0FF"),
                ["OnAccent"]      = Hex("#FFFFFF"),
                ["Success"]       = Hex("#2EA043"),
                ["SuccessText"]   = Hex("#56D364"),
                ["SuccessBg"]     = Hex("#262EA043"),
                ["Warning"]       = Hex("#BB8009"),
                ["WarningText"]   = Hex("#E3B341"),
                ["WarningBg"]     = Hex("#26BB8009"),
                ["Danger"]        = Hex("#DA3633"),
                ["DangerText"]    = Hex("#FF7B72"),
                ["DangerBg"]      = Hex("#26DA3633"),
                ["Info"]          = Hex("#3B82F6"),
                ["TextPrimary"]   = Hex("#E6E8EB"),
                ["TextSecondary"] = Hex("#A0A6AF"),
                ["TextMuted"]     = Hex("#6E7681"),
                ["TerminalBg"]    = Hex("#0B0D10"),
                ["TerminalText"]  = Hex("#D4D9DF"),
            }
        },

        // ── Clair : blanc cassé, bordures douces, accent bleu profond ──
        new AppTheme
        {
            Id = "light",
            Name = "Clair",
            Icon = "", // Brightness (soleil)
            VisualStyle = "Surfaces blanches, bordures douces, lisibilité maximale",
            IsDark = false,
            Colors = new Dictionary<string, Color>
            {
                ["BgDeep"]        = Hex("#F5F6F8"),
                ["BgSidebar"]     = Hex("#FBFBFC"),
                ["BgCard"]        = Hex("#FFFFFF"),
                ["BgCardHover"]   = Hex("#F6F7F9"),
                ["BgInput"]       = Hex("#F0F2F5"),
                ["BgSurface"]     = Hex("#F0F2F5"),
                ["Border"]        = Hex("#E2E5EA"),
                ["BorderStrong"]  = Hex("#C9CED6"),
                ["BorderAccent"]  = Hex("#2563EB"),
                ["Accent"]        = Hex("#2563EB"),
                ["AccentHover"]   = Hex("#1D4ED8"),
                ["AccentPressed"] = Hex("#1E40AF"),
                ["AccentDim"]     = Hex("#1A2563EB"),
                ["AccentText"]    = Hex("#1D4ED8"),
                ["OnAccent"]      = Hex("#FFFFFF"),
                ["Success"]       = Hex("#1A7F37"),
                ["SuccessText"]   = Hex("#1A7F37"),
                ["SuccessBg"]     = Hex("#1A1A7F37"),
                ["Warning"]       = Hex("#9A6700"),
                ["WarningText"]   = Hex("#9A6700"),
                ["WarningBg"]     = Hex("#1A9A6700"),
                ["Danger"]        = Hex("#CF222E"),
                ["DangerText"]    = Hex("#CF222E"),
                ["DangerBg"]      = Hex("#1ACF222E"),
                ["Info"]          = Hex("#2563EB"),
                ["TextPrimary"]   = Hex("#1F2328"),
                ["TextSecondary"] = Hex("#59636E"),
                ["TextMuted"]     = Hex("#8C959F"),
                ["TerminalBg"]    = Hex("#16191D"),
                ["TerminalText"]  = Hex("#D4D9DF"),
            }
        }
    };

    private static readonly string ThemeFile = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ScriptsWindows", "theme.txt");

    private static int _currentIndex = LoadSavedThemeIndex();
    private static string? _lastAppliedThemeId;

    public static event EventHandler<AppTheme>? ThemeChanged;

    public static AppTheme CurrentTheme => AvailableThemes[_currentIndex];

    public static AppTheme NextTheme()
    {
        _currentIndex = (_currentIndex + 1) % AvailableThemes.Count;
        SaveThemeIndex();
        ApplyTheme(CurrentTheme);
        return CurrentTheme;
    }

    public static void ApplyTheme(AppTheme theme)
    {
        if (_lastAppliedThemeId == theme.Id) return;
        _lastAppliedThemeId = theme.Id;

        // Contrôles ModernWpf (barres de titre, cases à cocher, défilement…) alignés sur le thème
        try
        {
            ModernWpf.ThemeManager.Current.ApplicationTheme = theme.IsDark
                ? ModernWpf.ApplicationTheme.Dark
                : ModernWpf.ApplicationTheme.Light;
            ModernWpf.ThemeManager.Current.AccentColor = theme.Colors["Accent"];
        }
        catch { }

        var res = Application.Current.Resources;

        foreach (var (token, color) in theme.Colors)
        {
            res[token + "Color"] = color;
            res[token] = MakeBrush(color);
        }

        // Clés historiques (anciennement des dégradés, désormais à plat)
        res["WindowGradientBrush"] = MakeBrush(theme.Colors["BgDeep"]);
        res["HeaderGradient"]      = MakeBrush(theme.Colors["BgDeep"]);
        res["AccentGradient"]      = MakeBrush(theme.Colors["Accent"]);

        // Ombre des cartes
        res["CardShadowColor"]   = theme.IsDark ? Colors.Black : Hex("#1F2328");
        res["CardShadowOpacity"] = theme.IsDark ? 0.25 : 0.06;

        ThemeChanged?.Invoke(null, theme);
    }

    /// <summary>Pinceau du thème courant (pour le code-behind) ; repli si la clé est absente.</summary>
    public static Brush GetBrush(string key, Brush? fallback = null)
        => Application.Current?.TryFindResource(key) as Brush ?? fallback ?? Brushes.Transparent;

    private static int LoadSavedThemeIndex()
    {
        try
        {
            if (File.Exists(ThemeFile))
            {
                var id = File.ReadAllText(ThemeFile).Trim();
                int idx = AvailableThemes.FindIndex(t => t.Id == id);
                if (idx >= 0) return idx;
            }
        }
        catch { }
        return 0;
    }

    private static void SaveThemeIndex()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ThemeFile)!);
            File.WriteAllText(ThemeFile, CurrentTheme.Id);
        }
        catch { }
    }

    private static SolidColorBrush MakeBrush(Color color)
    {
        var b = new SolidColorBrush(color);
        b.Freeze();
        return b;
    }
}
