# 📊 Suivi Refonte — Boîte à Scripts Windows v4.0 (C# + WPF)

> Dernière mise à jour : **2026-10-11 19:02**  
> Statut global : 🟢 **REFONTE COMPLÈTE + NOUVEAU DESIGN MULTI-THÈMES & TERMINAL DROIT**

---

## 🗺️ Vue d'ensemble des phases

| Phase | Nom | Statut | Progression |
|---|---|---|---|
| **1** | Fondations (projet, admin, runners, styles) | ✅ TERMINÉ | ▓▓▓▓▓▓▓▓▓▓ 100% |
| **2** | Shell UI (sidebar, cards, terminal droit, recherche) | ✅ TERMINÉ | ▓▓▓▓▓▓▓▓▓▓ 100% |
| **3** | Services core (Diagnostic, Réparation, Nettoyage, Réseau, DNS) | ✅ TERMINÉ | ▓▓▓▓▓▓▓▓▓▓ 100% |
| **4** | Services secondaires (Apps, Sécurité, Matériel, Export, Personnalisation) | ✅ TERMINÉ | ▓▓▓▓▓▓▓▓▓▓ 100% |
| **5** | Nouveautés UX : Thèmes commutables & Console latérale | ✅ TERMINÉ | ▓▓▓▓▓▓▓▓▓▓ 100% |

---

## 🎨 Nouveautés de l'Interface

