using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ScriptsWindows.Models;
using ScriptsWindows.Services;

namespace ScriptsWindows.Core;

/// <summary>
/// Répertoire central de tous les outils disponibles dans l'application.
/// Permet la navigation par catégorie, la recherche globale et l'affichage des favoris.
/// </summary>
public static class ToolRegistry
{
    private static readonly List<ScriptTool> _allTools = new();

    static ToolRegistry()
    {
        RegisterDiagnosticTools();
        RegisterRepairTools();
        RegisterCleaningTools();
        RegisterNetworkTools();
        RegisterAppTools();
        RegisterSecurityTools();
        RegisterHardwareTools();
        RegisterReportsTools();
        RegisterUserTools();
        RegisterExportTools();
        RegisterPersonalizationTools();
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

    private static void RegisterDiagnosticTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "sys_report",
            Name = "Rapport Système Complet",
            Description = "Analyse détaillée de l'OS, CPU, RAM, GPU, disques et réseau.",
            Category = "DIAGNOSTIC",
            Icon = "📊",
            Action = (log, ct) => DiagnosticService.GetSystemReport(log, ct),
            Tags = new[] { "system", "info", "cpu", "ram", "gpu", "matériel" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sys_battery_report",
            Name = "Rapport Santé Batterie",
            Description = "Génère et ouvre le rapport HTML complet de la batterie.",
            Category = "DIAGNOSTIC",
            Icon = "🔋",
            Action = (log, ct) => DiagnosticService.GetBatteryReport(log, ct),
            Tags = new[] { "batterie", "power", "autonomie", "pc portable" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sys_battery_status",
            Name = "Statut Batterie en Direct",
            Description = "Pourcentage de charge, tension et statut d'alimentation.",
            Category = "DIAGNOSTIC",
            Icon = "⚡",
            Action = (log, ct) => DiagnosticService.GetBatteryStatus(log, ct),
            Tags = new[] { "batterie", "charge", "live" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sys_temp_report",
            Name = "Température CPU / WMI",
            Description = "Interroge les zones thermiques ACPI et sondes disponibles.",
            Category = "DIAGNOSTIC",
            Icon = "🌡️",
            Action = (log, ct) => DiagnosticService.GetTemperatureReport(log, ct),
            Tags = new[] { "temperature", "chaleur", "cpu", "thermal" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sys_ram_check",
            Name = "Détails Mémoire RAM",
            Description = "Vitesse, fabricant, slots et capacité par barrette installée.",
            Category = "DIAGNOSTIC",
            Icon = "🧠",
            Action = (log, ct) => DiagnosticService.CheckRam(log, ct),
            Tags = new[] { "ram", "mémoire", "slots", "ddr" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sys_ram_usage",
            Name = "Top Processus RAM",
            Description = "Affiche la consommation globale et le top 10 des applications.",
            Category = "DIAGNOSTIC",
            Icon = "📈",
            Action = (log, ct) => DiagnosticService.GetRamUsage(log, ct),
            Tags = new[] { "ram", "processus", "consommation" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sys_diag_network",
            Name = "Diagnostic Connexion",
            Description = "Test de ping, interfaces actives, adresses IP et passerelles.",
            Category = "DIAGNOSTIC",
            Icon = "🌐",
            Action = (log, ct) => DiagnosticService.DiagNetwork(log, ct),
            Tags = new[] { "ping", "réseau", "ip", "connexion" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sys_bitlocker_check",
            Name = "État Chiffrement BitLocker",
            Description = "Vérifie si les disques sont protégés et chiffrés par BitLocker.",
            Category = "DIAGNOSTIC",
            Icon = "🔒",
            Action = (log, ct) => DiagnosticService.CheckBitLocker(log, ct),
            Tags = new[] { "bitlocker", "chiffrement", "sécurité" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sys_event_log",
            Name = "Journal Erreurs Critiques",
            Description = "Affiche les erreurs système et crashes récents.",
            Category = "DIAGNOSTIC",
            Icon = "⚠️",
            Action = (log, ct) => DiagnosticService.GetEventLog(log, ct),
            Tags = new[] { "logs", "eventvwr", "crash", "erreurs" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sys_defender_status",
            Name = "État Windows Defender",
            Description = "Vérifie la protection temps réel et la date des signatures.",
            Category = "DIAGNOSTIC",
            Icon = "🛡️",
            Action = (log, ct) => DiagnosticService.GetDefenderStatus(log, ct),
            Tags = new[] { "antivirus", "defender", "protection" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sys_disk_warnings",
            Name = "Santé Disques & SMART",
            Description = "Liste les alertes matérielles de disques et l'état de santé.",
            Category = "DIAGNOSTIC",
            Icon = "💽",
            Action = (log, ct) => DiagnosticService.GetDiskWarnings(log, ct),
            Tags = new[] { "smart", "disque", "hdd", "ssd", "nvme" }
        });
    }

    private static void RegisterRepairTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "res_sfc",
            Name = "Scan & Réparation SFC",
            Description = "Exécute sfc /scannow pour réparer les fichiers système corrompus.",
            Category = "REPARATION",
            Icon = "🔧",
            Action = (log, ct) => RepairService.RunSfc(log, ct),
            Tags = new[] { "sfc", "scannow", "intégrité", "fichiers" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "res_dism_health",
            Name = "Vérification DISM",
            Description = "Vérifie l'état de l'image Windows (CheckHealth).",
            Category = "REPARATION",
            Icon = "🩺",
            Action = (log, ct) => RepairService.RunDismCheck(log, ct),
            Tags = new[] { "dism", "checkhealth", "scanhealth" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "res_dism_restore",
            Name = "Restauration DISM",
            Description = "Répare l'image Windows en ligne via RestoreHealth.",
            Category = "REPARATION",
            Icon = "🩹",
            Action = (log, ct) => RepairService.RunDismRestore(log, ct),
            Tags = new[] { "dism", "restorehealth", "réparation" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "res_chkdsk",
            Name = "Vérification Disque CHKDSK",
            Description = "Exécute chkdsk C: pour planifier une vérification complète du disque.",
            Category = "REPARATION",
            Icon = "🔍",
            Action = (log, ct) => RepairService.RunChkdsk("C", log, ct),
            Tags = new[] { "chkdsk", "disque", "ntfs", "erreurs" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "res_wu_reset",
            Name = "Réinitialiser Windows Update",
            Description = "Arrête les services WU, vide SoftwareDistribution et redémarre.",
            Category = "REPARATION",
            Icon = "🔄",
            Action = (log, ct) => RepairService.ResetWindowsUpdate(log, ct),
            Tags = new[] { "windows update", "wu", "softwaredistribution", "maj" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "res_restart_explorer",
            Name = "Redémarrer l'Explorateur",
            Description = "Relance le processus explorer.exe pour débloquer la barre des tâches.",
            Category = "REPARATION",
            Icon = "🗂️",
            Action = (log, ct) => RepairService.RestartExplorer(log, ct),
            Tags = new[] { "explorer", "bureau", "barre des taches" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "res_clear_icon_cache",
            Name = "Vider le Cache des Icônes",
            Description = "Supprime iconcache.db pour corriger les icônes blanches ou corrompues.",
            Category = "REPARATION",
            Icon = "🖼️",
            Action = (log, ct) => RepairService.RepairIcons(log, ct),
            Tags = new[] { "icones", "cache", "iconcache" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "res_restore_point",
            Name = "Créer Point de Restauration",
            Description = "Crée immédiatement un point de restauration système Windows.",
            Category = "REPARATION",
            Icon = "🛡️",
            Action = (log, ct) => RepairService.CreateRestorePoint(log, ct),
            Tags = new[] { "restauration", "backup", "sauvegarde" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "res_network_dns",
            Name = "Réparation Réseau & DNS",
            Description = "Réinitialise Winsock et vide le cache DNS (netsh winsock reset + ipconfig /flushdns).",
            Category = "REPARATION",
            Icon = "🌐",
            Action = (log, ct) => RepairService.RepairNetworkDns(log, ct),
            Tags = new[] { "winsock", "dns", "flushdns", "réseau", "réparation" }
        });
    }

    private static void RegisterCleaningTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "cl_temp",
            Name = "Purger les Fichiers Temp",
            Description = "Vide les dossiers Temp Windows et utilisateur (%TEMP%).",
            Category = "NETTOYAGE",
            Icon = "🧹",
            Action = (log, ct) => CleaningService.CleanTemp(log, ct),
            Tags = new[] { "temp", "temporaires", "nettoyage", "espace" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "cl_wu",
            Name = "Nettoyer Cache Windows Update",
            Description = "Supprime les anciens fichiers de mise à jour téléchargés.",
            Category = "NETTOYAGE",
            Icon = "📦",
            Action = (log, ct) => CleaningService.CleanWindowsUpdate(log, ct),
            Tags = new[] { "update", "softwaredistribution", "cache" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "cl_dns",
            Name = "Vider le Cache DNS",
            Description = "Exécute ipconfig /flushdns pour purger les résolutions DNS locales.",
            Category = "NETTOYAGE",
            Icon = "⚡",
            Action = (log, ct) => CleaningService.FlushDns(log, ct),
            Tags = new[] { "dns", "flushdns", "réseau" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "cl_disk",
            Name = "Outil Nettoyage Disque (Cleanmgr)",
            Description = "Lance l'assistant officiel de nettoyage de disque Windows.",
            Category = "NETTOYAGE",
            Icon = "🗑️",
            Action = (log, ct) => CleaningService.CleanDisk(log, ct),
            Tags = new[] { "cleanmgr", "disque", "nettoyage" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "cl_all",
            Name = "Nettoyage Complet Unifié",
            Description = "Nettoie en une seule action les temporaires, logs, corbeille et cache DNS.",
            Category = "NETTOYAGE",
            Icon = "✨",
            Action = (log, ct) => CleaningService.CleanAll(log, ct),
            Tags = new[] { "global", "total", "nettoyage" }
        });
    }

    private static void RegisterNetworkTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "net_dns_cloudflare",
            Name = "Appliquer DNS Cloudflare",
            Description = "Configure 1.1.1.1 et 1.0.0.1 (sécurisé & ultra-rapide).",
            Category = "RESEAU",
            Icon = "☁️",
            Action = (log, ct) => DnsService.SetCloudflare(log, ct),
            Tags = new[] { "dns", "cloudflare", "1.1.1.1" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_dns_google",
            Name = "Appliquer DNS Google",
            Description = "Configure 8.8.8.8 et 8.8.4.4 sur l'interface principale.",
            Category = "RESEAU",
            Icon = "🔍",
            Action = (log, ct) => DnsService.SetGoogle(log, ct),
            Tags = new[] { "dns", "google", "8.8.8.8" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_dns_dhcp",
            Name = "Restaurer DNS Automatique (DHCP)",
            Description = "Rétablit les serveurs DNS fournis par votre box internet.",
            Category = "RESEAU",
            Icon = "🔄",
            Action = (log, ct) => DnsService.RestoreDhcp(log, ct),
            Tags = new[] { "dhcp", "dns", "box", "auto" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_clear_arp",
            Name = "Purger la Table ARP",
            Description = "Supprime les entrées du cache ARP local (arp -d *).",
            Category = "RESEAU",
            Icon = "🛡️",
            Action = (log, ct) => NetworkService.ClearArp(log, ct),
            Tags = new[] { "arp", "cache", "lan" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_reset_tcp",
            Name = "Réinitialiser Stack TCP/IP",
            Description = "Réinitialise netsh int ip reset et réinitialise Winsock.",
            Category = "RESEAU",
            Icon = "🔌",
            Action = (log, ct) => NetworkService.ResetTcpIp(log, ct),
            Tags = new[] { "winsock", "tcp", "ip", "reset" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_wifi_scan",
            Name = "Scanner les Réseaux Wi-Fi",
            Description = "Liste les bornes Wi-Fi environnantes avec puissance et canaux.",
            Category = "RESEAU",
            Icon = "📶",
            Action = (log, ct) => NetworkService.ScanWifi(log, ct),
            Tags = new[] { "wifi", "wlan", "scan", "onde" }
        });
    }

    private static void RegisterAppTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "app_list_updates",
            Name = "Vérifier Mises à Jour (Winget)",
            Description = "Recherche les nouvelles versions disponibles pour vos logiciels.",
            Category = "APPLICATIONS",
            Icon = "🔍",
            Action = (log, ct) => AppService.ListUpdates(log, ct),
            Tags = new[] { "winget", "update", "logiciels", "applications" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "app_update_all",
            Name = "Mettre à Jour Tous les Logiciels",
            Description = "Lance la mise à niveau automatique de toutes les applications via winget.",
            Category = "APPLICATIONS",
            Icon = "🚀",
            Action = (log, ct) => AppService.UpdateAll(log, ct),
            Tags = new[] { "upgrade", "all", "tout mettre a jour" }
        });
    }

    private static void RegisterSecurityTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "sec_wifi_passwords",
            Name = "Afficher Mots de Passe Wi-Fi",
            Description = "Extrait les clés de sécurité Wi-Fi enregistrées sur cet ordinateur.",
            Category = "SECURITE",
            Icon = "🔑",
            Action = (log, ct) => SecurityService.DumpWifiPasswords(log, ct),
            Tags = new[] { "wifi", "mot de passe", "passwords", "wpa" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sec_wd_quick",
            Name = "Analyse Rapide Defender",
            Description = "Déclenche une analyse rapide antivirus Microsoft Defender.",
            Category = "SECURITE",
            Icon = "🛡️",
            Action = (log, ct) => SecurityService.DefenderQuickScan(log, ct),
            Tags = new[] { "scan", "antivirus", "defender", "virus" }
        });
    }

    private static void RegisterHardwareTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "hw_smart_detail",
            Name = "Test SMART Approfondi",
            Description = "Interroge le bus de stockage pour l'usure et la santé des SSD/HDD.",
            Category = "MATERIEL",
            Icon = "💾",
            Action = (log, ct) => HardwareService.RunSmartTest(log, ct),
            Tags = new[] { "smart", "ssd", "santé", "disque" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "hw_power_perf",
            Name = "Plan Performances Ultimes",
            Description = "Active le profil d'alimentation Ultimate Performance de Windows.",
            Category = "MATERIEL",
            Icon = "⚡",
            Action = (log, ct) => HardwareService.SetPlanUltimate(log, ct),
            Tags = new[] { "batterie", "powercfg", "performance" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "hw_ram_diag",
            Name = "Diagnostic Mémoire RAM",
            Description = "Lance mdsched.exe pour tester la mémoire au prochain redémarrage.",
            Category = "MATERIEL",
            Icon = "🧠",
            Action = (log, ct) => HardwareService.RunRamTest(log, ct),
            Tags = new[] { "ram", "mémoire", "mdsched", "test" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "hw_resmon",
            Name = "Moniteur de Ressources",
            Description = "Ouvre resmon.exe pour visualiser CPU, RAM, disque et réseau en temps réel.",
            Category = "MATERIEL",
            Icon = "📊",
            Action = (log, ct) => HardwareService.LaunchResmon(log, ct),
            Tags = new[] { "resmon", "ressources", "cpu", "ram", "réseau" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "hw_msinfo32",
            Name = "Informations Système",
            Description = "Ouvre msinfo32 — vue complète du matériel et de la configuration Windows.",
            Category = "MATERIEL",
            Icon = "🖥️",
            Action = (log, ct) => HardwareService.LaunchMsinfo32(log, ct),
            Tags = new[] { "msinfo32", "système", "matériel", "configuration" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "hw_dxdiag",
            Name = "Diagnostic GPU & Audio (DxDiag)",
            Description = "Ouvre dxdiag pour diagnostiquer DirectX, carte graphique et audio.",
            Category = "MATERIEL",
            Icon = "🎮",
            Action = (log, ct) => HardwareService.LaunchDxdiag(log, ct),
            Tags = new[] { "dxdiag", "directx", "gpu", "audio", "graphique" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "hw_devmgmt",
            Name = "Gestionnaire de Périphériques",
            Description = "Ouvre devmgmt.msc pour gérer les drivers et périphériques installés.",
            Category = "MATERIEL",
            Icon = "🔌",
            Action = (log, ct) => HardwareService.LaunchDevmgmt(log, ct),
            Tags = new[] { "devmgmt", "périphériques", "drivers", "pilotes" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "hw_winsat",
            Name = "Stress Test Matériel (WinSAT)",
            Description = "Exécute winsat formal pour évaluer les performances CPU, RAM, GPU et disque.",
            Category = "MATERIEL",
            Icon = "⚙️",
            Action = (log, ct) => HardwareService.RunWinSat(log, ct),
            Tags = new[] { "winsat", "benchmark", "stress", "performance", "matériel" }
        });
    }

    private static void RegisterReportsTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "rpt_reliability",
            Name = "Moniteur de Fiabilité (Plantages)",
            Description = "Ouvre l'historique des plantages et erreurs système (perfmon /rel).",
            Category = "RAPPORTS",
            Icon = "📉",
            Action = (log, ct) => DiagnosticService.LaunchReliabilityMonitor(log, ct),
            Tags = new[] { "fiabilité", "plantage", "crash", "historique", "perfmon" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "rpt_perfmon",
            Name = "Rapport de Performances",
            Description = "Génère un rapport complet des performances système (perfmon /report).",
            Category = "RAPPORTS",
            Icon = "📈",
            Action = (log, ct) => DiagnosticService.LaunchPerfmonReport(log, ct),
            Tags = new[] { "performances", "rapport", "perfmon", "analyse" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "rpt_mrt",
            Name = "Antivirus Autonome (MRT)",
            Description = "Lance l'outil officiel Microsoft de suppression de logiciels malveillants.",
            Category = "RAPPORTS",
            Icon = "🦠",
            Action = (log, ct) => DiagnosticService.LaunchMrt(log, ct),
            Tags = new[] { "mrt", "malware", "virus", "antivirus", "microsoft" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "rpt_eventvwr",
            Name = "Observateur d'Événements",
            Description = "Ouvre eventvwr.msc pour consulter les journaux système, sécurité et application.",
            Category = "RAPPORTS",
            Icon = "📋",
            Action = (log, ct) => DiagnosticService.LaunchEventViewer(log, ct),
            Tags = new[] { "eventvwr", "journaux", "événements", "logs" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "rpt_powercfg_energy",
            Name = "Diagnostic Alimentation",
            Description = "Analyse la consommation électrique et génère un rapport HTML (powercfg /energy).",
            Category = "RAPPORTS",
            Icon = "⚡",
            Action = (log, ct) => HardwareService.RunPowercfgEnergy(log, ct),
            Tags = new[] { "alimentation", "énergie", "powercfg", "batterie", "rapport" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "rpt_wifi_report",
            Name = "Rapport Wi-Fi",
            Description = "Génère le rapport HTML Wi-Fi complet (netsh wlan show wlanreport).",
            Category = "RAPPORTS",
            Icon = "📶",
            Action = (log, ct) => DiagnosticService.GetWifiReport(log, ct),
            Tags = new[] { "wifi", "wlan", "rapport", "réseau", "wlanreport" }
        });
    }

    private static void RegisterUserTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "usr_list",
            Name = "Lister Comptes Locaux",
            Description = "Affiche les utilisateurs Windows, groupes et statuts de compte.",
            Category = "SECURITE",
            Icon = "👥",
            Action = (log, ct) => UserService.ListUsers(log, ct),
            Tags = new[] { "utilisateurs", "comptes", "admin" }
        });
    }

    private static void RegisterExportTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "exp_win_key",
            Name = "Récupérer Clé Windows",
            Description = "Lit la clé de produit originale depuis le BIOS/UEFI (OA3x).",
            Category = "EXTRACTION",
            Icon = "🔑",
            Action = (log, ct) => ExportService.GetWindowsKey(log, ct),
            Tags = new[] { "cle windows", "oem", "licence", "bios" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "exp_drivers",
            Name = "Sauvegarder Pilotes Système",
            Description = "Exporte tous les pilotes tiers installés vers un dossier de sauvegarde.",
            Category = "EXTRACTION",
            Icon = "💾",
            Action = (log, ct) => ExportService.ExportDrivers(log, ct),
            Tags = new[] { "pilotes", "drivers", "backup" }
        });
    }

    private static void RegisterPersonalizationTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "prs_godmode",
            Name = "Activer le God Mode",
            Description = "Crée le raccourci vers le panneau de configuration étendu GodMode.",
            Category = "PERSONNALISATION",
            Icon = "👑",
            Action = (log, ct) => PersonalizationService.CreateGodMode(log, ct),
            Tags = new[] { "godmode", "panneau de configuration" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "prs_classic_menu",
            Name = "Menu Contextuel Windows 10",
            Description = "Rétablit le clic droit classique complet sur Windows 11.",
            Category = "PERSONNALISATION",
            Icon = "🖱️",
            Action = (log, ct) => PersonalizationService.SetClassicMenu(log, ct),
            Tags = new[] { "clic droit", "contextuel", "windows 11" }
        });
    }
}
