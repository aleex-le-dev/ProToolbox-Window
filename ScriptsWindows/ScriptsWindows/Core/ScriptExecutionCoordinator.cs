using System.Windows;
using ScriptsWindows.Models;

namespace ScriptsWindows.Core;

/// <summary>
/// Coordonne l'exécution exclusive des scripts (Option 1 : un seul script à la fois).
/// Gère le verrouillage global des boutons et l'interruption utilisateur (CancellationToken).
/// </summary>
public static class ScriptExecutionCoordinator
{
    private static readonly object _lock = new();
    private static CancellationTokenSource? _activeCts;
    private static string? _activeToolId;
    private static string? _activeToolTitle;

    public static bool IsRunning
    {
        get
        {
            lock (_lock)
            {
                return _activeCts != null;
            }
        }
    }

    public static string? ActiveToolId
    {
        get
        {
            lock (_lock)
            {
                return _activeToolId;
            }
        }
    }

    public static string? ActiveToolTitle
    {
        get
        {
            lock (_lock)
            {
                return _activeToolTitle;
            }
        }
    }

    /// <summary>
    /// Déclenché quand l'état d'exécution change (true = démarré, false = terminé/stoppé).
    /// Toujours invoqué sur le thread UI pour faciliter le binding/refresh.
    /// </summary>
    public static event Action<bool>? StateChanged;

    /// <summary>
    /// Tente de démarrer un script de manière exclusive.
    /// Retourne false si un script est déjà en cours.
    /// </summary>
    public static bool TryBeginExecution(ScriptTool tool, out CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            if (_activeCts != null)
            {
                cancellationToken = CancellationToken.None;
                return false;
            }

            _activeCts = new CancellationTokenSource();
            _activeToolId = tool.Id;
            _activeToolTitle = tool.Title;
            cancellationToken = _activeCts.Token;
        }

        NotifyStateChanged(true);
        return true;
    }

    /// <summary>
    /// Interrompt le script en cours d'exécution.
    /// </summary>
    public static void StopActiveExecution()
    {
        CancellationTokenSource? cts;
        lock (_lock)
        {
            cts = _activeCts;
        }

        if (cts != null && !cts.IsCancellationRequested)
        {
            try
            {
                cts.Cancel();
            }
            catch { }
        }
    }

    /// <summary>
    /// Marque la fin d'exécution du script actuel et déverrouille l'application.
    /// </summary>
    public static void EndExecution(string toolId)
    {
        bool changed = false;
        lock (_lock)
        {
            if (_activeToolId == toolId || toolId == "")
            {
                _activeCts?.Dispose();
                _activeCts = null;
                _activeToolId = null;
                _activeToolTitle = null;
                changed = true;
            }
        }

        if (changed)
        {
            NotifyStateChanged(false);
        }
    }

    private static void NotifyStateChanged(bool isRunning)
    {
        if (Application.Current?.Dispatcher != null)
        {
            Application.Current.Dispatcher.InvokeAsync(() => StateChanged?.Invoke(isRunning));
        }
        else
        {
            StateChanged?.Invoke(isRunning);
        }
    }
}
