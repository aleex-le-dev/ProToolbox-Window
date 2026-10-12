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

    // Couleurs de texte pour adapter tous les thèmes (sombre, clair, couleur)
    public required Color TextPrimary { get; init; }
    public required Color TextSecondary { get; init; }
    public required Color TextMuted { get; init; }

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
            Name = "Aero Glass",
            Icon = "💎",
            VisualStyle = "Verre fumé translucide, bordures givrées & reflets cristallins",
            BgDeep = Color.FromRgb(8, 13, 26),
            BgDeepGradientEnd = Color.FromRgb(21, 34, 60),
            BgSidebar = Color.FromArgb(195, 14, 22, 42),      // Translucide verre givré
            BgCard = Color.FromArgb(140, 28, 44, 76),         // Verre fumé translucide
            BgCardHover = Color.FromArgb(200, 40, 62, 104),
            Accent = Color.FromRgb(56, 189, 248),             // Cyan glacier éclatant
            AccentHover = Color.FromRgb(125, 211, 252),
            AccentDim = Color.FromArgb(110, 14, 165, 233),
            Border = Color.FromArgb(130, 147, 197, 253),      // Reflet givré blanc-bleuté
            BorderAccent = Color.FromRgb(96, 165, 250),
            TextPrimary = Color.FromRgb(248, 250, 252),
            TextSecondary = Color.FromRgb(203, 213, 225),
            TextMuted = Color.FromRgb(148, 163, 184),
            ShadowColor = Color.FromRgb(56, 189, 248),
            ShadowOpacity = 0.32,
            ShadowBlur = 20,
            CardBorderThickness = 1.2,
            TerminalBg = Color.FromArgb(240, 6, 10, 18),
            TerminalText = Color.FromRgb(186, 230, 253)
        },

        // 2. THÈME COULEUR (Vibrance violette, magenta & rose coucher de soleil)
        new AppTheme
        {
            Id = "color",
            Name = "Thème Couleur",
            Icon = "🌈",
            VisualStyle = "Palette riche améthyste, violet profond & halos fuchsia chaleureux",
            BgDeep = Color.FromRgb(24, 9, 43),                // Violet profond nuit
            BgDeepGradientEnd = Color.FromRgb(46, 8, 84),     // Améthyste impérial
            BgSidebar = Color.FromArgb(235, 31, 10, 56),      // Teinte violette chaude
            BgCard = Color.FromArgb(210, 45, 18, 77),         // Cartes violettes chatoyantes
            BgCardHover = Color.FromArgb(245, 66, 27, 110),
            Accent = Color.FromRgb(244, 63, 94),              // Rose corail vibrant
            AccentHover = Color.FromRgb(251, 113, 133),
            AccentDim = Color.FromArgb(130, 244, 63, 94),
            Border = Color.FromArgb(150, 168, 85, 247),       // Ligne violette lumineuse
            BorderAccent = Color.FromRgb(236, 72, 153),       // Fuchsia vif
            TextPrimary = Color.FromRgb(255, 241, 242),
            TextSecondary = Color.FromRgb(251, 207, 232),
            TextMuted = Color.FromRgb(192, 132, 252),
            ShadowColor = Color.FromRgb(236, 72, 153),
            ShadowOpacity = 0.40,
            ShadowBlur = 22,
            CardBorderThickness = 1.4,
            TerminalBg = Color.FromArgb(245, 21, 5, 36),
            TerminalText = Color.FromRgb(254, 215, 170)
        },

        // 3. CLAIR ÉPURÉ (Thème lumineux pur & contrasté)
        new AppTheme
        {
            Id = "light",
            Name = "Clair Épuré",
            Icon = "☀️",
            VisualStyle = "Interface blanche moderne, finitions sobres & contraste parfait",
            BgDeep = Color.FromRgb(241, 245, 249),            // Gris perle doux
            BgDeepGradientEnd = Color.FromRgb(226, 232, 240),
            BgSidebar = Color.FromRgb(255, 255, 255),         // Sidebar blanche pure
            BgCard = Color.FromRgb(255, 255, 255),            // Cartes blanches pures
            BgCardHover = Color.FromRgb(248, 250, 252),
            Accent = Color.FromRgb(37, 99, 235),              // Bleu royal moderne
            AccentHover = Color.FromRgb(29, 78, 216),
            AccentDim = Color.FromArgb(60, 37, 99, 235),
            Border = Color.FromRgb(203, 213, 225),            // Bordures douces
            BorderAccent = Color.FromRgb(59, 130, 246),
            TextPrimary = Color.FromRgb(15, 23, 42),          // Texte foncé très lisible
            TextSecondary = Color.FromRgb(51, 65, 85),
            TextMuted = Color.FromRgb(100, 116, 139),
            ShadowColor = Color.FromRgb(15, 23, 42),
            ShadowOpacity = 0.08,
            ShadowBlur = 10,
            CardBorderThickness = 1.0,
            TerminalBg = Color.FromRgb(15, 23, 42),           // Terminal foncé contrasté
            TerminalText = Color.FromRgb(248, 250, 252)
        },

        // 4. CYBERPUNK NÉON (Noir OLED & Halos Cyan/Rose)
        new AppTheme
        {
            Id = "neon",
            Name = "Cyberpunk Néon",
            Icon = "⚡",
            VisualStyle = "Noir carbone OLED, lueurs néon cyan & contrastes tranchants",
            BgDeep = Color.FromRgb(7, 6, 10),
            BgDeepGradientEnd = Color.FromRgb(21, 13, 36),
            BgSidebar = Color.FromArgb(240, 13, 11, 20),
            BgCard = Color.FromArgb(200, 24, 18, 38),
            BgCardHover = Color.FromArgb(245, 38, 27, 61),
            Accent = Color.FromRgb(6, 182, 212),              // Cyan électrique
            AccentHover = Color.FromRgb(103, 232, 249),
            AccentDim = Color.FromArgb(120, 6, 182, 212),
            Border = Color.FromArgb(160, 6, 182, 212),        // Bordure néon cyan
            BorderAccent = Color.FromRgb(244, 63, 94),        // Rose néon
            TextPrimary = Color.FromRgb(248, 250, 252),
            TextSecondary = Color.FromRgb(165, 243, 252),
            TextMuted = Color.FromRgb(103, 232, 249),
            ShadowColor = Color.FromRgb(6, 182, 212),
            ShadowOpacity = 0.45,
            ShadowBlur = 22,
            CardBorderThickness = 1.4,
            TerminalBg = Color.FromArgb(245, 5, 5, 8),
            TerminalText = Color.FromRgb(103, 232, 249)
        },

        // 5. SOMBRE PRO (Anthracite mat & Élégance discrète)
        new AppTheme
        {
            Id = "obsidian",
            Name = "Sombre Pro",
            Icon = "🌑",
            VisualStyle = "Anthracite mat professionnel, confort oculaire & précision",
            BgDeep = Color.FromRgb(15, 17, 23),
            BgDeepGradientEnd = Color.FromRgb(22, 25, 34),
            BgSidebar = Color.FromRgb(19, 22, 31),
            BgCard = Color.FromRgb(28, 32, 43),
            BgCardHover = Color.FromRgb(38, 43, 58),
            Accent = Color.FromRgb(99, 102, 241),
            AccentHover = Color.FromRgb(129, 140, 248),
            AccentDim = Color.FromArgb(90, 99, 102, 241),
            Border = Color.FromRgb(42, 48, 66),
            BorderAccent = Color.FromRgb(79, 70, 229),
            TextPrimary = Color.FromRgb(248, 250, 252),
            TextSecondary = Color.FromRgb(148, 163, 184),
            TextMuted = Color.FromRgb(100, 116, 139),
            ShadowColor = Color.FromRgb(0, 0, 0),
            ShadowOpacity = 0.35,
            ShadowBlur = 12,
            CardBorderThickness = 1.0,
            TerminalBg = Color.FromRgb(11, 12, 16),
            TerminalText = Color.FromRgb(226, 232, 240)
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

        // Couleurs de texte (indispensables pour thème clair et couleur)
        res["TextPrimaryColor"]   = theme.TextPrimary;
        res["TextSecondaryColor"] = theme.TextSecondary;
        res["TextMutedColor"]     = theme.TextMuted;

        // SolidColorBrushes dynamiques (Frozen = lecture seule, haute performance)
        res["BgDeep"]      = MakeBrush(theme.BgDeep);
        res["BgSidebar"]   = MakeBrush(theme.BgSidebar);
        res["BgCard"]      = MakeBrush(theme.BgCard);
        res["BgCardHover"] = MakeBrush(theme.BgCardHover);
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
