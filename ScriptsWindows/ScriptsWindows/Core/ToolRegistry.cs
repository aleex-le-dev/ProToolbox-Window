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
        RegisterBootTools();             // Catégorie 12 : Démarrage & Récupération
        RegisterInnovationTools();       // Catégorie 13 : Nouveautés & Innovations
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
        ["nov_win11_ai_audit"] = "AlexToolkit.exe (Audit d'éligibilité Windows 11 24H2, TPM 2.0 et NPU)",

        // ─── Matériel (4) ───
        ["hw_winsat"] = "winsat.exe formal",
        ["hw_powercfg_energy"] = "powercfg.exe /energy /output energy-report.html",
        ["hw_power_perf"] = "AlexToolkit.exe (Interface de gestion des modes d'alimentation : Normal, Performances élevées, Performances ultimes)",
        ["nov_gaming_optimizer"] = "AlexToolkit.exe (Optimisation de latence, HAGS et Game Mode)",

        // ─── Réparation (7) ───
        ["repair_system_integrity"] = "AlexToolkit.exe (Centre d'intégrité Windows : DISM Scan/Restore et SFC)",
        ["repair_chkdsk"] = "chkdsk.exe (Interface interactive de réparation de disque NTFS)",
        ["repair_services_manager"] = "AlexToolkit.exe (Gestionnaire des services Windows : Spouleur, Windows Update...)",
        ["repair_spooler"] = "AlexToolkit.exe (Déblocage et purge des travaux d'impression)",
        ["repair_apps_shell"] = "AlexToolkit.exe (Réparation Store UWP, wsreset et cache d'icônes Explorer)",
        ["repair_restore_point"] = "AlexToolkit.exe (Points de restauration système Windows)",
        ["repair_export_drivers"] = "dism.exe /Online /Export-Driver /Destination:C:\\DriversBackup",

        // ─── Nettoyage (1) ───
        ["clean_system_unified"] = "AlexToolkit.exe (Interface interactive : Cleanmgr, temporaires, corbeille, caches et WinSxS)",

        // ─── Secrets et Clés (5) ───
        ["sec_wifi_show"] = "netsh.exe wlan show profiles key=clear",
        ["sec_pwd_generator"] = "AlexToolkit.exe (Générateur de mots de passe aléatoires et robustes)",
        ["sec_audit_sensitive"] = "AlexToolkit.exe (Audit C# approfondi .ssh, .aws, .env, documents secrets)",
        ["sec_win_key"] = "wmic.exe path softwarelicensingservice get OA3xOriginalProductKey",
        ["sec_browser_profiles"] = "AlexToolkit.exe (Lecteur SQLite natif Chrome, Edge, Brave, Firefox)",

        // ─── Réseau (7) ───
        ["net_center"] = "AlexToolkit.exe (Centre de contrôle réseau : diagnostic, DNS, scan et triage)",
        ["nov_dns_speed_test"] = "AlexToolkit.exe (Benchmark de latence et vitesse des serveurs DNS)",
        ["net_reset_all"] = "AlexToolkit.exe (Protocole de réinitialisation réseau en 7 étapes)",
        ["net_wifi_scan"] = "netsh.exe wlan show networks mode=bssid (Scanner visuel)",
        ["net_wifi_report"] = "netsh.exe wlan show wlanreport (Rapport graphique)",
        ["net_lan_scan"] = "AlexToolkit.exe (Scanner LAN des périphériques et machines locales)",
        ["net_flux_analysis"] = "netstat.exe (Moniteur des flux et connexions TCP actives)",

        // ─── Disque (4) ───
        ["disk_mgmt_gui"] = "diskmgmt.msc",
        ["disk_diskpart"] = "diskpart.exe",
        ["disk_defrag_gui"] = "dfrgui.exe",
        ["disk_bitlocker"] = "manage-bde.exe -status",

        // ─── Applications (1) ───
        ["app_installer_center"] = "AlexToolkit.exe (Installation d'applications unifiée : Chrome, VLC, 7-Zip, Sumatra PDF, PowerShell 7, Office)",

        // ─── Mises à jour (1) ───
        ["upd_winget_manager"] = "winget.exe upgrade (Gestionnaire de paquets interactif Winget)",

        // ─── Sécurité (4) ───
        ["sec_center"] = "AlexToolkit.exe (Centre de surveillance et sécurité unifié Defender, BSOD et RAM)",
        ["nov_privacy_shield"] = "AlexToolkit.exe (Bouclier de confidentialité et blocage télémétrie Windows)",
        ["sec_defender_quickscan"] = "powershell Start-MpScan -ScanType QuickScan",
        ["sec_perfmon_rel"] = "perfmon.exe /rel",

        // ─── Personnalisation (2) ───
        ["tweak_godmode"] = "explorer.exe \"shell:::{ED7BA470-8E54-465E-825C-99712043E01C}\"",
        ["tweak_classic_context"] = "AlexToolkit.exe (Bascule interactive : Menu classique Windows 10 / Moderne Windows 11)",

        // ─── Démarrage & Récupération (1) ───
        ["boot_center"] = "AlexToolkit.exe (Console unifiée de démarrage et récupération : sans échec, WinRE, BIOS/UEFI, BCD)"

        // ─── Nouveautés & Innovations (0 - Réservé aux futurs ajouts) ───
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
            t.Tags.Any(tag => tag.ToLowerInvariant().Contains(q)) ||
            t.Features.Any(feat => feat.ToLowerInvariant().Contains(q)));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 1 : 🔍 DIAGNOSTIC MATÉRIEL ET INTÉGRITÉ
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterDiagnosticTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "diag_center",
            Name = "Bilan de santé complet du PC",
            Description = "Vérifie d'un coup d'œil l'état de votre processeur, mémoire, disques durs, batterie et connexion pour s'assurer que tout fonctionne bien.",
            Features = new[]
            {
                "Processeur (CPU), mémoire vive (RAM) et charge",
                "Santé des disques durs & alertes S.M.A.R.T.",
                "Autonomie de la batterie & état du réseau"
            },
            Category = "DIAGNOSTIC",
            SubHeading = "Bilan général de l'ordinateur",
            Icon = "🔍",
            Action = (log, ct) => DiagnosticService.OpenDiagnosticCenterDialog(log, ct),
            Tags = new[] { "bilan", "santé", "diagnostic", "ordinateur", "processeur", "mémoire", "disques", "smart", "batterie", "réseau", "tout-en-un" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "nov_win11_ai_audit",
            Name = "Audit d'éligibilité Windows 11 24H2 et NPU IA",
            Description = "Vérifie les prérequis matériels avancés (POPCNT, SSE4.2, TPM 2.0, Secure Boot, RAM et présence d'un processeur neuronal NPU Copilot+).",
            Features = new[]
            {
                "Instructions CPU requises (POPCNT, SSE4.2)",
                "Sécurité matérielle : puce TPM 2.0 & Secure Boot",
                "Détection du processeur neuronal (NPU Copilot+)"
            },
            Category = "DIAGNOSTIC",
            SubHeading = "Bilan général de l'ordinateur",
            Icon = "🔍",
            Action = (log, ct) => InnovationService.OpenWin11AiAuditDialog(log, ct),
            Tags = new[] { "windows 11", "24h2", "tpm", "secure boot", "popcnt", "ia", "npu", "copilot", "éligibilité", "audit" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "diag_dxdiag",
            Name = "Vérifier l'écran, les jeux et le son (DxDiag)",
            Description = "Teste le bon fonctionnement de votre carte graphique, de votre affichage et de vos haut-parleurs en cas de problème vidéo ou audio.",
            Features = new[]
            {
                "Diagnostic de la carte graphique (DirectX / GPU)",
                "Vérification des écrans, pilotes et rafraîchissement",
                "Test des périphériques audio et haut-parleurs"
            },
            Category = "DIAGNOSTIC",
            SubHeading = "Vérifications rapides",
            Icon = "🎮",
            Action = (log, ct) => HardwareService.LaunchDxdiag(log, ct),
            Tags = new[] { "écran", "affichage", "jeux", "carte graphique", "gpu", "son", "audio", "dxdiag" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "diag_startup_manager",
            Name = "Accélérer le démarrage de Windows",
            Description = "Affiche la liste des logiciels qui se lancent au démarrage et vous permet de désactiver ceux qui ralentissent inutilement votre PC.",
            Features = new[]
            {
                "Inventaire des applications lancées au boot",
                "Mesure de l'impact sur le temps de démarrage",
                "Activation / désactivation en 1 clic sans risque"
            },
            Category = "DIAGNOSTIC",
            SubHeading = "Vérifications rapides",
            Icon = "🚀",
            Action = (log, ct) => DiagnosticService.OpenStartupManagerDialog(log, ct),
            Tags = new[] { "accélérer", "démarrage", "ralentissement", "programmes", "lancement", "vitesse", "boot", "optimiser" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 2 : 🖥️ PERFORMANCES ET ÉNERGIE
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterHardwareTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "hw_power_perf",
            Name = "Changer la vitesse du PC (Normal / Rapide)",
            Description = "Bascule facile : économisez de l'énergie en mode normal ou libérez toute la puissance du processeur pour les jeux et logiciels lourds.",
            Features = new[]
            {
                "Bascule instantanée : Économie ou Performance",
                "Optimisation de la fréquence d'horloge du CPU",
                "Idéal pour prolonger la batterie ou booster les jeux"
            },
            Category = "MATERIEL",
            SubHeading = "Gestion de l'alimentation et vitesse",
            Icon = "⚡",
            Action = (log, ct) => HardwareService.OpenPowerPlanDialog(log, ct),
            Tags = new[] { "vitesse", "puissance", "mode", "alimentation", "performances", "normal", "équilibré", "rapide", "batterie", "jeux" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "nov_gaming_optimizer",
            Name = "Optimiseur de latence et Mode Gaming Ultime",
            Description = "Configure la planification GPU matérielle accélérée (HAGS), désactive l'algorithme de Nagle pour réduire le ping et alloue 100% du CPU aux jeux.",
            Features = new[]
            {
                "Planification GPU matérielle accélérée (HAGS)",
                "Désactivation algorithme de Nagle (réduit le ping)",
                "Priorité CPU maximale allouée aux jeux vidéo"
            },
            Category = "MATERIEL",
            SubHeading = "Gestion de l'alimentation et vitesse",
            Icon = "⚡",
            Action = (log, ct) => InnovationService.OpenGamingOptimizerDialog(log, ct),
            Tags = new[] { "gaming", "jeu", "latence", "ping", "nagle", "fps", "hags", "boost", "vitesse", "réactivité" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "hw_winsat",
            Name = "Tester la puissance globale de mon PC",
            Description = "Calcule une note de performance pour mesurer la rapidité réelle de votre processeur, mémoire, carte graphique et disque dur.",
            Features = new[]
            {
                "Indice de performance officiel Windows (WinSAT)",
                "Scores détaillés : Processeur, RAM, 3D et Disque",
                "Rapport chiffré complet sur la vélocité matérielle"
            },
            Category = "MATERIEL",
            SubHeading = "Banc d'essai et performances",
            Icon = "⚙️",
            Action = (log, ct) => HardwareService.RunWinSat(log, ct),
            Tags = new[] { "tester", "note", "puissance", "vitesse", "benchmark", "score", "processeur", "graphique" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "hw_powercfg_energy",
            Name = "Détecter ce qui vide la batterie",
            Description = "Analyse pendant 1 minute comment votre ordinateur utilise l'énergie et liste les réglages ou applications qui consomment trop.",
            Features = new[]
            {
                "Audit d'efficacité énergétique pendant 60 secondes",
                "Identification des applications énergivores en veille",
                "Génération d'un rapport d'autonomie HTML complet"
            },
            Category = "MATERIEL",
            SubHeading = "Banc d'essai et performances",
            Icon = "🔋",
            Action = (log, ct) => HardwareService.RunPowercfgEnergy(log, ct),
            Tags = new[] { "batterie", "autonomie", "énergie", "consommation", "économiser", "rapport", "portable" }
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
            Name = "Réparer les fichiers système endommagés (SFC & DISM)",
            Description = "Scanne les fichiers internes de Windows et remplace automatiquement ceux qui sont cassés pour stopper les bugs, ralentissements et plantages.",
            Features = new[]
            {
                "Scan approfondi de tous les fichiers protégés (SFC)",
                "Restauration de l'image Windows Update (DISM)",
                "Résolution automatique des corruptions et bugs"
            },
            Category = "REPARATION",
            SubHeading = "Réparation du système et des disques",
            Icon = "🩺",
            Action = (log, ct) => RepairService.OpenSystemIntegrityDialog(log, ct),
            Tags = new[] { "réparer", "fichiers", "système", "corruption", "sfc", "dism", "scannow", "bugs", "plantage", "restaurer", "image" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "repair_chkdsk",
            Name = "Réparer les erreurs de disque dur (CHKDSK)",
            Description = "Vérifie la santé de votre disque dur ou SSD, détecte les erreurs d'écriture et répare les fichiers qui refusent de s'ouvrir.",
            Features = new[]
            {
                "Analyse des métadonnées système de fichiers NTFS",
                "Détection et isolation des secteurs défectueux",
                "Récupération automatique des dossiers illisibles"
            },
            Category = "REPARATION",
            SubHeading = "Réparation du système et des disques",
            Icon = "🔍",
            Action = (log, ct) => RepairService.OpenChkdskDialog(log, ct),
            Tags = new[] { "disque", "réparer", "chkdsk", "erreurs", "secteurs", "bloqué", "ntfs", "dossiers", "intégrité" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "repair_spooler",
            Name = "Débloquer l'imprimante et relancer les impressions",
            Description = "Votre impression est coincée ? Cette action vide la file d'attente bloquée, redémarre le service d'impression et débloque votre imprimante en 1 clic.",
            Features = new[]
            {
                "Purge immédiate des documents bloqués dans la file",
                "Arrêt, nettoyage du cache et relance du spouleur",
                "Résout les imprimantes bloquées en état 'Erreur'"
            },
            Category = "REPARATION",
            SubHeading = "Dépannage de l'imprimante et des services",
            Icon = "🖨️",
            Action = (log, ct) => RepairService.FixPrintSpooler(log, ct),
            Tags = new[] { "imprimante", "impression", "bloquée", "coincée", "file", "spooler", "spouleur", "vider", "nettoyer", "relancer" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "repair_services_manager",
            Name = "Gérer et redémarrer les services Windows",
            Description = "Pilotez facilement les programmes d'arrière-plan de Windows : redémarrez Windows Update, le son ou arrêtez les tâches bloquées.",
            Features = new[]
            {
                "Supervision des services système d'arrière-plan",
                "Redémarrage en 1 clic de Windows Update, Audio, etc.",
                "Arrêt immédiat des services figés ou en surcharge"
            },
            Category = "REPARATION",
            SubHeading = "Dépannage de l'imprimante et des services",
            Icon = "⚙️",
            Action = (log, ct) => RepairService.OpenServicesManagerDialog(log, ct),
            Tags = new[] { "services", "arrière-plan", "redémarrer", "windows update", "spooler", "bloqué", "arrêter", "processus" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "repair_apps_shell",
            Name = "Réparer le Microsoft Store et les icônes blanches",
            Description = "Répare la boutique Microsoft Store si elle refuse de s'ouvrir et reconstruit les images de vos raccourcis si vos icônes s'affichent en blanc.",
            Features = new[]
            {
                "Réinitialisation complète du Microsoft Store (wsreset)",
                "Purge et reconstruction du cache IconCache.db",
                "Restauration de l'affichage normal dans l'Explorateur"
            },
            Category = "REPARATION",
            SubHeading = "Boutique d'applications et affichage",
            Icon = "🏬",
            Action = (log, ct) => RepairService.OpenAppsShellRepairDialog(log, ct),
            Tags = new[] { "store", "boutique", "applications", "icônes", "blanches", "miniatures", "wsreset", "explorer", "affichage" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "repair_restore_point",
            Name = "Créer ou restaurer un point de retour",
            Description = "Créez une sauvegarde de secours pour pouvoir revenir en arrière à tout moment en cas de bug ou de mauvaise manipulation.",
            Features = new[]
            {
                "Création d'un point de sauvegarde immédiat",
                "Restauration facile en cas de fausse manipulation",
                "Accès direct à l'assistant officiel Windows (rstrui)"
            },
            Category = "REPARATION",
            SubHeading = "Sauvegardes et points de retour",
            Icon = "🔄",
            Action = (log, ct) => RepairService.OpenRestorePointDialog(log, ct),
            Tags = new[] { "sauvegarde", "restauration", "point de retour", "sécurité", "revenir", "annuler", "secours", "rstrui" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "repair_export_drivers",
            Name = "Sauvegarder les pilotes de mes équipements",
            Description = "Copie tous les pilotes de vos composants (carte graphique, Wi-Fi, son) dans un dossier pour pouvoir les réinstaller facilement.",
            Features = new[]
            {
                "Exportation des pilotes tiers vers C:\\DriversBackup",
                "Sauvegarde Carte graphique, Wi-Fi, Audio, Chipset",
                "Réinstallation express sans connexion Internet"
            },
            Category = "REPARATION",
            SubHeading = "Sauvegardes et points de retour",
            Icon = "💾",
            Action = (log, ct) => ExportService.ExportDrivers(log, ct),
            Tags = new[] { "pilotes", "drivers", "sauvegarder", "dossier", "export", "backup", "carte graphique", "wifi" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 4 : 🧹 NETTOYAGE ET ESPACE DISQUE
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterCleaningTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "clean_system_unified",
            Name = "Grand nettoyage du PC et des fichiers inutiles",
            Description = "Supprime les fichiers temporaires, vide la corbeille, nettoie les traces de navigation et libère plusieurs gigaoctets d'espace disque en toute sécurité.",
            Features = new[]
            {
                "Fichiers temporaires utilisateur, système et caches",
                "Vidage automatique de la Corbeille de tous les disques",
                "Nettoyage du magasin de composants Windows WinSxS"
            },
            Category = "NETTOYAGE",
            SubHeading = "Nettoyage système et disque",
            Icon = "🧹",
            Action = (log, ct) => CleaningService.OpenSystemCleanupDialog(log, ct),
            Tags = new[] { "nettoyer", "libérer", "espace", "disque", "temporaires", "corbeille", "cache", "navigateurs", "cleanmgr", "accélérer" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 5 : 🔑 MOTS DE PASSE ET CLÉS DE LICENCE
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterSecretsTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "sec_wifi_show",
            Name = "Retrouver mes mots de passe Wi-Fi enregistrés",
            Description = "Affiche immédiatement en clair le mot de passe de votre box Wi-Fi ainsi que tous les réseaux auxquels votre PC s'est déjà connecté.",
            Features = new[]
            {
                "Scan de tous les profils Wi-Fi mémorisés sur le PC",
                "Affichage instantané des clés de sécurité en clair",
                "Copie en 1 clic du mot de passe dans le presse-papiers"
            },
            Category = "SECRETS",
            SubHeading = "Mots de passe et Wi-Fi",
            Icon = "📶",
            Action = (log, ct) => SecurityService.DumpWifiPasswords(log, ct),
            Tags = new[] { "wifi", "mot de passe", "box", "clé", "clair", "retrouver", "code", "sans fil" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sec_pwd_generator",
            Name = "Générateur de mots de passe ultra-sécurisés",
            Description = "Crée en 1 clic des mots de passe incassables et aléatoires pour vos comptes en ligne et vous permet de les enregistrer en toute sécurité.",
            Features = new[]
            {
                "Création de clés cryptographiques à haute entropie",
                "Choix des longueurs et types de caractères",
                "Test visuel de robustesse et copie sécurisée"
            },
            Category = "SECRETS",
            SubHeading = "Mots de passe et Wi-Fi",
            Icon = "🔐",
            Action = (log, ct) => SecurityService.GenerateSecurePasswords(log, ct),
            Tags = new[] { "mot de passe", "générer", "créer", "sécurisé", "fort", "aléatoire", "comptes", "mdp" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sec_browser_profiles",
            Name = "Consulter l'historique de mes navigateurs web",
            Description = "Affiche les 35 dernières pages visitées sur vos navigateurs (Google Chrome, Edge, Firefox, Brave) sans même avoir besoin de les ouvrir.",
            Features = new[]
            {
                "Support de Chrome, Edge, Brave et Firefox",
                "Lecture directe des bases SQLite sans ouvrir les applis",
                "Affichage des 35 dernières pages et horodatages"
            },
            Category = "SECRETS",
            SubHeading = "Mots de passe et Wi-Fi",
            Icon = "🌐",
            Action = (log, ct) => ExportService.AnalyzeBrowserHistory(log, ct),
            Tags = new[] { "historique", "sites", "web", "pages", "chrome", "edge", "firefox", "brave", "navigation" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sec_win_key",
            Name = "Retrouver ma clé d'activation Windows originale",
            Description = "Lit la clé de licence officielle gravée dans la carte mère de votre PC, indispensable en cas de réinstallation complète de Windows.",
            Features = new[]
            {
                "Lecture de la clé OEM gravée dans le BIOS / UEFI",
                "Indispensable pour réactiver lors d'une réinstallation",
                "Copie immédiate de la licence officielle en 25 caractères"
            },
            Category = "SECRETS",
            SubHeading = "Licences et documents confidentiels",
            Icon = "🔑",
            Action = (log, ct) => ExportService.GetWindowsKey(log, ct),
            Tags = new[] { "clé", "licence", "windows", "activation", "produit", "carte mère", "oem", "bios", "uefi" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sec_audit_sensitive",
            Name = "Rechercher mes documents et codes confidentiels",
            Description = "Recherche dans vos dossiers personnels les fichiers contenant des mots de passe oubliés, des identifiants cloud ou des clés de sécurité.",
            Features = new[]
            {
                "Recherche de clés SSH, tokens AWS, fichiers .env",
                "Scan ciblé des dossiers personnels et profils",
                "Rapport préventif pour protéger vos données sensibles"
            },
            Category = "SECRETS",
            SubHeading = "Licences et documents confidentiels",
            Icon = "🔍",
            Action = (log, ct) => ExportService.ScanSensitiveDocs(log, ct),
            Tags = new[] { "recherche", "documents", "secrets", "confidentiels", "mots de passe", "ssh", "tokens", "sécurité" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 6 : 🌐 INTERNET ET RÉSEAU
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterNetworkTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "net_center",
            Name = "Centre de contrôle Internet & Réseau",
            Description = "Vérifiez votre connexion, testez la rapidité d'accès et changez de serveur DNS (Cloudflare, Google) pour accélérer le chargement des sites web.",
            Features = new[]
            {
                "Test de connectivité passerelle, Internet et DNS",
                "Changement rapide de DNS (Cloudflare, Google, Quad9)",
                "Affichage IP publique, locale et statut de carte"
            },
            Category = "RESEAU",
            SubHeading = "Dépannage et amélioration de la connexion",
            Icon = "🌐",
            Action = (log, ct) => NetworkService.OpenNetworkCenterDialog(log, ct),
            Tags = new[] { "internet", "connexion", "réseau", "dns", "accélérer", "sites", "cloudflare", "google", "triage", "diagnostic" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "nov_dns_speed_test",
            Name = "Benchmark de vitesse DNS et test de latence",
            Description = "Compare en direct les performances et le ping de Cloudflare, Google, Quad9 et de votre FAI pour vous recommander le DNS le plus rapide.",
            Features = new[]
            {
                "Mesure du ping réel en millisecondes sur 10 requêtes",
                "Comparatif Cloudflare (1.1.1.1), Google (8.8.8.8) et FAI",
                "Recommandation automatique du serveur le plus réactif"
            },
            Category = "RESEAU",
            SubHeading = "Dépannage et amélioration de la connexion",
            Icon = "🌐",
            Action = (log, ct) => InnovationService.OpenDnsBenchmarkDialog(log, ct),
            Tags = new[] { "dns", "benchmark", "vitesse", "latence", "ping", "cloudflare", "google", "quad9", "internet", "connexion" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_reset_all",
            Name = "Réparer la connexion Internet (Grand Reset)",
            Description = "Plus d'Internet ou connexion bloquée ? Remet à neuf l'ensemble des réglages réseau de Windows en 7 étapes automatiques.",
            Features = new[]
            {
                "Purge du cache DNS et vidage de la table ARP",
                "Réinitialisation complète pile TCP/IP et Winsock",
                "Libération et renouvellement immédiat de l'adresse IP"
            },
            Category = "RESEAU",
            SubHeading = "Dépannage et amélioration de la connexion",
            Icon = "⚡",
            Action = (log, ct) => NetworkService.ResetAll(log, ct),
            Tags = new[] { "panne", "internet", "réparer", "coupure", "reset", "réinitialiser", "bloqué", "dhcp", "wifi", "ethernet" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_wifi_scan",
            Name = "Détecteur des réseaux Wi-Fi voisins",
            Description = "Affiche les box et bornes Wi-Fi autour de chez vous avec des jauges visuelles colorées pour trouver le meilleur canal sans interférences.",
            Features = new[]
            {
                "Affichage box voisines et canaux (2.4 / 5 GHz)",
                "Jauges visuelles colorées de puissance de signal (dBm)",
                "Détection des interférences pour choisir le meilleur canal"
            },
            Category = "RESEAU",
            SubHeading = "Diagnostic et analyse du Wi-Fi",
            Icon = "📶",
            Action = (log, ct) => NetworkService.OpenWifiScannerDialog(log, ct),
            Tags = new[] { "wifi", "box", "voisins", "puissance", "signal", "jauges", "canaux", "scan", "sans fil" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_wifi_report",
            Name = "Bilan des déconnexions Wi-Fi (Graphique)",
            Description = "Affiche un graphique visuel des déconnexions et de la stabilité de votre connexion sans fil au cours des 3 derniers jours.",
            Features = new[]
            {
                "Génération du rapport officiel Windows wlanreport",
                "Historique visuel des pertes de signal sur 3 jours",
                "Diagnostic des erreurs de pilote ou de veille"
            },
            Category = "RESEAU",
            SubHeading = "Diagnostic et analyse du Wi-Fi",
            Icon = "📊",
            Action = (log, ct) => DiagnosticService.GetWifiReport(log, ct),
            Tags = new[] { "wifi", "déconnexions", "coupures", "santé", "stabilité", "graphique", "historique", "visuel", "rapport" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_lan_scan",
            Name = "Voir les appareils connectés sur ma Box",
            Description = "Découvre en direct les smartphones, ordinateurs, télés connectées et imprimantes branchés sur votre réseau avec leur adresse IP.",
            Features = new[]
            {
                "Scan rapide du sous-réseau local (192.168.x.x)",
                "Découverte des PC, smartphones, imprimantes et TV",
                "Affichage des adresses IP et adresses MAC détectées"
            },
            Category = "RESEAU",
            SubHeading = "Appareils branchés et connexions actives",
            Icon = "🖥️",
            Action = (log, ct) => NetworkService.OpenLanScannerDialog(log, ct),
            Tags = new[] { "box", "appareils", "qui est connecté", "téléphones", "ip", "réseau", "lan", "découverte", "routeur" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "net_flux_analysis",
            Name = "Voir les applications connectées à Internet",
            Description = "Liste filtrable en temps réel des logiciels qui communiquent avec l'extérieur, pratique pour débusquer les programmes suspects.",
            Features = new[]
            {
                "Moniteur en temps réel des flux TCP établis",
                "Identification du nom de processus, port et IP distante",
                "Détection facile des logiciels suspects ou espions"
            },
            Category = "RESEAU",
            SubHeading = "Appareils branchés et connexions actives",
            Icon = "🔍",
            Action = (log, ct) => NetworkService.OpenTcpConnectionsDialog(log, ct),
            Tags = new[] { "applications", "connexions", "internet", "flux", "espion", "ports", "processus", "surveiller", "tcp" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 7 : 💽 DISQUES ET STOCKAGE
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterDiskTools()
    {
        // Sous-titre interne : Organisation et gestion des disques
        _allTools.Add(new ScriptTool
        {
            Id = "disk_mgmt_gui",
            Name = "Gestionnaire de disques et clés USB",
            Description = "Outil visuel officiel de Windows pour voir vos disques durs, créer des espaces de stockage ou formater vos clés USB.",
            Features = new[]
            {
                "Visualisation graphique des volumes et partitions",
                "Création, réduction ou extension de volumes",
                "Attribution et modification des lettres de lecteurs"
            },
            Category = "DISQUE",
            SubHeading = "Organisation et gestion des disques",
            Icon = "💽",
            Action = (log, ct) => DiskService.LaunchDiskManagement(log, ct),
            Tags = new[] { "disques", "stockage", "clés usb", "partitions", "espaces", "formater", "gestionnaire", "diskmgmt" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "disk_diskpart",
            Name = "Nettoyage avancé de disque (Diskpart)",
            Description = "Invite de commandes avancée pour formater, effacer ou réparer un disque dur ou une clé USB récalcitrante.",
            Features = new[]
            {
                "Console système pour disques ou clés USB bloqués",
                "Nettoyage complet (clean), conversion GPT / MBR",
                "Déblocage des partitions protégées en écriture"
            },
            Category = "DISQUE",
            SubHeading = "Organisation et gestion des disques",
            Icon = "💻",
            Action = (log, ct) => DiskService.LaunchDiskpart(log, ct),
            Tags = new[] { "diskpart", "formater", "effacer", "avancé", "partitions", "clé usb", "cmd", "réparer" }
        });

        // Sous-titre interne : Santé et sécurité de vos fichiers
        _allTools.Add(new ScriptTool
        {
            Id = "disk_defrag_gui",
            Name = "Optimiser et accélérer les disques",
            Description = "Donne un coup de boost à vos disques : accélère les disques durs classiques et rafraîchit la mémoire des SSD récents.",
            Features = new[]
            {
                "Défragmentation ciblée des disques magnétiques",
                "Envoi de la commande TRIM pour rafraîchir les SSD",
                "Analyse et planification de l'état de fragmentation"
            },
            Category = "DISQUE",
            SubHeading = "Santé et sécurité de vos fichiers",
            Icon = "⚡",
            Action = (log, ct) => DiskService.LaunchDefragGui(log, ct),
            Tags = new[] { "optimiser", "accélérer", "défragmenter", "ssd", "trim", "disque dur", "vitesse", "boost" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "disk_bitlocker",
            Name = "Protéger et verrouiller un disque (BitLocker)",
            Description = "Verrouille l'accès à vos disques avec un mot de passe sécurisé pour que personne d'autre ne puisse lire vos données personnelles.",
            Features = new[]
            {
                "Statut de chiffrement en direct de chaque lecteur",
                "Verrouillage par mot de passe ou puce TPM",
                "Sauvegarde de la clé de récupération de secours"
            },
            Category = "DISQUE",
            SubHeading = "Santé et sécurité de vos fichiers",
            Icon = "🔒",
            Action = (log, ct) => DiskService.ManageBitLocker(log, ct),
            Tags = new[] { "verrouiller", "protéger", "bitlocker", "mot de passe", "chiffrement", "sécurité", "dossiers", "intrus" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 8 : 📦 INSTALLER VOS LOGICIELS
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterAppInstallationTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "app_installer_center",
            Name = "Installer vos applications indispensables",
            Description = "Installez ou mettez à jour facilement vos logiciels du quotidien (Google Chrome, VLC, 7-Zip, Sumatra PDF, PowerShell, Office) en cochant simplement ceux que vous voulez.",
            Features = new[]
            {
                "Navigateurs modernes (Chrome, Firefox, Brave)",
                "Utilitaires essentiels (VLC, 7-Zip, Sumatra PDF)",
                "Outils bureautiques et pro (PowerShell 7, VS Code)"
            },
            Category = "APPLICATIONS",
            SubHeading = "Installation et déploiement rapide",
            Icon = "📦",
            Action = (log, ct) => AppService.OpenAppInstallerDialog(log, ct),
            Tags = new[] { "installer", "applications", "logiciels", "programmes", "chrome", "vlc", "7zip", "office", "sumatra", "facile", "1-clic" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 9 : 🔄 MISES À JOUR DES PROGRAMMES
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterUpdatesTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "upd_winget_manager",
            Name = "Mettre à jour tous vos programmes",
            Description = "Recherche automatiquement les nouvelles versions de vos applications installées et met tout à jour en un seul clic.",
            Features = new[]
            {
                "Détection des mises à jour disponibles (Winget)",
                "Mise à jour sélective ou globale en 1 clic",
                "Téléchargement et installation silencieuse"
            },
            Category = "MISES_A_JOUR",
            SubHeading = "Gestionnaire de mises à jour",
            Icon = "🔄",
            Action = (log, ct) => AppService.OpenWingetManagerDialog(log, ct),
            Tags = new[] { "mises à jour", "programmes", "logiciels", "nouvelle version", "upgrade", "sécurité", "tout mettre à jour" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 10 : 🛡️ SÉCURITÉ ET ANTIVIRUS
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterSecurityTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "sec_center",
            Name = "Centre de protection antivirus et surveillance",
            Description = "Tableau de bord : vérifie si l'antivirus Windows est actif, surveille les plantages récents et identifie les logiciels qui saturent votre mémoire.",
            Features = new[]
            {
                "Statut de l'antivirus et du pare-feu Defender",
                "Surveillance des derniers plantages système (BSOD)",
                "Analyse des applications qui saturent la RAM"
            },
            Category = "SECURITE",
            SubHeading = "Protection globale de votre ordinateur",
            Icon = "🛡️",
            Action = (log, ct) => SecurityService.OpenSecurityCenterDialog(log, ct),
            Tags = new[] { "sécurité", "antivirus", "protection", "defender", "plantages", "ralentissement", "mémoire", "surveillance" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "nov_privacy_shield",
            Name = "Bouclier de confidentialité et anti-télémétrie",
            Description = "Désactive la télémétrie agressive de Microsoft, l'identifiant publicitaire ciblé, le suivi des frappes clavier et les suggestions de pub dans Démarrer.",
            Features = new[]
            {
                "Désactivation télémétrie et traçage Windows",
                "Blocage de l'identifiant pub et des suggestions",
                "Protection des frappes clavier et diagnostics"
            },
            Category = "SECURITE",
            SubHeading = "Confidentialité et vie privée",
            Icon = "🛡️",
            Action = (log, ct) => InnovationService.OpenPrivacyShieldDialog(log, ct),
            Tags = new[] { "confidentialité", "vie privée", "télémétrie", "diagtrack", "mouchard", "publicité", "sécurité", "tracking", "espion" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sec_defender_quickscan",
            Name = "Analyse antivirus rapide en 1 clic",
            Description = "Lance une vérification immédiate des fichiers sensibles et de la mémoire par Microsoft Defender pour s'assurer qu'aucun virus n'est actif.",
            Features = new[]
            {
                "Scan express des fichiers système et de la RAM",
                "Détection des malwares actifs en tâche de fond",
                "Exécution non-bloquante avec rapport immédiat"
            },
            Category = "SECURITE",
            SubHeading = "Vérifications express",
            Icon = "⚡",
            Action = (log, ct) => SecurityService.DefenderQuickScan(log, ct),
            Tags = new[] { "antivirus", "scan", "rapide", "analyse", "virus", "menaces", "defender", "sécurité" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "sec_perfmon_rel",
            Name = "Historique des plantages et bugs du PC",
            Description = "Affiche une frise chronologique officielle Windows avec une note de stabilité montrant les jours où des logiciels ou pilotes ont planté.",
            Features = new[]
            {
                "Frise chronologique de stabilité Windows (note 1-10)",
                "Détail des plantages matériels, pilotes ou logiciels",
                "Idéal pour repérer l'origine d'un bug récurrent"
            },
            Category = "SECURITE",
            SubHeading = "Vérifications express",
            Icon = "📉",
            Action = (log, ct) => DiagnosticService.LaunchReliabilityMonitor(log, ct),
            Tags = new[] { "plantages", "bugs", "historique", "stabilité", "fiabilité", "crashs", "problèmes", "perfmon" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 11 : 🎨 PERSONNALISATION DE WINDOWS
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterPersonalizationTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "tweak_godmode",
            Name = "Activer le dossier Tous les Réglages (God Mode)",
            Description = "Crée sur votre Bureau un dossier pratique regroupant plus de 200 raccourcis directs vers tous les réglages et options de Windows.",
            Features = new[]
            {
                "Création directe du dossier spécial sur le Bureau",
                "Plus de 200 réglages Windows classés par thème",
                "Accès rapide sans fouiller dans les paramètres"
            },
            Category = "PERSONNALISATION",
            SubHeading = "Options et réglages système",
            Icon = "👑",
            Action = (log, ct) => PersonalizationService.CreateGodMode(log, ct),
            Tags = new[] { "god mode", "réglages", "options", "panneau de configuration", "raccourci", "pratique", "bureau" }
        });

        _allTools.Add(new ScriptTool
        {
            Id = "tweak_classic_context",
            Name = "Retrouver le clic droit classique (Windows 10 / 11)",
            Description = "Bascule en 1 clic : réactivez le menu complet d'un clic droit comme sur Windows 10, ou revenez au menu moderne de Windows 11.",
            Features = new[]
            {
                "Bascule en 1 clic sans redémarrer l'ordinateur",
                "Restaure le menu complet classique Windows 10",
                "Ou rétablit le menu moderne de Windows 11"
            },
            Category = "PERSONNALISATION",
            SubHeading = "Options et réglages système",
            Icon = "🖱️",
            Action = (log, ct) => PersonalizationService.OpenContextMenuDialog(log, ct),
            Tags = new[] { "clic droit", "menu contextuel", "windows 10", "windows 11", "classique", "facile", "personnaliser" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 12 : ⚡ DÉMARRAGE & RÉCUPÉRATION
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterBootTools()
    {
        _allTools.Add(new ScriptTool
        {
            Id = "boot_center",
            Name = "Centre de démarrage et récupération",
            Description = "Interface unifiée : redémarrez en mode sans échec (minimal ou réseau), accédez au WinRE, basculez vers le BIOS/UEFI ou réparez le menu de démarrage cassé (BCD).",
            Features = new[]
            {
                "Redémarrage direct en Mode sans échec (Safe Mode)",
                "Accès en 1 clic au dépannage Windows (WinRE)",
                "Bascule immédiate vers le BIOS/UEFI et BCD"
            },
            Category = "DEMARRAGE",
            SubHeading = "Console interactive de démarrage",
            Icon = "⚡",
            Action = (log, ct) => BootService.OpenBootRecoveryDialog(log, ct),
            Tags = new[] { "démarrage", "récupération", "sans échec", "winre", "bios", "uefi", "bcd", "boot", "réparation", "redémarrage" }
        });
    }

    // ─────────────────────────────────────────────────────────────────────────
    // CATÉGORIE 13 : ✨ NOUVEAUTÉS & INNOVATIONS (RÉSERVÉE AUX PROCHAINS SCRIPTS)
    // ─────────────────────────────────────────────────────────────────────────
    private static void RegisterInnovationTools()
    {
        // Les 4 outils initiaux ont été intégrés dans leurs catégories respectives :
        // - Bouclier de confidentialité -> SECURITE
        // - Optimiseur Gaming -> MATERIEL
        // - Audit Win11 24H2 & IA -> DIAGNOSTIC
        // - Benchmark de vitesse DNS -> RESEAU
        // Cette catégorie reste active et prête à accueillir vos prochains scripts.
    }

}


