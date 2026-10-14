# 🛠️ Alex Toolkit — AleexLeDev

> Outil tout-en-un de diagnostic, réparation, optimisation et maintenance Windows 10/11.

---

## ⚡ 1. Compilation de l'exécutable

L'application se compile en un **fichier unique `.exe` autonome** (`PublishSingleFile`), ultra réactif (ReadyToRun AOT) et optimisé pour Windows 10/11.

### En 1 clic (recommandé)
Lance simplement le script à la racine :
> [`build.bat`](file:///c:/Users/Alex/Documents/DEV/PROJETS/GITHUB/SCRIPT-WINDOWS-ALEEXLEDEV/build.bat)  
Il compile le projet en Release et dépose directement [`AlexToolkit.exe`](file:///c:/Users/Alex/Documents/DEV/PROJETS/GITHUB/SCRIPT-WINDOWS-ALEEXLEDEV/AlexToolkit.exe) à la racine.

### En ligne de commande (PowerShell)
```powershell
dotnet publish ScriptsWindows/ScriptsWindows/ScriptsWindows.csproj -c Release -r win-x64 --no-self-contained -p:PublishSingleFile=true
```
L'exécutable généré se trouve dans :
`ScriptsWindows/ScriptsWindows/bin/Release/net8.0-windows/win-x64/publish/AlexToolkit.exe`

*(Note : Si le SDK .NET 8 n'est pas installé sur la machine :)*
```powershell
& ([scriptblock]::Create((Invoke-RestMethod -Uri https://dot.net/v1/dotnet-install.ps1))) -Channel 8.0 -InstallDir "$env:LOCALAPPDATA\Microsoft\dotnet"
```

---

## 🧹 2. Nettoyage Automatique

Le nettoyage des fichiers temporaires et des caches est **100% automatisé**.

Il suffit de double-cliquer sur :
> 👉 **[`clean.bat`](file:///c:/Users/Alex/Documents/DEV/PROJETS/GITHUB/SCRIPT-WINDOWS-ALEEXLEDEV/clean.bat)**

### Ce que le script nettoie automatiquement :
* 🗑️ **Dossiers de collecte & tests** : supprime `DiagNav/`
* 🗑️ **Rapports & logs résiduels** : supprime `Intelligence_Report.txt` et tous les fichiers `*.log`
* 🗑️ **Caches de compilation** : purge les dossiers `bin/` et `obj/` de Visual Studio / .NET

### Ce qui est conservé et sécurisé :
* ✅ Vos fichiers sources C# / WPF (`ScriptsWindows/`)
* ✅ Votre exécutable principal [`AlexToolkit.exe`](file:///c:/Users/Alex/Documents/DEV/PROJETS/GITHUB/SCRIPT-WINDOWS-ALEEXLEDEV/AlexToolkit.exe)

---

## 📁 3. Architecture du Projet C# / WPF

```text
SCRIPT-WINDOWS-ALEEXLEDEV/
├── AlexToolkit.exe                        ← Exécutable autonome compilé
├── build.bat                              ← Script de build en 1 clic
├── clean.bat                              ← Script de purge des caches
├── SUIVI_REFONTE.md                       ← Suivi d'avancement technique
│
└── ScriptsWindows/ScriptsWindows/
    ├── App.xaml / App.xaml.cs             ← Point d'entrée, styles globaux, élévation UAC
    ├── MainWindow.xaml / .cs              ← Fenêtre principale (Sidebar + Topbar + Terminal latéral à droite)
    │
    ├── Core/                              ← Moteurs et utilitaires
    │   ├── ThemeManager.cs                ← 5 thèmes visuels dynamiques (Glassmorphism, Néon, Matrix...)
    │   ├── AdminElevation.cs              ← Auto-élévation UAC admin si nécessaire
    │   ├── PowerShellRunner.cs            ← Exécuteur PowerShell async avec streaming live
    │   ├── CommandRunner.cs               ← Exécuteur de commandes CMD / Process
    │   ├── ToolRegistry.cs                ← Catalogue central de TOUS les outils
    │   └── FavoritesManager.cs            ← Persistance locale des favoris (%LocalAppData%)
    │
    ├── Services/                          ← Logique métier de chaque catégorie (100% C#)
    │   ├── DiagnosticService.cs           ← Infos système, WMI, batterie, RAM, thermal
    │   ├── RepairService.cs               ← SFC, DISM, CHKDSK, reset Windows Update, Explorer
    │   ├── BootService.cs                 ← Démarrage avancé, SafeBoot, WinRE, BIOS/UEFI, réparation BCD
    │   ├── CleaningService.cs             ← Temp, purge WU, Cleanmgr, DNS, navigateurs, clean unifié
    │   ├── NetworkService.cs              ← Wi-Fi scan, ARP, reset TCP/IP & Winsock
    │   ├── DnsService.cs                  ← Cloudflare (1.1.1.1), Google (8.8.8.8), DHCP
    │   ├── AppService.cs                  ← Winget (check MAJ, upgrade all)
    │   ├── SecurityService.cs             ← Mots de passe Wi-Fi, CredMan, Defender, EICAR
    │   ├── UserService.cs                 ← Comptes locaux, admin, SuperAdmin
    │   ├── HardwareService.cs             ← SMART disques, WinSAT, écran tactile, profil Ultimate
    │   ├── ExportService.cs               ← Clé Windows BIOS/UEFI, backup pilotes DISM
    │   ├── PersonalizationService.cs      ← God Mode, menu contextuel classique Win10
    │   └── InnovationService.cs           ← Télémétrie, débloat, mode gaming, audit IA 24H2, top fichiers, DNS bench
    │
    ├── Views/
    │   ├── Controls/
    │   │   ├── ToolCard.xaml / .cs        ← Carte d'outil avec halo/ombre dynamique, favori ⭐ et badge Admin
    │   │   ├── [PrivacyShield, Debloat, GamingOptimizer, Win11AiAudit, TopFiles, DnsBenchmark]Dialog.xaml / .cs ← Dialogues graphiques réversibles
    │   │   └── BootRecoveryDialog.xaml / .cs ← Console interactive de démarrage et choix du redémarrage
    │   └── Pages/
    │       ├── FavorisPage.xaml           ← Page d'accueil dynamique des favoris
    │       ├── SearchResultsPage.xaml     ← Page de recherche en direct instantanée
    │       └── [Applications, Demarrage, Diagnostic, Disque, Materiel, MisesAJour, Nettoyage, Nouveaute, Personnalisation, Reparation, Reseau, Secrets, Securite]Page.xaml
    │
    └── Assets/Styles/
        ├── Theme.xaml                     ← Variables de thèmes, transparence, gradients, lueurs néon
        └── Controls.xaml                  ← Styles des boutons, cartes, sidebar, scrollbars
```

---

## 🎨 4. Nouveautés d'Interface & Expérience Visuelle

### ⬛ Terminal Latéral à Droite (Contrôles 100% Internes)
* **Header épuré** : aucun bouton console ni indicateur dans la barre supérieure ; le haut de la fenêtre reste net et minimaliste.
* **Ouverture automatique** : la console (240px) s'ouvre à droite dès qu'un outil ou un script est lancé.
* **Bouton de fermeture direct sur la console** :
  * Cliquez sur **`✕`** dans la barre d'outils de la console pour la masquer.
  * Cliquez sur **`🗑️`** pour vider le flux de texte.
  * L'état **`● En cours`** est affiché directement dans l'en-tête de la console.

### 💎 Sélecteur de Thèmes Dynamique avec Transparence & Néon
Cliquez sur le bouton `🎨 Thème` dans le bandeau supérieur pour faire défiler instantanément **5 styles visuels complets** :
1. 💎 **Aero Glass (Translucide)** : Dégradé profond atmosphérique, cartes translucides effet verre fumé givré.
2. ⚡ **Cyberpunk Neon** : Contrastes intenses et halo néon rose rubis sous les cartes d'outils.
3. 🟢 **Matrix Phosphor** : Ambiance terminal hacker, vert phosphore émeraude.
4. ❄️ **Nordic Frost** : Verre polaire translucide givré et accents cyan glacier.
5. 👑 **Obsidian Luxury** : Noir OLED absolu, biseau et lueur dorée pour une finition de haute précision.

---

## 🧰 5. Récapitulatif des 13 Catégories d'Outils

1. **Applications** : Gestionnaire d'installation unifié et mise à jour de logiciels essentiels (Chrome, VLC, 7-Zip, Office...).
2. **Démarrage & Récupération** : Centre interactif unifié de démarrage et récupération (mode sans échec minimal/réseau, accès direct WinRE, bascule vers le BIOS/UEFI, réparation et réinitialisation du magasin BCD).
3. **Diagnostic** : Bilan de santé unifié (sondes CPU, barrettes et conso RAM, SMART, batterie, réseau), Audit d'éligibilité complet Windows 11 24H2 & NPU IA Copilot+, DirectX / DxDiag, programmes au démarrage.
4. **Disque** : Gestion des disques (diskmgmt), DISKPART, défragmentation / optimisation TRIM et chiffrement BitLocker.
5. **Matériel** : Optimiseur gaming et latence minimale (CPU 100%, TCP Nagle, HAGS), gestionnaire des plans d'alimentation (Normal, Élevé, Ultime), indice de performance WinSAT et rapport d'efficacité énergétique.
6. **Mises à jour** : Gestionnaire interactif des mises à jour applicatives via Winget.
7. **Nettoyage** : Console interactive de nettoyage (fichiers temporaires, Cleanmgr, caches navigateurs, corbeille et nettoyage WinSxS).
8. **Nouveautés & Innovations** : Espace dédié prêt à recevoir vos prochains scripts et outils innovants.
9. **Personnalisation** : Dossier Tous les Réglages (God Mode) et bascule vers le clic droit classique Windows 10.
10. **Réparation** : Centre d'intégrité Windows (DISM Scan/Restore & SFC), CHKDSK interactif, gestionnaire des services, déblocage spouleur et points de restauration.
11. **Réseau** : Centre de contrôle réseau, Benchmark de vitesse DNS en direct avec mesure de latence et ping, protocole de reset en 7 étapes, scanners Wi-Fi et LAN, analyse des connexions TCP actives.
12. **Secrets & Licences** : Affichage des clés Wi-Fi en clair, audit des fichiers sensibles (.ssh, .env), clé originale Windows BIOS et générateur de mots de passe.
13. **Sécurité** : Centre de protection antivirus Defender, Bouclier de confidentialité et anti-télémétrie réversible, analyse rapide 1-clic et moniteur de fiabilité des plantages (perfmon /rel).
