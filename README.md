# 🛠️ Boîte à Scripts Windows — AleexLeDev (v4.0 WPF)

> Mémo personnel du projet : structure du code, compilation de l'exécutable et nettoyage des fichiers temporaires.

---

## ⚡ 1. Compilation de l'exécutable

L'application se compile en un **fichier unique `.exe` autonome** (`PublishSingleFile`), léger (~3,6 Mo) et optimisé pour Windows 10/11.

### En 1 clic (recommandé)
Lance simplement le script à la racine :
> [`build.bat`](file:///c:/Users/Alex/Documents/DEV/PROJETS/GITHUB/SCRIPT-WINDOWS-ALEEXLEDEV/build.bat)  
Il compile le projet en Release et dépose directement [`ScriptsWindows.exe`](file:///c:/Users/Alex/Documents/DEV/PROJETS/GITHUB/SCRIPT-WINDOWS-ALEEXLEDEV/ScriptsWindows.exe) à la racine.

### En ligne de commande (PowerShell)
```powershell
dotnet publish ScriptsWindows/ScriptsWindows/ScriptsWindows.csproj -c Release -r win-x64 --no-self-contained -p:PublishSingleFile=true
```
L'exécutable généré se trouve dans :
`ScriptsWindows/ScriptsWindows/bin/Release/net8.0-windows/win-x64/publish/ScriptsWindows.exe`

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
* ✅ Vos fichiers sources (`ScriptsWindows/`, scripts `.sh` et `.bat`)
* ✅ Votre exécutable principal [`ScriptsWindows.exe`](file:///c:/Users/Alex/Documents/DEV/PROJETS/GITHUB/SCRIPT-WINDOWS-ALEEXLEDEV/ScriptsWindows.exe)
* 🔒 Vos identifiants locaux (`credentials.txt`, `webhook_tokens.txt`), qui restent protégés et ignorés par [`.gitignore`](file:///c:/Users/Alex/Documents/DEV/PROJETS/GITHUB/SCRIPT-WINDOWS-ALEEXLEDEV/.gitignore)

---

## 📁 3. Architecture du Projet C# / WPF

```text
SCRIPT-WINDOWS-ALEEXLEDEV/
├── ScriptsWindows.exe                     ← Exécutable autonome compilé
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
    │   ├── CleaningService.cs             ← Temp, purge WU, Cleanmgr, DNS, navigateurs, clean unifié
    │   ├── NetworkService.cs              ← Wi-Fi scan, ARP, reset TCP/IP & Winsock
    │   ├── DnsService.cs                  ← Cloudflare (1.1.1.1), Google (8.8.8.8), DHCP
    │   ├── AppService.cs                  ← Winget (check MAJ, upgrade all)
    │   ├── SecurityService.cs             ← Mots de passe Wi-Fi, CredMan, Defender, EICAR
    │   ├── UserService.cs                 ← Comptes locaux, admin, SuperAdmin
    │   ├── HardwareService.cs             ← SMART disques, WinSAT, écran tactile, profil Ultimate
    │   ├── ExportService.cs               ← Clé Windows BIOS/UEFI, backup pilotes DISM
    │   └── PersonalizationService.cs      ← God Mode, menu contextuel classique Win10
    │
    ├── Views/
    │   ├── Controls/
    │   │   └── ToolCard.xaml / .cs        ← Carte d'outil avec halo/ombre dynamique, favori ⭐ et badge Admin
    │   └── Pages/
    │       ├── FavorisPage.xaml           ← Page d'accueil dynamique des favoris
    │       ├── SearchResultsPage.xaml     ← Page de recherche en direct instantanée
    │       └── [Diagnostic, Reparation, Nettoyage, Reseau, Disque, Applications, Securite, Extraction, Materiel, Personnalisation]Page.xaml
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

## 🧰 5. Récapitulatif des 10 Catégories d'Outils

1. **Diagnostic** : Rapport complet WMI, santé batterie HTML, statut live, sondes CPU, barrettes et conso RAM, BitLocker, Defender, SMART.
2. **Réparation** : SFC /scannow, DISM Check & RestoreHealth, CHKDSK C:, reset composants WU, redémarrage Explorer, cache icônes, points de restauration.
3. **Nettoyage** : Vider temporaires, purge téléchargements WU, flush DNS, Cleanmgr, clés orphelines, caches navigateurs (Chrome, Edge, Firefox, Brave), nettoyage unifié.
4. **Réseau** : Appliquer DNS Cloudflare 1.1.1.1 / Google 8.8.8.8, restaurer DHCP, scan Wi-Fi, purge ARP, reset réseau TCP/IP et Winsock.
5. **Applications** : Lister les logiciels obsolètes et tout mettre à niveau via Winget.
6. **Sécurité** : Extraction des clés Wi-Fi en clair, Credential Manager, scans Defender rapide et complet, test EICAR.
7. **Utilisateurs** : Liste des comptes Windows locaux, ajout, droits admin, compte Super-Administrateur.
8. **Matériel** : Santé SMART approfondie, score WinSAT, on/off tactile, file d'impression spooler, plan d'alimentation Performances Ultimes.
9. **Extraction** : Clé produit Windows originale (BIOS OA3x), sauvegarde de tous les pilotes tiers avec DISM.
10. **Personnalisation** : Dossier God Mode bureau, rétablissement du clic droit classique Windows 10 sur Windows 11.
