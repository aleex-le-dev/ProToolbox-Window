using ScriptsWindows.Core;

namespace ScriptsWindows.Services;

/// <summary>Service réseau — porte net_*, cyber_* du .bat.</summary>
public class NetworkService
{
    public static async Task FlushDns(Action<string> onOutput, CancellationToken ct = default)
        => await CommandRunner.RunAsync("ipconfig /flushdns", onOutput, ct);

    public static async Task DisplayDns(Action<string> onOutput, CancellationToken ct = default)
        => await CommandRunner.RunAsync("ipconfig /displaydns", onOutput, ct);

    public static async Task ClearArp(Action<string> onOutput, CancellationToken ct = default)
        => await CommandRunner.RunAsync("arp -d *", onOutput, ct);

    public static async Task DisplayArp(Action<string> onOutput, CancellationToken ct = default)
        => await CommandRunner.RunAsync("arp -a", onOutput, ct);

    public static async Task RenewIp(Action<string> onOutput, CancellationToken ct = default)
    {
        await CommandRunner.RunAsync("ipconfig /release", onOutput, ct);
        await CommandRunner.RunAsync("ipconfig /renew", onOutput, ct);
    }

    public static async Task ResetTcpIp(Action<string> onOutput, CancellationToken ct = default)
        => await CommandRunner.RunAsync("netsh int ip reset", onOutput, ct);

    public static async Task ResetWinsock(Action<string> onOutput, CancellationToken ct = default)
        => await CommandRunner.RunAsync("netsh winsock reset", onOutput, ct);

    public static async Task ResetAll(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Grand Nettoyage Réseau (7 étapes)...");
        await FlushDns(onOutput, ct);
        await ClearArp(onOutput, ct);
        await ResetWinsock(onOutput, ct);
        await ResetTcpIp(onOutput, ct);
        await RenewIp(onOutput, ct);
        onOutput("[OK] Réseau réinitialisé — redémarrage conseillé");
    }

    public static async Task RestartAdapters(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
$adapters = Get-NetAdapter | Where-Object { $_.Status -eq 'Up' }
foreach ($a in $adapters) {
    Disable-NetAdapter -Name $a.Name -Confirm:$false -ErrorAction SilentlyContinue
    Start-Sleep 1
    Enable-NetAdapter -Name $a.Name -Confirm:$false -ErrorAction SilentlyContinue
    Write-Output ""[OK] $($a.Name) redémarrée""
}
", onOutput, ct);
    }

    // Triage connectivité  (cyber_triage)
    public static async Task RunTriage(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
Write-Output '--- TRIAGE DE CONNECTIVITÉ ---'
# IP locale
Get-NetIPAddress -AddressFamily IPv4 | Where-Object { $_.InterfaceAlias -notlike '*Loopback*' } | ForEach-Object {
    Write-Output ""[IP] $($_.InterfaceAlias) : $($_.IPAddress)""
}
# Gateway
$gw = (Get-NetRoute -DestinationPrefix '0.0.0.0/0' | Sort-Object RouteMetric | Select-Object -First 1).NextHop
Write-Output ""[GW] Passerelle : $gw""
# Ping Gateway
$r = Test-Connection $gw -Count 2 -Quiet -ErrorAction SilentlyContinue
Write-Output ""$(if($r){'[OK]'}else{'[ERR]'}) Ping Gateway ($gw)""
# DNS
$dns = (Get-DnsClientServerAddress -AddressFamily IPv4 | Select-Object -First 1).ServerAddresses
Write-Output ""[DNS] Serveurs : $($dns -join ', ')""
# Internet
$inet = Test-Connection 'google.com' -Count 1 -Quiet -ErrorAction SilentlyContinue
Write-Output ""$(if($inet){'[OK]'}else{'[ERR]'}) Accès Internet""
", onOutput, ct);
    }

    // Scanner LAN  (cyber_lan_auto)
    public static async Task ScanLan(Action<string> onOutput, CancellationToken ct = default)
    {
        onOutput("[i] Scan du réseau local en cours (peut prendre 1-2 min)...");
        await PowerShellRunner.RunAsync(@"
$gateway = (Get-NetRoute -DestinationPrefix '0.0.0.0/0' | Sort-Object RouteMetric | Select-Object -First 1).NextHop
$subnet  = ($gateway -replace '\.\d+$', '')
Write-Output ""[i] Plage scannée : $subnet.1-254""
1..254 | ForEach-Object -Parallel {
    $ip = ""$using:subnet.$_""
    if (Test-Connection $ip -Count 1 -TimeoutSeconds 1 -Quiet) {
        $name = try { [System.Net.Dns]::GetHostEntry($ip).HostName } catch { 'inconnu' }
        Write-Output ""[HOTE] $ip — $name""
    }
} -ThrottleLimit 50
Write-Output '[OK] Scan LAN terminé'
", onOutput, ct);
    }

    // Analyse flux réseau  (cyber_flux_analysis)
    public static async Task AnalyzeFlux(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
Write-Output '--- CONNEXIONS ACTIVES ---'
Get-NetTCPConnection | Where-Object { $_.State -eq 'Established' } | ForEach-Object {
    $proc = try { (Get-Process -Id $_.OwningProcess -ErrorAction SilentlyContinue).ProcessName } catch { 'inconnu' }
    Write-Output ""$proc — Local: $($_.LocalAddress):$($_.LocalPort) → Remote: $($_.RemoteAddress):$($_.RemotePort)""
} | Sort-Object | Select-Object -First 30
", onOutput, ct);
    }

    // Scan Wi-Fi  (net_wifi_scan)
    public static async Task ScanWifi(Action<string> onOutput, CancellationToken ct = default)
    {
        await CommandRunner.RunAsync("netsh wlan show networks mode=Bssid", onOutput, ct);
    }

    // Audit adaptateurs  (cyber_adapter_audit)
    public static async Task AuditAdapters(Action<string> onOutput, CancellationToken ct = default)
    {
        await PowerShellRunner.RunAsync(@"
Get-NetAdapter | ForEach-Object {
    Write-Output ""[$($_.Status)] $($_.Name)""
    Write-Output ""  MAC     : $($_.MacAddress)""
    Write-Output ""  Vitesse : $($_.LinkSpeed)""
    Write-Output ""  Type    : $($_.MediaType)""
}
", onOutput, ct);
    }
}
