# Alex Toolkit
by AleexLeDev – diagnostiquez, réparez, optimisez et sécurisez Windows 10 & 11 depuis une seule application moderne : bilan de santé matériel, réparation système en un clic, grand nettoyage, bouclier anti-télémétrie, optimisation gaming, outils réseau et console de démarrage avancée. Aucun outil tiers à installer, aucun compte, aucun traceur.

⚡ [Télécharger la dernière version (v4.x)](https://github.com/aleex-le-dev/SCRIPT-WINDOWS-ALEEXLEDEV/releases/latest) – un seul fichier `AlexToolkit_vX.Y.Z.exe` : double-cliquez et utilisez.  
📖 Documentation : [Guide d'utilisation](docs/manual.md) · [Suivi technique & refonte](SUIVI_REFONTE.md) · [Instructions de développement](CLAUDE.md)  
💬 Communauté : [GitHub Discussions](https://github.com/aleex-le-dev/SCRIPT-WINDOWS-ALEEXLEDEV/discussions) – retours d'expérience, questions, propositions de nouveaux outils.  
🗺️ Et ensuite ? [Roadmap](ROADMAP.md) · 📜 [Journal des modifications](CHANGELOG.md)

---

## Ce qu'il fait

* **Intervention technicien & rapport client** – checkup en un clic (point de restauration, nettoyage, antivirus, DISM/SFC, mises à jour Winget) puis rapport HTML imprimable : machine, état avant / après, opérations avec résultat et durée, conclusion, journal et signatures.
* **Bilan de santé & diagnostic** – tableau de bord complet (système, carte mère, BIOS, processeur, barrettes RAM, disques avec usure / température / heures / erreurs S.M.A.R.T., volumes, batterie, cartes réseau), audit complet dans la console, audit d'éligibilité Windows 11 24H2 / NPU Copilot+, gestion des programmes au démarrage et DxDiag.
* **Pannes & pilotes** – écrans bleus des 90 derniers jours avec code d'arrêt traduit et cause probable, arrêts brutaux, périphériques en erreur (codes 10, 28, 43…) expliqués et réinstallables en un clic.
* **Réparation système** – SFC & DISM avec suivi en direct, état et réinitialisation complète de Windows Update, CHKDSK interactif, déblocage du spouleur d'impression, gestion des services Windows, réparation du Microsoft Store et des icônes blanches, points de restauration et sauvegarde de tous les pilotes.
* **Démarrage & récupération** – console unifiée : mode sans échec (minimal ou réseau), redémarrage direct dans le BIOS/UEFI, accès à WinRE et reconstruction du magasin de démarrage BCD.
* **Grand nettoyage** – mesure puis purge des fichiers temporaires, du cache Windows Update, de la corbeille, des caches navigateurs (Chrome, Edge, Brave, Firefox) et des miniatures, vidage DNS/presse-papier et nettoyage optionnel du magasin WinSxS. Un script de nettoyage peut être déposé sur le Bureau.
* **Sécurité & confidentialité** – état de Microsoft Defender et analyse rapide, historique des plantages, processus les plus gourmands, bouclier anti-télémétrie réversible (télémétrie, identifiant publicitaire, suggestions du menu Démarrer).
* **Désinfection** – détection et correction (avec sauvegarde) des détournements : fichier hosts, proxy WinINET / WinHTTP, tâches planifiées et démarrages masqués ou encodés ; analyses Defender complète et hors ligne, installation d'AdwCleaner et Malwarebytes.
* **Comptes utilisateurs** – réinitialisation ou suppression de mot de passe, droits administrateur, activation, création et suppression de comptes locaux (fonctionne quelle que soit la langue de Windows).
* **Mots de passe & licences** – mots de passe Wi-Fi enregistrés, clé de licence Windows d'origine (BIOS), historique récent des navigateurs, recherche de fichiers sensibles (`.ssh`, `.aws`, `.env`, certificats, coffres KeePass…).
* **Internet & réseau** – centre de contrôle réseau, changement de DNS en un clic, grand reset réseau en 7 étapes (TCP/IP, Winsock, DNS, ARP…), scanner Wi-Fi des réseaux voisins, rapport graphique des déconnexions, scanner des appareils de la box, liste des applications connectées et **benchmark DNS réel** (Cloudflare, Google, Quad9, OpenDNS).
* **Performances & gaming** – bascule normal / performances maximales, optimiseur de latence (HAGS, désactivation de Nagle, priorité CPU aux jeux), **banc d'essai visuel** (jauge de la note globale, notes par composant, mesures réelles en Mo/s et Go/s, comparatif avec des PC types ou une évaluation précédente, historique) et rapport énergétique de la batterie.
* **Disques & migration** – gestion des disques, Diskpart, optimisation/TRIM, BitLocker, et **sauvegarde / transfert de profil** vers un nouveau PC (fichiers personnels, favoris Chrome / Edge / Firefox, réseaux Wi-Fi, liste des logiciels réinstallable via Winget).
* **Applications & mises à jour** – installation groupée des logiciels du quotidien via Winget (Chrome, VLC, 7-Zip, Sumatra PDF, PowerShell, Office…), ArtCraft Launcher, mise à jour de tous les programmes en un clic, et suppression des applications préinstallées inutiles (bloatware) sans jamais toucher aux composants essentiels.
* **Personnalisation** – menu contextuel classique Windows 10 / moderne Windows 11, ouverture de CMD ou PowerShell en administrateur depuis le clic droit, boutons Éteindre / Redémarrer sur le Bureau, dossier *God Mode*.
* **Audit de site web** – note globale A+ à D, checklist de 21 contrôles avant mise en ligne, speedtest (DNS, TLS, TTFB), analyse SEO (aperçu Google, structure H1-H6, mots-clés) et inspection SSL / en-têtes HTTP.

### Interface

* **Console latérale intégrée** – affiche en direct la sortie des commandes PowerShell et CMD avec code couleur (erreurs, succès, avertissements), redimensionnable, zoom avec *Ctrl + molette*. Les barres de progression (SFC, DISM, winget) se mettent à jour sur une seule ligne.
* **Un script à la fois** – pendant l'exécution, les boutons des autres outils sont grisés ; fermer la console interrompt proprement le script en cours.
* **Recherche instantanée** parmi les 49 outils (titre, description, mots-clés), **favoris** persistants et barre latérale qui se replie automatiquement.
* **Interface sobre et cohérente** – palette neutre avec une seule couleur d'accent, icônes Windows (Segoe Fluent Icons) et **2 thèmes** *Sombre* (par défaut) et *Clair*, appliqués à toutes les fenêtres et mémorisés d'une session à l'autre.
* **Mises à jour intégrées** – notification discrète lorsqu'une nouvelle version est publiée sur GitHub, téléchargement et remplacement automatiques.

---

## Catalogue des outils

13 catégories, 49 outils. Chaque outil est enregistré dans [`Core/ToolRegistry.cs`](ScriptsWindows/ScriptsWindows/Core/ToolRegistry.cs) ; l'arborescence complète (avec les commandes associées) peut être prévisualisée et copiée depuis la page **Paramètres** de l'application.

| Catégorie | Sous-catégorie | Outils |
|---|---|---|
| 🔍 **Bilan & santé du PC** | Bilan général de l'ordinateur | Bilan de santé complet du PC · Audit d'éligibilité Windows 11 24H2 et NPU IA |
| | Vérifications rapides | Vérifier l'écran, les jeux et le son (DxDiag) · Accélérer le démarrage de Windows |
| | Intervention technicien | Intervention technicien et rapport client |
| | Pannes et pilotes | Écrans bleus et périphériques en erreur |
| 🖥️ **Performances & énergie** | Gestion de l'alimentation et vitesse | Changer la vitesse du PC (Normal / Rapide) · Optimiseur de latence et Mode Gaming Ultime |
| | Banc d'essai et performances | Tester la puissance globale de mon PC (WinSAT) · Détecter ce qui vide la batterie |
| 🔧 **Dépannage & réparation** | Réparation du système et des disques | Réparer les fichiers système (SFC & DISM) · Réparer les erreurs de disque (CHKDSK) |
| | Dépannage de l'imprimante et des services | Débloquer l'imprimante · Gérer et redémarrer les services Windows |
| | Boutique d'applications et affichage | Réparer le Microsoft Store et les icônes blanches |
| | Sauvegardes et points de retour | Créer ou restaurer un point de retour · Sauvegarder les pilotes |
| 🧹 **Nettoyage & espace disque** | Nettoyage système et disque | Grand nettoyage du PC et des fichiers inutiles |
| 🔑 **Mots de passe & licences** | Mots de passe et Wi-Fi | Retrouver mes mots de passe Wi-Fi · Consulter l'historique de mes navigateurs |
| | Licences et documents confidentiels | Retrouver ma clé Windows originale · Rechercher mes documents confidentiels |
| 🌐 **Internet & réseau** | Dépannage et amélioration de la connexion | Centre de contrôle Internet & Réseau · Benchmark DNS · Réparer la connexion (Grand Reset) |
| | Diagnostic et analyse du Wi-Fi | Détecteur des réseaux Wi-Fi voisins · Bilan des déconnexions Wi-Fi |
| | Appareils branchés et connexions actives | Voir les appareils connectés sur ma box · Voir les applications connectées à Internet |
| 💽 **Disques et stockage** | Organisation et gestion des disques | Gestionnaire de disques · Diskpart |
| | Santé et sécurité de vos fichiers | Optimiser et accélérer les disques · BitLocker |
| | Sauvegarde et migration | Sauvegarder et transférer un profil utilisateur |
| 📦 **Installer vos logiciels** | Installation et déploiement rapide | Installer vos applications indispensables |
| | Suite créative & multimédia | Installer ArtCraft Launcher |
| | Nettoyage des applications | Supprimer les applications préinstallées |
| 🔄 **Mises à jour des programmes** | Gestionnaire de mises à jour | Mettre à jour tous vos programmes (Winget) |
| 🛡️ **Sécurité & antivirus** | Protection globale de votre ordinateur | Centre de protection antivirus et surveillance |
| | Confidentialité et vie privée | Bouclier de confidentialité et anti-télémétrie |
| | Vérifications express | Analyse antivirus rapide · Historique des plantages et bugs |
| | Désinfection | Désinfection et détournements |
| | Comptes et accès | Gérer les comptes utilisateurs |
| 🎨 **Personnalisation de Windows** | Options et réglages système | Dossier Tous les réglages (God Mode) |
| | Raccourcis et ergonomie du Bureau | Boutons Éteindre et Redémarrer sur le Bureau |
| | Menu contextuel & clic droit | Menu contextuel (classique / moderne, CMD & PowerShell admin) |
| ⚡ **Démarrage & récupération** | Console interactive de démarrage | Centre de démarrage et récupération |
| ✨ **Nouveautés & innovations** | Audit web, speedtest & SEO | Centre d'audit & analyseur de site web |

---

## Gratuit & open source

L'application et tous les modules décrits ci-dessus sont gratuits et open source (licence **MIT**, voir [Licence](#licence)).

* 🚀 Aucun compte requis, aucune inscription.
* 🛡️ Aucun composant payant masqué ni publicité.
* 📦 Un seul fichier exécutable, aucune DLL à côté.

---

## Installation

### Exécutable (recommandé)
1. Ouvrez la page [Releases](https://github.com/aleex-le-dev/SCRIPT-WINDOWS-ALEEXLEDEV/releases/latest).
2. Téléchargez le fichier `AlexToolkit_vX.Y.Z.exe` de la dernière version.
3. Lancez-le : une invite UAC demande les droits administrateur (nécessaires aux réparations système). L'application se relance automatiquement en administrateur si besoin.

### Prérequis
* Windows 10 ou Windows 11, 64 bits.
* [Runtime .NET 8 Desktop (x64)](https://dotnet.microsoft.com/download/dotnet/8.0) – l'exécutable est publié en mode *framework-dependent* : s'il manque, Windows propose de le télécharger au premier lancement.
* Droits administrateur pour les actions système (DISM/SFC, pilotes, BCD, réseau, services).

---

## Confidentialité

Alex Toolkit fonctionne **en local** :
* Aucune donnée personnelle, aucun résultat de diagnostic n'est envoyé à un serveur.
* Aucune télémétrie, aucun traceur d'utilisation.
* Les mots de passe, clés et historiques révélés par les outils sont uniquement affichés à l'écran, jamais transmis.

Les seules connexions réseau sont :
* la **vérification des mises à jour** au démarrage (API publique GitHub des releases) – désactivable dans **Paramètres** ;
* les outils dont c'est la fonction et que vous lancez vous-même : installation/mise à jour via Winget, téléchargement d'ArtCraft, benchmark DNS, pings du scanner réseau, audit de site web.

Données stockées sur votre machine :

| Fichier | Contenu |
|---|---|
| `%LocalAppData%\ScriptsWindows\favoris.txt` | Liste de vos outils favoris |
| `%LocalAppData%\ScriptsWindows\settings_update.json` | Préférences de mise à jour |
| `%LocalAppData%\ScriptsWindows\theme.txt` | Thème choisi (sombre / clair) |
| `%LocalAppData%\ScriptsWindows\technicien.txt` | Nom du technicien (pré-rempli dans les rapports) |
| `Documents\Alex Toolkit\Rapports\` | Rapports d'intervention HTML générés |
| `Documents\Alex Toolkit\Sauvegardes désinfection\` | État d'origine sauvegardé avant chaque correction (hosts, proxy, tâches, démarrage) |
| `crash_log.txt` (à côté de l'exécutable) | Journal d'erreurs, créé uniquement en cas de plantage |

---

## Développement & compilation

Prérequis : [SDK .NET 8](https://dotnet.microsoft.com/download/dotnet/8.0) (le runtime seul ne suffit pas pour compiler).

```powershell
# Cloner le dépôt
git clone https://github.com/aleex-le-dev/SCRIPT-WINDOWS-ALEEXLEDEV.git
cd SCRIPT-WINDOWS-ALEEXLEDEV

# Compiler en 1 clic
.\build.bat
```

`build.bat` :
1. incrémente automatiquement la version (+0.0.1) dans le `.csproj` via `ScriptsWindows/bump_version.ps1` ;
2. publie en Release x64, fichier unique, pré-compilé ReadyToRun ;
3. supprime les anciens exécutables et dépose `AlexToolkit_vX.Y.Z.exe` à la racine, prêt à être joint à une release GitHub.

### Commandes manuelles

```powershell
# Compilation de développement (sans changer la version)
dotnet build ScriptsWindows/ScriptsWindows/ScriptsWindows.csproj -c Debug

# Publication Release fichier unique (sans incrément de version)
dotnet publish ScriptsWindows/ScriptsWindows/ScriptsWindows.csproj -c Release -r win-x64 --no-self-contained -p:PublishSingleFile=true

# Supprimer bin/, obj/ et les logs résiduels
.\clean.bat
```

### Ajouter un outil

1. Enregistrer l'outil dans `Core/ToolRegistry.cs` (catégorie, `SubHeading`, description, mots-clés, action) et sa commande dans `CommandMap`.
2. Placer la logique dans le service correspondant (`Services/`), et la fenêtre éventuelle dans `Views/Controls/`.
3. Vérifier qu'il apparaît dans l'arborescence de la page **Paramètres**.
4. Mettre à jour ce README (description et catalogue) et le [CHANGELOG](CHANGELOG.md).

### Choix techniques

* **Asynchrone partout** – les commandes PowerShell / CMD s'exécutent hors du thread UI, avec sortie ligne par ligne et annulation (le processus et ses enfants sont tués).
* **UTF-8 forcé** sur les flux PowerShell ; encodage adapté par exécutable pour CMD (UTF-16 pour `sfc`, UTF-8 pour `winget`, OEM 850 sinon) afin de préserver les accents.
* **Console rendue par lots** (une mise à jour par frame, 3 000 lignes max) pour rester fluide même avec des milliers de lignes de sortie.
* **Collectes en parallèle** dans les centres de diagnostic et réseau ; scanner LAN et benchmark DNS en .NET natif (aucun processus lancé).
* **Démarrage rapide** – ReadyToRun + Tiered Compilation, pages pré-construites une par une pendant les temps morts de l'interface.
* **Single-file** – bibliothèques natives (SQLite) auto-extraites, seules les ressources françaises des dépendances sont embarquées.

### Architecture du code

```text
SCRIPT-WINDOWS-ALEEXLEDEV/
├── build.bat                         ← Compilation 1 clic (bump de version + publish single-file)
├── clean.bat                         ← Purge bin/, obj/ et logs
├── LICENSE                           ← Licence MIT
├── CHANGELOG.md · ROADMAP.md · SUIVI_REFONTE.md · CLAUDE.md
├── docs/manual.md                    ← Guide d'utilisation
│
└── ScriptsWindows/
    ├── bump_version.ps1              ← Incrément automatique de <Version> dans le .csproj
    ├── ScriptsWindows.sln
    └── ScriptsWindows/
        ├── ScriptsWindows.csproj     ← .NET 8 WPF, single-file, ReadyToRun
        ├── App.xaml / .cs            ← Point d'entrée, élévation UAC, gestion globale des erreurs
        ├── MainWindow.xaml / .cs     ← Barre latérale, recherche, navigation, console de sortie
        ├── Assets/Styles/            ← Theme.xaml (jetons de couleur, typo, rayons), Controls.xaml (boutons, navigation, défilement)
        ├── Models/                   ← ScriptTool, ToolCategory, UpdateModels
        ├── Core/
        │   ├── ToolRegistry.cs              ← Catalogue des 49 outils + CommandMap
        │   ├── PowerShellRunner.cs          ← Exécution PowerShell asynchrone (UTF-8, annulation)
        │   ├── CommandRunner.cs             ← Exécution CMD / winget / netsh… avec sortie live
        │   ├── ScriptExecutionCoordinator.cs← Un seul script à la fois, verrouillage et interruption
        │   ├── AdminElevation.cs            ← Détection et relance en administrateur
        │   ├── ThemeManager.cs              ← Thèmes Sombre / Clair (palettes, persistance)
        │   ├── IconGlyphs.cs                ← Icônes Windows par catégorie
        │   ├── DialogHelper.cs              ← Ouverture standard des fenêtres d'outils
        │   ├── FavoritesManager.cs          ← Favoris persistants
        │   └── FolderDialogHelper.cs        ← Sélecteur de dossier
        ├── Services/                 ← Logique métier
        │   ├── DiagnosticService · HardwareService · RepairService · BootService
        │   ├── CleaningService · DiskService · NetworkService · DnsService
        │   ├── SecurityService · ExportService · UserService · AppService
        │   ├── PersonalizationService · InnovationService · UpdateService
        │   ├── InterventionService (checkup + rapport) · TroubleshootService (BSOD, périphériques)
        │   ├── ProfileMigrationService · MalwareCleanupService · DebloatService
        └── Views/
            ├── SectionViewBuilder.cs ← Construction des pages par sous-catégories
            ├── Pages/                ← Une page par catégorie + Favoris, Recherche, Paramètres
            └── Controls/             ← ToolCard + fenêtres spécialisées (DiagnosticCenter,
                                         NetworkCenter, SecurityCenter, BootRecovery, SystemCleanup,
                                         LanScanner, DnsBenchmark, WebsiteAnalyzer, Intervention, CrashDriver,
                                         UserAccounts, ProfileMigration, MalwareCleanup, Debloat, Update…)
```

---

## Idées, questions et signalements

* 💡 **Idées & suggestions** : [GitHub Discussions → Ideas](https://github.com/aleex-le-dev/SCRIPT-WINDOWS-ALEEXLEDEV/discussions) – proposez et votez pour les prochains outils.
* 💬 **Questions & entraide** : [GitHub Discussions → Q&A](https://github.com/aleex-le-dev/SCRIPT-WINDOWS-ALEEXLEDEV/discussions).
* 🐛 **Signaler un bug** : [ouvrir un ticket](https://github.com/aleex-le-dev/SCRIPT-WINDOWS-ALEEXLEDEV/issues/new) – joignez le fichier `crash_log.txt` s'il existe.
* 📜 **Historique des versions** : [journal des modifications](CHANGELOG.md).

---

## Licence

Distribué sous licence **MIT** – © 2026 AleexLeDev. Texte complet : [LICENSE](LICENSE).

En résumé, vous pouvez librement utiliser, copier, modifier, fusionner, publier, distribuer, sous-licencier et vendre des copies du logiciel, à condition de conserver la mention de copyright et le texte de la licence dans toutes les copies ou parties substantielles.

Le logiciel est fourni **« tel quel », sans aucune garantie**. Alex Toolkit exécute des opérations système avec les droits administrateur (réparations, registre, BCD, disques, réseau) : l'auteur ne peut être tenu responsable d'une perte de données ou d'un dysfonctionnement. Créez un point de restauration avant toute modification importante (outil *Créer ou restaurer un point de retour*).
