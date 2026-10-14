namespace ScriptsWindows.Models;

/// <summary>
/// Représente un outil/script individuel.
/// Correspond à une entrée t[N] du .bat original.
/// </summary>
public class ScriptTool
{
    /// <summary>Identifiant unique (ex: "sys_report", "cl_temp")</summary>
    public required string Id { get; init; }

    /// <summary>Nom affiché</summary>
    public required string Name { get; init; }

    /// <summary>Titre affiché formaté selon la typographie française (acronymes préservés, pas de majuscules inutiles)</summary>
    public string Title => Views.SectionViewBuilder.FormatFrenchTitle(Name);

    /// <summary>Commande ou exécutable principal lancé par le script (ex: sfc.exe /scannow, winget.exe)</summary>
    public string Command { get; init; } = string.Empty;

    /// <summary>Retourne la commande définie ou celle résolue par le catalogue</summary>
    public string ExecutableOrCommand => !string.IsNullOrWhiteSpace(Command)
        ? Command
        : Core.ToolRegistry.GetCommand(Id);

    /// <summary>Description courte (ex: "CPU, RAM, GPU, Stockage, Réseau")</summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>Catégorie parente</summary>
    public required string Category { get; init; }

    /// <summary>Sous-titre / En-tête de sous-section interne</summary>
    public string SubHeading { get; init; } = string.Empty;

    /// <summary>Icône Emoji ou chemin vers une image</summary>
    public string Icon { get; init; } = "⚙️";

    /// <summary>Indique si l'outil requiert les droits administrateur</summary>
    public bool RequiresAdmin { get; init; } = true;

    /// <summary>Si true, l'outil n'est pas affiché dans le menu principal (HIDDEN dans le .bat)</summary>
    public bool IsHidden { get; init; }

    /// <summary>Action à exécuter quand l'outil est lancé</summary>
    public Func<Action<string>, CancellationToken, Task>? Action { get; set; }

    /// <summary>Points clés ou actions réalisées affichés sous forme de liste dans la carte</summary>
    public string[] Features { get; init; } = Array.Empty<string>();

    /// <summary>Tags pour la recherche</summary>
    public string[] Tags { get; init; } = Array.Empty<string>();
}
