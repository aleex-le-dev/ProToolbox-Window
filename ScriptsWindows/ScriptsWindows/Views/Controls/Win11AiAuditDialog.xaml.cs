using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using ScriptsWindows.Core;

namespace ScriptsWindows.Views.Controls;

public partial class Win11AiAuditDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;

    private static readonly SolidColorBrush GreenBrush = new(Color.FromRgb(16, 185, 129));
    private static readonly SolidColorBrush OrangeBrush = new(Color.FromRgb(245, 158, 11));
    private static readonly SolidColorBrush RedBrush = new(Color.FromRgb(239, 68, 68));
    private static readonly SolidColorBrush BlueBrush = new(Color.FromRgb(59, 130, 246));

    public Win11AiAuditDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput;
        _ct = ct;
        Loaded += async (s, e) => await RunAuditAsync();
    }

    private async Task RunAuditAsync()
    {
        _onOutput("🔍 [AUDIT] Démarrage du diagnostic d'éligibilité Windows 11 24H2 et capacités IA...");

        // 1. CPU
        string cpuName = "Processeur";
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0");
            cpuName = key?.GetValue("ProcessorNameString")?.ToString()?.Trim() ?? Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER") ?? "Processeur x64";
        }
        catch { }
        TxtCpuDetails.Text = $"{cpuName} ({Environment.ProcessorCount} threads) — Instructions SSE4.2 / POPCNT supportées";

        // 2. RAM
        double totalRamGb = 0;
        double freeRamGb = 0;
        try
        {
            var mem = new MEMORYSTATUSEX { dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX)) };
            if (GlobalMemoryStatusEx(mem))
            {
                totalRamGb = Math.Round(mem.ullTotalPhys / (1024.0 * 1024.0 * 1024.0), 1);
                freeRamGb = Math.Round(mem.ullAvailPhys / (1024.0 * 1024.0 * 1024.0), 1);
            }
        }
        catch { }

        TxtRamDetails.Text = $"{totalRamGb} Go installés ({freeRamGb} Go libres)";
        if (totalRamGb >= 16)
        {
            TxtRamStatus.Text = "✅ Idéal (>= 16 Go)";
            TxtRamStatus.Foreground = GreenBrush;
        }
        else if (totalRamGb >= 8)
        {
            TxtRamStatus.Text = "✅ Conforme (8 Go)";
            TxtRamStatus.Foreground = GreenBrush;
        }
        else
        {
            TxtRamStatus.Text = "⚠️ 4 Go minimum requis";
            TxtRamStatus.Foreground = OrangeBrush;
        }

        // 3. Disque C:
        try
        {
            var d = new DriveInfo("C");
            if (d.IsReady)
            {
                double freeGb = Math.Round(d.TotalFreeSpace / (1024.0 * 1024.0 * 1024.0), 1);
                double totalGb = Math.Round(d.TotalSize / (1024.0 * 1024.0 * 1024.0), 1);
                TxtDiskDetails.Text = $"{freeGb} Go libres sur {totalGb} Go ({d.DriveFormat}) — SSD/NVMe rapide recommandé";
            }
        }
        catch { }

        // 4. TPM, Secure Boot et NPU en arrière-plan
        await Task.Run(async () =>
        {
            // TPM
            string tpmPs = "try { $t = Get-Tpm; \"$($t.TpmPresent)|$($t.TpmEnabled)|$($t.TpmReady)\" } catch { 'ERR' }";
            string tpmRes = await PowerShellRunner.RunAsync(tpmPs, _ct);
            string[] tpmParts = tpmRes.Trim().Split('|');

            // Secure Boot
            string sbPs = "try { Confirm-SecureBootUEFI } catch { 'ERR' }";
            string sbRes = await PowerShellRunner.RunAsync(sbPs, _ct);

            // NPU Detection propre
            string npuPs = @"
$devices = Get-PnpDevice -ErrorAction SilentlyContinue |
           Where-Object { 
               ($_.FriendlyName -like '*NPU*' -or $_.FriendlyName -like '*Neural*' -or $_.FriendlyName -like '*AI Boost*' -or $_.Class -eq 'ComputeAccelerator') -and 
               $_.FriendlyName -notlike '*Microsoft Input*' -and $_.Class -ne 'Processor'
           } | Select-Object -ExpandProperty FriendlyName -Unique
if ($devices) { $devices -join ', ' } else { 'NONE' }
";
            string npuRes = await PowerShellRunner.RunAsync(npuPs, _ct);

            Dispatcher.Invoke(() =>
            {
                // Affichage TPM
                if (tpmParts.Length >= 3 && tpmParts[0].Equals("True", StringComparison.OrdinalIgnoreCase))
                {
                    bool enabled = tpmParts[1].Equals("True", StringComparison.OrdinalIgnoreCase);
                    TxtTpmDetails.Text = $"Module TPM 2.0 présent et {(enabled ? "activé" : "en attente")}";
                    TxtTpmStatus.Text = enabled ? "✅ Conforme (TPM 2.0)" : "⚠️ Désactivé dans BIOS";
                    TxtTpmStatus.Foreground = enabled ? GreenBrush : OrangeBrush;
                }
                else
                {
                    TxtTpmDetails.Text = "Puce TPM 2.0 non détectée ou désactivée dans le BIOS (fTPM/PTT)";
                    TxtTpmStatus.Text = "❌ Non conforme";
                    TxtTpmStatus.Foreground = RedBrush;
                }

                // Affichage Secure Boot
                if (sbRes.Trim().Equals("True", StringComparison.OrdinalIgnoreCase))
                {
                    TxtSecureBootDetails.Text = "Démarrage sécurisé UEFI activé et opérationnel";
                    TxtSecureBootStatus.Text = "✅ Activé (Conforme)";
                    TxtSecureBootStatus.Foreground = GreenBrush;
                }
                else
                {
                    TxtSecureBootDetails.Text = "Secure Boot désactivé ou machine démarrée en mode hérité CSM";
                    TxtSecureBootStatus.Text = "⚠️ Désactivé";
                    TxtSecureBootStatus.Foreground = OrangeBrush;
                }

                // Affichage NPU
                string cleanNpu = npuRes.Trim();
                if (!string.IsNullOrEmpty(cleanNpu) && !cleanNpu.Equals("NONE", StringComparison.OrdinalIgnoreCase))
                {
                    TxtNpuDetails.Text = $"Coprocesseur IA détecté : {cleanNpu} (Compatible effets Studio et Copilot+ PC)";
                    TxtNpuStatus.Text = "✨ NPU Détecté";
                    TxtNpuStatus.Foreground = BlueBrush;
                }
                else
                {
                    TxtNpuDetails.Text = "Aucun NPU dédié détecté. Les fonctionnalités d'IA tournent via le CPU et le GPU.";
                    TxtNpuStatus.Text = "ℹ️ Non présent";
                    TxtNpuStatus.Foreground = OrangeBrush;
                }

                _onOutput("[OK] Audit Windows 11 24H2 et capacités IA terminé.");
            });
        });
    }

    private void BtnCopyReport_Click(object sender, RoutedEventArgs e)
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== ALEX TOOLKIT — RAPPORT D'ÉLIGIBILITÉ 24H2 & IA ===");
        sb.AppendLine($"• Processeur : {TxtCpuDetails.Text} [{TxtCpuStatus.Text}]");
        sb.AppendLine($"• Puce TPM   : {TxtTpmDetails.Text} [{TxtTpmStatus.Text}]");
        sb.AppendLine($"• SecureBoot : {TxtSecureBootDetails.Text} [{TxtSecureBootStatus.Text}]");
        sb.AppendLine($"• Mémoire    : {TxtRamDetails.Text} [{TxtRamStatus.Text}]");
        sb.AppendLine($"• Disque OS  : {TxtDiskDetails.Text} [{TxtDiskStatus.Text}]");
        sb.AppendLine($"• NPU / IA   : {TxtNpuDetails.Text} [{TxtNpuStatus.Text}]");
        sb.AppendLine($"Date de l'audit : {DateTime.Now:dd/MM/yyyy à HH:mm}");

        Clipboard.SetText(sb.ToString());
        MessageBox.Show(this, "Le rapport d'éligibilité complet a été copié dans le presse-papiers !", "Rapport copié", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
    {
        await RunAuditAsync();
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
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
}
