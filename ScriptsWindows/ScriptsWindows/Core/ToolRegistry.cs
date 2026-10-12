using System;
using System.Collections.Generic;
using System.Linq;
using ScriptsWindows.Models;
using ScriptsWindows.Services;

namespace ScriptsWindows.Core;

/// <summary>
/// Répertoire central de tous les outils disponibles dans l'application (63 scripts répartis en 10 catégories).
/// Conforme à l'arborescence officielle ALEX TOOLKIT.
/// </summary>
public static class ToolRegistry
{
    private static readonly List<ScriptTool> _allTools = new();

    static ToolRegistry()
    {
        RegisterDiagnosticTools();       // 1. Diagnostic Matériel & Composants (13 scripts)
        RegisterRepairTools();           // 2. Réparation Intégrale (10 scripts)
        RegisterCleaningTools();         // 3. Nettoyage & Optimisation Espace (6 scripts)
        RegisterSecretsTools();          // 4. Récupération d'Identifiants & Secrets (5 scripts)
        RegisterNetworkTools();          // 5. Réseau & Configuration (6 scripts)
        RegisterDiskTools();             // 6. Disque Dur & Stockage (5 scripts)
        RegisterAppInstallationTools();  // 7. Installation d'Applications (6 scripts)
        RegisterUpdatesTools();          // 8. Mises à Jour & Inventaire Logiciel (4 scripts)
        RegisterSecurityTools();         // 9. Surveillance, Sécurité & Événements (6 scripts)
        RegisterPersonalizationTools();  // 10. Personnalisation & Système (2 scripts)
    }

    public static IReadOnlyList<ScriptTool> AllTools => _allTools;

    public static IEnumerable<ScriptTool> GetByCategory(string category)
        => _allTools.Where(t => t.Category.Equals(category, StringComparison.OrdinalIgnoreCase) && !t.IsHidden);

    public static IEnumerable<ScriptTool> GetFavorites()
    {
        var favIds = FavoritesManager.Default.Favorites;
        return _allTools.Where(t => favIds.Contains(t.Id));
    }

