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

        BtnTabSystem.Background   = tag == "TabSystem"   ? new SolidColorBrush(Color.FromRgb(0x4F, 0x46, 0xE5)) : Brushes.Transparent;
        BtnTabSystem.Foreground   = tag == "TabSystem"   ? Brushes.White : new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF));

        BtnTabCpuRam.Background   = tag == "TabCpuRam"   ? new SolidColorBrush(Color.FromRgb(0x4F, 0x46, 0xE5)) : Brushes.Transparent;
        BtnTabCpuRam.Foreground   = tag == "TabCpuRam"   ? Brushes.White : new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF));

        BtnTabStorage.Background  = tag == "TabStorage"  ? new SolidColorBrush(Color.FromRgb(0x4F, 0x46, 0xE5)) : Brushes.Transparent;
        BtnTabStorage.Foreground  = tag == "TabStorage"  ? Brushes.White : new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF));

        BtnTabPowerNet.Background = tag == "TabPowerNet" ? new SolidColorBrush(Color.FromRgb(0x4F, 0x46, 0xE5)) : Brushes.Transparent;
        BtnTabPowerNet.Foreground = tag == "TabPowerNet" ? Brushes.White : new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF));

        BtnTabTools.Background    = tag == "TabTools"    ? new SolidColorBrush(Color.FromRgb(0x4F, 0x46, 0xE5)) : Brushes.Transparent;
        BtnTabTools.Foreground    = tag == "TabTools"    ? Brushes.White : new SolidColorBrush(Color.FromRgb(0x9C, 0xA3, 0xAF));
    }

    private async Task LoadDiagnosticDataAsync()
    {
        TxtFeedback.Text = "Collecte des données de diagnostic système...";

        await Task.Run(async () =>
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
                string jsonCpu = await PowerShellRunner.RunAsync(psCpu, _ct);

                string psTemp = @"
try {
    $temps = Get-WmiObject MSAcpi_ThermalZoneTemperature -Namespace root/wmi -ErrorAction Stop
    $vals = @($temps | ForEach-Object { [math]::Round(($_.CurrentTemperature - 2732) / 10, 1) })
    $vals -join ' °C, ' + ' °C'
} catch { 'Non mesurable via sondes ACPI standard' }
";
                string tempStr = (await PowerShellRunner.RunAsync(psTemp, _ct)).Trim();

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
                string jsonRam = await PowerShellRunner.RunAsync(psRam, _ct);
                await Dispatcher.InvokeAsync(() => ParseCpuRamJson(jsonCpu, tempStr, jsonRam));
            }
            catch { }

            // 3. Disques & Stockage
            try
            {
                string psDisks = @"
$disks = @(Get-PhysicalDisk | ForEach-Object {
    [PSCustomObject]@{
        Name = $_.FriendlyName
        BusType = $_.BusType
        MediaType = $_.MediaType
        Size = ""$([math]::Round($_.Size / 1GB, 1)) GB""
        Health = $_.HealthStatus
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
        });

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

                    var color = health.Equals("Healthy", StringComparison.OrdinalIgnoreCase) || health.Equals("Sain", StringComparison.OrdinalIgnoreCase)
                        ? new SolidColorBrush(Color.FromRgb(0x34, 0xD3, 0x99))
                        : new SolidColorBrush(Color.FromRgb(0xF8, 0x71, 0x71));

                    PhysicalDisks.Add(new PhysicalDiskItem
                    {
                        Name = name,
                        BusType = bus,
                        MediaType = media,
                        SizeText = size,
                        HealthStatus = health,
                        HealthColor = color
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

                    var barColor = used > 90 ? new SolidColorBrush(Color.FromRgb(0xEF, 0x44, 0x44))
                                : used > 75 ? new SolidColorBrush(Color.FromRgb(0xF5, 0x9E, 0x0B))
                                : new SolidColorBrush(Color.FromRgb(0x3B, 0x82, 0xF6));

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
        TxtFeedback.Text = "Génération de l'audit complet du système...";
        await DiagnosticService.GetMotherboardBiosInfo(_onOutput, _ct);
        TxtFeedback.Text = "Audit complet terminé.";
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
