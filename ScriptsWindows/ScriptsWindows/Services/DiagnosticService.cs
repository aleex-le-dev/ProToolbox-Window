using ScriptsWindows.Core;

namespace ScriptsWindows.Services;

/// <summary>
/// Service de diagnostic système.
/// Porte toutes les fonctions sys_* et ev_* du .bat original.
/// </summary>
public class DiagnosticService
{
    // ────────────────────────────────────────────────────────────────
    //  RAPPORT SYSTÈME COMPLET  (sys_report)
    //  Équivalent : powershell Get-ComputerInfo + WMI queries
    // ────────────────────────────────────────────────────────────────
    public static async Task GetSystemReport(Action<string> onOutput, CancellationToken ct = default)
    {
        const string script = @"
$sep = '=' * 60
Write-Output $sep
Write-Output '  RAPPORT SYSTEME COMPLET'
Write-Output $sep

# OS
$os = Get-WmiObject Win32_OperatingSystem
Write-Output ""[OS] $($os.Caption) Build $($os.BuildNumber)""
Write-Output ""[OS] Version : $($os.Version)""
Write-Output ""[OS] Architecture : $($os.OSArchitecture)""

# CPU
$cpu = Get-WmiObject Win32_Processor
Write-Output ""[CPU] $($cpu.Name)""
Write-Output ""[CPU] Coeurs : $($cpu.NumberOfCores) / Threads : $($cpu.NumberOfLogicalProcessors)""
Write-Output ""[CPU] Vitesse max : $($cpu.MaxClockSpeed) MHz""

# RAM
$ram = Get-WmiObject Win32_PhysicalMemory | Measure-Object -Property Capacity -Sum
$ramGB = [math]::Round($ram.Sum / 1GB, 2)
Write-Output ""[RAM] Total : $ramGB GB""
$freeRam = [math]::Round($os.FreePhysicalMemory / 1MB, 2)
Write-Output ""[RAM] Libre : $freeRam GB""

# GPU
Get-WmiObject Win32_VideoController | ForEach-Object {
    Write-Output ""[GPU] $($_.Caption)""
    $vramMB = [math]::Round($_.AdapterRAM / 1MB, 0)
    Write-Output ""[GPU] VRAM : $vramMB MB""
}

# Disques
Get-WmiObject Win32_LogicalDisk -Filter 'DriveType=3' | ForEach-Object {
    $totalGB = [math]::Round($_.Size / 1GB, 2)
    $freeGB  = [math]::Round($_.FreeSpace / 1GB, 2)
    $usedPct = [math]::Round(($_.Size - $_.FreeSpace) / $_.Size * 100, 1)
    Write-Output ""[DISK] $($_.DeviceID) — Total: $totalGB GB / Libre: $freeGB GB ($usedPct% utilisé)""
}

# Réseau
Get-NetAdapter | Where-Object { $_.Status -eq 'Up' } | ForEach-Object {
    Write-Output ""[NET] $($_.Name) — $($_.LinkSpeed) — MAC: $($_.MacAddress)""
}

# IP
Get-NetIPAddress -AddressFamily IPv4 | Where-Object { $_.InterfaceAlias -notlike '*Loopback*' } | ForEach-Object {
    Write-Output ""[IP] $($_.InterfaceAlias) : $($_.IPAddress)""
}

Write-Output $sep
Write-Output '[OK] Rapport terminé'
Write-Output $sep
";
        await PowerShellRunner.RunAsync(script, onOutput, ct);
    }

