using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Win32;
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
        UpdateThemeState();
        TxtHierarchyPreview.Text = GenerateHierarchyText();
        _ = LoadSystemSpecsAsync();
    }

    public void ResetSections()
    {
    }

    // ─── Thème ────────────────────────────────────────────────────────
    private void BtnToggleTheme_Click(object sender, RoutedEventArgs e)
    {
        ThemeManager.NextTheme();
        UpdateThemeState();
    }

    private void UpdateThemeState()
    {
        var current = ThemeManager.CurrentTheme;
        TxtThemeIcon.Text = current.Icon;
        TxtThemeName.Text = current.Name;
        var nextName = current.Id == "light" ? "Noir Pur OLED" : "Clair Épuré";
        BtnToggleTheme.ToolTip = $"Thème actif : {current.Name}\nCliquez pour basculer vers {nextName}";
    }

    // ─── Spécifications PC (À propos) ─────────────────────────────────
    private async Task LoadSystemSpecsAsync()
    {
        await Task.Run(() =>
        {
            // Machine & Utilisateur
            string machine = $"{Environment.MachineName} (Session : {Environment.UserName})";

            // OS & Build
            var osVersion = Environment.OSVersion.Version;
            string osName = osVersion.Build >= 22000 ? "Windows 11" : "Windows 10";
            string osArch = Environment.Is64BitOperatingSystem ? "64 bits" : "32 bits";
            string os = $"{osName} (Build {osVersion.Build}, {osArch})";

            // CPU
            string cpu = "Processeur inconnu";
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0");
                cpu = key?.GetValue("ProcessorNameString")?.ToString()?.Trim() ?? "";
            }
            catch { }
            if (string.IsNullOrWhiteSpace(cpu))
                cpu = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER") ?? "Processeur";
            cpu += $" ({Environment.ProcessorCount} threads)";

            // RAM
            string ram = "Non disponible";
            try
            {
                var memStatus = new MEMORYSTATUSEX();
                memStatus.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
                if (GlobalMemoryStatusEx(memStatus))
                {
                    double totalGb = Math.Round(memStatus.ullTotalPhys / (1024.0 * 1024.0 * 1024.0), 1);
                    double freeGb = Math.Round(memStatus.ullAvailPhys / (1024.0 * 1024.0 * 1024.0), 1);
                    ram = $"{totalGb} Go ({freeGb} Go libres)";
                }
            }
            catch { }

            // Disque C:
            string disk = "Non disponible";
            try
            {
                var drive = new DriveInfo("C");
                if (drive.IsReady)
                {
                    double totalGb = Math.Round(drive.TotalSize / (1024.0 * 1024.0 * 1024.0), 1);
                    double freeGb = Math.Round(drive.TotalFreeSpace / (1024.0 * 1024.0 * 1024.0), 1);
                    disk = $"{freeGb} Go libres sur {totalGb} Go ({drive.DriveFormat})";
                }
            }
            catch { }

            // Privilèges & PowerShell
            bool isAdmin = false;
            try
            {
                using var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch { }

            string adminLabel = isAdmin ? "🛡️ Administrateur" : "Utilisateur standard";
            string ps = $"{adminLabel} • Windows PowerShell 5.1";

            Dispatcher.InvokeAsync(() =>
            {
                TxtSpecMachine.Text = machine;
                TxtSpecOS.Text = os;
                TxtSpecCPU.Text = cpu;
                TxtSpecRAM.Text = ram;
                TxtSpecDisk.Text = disk;
                TxtSpecPowerShell.Text = ps;
            });
        });
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private class MEMORYSTATUSEX
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

    // ─── Copie de l'arborescence ──────────────────────────────────────
    private void BtnCopyHierarchy_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var text = GenerateHierarchyText();
            Clipboard.SetText(text);
            ShowCopyFeedback("✅ Arborescence détaillée copiée dans le presse-papiers !");
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
    /// Génère la structure textuelle détaillée de toutes les catégories, sous-catégories et outils avec leurs commandes associées.
    /// </summary>
    public static string GenerateHierarchyText()
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== ALEX TOOLKIT — STRUCTURE DES MODULES, SCRIPTS & COMMANDES ===");
        sb.AppendLine();

        var categories = ToolRegistry.AllTools
            .GroupBy(t => t.Category)
            .OrderBy(cg => SectionViewBuilder.GetCategoryMeta(cg.Key).Name)
            .ToList();

        foreach (var catGroup in categories)
        {
            var (icon, name) = SectionViewBuilder.GetCategoryMeta(catGroup.Key);
            int totalTools = catGroup.Count();
            sb.AppendLine($"{icon} {name.ToUpperInvariant()} ({totalTools} outil{(totalTools > 1 ? "s" : "")})");

            var subGroups = catGroup
                .GroupBy(t => string.IsNullOrWhiteSpace(t.SubHeading) ? "Général" : SectionViewBuilder.FormatFrenchTitle(t.SubHeading))
                .OrderBy(sg => sg.Key);

            foreach (var sub in subGroups)
            {
                int count = sub.Count();
                sb.AppendLine($"   📂 {sub.Key} ({count} outil{(count > 1 ? "s" : "")})");
                foreach (var tool in sub.OrderBy(t => t.Title))
                {
                    string cmd = tool.ExecutableOrCommand;
                    sb.AppendLine($"      • {tool.Title}  →  [{cmd}]");
                }
            }

            sb.AppendLine();
        }

        return sb.ToString().TrimEnd();
    }

    /// <summary>
    /// Génère la structure au format Markdown avec listes détaillées des outils et leurs commandes.
    /// </summary>
    public static string GenerateMarkdownText()
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Alex Toolkit — Structure des modules, scripts & commandes");
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
            sb.AppendLine();

            var subGroups = catGroup
                .GroupBy(t => string.IsNullOrWhiteSpace(t.SubHeading) ? "Général" : SectionViewBuilder.FormatFrenchTitle(t.SubHeading))
                .OrderBy(sg => sg.Key);

            foreach (var sub in subGroups)
            {
                int count = sub.Count();
                sb.AppendLine($"### 📂 {sub.Key} ({count} outil{(count > 1 ? "s" : "")})");
                foreach (var tool in sub.OrderBy(t => t.Title))
                {
                    string cmd = tool.ExecutableOrCommand;
                    sb.AppendLine($"- **{tool.Title}** : `{cmd}`");
                }
                sb.AppendLine();
            }
        }

        return sb.ToString().TrimEnd();
    }
}
