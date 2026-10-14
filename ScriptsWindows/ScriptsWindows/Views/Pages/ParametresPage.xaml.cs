using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
        ThemeManager.ThemeChanged += (_, _) => UpdateThemeState();
    }

    private void ParametresPage_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateThemeState();
        TxtHierarchyPreview.Text = GenerateHierarchyText();
        UpdateBuildInfo();
        _ = LoadSystemSpecsAsync();
    }

    private void UpdateBuildInfo()
    {
        try
        {
            var assembly = typeof(ParametresPage).Assembly;
            var ver = assembly.GetName().Version;
            string versionStr = ver != null ? $"v{ver.Major}.{ver.Minor}.{ver.Build}" : "v4.1.0";

            string buildDateStr = GetBuildDate(assembly);
            string rawLabel = TxtAppBuildInfo.Text;
            string prefix = !string.IsNullOrWhiteSpace(rawLabel) && rawLabel.Contains("...")
                ? rawLabel.Replace("...", "").Trim()
                : "Dernière mise à jour le :";

            TxtAppBuildInfo.Text = $"{prefix} {buildDateStr}";
            TxtSpecApp.Text = $"Alex Toolkit {versionStr} (.NET 8.0 Windows x64)";
        }
        catch
        {
            TxtAppBuildInfo.Text = $"Dernière mise à jour le : {DateTime.Now:dd/MM/yyyy}";
            TxtSpecApp.Text = "Alex Toolkit v4.1.0 (.NET 8.0 Windows x64)";
        }
    }

    private static string GetBuildDate(Assembly assembly)
    {
        try
        {
            var metaAttr = assembly
                .GetCustomAttributes(typeof(AssemblyMetadataAttribute), false)
                .OfType<AssemblyMetadataAttribute>()
                .FirstOrDefault(a => a.Key == "BuildTimestamp");

            if (!string.IsNullOrWhiteSpace(metaAttr?.Value) && DateTime.TryParse(metaAttr.Value, out var dt))
            {
                return dt.ToString("dd/MM/yyyy à HH:mm");
            }
        }
        catch { }

        try
        {
            var exePath = Environment.ProcessPath;
            if (!string.IsNullOrEmpty(exePath) && File.Exists(exePath))
            {
                return File.GetLastWriteTime(exePath).ToString("dd/MM/yyyy à HH:mm");
            }
        }
        catch { }

        return DateTime.Now.ToString("dd/MM/yyyy à HH:mm");
    }

    public void ResetSections()
    {
        TxtHierarchyPreview.Text = GenerateHierarchyText();
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
        var nextName = current.Id == "light" ? "Noir pur oled" : "Clair épuré";
        BtnToggleTheme.ToolTip = $"Thème actif : {current.Name}\nCliquez pour basculer vers {nextName}";
    }

    // ─── Spécifications PC (À propos) ─────────────────────────────────
    private DispatcherTimer? _specsFeedbackTimer;

    private async Task LoadSystemSpecsAsync()
    {
        await Task.Run(() =>
        {
            // Droits d'administration
            bool isAdmin = AdminElevation.IsAdmin();
            string privileges = isAdmin
                ? "Administrateur (Privilèges élevés ✔)"
                : "Utilisateur standard (Droits restreints)";

            // Machine et Utilisateur
            string machine = $"{Environment.MachineName} (Session : {Environment.UserName})";

            // OS et Build
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

            // RAM : Total uniquement (pas de RAM libre)
            string ram = "Non disponible";
            try
            {
                var memStatus = new MEMORYSTATUSEX();
                memStatus.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
                if (GlobalMemoryStatusEx(memStatus))
                {
                    double totalGb = Math.Round(memStatus.ullTotalPhys / (1024.0 * 1024.0 * 1024.0), 1);
                    int roundedGb = (int)Math.Round(totalGb);
                    ram = (Math.Abs(totalGb - roundedGb) < 0.3) ? $"{roundedGb} Go" : $"{totalGb} Go";
                }
            }
            catch { }

            // ─── Disques & Stockage (Inventaire complet avec SSD/HDD/autre et %) ───
            var diskItems = new List<(string drive, string type, int usedPct, double freeGb, double totalGb, string label)>();
            try
            {
                // Détection des types physiques (SSD, NVMe, HDD)
                var typeMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                try
                {
                    string psTypes = @"
$pDisks = @(Get-PhysicalDisk -ErrorAction SilentlyContinue)
Get-Volume | Where-Object { $_.DriveLetter } | ForEach-Object {
    $letter = $_.DriveLetter
    $part = Get-Partition -DriveLetter $letter -ErrorAction SilentlyContinue
    $type = 'Disque'
    if ($part) {
        $pd = $pDisks | Where-Object { $_.DeviceId -eq $part.DiskNumber }
        if ($pd) {
            if ($pd.BusType -eq 'NVMe') { $type = 'SSD (NVMe)' }
            elseif ($pd.MediaType) { $type = [string]$pd.MediaType }
            elseif ($pd.BusType) { $type = [string]$pd.BusType }
        }
    }
    ""$letter`t$type""
}
";
                    var psi = new ProcessStartInfo("powershell.exe")
                    {
                        Arguments = $"-NoProfile -NonInteractive -ExecutionPolicy Bypass -Command \"{psTypes.Replace("\"", "\\\"")}\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        StandardOutputEncoding = Encoding.UTF8
                    };
                    using var p = Process.Start(psi);
                    if (p != null)
                    {
                        string outTypes = p.StandardOutput.ReadToEnd();
                        p.WaitForExit(3000);
                        foreach (var line in outTypes.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            var parts = line.Split('\t');
                            if (parts.Length >= 2)
                                typeMap[parts[0].Trim()] = parts[1].Trim();
                        }
                    }
                }
                catch { }

                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (!drive.IsReady) continue;
                    if (drive.DriveType != DriveType.Fixed && drive.DriveType != DriveType.Removable) continue;

                    string letter = drive.Name.TrimEnd('\\').TrimEnd(':');
                    string type = typeMap.TryGetValue(letter, out var t) && !string.IsNullOrWhiteSpace(t)
                        ? t
                        : (drive.DriveType == DriveType.Removable ? "Amovible / USB" : "Disque");

                    double totalGb = Math.Round(drive.TotalSize / (1024.0 * 1024.0 * 1024.0), 1);
                    double freeGb  = Math.Round(drive.TotalFreeSpace / (1024.0 * 1024.0 * 1024.0), 1);
                    int usedPct    = drive.TotalSize > 0
                        ? (int)Math.Round(((double)(drive.TotalSize - drive.TotalFreeSpace) / drive.TotalSize) * 100)
                        : 0;
                    string label   = !string.IsNullOrWhiteSpace(drive.VolumeLabel) ? drive.VolumeLabel : "";

                    diskItems.Add(($"{letter}:", type, usedPct, freeGb, totalGb, label));
                }
            }
            catch { }

            Dispatcher.InvokeAsync(() =>
            {
                TxtSpecPrivileges.Text = privileges;
                TxtSpecMachine.Text = machine;
                TxtSpecOS.Text = os;
                TxtSpecCPU.Text = cpu;
                TxtSpecRAM.Text = ram;

                PanelDisksSpecs.Children.Clear();
                if (diskItems.Count == 0)
                {
                    PanelDisksSpecs.Children.Add(new TextBlock
                    {
                        Text = "Aucun disque détecté",
                        FontSize = 13,
                        Foreground = (Brush)FindResource("TextSecondary")
                    });
                }
                else
                {
                    for (int i = 0; i < diskItems.Count; i++)
                    {
                        var d = diskItems[i];
                        var row = new DockPanel
                        {
                            Margin = new Thickness(0, i == 0 ? 0 : 5, 0, 0),
                            LastChildFill = true
                        };

                        // 1. Lettre de lecteur
                        var tbLetter = new TextBlock
                        {
                            Text = d.drive,
                            FontWeight = FontWeights.Bold,
                            FontSize = 13,
                            Margin = new Thickness(0, 0, 7, 0),
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        tbLetter.SetResourceReference(TextBlock.ForegroundProperty, "TextPrimary");
                        DockPanel.SetDock(tbLetter, Dock.Left);
                        row.Children.Add(tbLetter);

                        // 2. Badge du type physique (SSD, SSD (NVMe), HDD, etc.)
                        var badgeType = new Border
                        {
                            CornerRadius = new CornerRadius(4),
                            Padding = new Thickness(6, 1.5, 6, 1.5),
                            Margin = new Thickness(0, 0, 8, 0),
                            VerticalAlignment = VerticalAlignment.Center,
                            BorderThickness = new Thickness(0.8)
                        };
                        badgeType.SetResourceReference(Border.BackgroundProperty, "AccentDim");
                        badgeType.SetResourceReference(Border.BorderBrushProperty, "BorderAccent");

                        var tbType = new TextBlock
                        {
                            Text = d.type,
                            FontSize = 10.5,
                            FontWeight = FontWeights.SemiBold
                        };
                        tbType.SetResourceReference(TextBlock.ForegroundProperty, "Accent");
                        badgeType.Child = tbType;
                        DockPanel.SetDock(badgeType, Dock.Left);
                        row.Children.Add(badgeType);

                        // 3. Pourcentage d'occupation & Espace (ex: "21% utilisé — 1 466 Go libres sur 1 862 Go")
                        string labelPart = string.IsNullOrWhiteSpace(d.label) ? "" : $" ({d.label})";
                        string diskDesc = $"{d.usedPct}% utilisé{labelPart} — {d.freeGb:N0} Go libres sur {d.totalGb:N0} Go";

                        var tbDesc = new TextBlock
                        {
                            Text = diskDesc,
                            FontSize = 12.5,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        tbDesc.SetResourceReference(TextBlock.ForegroundProperty, "TextPrimary");
                        row.Children.Add(tbDesc);

                        PanelDisksSpecs.Children.Add(row);
                    }
                }
            });
        });
    }

    private void BtnCopySpecs_Click(object sender, RoutedEventArgs e)
    {
        var sb = new StringBuilder();
        sb.AppendLine("═══════════════════════════════════════════════════════════════");
        sb.AppendLine("  FICHE TECHNIQUE & SPÉCIFICATIONS — ALEX TOOLKIT");
        sb.AppendLine("═══════════════════════════════════════════════════════════════");
        sb.AppendLine($"• Application          : {TxtSpecApp.Text}");
        sb.AppendLine($"• Droits d'accès       : {TxtSpecPrivileges.Text}");
        sb.AppendLine($"• Ordinateur & Session : {TxtSpecMachine.Text}");
        sb.AppendLine($"• Système d'exploit.   : {TxtSpecOS.Text}");
        sb.AppendLine($"• Processeur (CPU)     : {TxtSpecCPU.Text}");
        sb.AppendLine($"• Mémoire vive (RAM)   : {TxtSpecRAM.Text}");
        sb.AppendLine("• Disques & Stockage   :");
        foreach (var child in PanelDisksSpecs.Children)
        {
            if (child is DockPanel dp)
            {
                var texts = dp.Children.OfType<TextBlock>().Select(t => t.Text).ToList();
                var badge = dp.Children.OfType<Border>().FirstOrDefault()?.Child as TextBlock;
                string line = $"  - {texts.FirstOrDefault()} [{badge?.Text}] {texts.LastOrDefault()}";
                sb.AppendLine(line);
            }
        }
        sb.AppendLine("═══════════════════════════════════════════════════════════════");

        try
        {
            Clipboard.SetText(sb.ToString());
            CopySpecsFeedbackBorder.Visibility = Visibility.Visible;
            _specsFeedbackTimer?.Stop();
            _specsFeedbackTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            _specsFeedbackTimer.Tick += (_, _) =>
            {
                _specsFeedbackTimer?.Stop();
                CopySpecsFeedbackBorder.Visibility = Visibility.Collapsed;
            };
            _specsFeedbackTimer.Start();
        }
        catch { }
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
        sb.AppendLine("=== ALEX TOOLKIT — STRUCTURE DES MODULES, SCRIPTS ET COMMANDES ===");
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
                .GroupBy(t => string.IsNullOrWhiteSpace(t.SubHeading) ? "Actions et outils rapides" : t.SubHeading.Trim())
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
        sb.AppendLine("# Alex Toolkit — Structure des modules, scripts et commandes");
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
                .GroupBy(t => string.IsNullOrWhiteSpace(t.SubHeading) ? "Actions et outils rapides" : t.SubHeading.Trim())
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
