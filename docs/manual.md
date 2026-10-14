# 📖 Guide d'Utilisation — Alex Toolkit (v4.x)

Bienvenue dans le manuel d'utilisation d'**Alex Toolkit**, la suite logicielle tout-en-un de diagnostic, réparation, nettoyage, optimisation et sécurité pour Windows 10 et 11.

---

## 🚀 Prise en main rapide

1. **Lancement de l'application** :  
   Double-cliquez sur `AlexToolkit.exe`. Une demande de confirmation de contrôle de compte d'utilisateur (UAC) s'affiche : acceptez-la pour permettre l'exécution des fonctions système avancées (réparation DISM/SFC, pilotes, BCD, etc.).
2. **Navigation** :  
   - La **Sidebar gauche** regroupe les 13 catégories d'outils ainsi que vos **Favoris**.
   - Le **Bandeau supérieur** propose la recherche universelle instantanée et le sélecteur de thèmes néon.
   - La **Console latérale droite** s'ouvre automatiquement dès qu'une commande ou un script est exécuté pour afficher en direct le flux de sortie en UTF-8.

---

## 🧰 Guide des Catégories

### 1. 🔍 Diagnostic Matériel & Système
- **Centre de Diagnostic Unifié** : analyse instantanée de la carte mère, du BIOS, du processeur, des barrettes de RAM, de la batterie et des disques S.M.A.R.T.
- **Audit Windows 11 24H2 & IA Copilot+** : vérifie la compatibilité de votre matériel avec les exigences de la mise à jour 24H2 (TPM 2.0, SecureBoot, processeur NPU).
- **DirectX Diagnostic (DxDiag)** : lance l'utilitaire d'inspection graphique et sonore de Windows.

### 2. 🛠️ Intégrité & Réparation
- **Centre d'Intégrité (DISM & SFC)** : scanne l'image système Windows et répare les fichiers corrompus en streaming direct dans la console.
- **CHKDSK Interactif** : analyse et corrige les anomalies du système de fichiers NTFS.
- **Gestionnaire des Services & Spouleur** : débloque les imprimantes en cas de file d'attente bloquée et redémarre les services essentiels.
- **Sauvegarde des Pilotes** : exporte en 1 clic l'intégralité des pilotes tiers installés vers un dossier de sauvegarde.

### 3. ⚡ Démarrage & Récupération
- **Bascule Mode Sans Échec** : redémarrez rapidement en mode sans échec (minimal ou avec prise en charge réseau).
- **Accès Direct BIOS/UEFI** : redémarre la machine directement sur l'écran du firmware sans frapper les touches F2/Suppr au démarrage.
- **Environnement WinRE & BCD** : réparez les erreurs du chargeur de démarrage Windows.

### 4. 🧹 Nettoyage & Optimisation
- **Nettoyage Unifié** : libère de l'espace disque en purgeant les fichiers temporaires, la corbeille, le cache Windows Update et les résidus des navigateurs.

### 5. 🛡️ Confidentialité & Sécurité
- **Bouclier Anti-Télémétrie** : désactive de manière réversible les mouchards publicitaires et la télémétrie Windows.
- **Affichage des Clés Wi-Fi** : retrouve en clair les mots de passe des réseaux Wi-Fi enregistrés sur votre machine.
- **Audit des Fichiers Sensibles** : recherche les fichiers de clés privées (`.ssh`, `.aws`, `.env`) exposés sur vos disques.

### 6. 🌐 Réseau & DNS
- **Reset Réseau en 7 Étapes** : réinitialise la pile TCP/IP, Winsock, le cache DNS et les tables ARP en cas de panne réseau.
- **Benchmark DNS** : compare en direct la latence des principaux résolveurs (Cloudflare, Google, Quad9) pour choisir le plus rapide.

---

## 🎨 Personnalisation & Thèmes

Cliquez sur le bouton **Thème** en haut à droite pour parcourir les 5 univers visuels :
- **Aero Glass** : verre fumé translucide, reflets givrés et halos indigo.
- **Cyberpunk Neon** : contrastes sombres et halos néon rose rubis.
- **Matrix Phosphor** : ambiance terminal hacker vert émeraude.
- **Nordic Frost** : transparence arctique et accents cyan glacier.
- **Obsidian Luxury** : noir OLED profond et touches dorées.

---

## ❓ Support & Contribution

Pour signaler une anomalie ou proposer un nouvel outil, visitez l'onglet [GitHub Issues](https://github.com/aleex-le-dev/SCRIPT-WINDOWS-ALEEXLEDEV/issues) ou [GitHub Discussions](https://github.com/aleex-le-dev/SCRIPT-WINDOWS-ALEEXLEDEV/discussions).
