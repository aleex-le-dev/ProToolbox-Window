using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Xml.Linq;
using ScriptsWindows.Core;

namespace ScriptsWindows.Services;

/// <summary>Résultat d'une évaluation WinSAT (notes 1,0 – 9,9 + mesures brutes).</summary>
public class BenchmarkResult
{
    public string Label { get; set; } = "";
    public DateTime Date { get; set; }
    public bool IsReference { get; set; }

    public double SystemScore { get; set; }
    public double CpuScore { get; set; }
    public double MemoryScore { get; set; }
    public double GraphicsScore { get; set; }
    public double GamingScore { get; set; }
    public double DiskScore { get; set; }

    public double CpuMultiMBs { get; set; }      // compression multi-cœur
    public double CpuSingleMBs { get; set; }     // compression mono-cœur
    public double MemBandwidthMBs { get; set; }
    public double DiskSeqReadMBs { get; set; }
    public double DiskRandReadMBs { get; set; }
    public double DesktopFps { get; set; }       // composition du bureau (DWM)

    public string DisplayName => IsReference ? Label : $"Ce PC — {Date:dd/MM/yyyy HH:mm}";
}

/// <summary>Matériel du PC (affiché avec les notes).</summary>
public class BenchmarkHardware
{
    public string Cpu { get; set; } = "";
    public string Gpu { get; set; } = "";
    public string Ram { get; set; } = "";
    public string Disk { get; set; } = "";
    public bool OnBattery { get; set; }
}

