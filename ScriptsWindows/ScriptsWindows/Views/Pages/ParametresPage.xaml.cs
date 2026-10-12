using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ScriptsWindows.Core;
using ScriptsWindows.Views;

namespace ScriptsWindows.Views.Pages;

public partial class ParametresPage : Page, ISectionPage
{
    private DispatcherTimer? _feedbackTimer;

    public ParametresPage()
    {
        InitializeComponent();
        Loaded += ParametresPage_Loaded;
    }

    private void ParametresPage_Loaded(object sender, RoutedEventArgs e)
    {
        TxtHierarchyPreview.Text = GenerateHierarchyText();
    }

    public void ResetSections()
    {
        // Pas de sous-sections à replier sur cette page
    }

    private void BtnCopyHierarchy_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var text = GenerateHierarchyText();
            Clipboard.SetText(text);
            ShowCopyFeedback("✅ Liste copiée dans le presse-papiers !");
        }
        catch (Exception ex)
        {
            ShowCopyFeedback($"❌ Erreur de copie : {ex.Message}");
        }
    }

    private void BtnCopyMarkdown_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var md = GenerateMarkdownText();
            Clipboard.SetText(md);
            ShowCopyFeedback("✅ Liste Markdown copiée dans le presse-papiers !");
        }
        catch (Exception ex)
        {
            ShowCopyFeedback($"❌ Erreur de copie : {ex.Message}");
        }
    }

    private void ShowCopyFeedback(string message)
    {
        TxtCopyFeedback.Text = message;
        CopyFeedbackBorder.Visibility = Visibility.Visible;

        _feedbackTimer?.Stop();
        _feedbackTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2.5) };
        _feedbackTimer.Tick += (_, _) =>
        {
            _feedbackTimer?.Stop();
            CopyFeedbackBorder.Visibility = Visibility.Collapsed;
        };
        _feedbackTimer.Start();
    }

    /// <summary>
    /// Génère la structure textuelle propre et claire de toutes les catégories et sous-catégories.
    /// </summary>
    public static string GenerateHierarchyText()
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== ALEX TOOLKIT — STRUCTURE DES CATÉGORIES & SOUS-CATÉGORIES ===");
        sb.AppendLine();

        var categories = ToolRegistry.AllTools
            .GroupBy(t => t.Category)
            .OrderBy(cg => SectionViewBuilder.GetCategoryMeta(cg.Key).Name)
            .ToList();

        foreach (var catGroup in categories)
        {
            var (icon, name) = SectionViewBuilder.GetCategoryMeta(catGroup.Key);
            int totalTools = catGroup.Count();
            sb.AppendLine($"{icon} {name} ({totalTools} outil{(totalTools > 1 ? "s" : "")})");

            var subGroups = catGroup
                .GroupBy(t => string.IsNullOrWhiteSpace(t.SubHeading) ? "Général" : SectionViewBuilder.FormatFrenchTitle(t.SubHeading))
                .OrderBy(sg => sg.Key);

            foreach (var sub in subGroups)
            {
                int count = sub.Count();
                sb.AppendLine($"   • {sub.Key} ({count} outil{(count > 1 ? "s" : "")})");
            }

            sb.AppendLine();
        }

        return sb.ToString().TrimEnd();
    }

    /// <summary>
    /// Génère la structure au format Markdown avec listes à puces.
    /// </summary>
    public static string GenerateMarkdownText()
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Alex Toolkit — Structure des modules");
        sb.AppendLine();

        var categories = ToolRegistry.AllTools
            .GroupBy(t => t.Category)
            .OrderBy(cg => SectionViewBuilder.GetCategoryMeta(cg.Key).Name)
            .ToList();

        foreach (var catGroup in categories)
        {
            var (icon, name) = SectionViewBuilder.GetCategoryMeta(catGroup.Key);
            int totalTools = catGroup.Count();
            sb.AppendLine($"## {icon} {name} ({totalTools} outil{(totalTools > 1 ? "s" : "")})");

            var subGroups = catGroup
                .GroupBy(t => string.IsNullOrWhiteSpace(t.SubHeading) ? "Général" : SectionViewBuilder.FormatFrenchTitle(t.SubHeading))
                .OrderBy(sg => sg.Key);

            foreach (var sub in subGroups)
            {
                int count = sub.Count();
                sb.AppendLine($"- **{sub.Key}** ({count} outil{(count > 1 ? "s" : "")})");
            }

            sb.AppendLine();
        }

        return sb.ToString().TrimEnd();
    }
}
