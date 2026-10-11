using System.Diagnostics;
using System.Text;

namespace ScriptsWindows.Core;

/// <summary>
/// Exécute des commandes CMD / netsh / winget / reg etc. de façon asynchrone.
/// Remplace tous les blocs "Process.Start cmd /c ..." du .bat.
/// </summary>
public class CommandRunner
{
    /// <summary>
    /// Exécute une commande cmd et retourne la sortie complète.
    /// </summary>
    public static async Task<string> RunAsync(string command, CancellationToken ct = default)
    {
        var sb = new StringBuilder();
        await RunAsync(command, line => sb.AppendLine(line), ct);
        return sb.ToString();
    }

    /// <summary>
    /// Exécute une commande cmd avec callback ligne par ligne (sortie live).
    /// </summary>
    public static async Task RunAsync(string command, Action<string> onOutput, CancellationToken ct = default)
    {
        var psi = new ProcessStartInfo("cmd.exe")
        {
            Arguments = $"/c {command}",
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            UseShellExecute        = false,
            CreateNoWindow         = true,
            StandardOutputEncoding = Encoding.GetEncoding(850), // OEM pour cmd
            StandardErrorEncoding  = Encoding.GetEncoding(850),
        };

        using var proc = new Process { StartInfo = psi, EnableRaisingEvents = true };

        proc.OutputDataReceived += (_, e) =>
        {
            if (e.Data != null) onOutput(e.Data);
        };
        proc.ErrorDataReceived += (_, e) =>
        {
            if (e.Data != null) onOutput($"[ERR] {e.Data}");
        };

        proc.Start();
        proc.BeginOutputReadLine();
        proc.BeginErrorReadLine();

        await proc.WaitForExitAsync(ct);
    }

    /// <summary>
    /// Lance une commande dans une fenêtre console visible (pour SFC, DISM, CHKDSK).
    /// L'utilisateur peut voir la progression dans la console.
    /// </summary>
    public static async Task RunVisibleAsync(string command, CancellationToken ct = default)
    {
        var psi = new ProcessStartInfo("cmd.exe")
        {
            Arguments = $"/k {command}",
            UseShellExecute = true,
            Verb = "runas",
        };

        using var proc = Process.Start(psi)!;
        await proc.WaitForExitAsync(ct);
    }

    /// <summary>
    /// Exécute une commande et retourne l'exitcode.
    /// </summary>
    public static async Task<int> RunAndGetExitCodeAsync(string command, CancellationToken ct = default)
    {
        var psi = new ProcessStartInfo("cmd.exe")
        {
            Arguments = $"/c {command}",
            UseShellExecute = false,
            CreateNoWindow  = true,
        };

        using var proc = Process.Start(psi)!;
        await proc.WaitForExitAsync(ct);
        return proc.ExitCode;
    }

    /// <summary>
    /// Ouvre une URL dans le navigateur par défaut.
    /// </summary>
    public static void OpenUrl(string url)
    {
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }

    /// <summary>
    /// Lance un exécutable avec ShellExecute (ex: cleanmgr, msconfig...).
    /// </summary>
    public static void StartProcess(string exeName, string args = "")
    {
        Process.Start(new ProcessStartInfo(exeName, args) { UseShellExecute = true });
    }
}
