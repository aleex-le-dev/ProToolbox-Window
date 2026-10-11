using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ScriptsWindows.Core;

namespace ScriptsWindows.Services;

/// <summary>
/// Service de gestion des disques, volumes et partitionnement.
/// </summary>
public static class DiskService
{
    /// <summary>Ouvre la console de Gestion des Disques Windows (diskmgmt.msc).</summary>
    public static Task LaunchDiskManagement(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Ouverture du Gestionnaire de disques Windows...");
        CommandRunner.StartProcess("diskmgmt.msc");
        onOutput("[OK] Gestionnaire de disques ouvert");
        return Task.CompletedTask;
    }

    /// <summary>Affiche la liste des disques physiques, modèles et santé.</summary>
    public static async Task GetPhysicalDisks(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Analyse des disques physiques...");
        await PowerShellRunner.RunAsync(@"
Write-Output '=== DISQUES PHYSIQUES ET ÉTAT S.M.A.R.T. ==='
Get-PhysicalDisk | Format-Table -AutoSize Number, FriendlyName, MediaType, HealthStatus, OperationalStatus, @{N='Taille (Go)';E={[math]::Round($_.Size/1GB, 1)}} | Out-String | Write-Output
", onOutput, ct);
    }

    /// <summary>Affiche les volumes, lettres de lecteur et espace libre.</summary>
    public static async Task GetVolumesStatus(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Analyse des volumes et de l'espace disque...");
        await PowerShellRunner.RunAsync(@"
Write-Output '=== VOLUMES ET ESPACE DISQUE ==='
Get-Volume | Where-Object { $_.DriveLetter } | ForEach-Object {
    $free = [math]::Round($_.SizeRemaining / 1GB, 2)
    $total = [math]::Round($_.Size / 1GB, 2)
    $pct = if ($total -gt 0) { [math]::Round(($free / $total) * 100, 1) } else { 0 }
    Write-Output ""Lecteur $($_.DriveLetter): [$($_.FileSystemType)] - Libre: $free Go / $total Go ($pct% libre)""
}
", onOutput, ct);
    }

    /// <summary>Exécute l'optimisation de disque (TRIM pour SSD, défragmentation pour HDD).</summary>
    public static async Task OptimizeDrives(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Optimisation automatique des lecteurs (TRIM / Défragmentation)...");
        await CommandRunner.RunAsync("defrag C: /O /V", onOutput, ct);
        onOutput("[OK] Optimisation du lecteur C: terminée");
    }

    /// <summary>Ouvre l'utilitaire graphique d'optimisation et défragmentation Windows (dfrgui.exe).</summary>
    public static Task LaunchDefragGui(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Ouverture de l'Optimiseur de lecteurs Windows...");
        CommandRunner.StartProcess("dfrgui.exe");
        onOutput("[OK] Utilitaire d'optimisation ouvert");
        return Task.CompletedTask;
    }

    /// <summary>Vérifie et planifie la réparation complète du disque C: via CHKDSK.</summary>
    public static async Task RunChkdskRepair(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Lancement de l'analyse CHKDSK /f /r sur le lecteur C:...");
        await CommandRunner.RunAsync("echo O | chkdsk C: /f /r", onOutput, ct);
    }

    /// <summary>Ouvre la console interactive DISKPART dans une invite de commande avec privilèges élevés.</summary>
    public static Task LaunchDiskpart(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Lancement de DISKPART dans une invite de commande...");
        CommandRunner.StartProcess("cmd.exe", "/c start cmd.exe /k diskpart");
        onOutput("[OK] Console DISKPART lancée");
        return Task.CompletedTask;
    }

    /// <summary>Lance l'outil de nettoyage de disque Windows (cleanmgr.exe).</summary>
    public static Task LaunchCleanmgr(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Lancement du Nettoyage de disque Windows...");
        CommandRunner.StartProcess("cleanmgr.exe", "/sagerun:1");
        onOutput("[OK] Nettoyage de disque lancé");
        return Task.CompletedTask;
    }
}
