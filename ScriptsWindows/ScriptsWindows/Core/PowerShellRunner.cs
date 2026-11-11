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
    private const string PsExe = "powershell.exe";
    private const string BaseArgs = "-NoProfile -NonInteractive -ExecutionPolicy Bypass";

    /// <summary>
    /// Préambule injecté avant chaque script inline :
    /// - sorties forcées en UTF-8 (accents préservés) ;
    /// - barre de progression désactivée (invisible en sortie redirigée, mais elle ralentit
    ///   fortement Invoke-WebRequest / Copy-Item sous Windows PowerShell 5.1).
    /// </summary>
    private const string Preamble =
        "[Console]::OutputEncoding = [System.Text.Encoding]::UTF8; $OutputEncoding = [System.Text.Encoding]::UTF8; $ProgressPreference = 'SilentlyContinue'; ";

    /// <summary>
    /// Exécute un script PS inline et retourne sa <b>sortie standard uniquement</b>.
    /// Le résultat est une donnée à interpréter (JSON, valeur…) : les messages d'erreur (stderr) en sont exclus,
    /// sinon un simple avertissement PowerShell (« [ERR] … ») rendrait le JSON illisible.
    /// Les erreurs sont conservées dans <see cref="LastErrors"/> pour le diagnostic.
    /// </summary>
    public static async Task<string> RunAsync(string script, CancellationToken ct = default)
    {
        var sb = new StringBuilder();
        var errors = new List<string>();
        await RunAsync(script, line =>
        {
            if (line.StartsWith("[ERR] ", StringComparison.Ordinal)) { lock (errors) errors.Add(line[6..]); }
            else lock (sb) sb.AppendLine(line);
        }, ct);
        LastErrors = errors;
        return sb.ToString();
    }

    /// <summary>Erreurs (stderr) du dernier appel à <see cref="RunAsync(string, CancellationToken)"/>.</summary>
    public static IReadOnlyList<string> LastErrors { get; private set; } = Array.Empty<string>();

    /// <summary>
    /// Exécute un script PS inline avec callback ligne par ligne (sortie live).
    /// </summary>
    public static Task RunAsync(string script, Action<string> onOutput, CancellationToken ct = default)
        => RunWithOutputAsync($"{BaseArgs} -Command \"{EscapeForArg(Preamble + script)}\"", onOutput, ct);

    /// <summary>
    /// Exécute un fichier .ps1 avec sortie live.
    /// </summary>
    public static Task RunFileAsync(string psFilePath, Action<string> onOutput, CancellationToken ct = default)
        => RunWithOutputAsync($"{BaseArgs} -File \"{EscapeForArg(psFilePath)}\"", onOutput, ct);

    /// <summary>
    /// Exécute un script PS et retourne la valeur exitcode.
    /// Utile pour les tests oui/non (ex : vérifier si BitLocker est actif).
    /// </summary>
    public static async Task<int> RunAndGetExitCodeAsync(string script, CancellationToken ct = default)
    {
        var psi = new ProcessStartInfo(PsExe)
        {
            Arguments = $"{BaseArgs} -Command \"{EscapeForArg(script)}\"",
            UseShellExecute = false,
            CreateNoWindow  = true,
        };

        using var proc = Process.Start(psi)!;
        await WaitOrKillAsync(proc, ct);
        return proc.ExitCode;
    }

    private static async Task RunWithOutputAsync(string arguments, Action<string> onOutput, CancellationToken ct)
    {
        var psi = new ProcessStartInfo(PsExe)
        {
            Arguments              = arguments,
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

        await WaitOrKillAsync(proc, ct);
    }

    /// <summary>Attend la fin du processus ; en cas d'annulation, tue toute l'arborescence.</summary>
    private static async Task WaitOrKillAsync(Process proc, CancellationToken ct)
    {
        using var reg = ct.CanBeCanceled
            ? ct.Register(() => { try { proc.Kill(entireProcessTree: true); } catch { } })
            : default;

        try
        {
            await proc.WaitForExitAsync(ct);
        }
        catch (OperationCanceledException)
        {
            try { proc.Kill(entireProcessTree: true); } catch { }
            throw;
        }
    }

    /// <summary>
    /// Échappe une chaîne destinée à être placée entre guillemets dans une ligne de commande Windows
    /// (règles CommandLineToArgvW) : les antislashs ne sont doublés que lorsqu'ils précèdent un guillemet
    /// ou la fin de chaîne. Sans antislash avant un guillemet, le résultat est identique à un simple
    /// remplacement de " par \".
    /// </summary>
    private static string EscapeForArg(string value)
    {
        var sb = new StringBuilder(value.Length + 16);
        int backslashes = 0;

        foreach (char c in value)
        {
            if (c == '\\')
            {
                backslashes++;
                continue;
            }

            if (c == '"')
            {
                sb.Append('\\', backslashes * 2 + 1);
                sb.Append('"');
            }
            else
            {
                sb.Append('\\', backslashes);
                sb.Append(c);
            }
            backslashes = 0;
        }

        // Antislashs finaux : suivis du guillemet fermant ajouté par l'appelant → à doubler
        sb.Append('\\', backslashes * 2);
        return sb.ToString();
    }
}