    // ────────────────────────────────────────────────────────────────
    //  CARTE MÈRE, BIOS & SYSTÈME  (sys_motherboard_bios)
    // ────────────────────────────────────────────────────────────────
    public static async Task GetMotherboardBiosInfo(Action<string> onOutput, CancellationToken ct = default)
    {
        const string script = @"
$sep = '=' * 60
Write-Output $sep
Write-Output '  CARTE MÈRE, BIOS & CONFIGURATION SYSTÈME'
Write-Output $sep

# Carte Mère
$bb = Get-CimInstance Win32_BaseBoard
Write-Output ""[CARTE MÈRE] Fabricant     : $($bb.Manufacturer)""
Write-Output ""[CARTE MÈRE] Modèle        : $($bb.Product)""
Write-Output ""[CARTE MÈRE] Numéro Série  : $($bb.SerialNumber)""
Write-Output ''

# BIOS / UEFI
$bios = Get-CimInstance Win32_BIOS
Write-Output ""[BIOS/UEFI] Fabricant     : $($bios.Manufacturer)""
Write-Output ""[BIOS/UEFI] Version       : $($bios.SMBIOSBIOSVersion)""
Write-Output ""[BIOS/UEFI] Date          : $($bios.ReleaseDate)""
Write-Output ''

# Processeur
$cpu = Get-CimInstance Win32_Processor
Write-Output ""[CPU] Modèle            : $($cpu.Name)""
Write-Output ""[CPU] Cœurs / Threads    : $($cpu.NumberOfCores) cœurs / $($cpu.NumberOfLogicalProcessors) threads""
Write-Output ""[CPU] Fréquence Max     : $($cpu.MaxClockSpeed) MHz""
Write-Output ''

# Machine & OS
$cs = Get-CimInstance Win32_ComputerSystem
$os = Get-CimInstance Win32_OperatingSystem
Write-Output ""[MACHINE] Nom PC        : $($cs.Name)""
Write-Output ""[MACHINE] Fabricant     : $($cs.Manufacturer)""
Write-Output ""[MACHINE] Modèle        : $($cs.Model)""
Write-Output ""[OS] Système             : $($os.Caption) ($($os.OSArchitecture))""
Write-Output ""[OS] Version / Build    : $($os.Version) (Build $($os.BuildNumber))""

Write-Output $sep
Write-Output '[OK] Audit système terminé avec succès'
Write-Output $sep
";
        await PowerShellRunner.RunAsync(script, onOutput, ct);
    }

