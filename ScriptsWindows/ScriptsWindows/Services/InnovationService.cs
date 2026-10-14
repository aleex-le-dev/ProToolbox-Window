using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ScriptsWindows.Views.Controls;

namespace ScriptsWindows.Services;

/// <summary>
/// Service dédié à la catégorie NOUVEAUTÉ :
/// Fournit des interfaces graphiques modernes (WPF Dialogs) et réversibles pour chaque outil :
/// 1. Bouclier de confidentialité & anti-télémétrie
/// 2. Optimiseur gaming & latence minimale
/// 3. Audit d'éligibilité Windows 11 24H2 & capacités IA
/// 4. Benchmark de vitesse DNS en temps réel
/// </summary>
public static class InnovationService
{
    // ─────────────────────────────────────────────────────────────────────────
    // 1. BOUCLIER DE CONFIDENTIALITÉ (INTERFACE GRAPHIQUE)
    // ─────────────────────────────────────────────────────────────────────────
    public static async Task OpenPrivacyShieldDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var dialog = new PrivacyShieldDialog(onOutput, ct)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 2. OPTIMISEUR GAMING & LATENCE (INTERFACE GRAPHIQUE)
    // ─────────────────────────────────────────────────────────────────────────
    public static async Task OpenGamingOptimizerDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var dialog = new GamingOptimizerDialog(onOutput, ct)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 3. AUDIT WINDOWS 11 24H2 & IA (INTERFACE GRAPHIQUE)
    // ─────────────────────────────────────────────────────────────────────────
    public static async Task OpenWin11AiAuditDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var dialog = new Win11AiAuditDialog(onOutput, ct)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 4. BENCHMARK DE VITESSE DNS (INTERFACE GRAPHIQUE)
    // ─────────────────────────────────────────────────────────────────────────
    public static async Task OpenDnsBenchmarkDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var dialog = new DnsBenchmarkDialog(onOutput, ct)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        });
    }
}
