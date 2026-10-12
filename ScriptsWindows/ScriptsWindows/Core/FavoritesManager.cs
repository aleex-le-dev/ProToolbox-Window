using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScriptsWindows.Core;

/// <summary>
/// Gestion des favoris utilisateur.
/// Compatible avec le fichier favoris.txt du .bat original
/// (une ligne = un ID d'outil, ex: "sys_report", "cl_temp"...).
/// </summary>
public class FavoritesManager
{
    private static readonly Lazy<FavoritesManager> _instance = new(() =>
    {
        string localFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ScriptsWindows");
        Directory.CreateDirectory(localFolder);
        string file = Path.Combine(localFolder, "favoris.txt");
        return new FavoritesManager(file);
    });

    public static FavoritesManager Default => _instance.Value;

    public static event Action<string, bool>? FavoriteChanged;

    public static bool IsFavorite(string toolId) => Default.IsFav(toolId);
    public static void ToggleFavorite(string toolId) => Default.Toggle(toolId);

    private readonly string _filePath;
    private HashSet<string> _favorites;

    public FavoritesManager(string filePath)
    {
        _filePath  = filePath;
        _favorites = Load();
    }

    /// <summary>Retourne la liste des IDs d'outils favoris.</summary>
    public IReadOnlySet<string> Favorites => _favorites;

    /// <summary>Vérifie si un outil est dans les favoris.</summary>
    public bool IsFav(string toolId) => _favorites.Contains(toolId);

    /// <summary>Ajoute ou retire un outil des favoris (toggle).</summary>
    public void Toggle(string toolId)
    {
        bool added;
        if (_favorites.Contains(toolId))
        {
            _favorites.Remove(toolId);
            added = false;
        }
        else
        {
            _favorites.Add(toolId);
            added = true;
        }

        Save();
        FavoriteChanged?.Invoke(toolId, added);
    }

    /// <summary>Ajoute un outil aux favoris.</summary>
    public void Add(string toolId)
    {
        if (_favorites.Add(toolId))
        {
            Save();
            FavoriteChanged?.Invoke(toolId, true);
        }
    }

    /// <summary>Retire un outil des favoris.</summary>
    public void Remove(string toolId)
    {
        if (_favorites.Remove(toolId))
        {
            Save();
            FavoriteChanged?.Invoke(toolId, false);
        }
    }

    private HashSet<string> Load()
    {
        if (!File.Exists(_filePath))
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        return File.ReadAllLines(_filePath)
                   .Select(l => l.Trim())
                   .Where(l => !string.IsNullOrWhiteSpace(l))
                   .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private void Save()
    {
        var dir = Path.GetDirectoryName(_filePath);
        if (dir != null && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        File.WriteAllLines(_filePath, _favorites.OrderBy(x => x));
    }
}
