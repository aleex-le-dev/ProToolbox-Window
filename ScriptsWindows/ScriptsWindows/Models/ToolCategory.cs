namespace ScriptsWindows.Models;

/// <summary>
/// Représente une catégorie d'outils (DIAGNOSTIC, REPARATION, etc.)
/// Correspond aux séparateurs "---:CATEGORIE" du .bat.
/// </summary>
public class ToolCategory
{
    /// <summary>Identifiant technique (ex: "DIAGNOSTIC")</summary>
    public required string Id { get; init; }

    /// <summary>Nom affiché dans la sidebar</summary>
    public required string Name { get; init; }

    /// <summary>Icône Emoji affichée dans la sidebar</summary>
    public required string Icon { get; init; }

    /// <summary>Couleur accent de la catégorie (hex)</summary>
    public string AccentColor { get; init; } = "#6366F1";

    /// <summary>Outils appartenant à cette catégorie</summary>
    public List<ScriptTool> Tools { get; init; } = new();

    /// <summary>Sous-menus de la catégorie (ex: menu Réseau → Wi-Fi, LAN, Distant)</summary>
    public List<ToolSubMenu> SubMenus { get; init; } = new();
}

/// <summary>Sous-menu d'une catégorie</summary>
public class ToolSubMenu
{
    public required string Name { get; init; }
    public string Icon { get; init; } = "📁";
    public List<ScriptTool> Tools { get; init; } = new();
}
