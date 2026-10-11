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
        RegisterDiskTools();
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
            Name = "Réparer Composants Windows Update",
            Description = "Arrête les services WU, réinitialise Catroot2/SoftwareDistribution et relance pour corriger les erreurs de mise à jour.",
            Category = "REPARATION",
            Icon = "🔄",
            Action = (log, ct) => RepairService.ResetWindowsUpdate(log, ct),
            Tags = new[] { "windows update", "wu", "softwaredistribution", "catroot2", "réparation" }
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
            Name = "Purger Téléchargements Windows Update",
            Description = "Supprime les fichiers temporaires téléchargés dans SoftwareDistribution\\Download pour libérer plusieurs Go d'espace.",
            Category = "NETTOYAGE",
            Icon = "📦",
            Action = (log, ct) => CleaningService.CleanWindowsUpdate(log, ct),
            Tags = new[] { "update", "softwaredistribution", "cache", "espace", "téléchargements" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "cl_browsers_thumbnails",
            Name = "Nettoyer Caches Navigateurs & Vignettes",
            Description = "Supprime les caches volumineux de Chrome, Edge, Firefox, Brave et purge le cache des miniatures Windows.",
            Category = "NETTOYAGE",
            Icon = "🌐",
            Action = (log, ct) => CleaningService.CleanBrowsers(log, ct),
            Tags = new[] { "navigateur", "cache", "chrome", "edge", "firefox", "vignettes", "miniatures" }
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

        _allTools.Add(new ScriptTool
        {
            Id = "cl_winsxs",
            Name = "Nettoyage Magasin WinSxS (DISM)",
            Description = "Purge et optimise le magasin des composants Windows Update (DISM /StartComponentCleanup /ResetBase).",
            Category = "NETTOYAGE",
            Icon = "🧹",
            Action = (log, ct) => CleaningService.CleanWinSxS(log, ct),
            Tags = new[] { "winsxs", "dism", "nettoyage", "cleanup", "resetbase" }
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
            Id = "net_reset_all",
            Name = "Réinitialisation Complète Pile Réseau",
            Description = "Réinitialise complètement TCP/IP, Winsock, purge le cache ARP et renouvelle le bail IP pour résoudre les coupures.",
            Category = "RESEAU",
            Icon = "🔌",
            Action = (log, ct) => NetworkService.ResetAll(log, ct),
            Tags = new[] { "réseau", "winsock", "tcp", "ip", "reset", "connexion", "réparation" }
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

    private static void RegisterDiskTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "dsk_mgmt_gui",
            Name = "Gestionnaire de Disques (GUI)",
            Description = "Ouvre l'utilitaire graphique de gestion des disques et partitions (diskmgmt.msc).",
            Category = "DISQUE",
            Icon = "💽",
            Action = (log, ct) => DiskService.LaunchDiskManagement(log, ct),
            Tags = new[] { "disque", "diskmgmt", "partition", "volume", "formater" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "dsk_volumes",
            Name = "Espace et Volumes Disponibles",
            Description = "Affiche la liste détaillée de chaque lecteur, système de fichiers et espace libre/occupé.",
            Category = "DISQUE",
            Icon = "📊",
            Action = (log, ct) => DiskService.GetVolumesStatus(log, ct),
            Tags = new[] { "espace", "volume", "lecteur", "go", "taille", "disque" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "dsk_physical",
            Name = "Santé et Disques Physiques",
            Description = "Affiche les disques durs/SSD détectés, types de média et état de santé S.M.A.R.T.",
            Category = "DISQUE",
            Icon = "💾",
            Action = (log, ct) => DiskService.GetPhysicalDisks(log, ct),
            Tags = new[] { "ssd", "hdd", "smart", "santé", "physique", "disque" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "dsk_optimize_trim",
            Name = "Optimisation Disque & TRIM SSD",
            Description = "Exécute la commande defrag /O (TRIM pour les SSD, réorganisation pour les disques durs).",
            Category = "DISQUE",
            Icon = "⚡",
            Action = (log, ct) => DiskService.OptimizeDrives(log, ct),
            Tags = new[] { "trim", "defrag", "optimiser", "ssd", "vitesse" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "dsk_defrag_gui",
            Name = "Défragmenteur de Disque (GUI)",
            Description = "Ouvre l'interface Windows de défragmentation et d'optimisation des lecteurs (dfrgui.exe).",
            Category = "DISQUE",
            Icon = "🛠️",
            Action = (log, ct) => DiskService.LaunchDefragGui(log, ct),
            Tags = new[] { "dfrgui", "defrag", "interface", "optimisation" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "dsk_diskpart",
            Name = "Console de Formatage DISKPART",
            Description = "Lance l'interpréteur de commandes DISKPART en mode administrateur pour le formatage avancé.",
            Category = "DISQUE",
            Icon = "💻",
            Action = (log, ct) => DiskService.LaunchDiskpart(log, ct),
            Tags = new[] { "diskpart", "formater", "nettoyer", "partition", "cmd" }
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
            Description = "Lance la mise à niveau automatique de toutes les applications via winget (winget upgrade --all).",
            Category = "APPLICATIONS",
            Icon = "🚀",
            Action = (log, ct) => AppService.UpdateAll(log, ct),
            Tags = new[] { "upgrade", "all", "tout mettre a jour", "winget" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "app_list_installed",
            Name = "Lister les Logiciels Installés",
            Description = "Affiche la liste complète des applications installées sur le système (winget list).",
            Category = "APPLICATIONS",
            Icon = "📋",
            Action = (log, ct) => AppService.GetInstalledApps(log, ct),
            Tags = new[] { "liste", "applications", "winget", "logiciels" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "app_install_chrome",
            Name = "Installer Google Chrome",
            Description = "Installe la dernière version de Google Chrome via Winget.",
            Category = "APPLICATIONS",
            Icon = "🌐",
            Action = (log, ct) => AppService.InstallChrome(log, ct),
            Tags = new[] { "chrome", "navigateur", "google", "web" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "app_install_vlc",
            Name = "Installer VLC Media Player",
            Description = "Installe le lecteur multimédia open-source VLC.",
            Category = "APPLICATIONS",
            Icon = "🎬",
            Action = (log, ct) => AppService.InstallVlc(log, ct),
            Tags = new[] { "vlc", "video", "media", "musique" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "app_install_sumatra",
            Name = "Installer Sumatra PDF",
            Description = "Lecteur de fichiers PDF et e-books ultra-léger et rapide.",
            Category = "APPLICATIONS",
            Icon = "📄",
            Action = (log, ct) => AppService.InstallSumatraPdf(log, ct),
            Tags = new[] { "pdf", "sumatra", "document", "lecteur" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "app_install_7zip",
            Name = "Installer 7-Zip",
            Description = "Compresseur et décompresseur d'archives open-source, gratuit et standard (7z, zip, rar, tar).",
            Category = "APPLICATIONS",
            Icon = "🗜️",
            Action = (log, ct) => AppService.Install7Zip(log, ct),
            Tags = new[] { "7zip", "zip", "rar", "archive", "compression", "open source" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "app_install_ps7",
            Name = "Installer PowerShell 7",
            Description = "Installe la version moderne et multiplateforme de PowerShell.",
            Category = "APPLICATIONS",
            Icon = "⚡",
            Action = (log, ct) => AppService.InstallPs7(log, ct),
            Tags = new[] { "powershell", "ps7", "terminal", "script" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "app_install_office",
            Name = "Télécharger Microsoft Office 2024",
            Description = "Ouvre la page de téléchargement officielle de la suite Office.",
            Category = "APPLICATIONS",
            Icon = "💼",
            Action = (log, ct) => AppService.InstallOffice(log, ct),
            Tags = new[] { "office", "word", "excel", "microsoft" }
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

        _allTools.Add(new ScriptTool
        {
            Id = "rpt_battery",
            Name = "Rapport & Santé Batterie (HTML + Direct)",
            Description = "Affiche le niveau de charge et tension en direct, puis génère et ouvre l'historique complet d'usure de la batterie.",
            Category = "RAPPORTS",
            Icon = "🔋",
            Action = (log, ct) => DiagnosticService.GetFullBatteryReport(log, ct),
            Tags = new[] { "batterie", "powercfg", "batteryreport", "rapport", "autonomie", "live", "santé" }
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
            Description = "Exporte et sauvegarde tous les pilotes tiers vers C:\\DriversBackup via DISM /Export-Driver.",
            Category = "EXTRACTION",
            Icon = "💾",
            Action = (log, ct) => ExportService.ExportDrivers(log, ct),
            Tags = new[] { "pilotes", "drivers", "backup", "dism", "export" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "exp_software_list",
            Name = "Exporter Liste des Logiciels",
            Description = "Génère un fichier texte sur le Bureau avec la liste complète des applications installées.",
            Category = "EXTRACTION",
            Icon = "📋",
            Action = (log, ct) => ExportService.ExportSoftwareList(log, ct),
            Tags = new[] { "export", "logiciels", "liste", "applications", "bureau" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "exp_wifi_profiles",
            Name = "Exporter Profils & Clés Wi-Fi",
            Description = "Sauvegarde tous les profils et clés Wi-Fi sous forme de fichiers XML sur le Bureau.",
            Category = "EXTRACTION",
            Icon = "📶",
            Action = (log, ct) => ExportService.ExportWifiAndApps(log, ct),
            Tags = new[] { "wifi", "export", "mot de passe", "profils", "xml" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "exp_sensitive_audit",
            Name = "Audit Documents Sensibles",
            Description = "Recherche les clés SSH, tokens d'API et identifiants stockés dans les dossiers utilisateur.",
            Category = "EXTRACTION",
            Icon = "🛡️",
            Action = (log, ct) => ExportService.ScanSensitiveDocs(log, ct),
            Tags = new[] { "audit", "sécurité", "ssh", "tokens", "secrets" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "exp_browser_history",
            Name = "Détecter Historique Web",
            Description = "Localise et affiche l'historique et profils SQLite de Google Chrome et Microsoft Edge.",
            Category = "EXTRACTION",
            Icon = "🌐",
            Action = (log, ct) => ExportService.AnalyzeBrowserHistory(log, ct),
            Tags = new[] { "historique", "chrome", "edge", "navigateur", "profil" }
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