/// <summary>
/// Banc d'essai basé sur WinSAT (outil officiel Windows) : lecture des résultats XML détaillés,
/// historique local, profils de comparaison et lancement d'une évaluation avec suivi des étapes.
/// </summary>
public static class BenchmarkService
{
    private static readonly string DataStore = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"Performance\WinSAT\DataStore");
    private static readonly string HistoryFile = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ScriptsWindows", "benchmarks.json");

    /// <summary>Repères indicatifs (ordres de grandeur WinSAT observés sur des configurations typiques).</summary>
    public static readonly IReadOnlyList<BenchmarkResult> References = new List<BenchmarkResult>
    {
        new() { IsReference = true, Label = "PC de bureau ancien (2015, disque dur)",
                SystemScore = 5.9, CpuScore = 7.2, MemoryScore = 7.4, GraphicsScore = 5.6, GamingScore = 6.0, DiskScore = 5.9,
                CpuMultiMBs = 250, CpuSingleMBs = 70, MemBandwidthMBs = 15000, DiskSeqReadMBs = 150, DiskRandReadMBs = 2, DesktopFps = 300 },
        new() { IsReference = true, Label = "Portable d'entrée de gamme (2020, SSD SATA)",
                SystemScore = 6.6, CpuScore = 8.0, MemoryScore = 8.1, GraphicsScore = 6.6, GamingScore = 9.9, DiskScore = 8.1,
                CpuMultiMBs = 350, CpuSingleMBs = 85, MemBandwidthMBs = 25000, DiskSeqReadMBs = 520, DiskRandReadMBs = 250, DesktopFps = 700 },
        new() { IsReference = true, Label = "Portable milieu de gamme (2023, SSD NVMe)",
                SystemScore = 8.0, CpuScore = 8.8, MemoryScore = 9.0, GraphicsScore = 8.0, GamingScore = 9.9, DiskScore = 9.0,
                CpuMultiMBs = 450, CpuSingleMBs = 95, MemBandwidthMBs = 50000, DiskSeqReadMBs = 3000, DiskRandReadMBs = 1000, DesktopFps = 1400 },
        new() { IsReference = true, Label = "PC gamer (2024, NVMe Gen4)",
                SystemScore = 9.3, CpuScore = 9.3, MemoryScore = 9.4, GraphicsScore = 9.9, GamingScore = 9.9, DiskScore = 9.5,
                CpuMultiMBs = 700, CpuSingleMBs = 110, MemBandwidthMBs = 80000, DiskSeqReadMBs = 6000, DiskRandReadMBs = 2200, DesktopFps = 3000 },
    };

    private static double D(string? s) => double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : 0;

    /// <summary>Lit un fichier « Formal.Assessment » de WinSAT.</summary>
    public static BenchmarkResult? Parse(string xmlPath)
    {
        try
        {
            var doc = XDocument.Load(xmlPath);
            var spr = doc.Root?.Element("WinSPR");
            var metrics = doc.Root?.Element("Metrics");
            if (spr == null) return null;

            double Metric(string group, string name) => D(metrics?.Element(group)?.Element(name)?.Value);
            double Disk(string kind) => D(metrics?.Element("DiskMetrics")?.Elements("AvgThroughput")
                                              .FirstOrDefault(e => (string?)e.Attribute("kind") == kind)?.Value);

            return new BenchmarkResult
            {
                Date = File.GetLastWriteTime(xmlPath),
                SystemScore = D(spr.Element("SystemScore")?.Value),
                CpuScore = D(spr.Element("CpuScore")?.Value),
                MemoryScore = D(spr.Element("MemoryScore")?.Value),
                GraphicsScore = D(spr.Element("GraphicsScore")?.Value),
                GamingScore = D(spr.Element("GamingScore")?.Value),
                DiskScore = D(spr.Element("DiskScore")?.Value),
                CpuMultiMBs = Metric("CPUMetrics", "CompressionMetric"),
                CpuSingleMBs = Metric("CPUMetrics", "CompressionMetricUP"),
                MemBandwidthMBs = Metric("MemoryMetrics", "Bandwidth"),
                DiskSeqReadMBs = Disk("Sequential Read"),
                DiskRandReadMBs = Disk("Random Read"),
                DesktopFps = Metric("GraphicsMetrics", "DWMFps"),
            };
        }
        catch { return null; }
    }

    /// <summary>Historique : évaluations mémorisées + celles présentes dans le DataStore de Windows (sans doublon).</summary>
    public static List<BenchmarkResult> LoadHistory()
    {
        var history = new List<BenchmarkResult>();
        try
        {
            if (File.Exists(HistoryFile))
                history = JsonSerializer.Deserialize<List<BenchmarkResult>>(File.ReadAllText(HistoryFile)) ?? new();
        }
        catch { }

        try
        {
            if (Directory.Exists(DataStore))
                foreach (var f in Directory.GetFiles(DataStore, "*Formal.Assessment*.WinSAT.xml"))
                    if (Parse(f) is { SystemScore: > 0 } r && !history.Any(h => Math.Abs((h.Date - r.Date).TotalMinutes) < 1))
                        history.Add(r);
        }
        catch { }

        history = history.OrderByDescending(h => h.Date).ToList();
        SaveHistory(history);
        return history;
    }

    private static void SaveHistory(List<BenchmarkResult> history)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(HistoryFile)!);
            File.WriteAllText(HistoryFile, JsonSerializer.Serialize(history.Take(50).ToList(), new JsonSerializerOptions { WriteIndented = true }));
        }
        catch { }
    }

    public static async Task<BenchmarkHardware> GetHardwareAsync(CancellationToken ct = default)
    {
        string json = await PowerShellRunner.RunAsync(@"
$cpu = (Get-CimInstance Win32_Processor | Select-Object -First 1).Name
$gpu = (Get-CimInstance Win32_VideoController | Where-Object { $_.Name -notmatch 'Remote|Virtual|Basic' } | Select-Object -ExpandProperty Name) -join ' + '
$ram = [math]::Round((Get-CimInstance Win32_ComputerSystem).TotalPhysicalMemory / 1GB)
$mem = Get-CimInstance Win32_PhysicalMemory | Select-Object -First 1
$speed = if ($mem.ConfiguredClockSpeed) { "" à $($mem.ConfiguredClockSpeed) MT/s"" } else { '' }
$sys = (Get-Partition -DriveLetter $env:SystemDrive[0] -ErrorAction SilentlyContinue | Get-Disk -ErrorAction SilentlyContinue)
$pd = if ($sys) { Get-PhysicalDisk | Where-Object { $_.DeviceId -eq ""$($sys.Number)"" } | Select-Object -First 1 } else { $null }
$disk = if ($pd) { ""$($pd.FriendlyName) ($($pd.MediaType), $($pd.BusType))"" } else { '' }
$bat = Get-CimInstance Win32_Battery -ErrorAction SilentlyContinue | Select-Object -First 1
[PSCustomObject]@{ Cpu = ""$cpu"".Trim(); Gpu = ""$gpu""; Ram = ""$ram Go$speed""; Disk = $disk; OnBattery = [bool]($bat -and $bat.BatteryStatus -eq 1) } | ConvertTo-Json -Compress
", ct);
        try { return JsonSerializer.Deserialize<BenchmarkHardware>(json.Trim()) ?? new(); }
        catch { return new(); }
    }

    /// <summary>
    /// Lance « winsat formal » et signale chaque étape (0..1, libellé). Retourne le nouveau résultat (déjà ajouté à l'historique).
    /// </summary>
    public static async Task<BenchmarkResult?> RunAsync(Action<double, string> onStep, Action<string> log, CancellationToken ct)
    {
        // Étapes de « winsat formal », dans l'ordre d'exécution (mots-clés FR / EN de la sortie)
        var steps = new (string[] Keys, double Progress, string Label)[]
        {
            (new[] { "Dwm", "Bureau", "Desktop" },                         0.10, "Graphismes du bureau"),
            (new[] { "Direct3D", "D3D" },                                  0.25, "Graphismes 3D (jeux)"),
            (new[] { "Media Foundation", "MFMedia", "MOOBE", "vidéo", "Video" }, 0.45, "Lecture et encodage vidéo"),
            (new[] { "l?UC", "l'UC", "l’UC", "CPU", "processeur" },         0.55, "Processeur"),
            (new[] { "mémoire", "Memory" },                                0.75, "Mémoire vive"),
            (new[] { "disque", "stockage", "Disk", "Storage" },            0.85, "Disque"),
        };

        DateTime started = DateTime.Now;
        double current = 0;
        onStep(0.02, "Préparation de l'évaluation…");

        await CommandRunner.RunAsync("winsat formal -restart clean", line =>
        {
            log(line);
            foreach (var s in steps)
            {
                if (s.Progress > current && s.Keys.Any(k => line.Contains(k, StringComparison.OrdinalIgnoreCase)))
                {
                    current = s.Progress;
                    onStep(s.Progress, s.Label);
                    break;
                }
            }
        }, ct);

        onStep(0.98, "Lecture des résultats…");
        var latest = Directory.Exists(DataStore)
            ? Directory.GetFiles(DataStore, "*Formal.Assessment*.WinSAT.xml").Select(f => new FileInfo(f))
                       .Where(f => f.LastWriteTime >= started.AddMinutes(-1)).OrderByDescending(f => f.LastWriteTime).FirstOrDefault()
            : null;
        if (latest == null) return null;

        var result = Parse(latest.FullName);
        if (result != null)
        {
            var history = LoadHistory(); // ajoute automatiquement la nouvelle évaluation
            result = history.FirstOrDefault(h => Math.Abs((h.Date - result.Date).TotalMinutes) < 1) ?? result;
        }
        onStep(1, "Terminé");
        return result;
    }

    /// <summary>Verdict global lisible à partir de la note système.</summary>
    public static (string Title, string Detail, string BrushKey) Verdict(double score) => score switch
    {
        >= 8.5 => ("Excellent", "Très rapide pour tous les usages, y compris les logiciels exigeants.", "SuccessText"),
        >= 7.0 => ("Bon", "Fluide pour la bureautique, Internet, la photo et le multimédia.", "SuccessText"),
        >= 5.5 => ("Correct", "Suffisant pour la bureautique et Internet ; limité pour les usages lourds.", "WarningText"),
        > 0    => ("Lent", "Le PC manque de puissance : ralentissements probables au quotidien.", "DangerText"),
        _      => ("Non évalué", "Lancez l'évaluation pour obtenir une note.", "TextMuted")
    };

    /// <summary>Conseil associé au composant le plus faible (celui qui fixe la note globale).</summary>
    public static string Advice(string component, double score) => component switch
    {
        "Disque" when score < 6.5 => "Disque mécanique ou très lent : le remplacer par un SSD est l'amélioration la plus visible et la plus rentable.",
        "Disque" when score < 8.5 => "SSD SATA : un SSD NVMe (si la carte mère le permet) accélérerait les chargements.",
        "Mémoire vive" when score < 7.5 => "Mémoire lente ou en canal unique : ajouter une seconde barrette identique (double canal) peut nettement aider.",
        "Graphismes" when score < 7.0 => "Graphismes intégrés modestes : adaptés à la bureautique et à la vidéo, pas aux jeux récents.",
        "Graphismes 3D" when score < 7.0 => "Performances 3D faibles : mettre à jour le pilote graphique ou envisager une carte dédiée pour jouer.",
        "Processeur" when score < 7.5 => "Processeur limité : fermer les programmes au démarrage et éviter le multitâche lourd.",
        _ => "Configuration équilibrée : aucun composant ne bride nettement les autres."
    };
}
