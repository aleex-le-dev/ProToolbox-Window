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

    // Couleurs d'accent et bordures
    public required Color Accent { get; init; }
    public required Color AccentHover { get; init; }
    public required Color AccentDim { get; init; }
    public required Color Border { get; init; }
    public required Color BorderAccent { get; init; }

    // Effets visuels (Glassmorphism & Neon Glow)
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
        // 1. AERO GLASS (Transparence givrée & Reflets bleutés)
        new AppTheme
        {
            Id = "glass",
            Name = "Aero Glass (Translucide)",
            Icon = "💎",
            VisualStyle = "Verre fumé translucide & reflets givrés",
            BgDeep = Color.FromRgb(10, 14, 26),
            BgDeepGradientEnd = Color.FromRgb(24, 30, 56),
            BgSidebar = Color.FromArgb(210, 15, 21, 38), // Translucide !
            BgCard = Color.FromArgb(160, 30, 41, 68),    // Verre translucide !
            BgCardHover = Color.FromArgb(220, 42, 58, 96),
            Accent = Color.FromRgb(99, 102, 241),
            AccentHover = Color.FromRgb(129, 140, 248),
            AccentDim = Color.FromArgb(120, 55, 48, 163),
            Border = Color.FromArgb(130, 129, 140, 248), // Bordure verre lumineuse
            BorderAccent = Color.FromRgb(129, 140, 248),
            ShadowColor = Color.FromRgb(99, 102, 241),
            ShadowOpacity = 0.35,
            ShadowBlur = 18,
            CardBorderThickness = 1.2,
            TerminalBg = Color.FromArgb(235, 8, 10, 18),
            TerminalText = Color.FromRgb(226, 232, 240)
        },

        // 2. CYBERPUNK NEON GLOW (Lueur néon rose/violette vive)
        new AppTheme
        {
            Id = "neon",
            Name = "Cyberpunk Neon",
            Icon = "⚡",
            VisualStyle = "Halo néon rose rubis & contrastes intenses",
            BgDeep = Color.FromRgb(13, 10, 18),
            BgDeepGradientEnd = Color.FromRgb(32, 14, 36),
            BgSidebar = Color.FromArgb(230, 20, 12, 28),
            BgCard = Color.FromArgb(190, 38, 18, 48),    // Translucide teinté magenta
            BgCardHover = Color.FromArgb(240, 58, 24, 74),
            Accent = Color.FromRgb(244, 63, 94),
            AccentHover = Color.FromRgb(251, 113, 133),
            AccentDim = Color.FromArgb(140, 225, 29, 72),
            Border = Color.FromArgb(160, 244, 63, 94),   // Lueur néon
            BorderAccent = Color.FromRgb(244, 63, 94),
            ShadowColor = Color.FromRgb(244, 63, 94),    // Glow rose néon sous les cartes !
            ShadowOpacity = 0.45,
            ShadowBlur = 20,
            CardBorderThickness = 1.5,
            TerminalBg = Color.FromArgb(245, 12, 8, 16),
            TerminalText = Color.FromRgb(254, 205, 211)
        },

        // 3. MATRIX PHOSPHOR (Vert Matrix avec transparence sombre)
        new AppTheme
        {
            Id = "matrix",
            Name = "Matrix Phosphor",
            Icon = "🟢",
            VisualStyle = "Ambiance terminal hacker & lueur émeraude",
            BgDeep = Color.FromRgb(6, 12, 9),
            BgDeepGradientEnd = Color.FromRgb(14, 28, 20),
            BgSidebar = Color.FromArgb(220, 9, 18, 13),
            BgCard = Color.FromArgb(170, 16, 36, 26),    // Vert sombre translucide
            BgCardHover = Color.FromArgb(230, 24, 54, 38),
            Accent = Color.FromRgb(16, 185, 129),
            AccentHover = Color.FromRgb(52, 211, 153),
            AccentDim = Color.FromArgb(120, 5, 150, 105),
            Border = Color.FromArgb(140, 52, 211, 153),  // Bordure néon verte
            BorderAccent = Color.FromRgb(16, 185, 129),
            ShadowColor = Color.FromRgb(16, 185, 129),   // Glow vert phosphore !
            ShadowOpacity = 0.40,
            ShadowBlur = 18,
            CardBorderThickness = 1.2,
            TerminalBg = Color.FromArgb(245, 5, 10, 7),
            TerminalText = Color.FromRgb(52, 211, 153)
        },

        // 4. NORDIC FROST (Verre arctique givré cyan)
        new AppTheme
        {
            Id = "frost",
            Name = "Nordic Frost",
            Icon = "❄️",
            VisualStyle = "Givre arctique & transparence bleutée",
            BgDeep = Color.FromRgb(8, 15, 28),
            BgDeepGradientEnd = Color.FromRgb(18, 32, 58),
            BgSidebar = Color.FromArgb(215, 12, 22, 40),
            BgCard = Color.FromArgb(150, 24, 44, 76),    // Verre polaire translucide
            BgCardHover = Color.FromArgb(210, 36, 64, 108),
            Accent = Color.FromRgb(56, 189, 248),
            AccentHover = Color.FromRgb(125, 211, 252),
            AccentDim = Color.FromArgb(120, 2, 132, 199),
            Border = Color.FromArgb(150, 125, 211, 252), // Reflet glacé
            BorderAccent = Color.FromRgb(56, 189, 248),
            ShadowColor = Color.FromRgb(56, 189, 248),
            ShadowOpacity = 0.35,
            ShadowBlur = 16,
            CardBorderThickness = 1.2,
            TerminalBg = Color.FromArgb(240, 6, 12, 22),
            TerminalText = Color.FromRgb(224, 242, 254)
        },

        // 5. OBSIDIAN GOLD (Noir OLED pur & Ardoise d'ingénieur)
        new AppTheme
        {
            Id = "obsidian",
            Name = "Obsidian Luxury",
            Icon = "👑",
            VisualStyle = "Noir OLED pur, biseau doré et haute précision",
            BgDeep = Color.FromRgb(10, 10, 11),
            BgDeepGradientEnd = Color.FromRgb(18, 17, 16),
            BgSidebar = Color.FromRgb(16, 16, 18),
            BgCard = Color.FromRgb(26, 26, 30),         // Solide ultra contrasté
            BgCardHover = Color.FromRgb(36, 35, 40),
            Accent = Color.FromRgb(245, 158, 11),
            AccentHover = Color.FromRgb(251, 191, 36),
            AccentDim = Color.FromArgb(100, 217, 119, 6),
            Border = Color.FromArgb(160, 245, 158, 11),  // Filet d'or
            BorderAccent = Color.FromRgb(245, 158, 11),
            ShadowColor = Color.FromRgb(245, 158, 11),
            ShadowOpacity = 0.20,
            ShadowBlur = 12,
            CardBorderThickness = 1.0,
            TerminalBg = Color.FromRgb(8, 8, 9),
            TerminalText = Color.FromRgb(254, 243, 199)
        }
    };

    private static int _currentIndex = 0;
    private static string? _lastAppliedThemeId;

    public static AppTheme CurrentTheme => AvailableThemes[_currentIndex];

    public static AppTheme NextTheme()
    {
        _currentIndex = (_currentIndex + 1) % AvailableThemes.Count;
        ApplyTheme(CurrentTheme);
        return CurrentTheme;
    }

    public static void ApplyTheme(AppTheme theme)
    {
        // Éviter de réappliquer le même thème inutilement
        if (_lastAppliedThemeId == theme.Id) return;
        _lastAppliedThemeId = theme.Id;

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

        // SolidColorBrushes dynamiques (Frozen = lecture seule, performant)
        res["BgDeep"]      = MakeBrush(theme.BgDeep);
        res["BgSidebar"]   = MakeBrush(theme.BgSidebar);
        res["BgCard"]      = MakeBrush(theme.BgCard);
        res["BgCardHover"] = MakeBrush(theme.BgCardHover);
        res["Accent"]      = MakeBrush(theme.Accent);
        res["AccentHover"] = MakeBrush(theme.AccentHover);
        res["AccentDim"]   = MakeBrush(theme.AccentDim);
        res["Border"]      = MakeBrush(theme.Border);
        res["BorderAccent"]= MakeBrush(theme.BorderAccent);

        // Effets visuels
        res["CardShadowColor"]      = theme.ShadowColor;
        res["CardShadowOpacity"]    = theme.ShadowOpacity;
        res["CardShadowBlur"]       = theme.ShadowBlur;
        res["CardBorderThickness"]  = new Thickness(theme.CardBorderThickness);

        // Dégradé de fond de fenêtre
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
