using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ScriptsWindows.Core;
using ScriptsWindows.Services;

namespace ScriptsWindows.Views.Controls;

public partial class DiagnosticCenterDialog : Window
{
    private readonly Action<string> _onOutput;
    private readonly CancellationToken _ct;

    public ObservableCollection<RamStickItem> RamSticks { get; } = new();
    public ObservableCollection<PhysicalDiskItem> PhysicalDisks { get; } = new();
    public ObservableCollection<VolumeItem> Volumes { get; } = new();
    public ObservableCollection<NetworkAdapterItem> NetworkAdapters { get; } = new();

    public DiagnosticCenterDialog(Action<string> onOutput, CancellationToken ct = default)
    {
        InitializeComponent();
        _onOutput = onOutput;
        _ct = ct;

        ItemsRamSticks.ItemsSource = RamSticks;
        ItemsPhysicalDisks.ItemsSource = PhysicalDisks;
        ItemsVolumesList.ItemsSource = Volumes;
        ItemsNetworkList.ItemsSource = NetworkAdapters;

        Loaded += async (_, _) => await LoadDiagnosticDataAsync();
    }

    private void TabButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn || btn.Tag is not string tag) return;

        PanelSystem.Visibility   = tag == "TabSystem"   ? Visibility.Visible : Visibility.Collapsed;
        PanelCpuRam.Visibility   = tag == "TabCpuRam"   ? Visibility.Visible : Visibility.Collapsed;
        PanelStorage.Visibility  = tag == "TabStorage"  ? Visibility.Visible : Visibility.Collapsed;
        PanelPowerNet.Visibility = tag == "TabPowerNet" ? Visibility.Visible : Visibility.Collapsed;
        PanelTools.Visibility    = tag == "TabTools"    ? Visibility.Visible : Visibility.Collapsed;

        BtnTabSystem.Background   = tag == "TabSystem"   ? (SolidColorBrush)ThemeManager.GetBrush("Accent") : Brushes.Transparent;
        BtnTabSystem.Foreground   = tag == "TabSystem"   ? (SolidColorBrush)ThemeManager.GetBrush("OnAccent") : (SolidColorBrush)ThemeManager.GetBrush("TextSecondary");

        BtnTabCpuRam.Background   = tag == "TabCpuRam"   ? (SolidColorBrush)ThemeManager.GetBrush("Accent") : Brushes.Transparent;
        BtnTabCpuRam.Foreground   = tag == "TabCpuRam"   ? (SolidColorBrush)ThemeManager.GetBrush("OnAccent") : (SolidColorBrush)ThemeManager.GetBrush("TextSecondary");

        BtnTabStorage.Background  = tag == "TabStorage"  ? (SolidColorBrush)ThemeManager.GetBrush("Accent") : Brushes.Transparent;
        BtnTabStorage.Foreground  = tag == "TabStorage"  ? (SolidColorBrush)ThemeManager.GetBrush("OnAccent") : (SolidColorBrush)ThemeManager.GetBrush("TextSecondary");

        BtnTabPowerNet.Background = tag == "TabPowerNet" ? (SolidColorBrush)ThemeManager.GetBrush("Accent") : Brushes.Transparent;
        BtnTabPowerNet.Foreground = tag == "TabPowerNet" ? (SolidColorBrush)ThemeManager.GetBrush("OnAccent") : (SolidColorBrush)ThemeManager.GetBrush("TextSecondary");

        BtnTabTools.Background    = tag == "TabTools"    ? (SolidColorBrush)ThemeManager.GetBrush("Accent") : Brushes.Transparent;
        BtnTabTools.Foreground    = tag == "TabTools"    ? (SolidColorBrush)ThemeManager.GetBrush("OnAccent") : (SolidColorBrush)ThemeManager.GetBrush("TextSecondary");
    }

    private async Task LoadDiagnosticDataAsync()
    {
        TxtFeedback.Text = "Collecte des données de diagnostic système...";

        // Les 4 groupes de collecte sont indépendants : ils s'exécutent en parallèle
        // (chaque lancement de powershell.exe coûte ~0,5-1 s, en série cela faisait 4-6 s d'attente).
        async Task LoadSystemAsync()
        {
            // 1. Système & Carte Mère
            try
            {
                string psSys = @"
$cs = Get-CimInstance Win32_ComputerSystem
$os = Get-CimInstance Win32_OperatingSystem
$bb = Get-CimInstance Win32_BaseBoard
$bios = Get-CimInstance Win32_BIOS
[PSCustomObject]@{
    Name = $cs.Name
    Manufacturer = $cs.Manufacturer
    Model = $cs.Model
    OsCaption = $os.Caption
    OsBuild = ""$($os.Version) (Build $($os.BuildNumber))""
    MoboManufacturer = $bb.Manufacturer
    MoboProduct = $bb.Product
    BiosVersion = $bios.SMBIOSBIOSVersion
    BiosDate = if ($bios.ReleaseDate) { (Get-Date $bios.ReleaseDate).ToString('dd/MM/yyyy') } else { '—' }
} | ConvertTo-Json -Compress
";
                string jsonSys = await PowerShellRunner.RunAsync(psSys, _ct);
                await Dispatcher.InvokeAsync(() => ParseSystemJson(jsonSys));
            }
            catch { }
        }

        async Task LoadCpuRamAsync()
        {
            // 2. CPU & RAM
            try
            {
                string psCpu = @"
$cpu = Get-CimInstance Win32_Processor
[PSCustomObject]@{
    Name = $cpu.Name
    Cores = ""$($cpu.NumberOfCores) Cœurs / $($cpu.NumberOfLogicalProcessors) Threads""
    Speed = ""$($cpu.MaxClockSpeed) MHz""
} | ConvertTo-Json -Compress
";
                var cpuTask = PowerShellRunner.RunAsync(psCpu, _ct);

                string psTemp = @"
try {
    $temps = Get-WmiObject MSAcpi_ThermalZoneTemperature -Namespace root/wmi -ErrorAction Stop
    $vals = @($temps | ForEach-Object { [math]::Round(($_.CurrentTemperature - 2732) / 10, 1) })
    $vals -join ' °C, ' + ' °C'
} catch { 'Non mesurable via sondes ACPI standard' }
";
                var tempTask = PowerShellRunner.RunAsync(psTemp, _ct);

                string psRam = @"
$mem = Get-CimInstance Win32_PhysicalMemory
$memTypes = @{ 20='DDR'; 21='DDR2'; 24='DDR3'; 26='DDR4'; 29='LPDDR3'; 30='LPDDR4'; 31='LPDDR4X'; 34='DDR5'; 35='LPDDR5X'; 36='LPDDR5' }
$sticks = @($mem | ForEach-Object {
    $capGB = [math]::Round($_.Capacity / 1GB, 2)
    $rawType = if ($_.SMBIOSMemoryType -gt 0) { $_.SMBIOSMemoryType } else { $_.MemoryType }
    $tName = if ($memTypes.ContainsKey([int]$rawType)) { $memTypes[[int]$rawType] } else { 'DDR/Standard' }
    [PSCustomObject]@{
        Slot = $_.DeviceLocator
        Cap = ""$capGB GB""
        Speed = ""$($_.Speed) MHz""
        Type = $tName
        Manuf = $_.Manufacturer
        CapRaw = $_.Capacity
    }
})
$total = [math]::Round(($mem | Measure-Object -Property Capacity -Sum).Sum / 1GB, 2)
[PSCustomObject]@{
    TotalGB = ""$total GB installés""
    Sticks = $sticks
} | ConvertTo-Json -Compress
";
                var ramTask = PowerShellRunner.RunAsync(psRam, _ct);

                await Task.WhenAll(cpuTask, tempTask, ramTask);
                string jsonCpu = cpuTask.Result;
                string tempStr = tempTask.Result.Trim();
                string jsonRam = ramTask.Result;
                await Dispatcher.InvokeAsync(() => ParseCpuRamJson(jsonCpu, tempStr, jsonRam));
            }
            catch { }
        }

        async Task LoadStorageAsync()
        {
            // 3. Disques & Stockage
            try
            {
                string psDisks = @"
$disks = @(Get-PhysicalDisk | ForEach-Object {
    # Compteurs S.M.A.R.T. (nécessite les droits administrateur ; certains contrôleurs ne les exposent pas)
    $rc = try { $_ | Get-StorageReliabilityCounter -ErrorAction Stop } catch { $null }
    $parts = @()
    $alert = $false
    if ($rc) {
        if ($rc.Wear -ne $null)          { $parts += ""Usure $($rc.Wear) %"";            if ($rc.Wear -ge 80) { $alert = $true } }
        if ($rc.Temperature -gt 0)       { $parts += ""$($rc.Temperature) °C"";          if ($rc.Temperature -ge 60) { $alert = $true } }
        if ($rc.PowerOnHours -gt 0)      { $parts += ""$($rc.PowerOnHours) h de fonctionnement"" }
        $errs = [int]$rc.ReadErrorsUncorrected + [int]$rc.WriteErrorsUncorrected
        if ($rc.ReadErrorsUncorrected -ne $null -or $rc.WriteErrorsUncorrected -ne $null) {
            $parts += ""$errs erreur(s) non corrigée(s)""; if ($errs -gt 0) { $alert = $true }
        }
    }
    [PSCustomObject]@{
        Name = $_.FriendlyName
        BusType = ""$($_.BusType)""
        MediaType = ""$($_.MediaType)""
        Size = ""$([math]::Round($_.Size / 1GB, 1)) GB""
        Health = ""$($_.HealthStatus)""
        Smart = if ($parts.Count) { $parts -join ' • ' } else { 'Compteurs S.M.A.R.T. non exposés par ce contrôleur' }
        SmartAlert = $alert
    }
})
$vols = @(Get-Volume | Where-Object { $_.DriveLetter -ne $null } | ForEach-Object {
    $totalGB = [math]::Round($_.Size / 1GB, 1)
    $freeGB = [math]::Round($_.SizeRemaining / 1GB, 1)
    $usedGB = [math]::Round(($_.Size - $_.SizeRemaining) / 1GB, 1)
    $pct = if ($_.Size -gt 0) { [math]::Round((($_.Size - $_.SizeRemaining) / $_.Size) * 100, 1) } else { 0 }
    [PSCustomObject]@{
        Drive = ""$($_.DriveLetter):""
        Label = if ($_.FileSystemLabel) { $_.FileSystemLabel } else { 'Disque local' }
        Space = ""$freeGB GB libres / $totalGB GB ($pct% utilisé)""
        UsedPct = $pct
    }
})
[PSCustomObject]@{
    Disks = $disks
    Volumes = $vols
} | ConvertTo-Json -Compress
";
                string jsonStorage = await PowerShellRunner.RunAsync(psDisks, _ct);
                await Dispatcher.InvokeAsync(() => ParseStorageJson(jsonStorage));
            }
            catch { }
        }

        async Task LoadPowerNetAsync()
        {
            // 4. Batterie & Réseau
            try
            {
                string psPowerNet = @"
$bat = Get-CimInstance Win32_Battery -ErrorAction SilentlyContinue
$batLvl = if ($bat) { ""$($bat.EstimatedChargeRemaining)%"" } else { 'Non disponible (PC fixe)' }
$batStat = if ($bat) {
    switch ($bat.BatteryStatus) {
        1 { 'En décharge' } 2 { 'Secteur (AC)' } 3 { 'Charge complète' }
        6 { 'En charge' } default { ""Statut $($bat.BatteryStatus)"" }
    }
} else { 'Alimentation secteur' }
$batTime = if ($bat -and $bat.EstimatedRunTime -gt 0 -and $bat.EstimatedRunTime -lt 60000) {
    ""$([math]::Floor($bat.EstimatedRunTime/60)) h $($bat.EstimatedRunTime%60) min""
} else { 'Illimitée (sur secteur)' }
$batVolt = if ($bat -and $bat.DesignVoltage -gt 0) { ""$([math]::Round($bat.DesignVoltage/1000.0, 2)) V"" } else { '—' }

$nets = @(Get-NetIPConfiguration | Where-Object { $_.IPv4Address } | ForEach-Object {
    [PSCustomObject]@{
        Alias = $_.InterfaceAlias
        Ip = $_.IPv4Address.IPAddress
        Gateway = if ($_.IPv4DefaultGateway) { $_.IPv4DefaultGateway.NextHop } else { '—' }
        Dns = if ($_.DnsServer) { ($_.DnsServer.ServerAddresses -join ', ') } else { '—' }
    }
})

[PSCustomObject]@{
    BatLvl = $batLvl
    BatStat = $batStat
    BatTime = $batTime
    BatVolt = $batVolt
    Networks = $nets
} | ConvertTo-Json -Compress
";
                string jsonPowerNet = await PowerShellRunner.RunAsync(psPowerNet, _ct);
                await Dispatcher.InvokeAsync(() => ParsePowerNetJson(jsonPowerNet));
            }
            catch { }
        }

        // Task.Run : le démarrage des processus (Process.Start) se fait hors du thread UI
        await Task.WhenAll(
            Task.Run(() => LoadSystemAsync()),
            Task.Run(() => LoadCpuRamAsync()),
            Task.Run(() => LoadStorageAsync()),
            Task.Run(() => LoadPowerNetAsync()));

        TxtFeedback.Text = "Diagnostic système et matériel à jour.";
    }

    private void ParseSystemJson(string json)
    {
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;
            TxtComputerName.Text = root.GetProperty("Name").GetString() ?? Environment.MachineName;
            TxtComputerModel.Text = $"{root.GetProperty("Manufacturer").GetString()} — {root.GetProperty("Model").GetString()}";
            TxtOsCaption.Text = root.GetProperty("OsCaption").GetString() ?? "Windows";
            TxtOsBuild.Text = root.GetProperty("OsBuild").GetString() ?? "";
            TxtMoboManufacturer.Text = root.GetProperty("MoboManufacturer").GetString() ?? "—";
            TxtMoboProduct.Text = root.GetProperty("MoboProduct").GetString() ?? "—";
            TxtBiosVersion.Text = root.GetProperty("BiosVersion").GetString() ?? "—";
            TxtBiosDate.Text = root.GetProperty("BiosDate").GetString() ?? "—";
        }
        catch { }
    }

    private void ParseCpuRamJson(string jsonCpu, string tempStr, string jsonRam)
    {
        try
        {
            using var docCpu = System.Text.Json.JsonDocument.Parse(jsonCpu);
            var rootCpu = docCpu.RootElement;
            TxtCpuName.Text = rootCpu.GetProperty("Name").GetString() ?? "Processeur Windows";
            TxtCpuCores.Text = rootCpu.GetProperty("Cores").GetString() ?? "—";
            TxtCpuSpeed.Text = rootCpu.GetProperty("Speed").GetString() ?? "—";
            TxtCpuTemp.Text = !string.IsNullOrWhiteSpace(tempStr) ? tempStr : "Non accessible";
        }
        catch { }

        try
        {
            using var docRam = System.Text.Json.JsonDocument.Parse(jsonRam);
            var rootRam = docRam.RootElement;
            TxtRamTotal.Text = rootRam.GetProperty("TotalGB").GetString() ?? "—";

            RamSticks.Clear();
            if (rootRam.TryGetProperty("Sticks", out var sticksElem) && sticksElem.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                int idx = 1;
                foreach (var s in sticksElem.EnumerateArray())
                {
                    string slot = s.TryGetProperty("Slot", out var sl) ? sl.GetString() ?? $"Slot {idx}" : $"Slot {idx}";
                    string cap = s.TryGetProperty("Cap", out var cp) ? cp.GetString() ?? "" : "";
                    string speed = s.TryGetProperty("Speed", out var sp) ? sp.GetString() ?? "" : "";
                    string type = s.TryGetProperty("Type", out var tp) ? tp.GetString() ?? "" : "";
                    string manuf = s.TryGetProperty("Manuf", out var mf) ? mf.GetString() ?? "" : "";

                    RamSticks.Add(new RamStickItem
                    {
                        SlotName = slot,
                        CapacityText = cap,
                        SpeedText = speed,
                        TypeText = type,
                        Manufacturer = manuf
                    });
                    idx++;
                }
            }
        }
        catch { }
    }

    private void ParseStorageJson(string json)
    {
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;

            PhysicalDisks.Clear();
            if (root.TryGetProperty("Disks", out var disksElem) && disksElem.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                foreach (var d in disksElem.EnumerateArray())
                {
                    string name = d.GetProperty("Name").GetString() ?? "";
                    string bus = d.GetProperty("BusType").GetString() ?? "";
                    string media = d.GetProperty("MediaType").GetString() ?? "Disque";
                    string size = d.GetProperty("Size").GetString() ?? "";
                    string health = d.GetProperty("Health").GetString() ?? "Healthy";
                    string smart = d.TryGetProperty("Smart", out var sm) ? sm.GetString() ?? "" : "";
                    bool smartAlert = d.TryGetProperty("SmartAlert", out var sa) && sa.ValueKind == System.Text.Json.JsonValueKind.True;

                    bool healthy = health.Equals("Healthy", StringComparison.OrdinalIgnoreCase) || health.Equals("Sain", StringComparison.OrdinalIgnoreCase);
                    var color = !healthy ? (SolidColorBrush)ThemeManager.GetBrush("DangerText")
                              : smartAlert ? (SolidColorBrush)ThemeManager.GetBrush("WarningText")
                              : (SolidColorBrush)ThemeManager.GetBrush("SuccessText");
                    if (healthy) health = smartAlert ? "À surveiller" : "Sain";

                    PhysicalDisks.Add(new PhysicalDiskItem
                    {
                        Name = name,
                        BusType = bus,
                        MediaType = media,
                        SizeText = size,
                        HealthStatus = health,
                        HealthColor = color,
                        SmartDetails = smart
                    });
                }
            }

            Volumes.Clear();
            if (root.TryGetProperty("Volumes", out var volsElem) && volsElem.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                foreach (var v in volsElem.EnumerateArray())
                {
                    string drive = v.GetProperty("Drive").GetString() ?? "";
                    string label = v.GetProperty("Label").GetString() ?? "";
                    string space = v.GetProperty("Space").GetString() ?? "";
                    double used = v.GetProperty("UsedPct").GetDouble();

                    var barColor = used > 90 ? (SolidColorBrush)ThemeManager.GetBrush("Danger")
                                : used > 75 ? (SolidColorBrush)ThemeManager.GetBrush("Warning")
                                : (SolidColorBrush)ThemeManager.GetBrush("Accent");

                    Volumes.Add(new VolumeItem
                    {
                        DriveLetter = drive,
                        Label = label,
                        SpaceUsageText = space,
                        UsedPercent = used,
                        UsageBarColor = barColor
                    });
                }
            }
        }
        catch { }
    }

    private void ParsePowerNetJson(string json)
    {
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;

            TxtBatteryLevel.Text = root.GetProperty("BatLvl").GetString() ?? "—";
            TxtBatteryStatus.Text = root.GetProperty("BatStat").GetString() ?? "—";
            TxtBatteryRuntime.Text = root.GetProperty("BatTime").GetString() ?? "—";
            TxtBatteryVoltage.Text = root.GetProperty("BatVolt").GetString() ?? "—";

            NetworkAdapters.Clear();
            if (root.TryGetProperty("Networks", out var netsElem) && netsElem.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                foreach (var n in netsElem.EnumerateArray())
                {
                    NetworkAdapters.Add(new NetworkAdapterItem
                    {
                        InterfaceAlias = n.GetProperty("Alias").GetString() ?? "",
                        IpAddress = n.GetProperty("Ip").GetString() ?? "",
                        Gateway = n.GetProperty("Gateway").GetString() ?? "—",
                        DnsServers = n.GetProperty("Dns").GetString() ?? "—"
                    });
                }
            }
        }
        catch { }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // BOUTONS D'ACTIONS ET OUTILS
    // ─────────────────────────────────────────────────────────────────────────
    private void BtnMdsched_Click(object sender, RoutedEventArgs e)
    {
        _onOutput("[i] Lancement du Diagnostic de mémoire Windows (mdsched.exe)...");
        CommandRunner.StartProcess("mdsched.exe");
        TxtFeedback.Text = "Outil de diagnostic mémoire Windows ouvert.";
    }

    private async void BtnBatteryReport_Click(object sender, RoutedEventArgs e)
    {
        TxtFeedback.Text = "Génération du rapport de batterie...";
        await DiagnosticService.GetBatteryReport(_onOutput, _ct);
        TxtFeedback.Text = "Rapport de batterie généré.";
    }

    private async void BtnPingTest_Click(object sender, RoutedEventArgs e)
    {
        TxtFeedback.Text = "Test ping en direct en cours...";
        await DiagnosticService.DiagNetwork(_onOutput, _ct);
        TxtFeedback.Text = "Test de connectivité réseau terminé.";
    }

    private async void BtnOpenStartup_Click(object sender, RoutedEventArgs e)
    {
        TxtFeedback.Text = "Ouverture du gestionnaire des programmes au démarrage...";
        await DiagnosticService.OpenStartupManagerDialog(_onOutput, _ct);
    }

    private async void BtnRunSfc_Click(object sender, RoutedEventArgs e)
    {
        TxtFeedback.Text = "Lancement de la commande SFC /scannow...";
        await RepairService.RunSfc(_onOutput, _ct);
        TxtFeedback.Text = "Analyse SFC terminée.";
    }

    private void BtnMsinfo_Click(object sender, RoutedEventArgs e)
    {
        CommandRunner.StartProcess("msinfo32.exe");
        TxtFeedback.Text = "Informations système ouvertes.";
    }

    private void BtnDxdiag_Click(object sender, RoutedEventArgs e)
    {
        CommandRunner.StartProcess("dxdiag.exe");
        TxtFeedback.Text = "Outil de diagnostic DirectX (DxDiag) ouvert.";
    }

    private void BtnDevmgmt_Click(object sender, RoutedEventArgs e)
    {
        CommandRunner.StartProcess("devmgmt.msc");
        TxtFeedback.Text = "Gestionnaire de périphériques ouvert.";
    }

    private void BtnResmon_Click(object sender, RoutedEventArgs e)
    {
        CommandRunner.StartProcess("resmon.exe");
        TxtFeedback.Text = "Moniteur de ressources ouvert.";
    }

    private async void BtnFullAudit_Click(object sender, RoutedEventArgs e)
    {
        // Audit complet en lecture seule : chaque volet est écrit dans la console, un échec n'interrompt pas les suivants
        var steps = new (string Label, Func<Task> Run)[]
        {
            ("Carte mère, BIOS, processeur et système", () => DiagnosticService.GetMotherboardBiosInfo(_onOutput, _ct)),
            ("Mémoire vive",                             () => DiagnosticService.GetRamUsage(_onOutput, _ct)),
            ("Disques : santé S.M.A.R.T. et espace",     () => DiskService.GetDisksHealthAndStorage(_onOutput, _ct)),
            ("Alertes disque du journal Windows",        () => DiagnosticService.GetDiskWarnings(_onOutput, _ct)),
            ("Batterie",                                 () => DiagnosticService.GetBatteryStatus(_onOutput, _ct)),
            ("Températures",                             () => DiagnosticService.GetTemperatureReport(_onOutput, _ct)),
            ("Protection antivirus",                     () => DiagnosticService.GetDefenderStatus(_onOutput, _ct)),
        };

        if (sender is Button btn) btn.IsEnabled = false;
        try
        {
            for (int i = 0; i < steps.Length; i++)
            {
                _ct.ThrowIfCancellationRequested();
                TxtFeedback.Text = $"Audit complet : {steps[i].Label} ({i + 1}/{steps.Length})...";
                _onOutput($"\n[i] ── {steps[i].Label} ──");
                try { await steps[i].Run(); }
                catch (OperationCanceledException) { throw; }
                catch (Exception ex) { _onOutput($"[!] {steps[i].Label} : {ex.Message}"); }
            }
            TxtFeedback.Text = "Audit complet terminé : résultats dans la console.";
            _onOutput("[OK] Audit complet terminé.");
        }
        catch (OperationCanceledException)
        {
            TxtFeedback.Text = "Audit interrompu.";
        }
        finally
        {
            if (sender is Button b) b.IsEnabled = true;
        }
    }

    private async void BtnRefreshAll_Click(object sender, RoutedEventArgs e)
    {
        await LoadDiagnosticDataAsync();
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}

public class RamStickItem
{
    public string SlotName { get; set; } = "";
    public string CapacityText { get; set; } = "";
    public string SpeedText { get; set; } = "";
    public string TypeText { get; set; } = "";
    public string Manufacturer { get; set; } = "";
}

public class PhysicalDiskItem
{
    public string Name { get; set; } = "";
    public string BusType { get; set; } = "";
    public string MediaType { get; set; } = "";
    public string SizeText { get; set; } = "";
    public string HealthStatus { get; set; } = "";
    public Brush HealthColor { get; set; } = Brushes.LightGreen;
    public string SmartDetails { get; set; } = "";
}

public class VolumeItem
{
    public string DriveLetter { get; set; } = "";
    public string Label { get; set; } = "";
    public string SpaceUsageText { get; set; } = "";
    public double UsedPercent { get; set; }
    public Brush UsageBarColor { get; set; } = Brushes.DodgerBlue;
}

public class NetworkAdapterItem
{
    public string InterfaceAlias { get; set; } = "";
    public string IpAddress { get; set; } = "";
    public string Gateway { get; set; } = "";
    public string DnsServers { get; set; } = "";
}
