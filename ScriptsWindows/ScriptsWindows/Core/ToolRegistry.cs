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
        // ─── Diagnostic (8) ───
        ["diag_mobo"] = "powershell Get-CimInstance Win32_BaseBoard, Win32_BIOS",
        ["diag_cpu"] = "powershell Get-CimInstance Win32_Processor / MSAcpi",
        ["diag_ram"] = "powershell Get-CimInstance Win32_PhysicalMemory",
        ["diag_smart"] = "powershell Get-PhysicalDisk",
        ["diag_volumes"] = "powershell Get-Volume",
        ["diag_battery"] = "powercfg.exe /batteryreport",
        ["diag_network"] = "powershell Get-NetAdapter, Test-NetConnection",
        ["diag_sfc"] = "sfc.exe /scannow",

        // ─── Matériel (7) ───
        ["hw_mdsched"] = "mdsched.exe",
        ["hw_dxdiag"] = "dxdiag.exe",
        ["hw_devmgmt"] = "devmgmt.msc",
        ["hw_resmon"] = "resmon.exe",
        ["hw_msinfo"] = "msinfo32.exe",
        ["hw_winsat"] = "winsat.exe formal",
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

        // ─── Applications (6) ───
        ["app_install_chrome"] = "winget.exe install Google.Chrome --silent",
        ["app_install_vlc"] = "winget.exe install VideoLAN.VLC --silent",
        ["app_install_sumatra"] = "winget.exe install SumatraPDF.SumatraPDF --silent",
        ["app_install_7zip"] = "winget.exe install 7zip.7zip --silent",
        ["app_install_pwsh"] = "winget.exe install Microsoft.PowerShell --silent",
        ["app_open_office"] = "start https://drive.google.com/file/d/1wf1eqVkXIM0f0R2963Ee4RKK-a_rgWld/view?usp=drive_link",

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
        // Sous-titre interne : Carte mère et Processeur
        _allTools.Add(new ScriptTool
        {
            Id = "diag_mobo",
            Name = "Carte mère, BIOS et système",
            Description = "Modèle, fabricant, version BIOS via Get-CimInstance Win32_BaseBoard, Win32_BIOS.",
            Category = "DIAGNOSTIC",
            SubHeading = "Carte mère et Processeur",
            Icon = "🖥️",
            Action = (log, ct) => DiagnosticService.GetMotherboardBiosInfo(log, ct),
            Tags = new[] { "carte mère", "bios", "système", "uefi", "processeur", "modèle" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "diag_cpu",
            Name = "CPU et températures",
            Description = "Cœurs, fréquence, charge CPU et sondes thermiques ACPI (MSAcpi_ThermalZoneTemperature).",
            Category = "DIAGNOSTIC",
            SubHeading = "Carte mère et Processeur",
            Icon = "🌡️",
            Action = (log, ct) => DiagnosticService.GetTemperatureReport(log, ct),
            Tags = new[] { "cpu", "température", "chaleur", "thermal", "fréquence", "cœurs" }
        });

        // Sous-titre interne : Mémoire vive (RAM)
        _allTools.Add(new ScriptTool
        {
            Id = "diag_ram",
            Name = "RAM - audit matériel",
            Description = "Vitesse, constructeur, slots et capacité par barrette (Win32_PhysicalMemory).",
            Category = "DIAGNOSTIC",
            SubHeading = "Mémoire vive (RAM)",
            Icon = "🧠",
            Action = (log, ct) => DiagnosticService.CheckRam(log, ct),
            Tags = new[] { "ram", "mémoire", "slots", "mhz", "vitesse", "constructeur", "ddr" }
        });

        // Sous-titre interne : Stockage physique
        _allTools.Add(new ScriptTool
        {
            Id = "diag_smart",
            Name = "Disques - santé S.M.A.R.T. et espace",
            Description = "État de santé S.M.A.R.T. en %, type de disque et espace libre/occupé par lecteur.",
            Category = "DIAGNOSTIC",
            SubHeading = "Stockage physique",
            Icon = "💾",
            Action = (log, ct) => DiskService.GetDisksHealthAndStorage(log, ct),
            Tags = new[] { "smart", "santé", "disques", "espace", "volumes", "ssd", "nvme", "hdd", "usure", "libre" }
        });

        // Sous-titre interne : Alimentation et Batterie
        _allTools.Add(new ScriptTool
        {
            Id = "diag_battery",
            Name = "Batterie - santé et usure",
            Description = "Tension/pourcentage direct et rapport d'usure (powercfg /batteryreport).",
            Category = "DIAGNOSTIC",
            SubHeading = "Alimentation et Batterie",
            Icon = "🔋",
            Action = (log, ct) => DiagnosticService.GetFullBatteryReport(log, ct),
            Tags = new[] { "batterie", "powercfg", "batteryreport", "santé", "usure", "tension" }
        });

        // Sous-titre interne : Interface réseau
        _allTools.Add(new ScriptTool
        {
            Id = "diag_network",
            Name = "Carte réseau et connectivité",
            Description = "Adaptateurs, débits négociés, adresses IP et test ping de connectivité.",
            Category = "DIAGNOSTIC",
            SubHeading = "Interface réseau",
            Icon = "🌐",
            Action = (log, ct) => DiagnosticService.DiagNetwork(log, ct),
            Tags = new[] { "réseau", "ping", "connectivité", "ip", "passerelle", "interface", "débit" }
        });

        // Sous-titre interne : Fichiers système Windows
        _allTools.Add(new ScriptTool
        {
            Id = "diag_sfc",
            Name = "Scan intégrité système (SFC)",
            Description = "Contrôle d'intégrité des fichiers protégés (sfc /scannow).",
            Category = "DIAGNOSTIC",
            SubHeading = "Fichiers système Windows",
            Icon = "🛡️",
            Action = (log, ct) => RepairService.RunSfc(log, ct),
            Tags = new[] { "sfc", "scannow", "intégrité", "fichiers", "système" }
        });

        // Sous-titre interne : Démarrage et Processus système
        _allTools.Add(new ScriptTool
        {
            Id = "diag_startup_manager",
            Name = "Gestionnaire des Programmes au Démarrage",
            Description = "Visualiser les logiciels qui se lancent avec Windows et désactiver les éléments superflus.",
            Category = "DIAGNOSTIC",
            SubHeading = "Démarrage et Processus système",
            Icon = "🚀",
            Action = (log, ct) => DiagnosticService.OpenStartupManagerDialog(log, ct),
            Tags = new[] { "démarrage", "startup", "boot", "programmes", "applications", "accélérer", "taskmgr" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 2 : 🖥️ MATÉRIEL ET PÉRIPHÉRIQUES
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterHardwareTools()
    {
        // Sous-titre interne : Mémoire vive (RAM)
        _allTools.Add(new ScriptTool
        {
            Id = "hw_mdsched",
            Name = "Diagnostic mémoire (mdsched)",
            Description = "Outil de diagnostic mémoire Windows au redémarrage (mdsched.exe).",
            Category = "MATERIEL",
            SubHeading = "Mémoire vive (RAM)",
            Icon = "🧠",
            Action = (log, ct) => HardwareService.RunRamTest(log, ct),
            Tags = new[] { "ram", "mdsched", "test", "mémoire", "diagnostic" }
        });

        // Sous-titre interne : Contrôleur graphique et Audio
        _allTools.Add(new ScriptTool
        {
            Id = "hw_dxdiag",
            Name = "Diagnostic matériel (DxDiag)",
            Description = "Diagnostic matériel DirectX, GPU et cartes audio (dxdiag.exe).",
            Category = "MATERIEL",
            SubHeading = "Contrôleur graphique et Audio",
            Icon = "🎮",
            Action = (log, ct) => HardwareService.LaunchDxdiag(log, ct),
            Tags = new[] { "dxdiag", "gpu", "directx", "vram", "audio", "graphique" }
        });

        // Sous-titre interne : Pilotes et Périphériques d'entrée/sortie
        _allTools.Add(new ScriptTool
        {
            Id = "hw_devmgmt",
            Name = "Gestionnaire de périphériques",
            Description = "Gestionnaire de périphériques (devmgmt.msc).",
            Category = "MATERIEL",
            SubHeading = "Pilotes et Périphériques d'entrée/sortie",
            Icon = "🔌",
            Action = (log, ct) => HardwareService.LaunchDevmgmt(log, ct),
            Tags = new[] { "devmgmt", "périphériques", "drivers", "pilotes", "erreurs" }
        });

        // Sous-titre interne : Banc d'essai et Performances globales
        _allTools.Add(new ScriptTool
        {
            Id = "hw_resmon",
            Name = "Moniteur de ressources (Resmon)",
            Description = "Moniteur de ressources en temps réel (resmon.exe).",
            Category = "MATERIEL",
            SubHeading = "Banc d'essai et Performances globales",
            Icon = "📊",
            Action = (log, ct) => HardwareService.LaunchResmon(log, ct),
            Tags = new[] { "resmon", "ressources", "cpu", "ram", "disque", "réseau" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "hw_msinfo",
            Name = "Informations système (Msinfo32)",
            Description = "Vue d'ensemble du matériel et configuration Windows (msinfo32.exe).",
            Category = "MATERIEL",
            SubHeading = "Banc d'essai et Performances globales",
            Icon = "ℹ️",
            Action = (log, ct) => HardwareService.LaunchMsinfo32(log, ct),
            Tags = new[] { "msinfo32", "matériel", "système", "os", "ressources" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "hw_winsat",
            Name = "Benchmark global (WinSAT)",
            Description = "Benchmark complet CPU, RAM, GPU et disques (winsat formal).",
            Category = "MATERIEL",
            SubHeading = "Banc d'essai et Performances globales",
            Icon = "⚙️",
            Action = (log, ct) => HardwareService.RunWinSat(log, ct),
            Tags = new[] { "benchmark", "winsat", "performance", "score", "cpu", "gpu", "ram", "disque" }
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
            Description = "Interface unifiée tout-en-un : diagnostic en direct, gestionnaire DNS, dépannage de connexion et scanners.",
            Category = "RESEAU",
            SubHeading = "",
            Icon = "🌐",
            Action = (log, ct) => NetworkService.OpenNetworkCenterDialog(log, ct),
            Tags = new[] { "centre", "réseau", "diagnostic", "dns", "triage", "reset", "wifi", "lan", "ip", "connectivité", "internet" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_reset_all",
            Name = "Dépannage & Grand Reset Réseau",
            Description = "Protocole curatif complet en 7 étapes : purge DNS/ARP, réinitialisation Winsock et TCP/IP, renouvellement IP.",
            Category = "RESEAU",
            SubHeading = "",
            Icon = "⚡",
            Action = (log, ct) => NetworkService.ResetAll(log, ct),
            Tags = new[] { "reset", "réinitialiser", "dépannage", "connexion", "coupure", "winsock", "tcpip", "dhcp", "arp", "flushdns" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_dns_manager",
            Name = "Gestionnaire et Changeur de DNS",
            Description = "Consulter le DNS actif, tester et appliquer en 1 clic (Cloudflare, Google, Quad9, DHCP).",
            Category = "RESEAU",
            SubHeading = "",
            Icon = "🛡️",
            Action = (log, ct) => DnsService.OpenDnsManagerDialog(log, ct),
            Tags = new[] { "dns", "cloudflare", "google", "quad9", "dhcp", "ipconfig", "changer", "serveurs", "sécurité" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_lan_scan",
            Name = "Scanner Réseau Local (LAN)",
            Description = "Découvrir tous les appareils et machines connectés sur la Box ou le sous-réseau local (IP et noms d'hôtes).",
            Category = "RESEAU",
            SubHeading = "",
            Icon = "🖥️",
            Action = (log, ct) => NetworkService.ScanLan(log, ct),
            Tags = new[] { "lan", "scan", "découverte", "machines", "hotes", "adresses", "ip", "box", "routeur" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_wifi_scan",
            Name = "Scanner les réseaux Wi-Fi",
            Description = "Analyse et inventaire des bornes sans fil environnantes avec puissance du signal en % et canaux (BSSID).",
            Category = "RESEAU",
            SubHeading = "",
            Icon = "📶",
            Action = (log, ct) => NetworkService.ScanWifi(log, ct),
            Tags = new[] { "wifi", "scan", "bornes", "puissance", "canaux", "wlan", "ssid", "signal" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_wifi_report",
            Name = "Rapport Wi-Fi complet (wlanreport)",
            Description = "Génération du rapport officiel Windows de diagnostic sans fil et ouverture automatique dans le navigateur.",
            Category = "RESEAU",
            SubHeading = "",
            Icon = "📊",
            Action = (log, ct) => DiagnosticService.GetWifiReport(log, ct),
            Tags = new[] { "wifi", "rapport", "html", "wlanreport", "historique", "santé", "déconnexions" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_flux_analysis",
            Name = "Connexions et Flux TCP Actifs",
            Description = "Afficher les connexions TCP établies en direct avec identification du processus Windows correspondant.",
            Category = "RESEAU",
            SubHeading = "",
            Icon = "🔍",
            Action = (log, ct) => NetworkService.AnalyzeFlux(log, ct),
            Tags = new[] { "connexions", "flux", "tcp", "ports", "processus", "netstat", "ip", "écoute" }
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
        // Sous-titre interne : Navigateurs web
        _allTools.Add(new ScriptTool
        {
            Id = "app_install_chrome",
            Name = "Installer Google Chrome",
            Description = "Installation silencieuse de Google Chrome via Winget.",
            Category = "APPLICATIONS",
            SubHeading = "Navigateurs web",
            Icon = "🌐",
            Action = (log, ct) => AppService.InstallChrome(log, ct),
            Tags = new[] { "chrome", "google", "navigateur", "web", "winget" }
        });

        // Sous-titre interne : Multimédia et Documents
        _allTools.Add(new ScriptTool
        {
            Id = "app_install_vlc",
            Name = "Installer VLC Media Player",
            Description = "Installation de VLC Media Player via Winget.",
            Category = "APPLICATIONS",
            SubHeading = "Multimédia et Documents",
            Icon = "🎬",
            Action = (log, ct) => AppService.InstallVlc(log, ct),
            Tags = new[] { "vlc", "vidéo", "médias", "musique", "winget" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "app_install_sumatra",
            Name = "Installer Sumatra PDF",
            Description = "Installation du lecteur PDF léger Sumatra.",
            Category = "APPLICATIONS",
            SubHeading = "Multimédia et Documents",
            Icon = "📄",
            Action = (log, ct) => AppService.InstallSumatraPdf(log, ct),
            Tags = new[] { "sumatra", "pdf", "lecteur", "winget" }
        });

        // Sous-titre interne : Utilitaires de compression et Système
        _allTools.Add(new ScriptTool
        {
            Id = "app_install_7zip",
            Name = "Installer 7-Zip",
            Description = "Installation de 7-Zip via Winget.",
            Category = "APPLICATIONS",
            SubHeading = "Utilitaires de compression et Système",
            Icon = "🗜️",
            Action = (log, ct) => AppService.Install7Zip(log, ct),
            Tags = new[] { "7zip", "archive", "compression", "winget", "installer" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "app_install_pwsh",
            Name = "Installer PowerShell 7",
            Description = "Installation de PowerShell 7 via Winget.",
            Category = "APPLICATIONS",
            SubHeading = "Utilitaires de compression et Système",
            Icon = "⚡",
            Action = (log, ct) => AppService.InstallPs7(log, ct),
            Tags = new[] { "powershell", "ps7", "terminal", "winget", "microsoft" }
        });

        // Sous-titre interne : Bureautique
        _allTools.Add(new ScriptTool
        {
            Id = "app_open_office",
            Name = "Télécharger Microsoft Office",
            Description = "Téléchargement de l'installateur Microsoft Office via Google Drive.",
            Category = "APPLICATIONS",
            SubHeading = "Bureautique",
            Icon = "💼",
            Action = (log, ct) => AppService.InstallOffice(log, ct),
            Tags = new[] { "office", "word", "excel", "microsoft", "téléchargement" }
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
        // Sous-titre interne : Mémoire vive et Processus système
        _allTools.Add(new ScriptTool
        {
            Id = "sec_top_ram",
            Name = "Top processus et charge RAM",
            Description = "Top 10 des applications les plus gourmandes en RAM.",
            Category = "SECURITE",
            SubHeading = "Mémoire vive et Processus système",
            Icon = "📈",
            Action = (log, ct) => DiagnosticService.GetRamUsage(log, ct),
            Tags = new[] { "processus", "ram", "charge", "consommation", "top" }
        });

        // Sous-titre interne : Journaux d'événements et Noyau Windows
        _allTools.Add(new ScriptTool
        {
            Id = "sec_event_errors",
            Name = "Journal erreurs critiques et plantages",
            Description = "Analyse des erreurs critiques récentes et BSOD dans le journal Système.",
            Category = "SECURITE",
            SubHeading = "Journaux d'événements et Noyau Windows",
            Icon = "⚠️",
            Action = (log, ct) => DiagnosticService.GetEventLog(log, ct),
            Tags = new[] { "erreurs", "journal", "plantages", "bsod", "critiques", "logs" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sec_perfmon_rel",
            Name = "Moniteur de fiabilité (Perfmon)",
            Description = "Historique de fiabilité et plantages du système (perfmon /rel).",
            Category = "SECURITE",
            SubHeading = "Journaux d'événements et Noyau Windows",
            Icon = "📉",
            Action = (log, ct) => DiagnosticService.LaunchReliabilityMonitor(log, ct),
            Tags = new[] { "fiabilité", "perfmon", "crashs", "historique", "plantages" }
        });

        // Sous-titre interne : Protection antivirale (Defender)
        _allTools.Add(new ScriptTool
        {
            Id = "sec_defender_status",
            Name = "État Windows Defender",
            Description = "État de protection en temps réel de Defender.",
            Category = "SECURITE",
            SubHeading = "Protection antivirale (Defender)",
            Icon = "🩺",
            Action = (log, ct) => DiagnosticService.GetDefenderStatus(log, ct),
            Tags = new[] { "defender", "état", "protection", "temps réel", "sécurité" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sec_defender_quickscan",
            Name = "Analyse rapide Defender",
            Description = "Analyse antivirus rapide (Start-MpScan -ScanType QuickScan).",
            Category = "SECURITE",
            SubHeading = "Protection antivirale (Defender)",
            Icon = "🛡️",
            Action = (log, ct) => SecurityService.DefenderQuickScan(log, ct),
            Tags = new[] { "defender", "scan", "antivirus", "rapide", "analyse" }
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

