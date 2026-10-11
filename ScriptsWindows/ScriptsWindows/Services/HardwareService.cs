using ScriptsWindows.Core;

namespace ScriptsWindows.Services;

/// <summary>Service matériel — hw_*, touch_*, print_*, pp_* du .bat.</summary>
public class HardwareService
{
    // SMART disques  (hw_smart)
    public static async Task RunSmartTest(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
Write-Output '--- TEST SMART DES DISQUES ---'
Get-PhysicalDisk | ForEach-Object {
    Write-Output ""[DISQUE] $($_.FriendlyName)""
    Write-Output ""  Santé  : $($_.HealthStatus)""
    Write-Output ""  Média  : $($_.MediaType)""
    Write-Output ""  Taille : $([math]::Round($_.Size/1GB,1)) GB""
    Write-Output ''
}
", onOutput, ct);
    }

    // WinSAT  (hw_winsat)
    public static async Task RunWinSat(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Lancement du score WinSAT (peut prendre 2-5 min)...");
        await CommandRunner.RunAsync("winsat formal", onOutput, ct);
    }

    // Test RAM  (hw_ram_test)
    public static Task RunRamTest(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Lancement de l'outil de diagnostic mémoire Windows...");
        CommandRunner.StartProcess("mdsched.exe");
        onOutput("[i] Le PC redémarrera pour effectuer le test");
        return Task.CompletedTask;
    }

    // Rapport matériel complet  (hw_full_report)
    public static async Task GetFullReport(Action<string> onOutput, CancellationToken ct = default)
    {
        await DiagnosticService.GetSystemReport(onOutput, ct);
        await RunSmartTest(onOutput, ct);
    }

    // Moniteur de ressources  (hw_resmon)
    public static Task LaunchResmon(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Lancement du Moniteur de ressources Windows...");
        CommandRunner.StartProcess("resmon.exe");
        onOutput("[OK] Moniteur de ressources ouvert");
        return Task.CompletedTask;
    }

    // Informations système  (hw_msinfo32)
    public static Task LaunchMsinfo32(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Lancement de msinfo32 — Informations système...");
        CommandRunner.StartProcess("msinfo32.exe");
        onOutput("[OK] Informations système ouvertes");
        return Task.CompletedTask;
    }

    // Diagnostic DirectX / GPU / Audio  (hw_dxdiag)
    public static Task LaunchDxdiag(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Lancement de DxDiag — Outil de diagnostic DirectX...");
        CommandRunner.StartProcess("dxdiag.exe");
        onOutput("[OK] DxDiag ouvert");
        return Task.CompletedTask;
    }

    // Gestionnaire de périphériques  (hw_devmgmt)
    public static Task LaunchDevmgmt(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Lancement du Gestionnaire de périphériques...");
        CommandRunner.StartProcess("devmgmt.msc");
        onOutput("[OK] Gestionnaire de périphériques ouvert");
        return Task.CompletedTask;
    }

