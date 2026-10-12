using System;
using System.Collections.Generic;
using System.Linq;
using ScriptsWindows.Models;
using ScriptsWindows.Services;

namespace ScriptsWindows.Core;

/// <summary>
/// Répertoire central de tous les outils disponibles dans l'application.
/// Structuré en 11 catégories principales avec sous-titres internes (subheadings).
/// Conforme à la directive UI/UX Alex Toolkit.
/// </summary>
public static class ToolRegistry
{
    private static readonly List<ScriptTool> _allTools = new();

    static ToolRegistry()
    {
        RegisterDiagnosticTools();       // Catégorie 1 : Diagnostic Matériel et Intégrité
        RegisterHardwareTools();         // Catégorie 2 : Matériel et Périphériques
        RegisterRepairTools();           // Catégorie 3 : Réparation Intégrale
        RegisterCleaningTools();         // Catégorie 4 : Nettoyage et Optimisation Espace
        RegisterSecretsTools();          // Catégorie 5 : Récupération d'Identifiants et Secrets
        RegisterNetworkTools();          // Catégorie 6 : Réseau et Configuration
        RegisterDiskTools();             // Catégorie 7 : Disque Dur et Stockage
        RegisterAppInstallationTools();  // Catégorie 8 : Installation d'Applications
        RegisterUpdatesTools();          // Catégorie 9 : Mises à Jour et Inventaire Logiciel
        RegisterSecurityTools();         // Catégorie 10 : Surveillance, Sécurité et Événements
        RegisterPersonalizationTools();  // Catégorie 11 : Personnalisation et Système
    }

    public static IReadOnlyList<ScriptTool> AllTools => _allTools;

    public static IEnumerable<ScriptTool> GetByCategory(string category)
        => _allTools.Where(t => t.Category.Equals(category, StringComparison.OrdinalIgnoreCase) && !t.IsHidden);

    public static IEnumerable<ScriptTool> GetFavorites()
    {
        var favIds = FavoritesManager.Default.Favorites;
        return _allTools.Where(t => favIds.Contains(t.Id));
    }

