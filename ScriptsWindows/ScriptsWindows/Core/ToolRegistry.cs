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
        RegisterDiagnosticTools();       // Catégorie 1 : Diagnostic Matériel & Intégrité
        RegisterHardwareTools();         // Catégorie 2 : Matériel & Périphériques
        RegisterRepairTools();           // Catégorie 3 : Réparation Intégrale
        RegisterCleaningTools();         // Catégorie 4 : Nettoyage & Optimisation Espace
        RegisterSecretsTools();          // Catégorie 5 : Récupération d'Identifiants & Secrets
        RegisterNetworkTools();          // Catégorie 6 : Réseau & Configuration
        RegisterDiskTools();             // Catégorie 7 : Disque Dur & Stockage
        RegisterAppInstallationTools();  // Catégorie 8 : Installation d'Applications
        RegisterUpdatesTools();          // Catégorie 9 : Mises à Jour & Inventaire Logiciel
        RegisterSecurityTools();         // Catégorie 10 : Surveillance, Sécurité & Événements
        RegisterPersonalizationTools();  // Catégorie 11 : Personnalisation & Système
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

        // ─── Réparation (10) ───
        ["repair_dism_check"] = "dism.exe /Online /Cleanup-Image /ScanHealth",
        ["repair_dism_restore"] = "dism.exe /Online /Cleanup-Image /RestoreHealth",
        ["repair_pipeline"] = "dism.exe /RestoreHealth && sfc.exe /scannow",
        ["repair_chkdsk"] = "chkdsk.exe C: /f /r",
        ["repair_export_drivers"] = "dism.exe /Online /Export-Driver /Destination:C:\\DriversBackup",
        ["repair_wu"] = "net stop wuauserv && ren SoftwareDistribution && net start wuauserv",
        ["repair_network_stack"] = "netsh.exe int ip reset && netsh.exe winsock reset && ipconfig.exe /flushdns",
        ["repair_spooler"] = "net stop spooler && del %systemroot%\\System32\\spool\\printers\\* /Q && net start spooler",
        ["repair_uwp_store"] = "wsreset.exe && powershell Add-AppxPackage",
        ["repair_icon_cache"] = "taskkill.exe /f /im explorer.exe && del %localappdata%\\IconCache.db && start explorer.exe",

        // ─── Nettoyage (6) ───
        ["clean_temp"] = "del /q /f /s %TEMP%\\* && del /q /f /s C:\\Windows\\Temp\\*",
        ["clean_wu_cache"] = "net stop wuauserv && del /q /f /s C:\\Windows\\SoftwareDistribution\\Download\\*",
        ["clean_browser_caches"] = "powershell Remove-Item (Caches Chrome/Edge/Firefox)",
        ["clean_cleanmgr"] = "cleanmgr.exe /sagerun:1",
        ["clean_full_unified"] = "powershell Clear-RecycleBin && ipconfig /flushdns && del %TEMP%\\*",
        ["clean_winsxs"] = "dism.exe /Online /Cleanup-Image /StartComponentCleanup /ResetBase",

        // ─── Secrets & Clés (5) ───
        ["sec_wifi_show"] = "netsh.exe wlan show profiles key=clear",
        ["sec_wifi_export"] = "netsh.exe wlan export profile folder=%USERPROFILE%\\Desktop key=clear",
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
        ["app_open_office"] = "start https://officecdn.microsoft.com / setup.exe",

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
        ["tweak_classic_context"] = "reg.exe add HKCU\\Software\\Classes\\CLSID\\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\\InprocServer32"
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
    // CATÉGORIE 1 : 🔍 DIAGNOSTIC MATÉRIEL & INTÉGRITÉ
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterDiagnosticTools()
    {
        // Sous-titre interne : Carte mère & Processeur
        _allTools.Add(new ScriptTool
        {
            Id = "diag_mobo",
            Name = "Carte mère, BIOS & système",
            Description = "Modèle, fabricant, version BIOS via Get-CimInstance Win32_BaseBoard, Win32_BIOS.",
            Category = "DIAGNOSTIC",
            SubHeading = "Carte mère & Processeur",
            Icon = "🖥️",
            Action = (log, ct) => DiagnosticService.GetMotherboardBiosInfo(log, ct),
            Tags = new[] { "carte mère", "bios", "système", "uefi", "processeur", "modèle" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "diag_cpu",
            Name = "CPU & températures",
            Description = "Cœurs, fréquence, charge CPU et sondes thermiques ACPI (MSAcpi_ThermalZoneTemperature).",
            Category = "DIAGNOSTIC",
            SubHeading = "Carte mère & Processeur",
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
            Name = "Disques - santé S.M.A.R.T.",
            Description = "Santé S.M.A.R.T. et type de disque (NVMe/SSD/HDD via Get-PhysicalDisk).",
            Category = "DIAGNOSTIC",
            SubHeading = "Stockage physique",
            Icon = "💾",
            Action = (log, ct) => DiskService.GetPhysicalDisks(log, ct),
            Tags = new[] { "smart", "santé", "disques", "ssd", "nvme", "hdd", "usure" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "diag_volumes",
            Name = "Disques - volumes & espace",
            Description = "Lecteurs, taux d'occupation et espace disponible (Get-Volume).",
            Category = "DIAGNOSTIC",
            SubHeading = "Stockage physique",
            Icon = "📊",
            Action = (log, ct) => DiskService.GetVolumesStatus(log, ct),
            Tags = new[] { "volumes", "espace", "disque", "libre", "taille", "partitions" }
        });

        // Sous-titre interne : Alimentation & Batterie
        _allTools.Add(new ScriptTool
        {
            Id = "diag_battery",
            Name = "Batterie - santé & usure",
            Description = "Tension/pourcentage direct et rapport d'usure (powercfg /batteryreport).",
            Category = "DIAGNOSTIC",
            SubHeading = "Alimentation & Batterie",
            Icon = "🔋",
            Action = (log, ct) => DiagnosticService.GetFullBatteryReport(log, ct),
            Tags = new[] { "batterie", "powercfg", "batteryreport", "santé", "usure", "tension" }
        });

        // Sous-titre interne : Interface réseau
        _allTools.Add(new ScriptTool
        {
            Id = "diag_network",
            Name = "Carte réseau & connectivité",
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
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 2 : 🖥️ MATÉRIEL & PÉRIPHÉRIQUES
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

        // Sous-titre interne : Contrôleur graphique & Audio
        _allTools.Add(new ScriptTool
        {
            Id = "hw_dxdiag",
            Name = "Diagnostic matériel (DxDiag)",
            Description = "Diagnostic matériel DirectX, GPU et cartes audio (dxdiag.exe).",
            Category = "MATERIEL",
            SubHeading = "Contrôleur graphique & Audio",
            Icon = "🎮",
            Action = (log, ct) => HardwareService.LaunchDxdiag(log, ct),
            Tags = new[] { "dxdiag", "gpu", "directx", "vram", "audio", "graphique" }
        });

        // Sous-titre interne : Pilotes & Périphériques d'entrée/sortie
        _allTools.Add(new ScriptTool
        {
            Id = "hw_devmgmt",
            Name = "Gestionnaire de périphériques",
            Description = "Gestionnaire de périphériques (devmgmt.msc).",
            Category = "MATERIEL",
            SubHeading = "Pilotes & Périphériques d'entrée/sortie",
            Icon = "🔌",
            Action = (log, ct) => HardwareService.LaunchDevmgmt(log, ct),
            Tags = new[] { "devmgmt", "périphériques", "drivers", "pilotes", "erreurs" }
        });

        // Sous-titre interne : Banc d'essai & Performances globales
        _allTools.Add(new ScriptTool
        {
            Id = "hw_resmon",
            Name = "Moniteur de ressources (Resmon)",
            Description = "Moniteur de ressources en temps réel (resmon.exe).",
            Category = "MATERIEL",
            SubHeading = "Banc d'essai & Performances globales",
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
            SubHeading = "Banc d'essai & Performances globales",
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
            SubHeading = "Banc d'essai & Performances globales",
            Icon = "⚙️",
            Action = (log, ct) => HardwareService.RunWinSat(log, ct),
            Tags = new[] { "benchmark", "winsat", "performance", "score", "cpu", "gpu", "ram", "disque" }
        });

        // Sous-titre interne : Gestion d'énergie
        _allTools.Add(new ScriptTool
        {
            Id = "hw_power_perf",
            Name = "Profil performances élevées",
            Description = "Profil d'alimentation Ultimate Performance / Hautes performances.",
            Category = "MATERIEL",
            SubHeading = "Gestion d'énergie",
            Icon = "⚡",
            Action = (log, ct) => HardwareService.SetPlanHigh(log, ct),
            Tags = new[] { "alimentation", "énergie", "performances", "powercfg", "profil" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 3 : 🔧 RÉPARATION INTÉGRALE
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterRepairTools()
    {
        // Sous-titre interne : Magasin d'images Windows (Component Store)
        _allTools.Add(new ScriptTool
        {
            Id = "repair_dism_check",
            Name = "Vérification DISM (ScanHealth)",
            Description = "Vérification de corruption du magasin (DISM /ScanHealth).",
            Category = "REPARATION",
            SubHeading = "Magasin d'images Windows (Component Store)",
            Icon = "🔎",
            Action = (log, ct) => RepairService.RunDismCheck(log, ct),
            Tags = new[] { "dism", "checkhealth", "scanhealth", "corruption", "image" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "repair_dism_restore",
            Name = "Restauration DISM (RestoreHealth)",
            Description = "Réparation en ligne du magasin (DISM /RestoreHealth).",
            Category = "REPARATION",
            SubHeading = "Magasin d'images Windows (Component Store)",
            Icon = "🩺",
            Action = (log, ct) => RepairService.RunDismRestore(log, ct),
            Tags = new[] { "dism", "restorehealth", "image", "windows", "réparation" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "repair_pipeline",
            Name = "Pipeline curatif global (DISM + SFC)",
            Description = "Exécution enchaînée DISM RestoreHealth puis sfc /scannow.",
            Category = "REPARATION",
            SubHeading = "Magasin d'images Windows (Component Store)",
            Icon = "🩹",
            Action = (log, ct) => RepairService.RunCurativePipeline(log, ct),
            Tags = new[] { "pipeline", "curatif", "dism", "sfc", "global", "restauration" }
        });

        // Sous-titre interne : Système de fichiers & Partitions
        _allTools.Add(new ScriptTool
        {
            Id = "repair_chkdsk",
            Name = "Correction disque (CHKDSK)",
            Description = "Réparation des erreurs disques et secteurs (chkdsk C: /f /r).",
            Category = "REPARATION",
            SubHeading = "Système de fichiers & Partitions",
            Icon = "🔍",
            Action = (log, ct) => RepairService.RunChkdsk("C", log, ct),
            Tags = new[] { "chkdsk", "disque", "réparation", "ntfs", "erreurs" }
        });

        // Sous-titre interne : Pilotes de périphériques
        _allTools.Add(new ScriptTool
        {
            Id = "repair_export_drivers",
            Name = "Sauvegarder pilotes système (DISM)",
            Description = "Export et sauvegarde vers C:\\DriversBackup (DISM /Export-Driver).",
            Category = "REPARATION",
            SubHeading = "Pilotes de périphériques",
            Icon = "💾",
            Action = (log, ct) => ExportService.ExportDrivers(log, ct),
            Tags = new[] { "pilotes", "drivers", "backup", "sauvegarde", "dism" }
        });

        // Sous-titre interne : Services de mise à jour Windows
        _allTools.Add(new ScriptTool
        {
            Id = "repair_wu",
            Name = "Réinitialisation Windows Update",
            Description = "Réinitialisation intégrale des composants WU (SoftwareDistribution/Catroot2).",
            Category = "REPARATION",
            SubHeading = "Services de mise à jour Windows",
            Icon = "🔄",
            Action = (log, ct) => RepairService.ResetWindowsUpdate(log, ct),
            Tags = new[] { "windows update", "wu", "softwaredistribution", "catroot2", "reset", "mise a jour" }
        });

        // Sous-titre interne : Pile réseau & Protocoles
        _allTools.Add(new ScriptTool
        {
            Id = "repair_network_stack",
            Name = "Réparation pile réseau & Winsock",
            Description = "Réinitialisation TCP/IP, Winsock, purge ARP et bail DHCP.",
            Category = "REPARATION",
            SubHeading = "Pile réseau & Protocoles",
            Icon = "🔌",
            Action = (log, ct) => NetworkService.ResetAll(log, ct),
            Tags = new[] { "réseau", "winsock", "tcp", "ip", "arp", "pile", "connexion" }
        });

        // Sous-titre interne : Sous-système d'impression
        _allTools.Add(new ScriptTool
        {
            Id = "repair_spooler",
            Name = "Déblocage spouleur d'impression",
            Description = "Purge de spool\\PRINTERS et relance du spouleur d'impression.",
            Category = "REPARATION",
            SubHeading = "Sous-système d'impression",
            Icon = "🖨️",
            Action = (log, ct) => RepairService.FixPrintSpooler(log, ct),
            Tags = new[] { "spouleur", "imprimante", "print", "spooler", "déblocage", "purge" }
        });

        // Sous-titre interne : Plateforme d'applications universelles (UWP)
        _allTools.Add(new ScriptTool
        {
            Id = "repair_uwp_store",
            Name = "Réparation Microsoft Store & UWP",
            Description = "Réenregistrement complet du Microsoft Store et paquets UWP.",
            Category = "REPARATION",
            SubHeading = "Plateforme d'applications universelles (UWP)",
            Icon = "🏬",
            Action = (log, ct) => RepairService.RepairWindowsStore(log, ct),
            Tags = new[] { "store", "microsoft store", "uwp", "wsreset", "applications", "paquets" }
        });

        // Sous-titre interne : Interface graphique (Shell Windows)
        _allTools.Add(new ScriptTool
        {
            Id = "repair_icon_cache",
            Name = "Reconstruction cache icônes & miniatures",
            Description = "Suppression de iconcache*.db / thumbcache*.db et relance d'Explorer.",
            Category = "REPARATION",
            SubHeading = "Interface graphique (Shell Windows)",
            Icon = "🖼️",
            Action = (log, ct) => RepairService.RepairIcons(log, ct),
            Tags = new[] { "icones", "miniatures", "cache", "iconcache", "thumbcache", "explorer" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 4 : 🧹 NETTOYAGE & OPTIMISATION ESPACE
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterCleaningTools()
    {
        // Sous-titre interne : Stockage temporaire OS & Profils
        _allTools.Add(new ScriptTool
        {
            Id = "clean_temp",
            Name = "Purger les fichiers temp",
            Description = "Purge des dossiers temporaires (Windows\\Temp et %TEMP%).",
            Category = "NETTOYAGE",
            SubHeading = "Stockage temporaire OS & Profils",
            Icon = "📁",
            Action = (log, ct) => CleaningService.CleanTemp(log, ct),
            Tags = new[] { "temp", "temporaires", "purge", "fichiers" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "clean_wu_cache",
            Name = "Purger téléchargements Windows Update",
            Description = "Suppression du dossier SoftwareDistribution\\Download.",
            Category = "NETTOYAGE",
            SubHeading = "Stockage temporaire OS & Profils",
            Icon = "📦",
            Action = (log, ct) => CleaningService.CleanWindowsUpdate(log, ct),
            Tags = new[] { "windows update", "téléchargements", "softwaredistribution", "download", "purge" }
        });

        // Sous-titre interne : Navigateurs web & Explorateur
        _allTools.Add(new ScriptTool
        {
            Id = "clean_browser_caches",
            Name = "Nettoyer caches navigateurs & vignettes",
            Description = "Nettoyage des caches (Chrome, Edge, Firefox, Brave) et vignettes.",
            Category = "NETTOYAGE",
            SubHeading = "Navigateurs web & Explorateur",
            Icon = "🌐",
            Action = (log, ct) => CleaningService.CleanBrowsers(log, ct),
            Tags = new[] { "navigateurs", "cache", "chrome", "edge", "firefox", "brave", "vignettes", "miniatures" }
        });

        // Sous-titre interne : Disque système (Nettoyage natif)
        _allTools.Add(new ScriptTool
        {
            Id = "clean_cleanmgr",
            Name = "Outil nettoyage disque (Cleanmgr)",
            Description = "Assistant de nettoyage de disque natif (cleanmgr.exe).",
            Category = "NETTOYAGE",
            SubHeading = "Disque système (Nettoyage natif)",
            Icon = "🗑️",
            Action = (log, ct) => CleaningService.CleanDisk(log, ct),
            Tags = new[] { "cleanmgr", "disque", "nettoyage", "assistant" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "clean_full_unified",
            Name = "Nettoyage complet unifié",
            Description = "Purge en 1 clic : corbeille, logs, temporaires et cache DNS.",
            Category = "NETTOYAGE",
            SubHeading = "Disque système (Nettoyage natif)",
            Icon = "✨",
            Action = (log, ct) => CleaningService.CleanAll(log, ct),
            Tags = new[] { "nettoyage", "unifié", "complet", "temp", "corbeille", "dns", "logs" }
        });

        // Sous-titre interne : Magasin de composants Windows
        _allTools.Add(new ScriptTool
        {
            Id = "clean_winsxs",
            Name = "Nettoyage magasin WinSxS (DISM)",
            Description = "Optimisation du dossier WinSxS (DISM /StartComponentCleanup /ResetBase).",
            Category = "NETTOYAGE",
            SubHeading = "Magasin de composants Windows",
            Icon = "🧹",
            Action = (log, ct) => CleaningService.CleanWinSxS(log, ct),
            Tags = new[] { "winsxs", "dism", "optimisation", "cleanup", "resetbase", "espace" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 5 : 🔑 RÉCUPÉRATION D'IDENTIFIANTS & SECRETS
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

        _allTools.Add(new ScriptTool
        {
            Id = "sec_wifi_export",
            Name = "Exporter profils & clés Wi-Fi",
            Description = "Sauvegarde des profils et clés Wi-Fi en XML sur le Bureau.",
            Category = "SECRETS",
            SubHeading = "Cartes réseau sans fil (Wi-Fi)",
            Icon = "💾",
            Action = (log, ct) => ExportService.ExportWifiAndApps(log, ct),
            Tags = new[] { "export", "wifi", "xml", "bureau", "profils", "sauvegarde" }
        });

        // Sous-titre interne : Dossiers utilisateurs & Clés d'authentification
        _allTools.Add(new ScriptTool
        {
            Id = "sec_audit_sensitive",
            Name = "Audit documents sensibles",
            Description = "Inventaire des clés SSH (.ssh), identifiants Cloud (.aws, .docker), fichiers .env et mots de passe.",
            Category = "SECRETS",
            SubHeading = "Dossiers utilisateurs & Clés d'authentification",
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
            Name = "Extraire l'historique web & profils",
            Description = "Extraction SQLite directe des 35 dernières pages visitées (Chrome, Edge, Brave, Firefox).",
            Category = "SECRETS",
            SubHeading = "Navigateurs web (Bases locales)",
            Icon = "🌐",
            Action = (log, ct) => ExportService.AnalyzeBrowserHistory(log, ct),
            Tags = new[] { "historique", "web", "profils", "chrome", "edge", "sqlite" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 6 : 🌐 RÉSEAU & CONFIGURATION
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterNetworkTools()
    {
        // Sous-titre interne : Serveurs de noms de domaine (DNS)
        _allTools.Add(new ScriptTool
        {
            Id = "net_dns_cloudflare",
            Name = "Appliquer DNS Cloudflare",
            Description = "Application des DNS Cloudflare (1.1.1.1 / 1.0.0.1).",
            Category = "RESEAU",
            SubHeading = "Serveurs de noms de domaine (DNS)",
            Icon = "☁️",
            Action = (log, ct) => DnsService.SetCloudflare(log, ct),
            Tags = new[] { "dns", "cloudflare", "1.1.1.1", "rapide", "sécurisé" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_dns_google",
            Name = "Appliquer DNS Google",
            Description = "Application des DNS Google (8.8.8.8 / 8.8.4.4).",
            Category = "RESEAU",
            SubHeading = "Serveurs de noms de domaine (DNS)",
            Icon = "🔍",
            Action = (log, ct) => DnsService.SetGoogle(log, ct),
            Tags = new[] { "dns", "google", "8.8.8.8", "configuration" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_dns_dhcp",
            Name = "Restaurer DNS automatique (DHCP)",
            Description = "Rétablissement du mode automatique (DHCP de la box).",
            Category = "RESEAU",
            SubHeading = "Serveurs de noms de domaine (DNS)",
            Icon = "🔄",
            Action = (log, ct) => DnsService.RestoreDhcp(log, ct),
            Tags = new[] { "dns", "dhcp", "automatique", "box", "routeur", "restaurer" }
        });

        // Sous-titre interne : Table de routage & Cache ARP
        _allTools.Add(new ScriptTool
        {
            Id = "net_arp_clear",
            Name = "Purger la table ARP",
            Description = "Purge complète de la table ARP (arp -d *).",
            Category = "RESEAU",
            SubHeading = "Table de routage & Cache ARP",
            Icon = "🛡️",
            Action = (log, ct) => NetworkService.ClearArp(log, ct),
            Tags = new[] { "arp", "purge", "table", "cache", "mac", "ip" }
        });

        // Sous-titre interne : Carte sans fil & Environnement radio
        _allTools.Add(new ScriptTool
        {
            Id = "net_wifi_scan",
            Name = "Scanner les réseaux Wi-Fi",
            Description = "Scan des bornes Wi-Fi avec puissance et canaux.",
            Category = "RESEAU",
            SubHeading = "Carte sans fil & Environnement radio",
            Icon = "📶",
            Action = (log, ct) => NetworkService.ScanWifi(log, ct),
            Tags = new[] { "wifi", "scan", "bornes", "puissance", "canaux", "wlan" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_wifi_report",
            Name = "Rapport Wi-Fi complet",
            Description = "Génération du rapport de diagnostic complet (netsh wlan show wlanreport).",
            Category = "RESEAU",
            SubHeading = "Carte sans fil & Environnement radio",
            Icon = "📊",
            Action = (log, ct) => DiagnosticService.GetWifiReport(log, ct),
            Tags = new[] { "wifi", "rapport", "html", "wlanreport", "historique" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 7 : 💽 DISQUE DUR & STOCKAGE
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterDiskTools()
    {
        // Sous-titre interne : Tables de partitions & Volumes
        _allTools.Add(new ScriptTool
        {
            Id = "disk_mgmt_gui",
            Name = "Gestionnaire de disques (GUI)",
            Description = "Console de gestion des partitions (diskmgmt.msc).",
            Category = "DISQUE",
            SubHeading = "Tables de partitions & Volumes",
            Icon = "💽",
            Action = (log, ct) => DiskService.LaunchDiskManagement(log, ct),
            Tags = new[] { "diskmgmt", "gestionnaire", "partitions", "volumes", "msc" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "disk_diskpart",
            Name = "Console de formatage DISKPART",
            Description = "Invite de commandes DISKPART en mode administrateur.",
            Category = "DISQUE",
            SubHeading = "Tables de partitions & Volumes",
            Icon = "💻",
            Action = (log, ct) => DiskService.LaunchDiskpart(log, ct),
            Tags = new[] { "diskpart", "formatage", "cmd", "partitions", "admin" }
        });

        // Sous-titre interne : Supports Flash & Mécaniques (SSD / HDD)
        _allTools.Add(new ScriptTool
        {
            Id = "disk_defrag_gui",
            Name = "Défragmenteur de disque (GUI)",
            Description = "Défragmenteur de disque Windows (dfrgui.exe).",
            Category = "DISQUE",
            SubHeading = "Supports Flash & Mécaniques (SSD / HDD)",
            Icon = "🛠️",
            Action = (log, ct) => DiskService.LaunchDefragGui(log, ct),
            Tags = new[] { "défragmenteur", "dfrgui", "optimisation", "disque", "lecteurs" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "disk_trim_opt",
            Name = "Optimisation disque & TRIM SSD",
            Description = "Commande d'optimisation TRIM pour SSD (defrag /O).",
            Category = "DISQUE",
            SubHeading = "Supports Flash & Mécaniques (SSD / HDD)",
            Icon = "⚡",
            Action = (log, ct) => DiskService.OptimizeDrives(log, ct),
            Tags = new[] { "trim", "ssd", "defrag", "optimisation", "disque" }
        });

        // Sous-titre interne : Sécurité & Chiffrement de volume
        _allTools.Add(new ScriptTool
        {
            Id = "disk_bitlocker",
            Name = "État chiffrement BitLocker",
            Description = "État de protection et de chiffrement des disques (manage-bde).",
            Category = "DISQUE",
            SubHeading = "Sécurité & Chiffrement de volume",
            Icon = "🔒",
            Action = (log, ct) => DiagnosticService.CheckBitLocker(log, ct),
            Tags = new[] { "bitlocker", "chiffrement", "sécurité", "volumes", "manage-bde" }
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

        // Sous-titre interne : Multimédia & Documents
        _allTools.Add(new ScriptTool
        {
            Id = "app_install_vlc",
            Name = "Installer VLC Media Player",
            Description = "Installation de VLC Media Player via Winget.",
            Category = "APPLICATIONS",
            SubHeading = "Multimédia & Documents",
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
            SubHeading = "Multimédia & Documents",
            Icon = "📄",
            Action = (log, ct) => AppService.InstallSumatraPdf(log, ct),
            Tags = new[] { "sumatra", "pdf", "lecteur", "winget" }
        });

        // Sous-titre interne : Utilitaires de compression & Système
        _allTools.Add(new ScriptTool
        {
            Id = "app_install_7zip",
            Name = "Installer 7-Zip",
            Description = "Installation de 7-Zip via Winget.",
            Category = "APPLICATIONS",
            SubHeading = "Utilitaires de compression & Système",
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
            SubHeading = "Utilitaires de compression & Système",
            Icon = "⚡",
            Action = (log, ct) => AppService.InstallPs7(log, ct),
            Tags = new[] { "powershell", "ps7", "terminal", "winget", "microsoft" }
        });

        // Sous-titre interne : Bureautique
        _allTools.Add(new ScriptTool
        {
            Id = "app_open_office",
            Name = "Télécharger Microsoft Office 2024",
            Description = "Accès à l'installateur officiel Microsoft Office.",
            Category = "APPLICATIONS",
            SubHeading = "Bureautique",
            Icon = "💼",
            Action = (log, ct) => AppService.InstallOffice(log, ct),
            Tags = new[] { "office", "word", "excel", "microsoft", "téléchargement" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 9 : 🔄 MISES À JOUR & INVENTAIRE LOGICIEL
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterUpdatesTools()
    {
        // Sous-titre interne : Gestionnaire de paquets (Winget)
        _allTools.Add(new ScriptTool
        {
            Id = "upd_list_apps",
            Name = "Lister les logiciels installés",
            Description = "Liste de l'ensemble des logiciels installés (winget list).",
            Category = "MISES_A_JOUR",
            SubHeading = "Gestionnaire de paquets (Winget)",
            Icon = "📋",
            Action = (log, ct) => AppService.GetInstalledApps(log, ct),
            Tags = new[] { "logiciels", "inventaire", "liste", "winget", "applications" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "upd_check_updates",
            Name = "Vérifier mises à jour disponibles",
            Description = "Détection des mises à jour disponibles (winget upgrade).",
            Category = "MISES_A_JOUR",
            SubHeading = "Gestionnaire de paquets (Winget)",
            Icon = "🔍",
            Action = (log, ct) => AppService.ListUpdates(log, ct),
            Tags = new[] { "mises a jour", "vérifier", "winget", "disponibles", "obsolètes" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "upd_apply_all",
            Name = "Mettre à jour tous les logiciels",
            Description = "Mise à niveau automatique de toutes les applications (winget upgrade --all).",
            Category = "MISES_A_JOUR",
            SubHeading = "Gestionnaire de paquets (Winget)",
            Icon = "🚀",
            Action = (log, ct) => AppService.UpdateAll(log, ct),
            Tags = new[] { "mise a jour", "upgrade", "all", "winget", "logiciels" }
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
    // CATÉGORIE 10 : 🛡️ SURVEILLANCE, SÉCURITÉ & ÉVÉNEMENTS
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterSecurityTools()
    {
        // Sous-titre interne : Mémoire vive & Processus système
        _allTools.Add(new ScriptTool
        {
            Id = "sec_top_ram",
            Name = "Top processus & charge RAM",
            Description = "Top 10 des applications les plus gourmandes en RAM.",
            Category = "SECURITE",
            SubHeading = "Mémoire vive & Processus système",
            Icon = "📈",
            Action = (log, ct) => DiagnosticService.GetRamUsage(log, ct),
            Tags = new[] { "processus", "ram", "charge", "consommation", "top" }
        });

        // Sous-titre interne : Journaux d'événements & Noyau Windows
        _allTools.Add(new ScriptTool
        {
            Id = "sec_event_errors",
            Name = "Journal erreurs critiques & plantages",
            Description = "Analyse des erreurs critiques récentes et BSOD dans le journal Système.",
            Category = "SECURITE",
            SubHeading = "Journaux d'événements & Noyau Windows",
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
            SubHeading = "Journaux d'événements & Noyau Windows",
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
    // CATÉGORIE 11 : 🎨 PERSONNALISATION & SYSTÈME
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterPersonalizationTools()
    {
        // Sous-titre interne : Panneau de configuration étendu
        _allTools.Add(new ScriptTool
        {
            Id = "tweak_godmode",
            Name = "Activer le God Mode",
            Description = "Création du raccourci dossier God Mode.",
            Category = "PERSONNALISATION",
            SubHeading = "Panneau de configuration étendu",
            Icon = "👑",
            Action = (log, ct) => PersonalizationService.CreateGodMode(log, ct),
            Tags = new[] { "god mode", "panneau de configuration", "raccourci" }
        });

        // Sous-titre interne : Explorateur Windows 11
        _allTools.Add(new ScriptTool
        {
            Id = "tweak_classic_context",
            Name = "Menu contextuel Windows 10",
            Description = "Rétablissement du menu clic droit classique Windows 10.",
            Category = "PERSONNALISATION",
            SubHeading = "Explorateur Windows 11",
            Icon = "🖱️",
            Action = (log, ct) => PersonalizationService.SetClassicMenu(log, ct),
            Tags = new[] { "menu contextuel", "windows 10", "clic droit", "windows 11" }
        });
    }
}