    public static IEnumerable<ScriptTool> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return Enumerable.Empty<ScriptTool>();
        var q = query.Trim().ToLowerInvariant();
        return _allTools.Where(t =>
            t.Name.ToLowerInvariant().Contains(q) ||
            t.Description.ToLowerInvariant().Contains(q) ||
            t.Category.ToLowerInvariant().Contains(q) ||
            t.Tags.Any(tag => tag.ToLowerInvariant().Contains(q)));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 1. 🔍 DIAGNOSTIC MATÉRIEL & COMPOSANTS (13 scripts)
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterDiagnosticTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "diag_battery",
            Name = "Batterie - Santé & Usure",
            Description = "Tension en direct, capacité théorique vs réelle (powercfg /batteryreport).",
            Category = "DIAGNOSTIC",
            Icon = "🔋",
            Action = (log, ct) => DiagnosticService.GetFullBatteryReport(log, ct),
            Tags = new[] { "batterie", "powercfg", "batteryreport", "santé", "usure", "tension" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "diag_winsat",
            Name = "Benchmark Global (WinSAT)",
            Description = "Évaluation des performances CPU, RAM, GPU et stockage (winsat formal).",
            Category = "DIAGNOSTIC",
            Icon = "⚙️",
            Action = (log, ct) => HardwareService.RunWinSat(log, ct),
            Tags = new[] { "benchmark", "winsat", "performance", "score", "cpu", "gpu", "ram", "disque" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "diag_sys_report",
            Name = "Carte Mère, BIOS & Système",
            Description = "Modèle, version BIOS, processeur et configuration globale.",
            Category = "DIAGNOSTIC",
            Icon = "🖥️",
            Action = (log, ct) => DiagnosticService.GetMotherboardBiosInfo(log, ct),
            Tags = new[] { "carte mère", "bios", "système", "uefi", "processeur", "modèle" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "diag_network",
            Name = "Carte Réseau & Connectivité",
            Description = "Interfaces, débit négocié, passerelle et test ping.",
            Category = "DIAGNOSTIC",
            Icon = "🌐",
            Action = (log, ct) => DiagnosticService.DiagNetwork(log, ct),
            Tags = new[] { "réseau", "ping", "connectivité", "ip", "passerelle", "interface", "débit" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "diag_temp",
            Name = "CPU & Températures",
            Description = "Fréquence, charge, cœurs et relevé des sondes ACPI / WMI.",
            Category = "DIAGNOSTIC",
            Icon = "🌡️",
            Action = (log, ct) => DiagnosticService.GetTemperatureReport(log, ct),
            Tags = new[] { "cpu", "température", "chaleur", "thermal", "fréquence", "cœurs" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "diag_disk_smart",
            Name = "Disques - Santé S.M.A.R.T.",
            Description = "État physique, usure et type de média (NVMe / SSD / HDD).",
            Category = "DIAGNOSTIC",
            Icon = "💾",
            Action = (log, ct) => DiskService.GetPhysicalDisks(log, ct),
            Tags = new[] { "smart", "santé", "disques", "ssd", "nvme", "hdd", "usure" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "diag_disk_volumes",
            Name = "Disques - Volumes & Espace",
            Description = "Espace libre, occupé et systèmes de fichiers.",
            Category = "DIAGNOSTIC",
            Icon = "📊",
            Action = (log, ct) => DiskService.GetVolumesStatus(log, ct),
            Tags = new[] { "volumes", "espace", "disque", "libre", "taille", "partitions" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "diag_devmgmt",
            Name = "Gestionnaire de Périphériques (Devmgmt)",
            Description = "Contrôle des composants en erreur ou pilotes manquants (devmgmt.msc).",
            Category = "DIAGNOSTIC",
            Icon = "🔌",
            Action = (log, ct) => HardwareService.LaunchDevmgmt(log, ct),
            Tags = new[] { "devmgmt", "périphériques", "drivers", "pilotes", "erreurs" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "diag_dxdiag",
            Name = "GPU & Audio (DxDiag)",
            Description = "Spécifications DirectX, VRAM, pilotes d'affichage et sorties audio (dxdiag).",
            Category = "DIAGNOSTIC",
            Icon = "🎮",
            Action = (log, ct) => HardwareService.LaunchDxdiag(log, ct),
            Tags = new[] { "dxdiag", "gpu", "directx", "vram", "audio", "graphique" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "diag_msinfo32",
            Name = "Informations Système (Msinfo32)",
            Description = "Vue exhaustive du matériel, des ressources et de l'OS (msinfo32).",
            Category = "DIAGNOSTIC",
            Icon = "ℹ️",
            Action = (log, ct) => HardwareService.LaunchMsinfo32(log, ct),
            Tags = new[] { "msinfo32", "matériel", "système", "os", "ressources" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "diag_ram_audit",
            Name = "RAM - Audit Matériel",
            Description = "Vitesse (MHz), constructeur, slots utilisés et capacité par barrette.",
            Category = "DIAGNOSTIC",
            Icon = "🧠",
            Action = (log, ct) => DiagnosticService.CheckRam(log, ct),
            Tags = new[] { "ram", "mémoire", "slots", "mhz", "vitesse", "constructeur", "ddr" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "diag_ram_test",
            Name = "RAM - Test Mémoire Intégral",
            Description = "Diagnostic mémoire Windows au redémarrage (mdsched.exe).",
            Category = "DIAGNOSTIC",
            Icon = "🛠️",
            Action = (log, ct) => HardwareService.RunRamTest(log, ct),
            Tags = new[] { "ram", "mdsched", "test", "mémoire", "diagnostic" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "diag_sfc",
            Name = "Scan Intégrité Système (SFC)",
            Description = "Analyse d'intégrité des fichiers système protégés (sfc /scannow).",
            Category = "DIAGNOSTIC",
            Icon = "🛡️",
            Action = (log, ct) => RepairService.RunSfc(log, ct),
            Tags = new[] { "sfc", "scannow", "intégrité", "fichiers", "système" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 2. 🔧 RÉPARATION INTÉGRALE (10 scripts)
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterRepairTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "rep_chkdsk",
            Name = "Correction Disque (CHKDSK)",
            Description = "Réparation du système de fichiers (chkdsk C: /f /r).",
            Category = "REPARATION",
            Icon = "🔍",
            Action = (log, ct) => RepairService.RunChkdsk("C", log, ct),
            Tags = new[] { "chkdsk", "disque", "réparation", "ntfs", "erreurs" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "rep_spooler",
            Name = "Déblocage Spouleur d'Impression",
            Description = "Purge de System32\\spool\\PRINTERS et relance du service.",
            Category = "REPARATION",
            Icon = "🖨️",
            Action = (log, ct) => RepairService.FixPrintSpooler(log, ct),
            Tags = new[] { "spouleur", "imprimante", "print", "spooler", "déblocage", "purge" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "rep_pipeline",
            Name = "Pipeline Curatif Global (DISM + SFC)",
            Description = "Enchaînement automatique DISM Restore puis SFC.",
            Category = "REPARATION",
            Icon = "🩹",
            Action = (log, ct) => RepairService.RunCurativePipeline(log, ct),
            Tags = new[] { "pipeline", "curatif", "dism", "sfc", "global", "restauration" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "rep_icon_cache",
            Name = "Reconstruction Cache Icônes & Miniatures",
            Description = "Purge de iconcache*.db et thumbcache*.db.",
            Category = "REPARATION",
            Icon = "🖼️",
            Action = (log, ct) => RepairService.RepairIcons(log, ct),
            Tags = new[] { "icones", "miniatures", "cache", "iconcache", "thumbcache", "explorer" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "rep_wu_reset",
            Name = "Réinitialisation Complète Windows Update",
            Description = "Purge SoftwareDistribution/Catroot2 et reset services.",
            Category = "REPARATION",
            Icon = "🔄",
            Action = (log, ct) => RepairService.ResetWindowsUpdate(log, ct),
            Tags = new[] { "windows update", "wu", "softwaredistribution", "catroot2", "reset", "mise a jour" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "rep_uwp_store",
            Name = "Réparation Applications & Microsoft Store",
            Description = "Réenregistrement des paquets UWP et du Store.",
            Category = "REPARATION",
            Icon = "🏬",
            Action = (log, ct) => RepairService.RepairWindowsStore(log, ct),
            Tags = new[] { "store", "microsoft store", "uwp", "wsreset", "applications", "paquets" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "rep_net_reset",
            Name = "Réparation Pile Réseau & Winsock",
            Description = "Reset TCP/IP, Winsock, purge ARP et renouvellement IP.",
            Category = "REPARATION",
            Icon = "🔌",
            Action = (log, ct) => NetworkService.ResetAll(log, ct),
            Tags = new[] { "réseau", "winsock", "tcp", "ip", "arp", "pile", "connexion" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "rep_dism_restore",
            Name = "Restauration Image Windows (DISM Restore)",
            Description = "Réparation via Windows Update (DISM /RestoreHealth).",
            Category = "REPARATION",
            Icon = "🩺",
            Action = (log, ct) => RepairService.RunDismRestore(log, ct),
            Tags = new[] { "dism", "restorehealth", "image", "windows", "réparation" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "rep_drivers_backup",
            Name = "Sauvegarder Pilotes Système (DISM)",
            Description = "Sauvegarde préventive vers C:\\DriversBackup.",
            Category = "REPARATION",
            Icon = "💾",
            Action = (log, ct) => ExportService.ExportDrivers(log, ct),
            Tags = new[] { "pilotes", "drivers", "backup", "sauvegarde", "dism" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "rep_dism_check",
            Name = "Vérification Image Windows (DISM Check)",
            Description = "Analyse de corruption du magasin (DISM /ScanHealth).",
            Category = "REPARATION",
            Icon = "🔎",
            Action = (log, ct) => RepairService.RunDismCheck(log, ct),
            Tags = new[] { "dism", "checkhealth", "scanhealth", "corruption", "image" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 3. 🧹 NETTOYAGE & OPTIMISATION ESPACE (6 scripts)
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterCleaningTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "cl_all",
            Name = "Nettoyage Complet Unifié",
            Description = "Fichiers temporaires, corbeille, logs et cache DNS en un passage.",
            Category = "NETTOYAGE",
            Icon = "✨",
            Action = (log, ct) => CleaningService.CleanAll(log, ct),
            Tags = new[] { "nettoyage", "unifié", "complet", "temp", "corbeille", "dns", "logs" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "cl_winsxs",
            Name = "Nettoyage Magasin WinSxS (DISM)",
            Description = "Optimisation des composants obsolètes (/StartComponentCleanup /ResetBase).",
            Category = "NETTOYAGE",
            Icon = "🧹",
            Action = (log, ct) => CleaningService.CleanWinSxS(log, ct),
            Tags = new[] { "winsxs", "dism", "optimisation", "cleanup", "resetbase", "espace" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "cl_browsers",
            Name = "Nettoyer Caches Navigateurs & Vignettes",
            Description = "Purge Chrome, Edge, Firefox, Brave et miniatures.",
            Category = "NETTOYAGE",
            Icon = "🌐",
            Action = (log, ct) => CleaningService.CleanBrowsers(log, ct),
            Tags = new[] { "navigateurs", "cache", "chrome", "edge", "firefox", "brave", "vignettes", "miniatures" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "cl_disk",
            Name = "Outil Nettoyage Disque (Cleanmgr)",
            Description = "Lancement de l'assistant officiel cleanmgr.exe.",
            Category = "NETTOYAGE",
            Icon = "🗑️",
            Action = (log, ct) => CleaningService.CleanDisk(log, ct),
            Tags = new[] { "cleanmgr", "disque", "nettoyage", "assistant" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "cl_temp",
            Name = "Purger les Fichiers Temp",
            Description = "Nettoyage de C:\\Windows\\Temp et %TEMP%.",
            Category = "NETTOYAGE",
            Icon = "📁",
            Action = (log, ct) => CleaningService.CleanTemp(log, ct),
            Tags = new[] { "temp", "temporaires", "purge", "fichiers" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "cl_wu",
            Name = "Purger Téléchargements Windows Update",
            Description = "Suppression de SoftwareDistribution\\Download.",
            Category = "NETTOYAGE",
            Icon = "📦",
            Action = (log, ct) => CleaningService.CleanWindowsUpdate(log, ct),
            Tags = new[] { "windows update", "téléchargements", "softwaredistribution", "download", "purge" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 4. 🔑 RÉCUPÉRATION D'IDENTIFIANTS & SECRETS (5 scripts)
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterSecretsTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "sec_wifi_pwd",
            Name = "Afficher Mots de Passe Wi-Fi",
            Description = "Extrait et affiche les clés Wi-Fi enregistrées en clair.",
            Category = "SECRETS",
            Icon = "📶",
            Action = (log, ct) => SecurityService.DumpWifiPasswords(log, ct),
            Tags = new[] { "wifi", "mot de passe", "clés", "clair", "sécurité" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sec_audit_docs",
            Name = "Audit Documents Sensibles",
            Description = "Détection des clés SSH, tokens et mots de passe locaux.",
            Category = "SECRETS",
            Icon = "🔍",
            Action = (log, ct) => ExportService.ScanSensitiveDocs(log, ct),
            Tags = new[] { "audit", "documents", "ssh", "tokens", "secrets", "mots de passe" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sec_browser_hist",
            Name = "Détecter Historique Web & Profils",
            Description = "Localisation des bases SQLite Chrome et Edge.",
            Category = "SECRETS",
            Icon = "🌐",
            Action = (log, ct) => ExportService.AnalyzeBrowserHistory(log, ct),
            Tags = new[] { "historique", "web", "profils", "chrome", "edge", "sqlite" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sec_export_wifi",
            Name = "Exporter Profils & Clés Wi-Fi",
            Description = "Sauvegarde des profils XML sur le Bureau.",
            Category = "SECRETS",
            Icon = "💾",
            Action = (log, ct) => ExportService.ExportWifiAndApps(log, ct),
            Tags = new[] { "export", "wifi", "xml", "bureau", "profils", "sauvegarde" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sec_win_key",
            Name = "Récupérer Clé Produit Windows",
            Description = "Lecture de la clé de licence dans le BIOS/UEFI (OA3x).",
            Category = "SECRETS",
            Icon = "🔑",
            Action = (log, ct) => ExportService.GetWindowsKey(log, ct),
            Tags = new[] { "clé produit", "windows", "licence", "oem", "bios", "uefi", "oa3x" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 5. 🌐 RÉSEAU & CONFIGURATION (6 scripts)
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterNetworkTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "net_dns_cf",
            Name = "Appliquer DNS Cloudflare",
            Description = "Configuration 1.1.1.1 et 1.0.0.1.",
            Category = "RESEAU",
            Icon = "☁️",
            Action = (log, ct) => DnsService.SetCloudflare(log, ct),
            Tags = new[] { "dns", "cloudflare", "1.1.1.1", "rapide", "sécurisé" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_dns_goog",
            Name = "Appliquer DNS Google",
            Description = "Configuration 8.8.8.8 et 8.8.4.4.",
            Category = "RESEAU",
            Icon = "🔍",
            Action = (log, ct) => DnsService.SetGoogle(log, ct),
            Tags = new[] { "dns", "google", "8.8.8.8", "configuration" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_clear_arp",
            Name = "Purger la Table ARP",
            Description = "Suppression des correspondances IP/MAC obsolètes (arp -d *).",
            Category = "RESEAU",
            Icon = "🛡️",
            Action = (log, ct) => NetworkService.ClearArp(log, ct),
            Tags = new[] { "arp", "purge", "table", "cache", "mac", "ip" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_wifi_rpt",
            Name = "Rapport Wi-Fi Complet",
            Description = "Génération du rapport HTML officiel (netsh wlan show wlanreport).",
            Category = "RESEAU",
            Icon = "📊",
            Action = (log, ct) => DiagnosticService.GetWifiReport(log, ct),
            Tags = new[] { "wifi", "rapport", "html", "wlanreport", "historique" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_dns_dhcp",
            Name = "Restaurer DNS Automatique (DHCP)",
            Description = "Rétablissement des DNS dynamiques du routeur.",
            Category = "RESEAU",
            Icon = "🔄",
            Action = (log, ct) => DnsService.RestoreDhcp(log, ct),
            Tags = new[] { "dns", "dhcp", "automatique", "box", "routeur", "restaurer" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_wifi_scan",
            Name = "Scanner les Réseaux Wi-Fi",
            Description = "Détection des bornes à portée avec puissances et canaux.",
            Category = "RESEAU",
            Icon = "📶",
            Action = (log, ct) => NetworkService.ScanWifi(log, ct),
            Tags = new[] { "wifi", "scan", "bornes", "puissance", "canaux", "wlan" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 6. 💽 DISQUE DUR & STOCKAGE (5 scripts)
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterDiskTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "dsk_diskpart",
            Name = "Console de Formatage DISKPART",
            Description = "Outil en ligne de commande administrateur.",
            Category = "DISQUE",
            Icon = "💻",
            Action = (log, ct) => DiskService.LaunchDiskpart(log, ct),
            Tags = new[] { "diskpart", "formatage", "cmd", "partitions", "admin" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "dsk_defrag_gui",
            Name = "Défragmenteur de Disque (GUI)",
            Description = "Ouvre dfrgui.exe.",
            Category = "DISQUE",
            Icon = "🛠️",
            Action = (log, ct) => DiskService.LaunchDefragGui(log, ct),
            Tags = new[] { "défragmenteur", "dfrgui", "optimisation", "disque", "lecteurs" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "dsk_bitlocker",
            Name = "État Chiffrement BitLocker",
            Description = "Contrôle du chiffrement des volumes (manage-bde -status).",
            Category = "DISQUE",
            Icon = "🔒",
            Action = (log, ct) => DiagnosticService.CheckBitLocker(log, ct),
            Tags = new[] { "bitlocker", "chiffrement", "sécurité", "volumes", "manage-bde" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "dsk_mgmt_gui",
            Name = "Gestionnaire de Disques (GUI)",
            Description = "Ouvre diskmgmt.msc.",
            Category = "DISQUE",
            Icon = "💽",
            Action = (log, ct) => DiskService.LaunchDiskManagement(log, ct),
            Tags = new[] { "diskmgmt", "gestionnaire", "partitions", "volumes", "msc" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "dsk_trim",
            Name = "Optimisation Disque & TRIM SSD",
            Description = "Commande defrag /O (TRIM / Défragmentation).",
            Category = "DISQUE",
            Icon = "⚡",
            Action = (log, ct) => DiskService.OptimizeDrives(log, ct),
            Tags = new[] { "trim", "ssd", "defrag", "optimisation", "disque" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 7. 📦 INSTALLATION D'APPLICATIONS (6 scripts)
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterAppInstallationTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "app_install_7zip",
            Name = "Installer 7-Zip",
            Description = "7zip.7zip via Winget.",
            Category = "APPLICATIONS",
            Icon = "🗜️",
            Action = (log, ct) => AppService.Install7Zip(log, ct),
            Tags = new[] { "7zip", "archive", "compression", "winget", "installer" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "app_install_chrome",
            Name = "Installer Google Chrome",
            Description = "Google.Chrome via Winget.",
            Category = "APPLICATIONS",
            Icon = "🌐",
            Action = (log, ct) => AppService.InstallChrome(log, ct),
            Tags = new[] { "chrome", "google", "navigateur", "web", "winget" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "app_install_ps7",
            Name = "Installer PowerShell 7",
            Description = "Microsoft.PowerShell via Winget.",
            Category = "APPLICATIONS",
            Icon = "⚡",
            Action = (log, ct) => AppService.InstallPs7(log, ct),
            Tags = new[] { "powershell", "ps7", "terminal", "winget", "microsoft" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "app_install_sumatra",
            Name = "Installer Sumatra PDF",
            Description = "SumatraPDF.SumatraPDF via Winget.",
            Category = "APPLICATIONS",
            Icon = "📄",
            Action = (log, ct) => AppService.InstallSumatraPdf(log, ct),
            Tags = new[] { "sumatra", "pdf", "lecteur", "winget" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "app_install_vlc",
            Name = "Installer VLC Media Player",
            Description = "VideoLAN.VLC via Winget.",
            Category = "APPLICATIONS",
            Icon = "🎬",
            Action = (log, ct) => AppService.InstallVlc(log, ct),
            Tags = new[] { "vlc", "vidéo", "médias", "musique", "winget" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "app_install_office",
            Name = "Télécharger Microsoft Office 2024",
            Description = "Ouvre le portail de téléchargement officiel Office.",
            Category = "APPLICATIONS",
            Icon = "💼",
            Action = (log, ct) => AppService.InstallOffice(log, ct),
            Tags = new[] { "office", "word", "excel", "microsoft", "téléchargement" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 8. 🔄 MISES À JOUR & INVENTAIRE LOGICIEL (4 scripts)
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterUpdatesTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "maj_list_installed",
            Name = "Lister les Logiciels Installés",
            Description = "Inventaire du parc applicatif via winget list.",
            Category = "MISES_A_JOUR",
            Icon = "📋",
            Action = (log, ct) => AppService.GetInstalledApps(log, ct),
            Tags = new[] { "logiciels", "inventaire", "liste", "winget", "applications" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "maj_defender_sig",
            Name = "Mettre à Jour Signatures Defender",
            Description = "Téléchargement forcé des signatures (Update-MpSignature).",
            Category = "MISES_A_JOUR",
            Icon = "🛡️",
            Action = (log, ct) => SecurityService.UpdateDefender(log, ct),
            Tags = new[] { "defender", "signatures", "antivirus", "mise a jour", "mp-signature" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "maj_update_all",
            Name = "Mettre à Jour Tous les Logiciels",
            Description = "Mise à niveau globale (winget upgrade --all --include-unknown).",
            Category = "MISES_A_JOUR",
            Icon = "🚀",
            Action = (log, ct) => AppService.UpdateAll(log, ct),
            Tags = new[] { "mise a jour", "upgrade", "all", "winget", "logiciels" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "maj_check_updates",
            Name = "Vérifier Mises à Jour Disponibles",
            Description = "Analyse des versions obsolètes via winget upgrade.",
            Category = "MISES_A_JOUR",
            Icon = "🔍",
            Action = (log, ct) => AppService.ListUpdates(log, ct),
            Tags = new[] { "mises a jour", "vérifier", "winget", "disponibles", "obsolètes" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 9. 🛡️ SURVEILLANCE, SÉCURITÉ & ÉVÉNEMENTS (6 scripts)
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterSecurityTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "sec_defender_quick",
            Name = "Analyse Rapide Defender",
            Description = "Lancement d'un scan antiviral rapide (Start-MpScan -ScanType QuickScan).",
            Category = "SECURITE",
            Icon = "🛡️",
            Action = (log, ct) => SecurityService.DefenderQuickScan(log, ct),
            Tags = new[] { "defender", "scan", "antivirus", "rapide", "analyse" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sec_defender_status",
            Name = "État Windows Defender",
            Description = "Contrôle de la protection temps réel et des moteurs.",
            Category = "SECURITE",
            Icon = "🩺",
            Action = (log, ct) => DiagnosticService.GetDefenderStatus(log, ct),
            Tags = new[] { "defender", "état", "protection", "temps réel", "sécurité" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sec_event_log",
            Name = "Journal Erreurs Critiques & Plantages",
            Description = "Filtre sur les derniers événements d'erreur système et BSOD.",
            Category = "SECURITE",
            Icon = "⚠️",
            Action = (log, ct) => DiagnosticService.GetEventLog(log, ct),
            Tags = new[] { "erreurs", "journal", "plantages", "bsod", "critiques", "logs" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sec_reliability",
            Name = "Moniteur de Fiabilité (Perfmon)",
            Description = "Historique graphique des crashs Windows (perfmon /rel).",
            Category = "SECURITE",
            Icon = "📉",
            Action = (log, ct) => DiagnosticService.LaunchReliabilityMonitor(log, ct),
            Tags = new[] { "fiabilité", "perfmon", "crashs", "historique", "plantages" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sec_resmon",
            Name = "Moniteur de Ressources (Resmon)",
            Description = "Surveillance CPU/RAM/Disque/Réseau en temps réel (resmon.exe).",
            Category = "SECURITE",
            Icon = "📊",
            Action = (log, ct) => HardwareService.LaunchResmon(log, ct),
            Tags = new[] { "resmon", "ressources", "cpu", "ram", "disque", "réseau", "surveillance" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sec_ram_usage",
            Name = "Top Processus & Charge RAM",
            Description = "Affichage des applications les plus consommatrices.",
            Category = "SECURITE",
            Icon = "📈",
            Action = (log, ct) => DiagnosticService.GetRamUsage(log, ct),
            Tags = new[] { "processus", "ram", "charge", "consommation", "top" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // 10. 🎨 PERSONNALISATION & SYSTÈME (2 scripts)
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterPersonalizationTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "prs_godmode",
            Name = "Activer le God Mode",
            Description = "Création du raccourci vers le panneau de configuration étendu.",
            Category = "PERSONNALISATION",
            Icon = "👑",
            Action = (log, ct) => PersonalizationService.CreateGodMode(log, ct),
            Tags = new[] { "god mode", "panneau de configuration", "raccourci" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "prs_classic_menu",
            Name = "Menu Contextuel Windows 10",
            Description = "Rétablissement du clic droit classique complet sous Windows 11.",
            Category = "PERSONNALISATION",
            Icon = "🖱️",
            Action = (log, ct) => PersonalizationService.SetClassicMenu(log, ct),
            Tags = new[] { "menu contextuel", "windows 10", "clic droit", "windows 11" }
        });
    }
}