### 1. Modernisation UI/UX de Pointe (Inspirée de Linear, Raycast & Fluent 2)
- **Élimination complète des doublons de titre** : suppression des en-têtes redondants qui doublaient le titre du header dans les 10 pages de catégories. La grille d'outils démarre directement avec un espacement aéré.
- **Correction des caractères accentués (UTF-8)** : forçage de l'encodage `[Console]::OutputEncoding` et `$OutputEncoding` en UTF-8 dans [`PowerShellRunner.cs`](file:///c:/Users/Alex/Documents/DEV/PROJETS/GITHUB/SCRIPT-WINDOWS-ALEEXLEDEV/ScriptsWindows/ScriptsWindows/Core/PowerShellRunner.cs), éliminant définitivement les caractères corrompus (ex: `Santé` au lieu de `Sant`).
- **Cartes d'outils (`ToolCard`) style SaaS / Desktop moderne avec listes d'actions directes** :
  - **Description agrandie et aérée** : dimensions portées à `325 × 215 px` (au lieu de 308 × 148 px) offrant une visibilité immédiate sur les fonctionnalités de chaque outil sans coupure de texte.
  - **Listes à puces directes (`Features`) intégrées sur chaque carte** : 3 actions ou composants clés réels affichés avec puces colorées dynamiques (couleur d'accent du thème actif) pour identifier instantanément ce que fait l'outil.
  - **Recherche enrichie** : le moteur de recherche universel scanne désormais également les éléments de liste (`Features`) en plus du titre, de la description et des tags.
  - **Badge d'icône structuré** : l'icône de chaque outil est logée dans un carré arrondi (`CornerRadius="8"`) avec fond d'accent doux translucide.
  - **Badge Admin translucide** : pilule discrète (`#1EF43F5E`) avec contour fin et typographie soignée.
  - **Bouton d'action interactif** : libellé `▶ Exécuter` avec icône de lecture intégrée et rayon moderne.
  - Rayons de courbure harmonisés à `12px` et ombres douces plus diffuses.
- **Barre de recherche avec Watermark dynamique** : texte indicatif *"Rechercher un outil..."* automatique lorsque le champ est vide.
- **Header épuré & Sélecteur Pill** : bouton de thème transformé en badge pilule élégant avec bordure fine et hover subtil.
- **Sous-catégories toutes dépliées par défaut** : toutes les sous-catégories de toutes les pages (et favoris) sont désormais dépliées par défaut (`Visibility.Visible`, chevron `▼`), garantissant qu'aucun script n'est masqué et que l'ensemble des outils est visible immédiatement. Le clic sur un en-tête permet de basculer indépendamment la section souhaitée.
- **Console latérale style Dev Tools** : en-tête `>_ Console` en Cascadia Code avec boutons `🗑️` et `✕` sans bordures superflues, largeur compacte calibrée (réservant 740 px pour les cartes) pour maintenir 2 colonnes de cartes côte à côte.

### 2. Sélecteur de Thèmes Dynamique avec Transparence & Effets Néon
- Gestionnaire de thèmes dédié : [`ThemeManager.cs`](file:///c:/Users/Alex/Documents/DEV/PROJETS/GITHUB/SCRIPT-WINDOWS-ALEEXLEDEV/ScriptsWindows/ScriptsWindows/Core/ThemeManager.cs).
- **Sidebar gauche allégée** : le bloc inférieur inutile a été supprimé afin de maximiser l'espace vertical pour la liste des catégories d'outils.
- **Icône de thème discrète** : positionnée dans le bandeau supérieur à côté du bouton `⬛ Console` avec infobulle descriptive du style en cours.
- **Console d'exécution préservée** : aucun log superflu lors du changement de thème, la console reste 100% dédiée aux sorties des scripts et outils système.
- 5 styles visuels complets commutables instantanément avec transparence alpha et ombres/lueurs néon :
  1. 💎 **Aero Glass (Translucide)** : Verre fumé translucide, fond à dégradé atmosphérique, reflets givrés et halos indigo.
  2. ⚡ **Cyberpunk Neon** : Halo néon rose rubis intense, bordures lumineuses et contrastes magenta translucides.
  3. 🟢 **Matrix Phosphor** : Ambiance terminal hacker sombre avec reflets émeraude et lueur phosphorescente.
  4. ❄️ **Nordic Frost** : Verre arctique givré, transparence polaire et touches cyan glacier.
  5. 👑 **Obsidian Luxury** : Noir OLED pur, filets d'or chaud, contrastes métalliques haute précision.
- Effets dynamiques intégrés sur [`ToolCard.xaml`](file:///c:/Users/Alex/Documents/DEV/PROJETS/GITHUB/SCRIPT-WINDOWS-ALEEXLEDEV/ScriptsWindows/ScriptsWindows/Views/Controls/ToolCard.xaml) : `DropShadowEffect` réactif (couleur, rayon et opacité selon le thème) et biseau personnalisé `CardBorderThickness`.

### 3. Centres Unifiés & Modernisation des Catégories
- **🔍 Diagnostic Matériel et Intégrité (`DiagnosticPage.xaml.cs`)** :
  - **Centre de Diagnostic Matériel et Système (`DiagnosticCenterDialog.xaml`)** :
    1. 🖥️ **Système & BIOS** : Identité complète de la machine (Nom, Fabricant, Modèle), Windows et Build, Carte mère et version / date du BIOS/UEFI.
    2. 🧠 **CPU & RAM** : Processeur, cœurs/threads, fréquence, sondes thermiques et détail des barrettes de RAM (slots, type DDR, fréquence, capacité).
    3. 💾 **Stockage & S.M.A.R.T.** : État de santé des disques physiques (NVMe, SSD, HDD), jauges d'espace libre et d'occupation des volumes.
    4. 🔋 **Batterie & Réseau** : Niveau %, statut secteur/décharge, tension, autonomie estimée, cartes réseau actives (IP, Passerelle, DNS) et test Ping direct.
    5. ⚡ **Outils & Actions** : Accès direct au gestionnaire de démarrage, analyse SFC, msinfo32, dxdiag, devmgmt, resmon et mdsched.
  - Outils rapides 1-clic : Diagnostic matériel DirectX / GPU (`diag_dxdiag`, déplacé depuis Matériel), Gestionnaire des programmes au démarrage (`StartupManagerDialog.xaml`) et rapport complet de batterie HTML.
- **🖥️ Matériel et Périphériques (`MaterielPage.xaml.cs`)** :
  - Épuration des doublons de diagnostic (`mdsched`, `msinfo32`, `resmon` absorbés par le Centre de Diagnostic).
  - Restructurée en outils de gestion physique et banc d'essai :
    1. 🔌 **Périphériques et Pilotes** : Gestionnaire de périphériques officiel (`devmgmt.msc`).
    2. 🚀 **Performances et Alimentation** : Benchmark global WinSAT (`winsat formal`), Rapport d'efficacité énergétique 60s (`powercfg /energy`) et Activation immédiate du mode Performances Élevées.
- **📦 Gestionnaire de Paquets Winget (`WingetUpdatesDialog.xaml`)** : Sélection interactive des logiciels à mettre à jour, sélection globale, détection et fallback `--force` en cas de conflit de portée.
- **🧹 Nettoyage Système & Disque (`SystemCleanupDialog.xaml`)** : Assistant Cleanmgr, purge des fichiers temporaires (%TEMP%), corbeille, cache Windows Update, navigateurs et sélection personnalisée.
- **🎨 Personnalisation (`ContextMenuDialog.xaml`)** : Bascule interactive entre le clic droit classique Windows 10 et moderne Windows 11 avec redémarrage d'Explorer.
- **🔧 Réparation Intégrale (`ReparationPage.xaml.cs`)** :
  - Restructurée en **4 sous-catégories accordéons** élégantes via `PopulateSections` :
    1. 🩺 **Intégrité Système et Disque** : Centre d'intégrité DISM/SFC (`SystemIntegrityDialog.xaml`) & Analyse CHKDSK (`ChkdskDialog.xaml`).
    2. ⚙️ **Services et Impression** : Gestionnaire des Services Windows (`ServicesManagerDialog.xaml`) & Déblocage / Purge file d'impression (`FixPrintSpooler`).
    3. 🏬 **Applications et Interface Windows** : Réparation Store UWP & Interface (`AppsShellRepairDialog.xaml`).
    4. 🔄 **Sauvegarde et Restauration** : Points de Restauration Système (`RestorePointDialog.xaml`) & Sauvegarder pilotes DISM.
- **🌐 Réseau et Configuration (`ReseauPage.xaml.cs`)** :
  - Restructurée en **3 sous-catégories accordéons** élégantes via `PopulateSections` :
    1. 🌐 **Connectivité et Configuration Globale** :
       - *Centre de Contrôle Réseau* (`NetworkCenterDialog.xaml` avec 4 onglets : Diagnostic, Gestionnaire DNS Cloudflare/Google/Quad9/DHCP, Dépannage, Scanners).
       - *Dépannage & Grand Reset Réseau* (protocole curatif en 7 étapes).
    2. 📡 **Découverte Sans Fil et Wi-Fi** :
       - *Scanner et Analyseur Wi-Fi (Graphique)* (`WifiScannerDialog.xaml`) : vue interactive des bornes Wi-Fi avec jauges de signal colorées en %, canaux et sécurité.
       - *Rapport de Santé Wi-Fi (Graphique)* (`WifiReportDialog.xaml`) : visionneuse interactive intégrée du rapport officiel Windows avec graphiques des 3 derniers jours, export HTML et ouverture plein écran.
    3. 🖥️ **Cartographie LAN et Surveillance des Flux** :
       - *Scanner Réseau Local (LAN Graphique)* (`LanScannerDialog.xaml`) : détection interactive des appareils connectés sur la Box avec ping en direct et copie rapide d'IP.
- **🛡️ Surveillance, Sécurité et Événements (`SecuritePage.xaml.cs`)** :
  - Restructurée en **2 sous-catégories accordéons** modernes et épurées (doublons console absorbés par le Centre) :
    1. 🛡️ **Centre de Sécurité et Surveillance** :
       - *Centre de Surveillance et Sécurité* (`SecurityCenterDialog.xaml` avec 3 onglets interactifs : Protection Defender temps réel, Journal Erreurs/Crashs BSOD 24h, et Top 15 Processus RAM avec jauges).
    2. ⚡ **Actions et Outils Rapides** :
       - *Analyse rapide Defender* (scan mémoire et exécutables sensibles en 1 clic direct).
       - *Moniteur de fiabilité (Perfmon)* (historique visuel de stabilité au fil du temps en 1 clic direct).
- **📦 Installation d'Applications (`ApplicationsPage.xaml.cs`)** :
  - **Gestionnaire Unifié d'Installation (`AppInstallerDialog.xaml`)** :
    - Regroupe strictement vos **6 applications indispensables** :
      1. 🌐 **Google Chrome** (Navigateur web rapide et sécurisé)
      2. 🎬 **VLC Media Player** (Lecteur multimédia universel)
      3. 📄 **Sumatra PDF** (Lecteur PDF léger et rapide)
      4. 🗜️ **7-Zip** (Gestionnaire d'archives haute compression)
      5. ⚡ **PowerShell 7** (Terminal moderne multiplateforme)
      6. 💼 **Microsoft Office** (Installateur complet via lien Google Drive)
    - **Options interactives** : cases à cocher pour sélection multiple, bouton *Tout cocher* / *Tout décocher*, déploiement groupé en 1 clic avec barre de progression et boutons d'installation individuels par ligne.
    - **Affichage direct** : la carte de l'outil est affichée directement sur la page sans sous-catégorie ni bandeau d'accordéon superflu.
    - Anciens scripts individuels 1-clic supprimés de la page pour une interface 100% épurée et sans doublons.

- **🖥️ Matériel et Périphériques (`MaterielPage.xaml.cs`)** :
  - Gestionnaire de périphériques retiré de cette section (déjà accessible en 1 clic dans le Centre de Diagnostic unifié, onglet Outils/Actions).
  - Bascule en **affichage direct** (`PopulateDirect`) en grille pour les 3 outils restants :
    1. ⚙️ *Benchmark global (WinSAT)*
    2. ⚡ *Rapport d'efficacité énergétique*
    3. ⚡ **Plans d'alimentation (Normal / Performances)** (`PowerPlanDialog.xaml`) :
       - **Bascule interactive & réversible** : détection en temps réel du plan actif avec badge vert.
       - **Choix du mode** : lister tous les profils installés sur la machine avec bouton d'activation par profil.
       - **Raccourcis 1 clic** :
         - ⚖️ *Mode Normal (Équilibré)* : retour immédiat au plan par défaut Windows.
         - 🚀 *Hautes Performances* : pour le plein potentiel du CPU.
         - ⚡ *Performances Ultimes* : déblocage et activation du scheme ultime sans restriction.

- **💽 Disque Dur et Stockage (`DisquePage.xaml.cs`)** :
  - Doublon console `disk_trim_opt` supprimé (déjà pris en charge graphiquement par `dfrgui.exe`).
  - Ajout du **Gestionnaire de disques officiel Windows** (`diskmgmt.msc`).
  - Restructuration en **2 sous-catégories symétriques et équilibrées** :
    1. 💻 **Volumes et Partitions** : Gestionnaire de disques (GUI) + Console de formatage DISKPART.
    2. ⚡ **Maintenance et Chiffrement** : Optimisation & Défragmentation (GUI) + Gestion et Chiffrement BitLocker.

- **🔄 Mises à Jour et Inventaire Logiciel (`MisesAJourPage.xaml.cs`)** :
  - Doublon `upd_defender_sigs` supprimé (centralisé dans Sécurité).
  - Bascule en **affichage direct** (`PopulateDirect`) avec le **Gestionnaire de Paquets Winget** en pleine page sans accordéon superflu.

- **🧹 Nettoyage et Optimisation Espace (`NettoyagePage.xaml.cs`)** :
  - Doublon console `clean_winsxs` supprimé (intégré au Centre de Nettoyage).
  - Bascule en **affichage direct** (`PopulateDirect`) avec le **Centre de Nettoyage Système et Disque**.

- **🔑 Récupération d'Identifiants et Secrets (`SecretsPage.xaml.cs`)** :
  - Suppression des 4 accordéons isolés d'un seul outil.
  - Bascule en **affichage direct** (`PopulateDirect`) en grille moderne regroupant les 5 outils de secrets.

- **🔍 Diagnostic Matériel et Intégrité (`DiagnosticPage.xaml.cs`)** :
  - Doublon `diag_battery` retiré (déjà disponible dans l'onglet 4 du Centre de Diagnostic avec génération HTML).
  - Organisation épurée : Centre de Diagnostic unifié (5 onglets) + DxDiag et Gestionnaire de démarrage rapide.

- **⚡ Démarrage et Récupération (`DemarragePage.xaml.cs`)** :
  - Suppression des 4 doublons redondants (`boot_safemode`, `boot_winre`, `boot_uefi_bios`, `boot_bcd_repair`) entièrement unifiés dans la boîte de dialogue du Centre.
  - Bascule en **affichage direct** (`PopulateDirect`) avec le **Centre de Démarrage et Récupération** (`BootRecoveryDialog.xaml`) en pleine page sans accordéons superflus.

- **✨ Nouveautés & Innovations (`NouveautePage.xaml.cs`)** :
  - **Intégration native des 4 outils** dans leurs catégories de référence :
    1. 🛡️ **Bouclier de confidentialité et anti-télémétrie (`PrivacyShieldDialog.xaml`)** -> Reclassé dans **🛡️ Sécurité** (`SECURITE`, section *« Confidentialité et vie privée »*).
    2. ⚡ **Optimiseur de latence et Mode Gaming Ultime (`GamingOptimizerDialog.xaml`)** -> Reclassé dans **🖥️ Matériel / Performances** (`MATERIEL`, section *« Gestion de l'alimentation et vitesse »*).
    3. 🔍 **Audit d'éligibilité Windows 11 24H2 & IA (`Win11AiAuditDialog.xaml`)** -> Reclassé dans **🔍 Diagnostic** (`DIAGNOSTIC`, section *« Bilan général de l'ordinateur »*).
    4. 🌐 **Benchmark de vitesse DNS en temps réel (`DnsBenchmarkDialog.xaml`)** -> Reclassé dans **🌐 Réseau** (`RESEAU`, section *« Dépannage et amélioration de la connexion »*).
  - La catégorie **Nouveautés & Innovations** reste présente dans le menu avec un composant d'accueil épuré (*Empty State* stylisé), prête à accueillir les futurs scripts et ajouts.

---

## 👥 Refonte des Textes & Vulgarisation Grand Public

Tous les textes ont été réécrits pour être compris instantanément par des utilisateurs sans compétences informatiques :
- **Bénéfice utilisateur direct** mis en avant plutôt que le nom de la commande interne (ex: "Débloquer l'imprimante" au lieu de "Purge du spouleur", "Retrouver mes mots de passe Wi-Fi" au lieu de "Netsh wlan show profiles", "Accélérer le démarrage de Windows" au lieu de "Gestionnaire du démarrage").
- **Explications simples et claires** : chaque script explique en 1 ou 2 phrases concrètes son utilité et pourquoi l'utiliser.
- **Titres de sections et sous-titres accueillants** : "Bilan & Santé du PC", "Performances & Énergie", "Dépannage & Réparation", "Installer vos logiciels", etc.

---

## 🖱️ Navigation Intuitive au Survol & Stabilité des Sous-Catégories

- **Navigation instantanée au survol (Hover)** :
  - Passer le curseur de la souris sur une catégorie de la barre latérale gauche affiche immédiatement la page correspondante avec un micro-délai de 50 ms (anti-flicker lors des passages rapides).
  - Le clic reste fonctionnel à 0 ms de délai.
- **Réduction automatique de la colonne catégories en mode compact (Mini-sidebar)** :
  - Dès que la souris se déplace vers les sous-catégories et les cartes d'outils, la barre latérale se rétracte automatiquement à **64 px** avec une animation fluide (`DoubleAnimation`).
  - **Vue épurée en mode compact** : seuls le logo (`⚡`), l'icône de recherche (`🔍`) et les icônes des catégories / favoris / paramètres restent visibles, parfaitement centrés verticalement et horizontalement dans leur contour (48x36 px avec bords arrondis). Tous les textes, badges et libellés superflus sont masqués.
  - **Survol et Tooltips** : chaque icône dispose d'un `ToolTip` explicatif au survol.
  - **Dépliage automatique au retour** : lorsque le curseur revient sur la barre latérale, celle-ci s'agrandit instantanément à **240 px** pour retrouver l'intégralité des noms et des compteurs de scripts.
  - **Accès rapide à la recherche** : cliquer sur l'icône `🔍` en mode compact déplie la barre latérale et place directement le focus dans le champ de saisie.
- **Maintien de la catégorie active lors du déplacement vers les sous-catégories** :
  - Dès que la souris quitte la barre latérale pour entrer dans la zone de contenu (cartes et sous-catégories), les timers sont verrouillés et la catégorie active reste figée.
  - L'utilisateur peut explorer, faire défiler et cliquer sur les outils sans aucun changement intempestif de page.
- **Dépliage automatique complet** :
  - Toutes les sous-catégories et sections sont désormais dépliées par défaut dès l'ouverture d'une page.
- **Masquage moderne de l'ascenseur de la barre latérale** :
  - L'ascenseur vertical a été complètement masqué (`VerticalScrollBarVisibility="Hidden"`), offrant un design épuré, ultra-moderne et uniforme (sans barre de défilement apparente).
  - Le défilement fluide à la molette de la souris reste 100% actif et fonctionnel.
- **Défocalisation intuitive de la barre de recherche** :
  - Tout clic dans les sous-catégories, sur une carte, un bouton ou en dehors de la boîte de recherche retire immédiatement et obligatoirement le focus et le curseur clignotant via `ClearSearchFocus()` (avec dissociation du `FocusScope` WPF et transfert du focus vers la racine `MainRootGrid`).
  - L'appui sur la touche `Échap` (`Escape`) réinitialise également le texte de recherche et retire le focus.
  - Dès le clic dans la zone de contenu ou après perte de focus, la barre latérale se rétracte instantanément en mode compact.
- **Calibrage de la console de logs** :
  - Largeur de console optimisée (360 px max, 740 px réservés) garantissant l'affichage systématique de 2 colonnes de cartes côte à côte.
- **Fiche de spécifications 100% PC dans les Paramètres (Sans icônes)** :
  - Remplacement des cartes éparpillées par **1 seul bloc unifié ligne par ligne** façon fiche technique officielle sobre et épurée (sans icônes, uniquement les libellés et caractéristiques) :
    1. *Ordinateur & Utilisateur* (Machine et nom de session active)
    2. *Droits d'accès* (Administrateur / Standard)
    3. *Système d'exploitation* (Édition, numéro de Build et architecture)
    4. *Processeur (CPU)* (Modèle exact et nombre de threads)
    5. *Mémoire vive (RAM)* (Capacité totale uniquement, ex. 32 Go)
    6. *Disques & Stockage* (Liste de tous les lecteurs prêts : type physique SSD, SSD (NVMe), HDD, Amovible/USB, pourcentage d'utilisation %, espace libre et total — chaque disque sur sa propre ligne)
  - Retrait de toutes les icônes emoji (dans l'interface et dans le rapport texte) pour une présentation purement technique.
  - Bouton interactif `Copier la fiche` pour exporter l'ensemble de la fiche technique formatée dans le presse-papiers en un clic.

---

## 🚀 Résultat de Publication

- **Fichier exécutable unique** : [`AlexToolkit.exe`](file:///c:/Users/Alex/Documents/DEV/PROJETS/GITHUB/SCRIPT-WINDOWS-ALEEXLEDEV/AlexToolkit.exe) (à la racine)
- **Taille** : **~10.8 Mo** (autonome, single-file Release x64)
- **Architecture** : Windows x64
- **Manifeste** : Élévation automatique UAC Administrateur requise



