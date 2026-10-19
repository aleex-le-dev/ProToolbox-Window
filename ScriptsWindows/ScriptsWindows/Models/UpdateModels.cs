using System;
using System.Collections.Generic;

namespace ScriptsWindows.Models;

/// <summary>
/// Informations relatives à une release GitHub / Miroir distant.
/// </summary>
public class ReleaseInfo
{
    public required string TagName { get; init; }
    public string VersionString => TagName.TrimStart('v', 'V');
    public Version? Version { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime PublishedAt { get; init; }
    public string HtmlUrl { get; init; } = string.Empty;
    public string DownloadUrl { get; init; } = string.Empty;
    public long FileSizeBytes { get; init; }
    public bool IsNewer { get; set; }
    public bool IsCurrent { get; set; }

    public string FileSizeFormatted
    {
        get
        {
            if (FileSizeBytes <= 0) return "Taille inconnue";
            double mb = FileSizeBytes / (1024.0 * 1024.0);
            return $"{mb:F1} Mo";
        }
    }

    public string PublishedAtFormatted => PublishedAt > DateTime.MinValue
        ? PublishedAt.ToLocalTime().ToString("dd/MM/yyyy à HH:mm")
        : "Date inconnue";
}

/// <summary>
/// Résultat d'une vérification de mise à jour.
/// </summary>
public class UpdateCheckResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public ReleaseInfo? LatestRelease { get; init; }
    public List<ReleaseInfo> AllReleases { get; init; } = new();
    public Version CurrentVersion { get; init; } = new Version(4, 1, 0);

    public bool HasUpdate => Success && LatestRelease != null && LatestRelease.IsNewer;
}

/// <summary>
/// Paramètres de mise à jour persistés localement.
/// </summary>
public class UpdateSettings
{
    public bool AutoCheckOnStartup { get; set; } = true;
    public DateTime? LastCheckDate { get; set; }
    public string? LastKnownLatestVersion { get; set; }
}
