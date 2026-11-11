namespace ScriptsWindows.Core;

/// <summary>
/// Glyphes de la police d'icônes Windows (Segoe Fluent Icons / Segoe MDL2 Assets).
/// Source unique pour la barre latérale, les cartes d'outils et les en-têtes de section :
/// une icône par catégorie, cohérente partout (remplace les emojis).
/// </summary>
public static class IconGlyphs
{
    public const string Favorite       = "";
    public const string FavoriteFilled = "";
    public const string ChevronDown    = "";
    public const string ChevronRight   = "";
    public const string Play           = "";
    public const string Pin            = "";
    public const string Settings       = "";
    public const string Search         = "";

    /// <summary>Icône d'une catégorie d'outils (même glyphe que dans la barre latérale).</summary>
    public static string ForCategory(string? category) => (category ?? "").ToUpperInvariant() switch
    {
        "APPLICATIONS"     => "", // AllApps
        "DEMARRAGE"        => "", // PowerButton
        "DIAGNOSTIC"       => "", // Diagnostic
        "DISQUE"           => "", // HardDrive
        "MATERIEL"         => "", // TVMonitor
        "MISES_A_JOUR"     => "", // Sync
        "NETTOYAGE"        => "", // Delete
        "NOUVEAUTE"        => "", // Lightbulb
        "PERSONNALISATION" => "", // Color
        "REPARATION"       => "", // Repair
        "RESEAU"           => "", // Globe
        "SECRETS"          => "", // Permissions
        "SECURITE"         => "", // Shield
        _                  => ""  // Setting
    };
}
