using ScriptsWindows.Core;

namespace ScriptsWindows.Services;

/// <summary>
/// Service DNS — porte install_cloudflare_full, install_google_full, restore_dns, show_dns_config.
/// </summary>
public class DnsService
{
    // Détecte l'interface réseau active (équivalent :get_interface du .bat)
    private static async Task<string> GetActiveInterface()
    {
        var result = await PowerShellRunner.RunAsync(
            "Get-NetAdapter | Where-Object { $_.Status -eq 'Up' -and $_.Virtual -eq $false } | Sort-Object -Property LinkSpeed -Descending | Select-Object -First 1 -ExpandProperty Name");
        return result.Trim();
    }

    // DNS Cloudflare  (install_cloudflare_full)
    public static async Task SetCloudflare(Action<string> onOutput, CancellationToken ct = default)
    {
        var iface = await GetActiveInterface();
        if (string.IsNullOrWhiteSpace(iface))
        {
            onOutput("[ERR] Aucune interface réseau active trouvée"); return;
        }
        onOutput($"[i] Interface : {iface}");
        onOutput("[i] Configuration DNS Cloudflare IPv4 + IPv6...");
        await CommandRunner.RunAsync("net stop Dnscache /y", onOutput, ct);
        await CommandRunner.RunAsync($"netsh interface ip set dns \"{iface}\" static 1.1.1.1", onOutput, ct);
        await CommandRunner.RunAsync($"netsh interface ip add dns \"{iface}\" 1.0.0.1 index=2", onOutput, ct);
        await CommandRunner.RunAsync($"netsh interface ipv6 set dns \"{iface}\" static 2606:4700:4700::1111", onOutput, ct);
        await CommandRunner.RunAsync($"netsh interface ipv6 add dns \"{iface}\" 2606:4700:4700::1001 index=2", onOutput, ct);
        await CommandRunner.RunAsync("net start Dnscache", onOutput, ct);
        await CommandRunner.RunAsync("ipconfig /flushdns", onOutput, ct);
        onOutput("[OK] DNS Cloudflare configurés : 1.1.1.1 / 1.0.0.1");
    }

    // DNS Google  (install_google_full)
    public static async Task SetGoogle(Action<string> onOutput, CancellationToken ct = default)
    {
        var iface = await GetActiveInterface();
        if (string.IsNullOrWhiteSpace(iface))
        {
            onOutput("[ERR] Aucune interface réseau active trouvée"); return;
        }
        onOutput($"[i] Interface : {iface}");
        onOutput("[i] Configuration DNS Google IPv4 + IPv6...");
        await CommandRunner.RunAsync("net stop Dnscache /y", onOutput, ct);
        await CommandRunner.RunAsync($"netsh interface ip set dns \"{iface}\" static 8.8.8.8", onOutput, ct);
        await CommandRunner.RunAsync($"netsh interface ip add dns \"{iface}\" 8.8.4.4 index=2", onOutput, ct);
        await CommandRunner.RunAsync($"netsh interface ipv6 set dns \"{iface}\" static 2001:4860:4860::8888", onOutput, ct);
        await CommandRunner.RunAsync($"netsh interface ipv6 add dns \"{iface}\" 2001:4860:4860::8844 index=2", onOutput, ct);
        await CommandRunner.RunAsync("net start Dnscache", onOutput, ct);
        await CommandRunner.RunAsync("ipconfig /flushdns", onOutput, ct);
        onOutput("[OK] DNS Google configurés : 8.8.8.8 / 8.8.4.4");
    }

    // Restaurer DHCP  (restore_dns)
    public static async Task RestoreDhcp(Action<string> onOutput, CancellationToken ct = default)
    {
        var iface = await GetActiveInterface();
        if (string.IsNullOrWhiteSpace(iface))
        {
            onOutput("[ERR] Aucune interface réseau active trouvée"); return;
        }
        await CommandRunner.RunAsync("net stop Dnscache /y", onOutput, ct);
        await CommandRunner.RunAsync($"netsh interface ip set dns \"{iface}\" dhcp", onOutput, ct);
        await CommandRunner.RunAsync($"netsh interface ipv6 set dns \"{iface}\" dhcp", onOutput, ct);
        await CommandRunner.RunAsync("net start Dnscache", onOutput, ct);
        await CommandRunner.RunAsync("ipconfig /flushdns", onOutput, ct);
        onOutput("[OK] DNS DHCP restaurés (automatique)");
    }

    // Afficher configuration DNS  (show_dns_config)
    public static async Task ShowConfig(Action<string> onOutput, CancellationToken ct = default)
    {
        var iface = await GetActiveInterface();
        onOutput($"[i] Interface active : {iface}");
        onOutput("--- DNS IPv4 ---");
        await CommandRunner.RunAsync($"netsh interface ip show dns \"{iface}\"", onOutput, ct);
        onOutput("--- DNS IPv6 ---");
        await CommandRunner.RunAsync($"netsh interface ipv6 show dns \"{iface}\"", onOutput, ct);
    }
}
