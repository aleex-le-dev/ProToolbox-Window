using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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

    /// <summary>Affiche la santé S.M.A.R.T. en % et l'espace de stockage de chaque disque physique et volume.</summary>
    public static async Task GetDisksHealthAndStorage(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Analyse approfondie des disques, de leur santé et de l'espace...");
        await PowerShellRunner.RunAsync(@"
$pDisks = @(Get-PhysicalDisk -ErrorAction SilentlyContinue)
$disks = @(Get-Disk -ErrorAction SilentlyContinue)

if (-not $disks -or $disks.Count -eq 0) {
    Write-Output '[!] Aucun disque détecté par Windows'
    return
}

foreach ($d in $disks) {
    $pd = $pDisks | Where-Object { $_.DeviceId -eq $d.Number -or $_.FriendlyName -eq $d.FriendlyName } | Select-Object -First 1
    $media = if ($pd.MediaType) { $pd.MediaType } else { 'Disque' }
    $bus = if ($pd.BusType) { $pd.BusType } else { $d.BusType }
    
    # Évaluation de la santé S.M.A.R.T. en %
    $healthLabel = '100% (Sain / Intact)'
    $counter = try { Get-StorageReliabilityCounter -Disk $pd -ErrorAction SilentlyContinue } catch { $null }
    if ($counter -and $counter.Wear -ne $null) {
        $wear = [int]$counter.Wear
        $healthPct = [math]::Max(0, 100 - $wear)
        $healthLabel = ""$healthPct% (Usure : $wear%)""
    } else {
        if ($d.HealthStatus -eq 'Healthy') { $healthLabel = '100% (Sain / Intact)' }
        elseif ($d.HealthStatus -eq 'Warning') { $healthLabel = '70% (Avertissement S.M.A.R.T.)' }
        elseif ($d.HealthStatus -eq 'Unhealthy') { $healthLabel = '0% (Défaillant / Critique)' }
        else { $healthLabel = ""$($d.HealthStatus)"" }
    }
    
    $sizeGB = [math]::Round($d.Size / 1GB, 1)
    
    Write-Output '=================================================='
    Write-Output "" DISQUE $($d.Number) : $($d.FriendlyName)""
    Write-Output '=================================================='
    Write-Output ""• Type             : $media ($bus)""
    Write-Output ""• Capacité totale  : $sizeGB Go""
    Write-Output ""• Santé S.M.A.R.T. : $healthLabel""
    Write-Output ""• Statut           : $($d.OperationalStatus)""
    
    $parts = $d | Get-Partition -ErrorAction SilentlyContinue
    $volumes = $parts | Get-Volume -ErrorAction SilentlyContinue | Where-Object { $_.DriveLetter }
    
    if ($volumes) {
        Write-Output '• Volumes et Espace :'
        foreach ($v in $volumes) {
            $freeGB = [math]::Round($v.SizeRemaining / 1GB, 2)
            $totGB  = [math]::Round($v.Size / 1GB, 2)
            $usedGB = [math]::Round($totGB - $freeGB, 2)
            $pctFree = if ($totGB -gt 0) { [math]::Round(($freeGB / $totGB) * 100, 1) } else { 0 }
            $pctUsed = if ($totGB -gt 0) { [math]::Round(($usedGB / $totGB) * 100, 1) } else { 0 }
            Write-Output ""   -> Lecteur $($v.DriveLetter): [$($v.FileSystemType)]""
            Write-Output ""      - Espace libre   : $freeGB Go sur $totGB Go ($pctFree% disponible)""
            Write-Output ""      - Espace utilisé : $usedGB Go ($pctUsed% occupé)""
        }
    } else {
        Write-Output '• Volumes          : Aucune partition avec lettre attribuée'
    }
    Write-Output ''
}
", onOutput, ct);
    }

    /// <summary>Rétro-compatibilité : appelle l'analyse unifiée des disques et de l'espace.</summary>
    public static Task GetPhysicalDisks(Action<string> onOutput, CancellationToken ct = default)
        => GetDisksHealthAndStorage(onOutput, ct);

    /// <summary>Rétro-compatibilité : appelle l'analyse unifiée des disques et de l'espace.</summary>
    public static Task GetVolumesStatus(Action<string> onOutput, CancellationToken ct = default)
        => GetDisksHealthAndStorage(onOutput, ct);

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

    /// <summary>Ouvre l'interface interactive de gestion et chiffrement BitLocker (sélection de disque, état, activation/désactivation).</summary>
    public static async Task ManageBitLocker(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Analyse de l'état BitLocker sur tous les lecteurs...");
        await DiagnosticService.CheckBitLocker(onOutput, ct);

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var dialog = new Views.Controls.BitLockerDialog(onOutput, ct)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        });
    }
}