    // Diagnostic d'alimentation  (hw_powercfg_energy)
    public static async Task RunPowercfgEnergy(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Analyse de consommation d'énergie en cours (60 secondes)...");
        var reportPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "energy-report.html");
        await CommandRunner.RunAsync($"powercfg /energy /output \"{reportPath}\" /duration 60", onOutput, ct);
        if (System.IO.File.Exists(reportPath))
        {
            onOutput($"[OK] Rapport généré : {reportPath}");
            CommandRunner.StartProcess(reportPath);
        }
        else
        {
            onOutput("[!] Rapport non généré — essayez avec les droits Administrateur");
        }
    }

    // Écran tactile — activer  (touch_enable)
    public static async Task EnableTouchscreen(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
$dev = Get-PnpDevice | Where-Object { $_.Class -eq 'HIDClass' -and $_.FriendlyName -like '*Touch*' }
if ($dev) {
    Enable-PnpDevice -InstanceId $dev.InstanceId -Confirm:$false
    Write-Output '[OK] Écran tactile activé'
} else {
    Write-Output '[!] Aucun écran tactile HID trouvé'
}
", onOutput, ct);
    }

    // Écran tactile — désactiver  (touch_disable)
    public static async Task DisableTouchscreen(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
$dev = Get-PnpDevice | Where-Object { $_.Class -eq 'HIDClass' -and $_.FriendlyName -like '*Touch*' }
if ($dev) {
    Disable-PnpDevice -InstanceId $dev.InstanceId -Confirm:$false
    Write-Output '[OK] Écran tactile désactivé'
} else {
    Write-Output '[!] Aucun écran tactile HID trouvé'
}
", onOutput, ct);
    }

    // Lister imprimantes  (print_list)
    public static async Task ListPrinters(Action<string> onOutput, CancellationToken ct = default)
        => await PowerShellRunner.RunAsync("Get-Printer | Format-Table Name, PrinterStatus, PortName -AutoSize", onOutput, ct);

    // Vider file d'impression  (print_clear_queue)
    public static async Task ClearPrintQueue(Action<string> onOutput, CancellationToken ct = default)
    {
        await CommandRunner.RunAsync("net stop spooler", onOutput, ct);
        await PowerShellRunner.RunAsync(@"Remove-Item 'C:\Windows\System32\spool\PRINTERS\*' -Recurse -Force -ErrorAction SilentlyContinue; Write-Output '[OK] File vidée'", onOutput, ct);
        await CommandRunner.RunAsync("net start spooler", onOutput, ct);
    }

    // Redémarrer spooler  (print_restart_spooler)
    public static async Task RestartSpooler(Action<string> onOutput, CancellationToken ct = default)
    {
        await CommandRunner.RunAsync("net stop spooler", onOutput, ct);
        await Task.Delay(500, ct);
        await CommandRunner.RunAsync("net start spooler", onOutput, ct);
        onOutput("[OK] Spooler redémarré");
    }

    // Plans d'alimentation
    public static async Task SetPowerPlan(string guid, string name, Action<string> onOutput, CancellationToken ct = default)
    {
        await CommandRunner.RunAsync($"powercfg /setactive {guid}", onOutput, ct);
        onOutput($"[OK] Plan activé : {name}");
    }

    public static Task SetPlanBalanced(Action<string> onOutput, CancellationToken ct = default)
        => SetPowerPlan("381b4222-f694-41f0-9685-ff5bb260df2e", "Équilibré", onOutput, ct);

    public static Task SetPlanSaver(Action<string> onOutput, CancellationToken ct = default)
        => SetPowerPlan("a1841308-3541-4fab-bc81-f71556f20b4a", "Économies d'énergie", onOutput, ct);

    public static Task SetPlanHigh(Action<string> onOutput, CancellationToken ct = default)
        => SetPowerPlan("8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c", "Hautes performances", onOutput, ct);

    public static async Task SetPlanUltimate(Action<string> onOutput, CancellationToken ct = default)
    {
        // Activer d'abord le plan Ultimate (masqué par défaut)
        await CommandRunner.RunAsync("powercfg -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61", onOutput, ct);
        await CommandRunner.RunAsync("powercfg /setactive e9a42b02-d5df-448d-aa00-03f14749eb61", onOutput, ct);
        onOutput("[OK] Plan Performances Ultimes activé");
    }

    public static async Task GetCurrentPlan(Action<string> onOutput, CancellationToken ct = default)
        => await CommandRunner.RunAsync("powercfg /getactivescheme", onOutput, ct);

    public static async Task ListPowerPlans(Action<string> onOutput, CancellationToken ct = default)
        => await CommandRunner.RunAsync("powercfg /list", onOutput, ct);

    // Santé disque S.M.A.R.T. (wmic diskdrive get status)
    public static async Task GetSmartWmicStatus(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Interrogation de l'état S.M.A.R.T. via WMIC...");
        await CommandRunner.RunAsync("wmic diskdrive get status,model,caption", onOutput, ct);
    }
}
