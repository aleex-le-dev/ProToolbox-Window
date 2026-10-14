# Alex Toolkit
by AleexLeDev – diagnostiquez, réparez, optimisez et sécurisez votre système Windows 10 & 11 au sein d'une interface néon moderne : bilan de santé matériel en direct, intégrité Windows en un clic, purge des caches, bouclier anti-télémétrie, optimisation gaming à faible latence et console de démarrage avancée. Aucun outil tiers requis, aucun cloud, aucun traceur, conçu pour un usage instantané et autonome.

⚡ [Télécharger la dernière version (v4.x)](https://github.com/aleex-le-dev/SCRIPT-WINDOWS-ALEEXLEDEV/releases/latest) – exécutable autonome unique `AlexToolkit.exe` : double-cliquez et utilisez. Rien à installer.  
📖 Documentation : [Guide d'utilisation](docs/manual.md) · [Suivi technique & Refonte](SUIVI_REFONTE.md) (également dans ce dépôt : [CLAUDE.md](CLAUDE.md))  
💬 Communauté : [GitHub Discussions](https://github.com/aleex-le-dev/SCRIPT-WINDOWS-ALEEXLEDEV/discussions) – partagez vos retours d'expérience, posez vos questions, proposez de nouveaux scripts.  
🗺️ Ce qui arrive ensuite : [Roadmap](ROADMAP.md) – fonctionnalités prévues, nouvelles catégories et optimisations de performance.

---

## Ce qu'il fait

* **Centre de diagnostic & santé matérielle** – analyse en temps réel de votre configuration (processeur, barrettes et consommation RAM, santé S.M.A.R.T. des disques NVMe/SSD, jauge et usure de la batterie, bilan thermique). Inclut l'audit d'éligibilité complet Windows 11 24H2, TPM 2.0 et processeurs NPU IA Copilot+, ainsi que l'accès direct aux outils DxDiag et gestionnaire de démarrage.
* **Intégrité & réparation système** – diagnostic et restauration automatique des fichiers corrompus via DISM Scan/Restore et SFC avec suivi en direct, interface interactive CHKDSK pour volumes NTFS, déblocage et purge instantanée du spouleur d'impression, réinitialisation du Windows Store UWP et sauvegarde en un clic de tous vos pilotes matériels.
* **Démarrage avancé & récupération** – console unifiée de gestion du boot pour basculer en un clic vers le mode sans échec (minimal ou réseau), redémarrer directement dans le BIOS/UEFI sans appuyer frénétiquement sur une touche, accéder à l'environnement WinRE ou reconstruire le magasin de démarrage BCD.
* **Nettoyage & optimisation d'espace** – console interactive de purge unifiée ciblant les fichiers temporaires système et utilisateur, le cache Windows Update, le dossier WinSxS, la corbeille et les résidus de navigation (Google Chrome, Microsoft Edge, Brave, Mozilla Firefox).
* **Bouclier de confidentialité & sécurité** – désactivation réversible de la télémétrie Windows et des mouchards publicitaires, contrôle de l'antivirus Windows Defender avec analyse rapide 1-clic, révélation immédiate des clés de sécurité Wi-Fi enregistrées, audit approfondi des fichiers sensibles (`.ssh`, `.aws`, `.env`) et extraction de la clé de licence Windows d'origine inscrite dans le BIOS.
* **Optimiseur réseau & benchmark DNS** – protocole exclusif de réinitialisation réseau en 7 étapes (TCP/IP, Winsock, DNS, ARP), scanner visuel des réseaux Wi-Fi environnants et des périphériques LAN, moniteur des flux et connexions TCP actives, et banc d'essai comparatif de vitesse DNS en temps réel (Cloudflare, Google, Quad9, OpenDNS).
* **Mode gaming & latence minimale** – optimiseur dédié aux joueurs pour minimiser la latence système : activation en un clic du plan d'alimentation *Performances ultimes*, réduction de l'algorithme TCP Nagle, accélération matérielle graphique (HAGS) et priorisation Game Mode.
* **Terminal latéral intégré & thèmes néon** – console latérale rétractable (240px) avec affichage en direct et en continu des flux PowerShell et CMD, recherche instantanée parmi plus de 50 outils, gestionnaire de favoris locaux avec persistance, et bascule instantanée entre 5 thèmes visuels dynamiques (*Aero Glass*, *Cyberpunk Neon*, *Matrix Phosphor*, *Nordic Frost*, *Obsidian Luxury*).

**Également inclus** : gestionnaire d'installation unifié d'applications populaires via Winget, bascule rapide du menu contextuel Windows 11 vers le style classique Windows 10, raccourci direct vers le dossier Tous les réglages (*God Mode*), auto-élévation UAC avec conservation des privilèges et exécution asynchrone non-bloquante pour une fluidité absolue.

---

## Gratuit & 100% open source

L'application et l'intégralité des modules décrits ci-dessus sont entièrement gratuits et sous licence open source **MIT**.

* 🚀 Aucun compte requis, aucune inscription.
* 🛡️ Aucun composant payant masqué ni publicité.
* 📦 Architecture 100% autonome sans dépendance DLL externe résiduelle.

---

## Installation

### Exécutable autonome (recommandé)
1. Rendez-vous dans la section [Releases](https://github.com/aleex-le-dev/SCRIPT-WINDOWS-ALEEXLEDEV/releases/latest).
2. Téléchargez [`AlexToolkit.exe`](AlexToolkit.exe).
3. Lancez l'exécutable (une invite UAC vous demandera les droits administrateur pour permettre les réparations système).
4. Profitez immédiatement de tous les outils sans rien installer.

### Prérequis
* Windows 10 ou Windows 11 (64 bits).
* Droits d'administrateur recommandés pour les actions système de bas niveau (réparations DISM/SFC, pilotes, BCD, réseau).
* Runtime .NET 8.0 pour Windows Desktop (si vous utilisez la version non autonome ; la version `AlexToolkit.exe` SingleFile intègre tout le nécessaire).

---

## Confidentialité & souveraineté (privacy)

Alex Toolkit fonctionne **100% en local et hors-ligne** :
* Aucune donnée personnelle, identifiant ou diagnostic n'est envoyé vers des serveurs externes.
* Aucun traceur d'utilisation, aucune télémétrie tierce intégrée.
* Vos favoris et réglages d'interface sont stockés uniquement sur votre machine locale dans `%LocalAppData%\AlexToolkit`.
* Vous gardez le contrôle total sur chaque commande exécutée : chaque script est prévisualisable et transparent.

---

## Développement & compilation

Pour cloner et compiler le projet localement avec le SDK .NET 8 :

```powershell
# Cloner le dépôt
git clone https://github.com/aleex-le-dev/SCRIPT-WINDOWS-ALEEXLEDEV.git
cd SCRIPT-WINDOWS-ALEEXLEDEV

# Compiler l'exécutable autonome en 1 clic
.\build.bat
```

### Commandes manuelles (PowerShell / CLI)

```powershell
# Compilation Release Single-File optimisée (ReadyToRun AOT)
dotnet publish ScriptsWindows/ScriptsWindows/ScriptsWindows.csproj -c Release -r win-x64 --no-self-contained -p:PublishSingleFile=true

# Nettoyer les caches temporaires, logs et builds résiduels
.\clean.bat
```

### Architecture du code

```text
SCRIPT-WINDOWS-ALEEXLEDEV/
├── AlexToolkit.exe                        ← Exécutable autonome final généré
├── build.bat                              ← Script de compilation automatique en 1 clic
├── clean.bat                              ← Script de purge des caches et logs
│
└── ScriptsWindows/ScriptsWindows/
    ├── App.xaml / App.xaml.cs             ← Point d'entrée, UAC, styles et injection
    ├── MainWindow.xaml / .cs              ← Vue maîtresse (Sidebar, Topbar et Terminal latéral)
    ├── Core/                              ← Moteurs d'exécution asynchrones, thèmes, registre d'outils
    ├── Services/                          ← Logique métier C# (Diagnostic, Réparation, Boot, Nettoyage...)
    ├── Views/                             ← Dialogues spécialisés, cartes interactives et pages
    └── Assets/Styles/                     ← Feuilles de style XAML (lueurs, glassmorphism, contrôles)
```

---

## Idées, questions et signalements

* 💡 **Idées & suggestions** : [GitHub Discussions → Ideas](https://github.com/aleex-le-dev/SCRIPT-WINDOWS-ALEEXLEDEV/discussions) – votez et proposez les prochains scripts ou outils à intégrer.
* 💬 **Questions & entraide** : [GitHub Discussions → Q&A](https://github.com/aleex-le-dev/SCRIPT-WINDOWS-ALEEXLEDEV/discussions).
* 🐛 **Signaler un bug** : [Ouvrir un ticket d'incident](https://github.com/aleex-le-dev/SCRIPT-WINDOWS-ALEEXLEDEV/issues/new).
* 📜 **Historique des versions** : [Consulter le journal des modifications](CHANGELOG.md).