    private static readonly Dictionary<string, string> CommandMap = new(StringComparer.OrdinalIgnoreCase)
    {
        // ─── Diagnostic (4) ───
        ["diag_center"] = "AlexToolkit.exe (Centre de diagnostic unifié : carte mère, BIOS, CPU, RAM, disques S.M.A.R.T., batterie, réseau)",
        ["diag_dxdiag"] = "dxdiag.exe",
        ["diag_startup_manager"] = "AlexToolkit.exe (Gestionnaire des programmes au démarrage de Windows)",
        ["diag_battery"] = "powercfg.exe /batteryreport",

        // ─── Matériel (4) ───
        ["hw_devmgmt"] = "devmgmt.msc",
        ["hw_winsat"] = "winsat.exe formal",
        ["hw_powercfg_energy"] = "powercfg.exe /energy /output energy-report.html",
        ["hw_power_perf"] = "powercfg.exe -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61",

        // ─── Réparation (6) ───
        ["repair_system_integrity"] = "AlexToolkit.exe (Centre d'intégrité Windows : DISM Scan/Restore et SFC)",
        ["repair_chkdsk"] = "chkdsk.exe (Interface interactive de réparation de disque NTFS)",
        ["repair_services_manager"] = "AlexToolkit.exe (Gestionnaire des services Windows : Spouleur, Windows Update...)",
        ["repair_restore_point"] = "AlexToolkit.exe (Points de restauration système Windows)",
        ["repair_apps_shell"] = "AlexToolkit.exe (Réparation Store UWP, wsreset et cache d'icônes Explorer)",
        ["repair_export_drivers"] = "dism.exe /Online /Export-Driver /Destination:C:\\DriversBackup",

        // ─── Nettoyage (2) ───
        ["clean_system_unified"] = "AlexToolkit.exe (Interface interactive : Cleanmgr, temporaires, corbeille, caches et WinSxS)",
        ["clean_winsxs"] = "dism.exe /Online /Cleanup-Image /StartComponentCleanup /ResetBase",

        // ─── Secrets et Clés (5) ───
        ["sec_wifi_show"] = "netsh.exe wlan show profiles key=clear",
        ["sec_audit_sensitive"] = "AlexToolkit.exe (Audit C# approfondi .ssh, .aws, .env, documents secrets)",
        ["sec_win_key"] = "wmic.exe path softwarelicensingservice get OA3xOriginalProductKey",
        ["sec_browser_profiles"] = "AlexToolkit.exe (Lecteur SQLite natif Chrome, Edge, Brave, Firefox)",

        // ─── Réseau (6) ───
        ["net_dns_cloudflare"] = "powershell Set-DnsClientServerAddress (1.1.1.1, 1.0.0.1)",
        ["net_dns_google"] = "powershell Set-DnsClientServerAddress (8.8.8.8, 8.8.4.4)",
        ["net_dns_dhcp"] = "powershell Set-DnsClientServerAddress -ResetServerAddresses",
        ["net_arp_clear"] = "arp.exe -d *",
        ["net_wifi_scan"] = "netsh.exe wlan show networks mode=bssid",
        ["net_wifi_report"] = "netsh.exe wlan show wlanreport",

        // ─── Disque (5) ───
        ["disk_mgmt_gui"] = "diskmgmt.msc",
        ["disk_diskpart"] = "diskpart.exe",
        ["disk_defrag_gui"] = "dfrgui.exe",
        ["disk_trim_opt"] = "defrag.exe /O /C",
        ["disk_bitlocker"] = "manage-bde.exe -status",

        // ─── Applications (1) ───
        ["app_installer_center"] = "AlexToolkit.exe (Installation d'applications unifiée : Chrome, VLC, 7-Zip, Sumatra PDF, PowerShell 7, Office)",

        // ─── Mises à jour (4) ───
        ["upd_list_apps"] = "winget.exe list",
        ["upd_check_updates"] = "winget.exe upgrade",
        ["upd_apply_all"] = "winget.exe upgrade --all --include-unknown",
        ["upd_defender_sigs"] = "powershell Update-MpSignature",

        // ─── Sécurité (5) ───
        ["sec_top_ram"] = "powershell Get-Process | Sort-Object WorkingSet64 -Descending",
        ["sec_event_errors"] = "powershell Get-WinEvent (System, Level=1,2)",
        ["sec_perfmon_rel"] = "perfmon.exe /rel",
        ["sec_defender_status"] = "powershell Get-MpComputerStatus",
        ["sec_defender_quickscan"] = "powershell Start-MpScan -ScanType QuickScan",

        // ─── Personnalisation (2) ───
        ["tweak_godmode"] = "explorer.exe \"shell:::{ED7BA470-8E54-465E-825C-99712043E01C}\"",
        ["tweak_classic_context"] = "AlexToolkit.exe (Bascule interactive : Menu classique Windows 10 / Moderne Windows 11)"
    };


    public static string GetCommand(string toolId)
    {
        if (CommandMap.TryGetValue(toolId ?? "", out var cmd))
            return cmd;
        return "powershell.exe";
    }

