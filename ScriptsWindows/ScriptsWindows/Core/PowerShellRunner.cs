using System.Diagnostics;
using System.Text;

namespace ScriptsWindows.Core;

/// <summary>
/// Exécute des scripts PowerShell de façon asynchrone avec sortie live.
/// Équivalent de tous les blocs "powershell -NoProfile -ExecutionPolicy Bypass -Command ..."
/// présents dans le .bat original.
/// </summary>
public class PowerShellRunner
{
    private static readonly string PsExe = "powershell.exe";

    /// <summary>
    /// Exécute un script PS inline et retourne la sortie complète.
    /// </summary>
    public static async Task<string> RunAsync(string script, CancellationToken ct = default)
    {
        var sb = new StringBuilder();
        await RunAsync(script, line => sb.AppendLine(line), ct);
        return sb.ToString();
    }

    /// <summary>
    /// Exécute un script PS inline avec callback ligne par ligne (sortie live).
    /// </summary>
    public static async Task RunAsync(string script, Action<string> onOutput, CancellationToken ct = default)
    {
        var psi = new ProcessStartInfo(PsExe)
        {
            Arguments = $"-NoProfile -NonInteractive -ExecutionPolicy Bypass -Command \"[Console]::OutputEncoding = [System.Text.Encoding]::UTF8; $OutputEncoding = [System.Text.Encoding]::UTF8; {EscapeForArg(script)}\"",
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            UseShellExecute        = false,
            CreateNoWindow         = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding  = Encoding.UTF8,
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
    /// Exécute un fichier .ps1 avec sortie live.
    /// </summary>
    public static async Task RunFileAsync(string psFilePath, Action<string> onOutput, CancellationToken ct = default)
    {
        var psi = new ProcessStartInfo(PsExe)
        {
            Arguments = $"-NoProfile -NonInteractive -ExecutionPolicy Bypass -File \"{psFilePath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            UseShellExecute        = false,
            CreateNoWindow         = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding  = Encoding.UTF8,
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
    /// Exécute un script PS et retourne la valeur exitcode.
    /// Utile pour les tests oui/non (ex : vérifier si BitLocker est actif).
    /// </summary>
    public static async Task<int> RunAndGetExitCodeAsync(string script, CancellationToken ct = default)
    {
        var psi = new ProcessStartInfo(PsExe)
        {
            Arguments = $"-NoProfile -NonInteractive -ExecutionPolicy Bypass -Command \"{EscapeForArg(script)}\"",
            UseShellExecute = false,
            CreateNoWindow  = true,
        };

        using var proc = Process.Start(psi)!;
        await proc.WaitForExitAsync(ct);
        return proc.ExitCode;
    }

    private static string EscapeForArg(string script) =>
        script.Replace("\"", "\\\"");
}