    // ────────────────────────────────────────────────────────────────
    //  RAPPORT BATTERIE  (sys_battery_report)
    // ────────────────────────────────────────────────────────────────
    public static async Task GetBatteryReport(Action<string> onOutput, CancellationToken ct = default)
    {
        var targetDir = Core.FolderDialogHelper.AskAndCreateDestinationFolder(
            title: "Sélectionnez l'emplacement où sauvegarder le rapport batterie",
            folderPrefix: "Rapport_Batterie",
            confirmQuestion: "Souhaitez-vous enregistrer le rapport d'usure et d'autonomie de la batterie dans un dossier de votre choix ?",
            confirmTitle: "Rapport de batterie — Alex Toolkit"
        );

        string reportPath = targetDir != null
            ? System.IO.Path.Combine(targetDir, "battery-report.html")
            : System.IO.Path.Combine(System.IO.Path.GetTempPath(), "battery-report.html");

        await PowerShellRunner.RunAsync($@"
$reportPath = '{reportPath.Replace("'", "''")}'
& powercfg /batteryreport /output $reportPath | Out-Null
if (Test-Path $reportPath) {{
    Write-Output '[OK] Rapport batterie généré'
    Write-Output ""[i] Emplacement : $reportPath""
    Start-Process $reportPath
}} else {{
    Write-Output '[!] Aucune batterie détectée ou rapport impossible'
}}
", onOutput, ct);

        if (targetDir != null && System.IO.File.Exists(reportPath))
        {
            onOutput($"✅ [SUCCÈS] Rapport batterie enregistré dans : {targetDir}");
            Core.FolderDialogHelper.OpenInExplorer(targetDir);
        }
    }

    // ────────────────────────────────────────────────────────────────
    //  STATUT BATTERIE EN DIRECT  (sys_battery_status)
    // ────────────────────────────────────────────────────────────────
    public static async Task GetBatteryStatus(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
$bat = Get-WmiObject Win32_Battery
if ($bat) {
    Write-Output ""[BATTERIE] Niveau : $($bat.EstimatedChargeRemaining)%""
    $status = switch ($bat.BatteryStatus) {
        1 { 'Décharge' } 2 { 'Secteur (AC)' } 3 { 'Charge complète' }
        6 { 'En charge' } 7 { 'Charge faible' } 8 { 'Critique' }
        default { ""Inconnu ($($bat.BatteryStatus))"" }
    }
    Write-Output ""[BATTERIE] Statut : $status""

    $runtime = if ($bat.EstimatedRunTime -and $bat.EstimatedRunTime -lt 60000 -and $bat.EstimatedRunTime -gt 0) {
        $h = [math]::Floor($bat.EstimatedRunTime / 60)
        $m = $bat.EstimatedRunTime % 60
        if ($h -gt 0) { ""$h h $m min ($($bat.EstimatedRunTime) minutes)"" } else { ""$m minutes"" }
    } elseif ($bat.BatteryStatus -eq 2 -or $bat.BatteryStatus -eq 3) {
        'Alimentation secteur (illimitée)'
    } else {
        'Évaluation en cours...'
    }
    Write-Output ""[BATTERIE] Autonomie estimée : $runtime""

    $voltage = if ($bat.DesignVoltage -and $bat.DesignVoltage -gt 0) {
        $v = [math]::Round($bat.DesignVoltage / 1000.0, 2)
        ""$v V ($($bat.DesignVoltage) mV)""
    } else {
        'Non disponible'
    }
    Write-Output ""[BATTERIE] Tension : $voltage""
} else {
    Write-Output '[!] Aucune batterie détectée (PC fixe ou non supporté)'
}
", onOutput, ct);
    }

    // Rapport complet batterie : statut direct + rapport HTML
    public static async Task GetFullBatteryReport(Action<string> onOutput, CancellationToken ct = default)
    {
        await GetBatteryStatus(onOutput, ct);
        onOutput("");
        onOutput("[i] Génération de l'historique complet d'usure et de cycles (HTML)...");
        await GetBatteryReport(onOutput, ct);
    }

    // ────────────────────────────────────────────────────────────────
    //  TEMPÉRATURE CPU/GPU  (sys_temp_report)
    // ────────────────────────────────────────────────────────────────
    public static async Task GetTemperatureReport(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
$sep = '-' * 50
Write-Output $sep
Write-Output '  RAPPORT DE TEMPÉRATURE'
Write-Output $sep

# WMI Temperature (si disponible)
try {
    $temps = Get-WmiObject MSAcpi_ThermalZoneTemperature -Namespace root/wmi -ErrorAction Stop
    foreach ($t in $temps) {
        $celsius = [math]::Round(($t.CurrentTemperature - 2732) / 10, 1)
        Write-Output ""[TEMP] $($t.InstanceName) : $celsius °C""
    }
} catch {
    Write-Output '[!] Capteurs WMI non accessibles'
    Write-Output '[i] Utilisez HWMonitor ou HWiNFO pour des mesures précises'
}
Write-Output $sep
", onOutput, ct);
    }

    // ────────────────────────────────────────────────────────────────
    //  TEST RAM  (sys_ram_check)
    // ────────────────────────────────────────────────────────────────
    public static async Task CheckRam(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
$sep = '-' * 50
Write-Output $sep
Write-Output '  ANALYSE MÉMOIRE RAM'
Write-Output $sep

$sticks = @(Get-CimInstance Win32_PhysicalMemory -ErrorAction SilentlyContinue)
if (-not $sticks -or $sticks.Count -eq 0) {
    $sticks = @(Get-WmiObject Win32_PhysicalMemory -ErrorAction SilentlyContinue)
}
if (-not $sticks -or $sticks.Count -eq 0) { Write-Output '[!] Aucune information mémoire disponible'; return }

$memTypes = @{ 20='DDR'; 21='DDR2'; 24='DDR3'; 26='DDR4'; 29='LPDDR3'; 30='LPDDR4'; 31='LPDDR4X'; 34='DDR5'; 35='LPDDR5X'; 36='LPDDR5' }
$formFactors = @{ 8='DIMM'; 12='SODIMM'; 0='Intégrée (Soudée)' }

$i = 1
foreach ($s in $sticks) {
    $capacityGB = [math]::Round($s.Capacity / 1GB, 2)
    $rawType = if ($s.SMBIOSMemoryType -gt 0) { $s.SMBIOSMemoryType } else { $s.MemoryType }
    $tName = if ($memTypes.ContainsKey([int]$rawType)) { $memTypes[[int]$rawType] } elseif ($rawType -gt 0) { ""Type $rawType"" } else { ""Standard"" }
    $fName = if ($formFactors.ContainsKey([int]$s.FormFactor)) { $formFactors[[int]$s.FormFactor] } else { ""Format $($s.FormFactor)"" }

    Write-Output ""[SLOT $i] Capacité : $capacityGB GB""
    Write-Output ""[SLOT $i] Vitesse  : $($s.Speed) MHz""
    Write-Output ""[SLOT $i] Type     : $tName ($fName)""
    Write-Output ""[SLOT $i] Fabricant: $($s.Manufacturer)""
    $i++
}

$total = ($sticks | Measure-Object -Property Capacity -Sum).Sum
$totalGB = [math]::Round($total / 1GB, 2)
$usedSlots = $sticks.Count
Write-Output ""$sep""
Write-Output ""[TOTAL] $totalGB GB installés — $usedSlots slots utilisés""

# Test outil Windows
Write-Output '[i] Pour un test approfondi, lancez : mdsched.exe'
", onOutput, ct);
    }

    // ────────────────────────────────────────────────────────────────
    //  CONSOMMATION RAM EN DIRECT  (sys_ram_usage)
    // ────────────────────────────────────────────────────────────────
    public static async Task GetRamUsage(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
$os = Get-WmiObject Win32_OperatingSystem
$totalGB = [math]::Round($os.TotalVisibleMemorySize / 1MB, 2)
$freeGB  = [math]::Round($os.FreePhysicalMemory / 1MB, 2)
$usedGB  = [math]::Round($totalGB - $freeGB, 2)
$pct     = [math]::Round($usedGB / $totalGB * 100, 1)

Write-Output ""[RAM] Total : $totalGB GB""
Write-Output ""[RAM] Utilisée : $usedGB GB ($pct%)""
Write-Output ""[RAM] Libre : $freeGB GB""

Write-Output ''
Write-Output '--- TOP 10 PROCESSUS (RAM) ---'
Get-Process | Sort-Object WorkingSet64 -Descending | Select-Object -First 10 | ForEach-Object {
    $mb = [math]::Round($_.WorkingSet64 / 1MB, 1)
    Write-Output ""  $mb MB — $($_.ProcessName)""
}
", onOutput, ct);
    }

    // ────────────────────────────────────────────────────────────────
    //  DIAGNOSTIC RÉSEAU  (sys_diag_network)
    // ────────────────────────────────────────────────────────────────
    public static async Task DiagNetwork(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
Write-Output '--- DIAGNOSTIC RÉSEAU ---'
Write-Output ''

# Test ping
$targets = @('8.8.8.8', '1.1.1.1', 'google.com')
foreach ($t in $targets) {
    $r = Test-Connection $t -Count 2 -Quiet
    $status = if ($r) { '[OK]' } else { '[ERR]' }
    Write-Output ""$status Ping $t""
}

Write-Output ''
Write-Output '--- CONFIGURATION IP ---'
Get-NetIPConfiguration | Where-Object { $_.IPv4Address } | ForEach-Object {
    Write-Output ""Interface : $($_.InterfaceAlias)""
    Write-Output ""  IP      : $($_.IPv4Address.IPAddress)""
    Write-Output ""  Gateway : $($_.IPv4DefaultGateway.NextHop)""
    Write-Output ""  DNS     : $($_.DnsServer.ServerAddresses -join ', ')""
    Write-Output ''
}
", onOutput, ct);
    }

    // ────────────────────────────────────────────────────────────────
    //  ÉTAT BITLOCKER  (sys_bitlocker_check)
    // ────────────────────────────────────────────────────────────────
    public static async Task CheckBitLocker(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
Write-Output '--- ÉTAT BITLOCKER ---'
try {
    $vols = Get-BitLockerVolume -ErrorAction Stop
    foreach ($v in $vols) {
        $status = if ($v.ProtectionStatus -eq 'On') { '[OK] ACTIVÉ' } else { '[!] DÉSACTIVÉ' }
        Write-Output ""$status — $($v.MountPoint) — $($v.EncryptionMethod)""
    }
} catch {
    Write-Output '[!] BitLocker non disponible sur cette édition Windows'
}
", onOutput, ct);
    }

    // ────────────────────────────────────────────────────────────────
    //  JOURNAL DES ERREURS  (sys_event_log)
    // ────────────────────────────────────────────────────────────────
    public static async Task GetEventLog(Action<string> onOutput, CancellationToken ct = default)
        => await GetCriticalEvents(1, onOutput, ct);

    public static async Task GetCriticalEvents(int days, Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync($@"
$since = (Get-Date).AddDays(-{days})
Write-Output ""--- ERREURS CRITIQUES (dernières {days}h) ---""
$events = Get-WinEvent -FilterHashtable @{{
    LogName   = 'System','Application'
    Level     = 1,2
    StartTime = $since
}} -MaxEvents 30 -ErrorAction SilentlyContinue

if (-not $events) {{ Write-Output '[OK] Aucune erreur critique trouvée'; return }}

foreach ($e in $events) {{
    Write-Output ""[$($e.TimeCreated.ToString('dd/MM HH:mm'))] [$($e.LevelDisplayName)] $($e.ProviderName)""
    Write-Output ""  → $($e.Message.Substring(0, [Math]::Min(120, $e.Message.Length)))""
}}
Write-Output ""Total: $($events.Count) événements trouvés""
", onOutput, ct);
    }

    public static async Task GetAppErrors(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
$since = (Get-Date).AddHours(-24)
Write-Output '--- ERREURS APPLICATIONS 24H ---'
$events = Get-WinEvent -FilterHashtable @{
    LogName   = 'Application'
    Level     = 2
    StartTime = $since
} -MaxEvents 20 -ErrorAction SilentlyContinue

if (-not $events) { Write-Output '[OK] Aucune erreur application'; return }
foreach ($e in $events) {
    Write-Output ""[$($e.TimeCreated.ToString('dd/MM HH:mm'))] $($e.ProviderName)""
    Write-Output ""  → $($e.Message.Substring(0, [Math]::Min(100, $e.Message.Length)))""
}
", onOutput, ct);
    }

    // ────────────────────────────────────────────────────────────────
    //  ÉTAT WINDOWS DEFENDER  (sys_defender)
    // ────────────────────────────────────────────────────────────────
    public static async Task GetDefenderStatus(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
Write-Output '--- ÉTAT WINDOWS DEFENDER ---'
try {
    $def = Get-MpComputerStatus -ErrorAction Stop
    $rt  = if ($def.RealTimeProtectionEnabled) { '[OK] ACTIVÉE' } else { '[ERR] DÉSACTIVÉE' }
    Write-Output ""Protection temps réel : $rt""
    Write-Output ""Signatures antivirus  : $($def.AntivirusSignatureLastUpdated.ToString('dd/MM/yyyy HH:mm'))""
    Write-Output ""Signatures spyware    : $($def.AntispywareSignatureLastUpdated.ToString('dd/MM/yyyy HH:mm'))""
    Write-Output ""Version engine        : $($def.AMEngineVersion)""
    $threats = if ($def.QuickScanOverdue) { '[!] Scan en retard' } else { '[OK] À jour' }
    Write-Output ""Dernier scan          : $threats""
} catch {
    Write-Output '[!] Windows Defender non accessible ou désactivé'
}
", onOutput, ct);
    }

    // ────────────────────────────────────────────────────────────────
    //  ALERTES DISQUE SMART  (ev_disk_warn)
    // ────────────────────────────────────────────────────────────────
    public static async Task GetDiskWarnings(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
Write-Output '--- ALERTES DISQUE (SMART) ---'
$events = Get-WinEvent -FilterHashtable @{
    LogName   = 'System'
    ProviderName = 'disk','nvme'
    Level     = 2,3
} -MaxEvents 20 -ErrorAction SilentlyContinue

if (-not $events) {
    Write-Output '[OK] Aucune alerte disque dans le journal système'
} else {
    foreach ($e in $events) {
        Write-Output ""[$($e.TimeCreated.ToString('dd/MM HH:mm'))] $($e.Message.Substring(0, [Math]::Min(120, $e.Message.Length)))""
    }
}

# Infos disques physiques
Write-Output ''
Write-Output '--- DISQUES PHYSIQUES ---'
Get-PhysicalDisk | ForEach-Object {
    Write-Output ""$($_.FriendlyName) — $($_.MediaType) — $($_.HealthStatus) — $([math]::Round($_.Size/1GB,1)) GB""
}
", onOutput, ct);
    }

    // ────────────────────────────────────────────────────────────────
    //  OBSERVATEUR D'ÉVÉNEMENTS  (rpt_eventvwr)
    // ────────────────────────────────────────────────────────────────
    public static Task LaunchEventViewer(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Lancement de l'Observateur d'événements...");
        Core.CommandRunner.StartProcess("eventvwr.msc");
        onOutput("[OK] Observateur d'événements ouvert");
        return Task.CompletedTask;
    }

    // ────────────────────────────────────────────────────────────────
    //  MONITEUR DE FIABILITÉ  (rpt_perfmon_rel)
    // ────────────────────────────────────────────────────────────────
    public static Task LaunchReliabilityMonitor(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Lancement du Moniteur de fiabilité (historique des plantages)...");
        Core.CommandRunner.StartProcess("perfmon.exe", "/rel");
        onOutput("[OK] Moniteur de fiabilité ouvert");
        return Task.CompletedTask;
    }

    // ────────────────────────────────────────────────────────────────
    //  RAPPORT DE PERFORMANCES  (rpt_perfmon_report)
    // ────────────────────────────────────────────────────────────────
    public static Task LaunchPerfmonReport(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Génération du rapport de performances système...");
        Core.CommandRunner.StartProcess("perfmon.exe", "/report");
        onOutput("[OK] Rapport de performances lancé (peut prendre 1 min)");
        return Task.CompletedTask;
    }

    // ────────────────────────────────────────────────────────────────
    //  OUTIL ANTIVIRUS AUTONOME MRT  (rpt_mrt)
    // ────────────────────────────────────────────────────────────────
    public static Task LaunchMrt(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Lancement de l'outil de suppression de logiciels malveillants (MRT)...");
        Core.CommandRunner.StartProcess("mrt.exe");
        onOutput("[OK] MRT ouvert — suivez l'assistant pour analyser votre PC");
        return Task.CompletedTask;
    }

    // ────────────────────────────────────────────────────────────────
    //  RAPPORT WI-FI  (rpt_wifi_report)
    // ────────────────────────────────────────────────────────────────
    public static async Task GetWifiReport(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Génération du rapport Wi-Fi...");
        await Core.CommandRunner.RunAsync("netsh wlan show wlanreport", onOutput, ct);
        var sourceReportPath = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            @"Microsoft\Windows\WlanReport\wlan-report-latest.html");

        if (System.IO.File.Exists(sourceReportPath))
        {
            var targetDir = Core.FolderDialogHelper.AskAndCreateDestinationFolder(
                title: "Sélectionnez l'emplacement où sauvegarder le rapport Wi-Fi",
                folderPrefix: "Rapport_WiFi",
                confirmQuestion: "Le rapport Wi-Fi complet a été généré.\n\nSouhaitez-vous copier ce rapport dans un dossier de sauvegarde de votre choix ?",
                confirmTitle: "Rapport Wi-Fi — Alex Toolkit"
            );

            if (!string.IsNullOrEmpty(targetDir))
            {
                string destFile = System.IO.Path.Combine(targetDir, "wlan-report.html");
                System.IO.File.Copy(sourceReportPath, destFile, true);
                onOutput($"✅ [SUCCÈS] Rapport Wi-Fi copié dans : {destFile}");
                Core.CommandRunner.StartProcess(destFile);
                Core.FolderDialogHelper.OpenInExplorer(targetDir);
            }
            else
            {
                onOutput($"[OK] Rapport Wi-Fi : {sourceReportPath}");
                Core.CommandRunner.StartProcess(sourceReportPath);
            }
        }
        else
        {
            onOutput("[i] Rapport généré — cherchez wlan-report-latest.html dans ProgramData\\Microsoft\\Windows\\WlanReport");
        }
    }
}
