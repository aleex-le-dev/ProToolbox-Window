using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ScriptsWindows.Core;

public class AppTheme
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Icon { get; init; }
    public required string VisualStyle { get; init; } // Description du rendu visuel

    // Couleurs de fond
    public required Color BgDeep { get; init; }
    public required Color BgDeepGradientEnd { get; init; }
    public required Color BgSidebar { get; init; }
    public required Color BgCard { get; init; }
    public required Color BgCardHover { get; init; }
    public required Color BgInput { get; init; }

    // Couleurs d'accent et bordures
    public required Color Accent { get; init; }
    public required Color AccentHover { get; init; }
    public required Color AccentDim { get; init; }
    public required Color Border { get; init; }
    public required Color BorderAccent { get; init; }

    // Couleurs de texte pour adapter tous les thèmes
    public required Color TextPrimary { get; init; }
    public required Color TextSecondary { get; init; }
    public required Color TextMuted { get; init; }

    // Effets visuels des cartes
    public required Color ShadowColor { get; init; }
    public required double ShadowOpacity { get; init; }
    public required double ShadowBlur { get; init; }
    public required double CardBorderThickness { get; init; }

    // Terminal
    public required Color TerminalBg { get; init; }
    public required Color TerminalText { get; init; }
}

public static class ThemeManager
{
    public static readonly List<AppTheme> AvailableThemes = new()
    {
        // 1. NOIR PUR OLED (PAR DÉFAUT - Vrai noir absolu #000000)
        new AppTheme
        {
            Id = "obsidian",
            Name = "Noir pur oled",
            Icon = "🌙",
            VisualStyle = "Noir absolu profond (#000000), contrastes tranchants et pureté OLED",
            BgDeep = Color.FromRgb(0, 0, 0),                 // Noir absolu pur 0,0,0
            BgDeepGradientEnd = Color.FromRgb(2, 2, 4),      // Noir profond
            BgSidebar = Color.FromRgb(6, 6, 8),              // Noir pur sidebar
            BgCard = Color.FromRgb(14, 14, 18),              // Noir intense contrasté
            BgCardHover = Color.FromRgb(24, 24, 30),
            BgInput = Color.FromRgb(24, 24, 30),             // Fond des badges
            Accent = Color.FromRgb(99, 102, 241),
            AccentHover = Color.FromRgb(129, 140, 248),
            AccentDim = Color.FromArgb(80, 99, 102, 241),
            Border = Color.FromRgb(32, 32, 42),              // Filet sombre discret
            BorderAccent = Color.FromRgb(99, 102, 241),
            TextPrimary = Color.FromRgb(255, 255, 255),      // Blanc pur éclatant sur fond noir
            TextSecondary = Color.FromRgb(160, 165, 178),
            TextMuted = Color.FromRgb(100, 105, 118),
            ShadowColor = Color.FromRgb(0, 0, 0),
            ShadowOpacity = 0.70,
            ShadowBlur = 12,
            CardBorderThickness = 1.0,
            TerminalBg = Color.FromRgb(0, 0, 0),             // Terminal noir pur 0,0,0
            TerminalText = Color.FromRgb(245, 245, 250)
        },

        // 2. CLAIR ÉPURÉ
        new AppTheme
        {
            Id = "light",
            Name = "Clair épuré",
            Icon = "☀️",
            VisualStyle = "Interface blanche moderne, finitions sobres et contraste parfait",
            BgDeep = Color.FromRgb(248, 250, 252),            // Blanc gris très doux (Slate 50)
            BgDeepGradientEnd = Color.FromRgb(241, 245, 249), // Slate 100
            BgSidebar = Color.FromRgb(255, 255, 255),         // Blanc pur
            BgCard = Color.FromRgb(255, 255, 255),            // Blanc pur
            BgCardHover = Color.FromRgb(241, 245, 249),       // Survol subtil
            BgInput = Color.FromRgb(241, 245, 249),           // Fond des badges
            Accent = Color.FromRgb(37, 99, 235),              // Bleu royal moderne
            AccentHover = Color.FromRgb(29, 78, 216),
            AccentDim = Color.FromArgb(40, 37, 99, 235),      // Voile bleuté léger
            Border = Color.FromRgb(226, 232, 240),            // Bordures douces
            BorderAccent = Color.FromRgb(59, 130, 246),
            TextPrimary = Color.FromRgb(15, 23, 42),          // Texte principal foncé très lisible
            TextSecondary = Color.FromRgb(71, 85, 105),       // Texte secondaire
            TextMuted = Color.FromRgb(148, 163, 184),         // Texte atténué
            ShadowColor = Color.FromRgb(15, 23, 42),
            ShadowOpacity = 0.06,
            ShadowBlur = 10,
            CardBorderThickness = 1.0,
            TerminalBg = Color.FromRgb(15, 23, 42),           // Terminal foncé contrasté
            TerminalText = Color.FromRgb(248, 250, 252)
        }
    };

