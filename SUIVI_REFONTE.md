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
- **Cartes d'outils (`ToolCard`) style SaaS / Desktop moderne** :
  - **Badge d'icône structuré** : l'icône de chaque outil est logée dans un carré arrondi (`CornerRadius="8"`) avec fond d'accent doux translucide.
  - **Badge Admin translucide** : pilule discrète (`#1EF43F5E`) avec contour fin et typographie soignée.
  - **Bouton d'action interactif** : libellé `▶ Exécuter` avec icône de lecture intégrée et rayon moderne.
  - Rayons de courbure harmonisés à `12px` et ombres douces plus diffuses.
- **Barre de recherche avec Watermark dynamique** : texte indicatif *"Rechercher un outil..."* automatique lorsque le champ est vide.
- **Header épuré & Sélecteur Pill** : bouton de thème transformé en badge pilule élégant avec bordure fine et hover subtil.
- **Console latérale style Dev Tools** : en-tête `>_ Console` en Cascadia Code avec boutons `🗑️` et `✕` sans bordures superflues.

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

---

## 🚀 Résultat de Publication

- **Fichier exécutable unique** : [`AlexToolkit.exe`](file:///c:/Users/Alex/Documents/DEV/PROJETS/GITHUB/SCRIPT-WINDOWS-ALEEXLEDEV/AlexToolkit.exe) (à la racine)
- **Taille** : **~3,7 Mo** (autonome, sans installeur lourd)
- **Architecture** : Windows x64
- **Manifeste** : Élévation automatique UAC Administrateur requise