    public static IEnumerable<ScriptTool> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return Enumerable.Empty<ScriptTool>();
        var q = query.Trim().ToLowerInvariant();
        return _allTools.Where(t =>
            t.Name.ToLowerInvariant().Contains(q) ||
            t.Description.ToLowerInvariant().Contains(q) ||
            t.Category.ToLowerInvariant().Contains(q) ||
            t.SubHeading.ToLowerInvariant().Contains(q) ||
            t.Tags.Any(tag => tag.ToLowerInvariant().Contains(q)));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 1 : 🔍 DIAGNOSTIC MATÉRIEL ET INTÉGRITÉ
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterDiagnosticTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "diag_center",
            Name = "Centre de Diagnostic Matériel et Système",
            Description = "Interface unifiée tout-en-un : audit complet de la carte mère, BIOS, CPU, RAM, santé S.M.A.R.T., batterie et réseau.",
            Category = "DIAGNOSTIC",
            SubHeading = "Centre de Diagnostic et Audit",
            Icon = "🔍",
            Action = (log, ct) => DiagnosticService.OpenDiagnosticCenterDialog(log, ct),
            Tags = new[] { "centre", "diagnostic", "matériel", "système", "cpu", "ram", "disques", "smart", "batterie", "bios", "carte mère", "réseau" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "diag_dxdiag",
            Name = "Diagnostic matériel (DxDiag)",
            Description = "Diagnostic complet DirectX, carte graphique GPU et périphériques audio (dxdiag.exe).",
            Category = "DIAGNOSTIC",
            SubHeading = "Outils et Actions Rapides",
            Icon = "🎮",
            Action = (log, ct) => HardwareService.LaunchDxdiag(log, ct),
            Tags = new[] { "dxdiag", "gpu", "directx", "vram", "audio", "graphique", "diagnostic" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "diag_startup_manager",
            Name = "Gestionnaire des Programmes au Démarrage",
            Description = "Visualiser les logiciels qui se lancent avec Windows et désactiver les éléments superflus.",
            Category = "DIAGNOSTIC",
            SubHeading = "Outils et Actions Rapides",
            Icon = "🚀",
            Action = (log, ct) => DiagnosticService.OpenStartupManagerDialog(log, ct),
            Tags = new[] { "démarrage", "startup", "boot", "programmes", "applications", "accélérer", "taskmgr" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "diag_battery",
            Name = "Batterie - santé et usure",
            Description = "Tension/pourcentage direct et rapport d'usure complet (powercfg /batteryreport).",
            Category = "DIAGNOSTIC",
            SubHeading = "Outils et Actions Rapides",
            Icon = "🔋",
            Action = (log, ct) => DiagnosticService.GetFullBatteryReport(log, ct),
            Tags = new[] { "batterie", "powercfg", "batteryreport", "santé", "usure", "tension" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 2 : 🖥️ MATÉRIEL ET PÉRIPHÉRIQUES
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterHardwareTools()
    {
        // Sous-titre interne : Périphériques et Pilotes
        _allTools.Add(new ScriptTool
        {
            Id = "hw_devmgmt",
            Name = "Gestionnaire de périphériques",
            Description = "Gestionnaire de périphériques officiel Windows (devmgmt.msc).",
            Category = "MATERIEL",
            SubHeading = "Périphériques et Pilotes",
            Icon = "🔌",
            Action = (log, ct) => HardwareService.LaunchDevmgmt(log, ct),
            Tags = new[] { "devmgmt", "périphériques", "drivers", "pilotes", "erreurs" }
        });

        // Sous-titre interne : Performances et Alimentation
        _allTools.Add(new ScriptTool
        {
            Id = "hw_winsat",
            Name = "Benchmark global (WinSAT)",
            Description = "Benchmark complet CPU, RAM, GPU et disques (winsat formal).",
            Category = "MATERIEL",
            SubHeading = "Performances et Alimentation",
            Icon = "⚙️",
            Action = (log, ct) => HardwareService.RunWinSat(log, ct),
            Tags = new[] { "benchmark", "winsat", "performance", "score", "cpu", "gpu", "ram", "disque" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "hw_powercfg_energy",
            Name = "Rapport d'efficacité énergétique",
            Description = "Analyse matérielle de consommation d'énergie sur 60 secondes avec rapport officiel (powercfg /energy).",
            Category = "MATERIEL",
            SubHeading = "Performances et Alimentation",
            Icon = "⚡",
            Action = (log, ct) => HardwareService.RunPowercfgEnergy(log, ct),
            Tags = new[] { "énergie", "consommation", "powercfg", "batterie", "matériel", "rapport" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "hw_power_perf",
            Name = "Activer le mode Performances Élevées",
            Description = "Bascule matérielle immédiate vers le plan d'alimentation hautes performances de Windows.",
            Category = "MATERIEL",
            SubHeading = "Performances et Alimentation",
            Icon = "🚀",
            Action = (log, ct) => HardwareService.SetPlanHigh(log, ct),
            Tags = new[] { "performance", "alimentation", "powercfg", "vitesse", "cpu", "boost" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 3 : 🔧 RÉPARATION INTÉGRALE
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterRepairTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "repair_system_integrity",
            Name = "Centre d'intégrité Windows (DISM & SFC)",
            Description = "Diagnostic et réparation de l'image Windows : ScanHealth, RestoreHealth, SFC scannow ou cure complète.",
            Category = "REPARATION",
            SubHeading = "Intégrité Système et Disque",
            Icon = "🩺",
            Action = (log, ct) => RepairService.OpenSystemIntegrityDialog(log, ct),
            Tags = new[] { "dism", "sfc", "intégrité", "image", "réparation", "scanhealth", "restorehealth", "scannow", "curatif", "corruption" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "repair_chkdsk",
            Name = "Vérification et Réparation CHKDSK",
            Description = "Choisir le lecteur et lancer une analyse (lecture seule, correction /f ou réparation complète /r).",
            Category = "REPARATION",
            SubHeading = "Intégrité Système et Disque",
            Icon = "🔍",
            Action = (log, ct) => RepairService.OpenChkdskDialog(log, ct),
            Tags = new[] { "chkdsk", "disque", "réparation", "ntfs", "erreurs", "secteurs" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "repair_services_manager",
            Name = "Gestionnaire des Services Windows",
            Description = "Piloter, démarrer, arrêter ou redémarrer les services clés (Spouleur, Windows Update, SysMain, Télémétrie...).",
            Category = "REPARATION",
            SubHeading = "Services et Impression",
            Icon = "⚙️",
            Action = (log, ct) => RepairService.OpenServicesManagerDialog(log, ct),
            Tags = new[] { "services", "spooler", "wuauserv", "sysmain", "démarrer", "arrêter", "processus", "msc" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "repair_spooler",
            Name = "Déblocage et Purge file d'impression",
            Description = "Arrête le spouleur, supprime tous les travaux bloqués dans PRINTERS et redémarre l'impression.",
            Category = "REPARATION",
            SubHeading = "Services et Impression",
            Icon = "🖨️",
            Action = (log, ct) => RepairService.FixPrintSpooler(log, ct),
            Tags = new[] { "spooler", "impression", "imprimante", "file", "bloquée", "vider", "purge", "documents", "travaux", "attente" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "repair_apps_shell",
            Name = "Réparation Store UWP & Interface",
            Description = "Réinitialiser le Microsoft Store (wsreset), réenregistrer les paquets applicatifs et reconstruire le cache d'icônes.",
            Category = "REPARATION",
            SubHeading = "Applications et Interface Windows",
            Icon = "🏬",
            Action = (log, ct) => RepairService.OpenAppsShellRepairDialog(log, ct),
            Tags = new[] { "store", "microsoft store", "uwp", "wsreset", "icones", "miniatures", "cache", "explorer", "applications" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "repair_restore_point",
            Name = "Points de Restauration Système",
            Description = "Lister les points existants, créer un point de restauration nommé et lancer la restauration.",
            Category = "REPARATION",
            SubHeading = "Sauvegarde et Restauration",
            Icon = "🔄",
            Action = (log, ct) => RepairService.OpenRestorePointDialog(log, ct),
            Tags = new[] { "restauration", "point", "sauvegarde", "checkpoint", "sécurité", "rstrui" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "repair_export_drivers",
            Name = "Sauvegarder pilotes système (DISM)",
            Description = "Export et sauvegarde de tous les pilotes système vers un dossier au choix (DISM /Export-Driver).",
            Category = "REPARATION",
            SubHeading = "Sauvegarde et Restauration",
            Icon = "💾",
            Action = (log, ct) => ExportService.ExportDrivers(log, ct),
            Tags = new[] { "pilotes", "drivers", "backup", "sauvegarde", "dism" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 4 : 🧹 NETTOYAGE ET OPTIMISATION ESPACE
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterCleaningTools()
    {
        // Sous-titre interne : Nettoyage et Maintenance Globale
        _allTools.Add(new ScriptTool
        {
            Id = "clean_system_unified",
            Name = "Nettoyage Système et Disque",
            Description = "Centre interactif complet : assistant Cleanmgr, purge des temporaires (%TEMP%), corbeille, cache Windows Update, navigateurs et sélection personnalisée.",
            Category = "NETTOYAGE",
            SubHeading = "Nettoyage et Maintenance Globale",
            Icon = "🧹",
            Action = (log, ct) => CleaningService.OpenSystemCleanupDialog(log, ct),
            Tags = new[] { "cleanmgr", "disque", "nettoyage", "système", "temp", "corbeille", "cache", "unifié", "assistant", "navigateurs", "dns" }
        });

        // Sous-titre interne : Magasin de composants Windows
        _allTools.Add(new ScriptTool
        {
            Id = "clean_winsxs",
            Name = "Nettoyage magasin WinSxS (DISM)",
            Description = "Optimisation approfondie du dossier WinSxS et purge des composants obsolètes (DISM /StartComponentCleanup /ResetBase).",
            Category = "NETTOYAGE",
            SubHeading = "Magasin de composants Windows",
            Icon = "🛠️",
            Action = (log, ct) => CleaningService.CleanWinSxS(log, ct),
            Tags = new[] { "winsxs", "dism", "optimisation", "cleanup", "resetbase", "espace", "composants" }
        });
    }


    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 5 : 🔑 RÉCUPÉRATION D'IDENTIFIANTS ET SECRETS
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterSecretsTools()
    {
        // Sous-titre interne : Cartes réseau sans fil (Wi-Fi)
        _allTools.Add(new ScriptTool
        {
            Id = "sec_wifi_show",
            Name = "Afficher mots de passe Wi-Fi",
            Description = "Affichage console immédiat des clés Wi-Fi en clair.",
            Category = "SECRETS",
            SubHeading = "Cartes réseau sans fil (Wi-Fi)",
            Icon = "📶",
            Action = (log, ct) => SecurityService.DumpWifiPasswords(log, ct),
            Tags = new[] { "wifi", "mot de passe", "clés", "clair", "sécurité" }
        });

        // Sous-titre interne : Dossiers utilisateurs et Clés d'authentification
        _allTools.Add(new ScriptTool
        {
            Id = "sec_pwd_generator",
            Name = "Générer mots de passe robustes",
            Description = "Génération de clés et mots de passe aléatoires sécurisés avec sauvegarde dans un dossier dédié.",
            Category = "SECRETS",
            SubHeading = "Dossiers utilisateurs et Clés d'authentification",
            Icon = "🔐",
            Action = (log, ct) => SecurityService.GenerateSecurePasswords(log, ct),
            Tags = new[] { "mot de passe", "génération", "aléatoire", "sécurité", "robuste", "mdp" }
        });
        _allTools.Add(new ScriptTool
        {
            Id = "sec_audit_sensitive",
            Name = "Audit documents sensibles",
            Description = "Inventaire des clés SSH (.ssh), identifiants Cloud (.aws, .docker), fichiers .env et mots de passe.",
            Category = "SECRETS",
            SubHeading = "Dossiers utilisateurs et Clés d'authentification",
            Icon = "🔍",
            Action = (log, ct) => ExportService.ScanSensitiveDocs(log, ct),
            Tags = new[] { "audit", "documents", "ssh", "tokens", "secrets", "mots de passe" }
        });

        // Sous-titre interne : Puce de sécurité / Carte mère (UEFI)
        _allTools.Add(new ScriptTool
        {
            Id = "sec_win_key",
            Name = "Récupérer clé produit Windows",
            Description = "Lecture de la clé de licence d'origine intégrée au BIOS/UEFI.",
            Category = "SECRETS",
            SubHeading = "Puce de sécurité / Carte mère (UEFI)",
            Icon = "🔑",
            Action = (log, ct) => ExportService.GetWindowsKey(log, ct),
            Tags = new[] { "clé produit", "windows", "licence", "oem", "bios", "uefi", "oa3x" }
        });

        // Sous-titre interne : Navigateurs web (Bases locales)
        _allTools.Add(new ScriptTool
        {
            Id = "sec_browser_profiles",
            Name = "Extraire l'historique web et profils",
            Description = "Extraction SQLite directe des 35 dernières pages visitées (Chrome, Edge, Brave, Firefox).",
            Category = "SECRETS",
            SubHeading = "Navigateurs web (Bases locales)",
            Icon = "🌐",
            Action = (log, ct) => ExportService.AnalyzeBrowserHistory(log, ct),
            Tags = new[] { "historique", "web", "profils", "chrome", "edge", "sqlite" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 6 : 🌐 RÉSEAU ET CONFIGURATION
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterNetworkTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "net_center",
            Name = "Centre de Contrôle Réseau",
            Description = "Interface unifiée tout-en-un : diagnostic en direct, gestionnaire DNS (Cloudflare, Google, Quad9), dépannage et scanners.",
            Category = "RESEAU",
            SubHeading = "Connectivité et Configuration Globale",
            Icon = "🌐",
            Action = (log, ct) => NetworkService.OpenNetworkCenterDialog(log, ct),
            Tags = new[] { "centre", "réseau", "diagnostic", "dns", "triage", "reset", "wifi", "lan", "ip", "connectivité", "internet", "cloudflare", "google", "quad9", "dhcp", "changer" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_reset_all",
            Name = "Dépannage & Grand Reset Réseau",
            Description = "Protocole curatif complet en 7 étapes : purge DNS/ARP, réinitialisation Winsock et TCP/IP, renouvellement IP.",
            Category = "RESEAU",
            SubHeading = "Connectivité et Configuration Globale",
            Icon = "⚡",
            Action = (log, ct) => NetworkService.ResetAll(log, ct),
            Tags = new[] { "reset", "réinitialiser", "dépannage", "connexion", "coupure", "winsock", "tcpip", "dhcp", "arp", "flushdns" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_wifi_scan",
            Name = "Scanner et Analyseur Wi-Fi (Graphique)",
            Description = "Visualiseur interactif des réseaux Wi-Fi voisins avec jauges de signal, pourcentages, canaux et sécurité.",
            Category = "RESEAU",
            SubHeading = "Découverte Sans Fil et Wi-Fi",
            Icon = "📶",
            Action = (log, ct) => NetworkService.OpenWifiScannerDialog(log, ct),
            Tags = new[] { "wifi", "scan", "bornes", "puissance", "canaux", "wlan", "ssid", "signal", "jauges", "visuel" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_wifi_report",
            Name = "Rapport de Santé Wi-Fi (Graphique)",
            Description = "Interface visuelle interactive du rapport officiel Windows avec graphiques des 3 derniers jours et export HTML.",
            Category = "RESEAU",
            SubHeading = "Découverte Sans Fil et Wi-Fi",
            Icon = "📊",
            Action = (log, ct) => DiagnosticService.GetWifiReport(log, ct),
            Tags = new[] { "wifi", "rapport", "html", "wlanreport", "historique", "santé", "déconnexions", "graphique", "visuel" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_lan_scan",
            Name = "Scanner Réseau Local (LAN Graphique)",
            Description = "Visualiseur des appareils et machines connectés sur la Box avec statut ping en direct et copie rapide d'IP.",
            Category = "RESEAU",
            SubHeading = "Cartographie LAN et Surveillance des Flux",
            Icon = "🖥️",
            Action = (log, ct) => NetworkService.OpenLanScannerDialog(log, ct),
            Tags = new[] { "lan", "scan", "découverte", "machines", "hotes", "adresses", "ip", "box", "routeur", "visuel" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_flux_analysis",
            Name = "Moniteur des Connexions TCP Actives",
            Description = "Visualiseur filtrable des connexions TCP établies avec identification des logiciels et ports associés.",
            Category = "RESEAU",
            SubHeading = "Cartographie LAN et Surveillance des Flux",
            Icon = "🔍",
            Action = (log, ct) => NetworkService.OpenTcpConnectionsDialog(log, ct),
            Tags = new[] { "connexions", "flux", "tcp", "ports", "processus", "netstat", "ip", "écoute", "visuel", "moniteur" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 7 : 💽 DISQUE DUR ET STOCKAGE
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterDiskTools()
    {
        // Sous-titre interne : Tables de partitions et Volumes

        _allTools.Add(new ScriptTool
        {
            Id = "disk_diskpart",
            Name = "Console de formatage DISKPART",
            Description = "Invite de commandes DISKPART en mode administrateur.",
            Category = "DISQUE",
            SubHeading = "Tables de partitions et Volumes",
            Icon = "💻",
            Action = (log, ct) => DiskService.LaunchDiskpart(log, ct),
            Tags = new[] { "diskpart", "formatage", "cmd", "partitions", "admin" }
        });

        // Sous-titre interne : Supports Flash et Mécaniques (SSD / HDD)
        _allTools.Add(new ScriptTool
        {
            Id = "disk_defrag_gui",
            Name = "Défragmenteur de disque (GUI)",
            Description = "Défragmenteur de disque Windows (dfrgui.exe).",
            Category = "DISQUE",
            SubHeading = "Supports Flash et Mécaniques (SSD / HDD)",
            Icon = "🛠️",
            Action = (log, ct) => DiskService.LaunchDefragGui(log, ct),
            Tags = new[] { "défragmenteur", "dfrgui", "optimisation", "disque", "lecteurs" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "disk_trim_opt",
            Name = "Optimisation disque et TRIM SSD",
            Description = "Commande d'optimisation TRIM pour SSD (defrag /O).",
            Category = "DISQUE",
            SubHeading = "Supports Flash et Mécaniques (SSD / HDD)",
            Icon = "⚡",
            Action = (log, ct) => DiskService.OptimizeDrives(log, ct),
            Tags = new[] { "trim", "ssd", "defrag", "optimisation", "disque" }
        });

        // Sous-titre interne : Sécurité et Chiffrement de volume
        _allTools.Add(new ScriptTool
        {
            Id = "disk_bitlocker",
            Name = "Gestion et Chiffrement BitLocker",
            Description = "Consulter l'état, choisir le lecteur et activer ou désactiver la protection BitLocker.",
            Category = "DISQUE",
            SubHeading = "Sécurité et Chiffrement de volume",
            Icon = "🔒",
            Action = (log, ct) => DiskService.ManageBitLocker(log, ct),
            Tags = new[] { "bitlocker", "chiffrement", "sécurité", "volumes", "manage-bde", "activer", "désactiver" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 8 : 📦 INSTALLATION D'APPLICATIONS
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterAppInstallationTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "app_installer_center",
            Name = "Installation d'Applications",
            Description = "Interface unifiée : sélectionnez et installez vos logiciels indispensables (Google Chrome, VLC, 7-Zip, Sumatra PDF, PowerShell 7, Microsoft Office) en un clic.",
            Category = "APPLICATIONS",
            SubHeading = "",
            Icon = "📦",
            Action = (log, ct) => AppService.OpenAppInstallerDialog(log, ct),
            Tags = new[] { "applications", "logiciels", "winget", "installer", "chrome", "vlc", "7zip", "sumatra", "powershell", "office", "unifié" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 9 : 🔄 MISES À JOUR ET INVENTAIRE LOGICIEL
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterUpdatesTools()
    {
        // Sous-titre interne : Gestionnaire de paquets (Winget)
        _allTools.Add(new ScriptTool
        {
            Id = "upd_winget_manager",
            Name = "Gestionnaire de Paquets Winget",
            Description = "Lister les logiciels, choisir les mises à niveau à appliquer ou tout mettre à jour en un clic.",
            Category = "MISES_A_JOUR",
            SubHeading = "Gestionnaire de paquets (Winget)",
            Icon = "📦",
            Action = (log, ct) => AppService.OpenWingetManagerDialog(log, ct),
            Tags = new[] { "winget", "logiciels", "mises a jour", "upgrade", "inventaire", "paquets", "applications" }
        });

        // Sous-titre interne : Moteur antivirus Microsoft Defender
        _allTools.Add(new ScriptTool
        {
            Id = "upd_defender_sigs",
            Name = "Mettre à jour signatures Defender",
            Description = "Mise à jour immédiate des signatures de sécurité (Update-MpSignature).",
            Category = "MISES_A_JOUR",
            SubHeading = "Moteur antivirus Microsoft Defender",
            Icon = "🛡️",
            Action = (log, ct) => SecurityService.UpdateDefender(log, ct),
            Tags = new[] { "defender", "signatures", "antivirus", "mise a jour", "mp-signature" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 10 : 🛡️ SURVEILLANCE, SÉCURITÉ ET ÉVÉNEMENTS
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterSecurityTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "sec_center",
            Name = "Centre de Surveillance et Sécurité",
            Description = "Interface unifiée tout-en-un : état Defender, scan antivirus, erreurs critiques BSOD et consommation RAM des processus.",
            Category = "SECURITE",
            SubHeading = "Centre de Sécurité et Surveillance",
            Icon = "🛡️",
            Action = (log, ct) => SecurityService.OpenSecurityCenterDialog(log, ct),
            Tags = new[] { "centre", "sécurité", "defender", "antivirus", "protection", "temps réel", "erreurs", "bsod", "ram", "processus", "surveillance" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sec_defender_quickscan",
            Name = "Analyse rapide Defender",
            Description = "Lancement immédiat d'une analyse antivirus rapide des fichiers et mémoire système.",
            Category = "SECURITE",
            SubHeading = "Actions et Outils Rapides",
            Icon = "⚡",
            Action = (log, ct) => SecurityService.DefenderQuickScan(log, ct),
            Tags = new[] { "defender", "scan", "antivirus", "rapide", "analyse", "virus" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sec_perfmon_rel",
            Name = "Moniteur de fiabilité (Perfmon)",
            Description = "Historique graphique de la fiabilité et des plantages du système au fil des semaines.",
            Category = "SECURITE",
            SubHeading = "Actions et Outils Rapides",
            Icon = "📉",
            Action = (log, ct) => DiagnosticService.LaunchReliabilityMonitor(log, ct),
            Tags = new[] { "fiabilité", "perfmon", "crashs", "historique", "plantages" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 11 : 🎨 PERSONNALISATION ET SYSTÈME
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterPersonalizationTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "tweak_godmode",
            Name = "Activer le God Mode",
            Description = "Création du raccourci dossier God Mode permettant d'accéder à tous les réglages avancés de Windows.",
            Category = "PERSONNALISATION",
            SubHeading = "",
            Icon = "👑",
            Action = (log, ct) => PersonalizationService.CreateGodMode(log, ct),
            Tags = new[] { "god mode", "panneau de configuration", "raccourci", "paramètres" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "tweak_classic_context",
            Name = "Menu contextuel (Windows 10 / 11)",
            Description = "Bascule interactive : activer le clic droit classique Windows 10 ou rétablir le menu d'origine Windows 11.",
            Category = "PERSONNALISATION",
            SubHeading = "",
            Icon = "🖱️",
            Action = (log, ct) => PersonalizationService.OpenContextMenuDialog(log, ct),
            Tags = new[] { "menu contextuel", "windows 10", "windows 11", "clic droit", "classique", "moderne", "restaurer", "arriere" }
        });
    }

}