    private static int _currentIndex = 0; // Démarre sur 0 = Noir Pur OLED
    private static string? _lastAppliedThemeId;

    public static event EventHandler<AppTheme>? ThemeChanged;

    public static AppTheme CurrentTheme => AvailableThemes[_currentIndex];

    public static AppTheme NextTheme()
    {
        _currentIndex = (_currentIndex + 1) % AvailableThemes.Count;
        ApplyTheme(CurrentTheme);
        return CurrentTheme;
    }

    public static void ApplyTheme(AppTheme theme)
    {
        if (_lastAppliedThemeId == theme.Id) return;
        _lastAppliedThemeId = theme.Id;

        try
        {
            ModernWpf.ThemeManager.Current.ApplicationTheme = theme.Id == "light"
                ? ModernWpf.ApplicationTheme.Light
                : ModernWpf.ApplicationTheme.Dark;
        }
        catch { }

        var res = Application.Current.Resources;

        // Couleurs fondamentales
        res["BgDeepColor"]      = theme.BgDeep;
        res["BgSidebarColor"]   = theme.BgSidebar;
        res["BgCardColor"]      = theme.BgCard;
        res["BgCardHoverColor"] = theme.BgCardHover;
        res["AccentColor"]      = theme.Accent;
        res["AccentHoverColor"] = theme.AccentHover;
        res["AccentDimColor"]   = theme.AccentDim;
        res["BorderColor"]      = theme.Border;
        res["BorderAccentColor"]= theme.BorderAccent;

        // Couleurs de texte
        res["TextPrimaryColor"]   = theme.TextPrimary;
        res["TextSecondaryColor"] = theme.TextSecondary;
        res["TextMutedColor"]     = theme.TextMuted;

        // SolidColorBrushes dynamiques (Frozen = lecture seule, haute performance)
        res["BgDeep"]      = MakeBrush(theme.BgDeep);
        res["BgSidebar"]   = MakeBrush(theme.BgSidebar);
        res["BgCard"]      = MakeBrush(theme.BgCard);
        res["BgCardHover"] = MakeBrush(theme.BgCardHover);
        res["BgInput"]     = MakeBrush(theme.BgInput);
        res["Accent"]      = MakeBrush(theme.Accent);
        res["AccentHover"] = MakeBrush(theme.AccentHover);
        res["AccentDim"]   = MakeBrush(theme.AccentDim);
        res["Border"]      = MakeBrush(theme.Border);
        res["BorderAccent"]= MakeBrush(theme.BorderAccent);

        res["TextPrimary"]   = MakeBrush(theme.TextPrimary);
        res["TextSecondary"] = MakeBrush(theme.TextSecondary);
        res["TextMuted"]     = MakeBrush(theme.TextMuted);

        // Effets visuels des cartes
        res["CardShadowColor"]      = theme.ShadowColor;
        res["CardShadowOpacity"]    = theme.ShadowOpacity;
        res["CardShadowBlur"]       = theme.ShadowBlur;
        res["CardBorderThickness"]  = new Thickness(theme.CardBorderThickness);

        // Dégradé atmosphérique de fond
        res["WindowGradientBrush"] = MakeGradient(
            new Point(0, 0), new Point(1, 1),
            (theme.BgDeep, 0.0),
            (theme.BgDeepGradientEnd, 1.0));

        // Dégradé du header
        res["HeaderGradient"] = MakeGradient(
            new Point(0, 0), new Point(1, 0),
            (theme.BgSidebar, 0.0),
            (theme.BgDeep, 1.0));

        // Dégradé d'accent
        res["AccentGradient"] = MakeGradient(
            new Point(0, 0), new Point(1, 1),
            (theme.Accent, 0.0),
            (theme.AccentHover, 1.0));

        // Terminal
        res["TerminalBg"]   = MakeBrush(theme.TerminalBg);
        res["TerminalText"] = MakeBrush(theme.TerminalText);

        ThemeChanged?.Invoke(null, theme);
    }

    private static SolidColorBrush MakeBrush(Color color)
    {
        var b = new SolidColorBrush(color);
        b.Freeze();
        return b;
    }

    private static LinearGradientBrush MakeGradient(Point start, Point end, params (Color color, double offset)[] stops)
    {
        var grad = new LinearGradientBrush { StartPoint = start, EndPoint = end };
        foreach (var (color, offset) in stops)
            grad.GradientStops.Add(new GradientStop(color, offset));
        grad.Freeze();
        return grad;
    }
}
