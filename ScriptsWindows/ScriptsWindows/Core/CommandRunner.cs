using System.Diagnostics;
using System.IO;
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

    static CommandRunner()
    {
        try
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        catch { }
    }

    /// <summary>
    /// Nom de l'exécutable appelé (premier mot de la commande, sans chemin, guillemets ni ".exe").
    /// Évite les faux positifs d'un simple Contains (ex : un nom d'interface contenant « sfc »).
    /// </summary>
    private static string GetExecutableName(string command)
    {
        var span = command.AsSpan().TrimStart();
        if (span.Length > 0 && span[0] == '"')
        {
            span = span[1..];
            int endQuote = span.IndexOf('"');
            if (endQuote >= 0) span = span[..endQuote];
        }
        else
        {
            int space = span.IndexOfAny(' ', '\t');
            if (space >= 0) span = span[..space];
        }

        var name = Path.GetFileName(span.ToString());
        return name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ? name[..^4] : name;
    }

    private static Encoding GetSafeEncoding(string command)
    {
        string exe = GetExecutableName(command);

        // SFC émet obligatoirement en Unicode (UTF-16 LE) sous Windows
        if (exe.Equals("sfc", StringComparison.OrdinalIgnoreCase))
        {
            return Encoding.Unicode;
        }

        // Winget et PowerShell émettent généralement en UTF-8
        if (exe.Equals("winget", StringComparison.OrdinalIgnoreCase) ||
            exe.Equals("powershell", StringComparison.OrdinalIgnoreCase) ||
            exe.Equals("pwsh", StringComparison.OrdinalIgnoreCase))
        {
            return Encoding.UTF8;
        }

        try
        {
            return Encoding.GetEncoding(850);
        }
        catch
        {
            return Encoding.UTF8;
        }
    }

    /// <summary>
    /// Lit un flux de façon asynchrone et émet chaque ligne immédiatement.
    /// Gère à la fois les fins de ligne standard (\n, \r\n) et les retours chariot seuls (\r)
    /// couramment utilisés par les outils à progression en direct (SFC, DISM, winget, curl).
    /// </summary>
    private static async Task ReadStreamLinesAsync(StreamReader reader, Action<string> onOutput, CancellationToken ct)
    {
        var buffer = new char[512];
        var currentLine = new StringBuilder();
        bool lastWasCr = false;

        while (true)
        {
            int read;
            try
            {
                read = await reader.ReadAsync(buffer.AsMemory(0, buffer.Length), ct);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            if (read <= 0) break;

            for (int i = 0; i < read; i++)
            {
                char c = buffer[i];

                if (c == '\r')
                {
                    lastWasCr = true;
                    string line = currentLine.ToString().Trim();
                    currentLine.Clear();
                    if (!string.IsNullOrEmpty(line))
                    {
                        onOutput(line);
                    }
                }
                else if (c == '\n')
                {
                    if (lastWasCr)
                    {
                        // Séquence \r\n : déjà émise lors du \r
                        lastWasCr = false;
                        continue;
                    }
                    lastWasCr = false;
                    string line = currentLine.ToString().Trim();
                    currentLine.Clear();
                    if (!string.IsNullOrEmpty(line))
                    {
                        onOutput(line);
                    }
                }
                else
                {
                    lastWasCr = false;
                    if (c != '\0')
                    {
                        currentLine.Append(c);
                    }
                }
            }
        }

        if (currentLine.Length > 0)
        {
            string remaining = currentLine.ToString().Trim();
            if (!string.IsNullOrEmpty(remaining))
            {
                onOutput(remaining);
            }
        }
    }

    /// <summary>
    /// Exécute une commande cmd avec callback ligne par ligne (sortie live).
    /// </summary>
    public static async Task RunAsync(string command, Action<string> onOutput, CancellationToken ct = default)
    {
        var encoding = GetSafeEncoding(command);

        var psi = new ProcessStartInfo("cmd.exe")
        {
            Arguments = $"/c {command}",
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            UseShellExecute        = false,
            CreateNoWindow         = true,
            StandardOutputEncoding = encoding,
            StandardErrorEncoding  = encoding,
        };

        using var proc = new Process { StartInfo = psi, EnableRaisingEvents = true };
        proc.Start();

        var outputTask = ReadStreamLinesAsync(proc.StandardOutput, onOutput, ct);
        var errorTask  = ReadStreamLinesAsync(proc.StandardError, line => onOutput($"[ERR] {line}"), ct);

        using var reg = ct.CanBeCanceled ? ct.Register(() =>
        {
            try { proc.Kill(entireProcessTree: true); } catch { }
        }) : default;

        try
        {
            await Task.WhenAll(outputTask, errorTask, proc.WaitForExitAsync(ct));
        }
        catch (OperationCanceledException)
        {
            try { proc.Kill(entireProcessTree: true); } catch { }
            throw;
        }
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
        // using : libère le handle du processus (on ne le suit pas)
        using var _ = Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }

    /// <summary>
    /// Lance un exécutable avec ShellExecute (ex: cleanmgr, msconfig...).
    /// </summary>
    public static void StartProcess(string exeName, string args = "")
    {
        using var _ = Process.Start(new ProcessStartInfo(exeName, args) { UseShellExecute = true });
    }
}
