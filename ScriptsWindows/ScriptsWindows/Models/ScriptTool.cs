namespace ScriptsWindows.Models;

/// <summary>
/// Représente un outil/script individuel.
/// Correspond à une entrée t[N] du .bat original.
/// </summary>
public class ScriptTool
{
    /// <summary>Identifiant unique (ex: "sys_report", "cl_temp")</summary>
    public required string Id { get; init; }

    /// <summary>Nom affiché (ex: "Rapport Système")</summary>
    public required string Name { get; init; }

    /// <summary>Description courte (ex: "CPU, RAM, GPU, Stockage, Réseau")</summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>Catégorie parente</summary>
    public required string Category { get; init; }

    /// <summary>Icône Emoji ou chemin vers une image</summary>
    public string Icon { get; init; } = "⚙️";

    /// <summary>Si true, l'outil n'est pas affiché dans le menu principal (HIDDEN dans le .bat)</summary>
    public bool IsHidden { get; init; }

    /// <summary>Action à exécuter quand l'outil est lancé</summary>
    public Func<IProgress<string>, CancellationToken, Task>? Action { get; set; }

    /// <summary>Tags pour la recherche</summary>
    public string[] Tags { get; init; } = Array.Empty<string>();
}
